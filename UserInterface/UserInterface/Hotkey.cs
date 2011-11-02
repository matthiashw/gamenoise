/* 
 * author: BK
 * 
 * created: 07.11.2008
 * 
 * modification history
 * --------------------
 * 
 * BK (30.11.08):
 * HotKeys now working
 * 
 */

using System;
using PlayControl;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace UserInterface
{
    /// <summary>
    /// creates a global keyboard hotkey.
    /// </summary>
    [DefaultEvent("HotkeyPressed")]
    public class Hotkey
    {
        public PlayControler APlayControler { get; set; }

        /// <summary>
        /// Occurs when the hotkey is pressed.
        /// </summary>
        public event EventHandler HotkeyPressed;

        private static readonly Object MyStaticLock = new Object();
        private static int _hotkeyCounter = 0xA000;

        private readonly int _hotkeyIndex;
        private readonly bool _isDisposed;
        private bool _isEnabled;
        private bool _isRegistered;
        private Keys _keyCode;
        private bool _ctrl, _alt, _shift, _windows;
        private readonly IntPtr _hWnd;

        //public event HotKeyPressedEventHandler HotKeyPressed;
        public delegate void HotKeyPressedEventHandler(string hotKeyID);

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public Hotkey() : this(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public Hotkey(bool isDisposed)
        {
            EventDispatchingNativeWindow.Instance.EventHandler += NwEventHandler;
            lock (MyStaticLock)
            {
                _hotkeyIndex = ++_hotkeyCounter;
            }
            _hWnd = EventDispatchingNativeWindow.Instance.Handle;
            _isDisposed = isDisposed;
        }

        /// <summary>
        /// Enables the hotkey. When the hotkey is enabled, pressing it causes a
        /// <c>HotkeyPressed</c> event instead of being handled by the active 
        /// application.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                UpdateHotkey(false);
            }
        }

        /// <summary>
        /// The key code of the hotkey.
        /// </summary>
        public Keys KeyCode
        {
            get
            {
                return _keyCode;
            }
            set
            {
                _keyCode = value;
                UpdateHotkey(true);
            }
        }

        /// <summary>
        /// Whether the shortcut includes the Control modifier.
        /// </summary>
        public bool Ctrl
        {
            get { return _ctrl; }
            set { _ctrl = value; UpdateHotkey(true); }
        }

        /// <summary>
        /// Whether this shortcut includes the Alt modifier.
        /// </summary>
        public bool Alt
        {
            get { return _alt; }
            set { _alt = value; UpdateHotkey(true); }
        }

        /// <summary>
        /// Whether this shortcut includes the shift modifier.
        /// </summary>
        public bool Shift
        {
            get { return _shift; }
            set { _shift = value; UpdateHotkey(true); }
        }

        /// <summary>
        /// Whether this shortcut includes the Windows key modifier. The windows key
        /// is an addition by Microsoft to the keyboard layout. It is located between
        /// Control and Alt and depicts a Windows flag.
        /// </summary>
        public bool WindowsKey
        {
            get { return _windows; }
            set { _windows = value; UpdateHotkey(true); }
        }

        /// <summary>
        /// Dispatching Native Window Event Handler
        /// </summary>
        /// <param name="m">Message</param>
        /// <param name="handled">Handled</param>
        void NwEventHandler(ref Message m, ref bool handled)
        {
            if (handled) return;
            if (m.Msg != WmHotkey || m.WParam.ToInt32() != _hotkeyIndex) return;
            if (HotkeyPressed != null)
                HotkeyPressed(this, EventArgs.Empty);
            handled = true;
        }

        /// <summary>
        /// Update Hotkey-Registration
        /// </summary>
        /// <param name="reregister"></param>
        private void UpdateHotkey(bool reregister)
        {
            bool shouldBeRegistered = _isEnabled && !_isDisposed;// && !DesignMode;
            if (_isRegistered && (!shouldBeRegistered || reregister))
            {
                // unregister hotkey
                UnregisterHotKey(_hWnd, _hotkeyIndex);
                _isRegistered = false;
            }
            if (!_isRegistered && shouldBeRegistered)
            {
                // register hotkey
                bool success = RegisterHotKey(_hWnd, _hotkeyIndex,
                    (_shift ? ModShift : 0) + (_ctrl ? ModControl : 0) +
                    (_alt ? ModAlt : 0) + (_windows ? ModWin : 0), (int)_keyCode);
                if (!success) throw new HotkeyAlreadyInUseException();
                _isRegistered = true;
            }
        }

        /// <summary>
        /// Set a Hotkey
        /// </summary>
        /// <param name="mAlt">Alt Key</param>
        /// <param name="mCtrl">Crtl Key</param>
        /// <param name="mShift">Shift Key</param>
        /// <param name="mWin">Windows Key</param>
        /// <param name="hKey">Any Key</param>
        public void SetHotkey(bool mAlt, bool mCtrl, bool mShift, bool mWin, Keys hKey)
        {
            Alt = mAlt;
            Ctrl = mCtrl;
            Shift = mShift;
            WindowsKey = mWin;
            KeyCode = hKey;
        }

        #region PInvoke Declarations

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int ModAlt = 0x0001;
        private const int ModControl = 0x0002;
        private const int ModShift = 0x0004;
        private const int ModWin = 0x0008;
        private const int WmHotkey = 0x0312;

        #endregion
    }


    /// <summary>
    /// Called by an EventDispatchingNativeWindow when a window message is received
    /// </summary>
    /// <param name="m">The message to handle.</param>
    /// <param name="handled">Whether the event has already been handled. If this value is true, the handler
    /// should return immediately. It may set the value to true to indicate that no others 
    /// should handle it. If the event is not handled by any handler, it is passed to the
    /// default WindowProc.</param>
    public delegate void WndProcEventHandler(ref Message m, ref bool handled);

    /// <summary>
    /// A Win32 native window that delegates window messages to handlers. So several
    /// components can use the same native window to save "USER resources". This class
    /// is useful when writing your own components.
    /// </summary>
    public sealed class EventDispatchingNativeWindow : NativeWindow
    {

        private static readonly Object MyLock = new Object();
        private static EventDispatchingNativeWindow _instance;

        /// <summary>
        /// A global instance which can be used by components that do not need
        /// their own window.
        /// </summary>
        public static EventDispatchingNativeWindow Instance
        {
            get
            {
                lock (MyLock)
                {
                    if (_instance == null)
                        _instance = new EventDispatchingNativeWindow();
                    return _instance;
                }
            }
        }

        /// <summary>
        /// Attach your event handlers here.
        /// </summary>
        public event WndProcEventHandler EventHandler;

        /// <summary>
        /// Create your own event dispatching window.
        /// </summary>
        public EventDispatchingNativeWindow()
        {
            CreateHandle(new CreateParams());
        }

        /// <summary>
        /// Parse messages passed to this window and send them to the event handlers.
        /// </summary>
        /// <param name="m">A System.Windows.Forms.Message that is associated with the 
        /// current Windows message.</param>
        protected override void WndProc(ref Message m)
        {
            bool handled = false;
            if (EventHandler != null)
                EventHandler(ref m, ref handled);
            if (!handled)
                base.WndProc(ref m);
        }
    }


    /// <summary>
    /// The exception is thrown when a hotkey should be registered that
    /// has already been registered by another application.
    /// </summary>
    public class HotkeyAlreadyInUseException : Exception { }
}
