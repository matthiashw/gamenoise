using Organisation;

namespace PlayControl
{
    public interface IPlayControler
    {
        IBassWrapper aBassWrapper { get; set; }
        void deleteEqualizerSetting(string eQ);
        void StopAddingSongs();
        string[] getEQPresetList();
        bool getPlaying();
        void loadEqualizer(string eQ);
        void nextSong();
        void pause();
        void play();
        void previousSong();
        void saveEqualizer(string eQ);
        void setEqualizer(int band, float freq);
        void setEqualizerAll(float[] frequencyList);
        void stop();
        void setPlayMode(playModeEnum mode);
        void addSongs(string[] files);
        IPlaylist tmpPlaylist { get; set; }
    }
}
