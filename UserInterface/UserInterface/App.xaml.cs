/* 
 * author: BK
 * 
 * created: 07.11.2008
 * 
 * modification history
 * --------------------
 * AK,PL,MHI,MHO(20.11.08):
 * Reference added in aPlayControler
 * 
 * MHI (26.11.08)
 * deleted References for Settings, LoadEQ, SaveEQ, not needed anymore
 * 
 */

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Microsoft.VisualBasic.ApplicationServices;


//Gamenoise Namespaces
using Organisation;
using PlayControl;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.Globalization;
using Interfaces;

namespace UserInterface
{
    /// <summary>
    /// Entry Point for the Entire Project
    /// </summary>
    public class EntryPoint
    {
        /// <summary>
        /// Main Method 
        /// </summary>
        /// <param name="args">Arguments from Windows</param>
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceManager manager = new SingleInstanceManager();
            manager.Run(args);
        }
    }

    /// <summary>
    /// A Wrapper for The WindowsFormsApplicationBase-Manager to enable Multiple Instances Detection to avoid Multiple Instances
    /// </summary>
    public class SingleInstanceManager : WindowsFormsApplicationBase
    {
        /// <summary>
        /// Our Application
        /// </summary>
        SingleInstanceApplication _app;

        /// <summary>
        /// Constructer That disables Multiple Instances
        /// </summary>
        public SingleInstanceManager()
        {
            IsSingleInstance = true;
        }

        /// <summary>
        /// Method for first Startup
        /// </summary>
        /// <param name="e">StartUpArgs (not used)</param>
        /// <returns>Startup Succesful</returns>
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            // First time app is launched
            _app = new SingleInstanceApplication();

            try
            {
                _app.Run();
            }
            catch (InvalidOperationException ex)
            {
                new Error(ex.Message, true, null);
            }
            catch (Win32Exception ex)
            {
                new Error(ex.Message, true, null);
            }
            return false;
        }

        /// <summary>
        /// Multiple Instances call this Method
        /// </summary>
        /// <param name="eventArgs">Startup Events</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            // Subsequent launches
            base.OnStartupNextInstance(eventArgs);
            _app.Activate(eventArgs);
        }
    }

    /// <summary>
    /// A Wrapper for the Single Instance Application from WindowBase
    /// </summary>
    public class SingleInstanceApplication : Application
    {
        /// <summary>
        /// StartUp Point for Single Application: Fileargs get here
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(System.Windows.StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Create and show the application's main window
           App window = new App();
           MainWindow = window.GetStandard();
           if(e.Args!=null)
           ((Standard)window.GetStandard()).AddFilesAndFolders(e.Args, true);
        }

        /// <summary>
        /// Reactivate the first Window
        /// </summary>
        /// <param name="e"></param>
        public void Activate(StartupNextInstanceEventArgs e )
        {
            // Reactivate application's main window
            MainWindow.Activate();
            if (e.CommandLine != null)
                ((Standard)MainWindow).AddFilesAndFolders(e.CommandLine.ToArray(), true);
        }
    }

    /// <summary>
    /// Application Entry Point
    /// </summary>
    public class App 
    {
        /// <summary>
        /// Main Window
        /// </summary>
        private Standard aStandard;

        /// <summary>
        /// Constructor for Main Window
        /// </summary>
        public App()
        {
            SplashScreen sp = new SplashScreen("pictures/splashscreen2.png");
            sp.Show(true);

            try
            {
                //Initialising Classes
                GameNoiseList aPlaylist = new GameNoiseList();
                Configuration aConfiguration = LoadSettings();
                PluginManager.PluginManager aPluginManager = new PluginManager.PluginManager();

                //Load Language
                Thread.CurrentThread.CurrentCulture = new CultureInfo(aConfiguration.Language);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(aConfiguration.Language);

                aStandard = new Standard(this, aConfiguration, aPluginManager);
                Ingame aIngame;

                try
                {
                    aIngame = new Ingame(aConfiguration, this);
                }
                catch (Exception)
                {
                    aIngame = null;
                }


                //Get Window Handler and initialize BASS
                Interfaces.IBassWrapper aBassWrapper = new BassWrapper(new WindowInteropHelper(aStandard).Handle);

                //Setting references in aStandard
                aStandard.ABassWrapperGui = (Interfaces.IBassWrapperGui)aBassWrapper;

                //Setting references in aPlayControler
                PlayControler aPlayControler = new PlayControler(aConfiguration.PathTmpEqualizer);
                aStandard.APlayControler = aPlayControler;
                aPlayControler.TmpPlaylist = aPlaylist;
                aPlayControler.ABassWrapper = aBassWrapper;
                aBassWrapper.aPlayControler = aPlayControler;
                aBassWrapper.aPluginManager = aPluginManager;

                aStandard.APlaylist = aPlaylist;
                aStandard.AIngame = aIngame;
                aStandard.LoadHotKeys();

                aPlaylist.AddObserver(aStandard);
                aBassWrapper.AddObserver(aStandard);
                aConfiguration.AddObserver(aStandard);
                aPlayControler.AddObserver(aStandard);

                //Ingame references
                if (aIngame != null)
                {
                    aPlaylist.AddObserver(aIngame);
                    aBassWrapper.AddObserver(aIngame);
                    aConfiguration.AddObserver(aIngame);
                }

                
                //Setting references in GameNoiseList
                aPlaylist.ABassWrapperOrganisation = (Interfaces.IBassWrapperOrganisation)aBassWrapper;
                aPlaylist.APluginManager = aPluginManager;

                //Show Standard GUI
                aStandard.Show();
                aConfiguration.InitNotify();
            }
            catch (FileNotFoundException e)
            {
                new Error("File: " + e.FileName + " not found!", true,aStandard);
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                new Error("Equalizer Settings File is courrupt: " + e.Message, false, aStandard);
            }
            catch (BassWrapperException e)
            {
                new Error(e.GetMessage(), false, aStandard);
            }
            catch (DllNotFoundException e)
            {
                new Error(e.Message, true, aStandard);
            }
            catch (NotSupportedException)
            {}
            catch (ArgumentNullException)
            {}
            catch(ArgumentException)
            {}


        }

        /// <summary>
        /// Get Method to get a Window Handle
        /// </summary>
        /// <returns>Window Handle to Main Window</returns>
        public Window GetStandard() 
        { 
            return aStandard;
        }

        /// <summary>
        /// This Method is loading the Configuration Files from your Home Dir and returns them
        /// </summary>
        /// <returns>Configuration Settings</returns>
        private static Configuration LoadSettings()
        {
            Configuration myConfig;

            try
            {   
                // Construct an instance of the XmlSerializer with the type
                // of object that is being deserialized.
                XmlSerializer mySerializer = new XmlSerializer(typeof(Configuration));
                
                // To read the file, create a FileStream.
                FileStream myFileStream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Gamenoise\\settings.xml", FileMode.Open);
                
                // Call the Deserialize method and cast to the object type.
                myConfig = (Configuration)
                mySerializer.Deserialize(myFileStream);
            }
            catch(Exception)
            {
                myConfig = new Configuration();
            }

            myConfig.Init();

            return myConfig;
        }

    }
}
