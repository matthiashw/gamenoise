/*
 * Author: AK
 * Created: 23.12.08
 * 
 * General Information:
 * Serialize Tmp Settings
 * 
 * 
 * MHI (01.02.09)
 * save also the last player state
 * 
 * */

using System;

namespace Organisation
{
    /// <summary>
    /// class for handling the temporary settings
    /// </summary>
    [Serializable]
    public class SaveTmpSettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SaveTmpSettings() { }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="eq">Equalizer</param>
        /// <param name="eqst">EQState</param>
        /// <param name="vol">Volume</param>
        /// <param name="bal">Balance</param>
        /// <param name="pln">playlistname</param>
        /// <param name="playm">Playmode</param>
        /// <param name="vis">visualisation</param>
        /// <param name="lofp">last opened Folderpath</param>
        /// <param name="showEq">is Equalizer shown</param>
        /// <param name="showPl">is Playlist shown</param>
        /// <param name="widthGn">Width of Player</param>
        /// <param name="heightGn">Height of Player</param>
        /// <param name="lastSongIndex">Index in playlist of last played song</param>
        /// <param name="lastSongPosition">Position of last played song</param>
        /// <param name="isPlay">if the player was closed while Playing</param>
        public SaveTmpSettings(float[] eq, 
                                bool eqst, 
                                float vol, 
                                float bal, 
                                string pln, 
                                int playm, 
                                int vis,
                                string lofp,
                                bool showEq,
                                bool showPl,
                                double widthGn,
                                double heightGn,
                                int lastSongIndex,
                                double lastSongPosition,
                                bool isPlay)
        {
            Equalizer = eq;
            Equalizerstate = eqst;
            Volume = vol;
            Balance = bal;
            Plname = pln;
            Playmode = playm;
            Visual = vis;
            LastOpenedFolderPath = lofp;
            IsEq = showEq;
            IsPl = showPl;
            GnWidth = widthGn;
            GnHeight = heightGn;
            SongIndex = lastSongIndex;
            SongPosition = lastSongPosition;
            Playing = isPlay;
        }


        public float[] Equalizer { get; set;}
        public bool Equalizerstate { get; set;}
        public float Volume { get; set;}
        public float Balance { get; set; }
        public string Plname { get; set; }
        public int Playmode { get; set; }
        public int Visual { get; set; }
        public string LastOpenedFolderPath { get; set; }
        public bool IsEq { get; set; }
        public bool IsPl { get; set; }
        public double GnWidth { get; set; }
        public double GnHeight { get; set; }
        public int SongIndex { get; set; }
        public double SongPosition { get; set; }
        public bool Playing { get; set; }
    }
}
