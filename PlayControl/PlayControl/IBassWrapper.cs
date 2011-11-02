using Observer;
using PluginManager;

namespace PlayControl
{

    /// <summary>
    /// Interface BassWrapper <-> Organisation (depratched)
    /// </summary>
    public interface IBassWrapperOrganisation : ISubject
    {

    }

    /// <summary>
    /// Interface BassWrapper <-> Gui-Komponent
    /// </summary>
    public interface IBassWrapperGui : ISubject
    {

        double getTotalTime();
        double getElapsedTime();
        void setVolume(float gain);
        bool setPlayPosition(double seconds);
        float[] getEqualizer();
        bool getPaused();
        float[] getFFTData();
        bool getChangeEQ();
        bool gettoggleEQ();
        void settoggleEQ(bool state);
        void setBalance(float bal);
        float getBalance();
        float getVolume();
    }

    public interface IBassWrapper : ISubject
    {
        IPlayControler aPlayControler { get; set; }
        IPluginManager aPluginManager { get; set; }
        int getStreamId();
        float getBalance();
        bool getChangeEQ();
        double getElapsedTime();
        float[] getEqualizer();
        float[] getFFTData();
        bool getPaused();
        bool getstopped();
        bool gettoggleEQ();
        double getTotalTime();
        float getVolume();
        void pause();
        void play(string songPath);
        void setBalance(float bal);
        void setEqualizer(float[] frequencyList);
        void setEqualizerBand(int band, float freq);
        bool setPlayPosition(double seconds);
        void settoggleEQ(bool state);
        void setVolume(float gain);
        void stop();
    }
}