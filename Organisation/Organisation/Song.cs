/*
 * Author: Pascal Laube
 * Created: 13.11.08
 * 
 * General Information:
 * 
 * 
 * Changes:
 * PL(17.11.08):
 * Took old methods from old Prototype and updated them as specified.
 * 
 * AK(01.12.08):
 * try catch for BassWrapperExceptions added
 * 
 * MHO(12.12.08)
 * Added Exception if Seek failed
 */

using Observer;

namespace Organisation
{
    /// <summary>
    /// Song is the abstract Song class for handling song information
    /// </summary>
    public abstract class Song : Subject, Interfaces.ISong 
    {
       
        protected string[] _tags;
        protected string _filePath;


        public string artist { 
            get { return _tags[1]; }
            set { _tags[1] = value; Notify(); } 
        }
        public string title { 
            get { return _tags[2]; }
            set { _tags[2] = value; Notify(); }
        }

        public string album
        {
            get { return _tags[0]; }
            set { _tags[0] = value; Notify(); }
        }

        public string artistAndtitle { get {
            string seperator;
            if (artist == "" || title == "") seperator = "";
            else seperator = " - ";
            return artist + seperator + title;
        } }
        public string length
        {
            get
            {
                if (_songlength == 0.0f) return "";
                return (((int)(_songlength / 60)).ToString("00;0#") + ":" + ((int)(_songlength % 60)).ToString("00;0#"));
            }
        }

        public double dlength { get { return _songlength; } }
        protected double _songlength;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="fp">filepath</param>
        /// <param name="bass">bass reference</param>
        protected Song(string fp, Interfaces.IBassWrapperOrganisation bass)
        {
            _aBassWrapperOrganisation = bass;
            _filePath = fp;
            try
            {
                _songlength = PlayControl.BassWrapper.GetTimeLength(_filePath);
               _tags = PlayControl.BassWrapper.GetTags(fp);
               if (_tags[1]!=null && _tags[2]!=null &&_tags[2] == "" && _tags[1] == "")
               {
                   
                   string[] splitted = fp.Split('\\');
                   _tags[1] = splitted[splitted.Length - 1];
               }

            }
            catch (PlayControl.BassWrapperException)
            {
                _tags = new string[3];
                string[] splitted = fp.Split('\\');
                _tags[1] = splitted[splitted.Length - 1];
            }
           
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="alb">album</param>
        /// <param name="art">artist</param>
        /// <param name="tit">title</param>
        /// <param name="fp">filepath</param>
        /// <param name="dur">duration</param>
        /// <param name="bass">bass refence</param>
        public Song(string alb, string art, string tit, string fp, double dur, Interfaces.IBassWrapperOrganisation bass)
        {
            _tags = new string[3];

            _tags[0] = alb;
            _tags[1] = art;
            _tags[2] = tit;

            _songlength = dur;

            _filePath = fp;

            _aBassWrapperOrganisation = bass;
        }


        protected Interfaces.IBassWrapperOrganisation _aBassWrapperOrganisation;


        

        /// <summary>
        /// Get the _tags for the current song 
        /// </summary>
        public abstract void getTagsFromFile();

        /// <summary>
        /// Get the _tags for the current song
        /// </summary>
        /// <returns>returns string array of all _tags for the song</returns>
        public virtual string [] getTags()
        {
            return _tags;
        }
        

        /// <summary>
        /// Get the path of the file
        /// </summary>
        /// <returns>returns string with the path of the file</returns>
        public virtual string getFilePath()
        {
            return _filePath;
        }
        
        /// <summary>
        /// set the path of the file
        /// </summary>
        /// <returns>returns string with the path of the file</returns>
#pragma warning disable 1574
        /// <param name="fl">filepath</param>
#pragma warning restore 1574
        public virtual void setFilePath(string fp)
        {
            _filePath = fp;
        }

        /// <summary>
        /// get the files total Play time
        /// </summary>
        /// <returns>returns string with the path of the file</returns>
        public abstract string getPlaytime();
    }
}
