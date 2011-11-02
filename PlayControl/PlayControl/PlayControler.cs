/* 
 * author: AK
 * 
 * created: 07.11.2008
 * 
 * modification history
 * --------------------
 * 
 * MHI (26.11.08):
 * added DeleteEqualizerSetting to delete a EQ Setting
 * 
 * AK (30.11.08):
 * Added Try-Catches
 * 
 * AK (04.12.08):
 * EQ save buggfix
 * 
 * Mho (13.01.08)
 * Path for equalizer changed
 *
 * PL (08.06.09)
 * Code Cleaning
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using System.IO;
using Observer;

namespace PlayControl
{
    public class PlayControler : Subject,Interfaces.IPlayControler
    {
        /// <summary>
        /// BassWrapper Reference
        /// </summary>
        public Interfaces.IBassWrapper ABassWrapper { get; set; }

        /// <summary>
        /// Playlist Reference
        /// </summary>
        public Interfaces.IPlaylist TmpPlaylist { get; set; }


        /// <summary>
        /// Equalizer Presents
        /// </summary>
        private readonly Dictionary<string,float[]> _eqList;

        /// <summary>
        /// Equalizer path
        /// </summary>
        private readonly String _pathTmpEqualizer;

        /// <summary>
        /// Is the Player playing a Song or Stopped
        /// </summary>
        private bool _playing;


        /// <summary>
        /// Get Method for playing
        /// </summary>
        /// <returns></returns>
        public bool GetPlaying()
        {
            return _playing;
        }

        /// <summary>
        /// is the current Song valid or not?
        /// </summary>
        private bool _validSong;


        /// <summary>
        /// Get Method for validSong
        /// </summary>
        /// <returns></returns>
        public bool GetValid()
        {
            return _validSong;
        }


        /// <summary>
        /// Constructor for PlayControler
        /// </summary>
        public PlayControler(String pathTmpEqualizer){
            _pathTmpEqualizer = pathTmpEqualizer;

            _playing = false;
            _validSong = true;
            _eqList = new Dictionary<string, float[]>();

            if (!File.Exists(pathTmpEqualizer))
                return;

            var reader = new FileStream(pathTmpEqualizer, FileMode.Open, FileAccess.Read);
            var bf = new BinaryFormatter();
            _eqList = (Dictionary<string, float[]>)bf.Deserialize(reader);
            reader.Close();

        }

      

        /// <summary>
        /// PlayerFunction: Play 
        /// gets called by UserInterface
        /// </summary>
        public void Play()
        {
            Interfaces.ISong nextSong = TmpPlaylist.getCurrentSong();

            if (TmpPlaylist.Playlist.Count == 0)
                return;
            if (!_playing)
            {
                if (TmpPlaylist.getCurrentSong() == null)
                {
                    TmpPlaylist.setNowPlayingPosition(0);
                }
                    
                ABassWrapper.play(nextSong.getFilePath(), ref _validSong);
                _playing = true;

            }
            else
                Pause();
        }


        /// <summary>
        /// PlayerFunction: Next
        /// gets called by UserInterface
        /// </summary>
        public void NextSong()
        {

            if (TmpPlaylist.Playlist.Count == 0)
                return;

                Interfaces.ISong nextSong = TmpPlaylist.getNextSong();

                while (true)
                {
                    if (ABassWrapper != null)
                        ABassWrapper.play(nextSong.getFilePath(), ref _validSong);

                    if (_validSong)
                    {
                        break;
                    }
                    nextSong = TmpPlaylist.getNextSong();
                }
        }


        /// <summary>
        /// PlayerFunction: Prev 
        /// gets called by UserInterface
        /// </summary>
        public void PreviousSong()
        {

            if (TmpPlaylist.Playlist.Count == 0)
                return;

                ABassWrapper.play(TmpPlaylist.getPrevSong().getFilePath(),ref _validSong);
        }


        /// <summary>
        /// PlayerFunction: Pause 
        /// gets called by UserInterface
        /// </summary>
        public void Pause()
        {
                ABassWrapper.pause();
        }

        /// <summary>
        /// PlayerFunction: Stop 
        /// gets called by UserInterface
        /// </summary>
        public void Stop()
        {
            ABassWrapper.stop();
            _playing = false;
        }

        /// <summary>
        /// loads the Equalizer Settings eQ with XML_Serialisation
        /// </summary>
        /// <param name="eQ">Equalizer Settings to Load</param>
        public void LoadEqualizer(String eQ)
        {
                ABassWrapper.setEqualizer(_eqList[eQ]);
        }

        /// <summary>
        /// Saves the recent Equalizer Settings
        /// </summary>
        /// <param name="eQ">Save Name for XML</param>
        public void SaveEqualizer(String eQ)
        {
            // Make new Instance
            var tmp = new float[10];
            var mom = ABassWrapper.getEqualizer();
            for (int i = 0; i < 10;i++ )
            {
                tmp[i] = mom[i];
            }

            try
            {

                _eqList.Add(eQ,tmp);

            }
            catch (ArgumentException)
            {
                _eqList.Add(eQ + "_", tmp);
            }
            var writer = new FileStream(_pathTmpEqualizer, FileMode.Create, FileAccess.Write);
            var bf = new BinaryFormatter();
            bf.Serialize(writer, _eqList);
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// Returns a List of the Equalizer Presets
        /// </summary>
        /// <returns>Presets</returns>
        public String[] GetEQPresetList()
        {
            var ret = _eqList.Select(eql => eql.Key).ToArray();


            return ret;
        }

        /// <summary>
        /// Sets an Equalizer Band
        /// </summary>
        /// <param name="band">Band (0-9)</param>
        /// <param name="freq">Frequency</param>
        public void SetEqualizer(int band, float freq)
        {
            ABassWrapper.setEqualizerBand(band, freq);
        }


        /// <summary>
        /// sets every Equalizer Band
        /// </summary>
        /// <param name="frequencyList">10 Band Float list</param>
        public void SetEqualizerAll(float[] frequencyList)
        {
            ABassWrapper.setEqualizer(frequencyList);
        }


        /// <summary>
        /// deletes an Equalizer Setting
        /// </summary>
        /// <param name="eQ">the Equalizer Setting name to delete</param>
        public void DeleteEqualizerSetting(string eQ)
        {
            try
            {
                _eqList.Remove(eQ);
                var writer = new FileStream(_pathTmpEqualizer, FileMode.OpenOrCreate, FileAccess.Write);
                var bf = new BinaryFormatter();
                bf.Serialize(writer, _eqList);
                writer.Flush();
                writer.Close();
            }
            catch (ArgumentNullException)
            {}
        }
    }
}
