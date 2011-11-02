/* 
 * author: MHO
 * 
 * created: 12.01.2008
 * 
 * modification history
 * --------------------
 * 
 * MHO (12.01.08):
 * Configuration-Class + ConfigHotKey-Class
 * 
 * BK (21.01.08):
 * Overlay-Position
 * 
 * MHI (02.02.09)
 * added 'resumePlayback' Checkbox
 * 
*/

using System;
using Observer;

namespace Organisation
{
    /// <summary>
    /// Possible positions of the overlay
    /// </summary>
    public enum OverlayPositionEnum { TOP = 0, Topleft = 1, Topright = 2, Bottom = 3, Bottomleft = 4, Bottomright = 5 };

    /// <summary>
    /// Saves all settings
    /// </summary>
    [Serializable]
    public class Configuration : Subject
    {
        /// <summary>
        /// Configuration of HotKey Play
        /// </summary>
        private ConfigHotKey _hotkeyPlay;

        /// <summary>
        /// Configuration of HotKey Stop
        /// </summary>
        private ConfigHotKey _hotkeyStop;

        /// <summary>
        /// Configuration of HotKey Prev
        /// </summary>
        private ConfigHotKey _hotkeyPrev;

        /// <summary>
        /// Configuration of HotKey Overlay
        /// </summary>
        private ConfigHotKey _hotkeyOverlay;

        /// <summary>
        /// Configuration of HotKey VolUp
        /// </summary>
        private ConfigHotKey _hotkeyVolUp;

        /// <summary>
        /// Configuration of HotKey VolDown
        /// </summary>
        private ConfigHotKey _hotkeyVolDown;

        /// <summary>
        /// Path to the application
        /// </summary>
        private String _pathSkins;

        /// <summary>
        /// Path to the Plugins
        /// </summary>
        private String _pathPlugins;

        /// <summary>
        /// Current skin
        /// </summary>
        private String _usedSkin;


        public string getFallBackSkin()
        {
            return "gamenoiseLight";
        }


        /// <summary>
        /// Configuration of Background-Color for the Overlay
        /// </summary>
        private ConfigOverlayColor _overlayColorBack;

        /// <summary>
        /// Configuration of Hotkey-Top-Color for the Overlay
        /// </summary>
        private ConfigOverlayColor _overlayColorHkTop;

        /// <summary>
        /// Configuration of Hotkey-Back-Color for the Overlay
        /// </summary>
        private ConfigOverlayColor _overlayColorHkBack;

        /// <summary>
        /// Configuration of Font-Color for the Overlay
        /// </summary>
        private ConfigOverlayColor _overlayColorFont;

        /// <summary>
        /// Configuration of Line-Color for the Overlay
        /// </summary>
        private ConfigOverlayColor _overlayColorLine;

        /// <summary>
        /// Configuration of ProgressBar-Color for the Overlay
        /// </summary>
        private ConfigOverlayColor _overlayColorProgress;

        /// <summary>
        /// Configuration of Overlay-Position
        /// </summary>
        private OverlayPositionEnum _overlayPosition;

        private bool _overlayTimerEnabled;
        private int _overlayTimerSeconds;

        /// <summary>
        /// Configuration for every plugin
        /// </summary>
        public class PluginSettings
        {
            public String Name { get; set; }
            public String Skin { get; set; }
        }


        /// <summary>
        /// Class that stores the configuration of an hotkey
        /// </summary>
        public class ConfigHotKey
        {
            /// <summary>
            /// Alt-Key state
            /// </summary>
            private bool _mAlt;
            /// <summary>
            /// Ctrl-Key state
            /// </summary>
            private bool _mCtrl;
            /// <summary>
            /// Shift-Key state
            /// </summary>
            private bool _mShift;
            /// <summary>
            /// Hotkey enabled or disabled
            /// </summary>
            private bool _enabled;
            /// <summary>
            /// Key on the keyboard
            /// </summary>
            private System.Windows.Forms.Keys _hKey;
            /// <summary>
            /// Configuration object
            /// </summary>
            private Configuration _myConfig;

            /// <summary>
            /// Constructor initialize the values
            /// </summary>
            public ConfigHotKey()
            {
                InitValues();
            }

            /// <summary>
            /// Sets the configuraiton object
            /// </summary>
            /// <param name="config">Configuration object</param>
            public void SetConfiguration(Configuration config)
            {
                _myConfig = config;
            }



            /// <summary>
            /// Initialize the values to the default value
            /// </summary>
            private void InitValues()
            {
                _mAlt = false;
                _mCtrl = false;
                _mShift = false;
                _enabled = false;
                _hKey = System.Windows.Forms.Keys.A;
            }

            /// <summary>
            /// Second destructor if no xml-file is available
            /// </summary>
            /// <param name="config">Configuration object</param>
            public ConfigHotKey(Configuration config)
            {
                InitValues();
                _myConfig = config;
            }

            /// <summary>
            /// Property for Alt-Key
            /// </summary>
            public bool MAlt {
                get { return _mAlt; }
                set { _mAlt = value; ParentNotify(); }
            }

            /// <summary>
            /// Property for Ctrl-Key
            /// </summary>
            public bool MCtrl
            {
                get { return _mCtrl; }
                set { _mCtrl = value; ParentNotify(); }
            }

            /// <summary>
            /// Property for Shift-Key
            /// </summary>
            public bool MShift
            {
                get { return _mShift; }
                set { _mShift = value; ParentNotify(); }
            }

            /// <summary>
            /// Property for Key
            /// </summary>
            public System.Windows.Forms.Keys HKey
            {
                get { return _hKey; }
                set { _hKey = value; ParentNotify(); }
            }

            /// <summary>
            /// Property for Enable/Disable
            /// </summary>
            public bool IsEnabled
            {
                get { return _enabled; }
                set { _enabled = value; ParentNotify(); }
            }

            /// <summary>
            /// Notify the configuration object
            /// </summary>
            private void ParentNotify()
            {

                    if(_myConfig!=null)
                    _myConfig.Notify();
 
            }
        }


        public class ConfigOverlayColor
        {
            private Byte _alpha;
            private Byte _red;
            private Byte _green;
            private Byte _blue;
            private Configuration _myConfig;

            public ConfigOverlayColor()
            {
                InitValues();
            }

            public void SetConfiguration(Configuration config)
            {
                _myConfig = config;
            }


            private void InitValues()
            {
                _alpha = 255;
                _red = 0;
                _green = 0;
                _blue = 0;
            }

            public ConfigOverlayColor(Configuration config)
            {
                InitValues();
                _myConfig = config;
            }

            public Byte Alpha
            {
                get { return _alpha; }
                set { _alpha = value; ParentNotify(); }
            }

            public Byte Red
            {
                get { return _red; }
                set { _red = value; ParentNotify(); }
            }

            public Byte Green
            {
                get { return _green; }
                set { _green = value; ParentNotify(); }
            }

            public Byte Blue
            {
                get { return _blue; }
                set { _blue = value; ParentNotify(); }
            }

            private void ParentNotify()
            {
                    if (_myConfig != null)
                        _myConfig.Notify();
            }
        }


        /// <summary>
        /// Contructor that initialize all variables to the default values
        /// </summary>
        public Configuration()
        {
            _overlayColorBack = new ConfigOverlayColor();
            _overlayColorHkTop = new ConfigOverlayColor();
            _overlayColorHkBack = new ConfigOverlayColor();
            _overlayColorFont = new ConfigOverlayColor();
            _overlayColorLine = new ConfigOverlayColor();
            _overlayColorProgress = new ConfigOverlayColor();

            MyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Gamenoise\\";

            ResetToDefault();
        }

        /// <summary>
        /// Resets all settings to default
        /// </summary>
        public void ResetToDefault()
        {
            _hotkeyPlay = new ConfigHotKey();
            _hotkeyStop = new ConfigHotKey();
            HotkeyNext = new ConfigHotKey();
            _hotkeyPrev = new ConfigHotKey();
            _hotkeyOverlay = new ConfigHotKey();
            _hotkeyVolUp = new ConfigHotKey();
            _hotkeyVolDown = new ConfigHotKey();

            _overlayColorBack.Red = 255;
            _overlayColorBack.Green = 255;
            _overlayColorBack.Blue = 255;

            _overlayColorHkTop.Red = 255;
            _overlayColorHkTop.Green = 179;
            _overlayColorHkTop.Blue = 0;

            _overlayColorHkBack.Red = 255;
            _overlayColorHkBack.Green = 255;
            _overlayColorHkBack.Blue = 255;

            _overlayColorFont.Red = 0;
            _overlayColorFont.Green = 0;
            _overlayColorFont.Blue = 0;

            _overlayColorLine.Red = 0;
            _overlayColorLine.Green = 0;
            _overlayColorLine.Blue = 0;

            _overlayColorProgress.Red = 255;
            _overlayColorProgress.Green = 179;
            _overlayColorProgress.Blue = 0;

            UsedSkin = "gamenoiseLight";
            MinimizeToTray = false;
            Language = "en-US";

            PathTmpSettings = MyDocumentsPath + "settings.tmp";
            PathTmpEqualizer = MyDocumentsPath + "Equalizer.set";
            PathTmpPlaylist = MyDocumentsPath + "playlist.tmp";
            PathFeaturedSongs = MyDocumentsPath + "songs";
            PathFeaturedSongsFlag = MyDocumentsPath + "addsongs.gn";
            ResumePlayback = true;
            
            
            //DONT CHANGE PathSettings, BECAUSE THE SAME PATH WILL BE USED IN App.xaml.cs TO LOAD THE FILE
            PathSettings = MyDocumentsPath + "settings.xml";
 

            Notify();
        }

        /// <summary>
        /// Initialize the configuration object to the hotkeys
        /// This function must be called after the deserializer to init the objects
        /// </summary>
        public void Init()
        {
            _hotkeyPlay.SetConfiguration(this);
            _hotkeyStop.SetConfiguration(this);
            HotkeyNext.SetConfiguration(this);
            _hotkeyPrev.SetConfiguration(this);
            _hotkeyOverlay.SetConfiguration(this);
            _hotkeyVolUp.SetConfiguration(this);
            _hotkeyVolDown.SetConfiguration(this);

            _overlayColorBack.SetConfiguration(this);
            _overlayColorHkTop.SetConfiguration(this);
            _overlayColorHkBack.SetConfiguration(this);
            _overlayColorFont.SetConfiguration(this);
            _overlayColorLine.SetConfiguration(this);
            _overlayColorProgress.SetConfiguration(this);

            //Set the skin path here
            _pathSkins = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Skins/";
            _pathPlugins = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Plugins/";

            System.IO.Directory.CreateDirectory(MyDocumentsPath);
           
        }

        /// <summary>
        /// Initialization Complete Notify
        /// </summary>
        public void InitNotify()
        {
            Notify();
        }

        /// <summary>
        /// Returns the directory of the skins
        /// </summary>
        /// <returns>Directory name of the skins</returns>
        public String GetSkinPath()
        {
            if (_pathSkins != null) return _pathSkins;
            return "";
        }

        /// <summary>
        /// Returns the directory of the Plugins
        /// </summary>
        /// <returns>Directory name of the skins</returns>
        public String GetPluginPath()
        {
            if (_pathPlugins != null) return _pathPlugins;
            return "";
        }

        #region misc

        public String UsedSkin {
            get
            {
                if (_usedSkin != null) return _usedSkin;
                return "";
            }
            set
            {
                _usedSkin = value;
                Notify();
            }
        }

        private bool _minimizeToTray;
        public bool MinimizeToTray
        {
            get { return _minimizeToTray; }
            set { _minimizeToTray = value; }
        }

        private bool _resumePlayback;
        public bool ResumePlayback
        {
            get { return _resumePlayback; }
            set { _resumePlayback = value; }
        }

        #endregion

        #region Paths

        private string _pathFeaturedSongs;
        public String PathFeaturedSongs
        {
            get { return _pathFeaturedSongs; }
            set { _pathFeaturedSongs = value; }
        }

        private string _pathFeaturedSongsFlag;
        public String PathFeaturedSongsFlag
        {
            get { return _pathFeaturedSongsFlag; }
            set { _pathFeaturedSongsFlag = value; }
        }

        private string _myDocumentsPath;
        public String MyDocumentsPath
        {
            get { return _myDocumentsPath; }
            set { _myDocumentsPath = value; }
        }

        private string _pathTmpSettings;
        public String PathTmpSettings
        {
            get { return _pathTmpSettings; }
            set { _pathTmpSettings = value; }
        }

        private string _pathTmpPlaylist;
        public String PathTmpPlaylist
        {
            get { return _pathTmpPlaylist; }
            set { _pathTmpPlaylist = value; }
        }

        private string _pathTmpEqualizer;
        public String PathTmpEqualizer
        {
            get { return _pathTmpEqualizer; }
            set { _pathTmpEqualizer = value; }
        }

        private string _pathSettings;
        public String PathSettings
        {
            get { return _pathSettings; }
            set { _pathSettings = value; }
        }

        #endregion

        #region HotKeyPropertys

        public ConfigHotKey HotkeyPlay
        {
            get { return _hotkeyPlay; }
            set { _hotkeyPlay = value; }
        }

        public ConfigHotKey HotkeyNext { get; set; }

        public ConfigHotKey HotkeyStop
        {
            get { return _hotkeyStop; }
            set { _hotkeyStop = value; }
        }

        public ConfigHotKey HotkeyPrev
        {
            get { return _hotkeyPrev; }
            set { _hotkeyPrev = value; }
        }

        public ConfigHotKey HotkeyOverlay
        {
            get { return _hotkeyOverlay; }
            set { _hotkeyOverlay = value; }
        }

        public ConfigHotKey HotkeyVolUp
        {
            get { return _hotkeyVolUp; }
            set { _hotkeyVolUp = value; }
        }

        public ConfigHotKey HotkeyVolDown
        {
            get { return _hotkeyVolDown; }
            set { _hotkeyVolDown = value; }
        }
        #endregion


        #region OverlayColorPropertys

        public ConfigOverlayColor OverlayColorBack
        {
            get { return _overlayColorBack; }
            set { _overlayColorBack = value; }
        }

        public ConfigOverlayColor OverlayColorHKTop
        {
            get { return _overlayColorHkTop; }
            set { _overlayColorHkTop = value; }
        }

        public ConfigOverlayColor OverlayColorHKBack
        {
            get { return _overlayColorHkBack; }
            set { _overlayColorHkBack = value; }
        }

        public ConfigOverlayColor OverlayColorFont
        {
            get { return _overlayColorFont; }
            set { _overlayColorFont = value; }
        }

        public ConfigOverlayColor OverlayColorLine
        {
            get { return _overlayColorLine; }
            set { _overlayColorLine = value; }
        }

        public ConfigOverlayColor OverlayColorProgress
        {
            get { return _overlayColorProgress; }
            set { _overlayColorProgress = value; }
        }

        #endregion

        
        public String Language
        {
            get;
            set;
        }

        public OverlayPositionEnum OverlayPosition
        {
            get { return _overlayPosition; }
            set { _overlayPosition = value; }
        }

        public bool OverlayTimerEnabled
        {
            get { return _overlayTimerEnabled; }
            set { _overlayTimerEnabled = value; }
        }

        public int OverlayTimerSeconds
        {
            get { return _overlayTimerSeconds; }
            set { _overlayTimerSeconds = value; }
        }

    }
}
