using Interfaces;

namespace Webservice
{
    public class MainPlugin : IPlugin, IInterPlugin
    {
        //Declarations of all our internal plugin variables
        private const string MyName = "Webservice";
        private const string MyDescription = "Exchanges information with gamenoise.de";
        private const string MyAuthor = "Mathias Hodler";
        private const string MyVersion = "0.1.0";

        private IPluginHost _myHost;
        private MyView _myMainInterface;
        private MySettings _mySettingsInterface;

        WebserviceClient _myWebserviceClient;

        #region PluginPropertys

        public IPluginHost Host
        {
            get
            {
                return _myHost;
            }
            set
            {
                _myHost = value;
            }
        }
        public string Name
        {
            get { return MyName; }
        }

        public string Description
        {
            get { return MyDescription; }
        }

        public string Author
        {
            get { return MyAuthor; }
        }

        public string Version
        {
            get { return MyVersion; }
        }

        public System.Windows.Controls.UserControl MainInterface
        {
            get { return _myMainInterface; }
        }
        #endregion

        #region PluginUnusedInterfaceMethods
            public bool isAbleToPlayback(string filename)
            {
                return false;
            }

            public int getBassStream(string filename)
            {
                return 0;
            }
        #endregion

        public void Initialize()
        {
            _myWebserviceClient = new WebserviceClient();
            _myMainInterface = new MyView(this, _myWebserviceClient, _myHost);
            _mySettingsInterface = new MySettings();
        }

        public void Dispose()
        { 
        }

        public IWebserviceClient GetWebserviceClient()
        {
            return _myWebserviceClient;
        }


        public System.Collections.ArrayList getDependencies()
        {
            return new System.Collections.ArrayList();
        }


        public System.Windows.Controls.UserControl SettingsInterface
        {
            get { return _mySettingsInterface; }
        }

    }
}
