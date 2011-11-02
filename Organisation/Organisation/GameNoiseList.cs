/*
 * Author: Pascal Laube
 * Created: 13.11.08
 * 
 * General Information:
 * 
 * 
 * Changes:
 * PL(17.11.08):
 * Took old methods from old Prototype and updated them as specified.
 * 
 * AK,MHO(18.11.08):
 * PlayMode is managed by Gamenoiselist by now
 * CurrentSong Reference get/set for PlayControler added
 * getRandomSong changed to private 
 * getNextSong / getPrevSong now include all Playmodes and set CurrentSong & PreviousSong
 * 
 * PL(22.11.08):
 * AddFolder Funktionalität hinzugefügt.
 * Fileüberprüfung (mp3 etc. hinzugefügt)
 * 
 * PL(04.12.08):
 * Playlist now fills in another Thread.
 * For adding items to listview, we change back to UI Thread
 * 
 * AK(13.12.08)
 * Added Shuffle Mode
 * 
 * PL(16.12.08):
 * LoadingPlaylist Window
 * 
 * AK(22.12.08)
 * Stop-Variable Added for Threads
 * 
 * PL(11.01.09)
 * Exception Class Added
 * 
 * MHI (02.02.09)
 * setNowPlayingPosition now without try catch and it also returns true now
 * 
 * PL (31.03.09)
 * Playlist is loading in different Thread now
 * 
 */

using System.Windows.Threading;
using System.Threading;
using System;
using System.Collections.ObjectModel;
using System.Collections .Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;
using Interfaces;
using Observer;
using PluginManager;

namespace Organisation
{

    public enum PlayModeEnum { PlaymodeNormal = 0, PlaymodeRandom = 1, PlaymodeRepeatSong = 2, PlaymodeRepeatList = 3 };


    /// <summary>
    /// Class for handling Playlist Exceptions
    /// </summary>
    public class PlaylistException : Exception
    {
        private readonly int _errorNumber;
        private readonly string _message;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="e">errorNr</param>
        /// <param name="m">message</param>
        public PlaylistException(int e, string m)
        {
            _errorNumber = e;
            _message = m;
        }
        /// <summary>
        /// GetErrorNumber
        /// </summary>
        /// <returns>_errorNumber</returns>
        public int GetErrorNumber()
        {
            return _errorNumber;
        }

        /// <summary>
        /// GetMessage
        /// </summary>
        /// <returns>message</returns>
        public string GetMessage()
        {

            return _message;
        }
    }
    
    /// <summary>
    /// Class that handles everything related to the playlist
    /// </summary>
    public class GameNoiseList :  Subject, IPlaylist
    {
        private ISong _currentSong;
        private readonly List<int> _shuffledList;
        private PlayModeEnum _playMode;
        private String _extension;
        private ISong _nowLoadingSong;
        private readonly int _showEvery;
        private int _showSongInLoadScreenCount;
        //public int loadingRecursionDepth;

        //The Obeservable Collection in which the songs will be saved
        private readonly ObservableCollection<ISong> _playlist;
        public ObservableCollection<ISong> Playlist { get { return _playlist; } }

        //Thread and Dispatcher Stuff
        private Thread _plthread;
        public Thread getPlthread { get { return _plthread; } }
        public bool stopRequest { get; set; }

        private Thread _xmlThread;
        public Thread getXmlThread { get { return _xmlThread; } }

        //Format for saving Doubles
        readonly NumberFormatInfo _provider;

        /// <summary>
        /// Dispatcher for handling Threadsafety
        /// </summary>
        public Dispatcher DispatcherThread { get; set; }

        private int _nowPlayingPosition;

        public IObserver aLoadingobserver { get; set; }

        public IBassWrapperOrganisation ABassWrapperOrganisation { get; set; }
        
        public PluginManager.PluginManager APluginManager { get; set; }

        public int loadingRecursionDepth { get; set; }
        /*
        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            regenerateList();
        }
        */
        /// <summary>
        /// Construktor, fetch a Song Object.
        /// </summary>
        public GameNoiseList()
        {
            stopRequest = false;
            _playlist = new ObservableCollection<ISong>();  
        //    _playlist.CollectionChanged += new  System.Collections.Specialized.NotifyCollectionChangedEventHandler(OnCollectionChanged);
            _nowPlayingPosition = 0;
            _currentSong = null;
            _shuffledList=new List<int>();
            regenerateList();
            _playMode = 0;
            loadingRecursionDepth = 0;
            _showEvery = 3;
            _showSongInLoadScreenCount = 0;
            _provider = new NumberFormatInfo {NumberDecimalSeparator = ","};
        }


        /// <summary>
        /// Add songs at the end of the current playlist.
        /// Also jumps between UI and PL Thread.
        /// </summary>
        /// <param name="alb">album</param>
        /// <param name="art">artist</param>
        /// <param name="tit">title</param>
        /// <param name="filepath"></param>
        /// <param name="dur">duration</param>
#pragma warning disable 1574
        /// <param name="allPlugins">plugins</param>
#pragma warning restore 1574
        /// <returns>Returns the Position if correct or -1 if fail</returns>
        public int addSong(string alb, string art, string tit, string filepath, double dur)
        {
            int i = -1;
            _extension = Path.GetExtension(filepath);
            ISong song;

            switch (_extension)
            {
                case ".mp3":
                    song = new Mp3Song(alb, art, tit, filepath, dur, ABassWrapperOrganisation);
                    song.AddObserver(getObservers());
                    _playlist.Add(song);
                    i = _playlist.IndexOf(song);
                    break;

                case ".ogg":
                    song = new OggSong(alb, art, tit, filepath, dur, ABassWrapperOrganisation);
                    song.AddObserver(getObservers());
                    _playlist.Add(song);
                    i = _playlist.IndexOf(song);
                    break;

                case ".wav":
                case ".mp1":
                case ".mp2":
                    song = new Mp3Song(alb, art, tit, filepath, dur, ABassWrapperOrganisation);
                    song.AddObserver(getObservers());
                    _playlist.Add(song);
                    i = _playlist.IndexOf(song);
                    break;

                default:
                    //Check if any plugin can playback the file
                    foreach (AvailablePlugin plugin in APluginManager.AvailablePlugins)
                    {
                        if (plugin.Instance.isAbleToPlayback(filepath))
                        {
                            song = new MiscSong(alb, art, tit, filepath, dur, ABassWrapperOrganisation);
                            song.AddObserver(getObservers());
                            _playlist.Add(song);
                            i = _playlist.IndexOf(song);
                            break;
                        }
                    }
                    break;
            }

            return i;

        }

        /// <summary>
        /// Add songs at the end of the current playlist.
        /// Also jumps between UI and PL Thread.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="index"></param>
        /// <returns>Returns the Position if correct or -1 if fail</returns>
        public int addSong(String filepath, int index)
        {
            if (stopRequest)
            {
                loadingRecursionDepth = 0;
                DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                  () =>
                                                                                  aLoadingobserver.
                                                                                      Update(this)));
                return -1;
            }
            int i = -1;
            _extension = Path.GetExtension(filepath);
            ISong song;
            //Console.WriteLine("Song add" + _playlist.Count);
            switch (_extension)
            {
                case ".mp3":
                    song = new Mp3Song(filepath, ABassWrapperOrganisation);
                    song.AddObserver(getObservers());
                    SetLoadingSong(song);
                    if (DispatcherThread == null)
                    {
                        _playlist.Add(song);
                        _showSongInLoadScreenCount++;
                        if (_showSongInLoadScreenCount == _showEvery)
                        {
                           aLoadingobserver.Update(song.artistAndtitle);
                            _showSongInLoadScreenCount = 0;
                        }
                    }
                    else
                    {
                        DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(delegate
                                                                                            {
                             _playlist.Add(song);
                             _showSongInLoadScreenCount++;
                             if (_showSongInLoadScreenCount == _showEvery)
                             {
                                aLoadingobserver.Update(song.artistAndtitle);
                                 _showSongInLoadScreenCount = 0;
                             }
                         }));
                    }

                    i = _playlist.IndexOf(song);
                   
                    break;
                case ".ogg":
                    song = new OggSong(filepath, ABassWrapperOrganisation);
                    song.AddObserver(getObservers());
                    if (DispatcherThread == null)
                    {
                        _playlist.Add(song);
                        _showSongInLoadScreenCount++;
                        if (_showSongInLoadScreenCount == _showEvery)
                        {
                          aLoadingobserver.Update(song.artistAndtitle);
                            _showSongInLoadScreenCount = 0;
                        }
                    }
                    else
                    {
                        DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(delegate
                                                                                            {
                            _playlist.Add(song);
                            _showSongInLoadScreenCount++;
                            if (_showSongInLoadScreenCount == _showEvery)
                            {
                             aLoadingobserver.Update(song.artistAndtitle);
                                _showSongInLoadScreenCount = 0;
                            }
                        }));
                    }
                    i = _playlist.IndexOf(song);
                   
                    break;
                case ".wav":
                case ".mp1":
                case ".mp2":
                    song = new Mp3Song(filepath, ABassWrapperOrganisation);
                    song.AddObserver(getObservers());
                    if (DispatcherThread == null)
                    {
                        _playlist.Add(song);
                        _showSongInLoadScreenCount++;
                        if (_showSongInLoadScreenCount == _showEvery)
                        {
                            aLoadingobserver.Update(song.artistAndtitle);
                            _showSongInLoadScreenCount = 0;
                        }
                    }
                    else
                    {
                        DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(delegate
                                                                                            {
                            _playlist.Add(song);
                            _showSongInLoadScreenCount++;
                            if (_showSongInLoadScreenCount == _showEvery)
                            {
                                aLoadingobserver.Update(song.artistAndtitle);
                                _showSongInLoadScreenCount = 0;
                            }
                        }));
                    }
                    i = _playlist.IndexOf(song);
                    
                    break;
                default:
                    //Check if any plugin can playback the file
                    foreach (AvailablePlugin plugin in APluginManager.AvailablePlugins)
                    {
                        if (plugin.Instance.isAbleToPlayback(filepath))
                        {
                            song = new MiscSong(filepath, ABassWrapperOrganisation);
                            song.AddObserver(getObservers());
                            _playlist.Add(song);
                            i = _playlist.IndexOf(song);
                            break;
                        }
                    }
                    break;
            }
            if (stopRequest)
            {
                loadingRecursionDepth = 0;
                if (DispatcherThread != null)
                    DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                      () =>
                                                                                                      aLoadingobserver.
                                                                                                          Update(this)));
                return i;
            }

            return i;
            
        }

        /// <summary>
        /// Starts the Thread for loading songs into playlist
        /// </summary>
        /// <param name="folderpath"></param>
        public void startPLThread(string folderpath)
        {
            stopRequest = false;
            _plthread = new Thread(addFolder);
            _plthread.Start(folderpath);
        }

        /// <summary>
        /// Starts the Thread for loading more dirs into playlist
        /// </summary>
        /// <param name="folderpath"></param>
        public void startPLThreadfolders(string [] folderpath)
        {
            stopRequest = false;
            _plthread = new Thread(AddFolders);
            _plthread.Start(folderpath);
        }

        /// <summary>
        /// Function initializing the folderload(started in another thread)
        /// </summary>
        /// <param name="folderpaths"></param>
        public void AddFolders(object folderpaths)
        {
            var folders=(string []) folderpaths;
            foreach (string folder in folders)
            {
                addFolder(folder);
            }
        }


        /// <summary>
        /// Adds complete Folders of Songs at the end of the current playlist
        /// </summary>
        /// <returns>Return nothing because of running in another Thread</returns>
        public void addFolder(object obj)
        {
            const int index = 1;
            loadingRecursionDepth++;
            //filepath parameter for addsong
            var data = (string) obj;
            //FIND ALL FILES IN FOLDER 
            var dir = new DirectoryInfo(data);
            if (stopRequest)
            {
                loadingRecursionDepth = 0;
                DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                  () =>
                                                                                                  aLoadingobserver.
                                                                                                      Update(this)));
                return;
            }
            try
            {
                foreach (var f in dir.GetFiles("*.*"))
                {
                    addSong(f.FullName, index);

                    if (stopRequest)
                    {
                        loadingRecursionDepth = 0;
                        DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                          () =>
                                                                                                          aLoadingobserver
                                                                                                              .Update(
                                                                                                              this)));
                        return;
                    }
                }
               
                regenerateList();
            //FIND ALL FOLDERS IN FOLDER 
            foreach (var g in dir.GetDirectories())
            {
                //LOAD FOLDERS 
                addFolder(g.FullName);
                if (stopRequest)
                {
                    loadingRecursionDepth = 0;
                    DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                      () =>
                                                                                                      aLoadingobserver.
                                                                                                          Update(this)));
                    return;
                }
            }
          
                loadingRecursionDepth--;
                if (loadingRecursionDepth == 0)
                {
                    if (DispatcherThread == null)
                    {
                        aLoadingobserver.Update(this);
                    }
                    else
                    {
                        DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                          () =>
                                                                                                          aLoadingobserver
                                                                                                              .Update(
                                                                                                              this)));
                    }
                }
            return;
            }
            catch (System.Security.SecurityException)
            {
                loadingRecursionDepth--;
                if (DispatcherThread == null)
                {
                    aLoadingobserver.Update(this);
                }
                else
                {
                    DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                      () =>
                                                                                                      aLoadingobserver.
                                                                                                          Update(this)));
                }
                return;
            }
            catch (UnauthorizedAccessException)
            {
                loadingRecursionDepth--;
                if (DispatcherThread == null)
                {
                    aLoadingobserver.Update(this);
                }
                else
                {
                    DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                      () =>
                                                                                                      aLoadingobserver.
                                                                                                          Update(this)));
                }
                return;
            }
        }

        /// <summary>
        /// Delete songs on their specific position
        /// </summary>
        /// <returns>Returns true if it is ok or false when it fail</returns>
        /// <param name="index"></param>
        public bool deleteSong(int index)
        {
           
            _playlist.RemoveAt(index);
            regenerateList();
            Notify();
            return true;
        }

        /// <summary>
        /// Delete all Songs in the Playlist
        /// </summary>
        public void deleteAllSongs()
        {
           
            _playlist.Clear();
            regenerateList();
            Notify();
        }
        
        /// <summary>
        /// Jump to the given song position
        /// </summary>
        /// <returns>Returns a reference of a song object</returns>
        public ISong getSongAtIndex(int index)
        {
                return _playlist[index];   
        }

        /// <summary>
        /// little helper
        /// </summary>
        /// <param name="i">index</param>
        /// <returns>true or false</returns>
        private bool Valuehelper(int i)
        {
            if (i == getNowPlayingPosition())
                return true;
            return false;
        }


        /// <summary>
        /// Checks for the next Song and returns it
        /// </summary>
        /// /// <returns>Returns a reference of a song object</returns>
        public ISong getNextSong()
        {
            switch(_playMode) 
            {
                case PlayModeEnum.PlaymodeRandom:
                    try
                    {
                        int sindex = _shuffledList.FindIndex(Valuehelper);
                        if (sindex == _shuffledList.Count - 1)
                            sindex = 0;
                        else
                            sindex++;
                        setNowPlayingPosition(_shuffledList[sindex]);
                    }
                    catch (ArgumentOutOfRangeException) { setNowPlayingPosition(Rand.Next(_playlist.Count - 1)); }
                    break;
                case PlayModeEnum.PlaymodeRepeatSong:
                    break;
                    
                case PlayModeEnum.PlaymodeNormal:
                    if (getNowPlayingPosition() + 1 < _playlist.Count)
                    {
                        setNowPlayingPosition(getNowPlayingPosition() + 1);
                    }
                    else
                    {
                        _currentSong = null;
                    }
                    break;
                default: 
                    if (getNowPlayingPosition()+1 == _playlist.Count)
                            setNowPlayingPosition(0);
                         else
                            setNowPlayingPosition(getNowPlayingPosition()+1);
                    break;
                   
                
            }
            Notify();
            return getCurrentSong();
        }

        /// <summary>
        /// Gives back the previous song
        /// </summary>
        /// /// <returns>Returns a reference of a song object</returns>
        public ISong getPrevSong()
        {
           
            if (_playMode == PlayModeEnum.PlaymodeRandom)
            {
                try
                {
                    int sindex = _shuffledList.FindIndex(Valuehelper);
                    if (sindex == 0)
                        sindex = _shuffledList.Count - 1;
                    else
                        sindex--;
                    setNowPlayingPosition(_shuffledList[sindex]);
                }
                catch (ArgumentOutOfRangeException) { setNowPlayingPosition(Rand.Next(_playlist.Count - 1)); }
                                 
            }
            else if(_playMode == PlayModeEnum.PlaymodeRepeatSong)
            {

            }
            else
            {
                if (getNowPlayingPosition() != 0)
                {   _nowPlayingPosition--;
                _currentSong = getSongAtIndex(_nowPlayingPosition);
                  
                }
                else
                {
                     setNowPlayingPosition(_playlist.Count-1);
                }
            }
            Notify();
            return _currentSong;
          
        }
     

/*
        /// <summary>
        /// Returns a random Song without flag
        /// </summary>
        /// <returns>Returns a reference of a song object</returns>
        private ISong getRandomSong()
        {
            return null;
        }
*/

        /// <summary>
        /// Returns the current song
        /// </summary>
        /// <returns>Return the index of the current song</returns>
        public int getNowPlayingPosition()
        {
            if (_nowPlayingPosition>=_playlist.Count || getCurrentSong() != _playlist[_nowPlayingPosition])
            {
                
                _nowPlayingPosition = -1;
                for (int i=0;i<_playlist.Count;i++)
                {
                    if (_playlist[i] == getCurrentSong())
                    {
                        _nowPlayingPosition = i;
                    }
                }
                if (_nowPlayingPosition == -1)
                    _nowPlayingPosition = 0;
            }
            return _nowPlayingPosition;
        }

        /// <summary>
        /// Set the current song position
        /// </summary>
        /// <returns>Return true if it is ok and false when it fails</returns>
        /// <param name="index"></param>
        public bool setNowPlayingPosition(int index)
        {
            _nowPlayingPosition = index;

            if (_playlist.Count > _nowPlayingPosition)
            {
                SetCurrentSong(_playlist[_nowPlayingPosition]);
                return true;
            }
            /*
            try
            {
                this.SetCurrentSong(_playlist[_nowPlayingPosition]);
            }
            catch(ArgumentOutOfRangeException e)
            {
                throw e;
            }*/

            return false;
        }
  
   
        /// <summary>
        /// Delivers current Song
        /// </summary>
        /// <returns>Current Song</returns>
        public ISong getCurrentSong()
        {
            return _currentSong;
        }

        /// <summary>
        /// Sets Current Song
        /// </summary>
        /// <param name="s">Song to Set</param>
        private void SetCurrentSong(ISong s)
        {
           
            _currentSong = s;
            Notify();
        }

        /// <summary>
        /// Delivers current Song loading into Playlist
        /// </summary>
        /// <returns>Current loading Song</returns>
        public ISong getLoadingSong()
        {
            return _nowLoadingSong;
        }

        /// <summary>
        /// Set current Song loading into Playlist
        /// </summary>
        /// <param name="s">Song to Set</param>
        private void SetLoadingSong(ISong s)
        {
            _nowLoadingSong = s;
        }

        /// <summary>
        /// Sets Playmode
        /// </summary>
        /// <param name="mode">Playmode</param>
        public void setPlayMode(int mode)
        {
            _playMode = (PlayModeEnum)mode;
            
            Notify();
           
        }

        /// <summary>
        /// Delivers PlayMode
        /// </summary>
        /// <returns>PlayMode</returns>
        public int getPlayMode()
        {
            return (int)_playMode;
        }


        /// <summary>
        /// Saves the Playlist given by fl
        /// </summary>
        /// <param name="fl">filepath</param>
        public void savePL(string fl)
        {
            string filename = fl;

            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            
                var xmlDoc = new XmlDocument();


                var xmlWriter = new XmlTextWriter(filename, System.Text.Encoding.UTF8)
                                    {Formatting = Formatting.Indented};
            xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                xmlWriter.WriteStartElement("Playlist");
                xmlWriter.Close();
                xmlDoc.Load(filename);


                XmlNode root = xmlDoc.DocumentElement;
                XmlElement tracklist = xmlDoc.CreateElement("tracklist");
            if (root != null) root.AppendChild(tracklist);


            foreach (ISong s in _playlist)
                {
                    
                    XmlElement track = xmlDoc.CreateElement("track");

                    XmlElement album = xmlDoc.CreateElement("album");
                    XmlText albumText = xmlDoc.CreateTextNode(s.getTags()[0]);

                    XmlElement artist = xmlDoc.CreateElement("artist");
                    XmlText artistText = xmlDoc.CreateTextNode(s.getTags()[1]);

                    XmlElement title = xmlDoc.CreateElement("title");
                    XmlText titleText = xmlDoc.CreateTextNode(s.getTags()[2]);

                    XmlElement fp = xmlDoc.CreateElement("filepath");
                    XmlText fpText = xmlDoc.CreateTextNode(s.getFilePath());

                    String durationS = s.dlength.ToString(_provider);
                    XmlElement duration = xmlDoc.CreateElement("duration");
                    XmlText durationText = xmlDoc.CreateTextNode(durationS);

                    tracklist.AppendChild(track);

                    track.AppendChild(album);
                    album.AppendChild(albumText);

                    track.AppendChild(artist);
                    artist.AppendChild(artistText);

                    track.AppendChild(title);
                    title.AppendChild(titleText);

                    track.AppendChild(fp);
                    fp.AppendChild(fpText);

                    track.AppendChild(duration);
                    duration.AppendChild(durationText);
                }

                xmlDoc.Save(filename); 
        }

        /// <summary>
        /// Starts the Thread for loading songs into playlist
        /// </summary>
#pragma warning disable 1574
        /// <param name="folderpath"></param>
#pragma warning restore 1574
        public void startXMLThread(string fileLoc)
        {
            _xmlThread = new Thread(loadPL);
            _xmlThread.Start(fileLoc);
        }


        /// <summary>
        /// Loads the Playlist given by fl
        /// </summary>
#pragma warning disable 1574
        /// <param name="fl">filepath</param>
#pragma warning restore 1574
        public void loadPL(object obj)
        {
            var fl = (string)obj;
            try
            {
                string fileName = fl;
                
                var doc = new XPathDocument(fileName);
                XPathNavigator nav = doc.CreateNavigator();

                String album;
                String artist;
                String title;
                String filepath;
                String duration;
                double dur;


                // Compile a standard XPath expression

                XPathExpression expr = nav.Compile("/Playlist/tracklist/track");
                XPathNodeIterator iterator = nav.Select(expr);

                while (iterator.MoveNext())
                {
                    iterator.Current.MoveToChild("album", string.Empty);
                    if (iterator.Current.Matches("album"))
                    {
                        album = iterator.Current.Value;
                        iterator.Current.MoveToParent();
                    }
                    else
                        album = "";
                    

                    iterator.Current.MoveToChild("artist", string.Empty);
                    if (iterator.Current.Matches("artist"))
                    {
                        artist = iterator.Current.Value;
                        iterator.Current.MoveToParent();
                    }
                    else
                        artist = "";

                    iterator.Current.MoveToChild("title", string.Empty);
                    if (iterator.Current.Matches("title"))
                    {
                        title = iterator.Current.Value;
                        iterator.Current.MoveToParent();
                    }
                    else
                        title = "";

                    iterator.Current.MoveToChild("filepath", string.Empty);
                    if (iterator.Current.Matches("filepath"))
                    {
                        filepath = iterator.Current.Value;
                        iterator.Current.MoveToParent();
                    }
                    else
                        filepath = "";

                    iterator.Current.MoveToChild("duration", string.Empty);
                    if (iterator.Current.Matches("duration"))
                    {
                        duration = iterator.Current.Value;
                        iterator.Current.MoveToParent();
                    }
                    else
                        duration = "0";

                    dur = Convert.ToDouble(duration,_provider);

                    if (DispatcherThread == null)
                    {
                        addSong(album, artist, title, filepath, dur);
                    }
                    else
                    {
                        string alb = album;
                        string index = artist;
                        string tit = title;
                        string s = filepath;
                        double d = dur;
                        DispatcherThread.Invoke(DispatcherPriority.DataBind, new Action(
                                                                                                          () =>
                                                                                                          addSong(
                                                                                                              alb,
                                                                                                              index,
                                                                                                              tit,
                                                                                                              s,
                                                                                                              d)));
                    }
                }
                regenerateList();
            }
            catch (XmlException)
            {
                var e = new PlaylistException(1, "Corruptive Playlist!") {Source = "Gamenoiselist"};
                throw e;
            }
        }

        /// <summary>
        /// Random Generator
        /// </summary>
        private static readonly Random Rand = new Random();

        /// <summary>
        /// Shuffle a List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ilist">list to shuffle</param>
        public static void Shuffle<T>(IList<T> ilist)
        {
            int iIndex;
            T tTmp;
            for (int i = 1; i < ilist.Count; ++i)
            {
                iIndex = Rand.Next(i + 1);
                tTmp = ilist[i];
                ilist[i] = ilist[iIndex];
                ilist[iIndex] = tTmp;
            }
        }

        /// <summary>
        /// regenerates List
        /// </summary>
        public void regenerateList()
        {
            if (_shuffledList.Count < _playlist.Count)
            {
                for (int i = _shuffledList.Count; i < _playlist.Count; i++)
                    _shuffledList.Add(i);
                Shuffle(_shuffledList);
            } else if (_shuffledList.Count > _playlist.Count){

                for (int i = 0; i < (_shuffledList.Count - _playlist.Count); i++)
                {
                    _shuffledList.Remove(_shuffledList.Max());
                }
            }

        }

       
    }
}