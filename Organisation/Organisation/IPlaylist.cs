using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using Observer;


namespace Organisation
{
    public enum playModeEnum { PLAYMODE_NORMAL = 0, PLAYMODE_RANDOM = 1, PLAYMODE_REPEAT_SONG = 2, PLAYMODE_REPEAT_LIST = 3 };

    public interface IPlaylist : ISubject
    {
        int addSong(ISong song);
        int addSong(ISong song, int index);
        ObservableCollection<ISong> Playlist{ get; }
        int loadingRecursionDepth { get; set; }
        Thread getPlthread { get; }
        void startPLThread(string folderpath);
        void startPLThreadfolders(string[] folderpath);
        void addFolder(object obj);
        bool deleteSong(int index);
        void deleteAllSongs();
        ISong getSongAtIndex(int index);
        ISong getNextSong();
        ISong getPrevSong();
        int getNowPlayingPosition();
        bool setNowPlayingPosition(int index);
        ISong getCurrentSong();
        ISong getLoadingSong();
        void setPlayMode(playModeEnum mode);
        int getPlayMode();
        void savePL(string fl);
        void startXMLThread(string fileLoc);
        void loadPL(object obj);
        void regenerateList();
    }
}