using System;
namespace Interfaces
{
    public interface IPluginManager
    {
        IAvailablePlugins AvailablePlugins { get; set; }
        void ClosePlugins();
        void FindPlugins(IPluginHost pluginHost);
        void FindPlugins(string Path, IPluginHost pluginHost);
        IAvailablePlugin GetPluginByName(String name);
    }
}
