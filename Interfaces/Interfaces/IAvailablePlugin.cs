using System;
namespace Interfaces
{
    public interface IAvailablePlugin
    {
        string AssemblyPath { get; set; }
        IPlugin Instance { get; set; }
    }
}
