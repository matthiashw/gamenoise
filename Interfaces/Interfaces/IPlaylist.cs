using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;



namespace Interfaces
{
    public interface IPlaylist
    {
        int addSong(string alb, string art, string tit, string filepath, double dur);
        int addSong(String filepath, int index);
        ObservableCollection<Interfaces.ISong> Playlist{ get; }
        IObserver aLoadingobserver { get; set; }
        int loadingRecursionDepth { get; set; }
        Dispatcher DispatcherThread {get; set;}
        Thread getPlthread { get; }
        bool stopRequest { get; set; }
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
        void setPlayMode(int mode);
        int getPlayMode();
        void savePL(string fl);
        void startXMLThread(string fileLoc);
        void loadPL(object obj);
        void regenerateList();
    }
}