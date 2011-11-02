using System;

namespace RadioNoise
{
    [Serializable()]
    public class Stream
    {
        public String Title { get; set; }
        public String Url { get; set; }
        public String Genre { get; set; }
        public String Description { get; set; }

        public Stream(string title, string url, string genre, string description)
        {
            Title = title;
            Url = url;
            Genre = genre;
            Description = description;
        }

        public Stream()
        {
        }
    }
}