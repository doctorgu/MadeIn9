using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Collections;

namespace DoctorGu
{
	public class CInfoWindow
	{
		public IntPtr HandleParent;
		public IntPtr Handle;
		public Rectangle PositionSize;
		public ProcessWindowStyle WindowStyle;
		public string Caption;

		public override string ToString()
		{
			string CaptionParent = (HandleParent != IntPtr.Zero) ? HandleParent.GetWindowCaptionByHandle() : "";
			return string.Format("Parent:{0} {1}, Current: {2} {3} {4} {5}", HandleParent, CaptionParent, Handle, PositionSize, WindowStyle, Caption);
			//return string.Concat(HandleParent, ", ", Handle, ", ", PositionSize, ", ", WindowStyle, ", ", Caption);
		}
	}

	public static class CWnd
	{
		enum GetWindow_Cmd : uint
		{
			GW_HWNDFIRST = 0,
			GW_HWNDLAST = 1,
			GW_HWNDNEXT = 2,
			GW_HWNDPREV = 3,
			GW_OWNER = 4,
			GW_CHILD = 5,
			GW_ENABLEDPOPUP = 6
		}

		const int SC_CLOSE = 0xF060;
		const int WM_SETFOCUS = 0x7;
		const int WM_SYSCOMMAND = 0x0112;

		const int SW_HIDE = 0;
		//const int SW_SHOWNORMAL = 1;
		const int SW_NORMAL = 1;
		const int SW_SHOWMINIMIZED = 2;
		//const int SW_SHOWMAXIMIZED = 3;
		const int SW_MAXIMIZE = 3;
		const int SW_SHOWNOACTIVATE = 4;
		const int SW_SHOW = 5;
		const int SW_MINIMIZE = 6;
		const int SW_SHOWMINNOACTIVE = 7;
		const int SW_SHOWNA = 8;
		const int SW_RESTORE = 9;

		[StructLayout(LayoutKind.Sequential)]
		struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}

		private struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}


		[DllImport("user32.dll")]
		static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		static extern IntPtr SetForegroundWindow(IntPtr hWnd);
		[DllImport("kernel32.dll")]
		static extern IntPtr GetCurrentThreadId();
		[DllImport("user32.dll")]
		static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);
		[DllImport("user32.dll")]
		static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, bool fAttach);
		[DllImport("user32.dll")]
		static extern IntPtr SetActiveWindow(IntPtr hWnd);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[DllImport(@"user32.dll", EntryPoint = "SetWindowPos", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		//해당 윈도우의 핸들에 대한 자식 윈도우 핸들이나 다음 윈도우 핸들을 얻음.
		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);
		//모든 윈도우 중 최상위인 DesktopWindow의 핸들을 얻음
		[DllImport("user32.dll", SetLastError = false)]
		static extern IntPtr GetDesktopWindow();
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool IsWindowVisible(IntPtr hWnd);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
		[DllImport("user32.dll")]
		static extern int PostMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);
		[DllImport("user32.dll")]
		static extern IntPtr GetParent(IntPtr hWnd);

		private static void GetHandleList(ref Dictionary<IntPtr, IntPtr> aChildParentHandle, IntPtr hWndParent, bool Deep)
		{
			//첫번째 윈도우의 핸들을 가져옴
			IntPtr hWnd = GetWindow(hWndParent, GetWindow_Cmd.GW_CHILD);
			if (hWnd == IntPtr.Zero)
				return;

			if (IsWindowVisible(hWnd))
				aChildParentHandle.Add(hWnd, hWndParent);

			while (true)
			{
				//다음 윈도우의 핸들을 가져옴
				hWnd = GetWindow(hWnd, GetWindow_Cmd.GW_HWNDNEXT);
				if (hWnd == IntPtr.Zero)
					break;

				if (IsWindowVisible(hWnd))
					aChildParentHandle.Add(hWnd, hWndParent);

				if (Deep)
					GetHandleList(ref aChildParentHandle, hWnd, Deep);
			}
		}

		public static List<CInfoWindow> GetAllVisibleWindowInfo(bool Deep)
		{
			List<CInfoWindow> aInfo = new List<CInfoWindow>();

			Dictionary<IntPtr, IntPtr> aChildParentHandle = new Dictionary<IntPtr, IntPtr>();
			GetHandleList(ref aChildParentHandle, GetDesktopWindow(), Deep);
			foreach (KeyValuePair<IntPtr, IntPtr> kv in aChildParentHandle)
			{
				CInfoWindow Info = GetWindowInfo(kv.Key);
				Info.HandleParent = kv.Value;
				aInfo.Add(Info);
			}

			return aInfo;
		}
		public static List<CInfoWindow> GetAllVisibleChildWindowInfo(this IntPtr hWndParent, bool Deep)
		{
			List<CInfoWindow> aInfo = new List<CInfoWindow>();

			Dictionary<IntPtr, IntPtr> aChildParentHandle = new Dictionary<IntPtr, IntPtr>();
			GetHandleList(ref aChildParentHandle, hWndParent, Deep);
			foreach (KeyValuePair<IntPtr, IntPtr> kv in aChildParentHandle)
			{
				CInfoWindow Info = GetWindowInfo(kv.Key);
				Info.HandleParent = kv.Value;
				aInfo.Add(Info);
			}

			return aInfo;
		}
		public static CInfoWindow GetWindowInfo(this IntPtr hWnd)
		{
			Rectangle PosSize = CWnd.GetWindowPositionSize(hWnd);
			ProcessWindowStyle WindowStyle = CWnd.GetWindowStyle(hWnd);
			string Caption = CWnd.GetWindowCaptionByHandle(hWnd);
			return new CInfoWindow() { Handle = hWnd, PositionSize = PosSize, WindowStyle = WindowStyle, Caption = Caption };
		}

		public static string GetWindowCaptionByHandle(this IntPtr hWnd)
		{
			StringBuilder Caption = new StringBuilder(256);
			int nLength = GetWindowText(hWnd, Caption, Caption.Capacity + 1);
			return Caption.ToString();
		}

		public static IntPtr GetHandleByWindowCaption(string WindowCaption)
		{
			IntPtr hWnd = FindWindow(null, WindowCaption);
			return hWnd;
		}

		public static IntPtr GetHandleByWindowCaptionLike(string WindowCaption)
		{
			Dictionary<IntPtr, IntPtr> aChildParent = new Dictionary<IntPtr, IntPtr>();
			GetHandleList(ref aChildParent, GetDesktopWindow(), true);

			foreach (KeyValuePair<IntPtr, IntPtr> kv in aChildParent)
			{
				string Caption = GetWindowCaptionByHandle(kv.Key);
				if (string.IsNullOrEmpty(Caption))
					continue;

				//Debug.WriteLine(Caption);
				if (CLang.Like(Caption.ToString(), WindowCaption, true))
					return kv.Key;
			}

			return IntPtr.Zero;
		}

		public static IntPtr GetHandleByWindowCaptionAndSizeLike(string Caption, Size Size, int SizeTolerance)
		{
			Dictionary<IntPtr, IntPtr> aChildParent = new Dictionary<IntPtr, IntPtr>();
			GetHandleList(ref aChildParent, GetDesktopWindow(), true);

			foreach (KeyValuePair<IntPtr, IntPtr> kv in aChildParent)
			{
				CInfoWindow Info = kv.Key.GetWindowInfo();
				if (!string.IsNullOrEmpty(Caption) && CLang.Like(Info.Caption, Caption, true)
					&& (Math.Abs(Info.PositionSize.Width - Size.Width) <= SizeTolerance)
					&& (Math.Abs(Info.PositionSize.Height - Size.Height) <= SizeTolerance)
					)
					return kv.Key;
			}

			return IntPtr.Zero;
		}

		public static void SetWindowPositionSize(this IntPtr hWnd, int X, int Y, int Width, int Height)
		{
			SetWindowPos(hWnd, (IntPtr)null, X, Y, Width, Height, 0u);
		}
		public static void SetWindowPositionSize(string WindowName, int X, int Y, int Width, int Height)
		{
			IntPtr hWnd = GetHandleByWindowCaption(WindowName);
			SetWindowPos(hWnd, (IntPtr)null, X, Y, Width, Height, 0u);
		}

		public static IntPtr GetFocused()
		{
			IntPtr hWnd = GetForegroundWindow();
			return hWnd;
		}

		public static string GetForegroundCaption()
		{
			IntPtr hWnd = GetForegroundWindow();

			StringBuilder sbCaption = new StringBuilder(256);
			GetWindowText(hWnd, sbCaption, sbCaption.Capacity);
			return sbCaption.ToString();
		}

		/// <summary>
		/// 맨 앞의 윈도우가 MDI 형식일 경우,
		/// "Happy Manager - [홈페이지 메인/고정글]" 에서 "Happy Manager"와 "홈페이지 메인/고정글"을 분리함.
		/// </summary>
		/// <param name="CaptionParentIs"></param>
		/// <param name="CaptionChildIs"></param>
		/// <example>
		/// string CaptionParent, CaptionChild;
		/// CWnd.GetForegroundCaption(out CaptionParent, out CaptionChild);
		/// </example>
		public static void GetForegroundCaption(out string CaptionParentIs, out string CaptionChildIs)
		{
			CaptionParentIs = "";
			CaptionChildIs = "";

			string Caption = GetForegroundCaption();
			CWinForm.GetMdiParentChildCaption(Caption, out CaptionParentIs, out CaptionChildIs);
		}

		public static Rectangle GetWindowPositionSize(this IntPtr hWnd)
		{
			RECT rct;
			bool IsSuccess = GetWindowRect(hWnd, out rct);
			if (!IsSuccess)
				return new Rectangle();

			return new Rectangle(rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top);
		}

		public static void CloseWindow(this IntPtr hWnd)
		{
			// close the window using API        
			int Ret = SendMessage(hWnd, WM_SYSCOMMAND, SC_CLOSE, 0);
		}

		public static int GetProcessId(this IntPtr hWnd)
		{
			uint ProcId;
			GetWindowThreadProcessId(hWnd, out ProcId);
			return (int)ProcId;
		}

		public static void SetFocus(this IntPtr hWnd)
		{
			SetForegroundWindow(hWnd);

			//PostMessage(hWnd, WM_SETFOCUS, 0, 0); 테스트 결과 포커스 안되어 주석

			//http://stackoverflow.com/questions/2671669/is-there-a-reliable-way-to-activate-set-focus-to-a-window-using-c
			//IntPtr currentThreadId = GetCurrentThreadId();
			//IntPtr otherThreadId = GetWindowThreadProcessId(hWnd, IntPtr.Zero);
			//if (otherThreadId == IntPtr.Zero)
			//    return;
			//if (otherThreadId != currentThreadId)
			//{
			//    AttachThreadInput(currentThreadId, otherThreadId, true);
			//}

			//SetActiveWindow(hWnd);

			//if (otherThreadId != currentThreadId)
			//{
			//    AttachThreadInput(currentThreadId, otherThreadId, false);
			//}
		}

		public static ProcessWindowStyle GetWindowStyle(this IntPtr hWnd)
		{
			WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			GetWindowPlacement(hWnd, ref placement);
			switch (placement.showCmd)
			{
				case SW_HIDE:
					return ProcessWindowStyle.Hidden;
				case SW_NORMAL:
					return ProcessWindowStyle.Normal;
				case SW_MINIMIZE:
				case SW_SHOWMINIMIZED:
					return ProcessWindowStyle.Minimized;
				case SW_MAXIMIZE:
					return ProcessWindowStyle.Maximized;
				default:
					throw new Exception(string.Format("Wrong showCmd: {0}", placement.showCmd));
			}
		}

		public static IntPtr GetHandleByProcessName(string ProcessName)
		{
			IntPtr hWndParent = IntPtr.Zero;

			foreach (Process ProcCur in Process.GetProcesses())
			{
				if (ProcCur.ProcessName == ProcessName)
				{
					hWndParent = ProcCur.MainWindowHandle;
					break;
				}
			}

			return hWndParent;
		}

		public static bool IsChildOfProcess(this IntPtr hWndChild, string[] aProcessParent)
		{
			foreach (string ProcessName in aProcessParent)
			{
				IntPtr hWndParent = GetHandleByProcessName(ProcessName);
				if (IsChildOf(hWndChild, hWndParent))
					return true;
			}

			return false;
		}
		public static bool IsChildOfProcess(this IntPtr hWndChild, string ProcessParent)
		{
			return IsChildOfProcess(hWndChild, new string[] { ProcessParent });
		}
		public static bool IsChildOf(this IntPtr hWndChild, IntPtr hWndParent)
		{
			if (hWndParent == IntPtr.Zero)
				return false;

			int nParent = 0;
			while (hWndChild != IntPtr.Zero)
			{
				hWndChild = GetParent(hWndChild);
				nParent++;

				if (hWndChild == hWndParent)
					return true;
			}

			return false;
		}
	}
}
