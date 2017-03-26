/// http://www.codeproject.com/KB/system/CSLLKeyboard.aspx
/// KEYBOARD.CS
/// (c) 2006 by Emma Burrows
/// This file contains the following items:
///  - KeyboardHook: class to enable low-level keyboard hook using
///    the Windows API.
///  - KeyboardHookEventHandler: delegate to handle the KeyIntercepted
///    event raised by the KeyboardHook class.
///  - KeyboardHookEventArgs: EventArgs class to contain the information
///    returned by the KeyIntercepted event.
///    
/// Change history:
/// 17/06/06: 1.0 - First version.
/// 18/06/06: 1.1 - Modified proc assignment in constructor to make class backward 
///                 compatible with 2003.
/// 10/07/06: 1.2 - Added support for modifier keys:
///                 -Changed filter in HookCallback to WM_KEYUP instead of WM_KEYDOWN
///                 -Imported GetKeyState from user32.dll
///                 -Moved native DLL imports to a separate internal class as this 
///                  is a Good Idea according to Microsoft's guidelines
/// 13/02/07: 1.3 - Improved modifier key support:
///                 -Added CheckModifiers() method
///                 -Deleted LoWord/HiWord methods as they weren't necessary
///                 -Implemented Barry Dorman's suggestion to AND GetKeyState
///                  values with 0x8000 to get their result
/// 23/03/07: 1.4 - Fixed bug which made the Alt key appear stuck
///                 - Changed the line
///                     if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
///                   to
///                     if (nCode >= 0)
///                     {
///                        if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
///                        ...
///                   Many thanks to "Scottie Numbnuts" for the solution.


using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using DoctorGu;

/// <summary>
/// Low-level keyboard intercept class to trap and suppress system keys.
/// </summary>
public class CKeyboardBlock : IDisposable
{
    /// <summary>
    /// Parameters accepted by the KeyboardHook constructor.
    /// </summary>
	[Flags]
	public enum KeyboardBlockType
	{
		None = 0,
		CtrlAndEsc = 1,
		AltAndEsc = 2,
		AltAndTab = 4,
		WindowKey = 8,
		All = CtrlAndEsc | AltAndEsc | AltAndTab | WindowKey
	}
	private KeyboardBlockType _BlockType = KeyboardBlockType.None;

	//public enum Parameters
	//{
	//    None,
	//    AllowAltTab,
	//    AllowWindowsKey,
	//    AllowAltTabAndWindows,
	//    PassAllKeysToNextApp
	//}

	////Internal parameters
	//private bool PassAllKeysToNextApp = false;
	//private bool AllowAltTab = false;
	//private bool AllowWindowsKey = false;

    //Keyboard API constants
    private const int WH_KEYBOARD_LL = 13;
	private const int WM_KEYDOWN= 0x0100;
    private const int WM_KEYUP = 0x0101;
	private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;

	private const int HC_ACTION = 0;
	private const int LLKHF_EXTENDED = 0x01;
	private const int LLKHF_ALTDOWN = 0x20;

	//http://msdn.microsoft.com/en-us/library/ms645540(v=VS.85).aspx
	public struct VirtualKeys
	{
		public const int VK_LBUTTON = 0x01;
		public const int VK_RBUTTON = 0x02;
		public const int VK_CANCEL = 0x03;
		public const int VK_MBUTTON = 0x04;
		public const int VK_XBUTTON1 = 0x05;
		public const int VK_XBUTTON2 = 0x06;
		public const int VK_BACK = 0x08;
		public const int VK_TAB = 0x09;
		public const int VK_CLEAR = 0x0C;
		public const int VK_RETURN = 0x0D;
		public const int VK_SHIFT = 0x10;
		public const int VK_CONTROL = 0x11;
		public const int VK_MENU = 0x12;
		public const int VK_PAUSE = 0x13;
		public const int VK_CAPITAL = 0x14;
		public const int VK_KANA = 0x15;
		public const int VK_HANGUEL = 0x15;
		public const int VK_HANGUL = 0x15;
		public const int VK_JUNJA = 0x17;
		public const int VK_FINAL = 0x18;
		public const int VK_HANJA = 0x19;
		public const int VK_KANJI = 0x19;
		public const int VK_ESCAPE = 0x1B;
		public const int VK_CONVERT = 0x1C;
		public const int VK_NONCONVERT = 0x1D;
		public const int VK_ACCEPT = 0x1E;
		public const int VK_MODECHANGE = 0x1F;
		public const int VK_SPACE = 0x20;
		public const int VK_PRIOR = 0x21;
		public const int VK_NEXT = 0x22;
		public const int VK_END = 0x23;
		public const int VK_HOME = 0x24;
		public const int VK_LEFT = 0x25;
		public const int VK_UP = 0x26;
		public const int VK_RIGHT = 0x27;
		public const int VK_DOWN = 0x28;
		public const int VK_SELECT = 0x29;
		public const int VK_PRINT = 0x2A;
		public const int VK_EXECUTE = 0x2B;
		public const int VK_SNAPSHOT = 0x2C;
		public const int VK_INSERT = 0x2D;
		public const int VK_DELETE = 0x2E;
		public const int VK_HELP = 0x2F;
		public const int VK_LWIN = 0x5B;
		public const int VK_RWIN = 0x5C;
		public const int VK_APPS = 0x5D;
		public const int VK_SLEEP = 0x5F;
		public const int VK_NUMPAD0 = 0x60;
		public const int VK_NUMPAD1 = 0x61;
		public const int VK_NUMPAD2 = 0x62;
		public const int VK_NUMPAD3 = 0x63;
		public const int VK_NUMPAD4 = 0x64;
		public const int VK_NUMPAD5 = 0x65;
		public const int VK_NUMPAD6 = 0x66;
		public const int VK_NUMPAD7 = 0x67;
		public const int VK_NUMPAD8 = 0x68;
		public const int VK_NUMPAD9 = 0x69;
		public const int VK_MULTIPLY = 0x6A;
		public const int VK_ADD = 0x6B;
		public const int VK_SEPARATOR = 0x6C;
		public const int VK_SUBTRACT = 0x6D;
		public const int VK_DECIMAL = 0x6E;
		public const int VK_DIVIDE = 0x6F;
		public const int VK_F1 = 0x70;
		public const int VK_F2 = 0x71;
		public const int VK_F3 = 0x72;
		public const int VK_F4 = 0x73;
		public const int VK_F5 = 0x74;
		public const int VK_F6 = 0x75;
		public const int VK_F7 = 0x76;
		public const int VK_F8 = 0x77;
		public const int VK_F9 = 0x78;
		public const int VK_F10 = 0x79;
		public const int VK_F11 = 0x7A;
		public const int VK_F12 = 0x7B;
		public const int VK_F13 = 0x7C;
		public const int VK_F14 = 0x7D;
		public const int VK_F15 = 0x7E;
		public const int VK_F16 = 0x7F;
		/*
		public const int VK_F17 = 0x80H;
		public const int VK_F18 = 0x81H;
		public const int VK_F19 = 0x82H;
		public const int VK_F20 = 0x83H;
		public const int VK_F21 = 0x84H;
		public const int VK_F22 = 0x85H;
		public const int VK_F23 = 0x86H;
		public const int VK_F24 = 0x87H;
		*/
		public const int VK_NUMLOCK = 0x90;
		public const int VK_SCROLL = 0x91;
		public const int VK_LSHIFT = 0xA0;
		public const int VK_RSHIFT = 0xA1;
		public const int VK_LCONTROL = 0xA2;
		public const int VK_RCONTROL = 0xA3;
		public const int VK_LMENU = 0xA4;
		public const int VK_RMENU = 0xA5;
		public const int VK_BROWSER_BACK = 0xA6;
		public const int VK_BROWSER_FORWARD = 0xA7;
		public const int VK_BROWSER_REFRESH = 0xA8;
		public const int VK_BROWSER_STOP = 0xA9;
		public const int VK_BROWSER_SEARCH = 0xAA;
		public const int VK_BROWSER_FAVORITES = 0xAB;
		public const int VK_BROWSER_HOME = 0xAC;
		public const int VK_VOLUME_MUTE = 0xAD;
		public const int VK_VOLUME_DOWN = 0xAE;
		public const int VK_VOLUME_UP = 0xAF;
		public const int VK_MEDIA_NEXT_TRACK = 0xB0;
		public const int VK_MEDIA_PREV_TRACK = 0xB1;
		public const int VK_MEDIA_STOP = 0xB2;
		public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
		public const int VK_LAUNCH_MAIL = 0xB4;
		public const int VK_LAUNCH_MEDIA_SELECT = 0xB5;
		public const int VK_LAUNCH_APP1 = 0xB6;
		public const int VK_LAUNCH_APP2 = 0xB7;
		public const int VK_OEM_1 = 0xBA;
		public const int VK_OEM_PLUS = 0xBB;
		public const int VK_OEM_COMMA = 0xBC;
		public const int VK_OEM_MINUS = 0xBD;
		public const int VK_OEM_PERIOD = 0xBE;
		public const int VK_OEM_2 = 0xBF;
		public const int VK_OEM_3 = 0xC0;
		public const int VK_OEM_4 = 0xDB;
		public const int VK_OEM_5 = 0xDC;
		public const int VK_OEM_6 = 0xDD;
		public const int VK_OEM_7 = 0xDE;
		public const int VK_OEM_8 = 0xDF;
		public const int VK_OEM_102 = 0xE2;
		public const int VK_PROCESSKEY = 0xE5;
		public const int VK_PACKET = 0xE7;
		public const int VK_ATTN = 0xF6;
		public const int VK_CRSEL = 0xF7;
		public const int VK_EXSEL = 0xF8;
		public const int VK_EREOF = 0xF9;
		public const int VK_PLAY = 0xFA;
		public const int VK_ZOOM = 0xFB;
		public const int VK_NONAME = 0xFC;
		public const int VK_PA1 = 0xFD;
		public const int VK_OEM_CLEAR = 0xFE;
	}

    //Variables used in the call to SetWindowsHookEx
    private HookHandlerDelegate proc;
    private IntPtr hookID = IntPtr.Zero;
    internal delegate IntPtr HookHandlerDelegate(
        int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

	///// <summary>
	///// Event triggered when a keystroke is intercepted by the 
	///// low-level hook.
	///// </summary>
	//public event KeyboardHookEventHandler KeyIntercepted;

    // Structure returned by the hook whenever a key is pressed
    internal struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        int scanCode;
        public int flags;
        int time;
        int dwExtraInfo;
    }

    #region Constructors
    /// <summary>
    /// Sets up a keyboard hook to trap all keystrokes without 
    /// passing any to other applications.
    /// </summary>
    public CKeyboardBlock()
    {
        proc = new HookHandlerDelegate(HookCallback);
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            hookID = NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
        }
    }

	///// <summary>
	///// Sets up a keyboard hook with custom parameters.
	///// </summary>
	///// <param name="param">A valid name from the Parameter enum; otherwise, the 
	///// default parameter Parameter.None will be used.</param>
	//public KeyboardHook(string param)
	//    : this()
	//{
	//    if (!String.IsNullOrEmpty(param) && Enum.IsDefined(typeof(Parameters), param))
	//    {
	//        SetParameters((Parameters)Enum.Parse(typeof(Parameters), param));
	//    }
	//}

    /// <summary>
    /// Sets up a keyboard hook with custom parameters.
    /// </summary>
    /// <param name="param">A value from the Parameters enum.</param>
    public CKeyboardBlock(KeyboardBlockType BlockType)
        : this()
    {
		_BlockType = BlockType;
    }
    
	//private void SetParameters(Parameters param)
	//{
	//    switch (param)
	//    {
	//        case Parameters.None:
	//            break;
	//        case Parameters.AllowAltTab:
	//            AllowAltTab = true;
	//            break;
	//        case Parameters.AllowWindowsKey:
	//            AllowWindowsKey = true;
	//            break;
	//        case Parameters.AllowAltTabAndWindows:
	//            AllowAltTab = true;
	//            AllowWindowsKey = true;
	//            break;
	//        case Parameters.PassAllKeysToNextApp:
	//            PassAllKeysToNextApp = true;
	//            break;
	//    }
	//}
    #endregion

	public enum ModifierKeys
	{
		None,
		Control,
		Shift,
		Alt,
		CapsLock,
	}

	public static bool GetModifierKeyPressed(ModifierKeys Key)
    {
		if (Key == ModifierKeys.Control)
		{
			if ((NativeMethods.GetKeyState(VirtualKeys.VK_CONTROL) & 0x8000) != 0)
			{
				return true;
			}
		}
		else if (Key == ModifierKeys.Shift)
		{
			if ((NativeMethods.GetKeyState(VirtualKeys.VK_SHIFT) & 0x8000) != 0)
			{
				return true;
			}
		}
		//Alt 키는 IsHooked에서 안되서 LLKHF_ALTDOWN 사용
		else if (Key == ModifierKeys.Alt)
		{
			if ((NativeMethods.GetKeyState(VirtualKeys.VK_MENU) & 0x8000) != 0)
			{
				return true;
			}
		}
		else if (Key == ModifierKeys.CapsLock)
		{
			if ((NativeMethods.GetKeyState(VirtualKeys.VK_CAPITAL) & 0x0001) != 0)
			{
				return true;
			}
		}

		return false;
    }

    #region Hook Callback Method
    /// <summary>
    /// Processes the key event captured by the hook.
    /// </summary>
	private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
	{
		if (nCode == HC_ACTION)
		{
			if (CLang.In(wParam, (IntPtr)WM_KEYDOWN, (IntPtr)WM_KEYUP, (IntPtr)WM_SYSKEYDOWN, (IntPtr)WM_SYSKEYUP))
			{
				if (IsHooked(lParam))
				{
					//OnKeyIntercepted(new KeyboardHookEventArgs(lParam.vkCode, AllowKey));

					return (System.IntPtr)1;
				}
			}
		}
		//Pass key to next application
		return NativeMethods.CallNextHookEx(hookID, nCode, wParam, ref lParam);

	}
	private bool IsHooked(KBDLLHOOKSTRUCT lParam)
	{
		if ((lParam.vkCode == VirtualKeys.VK_ESCAPE) && GetModifierKeyPressed(ModifierKeys.Control))
		{
			Debug.WriteLine("Ctrl + Esc");
			return ((_BlockType & KeyboardBlockType.CtrlAndEsc) == KeyboardBlockType.CtrlAndEsc);
		}
		if ((lParam.vkCode == VirtualKeys.VK_TAB) && ((lParam.flags & LLKHF_ALTDOWN) == LLKHF_ALTDOWN))
		{
			Debug.WriteLine("Alt + Tab");
			return ((_BlockType & KeyboardBlockType.AltAndTab) == KeyboardBlockType.AltAndTab);
		}
		if ((lParam.vkCode == VirtualKeys.VK_ESCAPE) && ((lParam.flags & LLKHF_ALTDOWN) == LLKHF_ALTDOWN))
		{
			Debug.WriteLine("Alt + Esc");
			return ((_BlockType & KeyboardBlockType.AltAndEsc) == KeyboardBlockType.AltAndEsc);
		}
		if (CLang.In(lParam.vkCode, VirtualKeys.VK_LWIN, VirtualKeys.VK_RWIN))
		{
			Debug.WriteLine("Window Key");
			return ((_BlockType & KeyboardBlockType.WindowKey) == KeyboardBlockType.WindowKey);
		}

		Debug.WriteLine("false");
		return false;
	}
    #endregion

    #region Event Handling
	///// <summary>
	///// Raises the KeyIntercepted event.
	///// </summary>
	///// <param name="e">An instance of KeyboardHookEventArgs</param>
	//public void OnKeyIntercepted(KeyboardHookEventArgs e)
	//{
	//    if (KeyIntercepted != null)
	//        KeyIntercepted(e);
	//}

	///// <summary>
	///// Delegate for KeyboardHook event handling.
	///// </summary>
	///// <param name="e">An instance of InterceptKeysEventArgs.</param>
	//public delegate void KeyboardHookEventHandler(KeyboardHookEventArgs e);

	///// <summary>
	///// Event arguments for the KeyboardHook class's KeyIntercepted event.
	///// </summary>
	//public class KeyboardHookEventArgs : System.EventArgs
	//{

	//    private string keyName;
	//    private int keyCode;
	//    private bool passThrough;

	//    /// <summary>
	//    /// The name of the key that was pressed.
	//    /// </summary>
	//    public string KeyName
	//    {
	//        get { return keyName; }
	//    }

	//    /// <summary>
	//    /// The virtual key code of the key that was pressed.
	//    /// </summary>
	//    public int KeyCode
	//    {
	//        get { return keyCode; }
	//    }

	//    /// <summary>
	//    /// True if this key combination was passed to other applications,
	//    /// false if it was trapped.
	//    /// </summary>
	//    public bool PassThrough
	//    {
	//        get { return passThrough; }
	//    }

	//    public KeyboardHookEventArgs(int evtKeyCode, bool evtPassThrough)
	//    {
	//        keyName = ((Keys)evtKeyCode).ToString();
	//        keyCode = evtKeyCode;
	//        passThrough = evtPassThrough;
	//    }

	//}

    #endregion

    #region IDisposable Members
    /// <summary>
    /// Releases the keyboard hook.
    /// </summary>
    public void Dispose()
    {
        NativeMethods.UnhookWindowsHookEx(hookID);
    }
    #endregion

    #region Native methods

    [ComVisibleAttribute(false),
     System.Security.SuppressUnmanagedCodeSecurity()]
    internal class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook,
            HookHandlerDelegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
        public static extern short GetKeyState(int keyCode);
        
    } 
 

    #endregion
}


