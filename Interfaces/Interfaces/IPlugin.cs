using System.Windows.Threading;
using System.ComponentModel;
using System.Collections;
namespace Interfaces
{

    public interface IPlugin
    {
        IPluginHost Host { get; set; }

        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }

        System.Windows.Controls.UserControl MainInterface { get; }
        System.Windows.Controls.UserControl SettingsInterface { get; }

        bool isAbleToPlayback(string filename);
        int getBassStream(string filename);
        ArrayList getDependencies();
        void Initialize();
        void Dispose();
    }

    public interface IPluginHost
    {
        void pluginFeedback(string Feedback, IPlugin Plugin);
        IPlaylist getPlaylist();
        IPlayControler getPlayControler();
        IBassWrapper getBasswrapper();
        IPluginManager getPluginmanager();
        string getFileSavePath();
        void pluginClose(IPlugin Plugin);
        void ShowPlugin(IPlugin Plugin);
        Dispatcher getDispatcher();
        BackgroundWorker getBackgroundWorker();
    }
}