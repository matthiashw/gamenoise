using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces;

namespace SamplePlugin2
{
    /// <summary>
    /// Plugin1
    /// </summary>
    public class Plugin2 : IPlugin  // <-- See how we inherited the IPlugin interface?
    {
        public Plugin2()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        //Declarations of all our internal plugin variables
        string myName = "another Plugin";
        string myDescription = "A Simple Text Editor. Very Simple!";
        string myAuthor = "Mathias Hodler";
        string myVersion = "1.0.0";
        IPluginHost myHost = null;
        int myWindowWidth = 269;
        int myWindowHeight = 122;


        myView myMainInterface = new myView();
        mySettings mySettingsInterface = new mySettings();

        /// <summary>
        /// Description of the Plugin's purpose
        /// </summary>
        public string Description
        {
            get { return myDescription; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author
        {
            get { return myAuthor; }

        }

        /// <summary>
        /// Host of the plugin.
        /// </summary>
        public IPluginHost Host
        {
            get { return myHost; }
            set { myHost = value; }
        }

        public string Name
        {
            get { return myName; }
        }

        public System.Windows.Controls.UserControl MainInterface
        {
            get { return myMainInterface; }
        }

        public string Version
        {
            get { return myVersion; }
        }

        public int WindowWidth { get { return myWindowWidth; } }
        public int WindowHeight { get { return myWindowHeight; } }

        public void Initialize()
        {
            //This is the first Function called by the host...
            //Put anything needed to start with here first
            myHost.pluginFeedback("Bin da, wer noch?", this);
        }

        public void Dispose()
        {
            //Put any cleanup code in here for when the program is stopped
        }


        #region IPlugin Member


        public bool isAbleToPlayback(string filename)
        {
            return false;
        }

        public int getBassStream(string filename)
        {
            throw new NotImplementedException();
        }

        #endregion

       
        public object pluginSpecial(object foo)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ArrayList getDependencies()
        {
            return new System.Collections.ArrayList();
        }


        public System.Windows.Controls.UserControl SettingsInterface
        {
            get { return mySettingsInterface; }
        }
    }
}
