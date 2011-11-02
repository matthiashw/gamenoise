using System;
namespace PluginManager
{
    public interface IAvailablePlugins : System.Collections.IList
    {
        void Add(IAvailablePlugin pluginToAdd);
        IAvailablePlugin Find(string pluginNameOrPath);
        void Remove(IAvailablePlugin pluginToRemove);
    }
}
