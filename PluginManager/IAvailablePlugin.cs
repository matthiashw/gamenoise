using Plugin;

namespace PluginManager
{
    public interface IAvailablePlugin
    {
        string AssemblyPath { get; set; }
        IPlugin Instance { get; set; }
    }
}
