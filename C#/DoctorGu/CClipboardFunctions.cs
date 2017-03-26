using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;   

namespace DoctorGu
{
	public struct SClipboardFormats
	{
		public const int CF_BITMAP = 2;
		public const int CF_DIB = 8;
		public const int CF_ENHMETAFILE = 14;
		public const int CF_METAFILEPICT = 3;
		public const int CF_OEMTEXT = 7;
		public const int CF_TEXT = 1;
		public const int CF_UNICODETEXT = 13;
	}

	/// <summary>
	/// Word의 CopyAsPicture로 복사한 MetaFile을 가져오기 위함.(CWord.SaveAsPng)
	/// </summary>
	public class CClipboardFunctions
	{
		[DllImport("user32.dll", EntryPoint = "OpenClipboard", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool OpenClipboard(IntPtr hWnd);

		[DllImport("user32.dll", EntryPoint = "EmptyClipboard", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool EmptyClipboard();

		[DllImport("user32.dll", EntryPoint = "SetClipboardData", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr SetClipboardData(int uFormat, IntPtr hWnd);

		[DllImport("user32.dll", EntryPoint = "CloseClipboard", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern bool CloseClipboard();

		[DllImport("user32.dll", EntryPoint = "GetClipboardData", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr GetClipboardData(int uFormat);

		[DllImport("user32.dll", EntryPoint = "IsClipboardFormatAvailable", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		public static extern short IsClipboardFormatAvailable(int uFormat);
	}
}
