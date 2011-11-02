#region fileheader
/* 
 * author: BK
 * 
 * created: 07.11.2008
 * 
 * modification history
 * --------------------
 * 
 * AK (19.11.08):
 * Observer Concept added
 * 
 * PL,AK,MHI,MHO (20.11.08)
 * Data Binding Added (c#) 
 * Mouseclick event handler (doubleklick) added
 * 
 * 19.11. MHI switch playMode works now also if not song is selected and also with dropdown list
 * 
 * MHO (21.11.08):
 * Visualization added.
 * Timer for Visualization added (maybe improvable with observer)
 * 
 * AK (22.11.08)
 * Thumb Back Bug cleaned
 * 
 * PL(22.11.08)
 * AddFolder Funktionalität hinzugefügt#
 * 
 * MHI (24.11.08)
 * added events for Volume and seekbar, left click and change value
 * 
 * MHO (26.11.08)
 * added functionality to 'Game' button
 * 
 * MHI (26.11.08)
 * - added functionality to the EQ sliders and the resetEQ button
 * - Settings, LoadEQ and SaveEQ window are now initialized in the function of the button to show them,
 *   this fixes the bug that a window can only be shown once
 *
 * MHI (26.11.08)
 * - update for EQ sliders
 * 
 * MHI (30.11.08)
 * - change Content of EQ On/Off button to EQ-Off, if the EQ is on and EQ-On, if the EQ is off
 * - showed Play-time changed from 00:00 to 0:00
 * 
 * BK (30.11.08):
 * HotKeys added
 * 
 * MHI (01.12.08)
 * saveEQ and loadEQ opens now as a Dialog (nothing else can be pressed)
 * 
 * 
 * AK (01.12.08)
 * Fixed Click Into Handler Seekbar
 * 
 * MHO (01.12.08)
 * Added functions to load a skin at startup
 * 
 * PL (02.12.08)
 * Implemented DragnDrop
 * 
 * MHO (02.12.08)
 * Implemented new visualizations
 * 
 * BK (02.12.08)
 * show/hide EQ- and Playlist-Panels
 * 
 * BK (04.12.08)
 * show/hide EQ- and Playlist-Panels improved
 * 
 * PL(04.12.08):
 * addFolder Button now calls Method for creating new Thread
 * 
 * AK(04.12.08):
 * Eq-Drag Delta fixed
 * 
 * PL(04.12.08):
 * DragnDrop improvised
 * 
 * MHI(05.12.08):
 * added dB label
 * changed the name of style files, for better usability
 * 
 * MHO(06.12.08):
 * Playlist columns set on change
 * 
 * AK(07.12.08):
 * Drag&Drop fixed whitespace after last element Drop
 * 
 * AK(07.12.08):
 * better AutoScroll 
 * 
 * MHO(09.12.08):
 * Song-Delete-Function
 *
 * PL(16.12.08):
 * LoadingPlaylist Window
 * 
 * MHO(21.12.08):
 * Scroll text added
 * 
 * PL(21.12.08)
 * save,load,new playlist added
 * 
 * AK(23.12.08)
 * Stop-Request for Threads
 * 
 * AK(23.12.08)
 * Tmp-Settings are now loaded and saved
 * 
 * AK(23.12.08)
 * Added Search Methods
 * 
 * PL(30.12.08)
 * Color the currently played song
 * 
 * MHI (02.01.09)
 * Playmode comboBox removed
 * volumebar label now percentage instead of db
 * Playmode set on start
 * 
 * MHI (08.01.09)
 * show artist+title in taskbar
 * 
 * PL (11.01.09)
 * yesno dialog added, playlistname added
 * 
 * PL (14.01.09)
 * Trayicon Funktions added
 * 
 * BK (14.01.09)
 * Ingame-Button changed to Checkbox
 * 
 * MHI (01.02.09)
 * reload last player state at player start
 * 
 */

#endregion

#region Note
/* NOTE
 * Why do I get a 'LoaderLock' Error when debugging the application?
 * 
 * A Loader lock is one of the Managed Debugging Assistants (MDAs) that were added
 * to VS2005 to help find hard to debug runtime issues. There is code in all Managed
 * DirectX 1.1 assemblies that causes this MDA to fire. Microsoft have confirmed they
 * are aware of the problem. However I do not expect to see a fix for MDX 1.1 since
 * current efforts are focused on MDX2.0/XNA Framework, it ONLY affects code run under
 * the debugger (i.e. it won't happen when users run your EXE).
 * 
 * To work around the problem you have several choices (3. recommended):
 *  
 * 1. Go back to using VS2003 and .Net 1.1
 * 2. Use MDX 2.0. Note that MDX 2.0 will never actually ship as it is being
 *    transformed into the XNA framework.
 * 3. Disable the loader lock MDA. Debug/Exceptions (ctrl-D, E), Open the Managed
 *    Debugging Assistants tree node and uncheck Loader Lock. This setting is per
 *    solution so it will only affect this solution.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Drawing;
using PlayControl;
using Organisation;
using System.Windows.Forms;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Markup;
using System.Windows.Controls.Primitives;
using System.Xml.Serialization;
using System.Reflection;
using System.ComponentModel;
using Interfaces;
using PluginManager;
using DragDropEffects=System.Windows.DragDropEffects;
using ListViewItem=System.Windows.Controls.ListViewItem;

namespace UserInterface
{
    /// <summary>
    /// Interaktionslogik für Standard.xaml
    /// </summary>
    public partial class Standard : IObserver, IPluginHost
    {
        #region References_Properties
        /// <summary>
        /// Reference to PlayControler
        /// </summary>
        public PlayControler APlayControler { get; set; }

        /// <summary>
        /// Reference to  BassWrapper
        /// </summary>
        public IBassWrapperGui ABassWrapperGui { get; set; }

        /// <summary>
        /// Reference to Playlist
        /// </summary>
        public IPlaylist APlaylist { get; set; }

        /// <summary>
        /// Reference to PluginManager
        /// </summary>
        public PluginManager.PluginManager APluginManager { get; set; }

        /// <summary>
        /// Ingame Overlay
        /// </summary>
        public Ingame AIngame { get; set; }

        /// <summary>
        /// Configuration Class for Settings
        /// </summary>
        public Configuration AConfiguration { get; set; }

        private readonly App _anAppHandle;

        private readonly Visualization _aVisualization;

        /// <summary>
        /// Set This as Lock if a Folder is already added to the Player
        /// </summary>
        public bool InsertFolderprop { set { _insertFolder = value; } }

        private readonly ObservableCollection<ISong> _searchsongs = new ObservableCollection<ISong>();
        public ObservableCollection<ISong> PlayListRef = new ObservableCollection<ISong>();
        public ObservableCollection<ISong> PlayListRefProp { get { return PlayListRef; } }

        #endregion

        #region AdditionalWindows
        // Some Windows, new statement is in the button which shows the window
        SaveEQ _aSaveEq;
        LoadEQ _aLoadEq;
        Settings _aSettings;
        LoadingTitle _aLoadingTitle;
        YesNo _aYesNoDialog; 
        #endregion

        #region Hotkeys
        // Hotkeys
        Hotkey _hotkeyPlayPause;
        Hotkey _hotkeyStop;
        Hotkey _hotkeyNext;
        Hotkey _hotkeyPrev;
        Hotkey _hotkeyOverlay;
        Hotkey _hotkeyVolUp;
        Hotkey _hotkeyVolDown; 
        #endregion

        #region Settings_CurrentState
        /// <summary>
        ///  Window Size to start
        /// </summary>
        public double SizedWidth = 397;

        //Saves the current used skin
        private string _currentSkin = "";
        private int _currentVis;
        private bool _changeSeekBarDragged;
        private int _currentHighlightedSong;
        private string _currentPlaylist;
        private string _lastOpenedFolderpath;
        private bool _insertFolder;

        #endregion

        #region TrayIcon
        //Trayicon
        private readonly NotifyIcon _notifyIcon1;

        //trayicon stuff
        private readonly IContainer _componentForTray;
        private ContextMenuStrip _contextMenuStrip1;
        private ToolStripMenuItem _playToolStripMenuItem;
        private ToolStripMenuItem _stopToolStripMenuItem;
        private ToolStripMenuItem _nextSongToolStripMenuItem;
        private ToolStripMenuItem _pevToolStripMenuItem;
        private ToolStripMenuItem _fileToolStripMenuItem;
        private ToolStripSeparator _toolStripSeparator1;
        private ToolStripMenuItem _addFileToolStripMenuItem;
        private ToolStripMenuItem _addFolderToolStripMenuItem;
        private ToolStripMenuItem _playlistToolStripMenuItem;
        private ToolStripMenuItem _newPlaylistToolStripMenuItem;
        private ToolStripMenuItem _openPlaylistToolStripMenuItem;
        private ToolStripMenuItem _savePlaylistToolStripMenuItem;
        private ToolStripMenuItem _equalizerToolStripMenuItem;
        private ToolStripMenuItem _onToolStripMenuItem;
        private ToolStripMenuItem _resetToolStripMenuItem;
        private ToolStripMenuItem _loadPresetToolStripMenuItem;
        private ToolStripMenuItem _savePresetToolStripMenuItem;
        private ToolStripMenuItem _settingsToolStripMenuItem;
        private ToolStripMenuItem _closeToolStripMenuItem;
        private ToolStripSeparator _toolStripSeparator2; 
        #endregion

        #region ScrollText
        //private System.Windows.Threading.DispatcherTimer clockVis;
        private readonly DispatcherTimer _clockScrollingText;


        readonly ScrollData _scrollDataArtist;
        readonly ScrollData _scrollDataTitle;
        readonly ScrollData _scrollDataAlbum;

        const int ScrollWaitTime = 2;
        const int ScrollSteps = 2;
        const int ScrollSpeed = 180; 
        #endregion

        #region Misc
        private DispatcherTimer _overlayTimer;

        //delegate for dragndrop
        delegate System.Windows.Point GetPositionDelegate(IInputElement element);

        private readonly Dictionary<String, Plugin> _dictPluginWindows;

        /// <summary>
        /// Background Worker for Visualisation 
        /// </summary>
        private BackgroundWorker _backgroundWorker1;

        /// <summary>
        /// For Totaltime Calculation
        /// </summary>
        private int _oldcount;
        private double _overallLength;
        

        const int VisTimerInMs = 25; 
        #endregion

       



        #region InitANDClosingMehtods
        /// <summary>
        /// Constructor for Main Window
        ///     - Init Visualisation
        ///     - Text Scrolling
        ///     - Add Event Handlers
        ///     - Apply a Skin 
        ///     - Initialize Tray Icon
        /// </summary>
        /// <param name="appHandle"></param>
        /// <param name="aConfiguration"></param>
        /// <param name="aPluginManager"></param>
        public Standard(App appHandle, Configuration aConfiguration, PluginManager.PluginManager aPluginManager)
        {
            _anAppHandle = appHandle;
            AConfiguration = aConfiguration;
            APluginManager = aPluginManager;
            _aVisualization = new Visualization(2048, VisTimerInMs);

            /*clockVis = new System.Windows.Threading.DispatcherTimer();
            clockVis.Interval = new TimeSpan(0, 0, 0, 0, VisTimerInMs);
            clockVis.Tick += new EventHandler(ClockVisTick);
            */


            _clockScrollingText = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, ScrollSpeed) };
            _clockScrollingText.Tick += ClockScrollingTextTick;
            _clockScrollingText.IsEnabled = true;

            _scrollDataArtist = new ScrollData();
            _scrollDataAlbum = new ScrollData { ScrollWidthCorrection = -8 };
            _scrollDataTitle = new ScrollData();

            _currentHighlightedSong = -1;

            _changeSeekBarDragged = false;

            InitializeComponent();
            _currentVis = 1;
            _insertFolder = false;

            // Initialise Click-Into BarHandlers
            changeSeekBar.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(ChangeSeekBarMouseLeftButtonDown), true);
            changeVolumeBar.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(ChangeVolumeBarMouseLeftButtonDown), true);
            changeBalance.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(changeBalanceSlide), true);
            sliderEQBand0.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand1.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand2.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand3.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand4.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand5.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand6.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand7.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand8.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);
            sliderEQBand9.AddHandler(PreviewMouseLeftButtonDownEvent, new RoutedEventHandler(sliderEQBand_DragCompleted), true);


            //Trayiconstuff
            _componentForTray = new Container();
            _notifyIcon1 = new NotifyIcon(_componentForTray);

            //Apply skin
            _currentSkin = aConfiguration.UsedSkin;
            ApplySkin(_currentSkin);

            //Gamnoise Plugin
            _dictPluginWindows = new Dictionary<string, Plugin>();
            InitPlugins();
        }

        /// <summary>
        /// Window Loaded sets the Playlist Reference & binds it to the listView
        /// 
        ///  - Set Drag&Drop event Handlers
        ///  - Backgroundworker initialisation
        ///  - Loading Tmp-Settings
        ///  - Set Totaltime Variable
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {

            PlayListRef = APlaylist.Playlist; //geaendert von ._Playlist (MHO)
            listView.DataContext = PlayListRefProp;
            System.Windows.Data.Binding bind = new System.Windows.Data.Binding();
            listView.SetBinding(ItemsControl.ItemsSourceProperty, bind);

            searchView.DataContext = _searchsongs;
            System.Windows.Data.Binding bind2 = new System.Windows.Data.Binding();
            searchView.SetBinding(ItemsControl.ItemsSourceProperty, bind2);


            listView.PreviewMouseLeftButtonDown += ListViewMouseLeftButtonDown;
            listView.Drop += ListViewDrop;
            listView.DragOver += ListViewDragOver;

            _aLoadingTitle = new LoadingTitle(this, APlaylist);

            //backgroundworker
            InitBackgroundWorker();


            //Variables for Totaltimecalculation
            _oldcount = 0;
            _overallLength = 0;

            try
            {
                TrayIcon();
            }
            catch (Exception)
            {
                new Error("Trayicon could not be initialized", false, null);
            }

            SaveTmpSettings tmpsets;

            APlaylist.aLoadingobserver = _aLoadingTitle;

            //Add Featured songs
            AddFeaturedSongs();

            if (File.Exists(AConfiguration.PathTmpPlaylist))
                APlaylist.loadPL(AConfiguration.PathTmpPlaylist);

            try
            {
                if (File.Exists(AConfiguration.PathTmpSettings))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(SaveTmpSettings));
                    StreamReader sr = new StreamReader(AConfiguration.PathTmpSettings);
                    tmpsets = (SaveTmpSettings)ser.Deserialize(sr);

                    ABassWrapperGui.setBalance(tmpsets.Balance);
                    ABassWrapperGui.setVolume(tmpsets.Volume);
                    APlayControler.SetEqualizerAll(tmpsets.Equalizer);
                    ABassWrapperGui.settoggleEQ(tmpsets.Equalizerstate);
                    labelPlaylistName.Content = tmpsets.Plname;
                    APlaylist.setPlayMode(tmpsets.Playmode);
                    _currentVis = tmpsets.Visual;
                    _lastOpenedFolderpath = tmpsets.LastOpenedFolderPath;

                    // set Eq window
                    if (tmpsets.IsEq)
                        showEq.IsChecked = true;

                    // set Pl window
                    if (tmpsets.IsPl)
                        showPl.IsChecked = true;

                    // set last width and height
                    Window.Width = tmpsets.GnWidth;
                    Window.Height = tmpsets.GnHeight;

                    // jump to last active song
                    APlaylist.setNowPlayingPosition(tmpsets.SongIndex);

                    // only Play if checkbox in settings is enabled
                    if (AConfiguration.ResumePlayback && tmpsets.Playing)
                    {
                        APlayControler.Play();
                        if (tmpsets.SongPosition != 0)
                            ABassWrapperGui.setPlayPosition(tmpsets.SongPosition);
                    }

                    _aVisualization.Reset();
                    sr.Close();
                }

            }
            catch (IOException)
            { }
            catch (InvalidOperationException)
            { }
            catch (ArgumentException)
            { }

            // Playmode
            // PlaymodeNormal = 0, PlaymodeRandom = 1, PlaymodeRepeatSong = 2, PlaymodeRepeatList = 3
            switch (APlaylist.getPlayMode())
            {
                case ((int)PlayModeEnum.PlaymodeNormal):
                    radioButtonModeNormal.IsChecked = true;
                    break;

                case ((int)PlayModeEnum.PlaymodeRandom):
                    radioButtonModeShuffle.IsChecked = true;
                    break;

                case ((int)PlayModeEnum.PlaymodeRepeatList):
                    radioButtonModeRepeatAll.IsChecked = true;
                    break;

                case ((int)PlayModeEnum.PlaymodeRepeatSong):
                    radioButtonModeRepeat.IsChecked = true;
                    break;
            }

        }

        /// <summary>
        /// shut down the Window, called from buttonCloseClick
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Window_Closed(object sender, EventArgs e)
        {


            APlaylist.stopRequest = true;
            APlaylist.savePL(AConfiguration.PathTmpPlaylist);

            // is Eq shown
            bool isEq = false;
            if (showEq.IsChecked == true)
                isEq = true;

            // is Pl shown
            bool isPl = false;
            if (showPl.IsChecked == true)
                isPl = true;

            // get current width and heigt
            double playerWidth = ActualWidth;
            double playerHeight = ActualHeight;

            // active song in playlist
            int currentSongIndex = APlaylist.getNowPlayingPosition();

            bool isPlay = false;
            double currentSongPosition = 0;

            // if a song is played and is not paused
            if (APlayControler.GetPlaying() && !ABassWrapperGui.getPaused())
            {
                // song plays, so get elapsed time
                currentSongPosition = ABassWrapperGui.getElapsedTime();
                isPlay = true;
            }

            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(SaveTmpSettings));
                FileStream str = new FileStream(AConfiguration.PathTmpSettings, FileMode.Create);
                SaveTmpSettings save = new SaveTmpSettings(ABassWrapperGui.getEqualizer(),
                    ABassWrapperGui.gettoggleEQ(), ABassWrapperGui.getVolume(),
                    ABassWrapperGui.getBalance(), (string)labelPlaylistName.Content,
                    APlaylist.getPlayMode(), _currentVis, _lastOpenedFolderpath,
                    isEq, isPl, playerWidth, playerHeight, currentSongIndex, currentSongPosition, isPlay);

                ser.Serialize(str, save);

                str.Close();
            }
            catch (InvalidOperationException)
            { }
            catch (ArgumentException)
            { }
            catch (NotSupportedException)
            { }
            catch (System.Security.SecurityException)
            { }
            catch (FileNotFoundException)
            { }
            catch (DirectoryNotFoundException)
            { }
            catch (PathTooLongException)
            { }
            catch (IOException)
            { }

            System.Windows.Application.Current.Shutdown();


            _notifyIcon1.Dispose();


            ABassWrapperGui.setVolume(0);
        }

        #endregion

        #region PluginMethods
        /// <summary>
        /// Initializes the avaiable Plugins
        /// </summary>
        private void InitPlugins()
        {
            //Find all Plugins
            APluginManager.FindPlugins(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Plugins", this);

            //Show all unloadable Plugins
            IAvailablePlugins unloadedPlugins = APluginManager.GetUnloadedPlugins();
            if (unloadedPlugins.Count > 0)
            {
                string errMsg = "Can't load following plugins:" + Environment.NewLine;
                foreach (AvailablePlugin plugin in unloadedPlugins)
                {
                    errMsg += plugin.Instance.Name + Environment.NewLine;
                }
                new Error(errMsg, false, null);
            }


            //Add Menuitems
            foreach (AvailablePlugin plugin in APluginManager.AvailablePlugins)
            {
                System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem
                                                            {
                                                                Header = plugin.Instance.Name,
                                                                Style = (Style)FindResource("contextMenuItem")
                                                            };
                item.Click += PluginMenuItemClick;
                GridWrapperContextPlugins.Items.Add(item);
            }

            //Add Menuitems
            foreach (AvailablePlugin plugin in APluginManager.AvailablePlugins)
            {
                System.Windows.Controls.MenuItem item = new System.Windows.Controls.MenuItem
                {
                    Header = plugin.Instance.Name,
                    Style = (Style)FindResource("contextMenuItem")
                };
                item.Click += PluginMenuItemClick;
                pluginContextMenu.Items.Add(item);
            }
        }

        void PluginMenuItemClick(object sender, RoutedEventArgs e)
        {
            String pluginName = ((System.Windows.Controls.MenuItem)sender).Header.ToString();

            ShowPlugin(pluginName);
        }

        private void ShowPlugin(String name)
        {
            //If the Window is still open, push it in the foreground
            if (_dictPluginWindows.ContainsKey(name))
            {
                _dictPluginWindows[name].Focus();
                return;
            }

            //Set the dockstyle of the plugin to fill, to fill up the space provided
            //plugin.Instance.MainInterface. = DockStyle.Fill;

            //Find the plugin on which was clicked
            if (APluginManager.GetPluginByName(name) == null)
            {
                new Error("Can't find Plugin " + name, false, null);
            }
            else
            {
                Plugin aPluginWindow = new Plugin(APluginManager.GetPluginByName(name).Instance);

                // apply skin for plugins
                aPluginWindow.ApplyPluginSkin(_currentSkin, AConfiguration, _anAppHandle);
                aPluginWindow.Closed += APluginWindowClosed;

                _dictPluginWindows.Add(name, aPluginWindow);
                aPluginWindow.Show();
            }

        }

        void APluginWindowClosed(object sender, EventArgs e)
        {
            _dictPluginWindows.Remove(((Plugin)sender).GetPlugin().Name);
        }

        #endregion

        #region HelperAndMisc

        /// <summary>
        /// Add the features songs automatically to the playlist
        /// </summary>
        private void AddFeaturedSongs()
        {
            if (File.Exists(AConfiguration.PathFeaturedSongsFlag) && Directory.Exists(AConfiguration.PathFeaturedSongs))
            {
                APlaylist.addFolder(AConfiguration.PathFeaturedSongs);
                File.Delete(AConfiguration.PathFeaturedSongsFlag);
            }
        }

        /// <summary>
        /// Sets the Color for the currently played Song 
        /// </summary>
        private void ColorCurrentPlayedSong()
        {
            // color is set in controls.xaml by set the listViewItem _tags
            // 1 = current Playing song
            // 0 = not Playing

            if (APlaylist.getCurrentSong() != null && !APlayControler.GetValid())
            {
                ListViewItem lvBad = GetListViewItem(APlaylist.getNowPlayingPosition()-1);
                ListViewItem lvNext = GetListViewItem(APlaylist.getNowPlayingPosition());
                if (lvBad != null)
                    lvBad.Tag = "2";
                if (lvNext != null)
                    lvNext.Tag = "1";
                    
            }
            else
            {
                
                if (_currentHighlightedSong != -1)
                {
                    ListViewItem lv1 = GetListViewItem(_currentHighlightedSong);

                    if (lv1 != null && (String)lv1.Tag != "2")
                        lv1.Tag = "0";
                }
                _currentHighlightedSong = APlaylist.getNowPlayingPosition();
                ListViewItem lv2 = GetListViewItem(_currentHighlightedSong);

                if (lv2 != null)
                    lv2.Tag = "1";
            }
        }

        /// <summary>
        /// Add Files And Folders to Playlist
        /// </summary>
        /// <param name="filePaths">FilePaths and Files to Add</param>
        /// <param name="autoPlay"></param>
        public void AddFilesAndFolders(string[] filePaths, bool autoPlay)
        {
            //Will be used for autoPlay
            int posOfFirstSong = -1;

            if (_insertFolder)
            {
                new Error("You are already adding some Files. Please be patient and try again later.", false, this);
                return;
            }

            Collection<string> folders = new Collection<string>();
            APlaylist.stopRequest = false;
            APlaylist.aLoadingobserver = _aLoadingTitle;

            foreach (string fileLoc in filePaths)
            {
                if (File.Exists(fileLoc))
                {
                    int pos = posOfFirstSong = APlaylist.addSong(fileLoc, 1);
                    if (posOfFirstSong == -1)
                        posOfFirstSong = pos;
                }

                if (Directory.Exists(fileLoc))
                {
                    folders.Add(fileLoc);
                }
            }
            APlaylist.regenerateList();
            APlaylist.stopRequest = true;

            if (folders.Count != 0)
            {
                APlaylist.DispatcherThread = Dispatcher;
                string[] folds = folders.ToArray();
                APlaylist.startPLThreadfolders(folds);
                try
                {
                    _aLoadingTitle.Show();
                }
                catch (InvalidOperationException)
                { }

                _insertFolder = true;
            }

            if (!autoPlay || posOfFirstSong == -1)
                return;

            APlayControler.Stop();
            APlaylist.setNowPlayingPosition(posOfFirstSong);
            APlayControler.Play();
        }

        /// <summary>
        /// Apply a skin to the form
        /// </summary>
        /// <param name="skinName">Name of the skin</param>
        void ApplySkin(String skinName)
        {

            //Catch all current styles
            Collection<ResourceDictionary> mergedDicts = Resources.MergedDictionaries;

            // Remove the existing skin dictionary, if one exists.
            if (mergedDicts.Count > 0)
            {
                //Everytime clearing the Dictionary the exception will occur
                //But the skins will be cleared anyway
                try
                {
                    mergedDicts.Clear();
                }
                catch (InvalidOperationException) { }
            }

            try
            {
                // Add standard style
                StreamReader sReaderStandard = new StreamReader(AConfiguration.GetSkinPath() + "Simple Styles.xaml");
                ResourceDictionary standardDict = XamlReader.Load(sReaderStandard.BaseStream) as ResourceDictionary;
                mergedDicts.Add(standardDict);

                //Get all files in the directory
                string[] files = Directory.GetFiles(AConfiguration.GetSkinPath() + skinName);
                foreach (string f in files)
                {
                    
                    //Check for invalid files
                    if (!f.EndsWith(".xaml"))
                        continue;

                    if (f.IndexOf('~') != -1)
                        continue;

                    //create Stream
                    StreamReader sReader = new StreamReader(f);

                    //Crating a ResourceDictionary of the stream
                    ResourceDictionary skinDict = XamlReader.Load(sReader.BaseStream) as ResourceDictionary;


                    // Add the new Dictionary
                    mergedDicts.Add(skinDict);
                }

                //refresh Visualization
                _aVisualization.Reset();
            }
            catch (DirectoryNotFoundException)
            {
                new Error("Skin Dictionary Missing", false, null);
            }
            catch (ArgumentNullException)
            {
                new Error("Error ApplySkin: Null Argument", false, null);
            }
            catch (ArgumentException)
            {
                new Error("Error ApplySkin: Argument Error", false, null);
            }
            catch (FileNotFoundException)
            {
                new Error("Skin file not found", false, null);
            }
            catch (IOException)
            {
                new Error("Error ApplySkin: IO Error", false, null);
            }
        }

        /// <summary>
        /// Timer for the different scrolling Texts
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">EventArgs</param>
        private void ClockScrollingTextTick(Object sender, EventArgs e)
        {
            if (APlaylist == null)
                return;

            //Scroll Album
            if (APlaylist.getCurrentSong() != null)
                UserInterfaceHelper.ScrollText(labelAlbum, _scrollDataAlbum, scrollAlbum, APlaylist.getCurrentSong().getTags()[0], " :: ",
                    ScrollWaitTime, ScrollSteps);
            //Scroll Artist
            if (APlaylist.getCurrentSong() != null)
                UserInterfaceHelper.ScrollText(labelArtist, _scrollDataArtist, scrollArtist, APlaylist.getCurrentSong().artist, " :: ",
                    ScrollWaitTime, ScrollSteps);

            //Scroll Title
            if (APlaylist.getCurrentSong() != null)
                UserInterfaceHelper.ScrollText(labelTitle, _scrollDataTitle, scrollTitle, APlaylist.getCurrentSong().title, " :: ",
                    ScrollWaitTime, ScrollSteps);
        }

        /// <summary>
        /// Update Method that is called to Update the Observer Standardt when Modell has changed
        /// </summary>
        /// <param name="subject">Changed Subject</param>
        public void Update(object subject)
        {
            if (subject is Configuration)
            {
                RefreshHotkeys();
                _overlayTimer.IsEnabled = ((Configuration)subject).OverlayTimerEnabled;
                _overlayTimer.Interval = TimeSpan.FromSeconds(((Configuration)subject).OverlayTimerSeconds);

                if (_currentSkin != ((Configuration)subject).UsedSkin)
                {
                    _currentSkin = ((Configuration)subject).UsedSkin;
                    ApplySkin(_currentSkin);
                }
            }
            else if (subject is GameNoiseList || subject is Song)
            {
                if (APlaylist.getCurrentSong() != null)
                {
                    scrollArtist.ScrollToLeftEnd();
                    scrollAlbum.ScrollToLeftEnd();
                    scrollTitle.ScrollToLeftEnd();

                    string artistText = APlaylist.getCurrentSong().getTags()[1];
                    string albumText = APlaylist.getCurrentSong().getTags()[0];
                    string titleText = APlaylist.getCurrentSong().getTags()[2];

                    labelArtist.Content = artistText;
                    labelAlbum.Content = albumText;
                    labelTitle.Content = titleText;

                    UserInterfaceHelper.InitScrollText(_scrollDataArtist, artistText);
                    UserInterfaceHelper.InitScrollText(_scrollDataAlbum, albumText);
                    UserInterfaceHelper.InitScrollText(_scrollDataTitle, titleText);

                    Window.Title = artistText + " - " + titleText + " | gamenoise";
                    int index = APlaylist.getNowPlayingPosition();

                    try
                    {
                        listView.ScrollIntoView(listView.Items.GetItemAt(index));
                    }
                    catch (ArgumentOutOfRangeException)
                    { }
                    catch (InvalidOperationException)
                    { }

                    if (_notifyIcon1 != null)
                    {
                        if ((artistText + " - " + titleText).Length > 64)
                            _notifyIcon1.Text = (artistText + " - " + titleText).Substring(0, 63);
                        else
                            _notifyIcon1.Text = (artistText + " - " + titleText);
                    }

                    ColorCurrentPlayedSong();
                }

                //Totaltime Calculation
                int newcount = APlaylist.Playlist.Count;


                if ((_oldcount != newcount && !_insertFolder) || (_overallLength == 0 && newcount != 0))
                {

                    try
                    {
                        _overallLength = 0;
                        for (int i = 0; i < newcount; i++)
                            _overallLength += APlaylist.Playlist[i].dlength;

                        _oldcount = newcount;
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        _oldcount = APlaylist.Playlist.Count;
                    }
                }

                labelTotalTimeTime.Content = ((int)(_overallLength / 3600 / 24)) + " Days " +
                            ((int)(_overallLength / 3600 % 24)).ToString("00;0#") + ":" + ((int)(_overallLength / 60 % 60)).ToString("00;0#") + ":"
                            + ((int)(_overallLength % 60)).ToString("00;0#");
            }
            else if (subject is BassWrapper)
            {
                if (((BassWrapper)subject).getPaused())
                {
                    // playcontrol buttons state
                    buttonPause.Content = "Play";
                    buttonPause.IsChecked = false;

                    // context menu state
                    GridWrapperContextMenuPlay.IsChecked = false;
                    GridWrapperContextMenuStop.IsChecked = false;

                    //clockVis.IsEnabled = false;
                }
                else
                {
                    if (((BassWrapper)subject).getstopped())
                    {
                        // playcontrol buttons state
                        buttonPause.IsChecked = false;

                        // context menu state
                        GridWrapperContextMenuPlay.IsChecked = false;
                        GridWrapperContextMenuStop.IsChecked = true;
                    }
                    else
                    {
                        // playcontrol buttons state
                        buttonPause.IsChecked = true;

                        // context menu state
                        GridWrapperContextMenuPlay.IsChecked = true;
                        GridWrapperContextMenuStop.IsChecked = false;
                        buttonPause.Content = "Pause";
                    }
                }


                if (APlayControler.GetPlaying())
                {
                    buttonStop.IsChecked = false;
                    GridWrapperContextMenuStop.IsChecked = false;
                }

                double time = ((BassWrapper)subject).getElapsedTime();
                double totaltime = ((BassWrapper)subject).getTotalTime();
                string timeString = ((int)(time / 60)).ToString("0;0#") + ":" + ((int)(time % 60)).ToString("00;0#");
                string totaltimeString = ((int)(totaltime / 60)).ToString("0;0#") + ":" + ((int)(totaltime % 60)).ToString("00;0#");


                // label for minutes
                labelTimeM.Content = ((int)(time / 60)).ToString("0;0#");

                // labels for tens seconds
                labelTimeST.Content = ((int)(((time % 60) * 10) / 100)).ToString("0;#");

                // labels for ones seconds
                labelTimeSO.Content = ((int)((time % 60) % 10)).ToString("0;#");


                // small time under the trackbar
                string showAllTime = timeString + " / " + totaltimeString;
                labelTimeInfo.Content = showAllTime;

                // tooltip for song progress
                changeSeekBar.ToolTip = showAllTime;

                // Volume label
                // "System.Globalization.CultureInfo.InvariantCulture" is for . instead of ,
                // labelVolume.Content = Math.Round(((((BassWrapper)subject).getVolume() - 100)), 2).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                // ********
                // is now % instead of db
                // labelVolume.Content = Math.Round(((((BassWrapper)subject).getVolume())), 2);
                labelVolume.Content = (int)((BassWrapper)subject).getVolume();


                if (_changeSeekBarDragged == false)
                    changeSeekBar.Value = (int)((time / totaltime) * 100);

                //Volume & Balance Bar
                changeBalance.Value = ((BassWrapper)subject).getBalance();
                changeVolumeBar.Value = ((BassWrapper)subject).getVolume();


                buttonEqualizerState.IsChecked = ((BassWrapper)subject).gettoggleEQ();
                float[] eqList = ((BassWrapper)subject).getEqualizer();

                sliderEQBand0.Value = eqList[0];
                sliderEQBand1.Value = eqList[1];
                sliderEQBand2.Value = eqList[2];
                sliderEQBand3.Value = eqList[3];
                sliderEQBand4.Value = eqList[4];
                sliderEQBand5.Value = eqList[5];
                sliderEQBand6.Value = eqList[6];
                sliderEQBand7.Value = eqList[7];
                sliderEQBand8.Value = eqList[8];
                sliderEQBand9.Value = eqList[9];

                sliderEQBand0.ToolTip = (float)sliderEQBand0.Value;
                sliderEQBand1.ToolTip = (float)sliderEQBand1.Value;
                sliderEQBand2.ToolTip = (float)sliderEQBand2.Value;
                sliderEQBand3.ToolTip = (float)sliderEQBand3.Value;
                sliderEQBand4.ToolTip = (float)sliderEQBand4.Value;
                sliderEQBand5.ToolTip = (float)sliderEQBand5.Value;
                sliderEQBand6.ToolTip = (float)sliderEQBand6.Value;
                sliderEQBand7.ToolTip = (float)sliderEQBand7.Value;
                sliderEQBand8.ToolTip = (float)sliderEQBand8.Value;
                sliderEQBand9.ToolTip = (float)sliderEQBand9.Value;

                //ColorCurrentPlayedSong();
            }
        }

        #endregion

        #region BackgroundWorker
        /// <summary>
        /// Initialize the Background Worker
        /// </summary>
        private void InitBackgroundWorker()
        {
            _backgroundWorker1 = new BackgroundWorker { WorkerReportsProgress = true };
            _backgroundWorker1.DoWork += BackgroundWorker1DoWork;

            _backgroundWorker1.RunWorkerAsync();

            _backgroundWorker1.ProgressChanged += BackgroundWorker1ProgressChanged;


            _overlayTimer = new DispatcherTimer();
            _overlayTimer.Tick += OverlayTimerTick;
            _overlayTimer.Interval = TimeSpan.FromSeconds(AConfiguration.OverlayTimerSeconds);
            _overlayTimer.IsEnabled = AConfiguration.OverlayTimerEnabled;
        }


        /// <summary>
        /// Backgourndworker which ticks the Vis
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void BackgroundWorker1DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                System.Threading.Thread.Sleep(25);
                _backgroundWorker1.ReportProgress(0);
            }
            // ReSharper disable FunctionNeverReturns
        }
        // ReSharper restore FunctionNeverReturns

        /// <summary>
        /// Is called when backgroundworker reports tick
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        private void BackgroundWorker1ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ClockVisTick();
        }

        /// <summary>
        /// Clock for the visualization. Every tick the visualization will be drawed
        /// </summary>
        private void ClockVisTick()
        {
            System.Drawing.ColorConverter trans = new System.Drawing.ColorConverter();

            if (mainWindow.Fill == null)
                return;


            System.Drawing.Color back = (System.Drawing.Color)trans.ConvertFromString(mainWindow.Fill.ToString());

            switch (_currentVis)
            {

                case (1):
                    _aVisualization.CreateSpectrumLine(new Visualization.SpectrumLineParams(canvasVis, ABassWrapperGui.getFFTData(), 110, 50, System.Drawing.Color.Orange, System.Drawing.Color.OrangeRed, back, 10, 2, false, true, 3, 1, true));
                    break;
                case (2):
                    _aVisualization.CreateSpectrumLine(new Visualization.SpectrumLineParams(canvasVis, ABassWrapperGui.getFFTData(), 110, 50, System.Drawing.Color.Orange, System.Drawing.Color.Orange, back, 2, 0, false, false, 3, 1, true));
                    break;
                case (3):
                    _aVisualization.CreateSpectrumLine(new Visualization.SpectrumLineParams(canvasVis, ABassWrapperGui.getFFTData(), 110, 50, System.Drawing.Color.Orange, System.Drawing.Color.Orange, back, 1, 0, true, false, 3, 1, true));
                    break;
                case (0):
                case (4):
                    _aVisualization.CreateSpectrumLine(new Visualization.SpectrumLineParams(canvasVis, ABassWrapperGui.getFFTData(), 110, 50, System.Drawing.Color.Orange, System.Drawing.Color.Orange, back, 1, 0, true, false, 3, 1, true));
                    canvasVis.Children.Clear();
                    break;
            }
        }

        private void OverlayTimerTick(object sender, EventArgs e)
        {
            _overlayTimer.Stop();
            _overlayTimer.IsEnabled = true;
            checkboxIngameShow.IsChecked = false;

        }

        #endregion

        #region ControllerMethods
        /// <summary>
        /// Calls the Stop function of the PlayControler to Stop the song
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonStopClick(object sender, EventArgs e)
        {
            try
            {
                APlayControler.Stop();
                buttonPause.IsChecked = false;
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }

        }

        /// <summary>
        /// starts a song, if already running Pause and unpause if paused
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonPlayPauseClick(object sender, EventArgs e)
        {
            if (listView.Items.Count == 0)
            {
                buttonOpenFileClick(sender, e);
                buttonPause.IsChecked = false;
            }
            else
            {
                try
                {
                    APlayControler.Play();
                }
                catch (BassWrapperException ex)
                {
                    new Error(ex.GetMessage(), false, this);
                }
                catch (NullReferenceException)
                { }

            }
        }

        /// <summary>
        /// shows the settings window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonSettingsClick(object sender, EventArgs e)
        {
            _aSettings = new Settings(AConfiguration, APluginManager);
            _aSettings.ShowDialog();
        }

        /// <summary>
        /// shows the settings window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonPluginsClick(object sender, EventArgs e)
        {
            pluginContextMenu.PlacementTarget = this;
            pluginContextMenu.IsOpen = true;
        }

        /// <summary>
        /// calls Shutdown to close the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonCloseClick(object sender, EventArgs e)
        {
            // Dont call this.Window_Closed(sender, e);
            // because then, Window_Closed will be called twice
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// minimize the window
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonMinimizeClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// calls the function PreviousSong of PlayControler to Play the previous song
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonPrevClick(object sender, EventArgs e)
        {
            try
            {
                APlayControler.PreviousSong();
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
            catch (NullReferenceException)
            { }

        }

        /// <summary>
        /// calls the function NextSong of PlayControler to Play the next song
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonNextClick(object sender, EventArgs e)
        {
            try
            {
                APlayControler.NextSong();
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
            catch (NullReferenceException)
            { }
        }

        /// <summary>
        /// Playmode Button Normal
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonModeNormal_Checked(object sender, RoutedEventArgs e)
        {
            APlaylist.setPlayMode(0);
        }

        /// <summary>
        /// Playmode Button Shuffle
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonModeShuffle_Checked(object sender, RoutedEventArgs e)
        {
            APlaylist.setPlayMode(1);
        }

        /// <summary>
        /// Playmode Button Repeat
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonModeRepeat_Checked(object sender, RoutedEventArgs e)
        {
            APlaylist.setPlayMode(2);
        }

        /// <summary>
        /// Playmode Button Repeatall
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonModeRepeatAll_Checked(object sender, RoutedEventArgs e)
        {
            APlaylist.setPlayMode(3);
        }

        /// <summary>
        /// slider event to change the Volume
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void changeVolumeBarSlide(object sender, EventArgs e)
        {
            ABassWrapperGui.setVolume((float)changeVolumeBar.Value);
        }

        /// <summary>
        /// Change the Seekbar Value
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void changeSeekBarSlide(object sender, EventArgs e)
        {
            if (!ABassWrapperGui.setPlayPosition((changeSeekBar.Value / 100) * ABassWrapperGui.getTotalTime()))
                new Error("Error changing Seek Bar", false, this);

            _changeSeekBarDragged = false;
        }

        /// <summary>
        /// Open File Dialog Button
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonOpenFileClick(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
                                     {
                                         Multiselect = true,
                                         Filter =
                                             "Audio Files (*.mp3, *.wav, *.ogg, *.mp1, *.mp2, *.aiff)|*.mp3;*.wav;*.ogg;*.mp1;*.mp2;*.aiff|All Files|*.*"
                                     };


            if (ofd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            APlaylist.stopRequest = false;
            string[] filePath = ofd.FileNames;
            APlaylist.aLoadingobserver = _aLoadingTitle;

            foreach (string song in filePath)
                APlaylist.addSong(song, 1);

            APlaylist.regenerateList();
            APlaylist.stopRequest = true;
        }

        /// <summary>
        /// Open Folder Dialog Button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOpenFolderClick(object sender, EventArgs e)
        {
            APlaylist.DispatcherThread = Dispatcher;
            FolderBrowserDialog fbd = new FolderBrowserDialog { SelectedPath = _lastOpenedFolderpath };

            if (_insertFolder)
            {
                new Error("You are already adding a FilePath. Please be patient and try again later", false, this);
                return;
            }

            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            string folderpath = fbd.SelectedPath;
            _lastOpenedFolderpath = folderpath;
            APlaylist.aLoadingobserver = _aLoadingTitle;
            APlaylist.startPLThread(folderpath);


            _aLoadingTitle.Show();
            _insertFolder = true;
        }

        /// <summary>
        /// creates a new playlist
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonNewPlaylistClick(object sender, EventArgs e)
        {
            _aYesNoDialog = new YesNo(this);

            if (_aYesNoDialog.ShowDialog() != true) return;

            APlaylist.deleteAllSongs();
            labelPlaylistName.Content = "- no name";
        }

        /// <summary>
        /// opens a playlist
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonOpenPlaylistClick(object sender, EventArgs e)
        {
            APlaylist.DispatcherThread = Dispatcher;

            var opd = new OpenFileDialog
                          {
                              Multiselect = false,
                              RestoreDirectory = true,
                              Title = "Load Playlist",
                              Filter = "Playlist Files (*.gnl)|*.gnl"
                          };

            if (opd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            APlaylist.deleteAllSongs();
            string filepath = opd.FileName;

            try
            {
                APlaylist.startXMLThread(filepath);
                _currentPlaylist = Path.GetFileNameWithoutExtension(filepath);
                labelPlaylistName.Content = "- " + _currentPlaylist;
            }
            catch (PlaylistException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
        }

        /// <summary>
        /// saves a playlist
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonSavePlaylistClick(object sender, EventArgs e)
        {
            var spd = new SaveFileDialog
                          {
                              DefaultExt = "gnl",
                              AddExtension = true,
                              RestoreDirectory = true,
                              Title = "Save Playlist",
                              Filter = "Playlist Files (*.gnl)|*.gnl"
                          };

            if (spd.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

            string filepath = spd.FileName;
            APlaylist.savePL(filepath);
            _currentPlaylist = Path.GetFileNameWithoutExtension(filepath);
            labelPlaylistName.Content = "- " + _currentPlaylist;
        }

        /// <summary>
        /// switch Equalizer on
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonEqualizerState_Checked(object sender, RoutedEventArgs e)
        {
            ABassWrapperGui.settoggleEQ(true);

            // set context menu state
            GridWrapperContextMenuEqOnOff.IsChecked = true;
            if (_onToolStripMenuItem != null)
                _onToolStripMenuItem.Checked = true;

        }

        /// <summary>
        /// switch Equalizer off
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonEqualizerState_Unchecked(object sender, RoutedEventArgs e)
        {
            ABassWrapperGui.settoggleEQ(false);

            // set context menu state
            GridWrapperContextMenuEqOnOff.IsChecked = false;
            _onToolStripMenuItem.Checked = false;
        }

        /// <summary>
        /// switch Equalizer on/off for trayicon
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void ButtonEqualizerStateForTrayIcon(object sender, EventArgs e)
        {
            if (ABassWrapperGui.gettoggleEQ())
            {
                ABassWrapperGui.settoggleEQ(false);
                GridWrapperContextMenuEqOnOff.IsChecked = false;
                _onToolStripMenuItem.Checked = false;
            }
            else
            {
                ABassWrapperGui.settoggleEQ(true);
                GridWrapperContextMenuEqOnOff.IsChecked = true;
                _onToolStripMenuItem.Checked = true;
            }
        }

        /// <summary>
        /// shows a text input box to save the name of the EQ setting
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonSaveEQClick(object sender, EventArgs e)
        {
            _aSaveEq = new SaveEQ(APlayControler, this);
            _aSaveEq.ShowDialog();
        }

        /// <summary>
        /// shows a list with all available EQ settings.
        /// can be selected by double clicking
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonLoadEQClick(object sender, EventArgs e)
        {
            _aLoadEq = new LoadEQ(APlayControler, this);
            _aLoadEq.ShowDialog();
        }

        /// <summary>
        /// resets all the Equalizer Bands to 0
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonResetEQClick(object sender, EventArgs e)
        {
            float[] eqList = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            APlayControler.SetEqualizerAll(eqList);
        }

        /// <summary>
        /// shows overlay
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void checkboxIngameShowChecked(object sender, RoutedEventArgs e)
        {
            if (AIngame == null)
            {
                new Error("Ingame ist deaktiviert!", false, this);
            }
            else
            {
                AIngame.Show();
                if (_overlayTimer.IsEnabled)
                {
                    _overlayTimer.Stop();
                    _overlayTimer.Start();
                }
            }
        }

        /// <summary>
        /// Hides Overlay
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void checkboxIngameShowUnChecked(object sender, RoutedEventArgs e)
        {
            if (AIngame == null)
            {
                new Error("Ingame ist deaktiviert!", false, this);
            }
            else
            {

                if (AIngame.IsVisible())
                    AIngame.Hide();
            }
        }

        /// <summary>
        /// slider to change the Balance
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void changeBalanceSlide(object sender, EventArgs e)
        {
            if ((changeBalance.Value <= 0.2 && changeBalance.Value > 0) || (changeBalance.Value >= -0.2 && changeBalance.Value < 0))
                changeBalance.Value = 0;

            try
            {
                ABassWrapperGui.setBalance((float)changeBalance.Value);
            }
            catch (BassWrapperException)
            { }

        }

        /// <summary>
        /// Drag the Window by just click on whitespace and drag
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// Double Click Action on ListView
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">Position of Mouseclick</param>
        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int width = SystemInformation.VerticalScrollBarWidth;
            System.Windows.Point mousePos = e.GetPosition(listView);
            if (mousePos.X >= (listView.ActualWidth - width))
                return;

            try
            {
                APlaylist.setNowPlayingPosition(listView.SelectedIndex);
                APlayControler.Stop();
                APlayControler.Play();
            }
            catch (ArgumentOutOfRangeException)
            {
                APlaylist.setNowPlayingPosition(0);
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
            catch (NullReferenceException)
            { }
        }

        /// <summary>
        /// Event Handler for Dragging the Seekbar Thumb
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void changeSeekBar_DragStarted(object sender, DragStartedEventArgs e)
        {
            _changeSeekBarDragged = true;
        }

        /// <summary>
        /// Event Handler for clicking in the seekbar,
        /// Left Button Down end the event
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void ChangeSeekBarMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            try
            {
                ABassWrapperGui.setPlayPosition((changeSeekBar.Value / 100) * ABassWrapperGui.getTotalTime());
            }
            catch (BassWrapperException)
            { }
        }

        /// <summary>
        /// Event Handler for clicking in the Volume bar,
        /// Left Button Down starts the event
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void ChangeVolumeBarMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            ABassWrapperGui.setVolume((float)changeVolumeBar.Value);
        }

        /// <summary>
        /// Event Handler to drag and set the EQ Bands
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void sliderEQBand_DragCompleted(object sender, EventArgs e)
        {
            float[] tmp = new float[10];
            tmp[0] = (float)sliderEQBand0.Value;
            tmp[1] = (float)sliderEQBand1.Value;
            tmp[2] = (float)sliderEQBand2.Value;
            tmp[3] = (float)sliderEQBand3.Value;
            tmp[4] = (float)sliderEQBand4.Value;
            tmp[5] = (float)sliderEQBand5.Value;
            tmp[6] = (float)sliderEQBand6.Value;
            tmp[7] = (float)sliderEQBand7.Value;
            tmp[8] = (float)sliderEQBand8.Value;
            tmp[9] = (float)sliderEQBand9.Value;

            APlayControler.SetEqualizerAll(tmp);
        }

        /// <summary>
        /// Window Size Changed Event Handler
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Window_SizeChanged(object sender, EventArgs e)
        {
            if (showPl.IsChecked == true) SizedWidth = ActualWidth;
        }

        /// <summary>
        /// Restore listView Window after Performence Help
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void showPl_Checked(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.CanResizeWithGrip;
            listView.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Hide listView for more Performence
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void showPl_Unchecked(object sender, RoutedEventArgs e)
        {
            ResizeMode = ResizeMode.NoResize;
            listView.Visibility = Visibility.Hidden;
            textBoxSearch.Text = "";
        }

        /// <summary>
        /// Switches the visualizations mode
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void canvasVis_MouseClick(object sender, MouseButtonEventArgs e)
        {
            _currentVis++;

            if (_currentVis > 4)
                _currentVis = 1;

            _aVisualization.Reset();
        }

        /// <summary>
        /// Fits the listview rows to the actual size
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">SizeChangedEventArgs</param>
        private void listView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            gv.Columns[1].Width = UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView).ActualWidth < 200 ? 0 : 40;

            if (UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView).ActualWidth - 15 - gv.Columns[1].Width < 0)
                gv.Columns[0].Width = 0;
            else
                gv.Columns[0].Width = UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView).ActualWidth - 15 - gv.Columns[1].Width;
        }

        /// <summary>
        /// Same as listView
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void searchView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            searchgv.Columns[1].Width = UserInterfaceHelper.GetVisualChild<ScrollViewer>(searchView).ActualWidth < 200 ? 0 : 40;

            if (UserInterfaceHelper.GetVisualChild<ScrollViewer>(searchView).ActualWidth - 15 - searchgv.Columns[1].Width < 0)
                searchgv.Columns[0].Width = 0;
            else
                searchgv.Columns[0].Width = UserInterfaceHelper.GetVisualChild<ScrollViewer>(searchView).ActualWidth - 15 - searchgv.Columns[1].Width;
        }

        /// <summary>
        /// Deletes the selected items of the listView if del is pressed
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void listView_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                MenuItemDelete_Click(null, null);
        }

        /// <summary>
        /// Handler for MenuItems
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void MenuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            IList items = listView.SelectedItems;

            if (items.Count == PlayListRefProp.Count)
            {
                APlaylist.deleteAllSongs();
            }
            else
            {
                try
                {
                    while (items.Count != 0)
                    {
                        System.Windows.Forms.Application.DoEvents();
                        APlaylist.deleteSong(listView.Items.IndexOf(items[0]));
                    }
                }
                catch (ArgumentOutOfRangeException)
                { //do nothing
                }
            }
        }

        /// <summary>
        /// Scrolls the text
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">MouseWheelEventArgs</param>
        private void labelArtist_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollArtist.ScrollToHorizontalOffset(scrollArtist.HorizontalOffset + ScrollSteps * 3);
            _scrollDataArtist.ScrollPreTicks = 0;
        }

        /// <summary>
        /// Adjusts the scroll text
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ScrollChangedEventArgs</param>
        private void scrollArtist_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UserInterfaceHelper.ScrollAdjust(scrollArtist, labelArtist, _scrollDataArtist);
        }

        /// <summary>
        /// Scrolls the text
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">MouseWheelEventArgs</param>
        private void scrollTitle_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UserInterfaceHelper.ScrollAdjust(scrollTitle, labelTitle, _scrollDataTitle);
        }

        /// <summary>
        /// Scrolls the text
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">MouseWheelEventArgs</param>
        private void scrollAlbum_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UserInterfaceHelper.ScrollAdjust(scrollAlbum, labelAlbum, _scrollDataAlbum);
        }

        /// <summary>
        /// Adjusts the scroll text
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">ScrollChangedEventArgs</param>
        private void labelAlbum_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollAlbum.ScrollToHorizontalOffset(scrollAlbum.HorizontalOffset + ScrollSteps * 3);
            _scrollDataAlbum.ScrollPreTicks = 0;
        }

        /// <summary>
        /// Scrolls the text
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">MouseWheelEventArgs</param>
        private void labelTitle_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scrollTitle.ScrollToHorizontalOffset(scrollTitle.HorizontalOffset + ScrollSteps * 3);
            _scrollDataTitle.ScrollPreTicks = 0;
        }

        /// <summary>
        /// Click on Search Clear Button
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonSearch_Click(object sender, RoutedEventArgs e)
        {
            textBoxSearch.Text = "";

        }

        /// <summary>
        /// Text in Search Box Changed Event 
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxSearch.Text))
            {
                listView.Visibility = Visibility.Hidden;
                searchView.Visibility = Visibility.Visible;

                // zusaetzlich noch das label fuer Search result zeigen
                labelSearchResult.Visibility = Visibility.Visible;

                _searchsongs.Clear();
                string[] textsearch = textBoxSearch.Text.ToLower().Split(' ');

                foreach (ISong song in APlaylist.Playlist)
                {
                    int addit = 0;
                    foreach (string str in textsearch)
                    {
                        if (song.artistAndtitle.ToLower().Contains(str)
                            || song.getFilePath().ToLower().Contains(str)
                            || song.getTags()[1].ToLower().Contains(str))
                            addit++;
                    }

                    if (addit == textsearch.Length)
                        _searchsongs.Add(song);
                }
            }
            else
            {
                searchView.Visibility = Visibility.Hidden;
                listView.Visibility = Visibility.Visible;
                labelSearchResult.Visibility = Visibility.Hidden;
            }

        }

        /// <summary>
        /// Double Click Search View (same as listview)
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">MousePos</param>
        private void searchView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int width = SystemInformation.VerticalScrollBarWidth;
            System.Windows.Point mousePos = e.GetPosition(searchView);
            if (mousePos.X >= (searchView.ActualWidth - width))
                return;

            if (searchView.SelectedItems.Count == 0)
                return;
            try
            {

                for (int i = 0; i < APlaylist.Playlist.Count; i++)
                {
                    if (APlaylist.Playlist[i] != searchView.SelectedItem) continue;
                    APlaylist.setNowPlayingPosition(i);
                    break;
                }

                APlayControler.Stop();
                APlayControler.Play();
            }
            catch (ArgumentOutOfRangeException)
            { }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
            catch (NullReferenceException)
            { }
        }

        /// <summary>
        /// Context Menu Item: Move to Played Position 
        /// Moves Songs to actual Playing Position
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void MoveToPlayingPosition_Click(object sender, RoutedEventArgs e)
        {
            if (APlaylist.getNowPlayingPosition() < 0 || APlaylist.getNowPlayingPosition() >= APlaylist.Playlist.Count)
                return;

            Collection<ISong> moveit = new Collection<ISong>();
            foreach (ISong song in searchView.SelectedItems)
            {
                if (song != APlaylist.getCurrentSong())
                {
                    moveit.Add(song);
                    PlayListRefProp.Remove(song);
                }
            }

            if (APlaylist.getNowPlayingPosition() == APlaylist.Playlist.Count - 1)
            {
                for (int i = 0; i < moveit.Count; i++)
                    PlayListRefProp.Add(moveit[i]);
            }
            else
            {
                try
                {
                    for (int i = 0; i < moveit.Count; i++)
                    {

                        if (APlayControler.GetPlaying() || APlaylist.getNowPlayingPosition() != 0)
                            PlayListRefProp.Insert(APlaylist.getNowPlayingPosition() + i + 1, moveit[i]);
                        else
                            PlayListRefProp.Insert(APlaylist.getNowPlayingPosition() + i, moveit[i]);

                    }
                }
                catch (ArgumentOutOfRangeException)
                { }

            }
            textBoxSearch.Text = "";
            listView.ScrollIntoView(GetListViewItem(APlaylist.getNowPlayingPosition()));
        }

        /// <summary>
        /// File Drags from Windows Explorer
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">Drag Arguments</param>
        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        /// <summary>
        /// Drop Files from Windows Explorer to Player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                AddFilesAndFolders((string[])(e.Data.GetData(System.Windows.DataFormats.FileDrop)), false);
        }

        /// <summary>
        /// Will be called when the window is minimized oder maximized
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (!AConfiguration.MinimizeToTray || WindowState != WindowState.Minimized)
                return;

            try
            {
                Hide();
            }
            catch (InvalidOperationException)
            { }

            ShowInTaskbar = false;
        }

        #endregion 

        #region HotkeyMethods
        /// <summary>
        /// loads the HotKeys
        /// </summary>
        public void LoadHotKeys()
        {
            _hotkeyPlayPause = new Hotkey();
            _hotkeyStop = new Hotkey();
            _hotkeyNext = new Hotkey();
            _hotkeyPrev = new Hotkey();
            _hotkeyOverlay = new Hotkey();
            _hotkeyVolUp = new Hotkey();
            _hotkeyVolDown = new Hotkey();

            _hotkeyPlayPause.APlayControler = APlayControler;
            _hotkeyStop.APlayControler = APlayControler;
            _hotkeyNext.APlayControler = APlayControler;
            _hotkeyPrev.APlayControler = APlayControler;
            _hotkeyOverlay.APlayControler = APlayControler;
            _hotkeyVolUp.APlayControler = APlayControler;
            _hotkeyVolDown.APlayControler = APlayControler;



            _hotkeyPlayPause.SetHotkey(AConfiguration.HotkeyPlay.MAlt,
                                      AConfiguration.HotkeyPlay.MCtrl,
                                      AConfiguration.HotkeyPlay.MShift,
                                      false,
                                      AConfiguration.HotkeyPlay.HKey);


            _hotkeyNext.SetHotkey(AConfiguration.HotkeyNext.MAlt,
                                  AConfiguration.HotkeyNext.MCtrl,
                                  AConfiguration.HotkeyNext.MShift,
                                  false,
                                  AConfiguration.HotkeyNext.HKey);

            _hotkeyPrev.SetHotkey(AConfiguration.HotkeyPrev.MAlt,
                                      AConfiguration.HotkeyPrev.MCtrl,
                                      AConfiguration.HotkeyPrev.MShift,
                                      false,
                                      AConfiguration.HotkeyPrev.HKey);

            _hotkeyStop.SetHotkey(AConfiguration.HotkeyStop.MAlt,
                                      AConfiguration.HotkeyStop.MCtrl,
                                      AConfiguration.HotkeyStop.MShift,
                                      false,
                                      AConfiguration.HotkeyStop.HKey);

            _hotkeyOverlay.SetHotkey(AConfiguration.HotkeyOverlay.MAlt,
                                      AConfiguration.HotkeyOverlay.MCtrl,
                                      AConfiguration.HotkeyOverlay.MShift,
                                      false,
                                      AConfiguration.HotkeyOverlay.HKey);

            _hotkeyVolUp.SetHotkey(AConfiguration.HotkeyVolUp.MAlt,
                                      AConfiguration.HotkeyVolUp.MCtrl,
                                      AConfiguration.HotkeyVolUp.MShift,
                                      false,
                                      AConfiguration.HotkeyVolUp.HKey);

            _hotkeyVolDown.SetHotkey(AConfiguration.HotkeyVolDown.MAlt,
                                      AConfiguration.HotkeyVolDown.MCtrl,
                                      AConfiguration.HotkeyVolDown.MShift,
                                      false,
                                      AConfiguration.HotkeyVolDown.HKey);

            _hotkeyPlayPause.HotkeyPressed += HotkeyPlayPauseHotkeyPressed;
            _hotkeyStop.HotkeyPressed += HotkeyStopHotkeyPressed;
            _hotkeyNext.HotkeyPressed += HotkeyNextHotkeyPressed;
            _hotkeyPrev.HotkeyPressed += HotkeyPrevHotkeyPressed;
            _hotkeyOverlay.HotkeyPressed += HotkeyOverlayHotkeyPressed;
            _hotkeyVolUp.HotkeyPressed += HotkeyVolUpHotkeyPressed;
            _hotkeyVolDown.HotkeyPressed += HotkeyVolDownHotkeyPressed;

            try
            {
                _hotkeyPlayPause.Enabled = AConfiguration.HotkeyPlay.IsEnabled;
                _hotkeyStop.Enabled = AConfiguration.HotkeyStop.IsEnabled;
                _hotkeyNext.Enabled = AConfiguration.HotkeyNext.IsEnabled;
                _hotkeyPrev.Enabled = AConfiguration.HotkeyPrev.IsEnabled;
                _hotkeyOverlay.Enabled = AConfiguration.HotkeyOverlay.IsEnabled;
                _hotkeyVolUp.Enabled = AConfiguration.HotkeyVolUp.IsEnabled;
                _hotkeyVolDown.Enabled = AConfiguration.HotkeyVolDown.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                //new Error("Could not register a hotkey (already in use).",_anAppHandle,false,this);
            }
        }

        /// <summary>
        /// Refreshes the hotkeys is something changed
        /// </summary>
        private void RefreshHotkeys()
        {
            try
            {
                _hotkeyPlayPause.Enabled = false;

                _hotkeyPlayPause.Alt = AConfiguration.HotkeyPlay.MAlt;
                _hotkeyPlayPause.Ctrl = AConfiguration.HotkeyPlay.MCtrl;
                _hotkeyPlayPause.Shift = AConfiguration.HotkeyPlay.MShift;
                _hotkeyPlayPause.KeyCode = AConfiguration.HotkeyPlay.HKey;

                _hotkeyPlayPause.Enabled = AConfiguration.HotkeyPlay.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                new Error("Could not register hotkey Play(already in use).", false, this);
            }

            try
            {
                _hotkeyStop.Enabled = false;

                _hotkeyStop.Alt = AConfiguration.HotkeyStop.MAlt;
                _hotkeyStop.Ctrl = AConfiguration.HotkeyStop.MCtrl;
                _hotkeyStop.Shift = AConfiguration.HotkeyStop.MShift;
                _hotkeyStop.KeyCode = AConfiguration.HotkeyStop.HKey;

                _hotkeyStop.Enabled = AConfiguration.HotkeyStop.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                new Error("Could not register hotkey Stop(already in use).", false, this);
            }

            try
            {
                _hotkeyNext.Enabled = false;

                _hotkeyNext.Alt = AConfiguration.HotkeyNext.MAlt;
                _hotkeyNext.Ctrl = AConfiguration.HotkeyNext.MCtrl;
                _hotkeyNext.Shift = AConfiguration.HotkeyNext.MShift;
                _hotkeyNext.KeyCode = AConfiguration.HotkeyNext.HKey;

                _hotkeyNext.Enabled = AConfiguration.HotkeyNext.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                new Error("Could not register hotkey Next(already in use).", false, this);
            }

            try
            {
                _hotkeyPrev.Enabled = false;

                _hotkeyPrev.Alt = AConfiguration.HotkeyPrev.MAlt;
                _hotkeyPrev.Ctrl = AConfiguration.HotkeyPrev.MCtrl;
                _hotkeyPrev.Shift = AConfiguration.HotkeyPrev.MShift;
                _hotkeyPrev.KeyCode = AConfiguration.HotkeyPrev.HKey;

                _hotkeyPrev.Enabled = AConfiguration.HotkeyPrev.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                new Error("Could not register hotkey Prev(already in use).", false, this);
            }

            try
            {
                _hotkeyOverlay.Enabled = false;

                _hotkeyOverlay.Alt = AConfiguration.HotkeyOverlay.MAlt;
                _hotkeyOverlay.Ctrl = AConfiguration.HotkeyOverlay.MCtrl;
                _hotkeyOverlay.Shift = AConfiguration.HotkeyOverlay.MShift;
                _hotkeyOverlay.KeyCode = AConfiguration.HotkeyOverlay.HKey;

                _hotkeyOverlay.Enabled = AConfiguration.HotkeyOverlay.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                new Error("Could not register hotkey Overlay(already in use).", false, this);
            }

            try
            {
                _hotkeyVolUp.Enabled = false;

                _hotkeyVolUp.Alt = AConfiguration.HotkeyVolUp.MAlt;
                _hotkeyVolUp.Ctrl = AConfiguration.HotkeyVolUp.MCtrl;
                _hotkeyVolUp.Shift = AConfiguration.HotkeyVolUp.MShift;
                _hotkeyVolUp.KeyCode = AConfiguration.HotkeyVolUp.HKey;

                _hotkeyVolUp.Enabled = AConfiguration.HotkeyVolUp.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                new Error("Could not register hotkey Volume Up(already in use).", false, this);
            }


            try
            {
                _hotkeyVolDown.Enabled = false;

                _hotkeyVolDown.Alt = AConfiguration.HotkeyVolDown.MAlt;
                _hotkeyVolDown.Ctrl = AConfiguration.HotkeyVolDown.MCtrl;
                _hotkeyVolDown.Shift = AConfiguration.HotkeyVolDown.MShift;
                _hotkeyVolDown.KeyCode = AConfiguration.HotkeyVolDown.HKey;

                _hotkeyVolDown.Enabled = AConfiguration.HotkeyVolDown.IsEnabled;
            }
            catch (HotkeyAlreadyInUseException)
            {
                new Error("Could not register hotkey Volume Down(already in use).", false, this);
            }
        }

        /// <summary>
        /// Event Handler for Play Hotkey
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void HotkeyPlayPauseHotkeyPressed(object sender, EventArgs e)
        {
            try
            {
                APlayControler.Play();
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
        }

        /// <summary>
        /// Event Handler for Stop Hotkey
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void HotkeyStopHotkeyPressed(object sender, EventArgs e)
        {
            try
            {
                APlayControler.Stop();
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
        }

        /// <summary>
        /// Event Handler for Next Hotkey
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void HotkeyNextHotkeyPressed(object sender, EventArgs e)
        {
            try
            {
                APlayControler.NextSong();
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
        }
        /// <summary>
        /// Event Handler for Prev Hotkey
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void HotkeyPrevHotkeyPressed(object sender, EventArgs e)
        {
            try
            {
                APlayControler.PreviousSong();
            }
            catch (BassWrapperException ex)
            {
                new Error(ex.GetMessage(), false, this);
            }
        }

        /// <summary>
        /// Event Handler for Overlay Hotkey
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void HotkeyOverlayHotkeyPressed(object sender, EventArgs e)
        {
            if (AIngame == null) new Error("Ingame is disabled!", false, this);

            if (AIngame != null)
                checkboxIngameShow.IsChecked = !AIngame.IsVisible();
            //AIngame.show();
        }

        /// <summary>
        /// Event Handler for Volume Up Hotkey
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void HotkeyVolUpHotkeyPressed(object sender, EventArgs e)
        {
            if (changeVolumeBar.Value < 100)
                changeVolumeBar.Value += 10;
            ABassWrapperGui.setVolume((float)changeVolumeBar.Value);
        }

        /// <summary>
        /// Event Handler for Volume Down Hotkey
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        void HotkeyVolDownHotkeyPressed(object sender, EventArgs e)
        {
            if (changeVolumeBar.Value > 0)
                changeVolumeBar.Value -= 10;
            ABassWrapperGui.setVolume((float)changeVolumeBar.Value);
        } 
        #endregion

        #region DragNDrop
        /// <summary>
        /// Handles all things to do while an item to be dragged is 
        /// atteched to the mouse.
        /// Also handles Scolling
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">MousePos</param>
        private void ListViewDragOver(object sender, System.Windows.DragEventArgs e)
        {
            System.Windows.Point mousePos = e.GetPosition(UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView));

            if (10 > mousePos.Y)
                UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView).LineUp();
            else if (UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView).ActualHeight - 10 < mousePos.Y)
                UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView).LineDown();
        }

        /// <summary>
        /// Checks if the dragged object could be dropped at position of mouse
        /// and drops if possible.
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">MousePos</param>
        void ListViewDrop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                return;

            int index = GetCurrentIndex(e.GetPosition);
            int underORover = 0;

            // check to see if drop can be done
            FrameworkElement ele = e.OriginalSource as FrameworkElement;
            if (ele != null && ele.DataContext != null)
            {
                /*
                if (ele.DataContext.GetType() != typeof(Organisation.Mp3Song) && ele.DataContext.GetType() != typeof(Interfaces.ISong))
                    return;
                 */
            }

            if (index < 0)
                return;

            ObservableCollection<ISong> selectedSongs =
                e.Data.GetData(typeof(ObservableCollection<ISong>))
                as ObservableCollection<ISong>;

            ISong dropTargetSong = PlayListRefProp[index];

            if (selectedSongs != null)
            {
                if (selectedSongs.Contains(dropTargetSong))
                {
                    try
                    {
                        listView.SelectedItems.Clear();

                        ListViewItem actualitem = GetListViewItem(index);
                        actualitem.IsSelected = true;

                        listView.SelectedItems.Add(actualitem);
                    }
                    catch (InvalidOperationException)
                    { }
                    catch (NotSupportedException)
                    { }

                    return;
                }
            }

            // check the order of the items
            List<ISong> dropList = new List<ISong>();
            foreach (ISong song in PlayListRefProp)
            {
                if (selectedSongs != null)
                    if (!selectedSongs.Contains(song))
                        continue;

                dropList.Add(song);
            }

            foreach (ISong song in dropList)
            {
                underORover = PlayListRefProp.IndexOf(song);
                PlayListRefProp.Remove(song);
            }

            // find index of drop target
            int selectIndex = PlayListRefProp.IndexOf(dropTargetSong);

            listView.SelectedItems.Clear();
            // insert items into observablecollection
            for (int i = 0; i < dropList.Count; i++)
            {
                try
                {
                    ISong song = dropList[i];
                    if (underORover > selectIndex)
                        PlayListRefProp.Insert(i + selectIndex, song);
                    else
                        PlayListRefProp.Insert(i + selectIndex + 1, song);
                    listView.SelectedItems.Add(song);
                }
                catch (ArgumentOutOfRangeException)
                { }
                catch (InvalidOperationException)
                { }
                catch (NotSupportedException)
                { }
            }
        }

        /// <summary>
        /// Returns the listview item at index(parameter)
        /// </summary>
        /// <param name="index">Index of Listview Item</param>
        /// <returns>ListViewItem</returns>
        ListViewItem GetListViewItem(int index)
        {
            if (listView.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;

            return listView.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        //For Drag&Drop Calculation
        private double _listViewItemSize;

        /// <summary>
        /// Returns the index of the item in the ListView on the (paramter) position
        /// </summary>
        /// <param name="getPosition">Position Delegate MousePos</param>
        /// <returns>int index</returns>
        int GetCurrentIndex(GetPositionDelegate getPosition)
        {
            int listoffset;
            Rect bounds = new Rect(0, 0, 0, 0);

            if (GetListViewItem(0) != null)
                bounds = VisualTreeHelper.GetDescendantBounds(GetListViewItem(0));

            if (bounds.Height != 0 && bounds.Height > _listViewItemSize)
                _listViewItemSize = bounds.Height;

            System.Windows.Point mousePos = getPosition((IInputElement)listView);

            if (_listViewItemSize != 0)
                listoffset = (int)(mousePos.Y / _listViewItemSize);
            else
                return 0;

            int index = (int)UserInterfaceHelper.GetVisualChild<ScrollViewer>(listView).VerticalOffset + listoffset;

            if (index < 0)
                return 0;

            if (index >= listView.Items.Count)
                return listView.Items.Count - 1;

            return index;
        }

        /// <summary>
        /// Starts the Drag if the Item under the curser is a valid Drag Item
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">MousePos</param>
        void ListViewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int index = GetCurrentIndex(e.GetPosition);

            // check to see if drag can be done
            FrameworkElement ele = e.OriginalSource as FrameworkElement;
            if (ele != null && ele.DataContext != null)
            {
                if (ele.DataContext.GetType() != typeof(Mp3Song) && ele.DataContext.GetType() != typeof(Song))
                    return;
            }

            const DragDropEffects allowedEffects = DragDropEffects.Move;

            if (e.LeftButton != MouseButtonState.Pressed || e.ClickCount != 1) 
                return;

            var draggSongs = new ObservableCollection<ISong>();

            foreach (ISong song in listView.SelectedItems)
                draggSongs.Add(song);

            ISong actDragSong = listView.Items[index] as ISong;

            if (listView.SelectedItems.Count < 2)
            {
                if (!draggSongs.Contains(actDragSong))
                    draggSongs.Clear();

                draggSongs.Add(actDragSong);
            }
            else
            {
                if (!draggSongs.Contains(actDragSong))
                    return;
            }

            try
            {
                DragDrop.DoDragDrop(listView, draggSongs, allowedEffects);
            }
            catch (ArgumentNullException)
            {}
            catch (NullReferenceException)
            {}

            ColorCurrentPlayedSong();
        } 
        #endregion

        #region Sorting
        /// <summary>
        /// Swap an Item
        /// </summary>
        /// <param name="leftItem">Left Item </param>
        /// <param name="rightItem">Right Item</param>
        private void QuickSwap(int leftItem, int rightItem)
        {
            if (rightItem > leftItem)
            {
                APlaylist.Playlist.Move(rightItem, leftItem);
                APlaylist.Playlist.Move(leftItem + 1, rightItem);
            }
        }

        /// <summary>
        /// Quicksort Playlist
        /// </summary>
        /// <param name="sortMode"></param>
        /// <param name="leftItem"></param>
        /// <param name="rightItem"></param>
        public void QuickSort(int sortMode, int leftItem, int rightItem)
        {
            string strLeft;
            string strRight;
            int lHold = leftItem;
            int rHold = rightItem;
            Random objRan = new Random();
            int pivot = objRan.Next(leftItem, rightItem);
            QuickSwap(pivot, leftItem);
            pivot = leftItem;
            leftItem++;

            while (rightItem >= leftItem)
            {
                string strPivot;
                switch (sortMode)
                {
                    case 0:
                        strLeft = APlaylist.Playlist[leftItem].title;
                        strRight = APlaylist.Playlist[rightItem].title;
                        strPivot = APlaylist.Playlist[pivot].title;
                        break;
                    case 1:
                        strLeft = APlaylist.Playlist[leftItem].artist;
                        strRight = APlaylist.Playlist[rightItem].artist;
                        strPivot = APlaylist.Playlist[pivot].artist;
                        break;
                    default:
                        strLeft = APlaylist.Playlist[leftItem].artist;
                        strRight = APlaylist.Playlist[rightItem].artist;
                        strPivot = APlaylist.Playlist[pivot].artist;
                        break;
                }
                int cmpLeftVal = strLeft.CompareTo(strPivot);
                int cmpRightVal = strRight.CompareTo(strPivot);

                if ((cmpLeftVal >= 0) && (cmpRightVal < 0))
                {
                    QuickSwap(leftItem, rightItem);
                }
                else
                {
                    if (cmpLeftVal >= 0)
                    {
                        rightItem--;
                    }
                    else
                    {
                        if (cmpRightVal < 0)
                        {
                            leftItem++;
                        }
                        else
                        {
                            rightItem--;
                            leftItem++;
                        }
                    }
                }
            }
            QuickSwap(pivot, rightItem);
            pivot = rightItem;
            if (pivot > lHold)
            {
                QuickSort(sortMode, lHold, pivot);
            }
            if (rHold > pivot + 1)
            {
                QuickSort(sortMode, pivot + 1, rHold);
            }
        }

        /// <summary>
        /// Sort Combobox changes
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.Items.Count > 1)
            {
                QuickSort(comboBoxSort.SelectedIndex, 0, APlaylist.Playlist.Count - 1);
            }
        } 
        #endregion

        #region TrayiconMethods
        /// <summary>
        /// Gets the Window back from tray
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void MaxFromTray(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (InvalidOperationException)
            { }

            WindowState = WindowState.Normal;
            ShowInTaskbar = true;
        }

        /// <summary>
        /// creates a tray icon and the corresponding context menu
        /// </summary>
        private void TrayIcon()
        {
            try
            {
                Assembly targetAssembly = Assembly.Load("ResourceLibrary");
                if (targetAssembly != null)
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    var icon = new Icon(targetAssembly.GetManifestResourceStream("ResourceLibrary.pictures.trayicon.ico"));
                    // ReSharper restore AssignNullToNotNullAttribute

                    _notifyIcon1.Icon = new Icon(icon, 1, 1);
                }
            }
            catch (ArgumentException)
            { }
            catch (FileNotFoundException)
            { }
            catch (FileLoadException)
            { }
            catch (BadImageFormatException)
            { }

            _notifyIcon1.DoubleClick += MaxFromTray;
            _notifyIcon1.Visible = true;

            _contextMenuStrip1 = new ContextMenuStrip();
            _playToolStripMenuItem = new ToolStripMenuItem();
            _stopToolStripMenuItem = new ToolStripMenuItem();
            _nextSongToolStripMenuItem = new ToolStripMenuItem();
            _pevToolStripMenuItem = new ToolStripMenuItem();
            _fileToolStripMenuItem = new ToolStripMenuItem();
            _addFileToolStripMenuItem = new ToolStripMenuItem();
            _addFolderToolStripMenuItem = new ToolStripMenuItem();
            _playlistToolStripMenuItem = new ToolStripMenuItem();
            _newPlaylistToolStripMenuItem = new ToolStripMenuItem();
            _openPlaylistToolStripMenuItem = new ToolStripMenuItem();
            _savePlaylistToolStripMenuItem = new ToolStripMenuItem();
            _equalizerToolStripMenuItem = new ToolStripMenuItem();
            _onToolStripMenuItem = new ToolStripMenuItem();
            _resetToolStripMenuItem = new ToolStripMenuItem();
            _loadPresetToolStripMenuItem = new ToolStripMenuItem();
            _savePresetToolStripMenuItem = new ToolStripMenuItem();
            _settingsToolStripMenuItem = new ToolStripMenuItem();
            _closeToolStripMenuItem = new ToolStripMenuItem();
            _toolStripSeparator1 = new ToolStripSeparator();
            _toolStripSeparator2 = new ToolStripSeparator();

            // 
            // _contextMenuStrip1
            // 
            try
            {
                _contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
                _playToolStripMenuItem,
                _stopToolStripMenuItem,
                _nextSongToolStripMenuItem,
                _pevToolStripMenuItem,
                _toolStripSeparator1,
                _fileToolStripMenuItem,
                _playlistToolStripMenuItem,
                _equalizerToolStripMenuItem,
                _toolStripSeparator2,
                _settingsToolStripMenuItem,
                _closeToolStripMenuItem
                });

                _contextMenuStrip1.Name = "_contextMenuStrip1";
                _contextMenuStrip1.Size = new System.Drawing.Size(153, 214);
            }
            catch (ArgumentNullException)
            { }
            catch (NotSupportedException)
            { }

            // 
            // _playToolStripMenuItem
            // 
            _playToolStripMenuItem.Name = "_playToolStripMenuItem";
            _playToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _playToolStripMenuItem.Text = "Play/Pause";
            _playToolStripMenuItem.Click += buttonPlayPauseClick;

            // 
            // _stopToolStripMenuItem
            // 
            _stopToolStripMenuItem.Name = "_stopToolStripMenuItem";
            _stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _stopToolStripMenuItem.Text = "Stop";
            _stopToolStripMenuItem.Click += buttonStopClick;

            // 
            // _nextSongToolStripMenuItem
            // 
            _nextSongToolStripMenuItem.Name = "_nextSongToolStripMenuItem";
            _nextSongToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _nextSongToolStripMenuItem.Text = "Next Song";
            _nextSongToolStripMenuItem.Click += buttonNextClick;

            // 
            // _pevToolStripMenuItem
            // 
            _pevToolStripMenuItem.Name = "_pevToolStripMenuItem";
            _pevToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _pevToolStripMenuItem.Text = "Previous Song";
            _pevToolStripMenuItem.Click += buttonPrevClick;

            // 
            // _fileToolStripMenuItem
            // 
            try
            {
                _fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                    _addFileToolStripMenuItem,
                    _addFolderToolStripMenuItem});

                _fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
                _fileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
                _fileToolStripMenuItem.Text = "File";
            }
            catch (ArgumentNullException)
            { }
            catch (NotSupportedException)
            { }

            // 
            // _addFileToolStripMenuItem
            // 
            _addFileToolStripMenuItem.Name = "_addFileToolStripMenuItem";
            _addFileToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _addFileToolStripMenuItem.Text = "Add File";
            _addFileToolStripMenuItem.Click += buttonOpenFileClick;

            // 
            // _addFolderToolStripMenuItem
            // 
            _addFolderToolStripMenuItem.Name = "_addFolderToolStripMenuItem";
            _addFolderToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _addFolderToolStripMenuItem.Text = "Add Folder";
            _addFolderToolStripMenuItem.Click += buttonOpenFolderClick;

            // 
            // _playlistToolStripMenuItem
            // 
            try
            {
                _playlistToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                    _newPlaylistToolStripMenuItem,
                    _openPlaylistToolStripMenuItem,
                    _savePlaylistToolStripMenuItem});
                _playlistToolStripMenuItem.Name = "_playlistToolStripMenuItem";
                _playlistToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
                _playlistToolStripMenuItem.Text = "Playlist";
            }
            catch (ArgumentNullException)
            { }
            catch (NotSupportedException)
            { }

            // 
            // _newPlaylistToolStripMenuItem
            // 
            _newPlaylistToolStripMenuItem.Name = "_newPlaylistToolStripMenuItem";
            _newPlaylistToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _newPlaylistToolStripMenuItem.Text = "New Playlist";
            _newPlaylistToolStripMenuItem.Click += buttonNewPlaylistClick;

            // 
            // _openPlaylistToolStripMenuItem
            // 
            _openPlaylistToolStripMenuItem.Name = "_openPlaylistToolStripMenuItem";
            _openPlaylistToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _openPlaylistToolStripMenuItem.Text = "Open Playlist";
            _openPlaylistToolStripMenuItem.Click += buttonOpenPlaylistClick;

            // 
            // _savePlaylistToolStripMenuItem
            // 
            _savePlaylistToolStripMenuItem.Name = "_savePlaylistToolStripMenuItem";
            _savePlaylistToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _savePlaylistToolStripMenuItem.Text = "Save Playlist";
            _savePlaylistToolStripMenuItem.Click += buttonSavePlaylistClick;

            // 
            // _equalizerToolStripMenuItem
            // 
            try
            {
                _equalizerToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                    _onToolStripMenuItem,
                    _resetToolStripMenuItem,
                    _loadPresetToolStripMenuItem,
                    _savePresetToolStripMenuItem});

                _equalizerToolStripMenuItem.Name = "_equalizerToolStripMenuItem";
                _equalizerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
                _equalizerToolStripMenuItem.Text = "Equalizer";
            }
            catch (ArgumentNullException)
            { }
            catch (NotSupportedException)
            { }

            // 
            // _onToolStripMenuItem
            // 
            _onToolStripMenuItem.Name = "_onToolStripMenuItem";
            _onToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _onToolStripMenuItem.Text = "On";
            _onToolStripMenuItem.Click += ButtonEqualizerStateForTrayIcon;

            _onToolStripMenuItem.Checked = ABassWrapperGui.gettoggleEQ();

            // 
            // _resetToolStripMenuItem
            // 
            _resetToolStripMenuItem.Name = "_resetToolStripMenuItem";
            _resetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _resetToolStripMenuItem.Text = "Reset";
            _resetToolStripMenuItem.Click += buttonResetEQClick;

            // 
            // _loadPresetToolStripMenuItem
            // 
            _loadPresetToolStripMenuItem.Name = "_loadPresetToolStripMenuItem";
            _loadPresetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _loadPresetToolStripMenuItem.Text = "Load Preset";
            _loadPresetToolStripMenuItem.Click += buttonLoadEQClick;

            // 
            // _savePresetToolStripMenuItem
            // 
            _savePresetToolStripMenuItem.Name = "_savePresetToolStripMenuItem";
            _savePresetToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _savePresetToolStripMenuItem.Text = "Save Preset";
            _savePresetToolStripMenuItem.Click += buttonSaveEQClick;

            // 
            // _settingsToolStripMenuItem
            // 
            _settingsToolStripMenuItem.Name = "_settingsToolStripMenuItem";
            _settingsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _settingsToolStripMenuItem.Text = "Settings";
            _settingsToolStripMenuItem.Click += buttonSettingsClick;

            // 
            // Close
            // 
            _closeToolStripMenuItem.Name = "_closeToolStripMenuItem";
            _closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            _closeToolStripMenuItem.Text = "Close";
            _closeToolStripMenuItem.Click += buttonCloseClick;

            // 
            // _toolStripSeparator1
            // 
            _toolStripSeparator1.Name = "_toolStripSeparator1";
            _toolStripSeparator1.Size = new System.Drawing.Size(149, 6);

            // 
            // _toolStripSeparator2
            // 
            _toolStripSeparator2.Name = "_toolStripSeparator2";
            _toolStripSeparator2.Size = new System.Drawing.Size(149, 6);

            _notifyIcon1.ContextMenuStrip = _contextMenuStrip1;
        } 
        #endregion

        #region IPluginHost Member

        public void pluginFeedback(string feedback, IPlugin plugin)
        {

        }

        public void pluginClose(IPlugin myPlugin)
        {
            _dictPluginWindows[myPlugin.Name].Close();
        }


        private void buttonPause_Checked(object sender, RoutedEventArgs e)
        {

        }

        public IPlaylist getPlaylist()
        {
            return APlaylist;
        }


        public IPlayControler getPlayControler()
        {
            return APlayControler;
        }

        public IBassWrapper getBasswrapper()
        {
            return (IBassWrapper)ABassWrapperGui;
        }



        public string getFileSavePath()
        {
            return AConfiguration.MyDocumentsPath;
        }

        public Dispatcher getDispatcher()
        {
            return Dispatcher;
        }

        public BackgroundWorker getBackgroundWorker()
        {
            return _backgroundWorker1;
        }


        public IPluginManager getPluginmanager()
        {
            return APluginManager;
        }

        public void ShowPlugin(IPlugin plugin)
        {
            ShowPlugin(plugin.Name);
        }


        #endregion

    }
}
