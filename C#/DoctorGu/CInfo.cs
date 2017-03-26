using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace DoctorGu
{
	public struct SFromTo
	{
		public int From;
		public int To;
		public SFromTo(int From, int To)
		{
			this.From = From;
			this.To = To;
		}
	}


	/// <summary>
	/// Summary description for Info.
	/// </summary>
	public class CInfo
	{
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

		public static void Main(string[] args)
		{
			if (IsDebugMode)
				Console.WriteLine("Debug");
			else
				Console.WriteLine("Release");

			Console.ReadLine();

		}

		/// <summary>
		/// 현재 Debug Mode인지, Release Mode인지를 확인함.
		/// 주의할 것은 DLL 안에 이 함수가 있다면 호출하는 실행파일이
		/// Debug Mode라도 Debug Mode가 아닌 것으로 리턴함.
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// if (IsDebugMode)
		///	 Console.WriteLine("Debug");
		/// else
		///	 Console.WriteLine("Release");
		/// </example>
		public static bool IsDebugMode
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}

		public static string GetExceptionText(Exception ex, string DelimCol, string DelimRow)
		{
			string s = "";

			for (Exception exCur = ex; exCur != null; exCur = exCur.InnerException)
			{
                XmlException exCurXml = exCur as XmlException;

                s += DelimRow
                        + DelimRow + "Message" + DelimCol + exCur.Message
                        + DelimRow + "Source" + DelimCol + exCur.Source;

                if (exCurXml != null)
                {
                    s += DelimRow + "SourceUri" + DelimCol + exCurXml.SourceUri
                        + DelimRow + "LineNumber" + DelimCol + exCurXml.LineNumber
                        + DelimRow + "LinePosition" + DelimCol + exCurXml.LinePosition;
                }
                else
                {
                    s += DelimRow + "StackTrace" + DelimCol + exCur.StackTrace
                        + DelimRow + "TargetSite" + DelimCol + exCur.TargetSite;
                }
  
                if (ex.Data.Count > 0)
                    s += DelimRow + "Data" + DelimCol + GetExceptionDataString(exCur);
			}
            
			if (!string.IsNullOrEmpty(s))
				s = s.Substring((DelimRow + DelimRow).Length);

			return s;
		}
		public static string GetExceptionText(Exception ex)
		{
			return GetExceptionText(ex, ":\r\n", "\r\n");
		}

        private static string GetExceptionDataString(Exception ex)
        {
            JavaScriptSerializer jsSerial = new JavaScriptSerializer();
            return jsSerial.Serialize(ex.Data);
        }

		/// <summary>
		/// ProgramFiles(x86) 폴더만으로 체크하므로 완전하지는 않음.
		/// </summary>
		public static bool Is64bit
		{
			get { return (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null); }
		}

		///// <summary>
		///// Platform target을 따라감. AnyCPU일 때만 정확함.
		///// </summary>
		///// <returns></returns>
		//public static bool Is64bit()
		//{
		//    return (IntPtr.Size == 8);
		//}
		////http://social.msdn.microsoft.com/Forums/en/csharpgeneral/thread/24792cdc-2d8e-454b-9c68-31a19892ca53
		//public static bool Is64Bit()
		//{
		//    bool retVal;
		//    IsWow64Process(Process.GetCurrentProcess().Handle, out retVal);
		//    return retVal;
		//}
	}
}
