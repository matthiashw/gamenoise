/* 
 * author: AK
 * 
 * created: 07.11.2008
 * 
 * modification history
 * --------------------
 * 
 * MHO (21.11.08):
 * getFFTData, fftBuffer  added
 * 
 * AK (21.11.08):
 * Stop bugfix
 * 
 * MHO (26.11.08):
 * re-set gain when calling Play()
 * 
 * MHI (27.11.08):
 * added getChangeEQ, returns if something in the EQ has changed
 * 
 * AK (30.11.08):
 * added switch function for EQ + BassWrapperException
 * 
 * AK (01.12.08):
 * fixed Pausebug
 * 
 * MHI (05.12.08):
 * EQ is now off from the start
 * 
 * PL (08.06.09)
 * Code Cleaning
 */

using System;
using Observer;
using PluginManager;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;

using System.IO;


namespace PlayControl
{
    
    /// <summary>
    /// Exception Class for Errors in BassWrapper
    /// </summary>
    public class BassWrapperException :  Exception
    {
        /// <summary>
        /// BassError Number
        /// </summary>
        private readonly BASSError _errorNumber;
        /// <summary>
        /// Message Text
        /// </summary>
        private readonly string _message;

        /// <summary>
        /// BassWrapperException 
        /// </summary>
        /// <param name="e">Error Number</param>
        /// <param name="m">Message</param>
        public BassWrapperException(BASSError e,string m)
        {
            _errorNumber = e;
            _message = m;
        }

        /// <summary>
        /// Get Method for Error Number
        /// </summary>
        /// <returns>Error Number</returns>
        public BASSError GetErrorNumber()
        {
            return _errorNumber;
        }

        /// <summary>
        /// Get Method for Error Message
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {

            return _message;
        }
    }

 

    /// <summary>
    /// Bass Wrapper Class to wrap the Bass Library
    /// </summary>
    public class BassWrapper : Subject, Interfaces.IBassWrapperGui, Interfaces.IBassWrapperOrganisation, Interfaces.IBassWrapper
    {
        
        /// <summary>
        /// Temporary Save Balance
        /// </summary>
        public float TmpBalance;

        public DSP_Gain Gain
        {
            get { return _gain; }
        }

        /// <summary>
        /// Get The Length of a Song 
        /// </summary>
        /// <param name="filepath">Song</param>
        /// <returns>Length in secs</returns>
        public static double GetTimeLength(string filepath)
        {
 
            int chan = Bass.BASS_StreamCreateFile(filepath, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            if (Bass.BASS_ErrorGetCode() == BASSError.BASS_OK)
            {

                double len = Bass.BASS_ChannelBytes2Seconds(chan, Bass.BASS_ChannelGetLength(chan));
                Bass.BASS_StreamFree(chan);
              
                return len;
                
            }
            return 0.0f;
        }

        /// <summary>
        /// The PlayControler Back-Reference for Next-Song
        /// </summary>
       public Interfaces.IPlayControler aPlayControler{get;set;}
       public Interfaces.IPluginManager aPluginManager { get; set; }

        /// <summary>
        /// Pause Variable
        /// </summary>
        private bool _paused;

        /// <summary>
        /// Stop Variable
        /// </summary>
         private bool _stopped;

        /// <summary>
        /// Volume 0-mute 1-full
        /// </summary>
        private float _volume;

        private bool _changeEq;

        /// <summary>
        /// Equalizer State
        /// true = EQ On
        /// false =EQ Off
        /// </summary>
        private bool _toggleEq;

        /// <summary>
        /// ID of Mixed-Bass stream
        /// </summary>
        private int _streamId;

        /// <summary>
        /// ID of Raw-Bass stream
        /// </summary>
        private int _rawstream;

        /// <summary>
        /// Equalizer Effect Handle
        /// </summary>
        private int _fxEq;

        /// <summary>
        /// Compressor Effect Handle
        /// </summary>
        private int _fxComp;

        /// <summary>
        /// DSP_Gain Effect Handle
        /// </summary>
        private DSP_Gain _gain;
        
        /// <summary>
        /// Equalizer Entry Setting (depratched)
        /// </summary>
        private const float Eqstart=0f;

        /// <summary>
        /// EQ Gains 
        /// </summary>
        private readonly float[] _equalizerTmp=  { 0,0,0,0,0,0,0,0,0,0};

        /// <summary>
        /// Matrix for Soundmixing in BassMix
        /// </summary>
        private readonly float[,] _matrix ={ // stereo to quad _matrix...
                     {1, 0}, // left front out = left in
                     {0, 1}, // right front out = right in
                     {1, 1}, // 
                     {1, 1}, // 
                     {1, 0}, // left rear out = left in
                     {0, 1} // right rear out = right in
                    };

        /// <summary>
        /// Notify for Elapsed Time
        /// </summary>
        private System.Windows.Threading.DispatcherTimer _etimeClock;

        /// <summary>
        /// Buffer for visualization
        /// </summary>
        private float[] _fftBuffer;

        /// <summary>
        /// This Method Sets the Equalizer Settings to a new Stream.
        /// </summary>
        private void EqualizerInit()
        {
            //############## BASSFX - Equalizer #######################
            // 60Hz, 170, 310, 600, 1K, 3K, 6K,12K,14K,16KHz
            // 10-band EQ
            // Set peaking equalizer effect with no bands
            bool loadbassfx = BassFx.LoadMe();
            var eq = new BASS_BFX_PEAKEQ();
            _fxEq = Bass.BASS_ChannelSetFX(_rawstream, BASSFXType.BASS_FX_BFX_PEAKEQ, 5);
           // _fxComp = Bass.BASS_ChannelSetFX(_rawstream, BASSFXType.BASS_FX_BFX_DAMP, 0);
            /*
            BASS_BFX_DAMP comp = new BASS_BFX_DAMP();
            if (Bass.BASS_FXGetParameters(_fxComp, comp))
            {
                //comp.Preset_Soft();
                /*
                comp.fTarget=0.92f;
                comp.fQuiet=0.02f;
                comp.fRate=0.01f;
                comp.fGain=1.0f;
                comp.fDelay = 0.5f;
                   
                comp.fTarget = 1f;
                comp.fQuiet =0f;
                comp.fRate = 0.02f;
                comp.fGain = 1f;
                comp.fDelay = 0.2f;
                    
                Bass.BASS_FXSetParameters(_fxComp, comp);
            }
            */
            _fxComp = Bass.BASS_ChannelSetFX(_rawstream, BASSFXType.BASS_FX_BFX_COMPRESSOR2, 0);
            //Add Compressor Effect
            var comp = new BASS_BFX_COMPRESSOR2();
            if (Bass.BASS_FXGetParameters(_fxComp, comp))
            {
                comp.fGain = 1f;
                comp.fAttack = 10f;
                comp.fRelease = 200f;
                comp.fThreshold = -10f;
                comp.fRatio = 3f;
                Bass.BASS_FXSetParameters(_fxComp, comp);
            }


            if (!loadbassfx)
            {
                //MessageBox.Show("Can't find bass_fx.dll");
            }
            // Setup the EQ bands
           
            //eq.fQ = 0f;
            eq.fBandwidth = 1f;
            eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;
           // setEqualizerBand(eq.lBand, 0.0f);
            //Band 1
            eq.lBand = 0;
            eq.fCenter = 60f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
          
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 2
            eq.lBand = 1;
            eq.fCenter = 170f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
           

            //setEqualizerBand(eq.lBand, 0.0f);
            //Band 3
            eq.lBand = 2;
            eq.fCenter = 310f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
       
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 4
            eq.lBand = 3;
            eq.fCenter = 600f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
           
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 5
            eq.lBand = 4;
            eq.fCenter = 1000f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
           
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 6
            eq.lBand = 5;
            eq.fCenter = 3000f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
           
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 7
            eq.lBand = 6;
            eq.fCenter = 6000f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
           
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 8
            eq.lBand = 7;
            eq.fCenter = 12000f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
           
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 9
            eq.lBand = 8;
            eq.fCenter = 14000f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
          
            //setEqualizerBand(eq.lBand, 0.0f);

            //Band 10
            eq.lBand = 9;
            eq.fCenter = 16000f;
            Bass.BASS_FXSetParameters(_fxEq, eq);
           
            //setEqualizerBand(eq.lBand, 0.0f);
            
        }

      
       

        /// <summary>
        /// Constructor for the Bass Wrapper Class
        /// </summary>
        /// <param name="handle">Window Handle</param>
        public BassWrapper(IntPtr handle)
        {
            //############### BASS - REGISTRATION #####################
            //BassNet.Registration("mail@example.com", "key");
            //############### BASS - INITIATION #######################

            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_CPSPEAKERS, handle, null);
            Console.WriteLine(Bass.BASS_PluginLoad("basswma.dll"));


            //############### BASS - SYNCPROC INITIATION (End of Song Procedure)
           
            TmpBalance = 0f;
            _volume = 100f;

            _fftBuffer = new float[2048];

            _etimeClock = new System.Windows.Threading.DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1, 0)};
            _etimeClock.Tick += EtimeClockTick;
            _stopped = true;
            _changeEq = false;
            _toggleEq = false;

            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK)
            {
                var e = new BassWrapperException(Bass.BASS_ErrorGetCode(),"Error while initialising Sound. Error Code: "+ Bass.BASS_ErrorGetCode())
                            {Source = "BassWrapper"};
                throw e;
                
            }

        }

        /// <summary>
        /// Destruktor to Free Bass resources
        /// </summary>
         ~BassWrapper()
        {
            try
            {

                Bass.BASS_Free();
            }
            catch(DllNotFoundException)
            {

            }
        }

        /// <summary>
        /// The Play Method plays a song by his path with the Bass Lib.
        /// </summary>
        /// <param name="songPath">Song Path to Play</param>
        /// <param name="validSong">returns if song is playable by plugin or bass</param>
        public void play(string songPath, ref bool validSong)
        {            
            Bass.BASS_StreamFree(_streamId);
            Bass.BASS_StreamFree(_rawstream);

            _paused = false;
            _stopped = false;


            //Check if a plugin can playback the file
            bool pluginPlayback = false;
            foreach (AvailablePlugin plugin in aPluginManager.AvailablePlugins)
            {
                if (plugin.Instance.isAbleToPlayback(songPath))
                {
                    _rawstream = plugin.Instance.getBassStream(songPath);
                    pluginPlayback = true;
                    validSong = true;
                    break;
                }
            }

            //If there were no plugin for playback, use the normal playback function
            if (pluginPlayback == false)
            {
                validSong = true;
                if (!File.Exists(songPath))
                    validSong = false;

                _rawstream = Bass.BASS_StreamCreateFile(songPath, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }


            _etimeClock = new System.Windows.Threading.DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1, 0)};
            _etimeClock.Tick += EtimeClockTick;
            _etimeClock.IsEnabled = true;
           

            BASS_CHANNELINFO info= Bass.BASS_ChannelGetInfo(_rawstream); // get source info

            _streamId = BassMix.BASS_Mixer_StreamCreate(info.freq, 6, BASSFlag.BASS_DEFAULT); // create 6 channel mixer with same rate
            BassMix.BASS_Mixer_StreamAddChannel(_streamId, _rawstream, BASSFlag.BASS_MIXER_MATRIX | BASSFlag.BASS_MIXER_DOWNMIX); // add source with _matrix mixing
            
            
            BassMix.BASS_Mixer_ChannelSetMatrix(_rawstream, _matrix); // apply the _matrix
            
            EqualizerInit(); // Initiate Eq
            _gain = new DSP_Gain(_rawstream, 10) {Gain_dBV = -5f};


            if (_toggleEq)
                setEqualizer(_equalizerTmp); //set last Values
            else
            {
                settoggleEQ(false);
            }
            
            setVolume(_volume); //set _gain

            if (!pluginPlayback)
                Bass.BASS_ChannelPlay(_streamId, false);
  
                       
            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK && Bass.BASS_ErrorGetCode() != BASSError.BASS_ERROR_HANDLE)
            {
                var e = new BassWrapperException(Bass.BASS_ErrorGetCode(),"Error while trying to Play " + songPath + ". Error Code: " + Bass.BASS_ErrorGetCode())
                            {Source = "BassWrapper"};
                validSong = false;
                throw e;
            }
            
        }

        /// <summary>
        /// Pause the Playing stream or if it is paused Play it aggain
        /// </summary>
        public void pause()
        {
            if (_paused)
            {
                float vol=_volume;
                setVolume(0);
                Bass.BASS_ChannelPlay(_streamId, false);
               
                _etimeClock = new System.Windows.Threading.DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 1, 0)};
                _etimeClock.Tick += EtimeClockTick;
                _etimeClock.IsEnabled = true;
                
               
                setVolume(vol);
                _paused = false;
            } 
            else {
                
                _etimeClock.IsEnabled = false;
                
                Bass.BASS_ChannelPause(_streamId);
               
                
                _paused = true;
            }
            Notify();
            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK && Bass.BASS_ErrorGetCode() != BASSError.BASS_ERROR_HANDLE)
            {
                var e = new BassWrapperException(Bass.BASS_ErrorGetCode(),"Error in Bass.dll; Error Code: " + Bass.BASS_ErrorGetCode())
                            {Source = "BassWrapper"};
                throw e;

            }
        }

        /// <summary>
        /// Stop playing stream
        /// </summary>
        public void stop()
        {
            _etimeClock.IsEnabled = false;
            _stopped = true;
            _paused = false;
            Bass.BASS_ChannelStop(_streamId);
           
            Bass.BASS_StreamFree(_streamId);
            Bass.BASS_StreamFree(_rawstream);
           
            Notify();
        }

      

        /// <summary>
        /// Method to Set Volume of Master Output
        /// </summary>
        /// <param name="gain">
        /// Volume:
        /// 1 = max
        /// 0 = min
        /// </param>
        public void setVolume(float gain) 
        {
            _volume = gain;
            Bass.BASS_ChannelSetAttribute(_streamId, BASSAttribute.BASS_ATTRIB_VOL, gain/100);
          
            Notify();
            
        }

        /// <summary>
        /// Volume Get-Method
        /// </summary>
        /// <returns>Volume  1 = max 0 = min</returns>
        public float getVolume()
        {
            return _volume;
        }

        /// <summary>
        /// Set Equalizer Settings
        /// </summary>
        /// <param name="frequencyList">10 Band Float list</param>
        public void setEqualizer(float[] frequencyList)
        {
            if (frequencyList == null)
                return;

            for (int i = 0; i < 10; i++)
            {
                
                _equalizerTmp[i] = frequencyList[i];
                if (!_stopped &&_toggleEq)
                {
                    var eq = new BASS_BFX_PEAKEQ {lBand = i};
                    // get values of the selected band
                    Bass.BASS_FXGetParameters(_fxEq, eq);
                    eq.fGain = frequencyList[i]+Eqstart;
                    Bass.BASS_FXSetParameters(_fxEq, eq);
               
                }
                
            }
            _changeEq = true;
            Notify();
            _changeEq = false;

            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK && Bass.BASS_ErrorGetCode() != BASSError.BASS_ERROR_HANDLE)
            {
                var e = new BassWrapperException(Bass.BASS_ErrorGetCode(),"Error in Bass.dll; Error Code: " + Bass.BASS_ErrorGetCode())
                            {Source = "BassWrapper"};
                throw e;

            }
            
        }

        /// <summary>
        /// Sets Single Equalizer Band
        /// </summary>
        /// <param name="band">Band to set</param>
        /// <param name="freq">Gain to set</param>
        public void setEqualizerBand(int band, float freq)
        {
            _equalizerTmp[band] = freq;
            if (!_stopped && _toggleEq)
            {
                var eq = new BASS_BFX_PEAKEQ {lBand = band};
                // get values of the selected band
                Bass.BASS_FXGetParameters(_fxEq, eq);
                eq.fGain = freq+Eqstart;
                Bass.BASS_FXSetParameters(_fxEq, eq);
                
            }
            _changeEq = true;
            Notify();
            _changeEq = false;

            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK && Bass.BASS_ErrorGetCode() != BASSError.BASS_ERROR_HANDLE)
            {
                var e = new BassWrapperException(Bass.BASS_ErrorGetCode(),"Error in Bass.dll; Error Code: " + Bass.BASS_ErrorGetCode())
                            {Source = "BassWrapper"};
                throw e;

            }

        }


        /// <summary>
        /// Get Method for recent Equalizer Settings
        /// </summary>
        /// <returns>Eq-Settings float Array</returns>
        public float[] getEqualizer()
        {

            return _equalizerTmp;
        }

        /// <summary>
        /// Set A new Postition to Play the stream
        /// </summary>
        /// <param name="seconds">Stream position</param>
        /// <returns>Succes-variable</returns>
        public bool setPlayPosition(double seconds)
        {
            if (seconds > getTotalTime())
            {
                return false;
            }
            if ( seconds== getTotalTime()){
                if (getTotalTime() > 1)
                {
                    seconds--;
                }
                else
                {
                    return false;
                }
            }


            Bass.BASS_ChannelSetPosition(_rawstream, seconds);
              
                _etimeClock = new System.Windows.Threading.DispatcherTimer();
                Notify();

                if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK && Bass.BASS_ErrorGetCode() != BASSError.BASS_ERROR_HANDLE)
                {
                    return false;

                }
                return true;
          

        }

        /// <summary>
        /// Method to Read Music Tags from Files
        /// </summary>
        /// <param name="songPath">Filepath</param>
        /// <returns>Tags 
        /// tags[0]=album
        /// tags[1]=artist
        /// tags[2]=title
        /// 
        /// </returns>
        public static string[] GetTags(String songPath)
        {
            var tags = new string[3];
            var tagInfo = new TAG_INFO();

            int stream = Bass.BASS_StreamCreateFile(songPath, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            BassTags.BASS_TAG_GetFromFile(stream, tagInfo);

            Bass.BASS_StreamFree(stream);
            // and display what we get
            tags[0] = tagInfo.album;
            tags[1] = tagInfo.artist.Trim();
            tags[2] = tagInfo.title.Trim();
            /*
                tags[3] = tagInfo.comment;
                tags[4] = tagInfo.genre;
                tags[5] = tagInfo.year;
                tags[6] = tagInfo.track;
                 * */


            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK && Bass.BASS_ErrorGetCode() != BASSError.BASS_ERROR_HANDLE)
            {
                var e = new BassWrapperException(Bass.BASS_ErrorGetCode(), "Tag Error") {Source = "BassWrapperTags"};
                throw e;
            }
            return tags;
        }

        /// <summary>
        /// Get Method for elapsed Time of Stream
        /// </summary>
        /// <returns>double Time (in secs)</returns>
        public double getElapsedTime()
        {
            if (_stopped)
                return 0;
                return Bass.BASS_ChannelBytes2Seconds(_rawstream, Bass.BASS_ChannelGetPosition(_rawstream));
        }

        /// <summary>
        /// Get Method for total Time of Stream
        /// </summary>
        /// <returns>double Time (in secs)</returns>
        public double getTotalTime()
        {
            if (_stopped)
                return 0.0001;
                return Bass.BASS_ChannelBytes2Seconds(_rawstream, Bass.BASS_ChannelGetLength(_rawstream));
        }

        /// <summary>
        /// Get Method for check if Stream is paused
        /// </summary>
        /// <returns>Pause variable</returns>
        public bool getPaused()
        {
            return _paused;
        }


        private void EtimeClockTick(object sender, EventArgs e)
        {
            if (getElapsedTime() == getTotalTime())
            {
                try
                {
                    aPlayControler.NextSong();
                }
                catch (NullReferenceException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }
            }
            Notify();
        }

        /// <summary>
        /// Catch the FFT-Data of the current steam
        /// </summary>
        /// <returns>Returns FFT-Data</returns>
        public float[] getFFTData()
        {
            if (_stopped)
            
                _fftBuffer = new float[2048];
            else
                Bass.BASS_ChannelGetData(_streamId, _fftBuffer, (int)BASSData.BASS_DATA_FFT4096);
            return _fftBuffer;
        }

        /// <summary>
        /// Returns if something in the EQ has changed
        /// </summary>
        /// <returns>Has Change?</returns>
        public bool getChangeEQ()
        {
            return _changeEq;
        }

        /// <summary>
        /// Switch Equalizer On/Off
        /// </summary>
        /// <param name="state">
        /// true = On 
        /// false = Off
        /// </param>
        public void settoggleEQ(bool state)
        {
            _toggleEq = state;
            if (state)
            {

                for (int i = 0; i < 10; i++)
                {

                    var eq = new BASS_BFX_PEAKEQ {lBand = i};
                    // get values of the selected band
                    Bass.BASS_FXGetParameters(_fxEq, eq);
                    eq.fGain = _equalizerTmp[i]+Eqstart;
                    Bass.BASS_FXSetParameters(_fxEq, eq);
                    
                }

            }
            else
            {

                for (int i = 0; i < 10; i++)
                {

                    var eq = new BASS_BFX_PEAKEQ {lBand = i};
                    // get values of the selected band
                    Bass.BASS_FXGetParameters(_fxEq, eq);
                    eq.fGain = Eqstart;
                    Bass.BASS_FXSetParameters(_fxEq, eq);
                  
                }


            }
            Notify();

        }

        /// <summary>
        /// Get Stop Variable
        /// </summary>
        /// <returns>Stop-Variable</returns>
        public bool getstopped()
        {
            return _stopped;
        }


        /// <summary>
        /// Get EQ-state
        /// </summary>
        /// <returns>true = On , false = Off</returns>
        public bool gettoggleEQ()
        {
            return _toggleEq;
        }

        /// <summary>
        /// Get The Balace
        /// </summary>
        /// <returns>-1 left / +1 right</returns>
        public float getBalance()
        {
            
            return TmpBalance;
        }

       

        /// <summary>
        /// Set The Balace
        /// </summary>
        /// <param name="bal">-1 left / +1 right</param>
        public void setBalance(float bal)
        {

            TmpBalance = bal;
            if(bal>0)
            {
            _matrix[0, 0] = 1-bal;
            _matrix[1, 1] = 1;
            _matrix[2, 0] = 1-bal;
            _matrix[2, 1] = 1;
            _matrix[3, 0] = 1-bal;
            _matrix[3, 1] = 1;
            _matrix[4, 0] = 1-bal;
            _matrix[5, 1] = 1;
            //_matrix[5]
            }
            else if (bal == 0)
            {
                _matrix[0,0] = 1;
                _matrix[1,1] = 1;
                _matrix[2, 0] = 1;
                _matrix[2, 1] = 1;
                _matrix[3, 0] = 1;
                _matrix[3, 1] = 1;
                _matrix[4,0] = 1;
                _matrix[5,1] = 1;

            }
            else
            {
                _matrix[0, 0] = 1;
                _matrix[1,1] = 1 + bal;
                _matrix[2, 0] = 1;
                _matrix[2, 1] = 1+bal;
                _matrix[3, 0] = 1;
                _matrix[3, 1] = 1+bal;
                _matrix[4, 0] = 1;
                _matrix[5,1] = 1 + bal;
               
            }

            BassMix.BASS_Mixer_ChannelSetMatrix(_rawstream, _matrix); // apply the _matrix
            if (Bass.BASS_ErrorGetCode() != BASSError.BASS_OK && Bass.BASS_ErrorGetCode() != BASSError.BASS_ERROR_HANDLE)
            {
                var e = new BassWrapperException(Bass.BASS_ErrorGetCode(), "Balance Error") {Source = "BassWrapper"};
                throw e;

            }
            Notify();
        }

        public int getStreamId()
        {
            return _streamId;
        }

    }
}
