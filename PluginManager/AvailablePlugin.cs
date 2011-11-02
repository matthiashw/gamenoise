namespace PluginManager
{
    /// <summary>
    /// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
    /// </summary>
    public class AvailablePlugin : Interfaces.IAvailablePlugin
    {
        //This is the actual AvailablePlugin object.. 
        //Holds an instance of the plugin to access
        //ALso holds assembly path... not really necessary
        private string _myAssemblyPath = "";

        public Interfaces.IPlugin Instance { get; set; }

        public string AssemblyPath
        {
            get { return _myAssemblyPath; }
            set { _myAssemblyPath = value; }
        }
    }
}