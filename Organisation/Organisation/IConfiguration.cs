using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Warning: This interfaces cant be used, because XML-Serializer doesnt support interfaces!

namespace Organisation
{
    /// <summary>
    /// Possible positions of the overlay
    /// </summary>
    public enum overlayPositionEnum { TOP = 0, TOPLEFT = 1, TOPRIGHT = 2, BOTTOM = 3, BOTTOMLEFT = 4, BOTTOMRIGHT = 5 };

    /// <summary>
    /// Not yet used
    /// </summary>
    public interface IConfiguration
    {
        string getSkinPath();
        IConfigHotKey hotkeyNext { get; set; }
        IConfigHotKey hotkeyOverlay { get; set; }
        IConfigHotKey hotkeyPlay { get; set; }
        IConfigHotKey hotkeyPrev { get; set; }
        IConfigHotKey hotkeyStop { get; set; }
        IConfigHotKey hotkeyVolDown { get; set; }
        IConfigHotKey hotkeyVolUp { get; set; }
        void init();
        void initNotify();
        string language { get; set; }
        bool minimizeToTray { get; set; }
        string myDocumentsPath { get; set; }
        IConfigOverlayColor overlayColorBack { get; set; }
        IConfigOverlayColor overlayColorFont { get; set; }
        IConfigOverlayColor overlayColorHKBack { get; set; }
        IConfigOverlayColor overlayColorHKTop { get; set; }
        IConfigOverlayColor overlayColorLine { get; set; }
        IConfigOverlayColor overlayColorProgress { get; set; }
        overlayPositionEnum overlayPosition { get; set; }
        bool overlayTimerEnabled { get; set; }
        int overlayTimerSeconds { get; set; }
        string pathFeaturedSongs { get; set; }
        string pathFeaturedSongsFlag { get; set; }
        string pathSettings { get; set; }
        string pathTmpEqualizer { get; set; }
        string pathTmpPlaylist { get; set; }
        string pathTmpSettings { get; set; }
        void resetToDefault();
        bool resumePlayback { get; set; }
        string usedSkin { get; set; }
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

    