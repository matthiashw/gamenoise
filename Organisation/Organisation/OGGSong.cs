namespace Organisation
{
    /// <summary>
    /// Class for handling OGGSongs
    /// </summary>
    class OggSong : Song
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fp">filepath</param>
        /// <param name="bass">bass reference</param>
        public OggSong(string fp, Interfaces.IBassWrapperOrganisation bass)
            : base(fp, bass) 
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
        public OggSong(string alb, string art, string tit, string fp, double dur, Interfaces.IBassWrapperOrganisation bass)
            : base(alb, art, tit, fp, dur, bass)
        {


        }

        /// <summary>
        /// get _tags from file and save them in an array
        /// </summary>
        public override  void  getTagsFromFile() { }

        /// <summary>
        /// get Playtime of this Song
        /// </summary>
        /// <returns>Playtime</returns>
        public override string getPlaytime() {
            return "" ;
        
        }

    }
}
