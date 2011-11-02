using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace RadioNoise
{
    [Serializable]
    public class StreamManager
    {
        private ObservableCollection<Stream> _myStreams = new ObservableCollection<Stream>();
        private Webservice.IWebserviceClient _aWebserviceClient;


        public void RemoveAll()
        {
            _myStreams.Clear();
        }

        public StreamManager(Webservice.IWebserviceClient aWebserviceClient)
        {
            _aWebserviceClient = aWebserviceClient;
        }

        public StreamManager()
        {
        }

        public void AddStream(Stream stream)
        {
            _myStreams.Add(stream);
        }

        public ObservableCollection<Stream> GetStreams()
        {
            return _myStreams;
        }

        public void Save(string path)
        {
            XmlTextWriter myXmlTextWriter = new XmlTextWriter(path, System.Text.Encoding.UTF8);
            myXmlTextWriter.Formatting = Formatting.Indented;
            myXmlTextWriter.WriteStartDocument(false);

            myXmlTextWriter.WriteComment("Erstellt von gamenoise");

            myXmlTextWriter.WriteStartElement("streams");

            IEnumerator enumerator = _myStreams.GetEnumerator();

            while (enumerator.MoveNext())
            {
                myXmlTextWriter.WriteStartElement("stream");

                myXmlTextWriter.WriteElementString("title", ((Stream)enumerator.Current).Title);
                myXmlTextWriter.WriteElementString("url", ((Stream)enumerator.Current).Url);
                myXmlTextWriter.WriteElementString("genre", ((Stream)enumerator.Current).Genre);
                myXmlTextWriter.WriteElementString("description", ((Stream)enumerator.Current).Description);

                myXmlTextWriter.WriteEndElement();
            }

            myXmlTextWriter.WriteEndElement();

            myXmlTextWriter.Flush();
            myXmlTextWriter.Close();
        }

        public void Load(string path)
        {
            if(File.Exists(path)){
                XmlDocument doc = new XmlDocument();
                doc.Load(path);

                XmlElement root = doc.DocumentElement;

                foreach (XmlNode stream in root.ChildNodes)
                {
                    Stream s = new Stream();
                    s.Title = stream["title"].InnerText;
                    s.Url = stream["url"].InnerText;
                    s.Genre = stream["genre"].InnerText;
                    s.Description = stream["description"].InnerText;

                    AddStream(s);
                }
            }
        }

    }
}
