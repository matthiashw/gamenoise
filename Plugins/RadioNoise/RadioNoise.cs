using System;
using Interfaces;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Wma;
using Un4seen.Bass.AddOn.Tags;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace RadioNoise
{
    public class RadioNoise : IPlugin, IObserver
    {
        //Declarations of all our internal plugin variables
        private const string MyName = "RadioNoise";
        private const string MyDescription = "Manages and plays radio streams";
        private const string MyAuthor = "gamenoise Team";
        private const string MyVersion = "0.1.0";

        //Internet Stream consts
/*
        private const string userAgent = "Gamenoise - your ingame musicplayer";
*/

        [FixedAddressValueType]
        public IntPtr MyUserAgentPtr;

        bool _firstUseOfBass;
        private SYNCPROC _mySync;
        private TAG_INFO _tagInfo;
        IPluginHost _myHost;
        private int _stream;
        private String _url;
        private bool _isWma;
        private bool _buffered;
        MyView _myMainInterface;
        MySettings _mySettingsInterface;


        public RadioNoise()
        {
            _stream = 0;
        }

        #region PluginPropertys
        /// <summary>
        /// Description of the Plugin's purpose
        /// </summary>
        public string Description
        {
            get { return MyDescription; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author
        {
            get { return MyAuthor; }

        }

        /// <summary>
        /// Host of the plugin.
        /// </summary>
        public IPluginHost Host
        {
            get { return _myHost; }
            set { _myHost = value; }
        }

        public string Name
        {
            get { return MyName; }
        }

        public System.Windows.Controls.UserControl MainInterface
        {
            get { return _myMainInterface; }
        }

        public string Version
        {
            get { return MyVersion; }
        }

        #endregion

        public void Initialize()
        {
            IPlugin webservicePlugin = _myHost.getPluginmanager().GetPluginByName("Webservice").Instance;

            Webservice.IWebserviceClient webserviceClient = ((Webservice.IInterPlugin)webservicePlugin).GetWebserviceClient();

            _myMainInterface = new MyView(_myHost, this, webserviceClient, webservicePlugin);
            _mySettingsInterface = new MySettings();
            _firstUseOfBass = true;

            // init settings
            
        }

        private void ConfigBassForRadioStreams()
        {
            Bass.BASS_SetConfigPtr(BASSConfig.BASS_CONFIG_NET_AGENT, MyUserAgentPtr);
            // so that we can display the buffering%
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_WMA_PREBUF, 0);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PREBUF, 0); 

            //Enable pls support
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1);

            //Add Observer
            _myHost.getBasswrapper().AddObserver(this);

            _firstUseOfBass = false;
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }

        public bool isAbleToPlayback(string filename)
        {
            return filename.StartsWith("http://");
        }

        private void DoBuffer(int stream, bool isWMA)
        {
            if (_buffered) return;

            if (!isWMA)
            {
                // display buffering for MP3, OGG...
       
                long len = Bass.BASS_StreamGetFilePosition(stream, BASSStreamFilePosition.BASS_FILEPOS_END);
                if (len == -1)
                    return; // typical for WMA streams
                // percentage of buffer filled
                float progress = (
                    Bass.BASS_StreamGetFilePosition(stream, BASSStreamFilePosition.BASS_FILEPOS_DOWNLOAD) -
                    Bass.BASS_StreamGetFilePosition(stream, BASSStreamFilePosition.BASS_FILEPOS_CURRENT)
                    ) * 100f / len;

                _myHost.getPlaylist().getCurrentSong().album = String.Format("Buffering... {0}%", progress);

                if (progress >= 100f)
                {
                    _buffered = true ; // over 75% full, enough
                    _myHost.getPlaylist().getCurrentSong().album = "";
                    Bass.BASS_ChannelPlay(_myHost.getBasswrapper().getStreamId(), false);
                }
            }
            else
            {
                // display buffering for WMA...
                long len = Bass.BASS_StreamGetFilePosition(stream, BASSStreamFilePosition.BASS_FILEPOS_WMA_BUFFER);
                if (len == -1L)
                    return;
                // percentage of buffer filled
                if (len >= 100L)
                {
                    _buffered = true; // over 75% full, enough
                    _myHost.getPlaylist().getCurrentSong().album = "";
                    Bass.BASS_ChannelPlay(_myHost.getBasswrapper().getStreamId(), false);
                }

                _myHost.getPlaylist().getCurrentSong().album = String.Format("Buffering... {0}%", len);
           }
        }

        #region IPlugin Member


        public int getBassStream(string filename)
        {
            if (_firstUseOfBass)
                ConfigBassForRadioStreams();

            _isWma = false;

            //BassNet.Registration("mail@example.com", "key");

            _stream = Bass.BASS_StreamCreateURL(filename, 0, BASSFlag.BASS_STREAM_STATUS | BASSFlag.BASS_STREAM_DECODE | 
                BASSFlag.BASS_SAMPLE_FLOAT, null, IntPtr.Zero);

            if (_stream == 0)
            {
                // try WMA streams...
                _stream = BassWma.BASS_WMA_StreamCreateFile(filename, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                if (_stream != 0)
                    _isWma = true;
                else
                    return 0;
            }

            _tagInfo = new TAG_INFO(filename);

            _url = filename;

            _myHost.getDispatcher().Invoke(System.Windows.Threading.DispatcherPriority.DataBind, new Action(
                                                                                                     () =>
                                                                                                     DoBuffer(_stream,
                                                                                                              _isWma)));

            _mySync = new SYNCPROC(MetaSync);
            Bass.BASS_ChannelSetSync(_stream, BASSSync.BASS_SYNC_META, 0, _mySync, IntPtr.Zero);
            Bass.BASS_ChannelSetSync(_stream, BASSSync.BASS_SYNC_WMA_CHANGE, 0, _mySync, IntPtr.Zero);

            _myHost.getBackgroundWorker().ProgressChanged += RadioNoiseProgressChanged;
            _buffered = false;

            return _stream;
        }

        void RadioNoiseProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DoBuffer(_stream, _isWma);
        }

        #endregion


        public void Update(object subject)
        {
            if (Bass.BASS_ChannelIsActive(_stream) == BASSActive.BASS_ACTIVE_PLAYING)
            {

                if (BassTags.BASS_TAG_GetFromURL(_stream, _tagInfo))
                {
                }
            }

            if (subject is IBassWrapper)
            {
                //System.Console.WriteLine("Melde mich zum Dienst!");
            }

        }


        private void MetaSync(int handle, int channel, int data, IntPtr user)
        {
            // BASS_SYNC_META is triggered on meta changes of SHOUTcast streams
            if (_tagInfo.UpdateFromMETA(Bass.BASS_ChannelGetTags(channel, BASSTag.BASS_TAG_META), false))
            {
                _myHost.getDispatcher().Invoke(System.Windows.Threading.DispatcherPriority.DataBind, new Action(UpdateTagDisplay));
            }
        }


        public delegate void UpdateTagDelegate();
        private void UpdateTagDisplay()
        {

            _myHost.getPlaylist().getCurrentSong().artist = _tagInfo.artist;
            _myHost.getPlaylist().getCurrentSong().title = _tagInfo.title;

            String rest = "";
            if (_tagInfo.album != "") rest += _tagInfo.album;
            if (_tagInfo.genre != "")
            {
                if (rest != "")
                    rest += " | ";
                rest += _tagInfo.genre;
            }
            if (_tagInfo.year != "")
            {
                if (rest != "")
                    rest += " | ";
                rest += _tagInfo.year;
            }
            if (_tagInfo.comment != "")
            {
                if (rest != "")
                    rest += " | ";
                rest += _tagInfo.comment;
            }
            _myHost.getPlaylist().getCurrentSong().album = rest;
        }

        public System.Collections.ArrayList getDependencies()
        {
            System.Collections.ArrayList myDependencies = new System.Collections.ArrayList {"Webservice"};
            return myDependencies;
        }

        public System.Windows.Controls.UserControl SettingsInterface
        {
            get { return _mySettingsInterface; }
        }
    }
}
