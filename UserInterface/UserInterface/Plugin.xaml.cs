using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using Organisation;

namespace UserInterface
{
    /// <summary>
    /// Interaktionslogik für Plugin.xaml
    /// </summary>
    public partial class Plugin
    {
        private readonly Interfaces.IPlugin _aPlugin;

        public Plugin(Interfaces.IPlugin aPlugin)
        {
            _aPlugin = aPlugin;
            InitializeComponent();
            pluginwindow.Title = aPlugin.Name;

            //this.Height = myPlugin.WindowHeight;
            //this.Width = myPlugin.WindowWidth;

            //Clear the current panel of any other plugin controls... 
            //Note: this only affects visuals.. doesn't close the instance of the plugin
            //pluginCanvas.Children.Clear();

            //Finally, add the usercontrol to the panel... Tadah!
            pluginCanvas.Children.Add(aPlugin.MainInterface);
        }

        public Interfaces.IPlugin GetPlugin()
        {
            return _aPlugin;
            //test
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

/*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pluginCanvas.Children.Clear();
            this.Close();
        }
*/

        /// <summary>
        /// Apply a skin to the form
        /// </summary>
        /// <param name="skinName">Name of the skin</param>
        /// <param name="aConfiguration"></param>
        /// <param name="anAppHandle"></param>
        public void ApplyPluginSkin(String skinName, Configuration aConfiguration, App anAppHandle)
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
                //Get all files in the directory
                string[] files = Directory.GetFiles(aConfiguration.GetPluginPath() + _aPlugin.Name + "/Skins/" + skinName);
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
            }
            catch (DirectoryNotFoundException)
            {
                // Use FallBackSkin if the current skin is in the plugin not available
                if (skinName != aConfiguration.getFallBackSkin())
                {
                    ApplyPluginSkin(aConfiguration.getFallBackSkin(), aConfiguration, anAppHandle);
                }
                else
                {
                    new Error("Skin Dictionary Missing", false, null);
                }
            }
            catch (FileNotFoundException)
            {
                new Error("Skin File Missing", false, null);
            }
            catch (PathTooLongException)
            {
                new Error("Skin Path too long", false, null);
            }
            catch (ArgumentNullException)
            {
                new Error("Null Argument in ApplyPluginSkin", false, null);
            }
            catch (ArgumentException)
            {
                new Error("Wrong Argument in ApplyPluginSkin", false, null);
            }
            catch (IOException)
            {
                new Error("IO Error in ApplyPluginSkin", false, null);
            }
            catch (UnauthorizedAccessException)
            {
                new Error("Unauthorized Access in ApplyPluginSkin", false, null);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            pluginCanvas.Children.Clear();
        }
    }
}
