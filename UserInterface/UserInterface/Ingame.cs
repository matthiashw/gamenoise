/* 
 * author: BK
 * 
 * created: 07.11.2008
 * 
 * modification history
 * --------------------
 * 
 * 26.11.08 MHO:
 * Overlay functions added
 * 
 * 01.12.08 MHO:
 * error handling added
 * Hotkeys added
 * 
 * 03.12.08 MHI:
 * error handling initOverlay
 * 
 * 04.12.08
 * try-catch blocks added + Error 
 * 
 * 30.12.08
 * Overlay buendig
 */

using System;
using Organisation;
using OverlayLib;
using System.Drawing;
using PlayControl;
using System.Drawing.Drawing2D;
using Microsoft.DirectX.DirectDraw;
using Interfaces;


namespace UserInterface
{

    /// <summary>
    /// Displays the 'Ingame-Overlay'
    /// </summary>
    public class Ingame : IObserver
    {
        /// <summary>
        /// Reference to the the Configuration object
        /// </summary>
        private readonly Configuration _myConfiguration;

        //Overlay Objekt  erstellen
        public static Overlay MyOverlay;

        //Song data
        private static double _time;
        private static double _totaltime;
        private static string _artist;
        private static string _playmode;
        private static string _album;
        private static string _title;

        //Overlay data
        private static String _hotkeyPause;
        private static String _hotkeyNext;
        private static String _hotkeyPrev;
        private static String _hotkeyStop;
        private static String _hotkeyVolumeUp;
        private static String _hotkeyVolumeDown;
        private static String _hotkeyHide;

        private bool _visible;
        private bool _updating;

        private static Color _colorBackground;// = Color.Black;
        private static Color _colorHotkeyTop;// = Color.DarkGray;
        private static Color _colorHotkeyBackground;// = Color.Gray;
        private static Color _colorFont;// = Color.White;
        private static Color _colorLines;// = Color.White;
        private static Color _colorProgress;// = Color.Orange;
        //private static Color colorHotkeyBottom;// = Color.Black;

        private static Organisation.OverlayPositionEnum _overlayPosition;

        private const int OverlayWidth = 474;
        private const int OverlayHeight = 76;
        private const int RoundedCorner = 10;
        private const int BorderHorizontal = 20;

        private const int ProgressBarTopMargin = 20;
        private const int ProgressBarHeight = 10;

        private const int HotkeyHeight = 14;
        private const int HotkeyWidth = 54;
        private const int HotkeyTopMargin = 40;
        private const int HotkeyRightMargin = 4;
        private const int HotkeyGroupMargin = 16;

        private const string OverlayFont = "Arial";
        private const int OverlayFontSize= 8;

        private static string _hotkeyLabelPause = "Pause";
        private const string HotkeyLabelPrev = "Prev";
        private const string HotkeyLabelNext = "Next";
        private const string HotkeyLabelStop = "Stop";
        private const string HotkeyLabelVolumeUp = "Vol.Up";
        private const string HotkeyLabelVolumeDown = "Vol.Down";
        private const string HotkeyLabelHide = "Hide";  

        private readonly App _anAppHandle;

        /// <summary>
        /// Initialize the local variables
        /// </summary>
        /// <param name="aConfiguration">The Configuration</param>
        /// <param name="appHandle">Handler of the Root-Window</param>
        public Ingame(Configuration aConfiguration, App appHandle)
        {
            _anAppHandle = appHandle;
            _myConfiguration = aConfiguration;

            _time = 0;
            _totaltime = 0;
            _artist = "";
            _title = "";

            _hotkeyPause = "Space";
            _hotkeyPrev = "P";
            _hotkeyNext = "N";
            _hotkeyStop = "Return";
            _hotkeyVolumeUp = "+";
            _hotkeyVolumeDown = "-";
            _hotkeyHide = "O";

            _colorBackground = Color.FromArgb(255, _myConfiguration.OverlayColorBack.Red,
                                                        _myConfiguration.OverlayColorBack.Green,
                                                        _myConfiguration.OverlayColorBack.Blue);
            _colorHotkeyTop = Color.FromArgb(255, _myConfiguration.OverlayColorHKTop.Red,
                                                        _myConfiguration.OverlayColorHKTop.Green,
                                                        _myConfiguration.OverlayColorHKTop.Blue);
            _colorHotkeyBackground = Color.FromArgb(255, _myConfiguration.OverlayColorHKBack.Red,
                                                        _myConfiguration.OverlayColorHKBack.Green,
                                                        _myConfiguration.OverlayColorHKBack.Blue);
            _colorLines = Color.FromArgb(255, _myConfiguration.OverlayColorLine.Red,
                                                        _myConfiguration.OverlayColorLine.Green,
                                                        _myConfiguration.OverlayColorLine.Blue);
            _colorFont = Color.FromArgb(255, _myConfiguration.OverlayColorFont.Red,
                                                        _myConfiguration.OverlayColorFont.Green,
                                                        _myConfiguration.OverlayColorFont.Blue);
            _colorProgress = Color.FromArgb(255, _myConfiguration.OverlayColorProgress.Red,
                                                        _myConfiguration.OverlayColorProgress.Green,
                                                        _myConfiguration.OverlayColorProgress.Blue);

            _visible = false;
            _updating = false;
            try
            {
                MyOverlay = new Overlay {Size = new Size(OverlayWidth, OverlayHeight)};

                _overlayPosition = _myConfiguration.OverlayPosition;
                SetPosition(_overlayPosition);

                //Initialize the Overlay
                InitOverlay();
            }
            catch(Exception)
            {}
        }
    
        /// <summary>
        /// Displays the overlay
        /// </summary>
        public void Show()
        {
            try
            {
                SetPosition(_overlayPosition);
                _visible = true;
                Update(null);
            }
            catch (Microsoft.DirectX.DirectXException)
            {
                _visible = false;
                new Error("Overlay could not be created.",false,null);
            }
            catch (ArgumentNullException)
            {
                _visible = false;
                new Error("Overlay could not be created.", false,null);
            }
        }

        /// <summary>
        /// Hides the overlay
        /// </summary>
        public void Hide()
        {
            _visible = false;
            try
            {
                MyOverlay.Hide();
            }
            catch (Microsoft.DirectX.DirectXException)
            {
                //maybe a surface lost exception
                //do nothing
            }
            catch (NullReferenceException)
            {}
        }

        /// <summary>
        /// Checks if the overlay is visible
        /// </summary>
        /// <returns>true if visible, false if not</returns>
        public bool IsVisible()
        {
            return _visible;
        }


        /// <summary>
        /// Update Method that is called to Update the Observer Standardt when Modell has changed
        /// </summary>
        /// <param name="subject">Changed Subject</param>
        public void Update(object subject)
        {
            try {
            if (_updating) return;
            _updating = true;

            if (subject is GameNoiseList)
            {
                if (((GameNoiseList)subject).getCurrentSong() != null)
                {
                    _artist = ((GameNoiseList)subject).getCurrentSong().getTags()[1];
                    _album = ((GameNoiseList)subject).getCurrentSong().getTags()[0];
                    _title = ((GameNoiseList)subject).getCurrentSong().getTags()[2];                    
                }
                switch (((GameNoiseList)subject).getPlayMode())
                {
                    case (int)PlayModeEnum.PlaymodeNormal:
                        _playmode = "normal";
                        break;
                    case (int)PlayModeEnum.PlaymodeRandom:
                        _playmode = "shuffle";
                        break;
                    case (int)PlayModeEnum.PlaymodeRepeatList:
                        _playmode = "repeat all";
                        break;
                    case (int)PlayModeEnum.PlaymodeRepeatSong:
                        _playmode = "repeate one";
                        break;
                }
            }
            else if (subject is BassWrapper)
            {
                _time = ((BassWrapper)subject).getElapsedTime();
                _totaltime = ((BassWrapper)subject).getTotalTime();

                if (((BassWrapper)subject).getPaused() || ((BassWrapper)subject).getstopped())
                {
                    _hotkeyLabelPause = "Play";
                }
                else 
                {
                    _hotkeyLabelPause = "Pause";
                }
            }
            else if (subject is Configuration)
            {
                _colorBackground = Color.FromArgb(255, ((Configuration)subject).OverlayColorBack.Red,
                                                            ((Configuration)subject).OverlayColorBack.Green,
                                                            ((Configuration)subject).OverlayColorBack.Blue);
                _colorHotkeyTop = Color.FromArgb(255, ((Configuration)subject).OverlayColorHKTop.Red,
                                                            ((Configuration)subject).OverlayColorHKTop.Green,
                                                            ((Configuration)subject).OverlayColorHKTop.Blue);
                _colorHotkeyBackground = Color.FromArgb(255, ((Configuration)subject).OverlayColorHKBack.Red,
                                                            ((Configuration)subject).OverlayColorHKBack.Green,
                                                            ((Configuration)subject).OverlayColorHKBack.Blue);
                _colorLines = Color.FromArgb(255, ((Configuration)subject).OverlayColorLine.Red,
                                                            ((Configuration)subject).OverlayColorLine.Green,
                                                            ((Configuration)subject).OverlayColorLine.Blue);
                _colorFont = Color.FromArgb(255, ((Configuration)subject).OverlayColorFont.Red,
                                                            ((Configuration)subject).OverlayColorFont.Green,
                                                            ((Configuration)subject).OverlayColorFont.Blue);
                _colorProgress = Color.FromArgb(255, ((Configuration)subject).OverlayColorProgress.Red,
                                                            ((Configuration)subject).OverlayColorProgress.Green,
                                                            ((Configuration)subject).OverlayColorProgress.Blue);

                _overlayPosition = ((Configuration)subject).OverlayPosition;

                string calc="";
                if (((Configuration)subject).HotkeyPlay.MCtrl)
                    calc =  "c+";
                if (((Configuration)subject).HotkeyPlay.MAlt)
                    calc += "a+";
                if (((Configuration)subject).HotkeyPlay.MShift)
                    calc = calc + "s+"; 
                _hotkeyPause = calc+ ((Configuration)subject).HotkeyPlay.HKey;
                calc = "";
                if (((Configuration)subject).HotkeyPrev.MCtrl)
                    calc = calc + "c+";
                if (((Configuration)subject).HotkeyPrev.MAlt)
                    calc += "a+";
                if (((Configuration)subject).HotkeyPrev.MShift)
                    calc = calc + "s+";
                _hotkeyPrev = calc + ((Configuration)subject).HotkeyPrev.HKey;
                calc = "";
                if (((Configuration)subject).HotkeyNext.MCtrl)
                    calc = calc + "c+";
                if (((Configuration)subject).HotkeyNext.MAlt)
                    calc += "a+";
                if (((Configuration)subject).HotkeyNext.MShift)
                    calc = calc + "s+";
                _hotkeyNext = calc + ((Configuration)subject).HotkeyNext.HKey;
                calc = "";
                if (((Configuration)subject).HotkeyStop.MCtrl)
                    calc = calc + "c+";
                if (((Configuration)subject).HotkeyStop.MAlt)
                    calc += "a+";
                if (((Configuration)subject).HotkeyStop.MShift)
                    calc = calc + "s+";
                _hotkeyStop = calc + ((Configuration)subject).HotkeyStop.HKey;
                calc = "";
                if (((Configuration)subject).HotkeyVolUp.MCtrl)
                    calc = calc + "c+";
                if (((Configuration)subject).HotkeyVolUp.MAlt)
                    calc += "a+";
                if (((Configuration)subject).HotkeyVolUp.MShift)
                    calc = calc + "s+";
                _hotkeyVolumeUp = calc + ((Configuration)subject).HotkeyVolUp.HKey;
                calc = "";
                if (((Configuration)subject).HotkeyVolDown.MCtrl)
                    calc = calc + "c+";
                if (((Configuration)subject).HotkeyVolDown.MAlt)
                    calc += "a+";
                if (((Configuration)subject).HotkeyVolDown.MShift)
                    calc = calc + "s+";
                _hotkeyVolumeDown = calc + ((Configuration)subject).HotkeyVolDown.HKey;

                calc = "";
                if (((Configuration)subject).HotkeyOverlay.MCtrl)
                    calc = calc + "c+";
                if (((Configuration)subject).HotkeyOverlay.MAlt)
                    calc += "a+";
                if (((Configuration)subject).HotkeyOverlay.MShift)
                    calc = calc + "s+";
                _hotkeyHide = calc + ((Configuration)subject).HotkeyOverlay.HKey;

            }

            
                if (_visible) MyOverlay.Update();
            }
            catch (OutOfCapsException e)
            {
                new Error("Can't display overlay.\nMaybe Vista Aero is enabled.\n" + e.Message, false, null);
                _visible = false;
            }
            catch (InvalidOperationException e)
            {
                new Error("Can't display overlay.\nMaybe another Application locked DirectX" + e.Message, false, null);
                _visible = false;
            }
            catch (NullReferenceException e)
            {
                new Error("Can't display overlay.\nMaybe another Application locked DirectX" + e.Message, false, null);
                _visible = false;
            }

            _updating = false;
        }

/*
        /// <summary>
        /// Converts the total time of the song to a human readable time
        /// </summary>
        /// <returns>Formated total time</returns>
        private static string getFormatedTotalTime()
        {
            return ((int)(_totaltime / 60)).ToString("00;0#") + ":" + ((int)(_totaltime % 60)).ToString("00;0#") + " / " + ((int)(_time / 60)).ToString("00;0#") + ":" + ((int)(_time % 60)).ToString("00;0#");
        }
*/

        /// <summary>
        /// Initialize the overlay
        /// </summary>
        private static void InitOverlay()
        {
            //Initialise overlay
            try
            {
                MyOverlay.Renderer = OnRender;
                MyOverlay.Initialise();
            }
            catch (Exception)
            {
                //new Error(e.Message, _anAppHandle, false,null);
            }
        }

        /// <summary>
        /// Sets the position of the overlay
        /// </summary>
        /// <param name="position">Position where the overlay should be drawn</param>
        public void SetPosition(Organisation.OverlayPositionEnum position)
        {
            int systemWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int systemHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            try
            {
                switch (position)
                {
                    case Organisation.OverlayPositionEnum.TOP:
                        MyOverlay.Position = new Point((systemWidth - MyOverlay.Size.Width) / 2, 0);
                        break;
                    case Organisation.OverlayPositionEnum.Topleft:
                        MyOverlay.Position = new Point(0, 0);
                        break;
                    case Organisation.OverlayPositionEnum.Topright:
                        MyOverlay.Position = new Point(systemWidth - MyOverlay.Size.Width, 0);
                        break;
                    case Organisation.OverlayPositionEnum.Bottom:
                        MyOverlay.Position = new Point((systemWidth - MyOverlay.Size.Width) / 2, systemHeight - MyOverlay.Size.Height);
                        break;
                    case Organisation.OverlayPositionEnum.Bottomleft:
                        MyOverlay.Position = new Point(0, systemHeight - MyOverlay.Size.Height);
                        break;
                    case Organisation.OverlayPositionEnum.Bottomright:
                        MyOverlay.Position = new Point(systemWidth - MyOverlay.Size.Width, systemHeight - MyOverlay.Size.Height);
                        break;
                }
            }
            catch (GraphicsException) { }
            if (!_visible) Hide();

        }

        /// <summary>
        /// Called when the overlay is redrawn
        /// </summary>
        /// <param name="g">Graphics g</param>
        public static void OnRender(Graphics g)
        {
            //Transparent background
            g.CopyFromScreen(MyOverlay.Boundings.Location, Point.Empty, MyOverlay.Boundings.Size, CopyPixelOperation.SourceCopy);

            // Background rectangle. Added RoundedCorner to height to hide the top corner.
            Rectangle background = new Rectangle(0, -RoundedCorner, MyOverlay.Size.Width - 2, MyOverlay.Size.Height- 2 + RoundedCorner);
            
            // abgerundetes Rechteck mit Farbverlauf zeichnen
            DrawRoundedRectangle(g, background, RoundedCorner, Color.Black, new SolidBrush(_colorBackground));
            
            //draw progressbar
            DrawProgressBar(g);

            //draw hotkeys
            DrawHotKeys(g);
        }


        /// <summary>
        /// Draws the ProgressBar
        /// </summary>
        /// <param name="g">Graphics Object on which the function draws</param>
        private static void DrawProgressBar(Graphics g)
        {
            //Create progressbar rectangle
            Rectangle rectProgressBar = new Rectangle(BorderHorizontal, ProgressBarTopMargin, MyOverlay.Size.Width - 2 * BorderHorizontal, ProgressBarHeight);

            //Define the width of the current progress
            int progressWidth = (int)((rectProgressBar.Width) / _totaltime * _time);

            //Create progressbar rectange
            Rectangle rectProgress = new Rectangle(rectProgressBar.X + 1, rectProgressBar.Y + 1, progressWidth, rectProgressBar.Height - 1);

            //Draw the bar
            g.DrawRectangle(new Pen(_colorLines), rectProgressBar);
            g.FillRectangle(new SolidBrush(_colorProgress), rectProgress);

            //Draw elapsed time
            g.DrawString(UserInterfaceHelper.GetFormatedCurrentTime(_time), new Font("Courier New", rectProgressBar.Height-4), new SolidBrush(_colorFont),
                         rectProgressBar.X+rectProgressBar.Width-30, rectProgressBar.Y+1);

            //Draw artist and title
            String seperator = "";
            if (_title != "" && _artist != "") seperator = " - ";

            g.DrawString(_title + seperator + _artist, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont),
                         rectProgressBar.X , rectProgressBar.Y - 15);


            //background for Playmode
            Rectangle rectPlaymode = new Rectangle(rectProgressBar.X + rectProgressBar.Width - 98, rectProgressBar.Y - 15, 100, 15);
            g.FillRectangle(new SolidBrush(_colorBackground), rectPlaymode);
            //draw Playmode
            g.DrawString("Mode: " + _playmode, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectPlaymode);

            //draws legend
            Rectangle rectLegend = new Rectangle(rectProgressBar.X + rectProgressBar.Width - 200, rectProgressBar.Y - 15, 100, 15);
            g.FillRectangle(new SolidBrush(_colorBackground), rectLegend);
            g.DrawString("c=ctrl a=alt s=shift", new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectLegend);
        }

        /// <summary>
        /// Draws the hotkeys
        /// </summary>
        /// <param name="g">Graphics Object on which the function draws</param>
        private static void DrawHotKeys(Graphics g)
        {
            Rectangle rectHotkeyPauseTop = new Rectangle(GetHotkeyXPosition(1), HotkeyTopMargin, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyPrevTop = new Rectangle(GetHotkeyXPosition(2), HotkeyTopMargin, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyNextTop = new Rectangle(GetHotkeyXPosition(3), HotkeyTopMargin, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyStopTop = new Rectangle(GetHotkeyXPosition(4), HotkeyTopMargin, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyVolumeUpTop = new Rectangle(GetHotkeyXPosition(5) + HotkeyGroupMargin, HotkeyTopMargin, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyVolumeDownTop = new Rectangle(GetHotkeyXPosition(6) + HotkeyGroupMargin, HotkeyTopMargin, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyHideTop = new Rectangle(GetHotkeyXPosition(7) + HotkeyGroupMargin * 2, HotkeyTopMargin, HotkeyWidth, HotkeyHeight);

            Rectangle rectHotkeyPauseBottom = new Rectangle(rectHotkeyPauseTop.X, rectHotkeyPauseTop.Y + HotkeyHeight, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyPrevBottom = new Rectangle(rectHotkeyPrevTop.X, rectHotkeyPrevTop.Y + HotkeyHeight, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyNextBottom = new Rectangle(rectHotkeyNextTop.X, rectHotkeyNextTop.Y + HotkeyHeight, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyStopBottom = new Rectangle(rectHotkeyStopTop.X, rectHotkeyStopTop.Y + HotkeyHeight, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyVolumeUpBottom = new Rectangle(rectHotkeyVolumeUpTop.X, rectHotkeyVolumeUpTop.Y + HotkeyHeight, HotkeyWidth, HotkeyHeight);
            Rectangle rectHotkeyVolumeDownBottom = new Rectangle(rectHotkeyVolumeDownTop.X, rectHotkeyVolumeDownTop.Y + HotkeyHeight, HotkeyWidth, HotkeyHeight );
            Rectangle rectHotkeyHideBottom = new Rectangle(rectHotkeyHideTop.X, rectHotkeyHideTop.Y + HotkeyHeight, HotkeyWidth, HotkeyHeight);



            g.FillRectangle(GetHotKeyBrush(rectHotkeyPauseTop), rectHotkeyPauseTop);
            g.FillRectangle(GetHotKeyBrush(rectHotkeyPrevTop), rectHotkeyPrevTop);
            g.FillRectangle(GetHotKeyBrush(rectHotkeyNextTop), rectHotkeyNextTop);
            g.FillRectangle(GetHotKeyBrush(rectHotkeyStopTop), rectHotkeyStopTop);
            g.FillRectangle(GetHotKeyBrush(rectHotkeyVolumeUpTop), rectHotkeyVolumeUpTop);
            g.FillRectangle(GetHotKeyBrush(rectHotkeyVolumeDownTop), rectHotkeyVolumeDownTop);
            g.FillRectangle(GetHotKeyBrush(rectHotkeyHideTop), rectHotkeyHideTop);

            g.DrawRectangle(new Pen(_colorLines), rectHotkeyPauseTop);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyPrevTop);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyNextTop);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyStopTop);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyVolumeUpTop);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyVolumeDownTop);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyHideTop);

            g.FillRectangle(new SolidBrush(_colorHotkeyBackground), rectHotkeyPauseBottom);
            g.FillRectangle(new SolidBrush(_colorHotkeyBackground), rectHotkeyPrevBottom);
            g.FillRectangle(new SolidBrush(_colorHotkeyBackground), rectHotkeyNextBottom);
            g.FillRectangle(new SolidBrush(_colorHotkeyBackground), rectHotkeyStopBottom);
            g.FillRectangle(new SolidBrush(_colorHotkeyBackground), rectHotkeyVolumeUpBottom);
            g.FillRectangle(new SolidBrush(_colorHotkeyBackground), rectHotkeyVolumeDownBottom);
            g.FillRectangle(new SolidBrush(_colorHotkeyBackground), rectHotkeyHideBottom);

            g.DrawRectangle(new Pen(_colorLines), rectHotkeyPauseBottom);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyPrevBottom);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyNextBottom);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyStopBottom);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyVolumeUpBottom);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyVolumeDownBottom);
            g.DrawRectangle(new Pen(_colorLines), rectHotkeyHideBottom);

            g.DrawString(_hotkeyLabelPause, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont),rectHotkeyPauseTop);
            g.DrawString(HotkeyLabelPrev, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyPrevTop);
            g.DrawString(HotkeyLabelNext, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyNextTop);
            g.DrawString(HotkeyLabelStop, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyStopTop);
            g.DrawString(HotkeyLabelVolumeUp, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyVolumeUpTop);
            g.DrawString(HotkeyLabelVolumeDown, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyVolumeDownTop);
            g.DrawString(HotkeyLabelHide, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyHideTop);

            g.DrawString(_hotkeyPause, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyPauseBottom);
            g.DrawString(_hotkeyPrev, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyPrevBottom);
            g.DrawString(_hotkeyNext, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyNextBottom);
            g.DrawString(_hotkeyStop, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyStopBottom);
            g.DrawString(_hotkeyVolumeUp, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyVolumeUpBottom);
            g.DrawString(_hotkeyVolumeDown, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyVolumeDownBottom);
            g.DrawString(_hotkeyHide, new Font(OverlayFont, OverlayFontSize), new SolidBrush(_colorFont), rectHotkeyHideBottom);
        }

        /// <summary>
        /// Creates the brush for the hotekey
        /// </summary>
        /// <param name="rect">Rectangle which defines the size of the brush</param>
        /// <returns>Returns the created brush</returns>
        private static Brush GetHotKeyBrush(Rectangle rect){
            return new LinearGradientBrush(rect, _colorHotkeyTop, _colorBackground, 45, true);
        }

        /// <summary>
        /// Calculates the x position of a hotkey
        /// </summary>
        /// <param name="number">The number of the Hotkey</param>
        /// <returns>Returns the x position</returns>
        private static int GetHotkeyXPosition(int number)
        {
            return BorderHorizontal + HotkeyWidth * (number - 1) + HotkeyRightMargin * (number - 1);
        }

        /// <summary>
        /// Creates a GraphicsPath with the size of the rectangle and the
        /// radius
        /// </summary>
        /// <param name="r">Rectangle to draw</param>
        /// <param name="radius">Radius of the corner</param>
        /// <returns></returns>
        private static GraphicsPath RectanglePath(RectangleF r, Single radius)
        {
            // Neues GraphicsPath-Objekt erstellen
            GraphicsPath path = new GraphicsPath();
            Single d = 2 * radius;

            if (radius < 1)
                // keine abgerundeten Ecken
                path.AddRectangle(r);
            else
            {
                // Linien und Bögen erstellen
                path.AddLine(r.X + radius, r.Y, r.X + r.Width - d, r.Y);
                path.AddArc(r.X + r.Width - d, r.Y, d, d, 270, 90);
                path.AddLine(r.X + r.Width, r.Y + radius, r.X + r.Width, r.Y + r.Height - d);
                path.AddArc(r.X + r.Width - d, r.Y + r.Height - d, d, d, 0, 90);
                path.AddLine(r.X + r.Width - d, r.Y + r.Height, r.X + radius, r.Y + r.Height);
                path.AddArc(r.X, r.Y + r.Height - d, d, d, 90, 90);
                path.AddLine(r.X, r.Y + r.Height - d, r.X, r.Y + radius);
                path.AddArc(r.X, r.Y, d, d, 180, 90);
            }
            path.CloseFigure();

            return (path);
        }
    
        /// <summary>
        /// Draws an rounded down rectange with fill
        /// </summary>
        /// <param name="g">Graphics-Objeckt, for the output</param>
        /// <param name="r">Rectangle-Strucutre</param>
        /// <param name="radius">Radius of the rounded corner</param>
        /// <param name="borderColor">Bordercolor of the rectangle</param>
        /// <param name="fillBrush">Brush-Object for the fill</param>
        
        private static void DrawRoundedRectangle(Graphics g, Rectangle r, Single radius, Color borderColor, Brush fillBrush)
        {
            // GraphicsPath erstellen
            GraphicsPath path = RectanglePath(r, radius);

            // Füllung zeichnen
            g.FillPath(fillBrush, path);

            // Rechteck mit angerundeten Ecken zeichnen
            g.DrawPath(new Pen(borderColor), path);

            // Ressourcen freigeben
            path.Dispose();
        }

        /// <summary>
        /// Abgerundetes Rechteck ohne Füllung zeichnen
        /// </summary>
        /// <param name="g">Graphics-Obkjekt, auf das die Ausgabe erfolgen soll</param>
        /// <param name="r">Rechteck-Struktur</param>
        /// <param name="radius">Radius der abgerundeten Ecken</param>
        /// <param name="borderColor">Rahmenfarbe des Rechtecks</param>
        private static void DrawRoundedRectangle(Graphics g, Rectangle r, Single radius, Color borderColor)
        {
          // GraphicsPath erstellen
          GraphicsPath path = RectanglePath(r, radius);
         
          // Rechteck mit angerundeten Ecken zeichnen
          g.DrawPath(new Pen(borderColor), path);
         
          // Ressourcen freigeben
          path.Dispose();
        }


        /// <summary>
        /// Refreshes the overlay
        /// </summary>
        private static void RefreshOverlay()
        {
            MyOverlay.Update();
        }

    }   
}
