using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Warning: This interfaces cant be used, because XML-Serializer doesnt support interfaces!

namespace Interfaces
{
    /// <summary>
    /// Possible positions of the overlay
    /// </summary>
    public enum overlayPositionEnum { TOP = 0, TOPLEFT = 1, TOPRIGHT = 2, BOTTOM = 3, BOTTOMLEFT = 4, BOTTOMRIGHT = 5 };

    /// <summary>
    /// Not yet used
    /// </summary>
    public interface IConfiguration : ISubject
    {
        string getFallBackSkin();
        string GetPluginPath();
        string GetSkinPath();
        void Init();
        void InitNotify();
        string Language { get; set; }
        bool MinimizeToTray { get; set; }
        string MyDocumentsPath { get; set; }
        bool OverlayTimerEnabled { get; set; }
        int OverlayTimerSeconds { get; set; }
        string PathFeaturedSongs { get; set; }
        string PathFeaturedSongsFlag { get; set; }
        string PathSettings { get; set; }
        string PathTmpEqualizer { get; set; }
        string PathTmpPlaylist { get; set; }
        string PathTmpSettings { get; set; }
        void ResetToDefault();
        bool ResumePlayback { get; set; }
        string UsedSkin { get; set; }
    }

    /// <summary>
    /// Not yet used
    /// </summary>
    public interface IConfigHotKey
    {
        void IConfigHotKey();

        void setConfiguration(IConfiguration config);

        bool mAlt{ get; set; }

        bool mCtrl{ get; set; }

        bool mShift{ get; set; }

        bool isEnabled { get; set; }

        System.Windows.Forms.Keys hKey{ get; set; }

    }

    public interface IConfigOverlayColor
    {
        void IConfigOverlayColor();

        void setConfiguration(IConfiguration config);

        bool Alpha { get; set; }

        bool Red { get; set; }

        bool Green { get; set; }

        bool Blue { get; set; }

    }
}

    