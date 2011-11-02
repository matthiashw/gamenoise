namespace Interfaces
{
    public interface IPlayControler
    {
        IBassWrapper ABassWrapper { get; set; }
        void DeleteEqualizerSetting(string eQ);
        string[] GetEQPresetList();
        bool GetPlaying();
        void LoadEqualizer(string eQ);
        void NextSong();
        void Pause();
        void Play();
        void PreviousSong();
        void SaveEqualizer(string eQ);
        void SetEqualizer(int band, float freq);
        void SetEqualizerAll(float[] frequencyList);
        void Stop();
        IPlaylist TmpPlaylist { get; set; }
    }
}
