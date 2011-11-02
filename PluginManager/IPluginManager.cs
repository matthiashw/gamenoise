using System;
using Plugin;

namespace PluginManager
{
    public interface IPluginManager
    {
        IAvailablePlugins AvailablePlugins { get; set; }
        void ClosePlugins();
        void FindPlugins(IPluginHost pluginHost);
        void FindPlugins(string path, IPluginHost pluginHost);
        IAvailablePlugin GetPluginByName(String name);
    }
}
