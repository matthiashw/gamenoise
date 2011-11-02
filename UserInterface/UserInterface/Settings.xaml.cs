/* 
 * author: BK
 * 
 * created: 07.11.2008
 * 
 * modification history
 * --------------------
 * 26.11. MHI constructor now gets a reference to Configuration
 * 
 * 15.11. MHO hotkeys and info windows added
 * 
 * MHI (02.02.09)
 * added 'resumePlayback' Checkbox
 * 
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Organisation;
using System.IO;
using System.Xml.Serialization;
using System.Globalization;
using System.ComponentModel;
using Interfaces;
using System.Collections;

namespace UserInterface
{
    /// <summary>
    /// Interaktionslogik für WindowSettings.xaml
    /// </summary>
    public partial class Settings : IObserver
    {
        /// <summary>
        /// Reference to the the Configuration object
        /// </summary>
        private readonly Configuration _myConfiguration;

        /// <summary>
        /// Converter to convert an input key to a key for hotkeys
        /// </summary>
        private readonly System.Windows.Forms.KeysConverter _converter;

        /// <summary>
        /// Overlay-Position
        /// </summary>
        private Organisation.OverlayPositionEnum _overlayPosition;

        /// <summary>
        /// PluginManager
        /// </summary>
        private IPluginManager _pluginManager;

        /// <summary>
        /// Culture converter
        /// </summary>
        readonly CultureInfoConverter _cultConvert;

        private bool _overlayTimerEnabled;
        private int _overlayTimerSeconds;

        /// <summary>
        /// Constructor to initialize all stuff
        /// </summary>
        /// <param name="aConfiguration">Reference to the the Configuration object</param>
        public Settings(Configuration aConfiguration, IPluginManager aPluginManager)
        {
            _myConfiguration = aConfiguration;
            _myConfiguration.AddObserver(this);
            _pluginManager = aPluginManager;
            _converter = new System.Windows.Forms.KeysConverter();
            _cultConvert = new CultureInfoConverter();
            InitializeComponent();
            WriteThanks();
        }

        /// <summary>
        /// Updates the data manually on initialization
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">EventArgs</param>
        private void Window_Initialized(object sender, EventArgs e)
        {
            CatchSkins();
            CatchPlugins();
            FillLanguageBox(comboBoxLanguage);

            Update(_myConfiguration);
        }

        

        /// <summary>
        /// Catches all available skins
        /// </summary>
        private void CatchSkins()
        {

            comboBoxSkin.Items.Clear();

            ArrayList skins = UserInterfaceHelper.GetDirectories(_myConfiguration.GetSkinPath());

            foreach (string pathSkins in skins)
            {
                comboBoxSkin.Items.Add(pathSkins);
            }
        }

        /// <summary>
        /// Catches all available Plugins
        /// </summary>
        private void CatchPlugins()
        {
            comboBoxPlugin.Items.Clear();

            foreach(Interfaces.IAvailablePlugin plugin  in _pluginManager.AvailablePlugins)
            {
                comboBoxPlugin.Items.Add(plugin.Instance.Name);
            }
        }

        /// <summary>
        /// Catches all available Plugin Skins
        /// </summary>
        private void CatchPluginSkins(String pluginFolder)
        {
            /*comboBoxPluginSkin.Items.Clear();

            ArrayList pluginSkins = UserInterfaceHelper.GetDirectories(_myConfiguration.GetPluginPath() + pluginFolder + "/Skins/");

            foreach (string pathPluginSkins in pluginSkins)
            {
                comboBoxPluginSkin.Items.Add(pathPluginSkins);
            }*/
        }
        
        /// <summary>
        /// Closes the form
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void buttonCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Saves the data and close the window
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">RoutedEventArgs</param>
        private void buttonOKClick(object sender, RoutedEventArgs e)
        {           
            SaveSettings();
            Close();
        }

        /// <summary>
        /// Resets the settings to default
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void buttonLoadDefaultClick(object sender, RoutedEventArgs e)
        {
            _myConfiguration.ResetToDefault();
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonApplyClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        /// <summary>
        /// Writes the text for the Thanks-Box to the textbox
        /// </summary>
        private void WriteThanks()
        {
            textBoxThanks.Text = "\r\nRobert Walter\r\n" +
                                "für seine Arbeit als Projektbetreuer und seiner hilfreichen Unterstützung.\r\n"/* + 
                                "\r\n" +
                                "Christian Traub\r\n" +
                                "für die Mitarbeit an der Projektplanung und Prototyps."*/;
        }

        /// <summary>
        /// Saves all settings
        /// </summary>
        private void SaveSettings()
        {
            //Disable Notify to get a better performance
            _myConfiguration.DisableNotify();

            SaveHotkeys();
            SaveGeneral();
            SaveOverlayColors();
            SaveOverlayPosition();
            SaveOverlayTimerSettings();

            //Enable Notify and update manually
            _myConfiguration.EnableNotify();
            _myConfiguration.Notify();
        }

        #region Hotkey
        /// <summary>
        /// Saves all hotkeys
        /// </summary>
        private void SaveHotkeys()
        {
            //Hotkeys
            _myConfiguration.HotkeyPlay.MAlt = (bool)checkBoxPlayAlt.IsChecked;
            _myConfiguration.HotkeyPlay.MCtrl = (bool)checkBoxPlayCtrl.IsChecked;
            _myConfiguration.HotkeyPlay.MShift = (bool)checkBoxPlayShift.IsChecked;
            _myConfiguration.HotkeyPlay.IsEnabled = (bool)checkBoxPlayEnable.IsChecked;
            _myConfiguration.HotkeyPlay.HKey = (System.Windows.Forms.Keys)_converter.ConvertFromString(textBoxPlay.Text);

            _myConfiguration.HotkeyStop.MAlt = (bool)checkBoxStopAlt.IsChecked;
            _myConfiguration.HotkeyStop.MCtrl = (bool)checkBoxStopCtrl.IsChecked;
            _myConfiguration.HotkeyStop.MShift = (bool)checkBoxStopShift.IsChecked;
            _myConfiguration.HotkeyStop.IsEnabled = (bool)checkBoxStopEnable.IsChecked;
            _myConfiguration.HotkeyStop.HKey = (System.Windows.Forms.Keys)_converter.ConvertFromString(textBoxStop.Text);

            _myConfiguration.HotkeyNext.MAlt = (bool)checkBoxNextAlt.IsChecked;
            _myConfiguration.HotkeyNext.MCtrl = (bool)checkBoxNextCtrl.IsChecked;
            _myConfiguration.HotkeyNext.MShift = (bool)checkBoxNextShift.IsChecked;
            _myConfiguration.HotkeyNext.IsEnabled = (bool)checkBoxNextEnable.IsChecked;
            _myConfiguration.HotkeyNext.HKey = (System.Windows.Forms.Keys)_converter.ConvertFromString(textBoxNext.Text);

            _myConfiguration.HotkeyPrev.MAlt = (bool)checkBoxPrevAlt.IsChecked;
            _myConfiguration.HotkeyPrev.MCtrl = (bool)checkBoxPrevCtrl.IsChecked;
            _myConfiguration.HotkeyPrev.MShift = (bool)checkBoxPrevShift.IsChecked;
            _myConfiguration.HotkeyPrev.IsEnabled = (bool)checkBoxPrevEnable.IsChecked;
            _myConfiguration.HotkeyPrev.HKey = (System.Windows.Forms.Keys)_converter.ConvertFromString(textBoxPrev.Text);

            _myConfiguration.HotkeyOverlay.MAlt = (bool)checkBoxOverlayAlt.IsChecked;
            _myConfiguration.HotkeyOverlay.MCtrl = (bool)checkBoxOverlayCtrl.IsChecked;
            _myConfiguration.HotkeyOverlay.MShift = (bool)checkBoxOverlayShift.IsChecked;
            _myConfiguration.HotkeyOverlay.IsEnabled = (bool)checkBoxOverlayEnable.IsChecked;
            _myConfiguration.HotkeyOverlay.HKey = (System.Windows.Forms.Keys)_converter.ConvertFromString(textBoxOverlay.Text);

            _myConfiguration.HotkeyVolDown.MAlt = (bool)checkBoxVolDownAlt.IsChecked;
            _myConfiguration.HotkeyVolDown.MCtrl = (bool)checkBoxVolDownCtrl.IsChecked;
            _myConfiguration.HotkeyVolDown.MShift = (bool)checkBoxVolDownShift.IsChecked;
            _myConfiguration.HotkeyVolDown.IsEnabled = (bool)checkBoxVolDownEnable.IsChecked;
            _myConfiguration.HotkeyVolDown.HKey = (System.Windows.Forms.Keys)_converter.ConvertFromString(textBoxVolDown.Text);

            _myConfiguration.HotkeyVolUp.MAlt = (bool)checkBoxVolUpAlt.IsChecked;
            _myConfiguration.HotkeyVolUp.MCtrl = (bool)checkBoxVolUpCtrl.IsChecked;
            _myConfiguration.HotkeyVolUp.MShift = (bool)checkBoxVolUpShift.IsChecked;
            _myConfiguration.HotkeyVolUp.IsEnabled = (bool)checkBoxVolUpEnable.IsChecked;
            _myConfiguration.HotkeyVolUp.HKey = (System.Windows.Forms.Keys)_converter.ConvertFromString(textBoxVolUp.Text);
        }

        /// <summary>
        /// Save General Settings
        /// </summary>
        private void SaveGeneral()
        {
            _myConfiguration.Language = ((CultureInfo)comboBoxLanguage.SelectedValue).TextInfo.CultureName;
            _myConfiguration.MinimizeToTray = (bool)checkBoxTray.IsChecked;
            _myConfiguration.UsedSkin = comboBoxSkin.Text;
            _myConfiguration.ResumePlayback = (bool)checkBoxResumePlay.IsChecked;
        }

        /// <summary>
        /// Try to convert the pressed key in a hotkey and set it to the textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBoxPlay_KeyUp(object sender, KeyEventArgs e)
        {
            String key = "";
            if (!TryKey(e, ref key)) return;
            textBoxPlay.Text = key;
            checkBoxPlayEnable.IsChecked = true;
        }

        /// <summary>
        /// Try to convert the pressed key in a hotkey and set it to the textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBoxStop_KeyUp(object sender, KeyEventArgs e)
        {
            String key = "";
            if (!TryKey(e, ref key)) return;
            textBoxStop.Text = key;
            checkBoxStopEnable.IsChecked = true;
        }

        /// <summary>
        /// Try to convert the pressed key in a hotkey and set it to the textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBoxNext_KeyUp(object sender, KeyEventArgs e)
        {
            String key = "";
            if (!TryKey(e, ref key)) return;
            textBoxNext.Text = key;
            checkBoxNextEnable.IsChecked = true;
        }

        /// <summary>
        /// Try to convert the pressed key in a hotkey and set it to the textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBoxPrev_KeyUp(object sender, KeyEventArgs e)
        {
            String key = "";
            if (!TryKey(e, ref key)) return;
            textBoxPrev.Text = key;
            checkBoxPrevEnable.IsChecked = true;
        }

        /// <summary>
        /// Try to convert the pressed key in a hotkey and set it to the textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBoxOverlay_KeyUp(object sender, KeyEventArgs e)
        {
            String key = "";
            if (!TryKey(e, ref key)) return;
            textBoxOverlay.Text = key;
            checkBoxOverlayEnable.IsChecked = true;
        }

        /// <summary>
        /// Try to convert the pressed key in a hotkey and set it to the textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBoxVolUp_KeyUp(object sender, KeyEventArgs e)
        {
            String key = "";
            if (!TryKey(e, ref key)) return;
            textBoxVolUp.Text = key;
            checkBoxVolUpEnable.IsChecked = true;
        }

        /// <summary>
        /// Try to convert the pressed key in a hotkey and set it to the textbox
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBoxVolDown_KeyUp(object sender, KeyEventArgs e)
        {
            String key = "";
            if (!TryKey(e, ref key)) return;
            textBoxVolDown.Text = key;
            checkBoxVolDownEnable.IsChecked = true;
        }


        /// <summary>
        /// Try to convert convert a insert key to a hotkey-type
        /// </summary>
        /// <param name="e">Pressed key</param>
        /// <param name="newString">converted key as string</param>
        /// <returns>Resturn the success of the conversion</returns>
        private bool TryKey(KeyEventArgs e, ref String newString)
        {
            String key = e.Key.ToString();

            // Some keys can't be converted. 
            // So we need to do it separat.
            switch (e.Key)
            {
                case Key.OemPlus:
                    key = "Oemplus";
                    break;
            }

            newString = key;

            try
            {
                _converter.ConvertFromString(key);
                return true;
            }
            catch (NotSupportedException)
            {
                new Error("This hotkey is locked!", false, this);
            }
            catch (Exception)
            {
                new Error("This hotkey is locked!", false,this);
            }
            
            return false;
        }

        /// <summary>
        /// This function catches the pressed key and hides it on the textbox
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyEventArgs</param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

        #region OverlayColors
        /// <summary>
        /// Saves all OverlayColors
        /// </summary>
        private void SaveOverlayColors()
        {
            //OverlayColors
            _myConfiguration.OverlayColorBack.Alpha = ((SolidColorBrush)rectangleColorBack.Fill).Color.A;
            _myConfiguration.OverlayColorBack.Red = ((SolidColorBrush)rectangleColorBack.Fill).Color.R;
            _myConfiguration.OverlayColorBack.Green = ((SolidColorBrush)rectangleColorBack.Fill).Color.G;
            _myConfiguration.OverlayColorBack.Blue = ((SolidColorBrush)rectangleColorBack.Fill).Color.B;

            _myConfiguration.OverlayColorHKTop.Alpha = ((SolidColorBrush)rectangleColorHKTop.Fill).Color.A;
            _myConfiguration.OverlayColorHKTop.Red = ((SolidColorBrush)rectangleColorHKTop.Fill).Color.R;
            _myConfiguration.OverlayColorHKTop.Green = ((SolidColorBrush)rectangleColorHKTop.Fill).Color.G;
            _myConfiguration.OverlayColorHKTop.Blue = ((SolidColorBrush)rectangleColorHKTop.Fill).Color.B;

            _myConfiguration.OverlayColorHKBack.Alpha = ((SolidColorBrush)rectangleColorHKBack.Fill).Color.A;
            _myConfiguration.OverlayColorHKBack.Red = ((SolidColorBrush)rectangleColorHKBack.Fill).Color.R;
            _myConfiguration.OverlayColorHKBack.Green = ((SolidColorBrush)rectangleColorHKBack.Fill).Color.G;
            _myConfiguration.OverlayColorHKBack.Blue = ((SolidColorBrush)rectangleColorHKBack.Fill).Color.B;

            _myConfiguration.OverlayColorFont.Alpha = ((SolidColorBrush)rectangleColorFont.Fill).Color.A;
            _myConfiguration.OverlayColorFont.Red = ((SolidColorBrush)rectangleColorFont.Fill).Color.R;
            _myConfiguration.OverlayColorFont.Green = ((SolidColorBrush)rectangleColorFont.Fill).Color.G;
            _myConfiguration.OverlayColorFont.Blue = ((SolidColorBrush)rectangleColorFont.Fill).Color.B;

            _myConfiguration.OverlayColorLine.Alpha = ((SolidColorBrush)rectangleColorLine.Fill).Color.A;
            _myConfiguration.OverlayColorLine.Red = ((SolidColorBrush)rectangleColorLine.Fill).Color.R;
            _myConfiguration.OverlayColorLine.Green = ((SolidColorBrush)rectangleColorLine.Fill).Color.G;
            _myConfiguration.OverlayColorLine.Blue = ((SolidColorBrush)rectangleColorLine.Fill).Color.B;

            _myConfiguration.OverlayColorProgress.Alpha = ((SolidColorBrush)rectangleColorProgress.Fill).Color.A;
            _myConfiguration.OverlayColorProgress.Red = ((SolidColorBrush)rectangleColorProgress.Fill).Color.R;
            _myConfiguration.OverlayColorProgress.Green = ((SolidColorBrush)rectangleColorProgress.Fill).Color.G;
            _myConfiguration.OverlayColorProgress.Blue = ((SolidColorBrush)rectangleColorProgress.Fill).Color.B;
        }

        #endregion


        #region OverlayPosition
        
        /// <summary>
        /// Saves the Overlay-Position
        /// </summary>
        private void SaveOverlayPosition()
        {
            _myConfiguration.OverlayPosition = _overlayPosition;
        }

        #endregion


        #region OverlayTimer
        /// <summary>
        /// Saves the Overlay-Timer-Settings
        /// </summary>
        private void SaveOverlayTimerSettings()
        {
            _myConfiguration.OverlayTimerEnabled = _overlayTimerEnabled;
            _myConfiguration.OverlayTimerSeconds = Math.Min(_overlayTimerSeconds, 1000000);
        }

        #endregion


        /// <summary>
        /// Updates all information
        /// </summary>
        /// <param name="subject">object</param>
        public void Update(object subject)
        {
            if (!(subject is Configuration)) return;
            
            //Hotkeys
            UpdateHotkeys(subject);

            //OverlayColors
            UpdateColors(subject);

            _overlayPosition = ((Configuration)subject).OverlayPosition;
            
            //OverlayPosition
            switch (_overlayPosition)
            {
                case Organisation.OverlayPositionEnum.TOP:
                    radioButtonTop.IsChecked = true;
                    break;
                case Organisation.OverlayPositionEnum.Topleft:
                    radioButtonTopLeft.IsChecked = true;
                    break;
                case Organisation.OverlayPositionEnum.Topright:
                    radioButtonTopRight.IsChecked = true;
                    break;
                case Organisation.OverlayPositionEnum.Bottom:
                    radioButtonBottom.IsChecked = true;
                    break;
                case Organisation.OverlayPositionEnum.Bottomleft:
                    radioButtonBottomLeft.IsChecked = true;
                    break;
                case Organisation.OverlayPositionEnum.Bottomright:
                    radioButtonBottomRight.IsChecked = true;
                    break;
            }

            _overlayTimerEnabled = ((Configuration)subject).OverlayTimerEnabled;
            _overlayTimerSeconds = ((Configuration)subject).OverlayTimerSeconds;
            if (_overlayTimerEnabled)
                radioButtonVisSeconds.IsChecked = true;
            else
                radioButtonVisAlways.IsChecked = true;
            textBoxVisSeconds.Text = Convert.ToString(_overlayTimerSeconds);

            comboBoxSkin.SelectedValue = ((Configuration)subject).UsedSkin;
            checkBoxTray.IsChecked = ((Configuration)subject).MinimizeToTray;
            checkBoxResumePlay.IsChecked = ((Configuration)subject).ResumePlayback;
                
            comboBoxLanguage.SelectedValue = _cultConvert.ConvertFromString(((Configuration)subject).Language);

            UpdatePreview();
        }

        /// <summary>
        /// update Color Information
        /// </summary>
        /// <param name="subject">object</param>
        private void UpdateColors(object subject)
        {
            Color tmpColor = new Color
            {
                A = ((Configuration)subject).OverlayColorBack.Alpha,
                R = ((Configuration)subject).OverlayColorBack.Red,
                G = ((Configuration)subject).OverlayColorBack.Green,
                B = ((Configuration)subject).OverlayColorBack.Blue
            };
            
            SolidColorBrush backColorBrush = new SolidColorBrush(tmpColor);
            rectangleColorBack.Fill = backColorBrush;

            tmpColor.A = ((Configuration)subject).OverlayColorHKTop.Alpha;
            tmpColor.R = ((Configuration)subject).OverlayColorHKTop.Red;
            tmpColor.G = ((Configuration)subject).OverlayColorHKTop.Green;
            tmpColor.B = ((Configuration)subject).OverlayColorHKTop.Blue;
            SolidColorBrush hkTopColorBrush = new SolidColorBrush(tmpColor);
            rectangleColorHKTop.Fill = hkTopColorBrush;

            tmpColor.A = ((Configuration)subject).OverlayColorHKBack.Alpha;
            tmpColor.R = ((Configuration)subject).OverlayColorHKBack.Red;
            tmpColor.G = ((Configuration)subject).OverlayColorHKBack.Green;
            tmpColor.B = ((Configuration)subject).OverlayColorHKBack.Blue;
            SolidColorBrush hkBackColorBrush = new SolidColorBrush(tmpColor);
            rectangleColorHKBack.Fill = hkBackColorBrush;

            tmpColor.A = ((Configuration)subject).OverlayColorFont.Alpha;
            tmpColor.R = ((Configuration)subject).OverlayColorFont.Red;
            tmpColor.G = ((Configuration)subject).OverlayColorFont.Green;
            tmpColor.B = ((Configuration)subject).OverlayColorFont.Blue;
            SolidColorBrush fontColorBrush = new SolidColorBrush(tmpColor);
            rectangleColorFont.Fill = fontColorBrush;

            tmpColor.A = ((Configuration)subject).OverlayColorLine.Alpha;
            tmpColor.R = ((Configuration)subject).OverlayColorLine.Red;
            tmpColor.G = ((Configuration)subject).OverlayColorLine.Green;
            tmpColor.B = ((Configuration)subject).OverlayColorLine.Blue;
            SolidColorBrush lineColorBrush = new SolidColorBrush(tmpColor);
            rectangleColorLine.Fill = lineColorBrush;

            tmpColor.A = ((Configuration)subject).OverlayColorProgress.Alpha;
            tmpColor.R = ((Configuration)subject).OverlayColorProgress.Red;
            tmpColor.G = ((Configuration)subject).OverlayColorProgress.Green;
            tmpColor.B = ((Configuration)subject).OverlayColorProgress.Blue;
            SolidColorBrush progressColorBrush = new SolidColorBrush(tmpColor);
            rectangleColorProgress.Fill = progressColorBrush;
        }

        /// <summary>
        /// updates Hotkey Information
        /// </summary>
        /// <param name="subject">object</param>
        private void UpdateHotkeys(object subject)
        {
            checkBoxPlayAlt.IsChecked = ((Configuration)subject).HotkeyPlay.MAlt;
            checkBoxPlayCtrl.IsChecked = ((Configuration)subject).HotkeyPlay.MCtrl;
            checkBoxPlayShift.IsChecked = ((Configuration)subject).HotkeyPlay.MShift;
            checkBoxPlayEnable.IsChecked = ((Configuration)subject).HotkeyPlay.IsEnabled;
            textBoxPlay.Text = ((Configuration)subject).HotkeyPlay.HKey.ToString();
                
            checkBoxStopAlt.IsChecked = ((Configuration)subject).HotkeyStop.MAlt;
            checkBoxStopCtrl.IsChecked = ((Configuration)subject).HotkeyStop.MCtrl;
            checkBoxStopShift.IsChecked = ((Configuration)subject).HotkeyStop.MShift;
            checkBoxStopEnable.IsChecked = ((Configuration)subject).HotkeyStop.IsEnabled;
            textBoxStop.Text = ((Configuration)subject).HotkeyStop.HKey.ToString();

            checkBoxNextAlt.IsChecked = ((Configuration)subject).HotkeyNext.MAlt;
            checkBoxNextCtrl.IsChecked = ((Configuration)subject).HotkeyNext.MCtrl;
            checkBoxNextShift.IsChecked = ((Configuration)subject).HotkeyNext.MShift;
            checkBoxNextEnable.IsChecked = ((Configuration)subject).HotkeyNext.IsEnabled;
            textBoxNext.Text = ((Configuration)subject).HotkeyNext.HKey.ToString();

            checkBoxPrevAlt.IsChecked = ((Configuration)subject).HotkeyPrev.MAlt;
            checkBoxPrevCtrl.IsChecked = ((Configuration)subject).HotkeyPrev.MCtrl;
            checkBoxPrevShift.IsChecked = ((Configuration)subject).HotkeyPrev.MShift;
            checkBoxPrevEnable.IsChecked = ((Configuration)subject).HotkeyPrev.IsEnabled;
            textBoxPrev.Text = ((Configuration)subject).HotkeyPrev.HKey.ToString();

            checkBoxOverlayAlt.IsChecked = ((Configuration)subject).HotkeyOverlay.MAlt;
            checkBoxOverlayCtrl.IsChecked = ((Configuration)subject).HotkeyOverlay.MCtrl;
            checkBoxOverlayShift.IsChecked = ((Configuration)subject).HotkeyOverlay.MShift;
            checkBoxOverlayEnable.IsChecked = ((Configuration)subject).HotkeyOverlay.IsEnabled;
            textBoxOverlay.Text = ((Configuration)subject).HotkeyOverlay.HKey.ToString();

            checkBoxVolDownAlt.IsChecked = ((Configuration)subject).HotkeyVolDown.MAlt;
            checkBoxVolDownCtrl.IsChecked = ((Configuration)subject).HotkeyVolDown.MCtrl;
            checkBoxVolDownShift.IsChecked = ((Configuration)subject).HotkeyVolDown.MShift;
            checkBoxVolDownEnable.IsChecked = ((Configuration)subject).HotkeyVolDown.IsEnabled;
            textBoxVolDown.Text = ((Configuration)subject).HotkeyVolDown.HKey.ToString();

            checkBoxVolUpAlt.IsChecked = ((Configuration)subject).HotkeyVolUp.MAlt;
            checkBoxVolUpCtrl.IsChecked = ((Configuration)subject).HotkeyVolUp.MCtrl;
            checkBoxVolUpShift.IsChecked = ((Configuration)subject).HotkeyVolUp.MShift;
            checkBoxVolUpEnable.IsChecked = ((Configuration)subject).HotkeyVolUp.IsEnabled;
            textBoxVolUp.Text = ((Configuration)subject).HotkeyVolUp.HKey.ToString();
        }

        /// <summary>
        /// Updates the preview image for the skin
        /// </summary>
        private void UpdatePreview()
        {
            ImageSourceConverter converter = new ImageSourceConverter();

            String file = _myConfiguration.GetSkinPath() + comboBoxSkin.Text + "/preview.jpg";
            if (File.Exists(file))
            {
                imagePreview.Source = (ImageSource)converter.ConvertFromString(file);
            }
            else
            {
                imagePreview.Source = null;
            }
        }

        /// <summary>
        /// When closing the window all settings will be stored.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">System.ComponentModel.CancelEventArgs</param>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            XmlSerializer mySerializer = new
            XmlSerializer(typeof(Configuration));
            
            // To write to a file, create a StreamWriter object.
            try
            {
                StreamWriter myWriter = new StreamWriter(_myConfiguration.PathSettings);
                mySerializer.Serialize(myWriter, _myConfiguration);
                myWriter.Close();
            }
            catch (UnauthorizedAccessException)
            {
                new Error("Failed to write Settings: Unauthorized Access", false, this);
            }
            catch (ArgumentException)
            {
                new Error("Failed to write Settings: Argument Error", false, this);
            }
            catch (DirectoryNotFoundException)
            {
                new Error("Failed to write Settings: Directory Not Found", false, this);
            }
            catch (PathTooLongException)
            {
                new Error("Failed to write Settings: Path too long", false, this);
            }
            catch (IOException)
            {
                new Error("Failed to write Settings: IO Error", false, this);
            }
            catch (System.Security.SecurityException)
            {
                new Error("Failed to write Settings: Security Error", false, this);
            }


            _myConfiguration.RemoveObserver(this);
        }


        /// <summary>
        /// Dialog Color Background
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonColorBack_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            ColorPickerDlg dlg = new ColorPickerDlg
                                     {
                                         Owner = this,
                                         ColorBrush = (SolidColorBrush) rectangleColorBack.Fill,
                                         Left = Left + 250,
                                         Top = Top + 260
                                     };

            // Configure the dialog box

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                rectangleColorBack.Fill = dlg.ColorBrush;
            }
        }

        /// <summary>
        /// Dialog Color Hotkey Top
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonColorHKTop_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            ColorPickerDlg dlg = new ColorPickerDlg
                                     {
                                         Owner = this,
                                         ColorBrush = (SolidColorBrush) rectangleColorHKTop.Fill,
                                         Left = Left + 250,
                                         Top = Top + 290
                                     };

            // Configure the dialog box

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                rectangleColorHKTop.Fill = dlg.ColorBrush;
            }
        }

        /// <summary>
        /// Dialog Hotkey Background
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonColorHKBack_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            ColorPickerDlg dlg = new ColorPickerDlg
                                     {
                                         Owner = this,
                                         ColorBrush = (SolidColorBrush) rectangleColorHKBack.Fill,
                                         Left = Left + 250,
                                         Top = Top + 290
                                     };

            // Configure the dialog box

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                rectangleColorHKBack.Fill = dlg.ColorBrush;
            }
        }

        /// <summary>
        /// Dialog Font Color
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonColorFont_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            ColorPickerDlg dlg = new ColorPickerDlg
                                     {
                                         Owner = this,
                                         ColorBrush = (SolidColorBrush) rectangleColorFont.Fill,
                                         Left = Left + 230,
                                         Top = Top + 260
                                     };

            // Configure the dialog box

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                rectangleColorFont.Fill = dlg.ColorBrush;
            }
        }

        /// <summary>
        /// Dialog Line Color
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonColorLine_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            ColorPickerDlg dlg = new ColorPickerDlg
                                     {
                                         Owner = this,
                                         ColorBrush = (SolidColorBrush) rectangleColorLine.Fill,
                                         Left = Left + 230,
                                         Top = Top + 290
                                     };

            // Configure the dialog box

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                rectangleColorLine.Fill = dlg.ColorBrush;
            }
        }

        /// <summary>
        /// Dialog Progress-Bar Color
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void buttonColorProgress_Click(object sender, RoutedEventArgs e)
        {
            // Instantiate the dialog box
            ColorPickerDlg dlg = new ColorPickerDlg
                                     {
                                         Owner = this,
                                         ColorBrush = (SolidColorBrush) rectangleColorProgress.Fill,
                                         Left = Left + 230,
                                         Top = Top + 290
                                     };

            // Configure the dialog box

            // Open the dialog box modally 
            dlg.ShowDialog();

            // Process data entered by user if dialog box is accepted
            if (dlg.DialogResult == true)
            {
                rectangleColorProgress.Fill = dlg.ColorBrush;
            }
        }


        /// <summary>
        /// Overlay Top Left Corner
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonTopLeft_Checked(object sender, RoutedEventArgs e)
        {
            rectangleOverlay.Margin = new Thickness(1, 1, 1, 1);
            _overlayPosition = Organisation.OverlayPositionEnum.Topleft;
        }

        /// <summary>
        /// Overlay Top 
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonTop_Checked(object sender, RoutedEventArgs e)
        {
            rectangleOverlay.Margin = new Thickness(26, 1, 1, 1);
            _overlayPosition = Organisation.OverlayPositionEnum.TOP;
        }

        /// <summary>
        /// Overlay Top Right Corner
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonTopRight_Checked(object sender, RoutedEventArgs e)
        {
            rectangleOverlay.Margin = new Thickness(52, 1, 1, 1);
            _overlayPosition = Organisation.OverlayPositionEnum.Topright;
        }

        /// <summary>
        /// Overlay Bottom Left Corner
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonBottomLeft_Checked(object sender, RoutedEventArgs e)
        {
            rectangleOverlay.Margin = new Thickness(1, 59, 1, 1);
            _overlayPosition = Organisation.OverlayPositionEnum.Bottomleft;
        }

        /// <summary>
        /// Overlay Bottom
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonBottom_Checked(object sender, RoutedEventArgs e)
        {
            rectangleOverlay.Margin = new Thickness(26, 59, 1, 1);
            _overlayPosition = Organisation.OverlayPositionEnum.Bottom;
        }

        /// <summary>
        /// Overlay Bottom Right Corner
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonBottomRight_Checked(object sender, RoutedEventArgs e)
        {
            rectangleOverlay.Margin = new Thickness(52, 59, 1, 1);
            _overlayPosition = Organisation.OverlayPositionEnum.Bottomright;
        }

        /// <summary>
        /// Overlay Timer Seconds
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void textBoxVisSeconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxVisSeconds.Text == "") return;
            try
            {
                _overlayTimerSeconds = Math.Max(Convert.ToInt32(textBoxVisSeconds.Text), 1);
            }
            catch (OverflowException)
            {
                //_overlayTimerSeconds = 100000;
            }
            catch (FormatException)
            {}

            textBoxVisSeconds.Text = _overlayTimerSeconds.ToString();
        }

        private void textBoxVisSeconds_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !AreAllValidNumericChars(e.Text);
            base.OnPreviewTextInput(e);
        }

        private static bool AreAllValidNumericChars(string str)
        {
            bool ret = true;
            if (str == NumberFormatInfo.CurrentInfo.NumberDecimalSeparator |
                str == NumberFormatInfo.CurrentInfo.PerMilleSymbol |
                str == NumberFormatInfo.CurrentInfo.PositiveInfinitySymbol |
                str == NumberFormatInfo.CurrentInfo.PositiveSign)
                return true;

            int l = str.Length;
            for (int i = 0; i < l; i++)
            {
                char ch = str[i];
                ret &= Char.IsDigit(ch);
            }

            return ret;
        }

        /// <summary>
        /// Overlay Timer Enabled
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonVisSeconds_Checked(object sender, RoutedEventArgs e)
        {
            _overlayTimerEnabled = true;
        }

        /// <summary>
        /// Overlay Always Visible
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void radioButtonVisAlways_Checked(object sender, RoutedEventArgs e)
        {
            _overlayTimerEnabled = false;
        }

        /// <summary>
        /// Skin Preview Changed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void comboBoxSkin_DropDownClosed(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Selected Plugin Changed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void comboBoxPlugin_DropDownClosed(object sender, EventArgs e)
        {
            pluginCanvas.Children.Clear();
            pluginCanvas.Children.Add(_pluginManager.GetPluginByName(comboBoxPlugin.Text).Instance.SettingsInterface);
        }

        /// <summary>
        /// Plugin Preview Changed
        /// </summary>
        /// <param name="sender">not used</param>
        /// <param name="e">not used</param>
        private void comboBoxPluginSkin_DropDownClosed(object sender, EventArgs e)
        {
        }

        private void FillLanguageBox(ItemsControl cb)
        {
            cb.Items.Clear();
            ArrayList folders = new ArrayList();
            
            //Get all folder in app dir.
            try
            {
                folders = UserInterfaceHelper.GetDirectories(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            }
            catch (PathTooLongException)
            {
                new Error("Path too long in FillLanguageBox", false, this);
            }
            catch (ArgumentException)
            {
                new Error("Argument Error in FillLanguageBox", false, this);
            }
            
            
            //String[] folders = Directory.GetDirectories(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location));
            CultureInfo[] cultures = CultureUtils.GetNeutralCultures();

            foreach (String folder in folders)
            {
                foreach (CultureInfo culture in cultures)
                {
                    String[] onlyFolderArray = folder.Split('\\');
                    String onlyFolder = onlyFolderArray[onlyFolderArray.Length - 1];

                    if (culture.TextInfo.CultureName != null)
                        if (onlyFolder.Equals(culture.TextInfo.CultureName))
                            cb.Items.Add(_cultConvert.ConvertFromString(onlyFolder));
                }
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Hyperlink.NavigateUri.ToString());
            }
            catch (Win32Exception)
            {
                
            }
            catch (ObjectDisposedException)
            {
                
            }
            catch (FileNotFoundException)
            {
                
            }
        }

    }

    

}
