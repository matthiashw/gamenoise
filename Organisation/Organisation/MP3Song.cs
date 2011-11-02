namespace Organisation
{
    /// <summary>
    /// Class for handling MP3Songs
    /// </summary>
    public class Mp3Song : Song
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fp">filepath</param>
        /// <param name="bass">bass reference</param>
        public Mp3Song(string fp, Interfaces.IBassWrapperOrganisation bass)
            : base(fp,bass)
        {
           

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
        public Mp3Song(string alb, string art, string tit, string fp, double dur, Interfaces.IBassWrapperOrganisation bass)
            : base(alb, art, tit, fp, dur, bass)
        {


        }

        /// <summary>
        /// get Playtime of this Song
        /// </summary>
        /// <returns>Playtime</returns>
        public override string getPlaytime() {
            /*MP3Header mp3hdr = new MP3Header();
            bool boolIsMP3 = mp3hdr.ReadMP3Information(this.getFilePath());
            if (boolIsMP3)
            {

                return (mp3hdr.intLength / 60).ToString() + ":" + (mp3hdr.intLength % 60).ToString();

            }
            else
            {
                return "";

            } */
            return "";
            
        }
  
        /// <summary>
        /// get _tags from file and save them in an array
        /// </summary>
        public override void getTagsFromFile() {
           /* try
            {
                FileInfo file = new FileInfo(base._filePath);
                Stream s = file.OpenRead();


                byte[] bytes = new byte[128];
                s.Seek(-128, SeekOrigin.End);
                int numBytesToRead = 128;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = s.Read(bytes, numBytesRead, numBytesToRead);

                    if (n == 0)
                    {
                        break;
                    }
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
                s.Close();
                String tag = ConvertByteToString(bytes, 0, 2);
                if (tag != "TAG")
                {
                    return;
                }

                base._tags[2] = ConvertByteToString(bytes, 3, 32);
                base._tags[1] = ConvertByteToString(bytes, 33, 62);
                // m_album = ConvertByteToString(bytes, 63, 92);
                //m_year = Int32.Parse(ConvertByteToString(bytes, 93, 96));
                // m_comment = ConvertByteToString(bytes, 97, 126);
                //m_genre = bytes[127];
            }
            catch (System.UnauthorizedAccessException)
            {
                return;
            }
            catch (System.Security.SecurityException)
            {
                return;
            }
            catch (System.IO.IOException e)
            {
                //seek schlaegt fehl
                throw e;
            }
            */
        }
        /*
        private  String ConvertByteToString(byte[] bytes, int pos1, int pos2)
        {
            //pos2 muß größer oder gleich pos1 sein und
            //pos2 darf Länge des Arrays nicht überschreiten
            if ((pos1 > pos2) || (pos2 > bytes.Length - 1))
            {
                throw new ArgumentException("Aruments out of range");
            }

            //Länge des zu betrachtenden Ausschnittes
            int length = pos2 - pos1 + 1;

            //neues Char-Array anlegen der Länge length
            Char[] chars = new Char[length];

            //packe alle Bytes von pos1 bis pos2 als
            //Char konvertiert in Array chars
           
            Exception e = new Exception();
           // throw e;
            
            for (int i = 0; i < length; i++)
            {

                chars[i] = Convert.ToChar(bytes[i + pos1]);

            }//end for

            //konvertiere Char-Array in String und gebe es zurück
            String s = new String(chars);
            int pos = s.IndexOf('\0');
            if(pos != -1)
                s = s.Substring(0, pos);

            return s;
        }

        */
    }
}
