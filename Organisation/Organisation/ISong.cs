using Observer;

namespace Organisation
{

    public interface ISong : ISubject
    {
        string getFilePath();
        void setFilePath(string fp);
        string getPlaytime();
        string artist { get; set; }
        string title { get; set; }
        string album { get; set; }
        string artistAndtitle{get;}
        string length{get;}
        double dlength { get; }
    }
}