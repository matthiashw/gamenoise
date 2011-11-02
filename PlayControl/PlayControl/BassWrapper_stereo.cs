using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Un4seen.Bass;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.AddOn.Fx;
using System.Drawing;
using System.Timers;
using System.Runtime.InteropServices;
using System.Windows.Forms; // Errors


namespace PlayControl
{
    /// <summary>
    /// Bass Wrapper Class to wrap the Bass Library
    /// </summary>
  
    class BassWrapper
    {
       
        /// <summary>
        /// Pause Variable
        /// </summary>
        private bool paused;

        /// <summary>
        /// Visual Object for Visualisation
        /// </summary>
        private Visuals vis;

        /// <summary>
        /// ID of Bass stream
        /// </summary>
        private int streamID; 

        /// <summary>
        /// Song End SYNC
        /// </summary>
        private SYNCPROC mySync;

        /// <summary>
        /// Equalizer Handle
        /// </summary>
        private int fxEQ;

        /// <summary>
        /// Save EQ Gains tempuarily
        /// </summary>
        private float[] equalizerTmp=  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        /// <summary>
        /// This Method Sets the Equalizer Settings to a new Stream.
        /// </summary>
        private void EqualizerInit()
        {
            //############## BASSFX - Equalizer #######################
            // 60Hz, 170, 310, 600, 1K, 3K, 6K,12K,14K,16KHz
            // 10-band EQ
            // Set peaking equalizer effect with no bands
            bool loadbassfx = BassFx.LoadMe();
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
            fxEQ = Bass.BASS_ChannelSetFX(streamID, BASSFXType.BASS_FX_BFX_PEAKEQ, 0);
            if (!loadbassfx)
            {
                MessageBox.Show("Can't find bass_fx.dll");
            }
            // Setup the EQ bands
           
            eq.fQ = 0f;
            eq.fBandwidth = 2.5f;
            eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);
            //Band 1
            eq.lBand = 0;
            eq.fCenter = 60f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 2
            eq.lBand = 1;
            eq.fCenter = 170f;
            Bass.BASS_FXSetParameters(fxEQ, eq);

            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);
            //Band 3
            eq.lBand = 2;
            eq.fCenter = 310f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 4
            eq.lBand = 3;
            eq.fCenter = 600f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 5
            eq.lBand = 4;
            eq.fCenter = 1000f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 6
            eq.lBand = 5;
            eq.fCenter = 3000f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 7
            eq.lBand = 6;
            eq.fCenter = 6000f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 8
            eq.lBand = 7;
            eq.fCenter = 12000f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 9
            eq.lBand = 8;
            eq.fCenter = 14000f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

            //Band 10
            eq.lBand = 9;
            eq.fCenter = 16000f;
            Bass.BASS_FXSetParameters(fxEQ, eq);
            setEqualizerBand(eq.lBand, equalizerTmp[eq.lBand]);

        }


        /// <summary>
        /// Constructor for the Bass Wrapper Class
        /// </summary>
        /// <param name="handle">Window Handle</param>
        public BassWrapper(IntPtr handle)
        {
            //############### BASS - REGISTRATION #####################
            //BassNet.Registration("mail@example.com", "key");
            //############### BASS - INITIATION #######################
            Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, handle, null);
            //############### BASS - VISUAL INITIATION ################
            this.vis = new Visuals();  
            //############### BASS - SYNCPROC INITIATION (End of Song Procedure)
            mySync = new SYNCPROC(EndSync);
           
            
        }

        /// <summary>
        /// Destruktor to Free Bass resources
        /// </summary>
         ~BassWrapper()
        {
            Bass.BASS_Free();
        }

        /// <summary>
        /// The Play Method plays a song by his path with the Bass Lib.
        /// </summary>
        /// <param name="SongPath">Song Path to Play</param>
        public void play(string songPath)
        {
                Bass.BASS_StreamFree(this.streamID);                
                this.paused = false;
                this.streamID = Bass.BASS_StreamCreateFile(songPath, 0, 0, BASSFlag.BASS_SAMPLE_FLOAT);
                EqualizerInit(); // Initiate Eq
                setEqualizer(equalizerTmp); //set last Values
                Bass.BASS_ChannelPlay(this.streamID, false);
                Bass.BASS_ChannelSetSync(this.streamID, BASSSync.BASS_SYNC_END | BASSSync.BASS_SYNC_MIXTIME,
                         0, mySync, IntPtr.Zero);
                
            
        }

        /// <summary>
        /// Pause the Playing stream or if it is paused play it aggain
        /// </summary>
        public void pause()
        {
            if (this.paused)
            {
                Bass.BASS_ChannelPlay(this.streamID, false);
                this.paused = false;
            } 
            else {
                
                Bass.BASS_ChannelPause(this.streamID);
                this.paused = true;
            }
        }

        /// <summary>
        /// Stop playing stream
        /// </summary>
        public void stop()
        {
            Bass.BASS_ChannelStop(this.streamID);
            Bass.BASS_StreamFree(this.streamID);
        }

        /// <summary>
        /// This method wraps the Visualisingmethods of the BASS Library
        /// </summary>
        /// <param name="g">Grafics Object</param>
        public void drawVisual(Graphics g)
        {
      
            vis.CreateSpectrumLine(this.streamID, g, new Rectangle(1, 1, 151, 83), Color.Orange, Color.White, Color.White, 10, 3, true, false, false);
               
        }

        /// <summary>
        /// This Method is is Called when a Song is at its end
        /// </summary>
        /// <param name="handle">The handle for the ended Song</param>
        /// <param name="channel">The channel that ended</param>
        /// <param name="data">Channel Data</param>
        /// <param name="user">User Data</param>
        private void EndSync(int handle, int channel, int data, IntPtr user)
        {
            // the 'channel' has ended - jump to the beginning
            MessageBox.Show("Lied vorbei");
            
            Bass.BASS_ChannelSetPosition(channel, 0L);
            
        }

        /// <summary>
        /// Method to Set Volume of Master Output
        /// </summary>
        /// <param name="gain">
        /// Volume:
        /// 1 = max
        /// 0 = min
        /// </param>
        public void setVolume(float gain) 
        {
            Bass.BASS_ChannelSetAttribute(streamID, BASSAttribute.BASS_ATTRIB_VOL, gain);

        }

        /// <summary>
        /// Set Equalizer Settings
        /// </summary>
        /// <param name="frequencyList">10 Band Float list</param>
        public void setEqualizer(float[] frequencyList)
        {
            
            for (int i = 0; i < 10; i++)
            {
                equalizerTmp[i] = frequencyList[i];
                BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
                // get values of the selected band
                eq.lBand = i;
                Bass.BASS_FXGetParameters(fxEQ, eq);
                eq.fGain = frequencyList[i];
                Bass.BASS_FXSetParameters(fxEQ, eq);
            }
            
        }

        /// <summary>
        /// Sets Single Equalizer Band
        /// </summary>
        /// <param name="band">Band to set</param>
        /// <param name="freq">Gain to set</param>
        public void setEqualizerBand(int band, float freq)
        {
            equalizerTmp[band] = freq;
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
            // get values of the selected band
            eq.lBand = band;
            Bass.BASS_FXGetParameters(fxEQ, eq);
            eq.fGain = freq;
            Bass.BASS_FXSetParameters(fxEQ, eq);


        }

        /// <summary>
        /// Set A new Postition to Play the stream
        /// </summary>
        /// <param name="seconds">Stream position</param>
        /// <returns>Succes-variable</returns>
        public bool setPlayPosition(double seconds)
        {
            if (seconds > getTotalTime())
            {
                return false;
            } else if ( seconds== getTotalTime()){
                if (getTotalTime() > 1)
                {
                    seconds--;
                }
                else
                {
                    return false;
                }
            }
               
            try
            {
                Bass.BASS_ChannelSetPosition(streamID, seconds);
                return true;
            }
            catch
            {
                return false;
            }

        }

        /// <summary>
        /// Method to Read Music Tags from Files
        /// </summary>
        /// <param name="songPath"></param>
        /// <returns></returns>
        public string[] getTags(String songPath)
        {
            string [] tags = new string [7];
            TAG_INFO tagInfo;
            tagInfo=BassTags.BASS_TAG_GetFromFile(songPath);
            
                // and display what we get
                tags[0] = tagInfo.album;
                tags[1] = tagInfo.artist;
                tags[2] = tagInfo.title;
                tags[3] = tagInfo.comment;
                tags[4] = tagInfo.genre;
                tags[5] = tagInfo.year;
                tags[6] = tagInfo.track;
                
                return tags;
            
        }

        /// <summary>
        /// Get Method for elapsed Time of Stream
        /// </summary>
        /// <returns>double Time (in secs)</returns>
        public double getElapsedTime()
        {
                    
            return Bass.BASS_ChannelBytes2Seconds(streamID,Bass.BASS_ChannelGetPosition(streamID));


        }

        /// <summary>
        /// Get Method for total Time of Stream
        /// </summary>
        /// <returns>double Time (in secs)</returns>
        public double getTotalTime()
        {
            return Bass.BASS_ChannelBytes2Seconds(streamID, Bass.BASS_ChannelGetLength(streamID));

        }


    }
}
