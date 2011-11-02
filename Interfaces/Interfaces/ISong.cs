namespace Interfaces
{

    public interface ISong : ISubject
    {
        void getTagsFromFile();
        string[] getTags();
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