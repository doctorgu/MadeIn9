using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
//using DoctorGu.ShellLib;

namespace DoctorGu
{
	public class CDownloadProgressChangedEventArgs : CProgressChangedEventArgs
	{
		//FileInfo의 GetHashCode는 같은 파일이라도 FileInfo를 생성할 때마다 다른 값을 리턴함.
		//그래서 전체 경로를 기초로 하는 ID 값을 만들려 했으나 한글 파일명이 있을 경우엔 컨트롤 이름으로 쓸 수 있는 ID를 만들 수가 없어 보류함.
		//public int SourceFileHashCode;

		public string FullPath;
		public long TotalBytesToReceive;
		public long BytesReceived;
		public string Tag;
	}

	public class CBeforeDirectoryCopyEventArgs : EventArgs
	{
		public string FolderSrc;
		public string FolderDest;
		public bool Cancel;
	}

	public class CBeforeFileCopyEventArgs : EventArgs
	{
		public string FullPathSrc;
		public string FullPathSrcNew;
		public string FullPathDest;
		public bool Cancel;
	}
	public class CAfterFileCopyEventArgs : EventArgs
	{
		public string FullPathSrc;
		public string FullPathSrcNew;
		public string FullPathDest;
	}
	public class CAfterFileCopyFailedEventArgs : CAfterFileCopyEventArgs
	{
		public Exception Exception;
	}

	public enum WebSafeImageExtension
	{
		[Description(".gif")]
		Gif,
		[Description(".jpg")]
		Jpg,
		[Description(".jpeg")]
		Jpeg,
		[Description(".bmp")]
		Bmp,
		[Description(".wmf")]
		Wmf,
		[Description(".png")]
		Png,
		[Description(".ico")]
		Ico,
	}

	public enum FileActionIfExists
	{
		Overwrite,
		RenameByNumbering,
	}

	/// <summary>
	/// 파일과 관련된 기능 구현.
	/// </summary>
	/// <remarks>
	/// 참고로 다음은 Environment.GetFolderPath 메쏘드가 리턴하는 값음.
	/// <code>
	/// ApplicationData: C:\Documents and Settings\doctorgu\Application Data
	/// CommonApplicationData: C:\Documents and Settings\All Users\Application Data
	/// CommonProgramFiles: C:\Program Files\Common Files
	/// Cookies: C:\Documents and Settings\doctorgu\Cookies
	/// DesktopDirectory: C:\Documents and Settings\doctorgu\바탕 화면
	/// Favorites: C:\Documents and Settings\doctorgu\Favorites
	/// History: C:\Documents and Settings\doctorgu\Local Settings\History
	/// InternetCache: C:\Documents and Settings\doctorgu\Local Settings\Temporary Internet Files
	/// LocalApplicationData: C:\Documents and Settings\doctorgu\Local Settings\Application Data
	/// Personal: C:\Documents and Settings\doctorgu\My Documents
	/// ProgramFiles: C:\Program Files
	/// Programs: C:\Documents and Settings\doctorgu\시작 메뉴\프로그램
	/// Recent: C:\Documents and Settings\doctorgu\Recent
	/// SendTo: C:\Documents and Settings\doctorgu\SendTo
	/// StartMenu: C:\Documents and Settings\doctorgu\시작 메뉴
	/// Startup: C:\Documents and Settings\doctorgu\시작 메뉴\프로그램\시작프로그램
	/// System: C:\WINNT\System32
	/// Templates: C:\Documents and Settings\doctorgu\Templates
	/// </code>
	/// </remarks>
	public class CFile
	{
		[DllImport("kernel32.dll")]
		private static extern int GetWindowsDirectory(StringBuilder lpBuffer, int nSize);

		[DllImport("kernel32.dll")]
		private static extern long GetVolumeInformation(string PathName, StringBuilder VolumeNameBuffer, UInt32 VolumeNameSize, ref UInt32 VolumeSerialNumber, ref UInt32 MaximumComponentLength, ref UInt32 FileSystemFlags, StringBuilder FileSystemNameBuffer, UInt32 FileSystemNameSize);

		/// <summary>Cancel 값을 false로 만들면 해당 작업이 취소됨.</summary>
		public event EventHandler<CBeforeDirectoryCopyEventArgs> BeforeDirectoryCopy;

		/// <summary>Cancel 값을 false로 만들면 해당 작업이 취소됨.</summary>
		public event EventHandler<CBeforeFileCopyEventArgs> BeforeFileCopy;

		public event EventHandler<CAfterFileCopyEventArgs> AfterFileCopy;

		public event EventHandler<CAfterFileCopyFailedEventArgs> AfterFileCopyFailed;

		public event EventHandler<CDownloadProgressChangedEventArgs> DownloadProgress;

		//파일이나 스트림을 받고 보내는 쪽의 크기를 맞추기 위함.
		//(받고 보내는 쪽의 크기가 서로 틀리는 경우 잘못된다는 것은 증명된 것이 없음)
		private const int _4096 = 4096;

		/// <summary>
		/// 텍스트를 파일에 기록함. 이미 존재하는 파일이면 덮어쓰게 되므로 이전 내용은 사라짐.
		/// </summary>
		/// <param name="FullPath">텍스트 파일의 전체 경로</param>
		/// <param name="Value">텍스트 파일에 기록될 문자열</param>
		/// <example>
		/// 다음은 "안녕"이란 문자열을 Hi.txt 파일에 기록합니다.
		/// <code>
		/// string FullPath = @"C:\Temp\Hi.txt";
		/// string Value = "안녕";
		/// CFile.WriteTextToFile(FullPath, Value);
		/// </code>
		/// </example>
		public static void WriteTextToFile(string FullPath, string Value, Encoding enc)
		{
			using (StreamWriter sw = new StreamWriter(FullPath, false, enc))
			{
				sw.Write(Value);
				sw.Close();
			}
		}
		public static void WriteTextToFile(string FullPath, string Value)
		{
			WriteTextToFile(FullPath, Value, Encoding.UTF8);
		}

		/// <summary>
		/// 바이트를 파일에 기록함. 이미 존재하는 파일이면 덮어쓰게 되므로 이전 내용은 사라짐.
		/// </summary>
		/// <param name="FullPath">이진 파일의 전체 경로</param>
		/// <param name="aValue">이진 파일에 기록된 바이트</param>
		/// <example>
		/// 다음은 ASPX 페이지에서 업로드된 파일을 test.gif로 저장합니다.
		/// <code>
		/// try
		/// {
		/// 	byte[] aValue = CFile.GetByteFromStream(Request.InputStream);
		/// 	CFile.WriteByteToFile(@"D:\upload\test.gif", aValue);
		/// }
		/// catch (Exception ex)
		/// {
		/// 	Response.Write(ex.Message + "\r\n" + ex.StackTrace);
		/// 	Response.End();
		/// }
		/// </code>
		/// </example>
		public static void WriteByteToFile(string FullPath, byte[] aValue)
		{
			using (FileStream strm = new FileStream(FullPath, FileMode.Create))
			{
				strm.Write(aValue, 0, aValue.Length);
				strm.Close();
			}
		}

		/// <summary>
		/// 텍스트를 기존 파일의 텍스트의 마지막 다음 줄에 추가함. 기존 파일이 없으면 새로운 파일이 만들어짐.
		/// </summary>
		/// <param name="FullPath">전체 경로</param>
		/// <param name="Value">추가될 문자열</param>
		/// <example>
		/// 다음은 Append.txt 파일에 "1줄", "2줄" 문자열을 추가하고, 파일의 내용을 읽어 출력합니다.
		/// <code>
		/// string FullPath = @"C:\Temp\Append.txt";
		/// CFile.AppendTextToFile(FullPath, "1줄");
		/// CFile.AppendTextToFile(FullPath, "2줄");
		///
		/// string Value = CFile.GetTextInFile(FullPath);
		/// Console.WriteLine(Value);
		///
		/// //--결과
		/// //1줄
		/// //2줄
		/// </code>
		/// </example>
		public static void AppendTextToFile(string FullPath, string Value, Encoding enc)
		{
			using (StreamWriter sw = new StreamWriter(FullPath, true, enc))
			{
				sw.WriteLine(Value);
				//sw.Close();
			}
		}
		public static void AppendTextToFile(string FullPath, string Value)
		{
			AppendTextToFile(FullPath, Value, Encoding.UTF8);
		}

		/// <summary>
		/// 텍스트 파일에 기록된 모든 문자열을 읽어서 리턴함.
		/// </summary>
		/// <param name="FullPath">전체 경로</param>
		/// <returns>텍스트 문자열</returns>
		/// <example>
		/// 다음은 Append.txt 파일에 "1줄", "2줄" 문자열을 추가하고, 파일의 내용을 읽어 출력합니다.
		/// <code>
		/// string FullPath = @"C:\Temp\Append.txt";
		/// CFile.AppendTextToFile(FullPath, "1줄");
		/// CFile.AppendTextToFile(FullPath, "2줄");
		///
		/// string Value = CFile.GetTextInFile(FullPath);
		/// Console.WriteLine(Value);
		///
		/// //--결과
		/// //1줄
		/// //2줄
		/// </code>
		/// </example>
		public static string GetTextInFile(string FullPath, Encoding enc)
		{
			string Value = "";
			using (FileStream fs = new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader sr = new StreamReader(fs, enc))
				{
					Value = sr.ReadToEnd();
					sr.Close();
				}
			}
			return Value;

			//string Value = "";
			//using (StreamReader sr = new StreamReader(FullPath, enc))
			//{
			//    Value = sr.ReadToEnd();
			//    sr.Close();
			//}
			//return Value;
		}
		public static string GetTextInFile(string FullPath)
		{
			return GetTextInFile(FullPath, Encoding.UTF8);
		}

		/// <summary>
		/// 이진 파일에 기록된 모든 바이트를 읽어서 리턴함.
		/// </summary>
		/// <param name="FullPath">이진 파일의 전체 경로</param>
		/// <returns>이진 파일</returns>
		/// <example>
		/// 다음은 again.mid 파일을 읽어 바이트 수를 출력합니다.
		/// <code>
		/// string FullPath = @"C:\Temp\again.mid";
		/// byte[] aValue = CFile.GetByteFromFile(FullPath);
		/// Console.WriteLine(aValue.Length); // 3116
		/// </code>
		/// </example>
		public static byte[] GetByteFromFile(string FullPath)
		{
			FileStream fs = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
			byte[] aFile = new Byte[fs.Length];
			fs.Read(aFile, 0, (int)fs.Length);
			fs.Close();
			return aFile;
		}

		public static byte[] GetByteFromString(string Value)
		{
			return GetByteFromString(Value, Encoding.UTF8);
		}
		public static byte[] GetByteFromString(string Value, Encoding en)
		{
			/*
http://social.msdn.microsoft.com/forums/en-US/csharpgeneral/thread/08e4553e-690e-458a-87a4-9762d8d405a6/

byte[] byteArray = {192, 65, 66, 67}; 

string str1 = System.Text.Encoding.ASCII.GetString(byteArray); 
string str2 = System.Text.Encoding.Unicode.GetString(byteArray); 
string str3 = System.Text.Encoding.UTF8.GetString(byteArray); 
string str4 = System.Text.Encoding.Default.GetString(byteArray); 

byte[] res1 = System.Text.Encoding.ASCII.GetBytes(str1);//returns {63, 65, 66, 67} 
byte[] res2 = System.Text.Encoding.Unicode.GetBytes(str2);//returns  {192, 65, 66, 67} 
byte[] res3 = System.Text.Encoding.UTF8.GetBytes(str3);//returns {239, 191, 189, 65, 66, 67} 
byte[] res4 = System.Text.Encoding.Default.GetBytes(str4);//returns {192, 65, 66, 67}; 
			*/

			return en.GetBytes(Value);
		}
		public static string GetStringFromByte(byte[] aValue, Encoding en)
		{
			return en.GetString(aValue);
		}
		public static string GetStringFromByte(byte[] aValue)
		{
			return GetStringFromByte(aValue, Encoding.UTF8);
		}

		/// <summary>
		/// Stream 형식의 데이터를 Byte 형식으로 변경해서 리턴함.
		/// </summary>
		/// <param name="Strm">Stream 개체</param>
		/// <param name="UseMemoryStream">true이면 MemoryStream을 이용함. 용량이 큰 파일인 경우 Memory를 많이 차지하므로 false로 하는 것이 좋음.</param>
		/// <returns>byte 형식의 개체</returns>
		/// <example>
		/// 다음은 ASPX 페이지에서 업로드된 파일을 Test.gif 파일에 씁니다.
		/// <code>
		/// try
		/// {
		/// 	byte[] aValue = CFile.GetByteFromStream(Request.InputStream);
		/// 	CFile.WriteByteToFile(@"C:\Temp\Test.gif", aValue);
		/// }
		/// catch (Exception ex)
		/// {
		/// 	Response.Write(ex.Message + "\r\n" + ex.StackTrace);
		/// 	Response.End();
		/// }
		/// </code>
		/// </example>
		public static byte[] GetByteFromStream(Stream Strm, bool UseMemoryStream)
		{
			if (!UseMemoryStream)
			{
				byte[] aByteAll = new byte[] { };

				byte[] aBuffer = new byte[_4096];
				int BytesRead = 0;
				while ((BytesRead = Strm.Read(aBuffer, 0, _4096)) > 0)
				{
					aByteAll = CFile.Combine(aByteAll, aBuffer, BytesRead);
				}

				return aByteAll;
			}
			else
			{
				using (MemoryStream ms = GetMemoryStreamFromStream(Strm))
				{
					return ms.ToArray();
				}
			}
		}

		/// <summary>
		/// TCP 전송 메세지의 경우 길이를 넘어 읽으면 에러가 나서 멈추므로 정확하게 StreamSize만큼만 읽음.
		/// </summary>
		public static byte[] GetByteFromStream(Stream Strm, int StreamSize)
		{
			long BytesReceived = 0;

			int BytesToRead = Convert.ToInt32(Math.Min(StreamSize, _4096));

			byte[] aByteAll = new byte[] { };

			byte[] aBuffer = new byte[BytesToRead];
			do
			{
				int BytesRead = Strm.Read(aBuffer, 0, BytesToRead);
				//2012-12-12 추가
				if (BytesRead <= 0)
					break;

				BytesReceived += BytesRead;

				aByteAll = CFile.Combine(aByteAll, aBuffer, BytesRead);

			} while ((BytesToRead = Convert.ToInt32(Math.Min((StreamSize - BytesReceived), _4096))) > 0);

			return aByteAll;
		}

		public static byte[] GetByteFromStream(Stream Strm)
		{
			bool UseMemoryStream = false;
			return GetByteFromStream(Strm, UseMemoryStream);
		}
		public static MemoryStream GetMemoryStreamFromByte(byte[] aByte)
		{
			MemoryStream ms = new MemoryStream();
			ms.Write(aByte, 0, aByte.Length);
			return ms;
		}
		public static MemoryStream GetMemoryStreamFromFile(string FullPath)
		{
			byte[] aByte = GetByteFromFile(FullPath);
			return GetMemoryStreamFromByte(aByte);
		}
		public static MemoryStream GetMemoryStreamFromStream(Stream stream)
		{
			byte[] buffer = new byte[_4096];
			MemoryStream ms = new MemoryStream();

			while (true)
			{
				int read = stream.Read(buffer, 0, buffer.Length);
				if (read <= 0)
				{
					return ms;
				}

				ms.Write(buffer, 0, read);
			}
		}

		public static string GetStringFromStream(Stream Strm)
		{
			byte[] aByte = GetByteFromStream(Strm);
			string s = GetStringFromByte(aByte);
			return s;
		}

		public static void CopyStream(System.IO.Stream From, System.IO.Stream To)
		{
			byte[] aBuffer = new byte[_4096];
			int BytesRead;
			while ((BytesRead = From.Read(aBuffer, 0, aBuffer.Length)) != 0)
			{
				To.Write(aBuffer, 0, BytesRead);
			}
		}

		/// <summary>
		/// 현재 파일을 쓸 수 있는 지 여부를 리턴함.
		/// </summary>
		public static bool GetIsWritableFile(string FullPath)
		{
			try
			{
				using (FileStream fs = File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.None))
				{
					fs.Close();
				}
			}
			catch (IOException)
			{
				return false;
			}

			return true;
		}


		/// <summary>
		/// 특정 폴더에서 패턴에 해당하는 첫번째 파일을 찾아서 리턴합니다.
		/// </summary>
		/// <param name="Folder">찾기 시작할 폴더명</param>
		/// <param name="Pattern">패턴(?, *)</param>
		/// <param name="IsIncludeSubFolder">하위 폴더를 포함할 지 여부</param>
		/// <returns>찾아진 첫번째 파일에 해당하는 FileInfo 개체. 찾지 못하면 null을 리턴함.</returns>
		/// <example>
		/// 다음은 txt 확장자를 가진 첫번째 파일의 C:\Temp 폴더에서 찾아서 전체 경로를 출력합니다.
		/// <code>
		/// string Folder = @"C:\Temp";
		/// string File = "*.txt";
		/// FileInfo fi = CFile.FindFirstFile(Folder, File, true);
		/// Console.WriteLine(fi.FullName);
		/// </code>
		/// </example>
		public static FileInfo FindFirstFile(string Folder, string Pattern, bool IsIncludeSubFolder)
		{
			DirectoryInfo di = new DirectoryInfo(Folder);
			FileInfo[] fi = di.GetFiles(Pattern,
				(IsIncludeSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

			if (fi.Length > 0)
			{
				return fi[0];
			}

			return null;
		}

		/// <summary>
		/// Pattern(*, ? 와일드카드 포함)에 해당하는 모든 파일의 FileInfo 배열을 리턴함.
		/// </summary>
		/// <param name="StartFolder">파일을 찾을 폴더의 위치</param>
		/// <param name="Pattern">*, ? 등을 포함하는 표현식(예:My*를 입력하면 My로 시작하는 모든 파일을 찾음.</param>
		/// <param name="IsIncludeSubFolder">StartFolder 폴더 안에 하위 폴더가 존재할 경우 하위 폴더 안의 파일까지 찾는 지 여부.</param>
		/// <returns>찾은 파일의 경로 전체를 항목으로 하는 배열</returns>
		/// <example>
		/// 다음은 C:\Temp 폴더에서 txt 확장자를 가진 모든 파일의 전체 경로와 찾은 개수를 출력합니다.
		/// <code>
		/// FileInfo[] aFullPath = CFile.FindFiles(@"C:\Temp", "*.txt", true);
		/// Console.WriteLine("찾은 파일:");
		/// foreach (FileInfo fi in aFullPath)
		/// {
		///	 Console.WriteLine(fi.FullName);
		/// }
		/// Console.WriteLine("찾은 개수: {0}", aFullPath.Length);
		/// 
		/// //--결과
		/// //찾은 파일:
		/// //C:\Temp\Append.txt
		/// //C:\Temp\Hi.txt
		/// //C:\Temp\sdd.txt
		/// //C:\Temp\f1\-wrapper.txt
		/// //C:\Temp\f1\sdd.txt
		/// //C:\Temp\f1\wrapper.txt
		/// //찾은 개수: 6
		/// </code>
		/// </example>
		public static FileInfo[] FindFiles(string StartFolder, string Pattern, bool IsIncludeSubFolder)
		{
			DirectoryInfo di = new DirectoryInfo(StartFolder);
			FileInfo[] afi = di.GetFiles(Pattern,
				(IsIncludeSubFolder ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));

			return afi;
		}

		/// <summary>
		/// 현재 Windows 폴더의 위치를 리턴함.
		/// Environment.GetFolderPath는 이 기능이 없음.
		/// </summary>
		/// <returns>Windows 폴더의 위치</returns>
		/// <example>
		/// <code>
		/// string Win = CFile.GetWindowsFolder();
		/// Console.WriteLine(Win); // "C:\Windows"
		/// </code>
		/// </example>
		public static string GetWindowsFolder()
		{
			StringBuilder WinDir = new StringBuilder(256);
			int Result = GetWindowsDirectory(WinDir, WinDir.Capacity);
			return WinDir.ToString();
		}

		/// <summary>
		/// Temp 폴더에 특정 확장자를 가진 임시 파일 이름을 생성하고 리턴함.
		/// Path.GetTempFileName() 함수는 실제로 파일을 만들지만 이 함수는 파일을 만들지 않음.
		/// 참고로 Temp 폴더 65536개 이상의 파일이 있으면 the file exists 에러 남.
		/// </summary>
		/// <returns>임시 파일의 전체 경로</returns>
		public static string GetTempFileName(string Extension)
		{
			if (!string.IsNullOrEmpty(Extension) && !Extension.StartsWith("."))
			{
				Extension = "." + Extension;
			}

			return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + Extension);

			//Random rnd = new Random();

			//string TempPath = Path.GetTempPath();
			//string FullPath = Path.Combine(TempPath, string.Concat("Temp", DateTime.Now.Second.ToString("00"), DateTime.Now.Millisecond.ToString("000"), rnd.Next(0, 1000).ToString("000"), Extension));

			//for (int i = 0, i2 = 1000; i < i2; i++)
			//{
			//    if (File.Exists(FullPath))
			//    {
			//        if (IsDeletePreviousTempFile)
			//        {
			//            try
			//            {
			//                File.Delete(FullPath);
			//            }
			//            catch (Exception)
			//            {
			//                //삭제가 실패했다면 다음 파일 이름을 가져옴.
			//                FullPath = Path.Combine(TempPath, string.Concat("Temp", DateTime.Now.Second.ToString("00"), DateTime.Now.Millisecond.ToString("000"), rnd.Next(0, 1000).ToString("000"), Extension));
			//                continue;
			//            }

			//            //삭제가 성공했다면 해당 파일 이름을 리턴함.
			//            return FullPath;
			//        }
			//        else
			//        {
			//            FullPath = Path.Combine(TempPath, string.Concat("Temp", DateTime.Now.Second.ToString("00"), DateTime.Now.Millisecond.ToString("000"), rnd.Next(0, 1000).ToString("000"), Extension));
			//        }
			//    }
			//    else
			//    {
			//        return FullPath;
			//    }
			//}

			//throw new Exception("임시 파일을 가져올 수 없습니다.");
		}
		public static string GetTempFileName()
		{
			string Extension = "";
			return GetTempFileName(Extension);
		}

		/// <summary>
		/// 파일이 만들어지기까지 시간이 걸리므로 다 만들어질 때까지 기다림.
		/// !!! 실제로 파일이 만들어지지 않았으나 Exists 메쏘드는 만들어진 것으로 읽어지는 경우도 있음.
		/// 가능하면 Process.WaitForExit 사용할 것.
		/// </summary>
		/// <param name="FullPath"></param>
		public static void WaitForFileCreation(string FullPath, int SecondsToWait = 5)
		{
			//throw new Exception("사용 불가");

			for (int i = 0; i < SecondsToWait; i++)
			{
				//다음 에러 발생할 수 있어 try 사용
				//The process cannot access the file 'D:\My\MadeIn9\국가영어\www.kept.or.kr\Debug\client\_AU_app_folder_date.20120908135555.zip.tmp' because it is being used by another process.
				try
				{
					FileInfo fi = new FileInfo(FullPath);
					if (fi.Exists)
					{
						if (fi.Length > 0)
							break;
					}
				}
				catch (Exception) { }

				System.Threading.Thread.Sleep(1000);
			}
		}

		public static void DeleteUntilSuccess(string FullPath, int MaxMilliseconds)
		{
			int nWaited = 0;
			while (File.Exists(FullPath) && (nWaited < MaxMilliseconds))
			{
				//AutoUpdate-Temp가 실행되고 AutoUpdate가 미쳐 종료되지 않았다면
				//바로 삭제되지 않음.
				try { File.Delete(FullPath); }
				catch { };

				nWaited += 100;
				Thread.Sleep(100);
			}
		}

		/// <summary>
		/// File.Delete는 WildCard를 지원하지 않으므로 만듦.
		/// </summary>
		/// <param name="Folder"></param>
		/// <param name="SearchPattern"></param>
		public static void Delete(string Folder, string SearchPattern, SearchOption SearchOption)
		{
			foreach (string FileCur in Directory.GetFiles(Folder, SearchPattern, SearchOption))
			{
				File.Delete(FileCur);
			}
		}
		public static void Delete(string Folder, string SearchPattern)
		{
			Delete(Folder, SearchPattern, SearchOption.TopDirectoryOnly);
		}

		/// <summary>
		/// Stream 개체를 String 개체로 변경해서 리턴함.
		/// </summary>
		/// <param name="strm">Stream 개체</param>
		/// <returns></returns>
		/// <example>
		/// 다음은 클라이언트에서 업로드된 XML 파일을 문자열 형식으로 바꿔서 엽니다.
		/// <code>
		/// string Xml = CFile.ConvertStreamToString(Request.InputStream);
		/// XDoc.LoadXml(Xml);
		/// </code>
		/// </example>
		public static string ConvertStreamToString(Stream strm, Encoding enc)
		{
			StreamReader reader = new StreamReader(strm, enc);
			return reader.ReadToEnd();

			//byte[] aByte = GetByteFromStream(strm);
			//string Value = GetStringFromByte(aByte, enc);
			//return Value;

			//StringBuilder sb = new StringBuilder();

			//if (strm.Position != 0)
			//    strm.Position = 0;

			//int Len = Convert.ToInt32(strm.Length);
			//byte[] aByt = new byte[Len];
			//strm.Read(aByt, 0, Len);

			//for (int i = 0, i2 = aByt.Length; i < i2; i++)
			//{
			//    sb.Append((char)aByt[i]);
			//}

			//return sb.ToString();
		}
		public static string ConvertStreamToString(Stream strm)
		{
			return ConvertStreamToString(strm, Encoding.UTF8);
		}

		/// <summary>
		/// 바이트 배열을 합침.
		/// </summary>
		/// <example>
		/// byte[] a1 = new byte[] { 32, 32 };
		/// byte[] a2 = new byte[] { 33, 33 };
		/// byte[] aNew = Combine(a1, a2); //32, 32, 33, 33
		/// </example>
		/// <param name="aByteArray"></param>
		/// <returns></returns>
		public static byte[] Combine(params byte[][] aByteArray)
		{
			byte[] aReturn = new byte[aByteArray.Sum(a => a.Length)];

			int Offset = 0;
			foreach (byte[] aByteCur in aByteArray)
			{
				System.Buffer.BlockCopy(aByteCur, 0, aReturn, Offset, aByteCur.Length);
				Offset += aByteCur.Length;
			}

			return aReturn;
		}
		public static byte[] Combine(byte[] aByteSrc, byte[] aByteDest, int LengthDest)
		{
			int LengthSrc = aByteSrc.Length;
			byte[] aReturn = new byte[LengthSrc + LengthDest];

			int Offset = 0;
			System.Buffer.BlockCopy(aByteSrc, 0, aReturn, Offset, LengthSrc);
			Offset += LengthSrc;
			System.Buffer.BlockCopy(aByteDest, 0, aReturn, Offset, LengthDest);

			return aReturn;
		}

		public static byte[] TrimEndNull(byte[] aValue)
		{
			int Length = aValue.Length;
			while ((Length > 0) && (aValue[Length - 1] == 0))
			{
				Length--;
			}

			byte[] aTrimmed = new byte[Length];
			if (Length > 0)
				Array.Copy(aValue, 0, aTrimmed, 0, Length);

			return aTrimmed;
		}


		/// <summary>
		/// 확장자가 브라우저에서 IMG 태그의 SRC 속성에 지정될 수 있는 지 여부를 리턴함.
		/// </summary>
		/// <param name="FileName">파일의 전체 경로</param>
		/// <returns>IMG 태그의 SRC 속성에 지정될 수 있는 지 여부</returns>
		/// <example>
		/// 다음은 특정 파일이 이미지인 경우 IMG 태그를 쓰고, 아니면 A 태그를 쓰는 예입니다.
		/// <code>
		/// <![CDATA[
		/// string Src = "http://www.yourserver.com/test.gif";
		/// 
		/// string Link = "";
		/// if (CFile.IsImageFileAvailableInBrowser(Src))
		/// {
		///	 Link = "<img src='" + Src + "'>";
		/// }
		/// else
		/// {
		///	 Link = "<a href='" + Src + "'>다운로드</a>";
		/// }
		/// 
		/// Console.WriteLine(Link); // "<img src='http://www.yourserver.com/test.gif'>"
		/// ]]>
		/// </code>
		/// </example>
		public static bool IsImageFileAvailableInBrowser(string FileNameOrExtension)
		{
			string Ext = Path.GetExtension(FileNameOrExtension).ToLower();

			string[] aImageExt = CReflection.GetAllEnumDescription(typeof(WebSafeImageExtension));
			return aImageExt.Contains(Ext);
		}

		/// <summary>
		/// 원본 폴더의 모든 파일과 폴더(하위 폴더 포함)를 대상 폴더에 복사함.
		/// </summary>
		/// <param name="FolderSrc">원본 폴더</param>
		/// <param name="FolderDest">대상 폴더</param>
		/// <example>
		/// 다음은 D:\Temp 폴더의 값을 모든 파일, 폴더를 D:\Temp2로 복사함.
		/// 그러나 확장자가 bat인 파일은 복사가 취소됨.
		/// <code>
		/// static void Main(string[] args)
		/// {
		///	 CFile f = new CFile();
		///	 //파일 복사 전에 발생하는 이벤트
		///	 f.BeforeFileCopy += new CFile.BeforeFileCopyEventHandler(f_BeforeFileCopy);
		///	 f.CopyDirectory(@"D:\Temp\log", @"D:\Temp2\log");
		/// }
		///
		/// private static void f_BeforeFileCopy(string FullPathSrc, string FullPathDest, ref bool Cancel)
		/// {
		///	 if (FullPathDest.EndsWith(".bat"))
		///	 {
		///		 //파일의 복사를 취소함.
		///		 Cancel = true;
		///	 }
		/// }
		/// </code>
		/// </example>
		public void CopyDirectory(string FolderSrc, string FolderDest, bool IgnoreCopyError)
		{
			if (this.BeforeDirectoryCopy != null)
			{
				CBeforeDirectoryCopyEventArgs e = new CBeforeDirectoryCopyEventArgs()
				{
					FolderSrc = FolderSrc,
					FolderDest = FolderDest
				};
				this.BeforeDirectoryCopy(this, e);

				if (e.Cancel)
					return;
			}

			string[] aFiles = null;

			if (FolderDest[FolderDest.Length - 1] != Path.DirectorySeparatorChar)
				FolderDest += Path.DirectorySeparatorChar;

			if (!Directory.Exists(FolderDest))
				Directory.CreateDirectory(FolderDest);

			try
			{
				//D:\System Volume Information의 경우는 액세스할 수 없다는 에러가 나므로 무시함.
				aFiles = Directory.GetFileSystemEntries(FolderSrc);
			}
			catch (Exception)
			{
				return;
			}

			foreach (string FullPathSrc in aFiles)
			{
				if (Directory.Exists(FullPathSrc))
				{
					CopyDirectory(FullPathSrc, FolderDest + Path.GetFileName(FullPathSrc), IgnoreCopyError);
				}
				else
				{
					string FullPathDest = FolderDest + Path.GetFileName(FullPathSrc);
					this.CopyFile(FullPathSrc, FullPathDest, IgnoreCopyError);
				}
			}
		}
		public void CopyDirectory(string FolderSrc, string FolderDest)
		{
			bool IgnoreCopyError = false;
			CopyDirectory(FolderSrc, FolderDest, IgnoreCopyError);
		}
		public void CopyFile(string FullPathSrc, string FullPathDest, bool IgnoreCopyError)
		{
			string FullPathSrcNew = "";

			if (this.BeforeFileCopy != null)
			{
				CBeforeFileCopyEventArgs e = new CBeforeFileCopyEventArgs()
				{
					FullPathSrc = FullPathSrc,
					FullPathDest = FullPathDest,
					Cancel = false
				};
				this.BeforeFileCopy(this, e);

				if (e.Cancel)
					return;

				if (!string.IsNullOrEmpty(e.FullPathSrcNew))
					FullPathSrcNew = e.FullPathSrcNew;
			}

			if (this.DownloadProgress != null)
			{
				long FileSize = CFile.GetFileSize(!string.IsNullOrEmpty(FullPathSrcNew) ? FullPathSrcNew : FullPathSrc);

				CDownloadProgressChangedEventArgs arg = new CDownloadProgressChangedEventArgs();
				arg.FullPath = FullPathDest;
				arg.TotalBytesToReceive = FileSize;
				arg.BytesReceived = FileSize;
				arg.ProgressPercentage = 100;
				this.DownloadProgress(this, arg);
			}

			if (IgnoreCopyError)
			{
				try
				{
					File.Copy((!string.IsNullOrEmpty(FullPathSrcNew) ? FullPathSrcNew : FullPathSrc), FullPathDest, true);
				}
				catch (Exception ex)
				{
					if (this.AfterFileCopyFailed != null)
					{
						this.AfterFileCopyFailed(this,
							new CAfterFileCopyFailedEventArgs()
							{
								FullPathSrc = FullPathSrc,
								FullPathSrcNew = FullPathSrcNew,
								FullPathDest = FullPathDest,
								Exception = ex
							});
					}

					return;
				}
			}
			else
			{
				File.Copy((!string.IsNullOrEmpty(FullPathSrcNew) ? FullPathSrcNew : FullPathSrc), FullPathDest, true);
			}

			if (this.AfterFileCopy != null)
			{
				this.AfterFileCopy(this,
					new CAfterFileCopyEventArgs()
					{
						FullPathSrc = FullPathSrc,
						FullPathSrcNew = FullPathSrcNew,
						FullPathDest = FullPathDest
					});
			}
		}
		public void CopyFile(string FullPathSrc, string FullPathDest)
		{
			bool IgnoreCopyError = false;
			CopyFile(FullPathSrc, FullPathDest, IgnoreCopyError);
		}

		/// <summary>
		/// FileInfo 개체가 폴더인 지 여부를 리턴함.
		/// </summary>
		/// <param name="fi">FileInfo 개체</param>
		/// <returns>폴더인 지 여부</returns>
		/// <example>
		/// <code>
		/// FileInfo File = new FileInfo(@"C:\Windows\setup.log");
		/// Console.WriteLine(CFile.GetIsFolder(File)); // False
		/// 
		/// FileInfo Folder = new FileInfo(@"C:\Windows");
		/// Console.WriteLine(CFile.GetIsFolder(Folder)); // True
		/// </code>
		/// </example>
		public static bool GetIsFolder(FileInfo fi)
		{
			return ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory);
		}

		public static string GetUniqueId(string FullPath)
		{
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create("SHA1");

			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(FullPath, FileMode.Open, FileAccess.Read);
			}
			catch (IOException)
			{
				string FullPathTemp = CFile.GetTempFileName();
				File.Copy(FullPath, FullPathTemp, true);
				fileStream = new FileStream(FullPathTemp, FileMode.Open, FileAccess.Read);
			}

			byte[] hashCode = hashAlgorithm.ComputeHash(fileStream);
			fileStream.Close();
			return Convert.ToBase64String(hashCode);
		}

		public static NameValueCollection GetUniqueIdAndFileName(string Folder, string Pattern)
		{
			NameValueCollection nv = new NameValueCollection();

			FileInfo[] afi = CFile.FindFiles(Folder, Pattern, true);
			foreach (FileInfo fi in afi)
			{
				string Id = GetUniqueId(fi.FullName);
				nv.Add(Id, fi.Name);
			}

			return nv;
		}

		/// <summary>
		/// RootFolder와 RootFolder 안의 모든 폴더와 파일을 삭제함.
		/// </summary>
		/// <param name="RootFolder"></param>
		public static void DeleteFolder(string RootFolder)
		{
			if (!Directory.Exists(RootFolder))
				return;

			EmptyFolder(RootFolder);

			try { Directory.Delete(RootFolder); }
			catch (Exception) { }
		}
		/// <summary>
		/// RootFolder 안의 모든 폴더와 파일을 삭제함.
		/// </summary>
		/// <param name="RootFolder"></param>
		public static void EmptyFolder(string RootFolder)
		{
			string[] aFullPath = Directory.GetFileSystemEntries(RootFolder);
			foreach (string FullPath in aFullPath)
			{
				if (Directory.Exists(FullPath))
				{
					EmptyFolder(FullPath);

					//하위 폴더나 파일이 존재하면 에러 나므로 무시함.
					//GetFiles로 비어있는 지 확인하는 것보다 에러 무시하는 것이 빠를 것 같음.
					try { Directory.Delete(FullPath); }
					catch (Exception) { }
				}
				else
				{
					try { File.Delete(FullPath); }
					catch (Exception) { }
				}
			}
		}

		public static void DeleteFolderHasNoFile(string RootFolder)
		{
			if (!Directory.Exists(RootFolder))
				return;

			EmptyFolderHasNoFile(RootFolder);

			if (Directory.GetFileSystemEntries(RootFolder).Length == 0)
				Directory.Delete(RootFolder);
		}
		public static void EmptyFolderHasNoFile(string RootFolder)
		{
			string[] aFolder = Directory.GetDirectories(RootFolder);
			foreach (string Folder in aFolder)
			{
				if (Directory.GetDirectories(Folder).Length != 0)
					EmptyFolderHasNoFile(Folder);

				if (Directory.GetFileSystemEntries(Folder).Length == 0)
					Directory.Delete(Folder);
			}
		}

		/// <summary>
		/// 임시 폴더의 모든 파일을 삭제함. 삭제시 에러나면 무시하고 다음 파일을 삭제함.
		/// </summary>
		/// <example>
		/// DeleteAllTempFile(".hwp");
		/// DeleteAllTempFile(".*");
		/// </example>
		/// <param name="Extension"></param>
		public static void DeleteAllTempFile(string Extension, int DaysBefore)
		{
			if (!Extension.StartsWith("."))
				Extension = "." + Extension;

			DirectoryInfo di = new DirectoryInfo(Path.GetTempPath());
			FileInfo[] afi = di.GetFiles("*" + Extension, SearchOption.AllDirectories);

			foreach (FileInfo fi in afi)
			{
				if ((DaysBefore > 0)
					&& (DateTime.Now.Subtract(fi.LastWriteTime).TotalDays >= DaysBefore))
					continue;

				try { fi.Delete(); }
				catch (Exception) { }
			}
		}

		public byte[] GetByteFromStream(Stream StrmToRead, long StreamSize)
		{
			long BytesReceived = 0;

			byte[] aBytesAll = new byte[0];

			CDownloadProgressChangedEventArgs arg = new CDownloadProgressChangedEventArgs();
			arg.TotalBytesToReceive = StreamSize;

			int BytesToRead = Convert.ToInt32(Math.Min(StreamSize, _4096));

			byte[] aBuffer = new byte[BytesToRead];

			do
			{
				int BytesRead = StrmToRead.Read(aBuffer, 0, BytesToRead);
				//2012-12-12 추가
				if (BytesRead <= 0)
					break;

				BytesReceived += BytesRead;

				if (this.DownloadProgress != null)
				{
					arg.BytesReceived = BytesReceived;
					arg.ProgressPercentage = Math.Min(100, Convert.ToInt32((arg.BytesReceived / (double)arg.TotalBytesToReceive) * 100));
					this.DownloadProgress(this, arg);
				}

				aBytesAll = CFile.Combine(aBytesAll, aBuffer.Take(BytesRead).ToArray());

			} while ((BytesToRead = Convert.ToInt32(Math.Min((StreamSize - BytesReceived), _4096))) > 0);

			return aBytesAll;
		}
		public string GetStringFromStream(Stream StrmToRead, long StreamSize)
		{
			byte[] aByte = GetByteFromStream(StrmToRead, StreamSize);
			return CFile.GetStringFromByte(aByte);
		}

		public long WriteFileFromStream(Stream Strm, string FullPath, long FileSize, DateTime FileLastWriteTime)
		{
			long BytesReceived = 0;

			using (FileStream fs = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
			{
				CDownloadProgressChangedEventArgs arg = new CDownloadProgressChangedEventArgs();
				arg.FullPath = FullPath;
				arg.TotalBytesToReceive = FileSize;

				int BytesToRead = Convert.ToInt32(Math.Min(FileSize, _4096));

				byte[] aBuffer = new byte[BytesToRead];

				do
				{
					int BytesRead = Strm.Read(aBuffer, 0, BytesToRead);
					//2012-12-12 추가
					if (BytesRead <= 0)
						break;

					BytesReceived += BytesRead;

					if (this.DownloadProgress != null)
					{
						arg.BytesReceived = BytesReceived;
						arg.ProgressPercentage = Math.Min(100, Convert.ToInt32((arg.BytesReceived / (double)arg.TotalBytesToReceive) * 100));
						this.DownloadProgress(this, arg);
					}

					fs.Write(aBuffer, 0, BytesRead);
				} while ((BytesToRead = Convert.ToInt32(Math.Min((FileSize - BytesReceived), _4096))) > 0);
			}

			if (FileLastWriteTime != DateTime.MinValue)
			{
				FileInfo fi = new FileInfo(FullPath);
				fi.LastWriteTime = FileLastWriteTime;
			}

			return BytesReceived;
		}
		public long WriteFileFromStream(Stream Strm, string FullPath, long FileSize)
		{
			return WriteFileFromStream(Strm, FullPath, FileSize, DateTime.MinValue);
		}

		public void WriteStreamFromFile(IEnumerable<Stream> aStrm, FileInfo fi)
		{
			//int SourceFileHashCode = fi.GetHashCode();
			long FileSize = fi.Length;

			using (FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
			{
				CDownloadProgressChangedEventArgs arg = new CDownloadProgressChangedEventArgs();
				arg.FullPath = fi.FullName;
				//arg.SourceFileHashCode = SourceFileHashCode;
				arg.TotalBytesToReceive = FileSize;

				long BytesReceived = 0;
				int BytesToRead = Convert.ToInt32(Math.Min(FileSize, _4096));

				byte[] aBuffer = new byte[BytesToRead];

				do
				{
					int BytesRead = fs.Read(aBuffer, 0, BytesToRead);
					//2012-12-12 추가
					if (BytesRead <= 0)
						break;

					BytesReceived += BytesRead;

					foreach (Stream StrmCur in aStrm)
					{
						StrmCur.Write(aBuffer, 0, BytesRead);
					}

					if (this.DownloadProgress != null)
					{
						arg.BytesReceived = BytesReceived;
						arg.ProgressPercentage = Math.Min(100, Convert.ToInt32((arg.BytesReceived / (double)arg.TotalBytesToReceive) * 100));
						this.DownloadProgress(this, arg);
					}

				} while ((BytesToRead = Convert.ToInt32(Math.Min((FileSize - BytesReceived), _4096))) > 0);
			}
		}
		public void WriteStreamFromFile(Stream Strm, FileInfo fi)
		{
			WriteStreamFromFile(new Stream[] { Strm }, fi);
		}

		public void WriteStreamFromStream(Stream StrmToRead, IEnumerable<Stream> aStrmToWrite, long BytesTotal)
		{
			CDownloadProgressChangedEventArgs arg = new CDownloadProgressChangedEventArgs();
			arg.FullPath = "";
			arg.TotalBytesToReceive = BytesTotal;

			int BytesReceived = 0;
			int BytesToRead = Convert.ToInt32(Math.Min(BytesTotal, _4096));

			byte[] aBuffer = new byte[BytesToRead];

			do
			{
				int BytesToWrite = StrmToRead.Read(aBuffer, 0, BytesToRead);

				foreach (Stream StrmToWrite in aStrmToWrite)
				{
					StrmToWrite.Write(aBuffer, 0, BytesToWrite);
				}

				BytesReceived += BytesToRead;

				if (this.DownloadProgress != null)
				{
					arg.BytesReceived = BytesReceived;
					arg.ProgressPercentage = Math.Min(100, Convert.ToInt32((BytesReceived / (double)arg.TotalBytesToReceive) * 100));
					this.DownloadProgress(this, arg);
				}

			} while ((BytesToRead = Convert.ToInt32(Math.Min((BytesTotal - BytesReceived), _4096))) > 0);
		}
		//네트워크로 보낼 때 마지막에 항상 중단되므로 주석.
		//public void WriteStreamFromStream(Stream StrmToRead, IEnumerable<Stream> aStrmToWrite, long FileSize)
		//{
		//    CDownloadProgressChangedEventArgs arg = new CDownloadProgressChangedEventArgs();
		//    arg.FullPath = "";
		//    arg.TotalBytesToReceive = FileSize;

		//    byte[] aBuffer = new byte[_4096];
		//    int BytesRead = 0;
		//    while ((BytesRead = StrmToRead.Read(aBuffer, 0, _4096)) > 0)
		//    {
		//        arg.BytesReceived += BytesRead;
		//        arg.ProgressPercentage = Math.Min(100, Convert.ToInt32((arg.BytesReceived / (double)arg.TotalBytesToReceive) * 100));
		//        if (this.DownloadProgress != null)
		//            this.DownloadProgress(this, arg);

		//        foreach (Stream StrmToWrite in aStrmToWrite)
		//        {
		//            StrmToWrite.Write(aBuffer, 0, BytesRead);
		//        }
		//        //aStrmToWrite.ForEach(StrmToWrite => StrmToWrite.Write(aBuffer, 0, BytesRead));
		//    }
		//}
		public void WriteStreamFromStream(Stream StrmToRead, Stream StrmToWrite, long BytesTotal)
		{
			WriteStreamFromStream(StrmToRead, new Stream[] { StrmToWrite }, BytesTotal);
		}

		/// <summary>
		/// 데이터 많을 때는 한꺼번에 쓰면 수신하는 쪽에서 에러 발생할 수 있으므로 _4096씩 쓰기 위함.
		/// </summary>
		/// <param name="aByteToRead"></param>
		/// <param name="StrmToWrite"></param>
		public static void WriteStreamFromByte(Byte[] aByteToRead, Stream StrmToWrite)
		{
			int BytesTotal = aByteToRead.Length;

			int BytesReceived = 0;
			int BytesToRead = Convert.ToInt32(Math.Min(BytesTotal, _4096));

			byte[] aBuffer = new byte[BytesToRead];

			do
			{
				StrmToWrite.Write(aByteToRead, BytesReceived, BytesToRead);
				BytesReceived += BytesToRead;
			} while ((BytesToRead = Convert.ToInt32(Math.Min((BytesTotal - BytesReceived), _4096))) > 0);

			//StrmToWrite.Write(aByteToRead, 0, aByteToRead.Length);
		}
		public static void WriteStreamFromByte(Byte[] aByteToRead, IEnumerable<Stream> aStrmToWrite)
		{
			for (int i = 0; i < aByteToRead.Length; i++)
			{
				foreach (Stream StrmToWrite in aStrmToWrite)
				{
					StrmToWrite.WriteByte(aByteToRead[i]);
				}
			}
		}

		public static void SplitTextFile(string FullPathSrc, string FolderDest, string NumberFormat, long FileSize, bool IsSplitAfterLineBreak, Encoding enc)
		{
			int nIndex = 0;

			if (!Directory.Exists(FolderDest))
				Directory.CreateDirectory(FolderDest);

			//byte[] buffer = new byte[65536];
			using (Stream strSrc = File.OpenRead(FullPathSrc))
			{
				while (strSrc.Position < strSrc.Length)
				{
					nIndex++;
					// Create a new sub File, and read into t
					string FullPathNew = Path.Combine(FolderDest, Path.GetFileNameWithoutExtension(FullPathSrc));
					FullPathNew += nIndex.ToString(NumberFormat) + Path.GetExtension(FullPathSrc);

					using (StreamWriter strDest = new StreamWriter(FullPathNew, false, enc))
					{
						int Count = (int)FileSize;
						byte[] buffer = new byte[Count];
						int Bytes = strSrc.Read(buffer, 0, Count);

						strDest.Write(enc.GetString(buffer, 0, Bytes));

						if (IsSplitAfterLineBreak)
						{
							List<byte> aByte = new List<byte>();
							int Byte = 0;
							while ((Byte = strSrc.ReadByte()) != -1)
							{
								aByte.Add((byte)Byte);

								if (Byte == '\n')
									break;
							}
							if (aByte.Count > 0)
							{
								strDest.Write(enc.GetString(aByte.ToArray()));
							}
						}
					}
				}
			}
		}
		//public static void SplitTextFile(string FullPathSrc, string FolderDest, string NumberFormat, long FileSize, bool IsSplitAfterLineBreak, Encoding enc)
		//{
		//    int nIndex = 0;

		//    byte[] buffer = new byte[65536];

		//    using (Stream strSrc = File.OpenRead(FullPathSrc))
		//    {
		//        while (strSrc.Position < strSrc.Length)
		//        {
		//            nIndex++;

		//            // Create a new sub File, and read into t
		//            string FullPathNew = Path.Combine(FolderDest, Path.GetFileNameWithoutExtension(FullPathSrc));
		//            FullPathNew += nIndex.ToString(NumberFormat) + Path.GetExtension(FullPathSrc);

		//            using (Stream strDest = File.OpenWrite(FullPathNew))
		//            {
		//                while (strDest.Position < FileSize)
		//                {
		//                    // Work out how many bytes to read
		//                    int bytes = strSrc.Read(buffer, 0, (int)Math.Min(FileSize, buffer.Length));
		//                    strDest.Write(buffer, 0, bytes);

		//                    if (IsSplitAfterLineBreak)
		//                    {
		//                        int Byte = 0;
		//                        while ((Byte != '\n') && (Byte != -1))
		//                        {
		//                            Byte = strSrc.ReadByte();
		//                            strDest.WriteByte((byte)Byte);

		//                            if (Byte != -1)
		//                                bytes++;
		//                        }
		//                    }

		//                    // Are we at the end of the file?
		//                    if (bytes < Math.Min(FileSize, buffer.Length))
		//                    {
		//                        break;
		//                    }
		//                }
		//            }
		//        }
		//    }
		//}

		public static long GetFileSize(string FullPath)
		{
			FileInfo fi = new FileInfo(FullPath);
			if (!fi.Exists)
				return 0;
			else
				return fi.Length;

			//try
			//{
			//    FileInfo f = new FileInfo(FullPath);
			//    return f.Length;
			//}
			//catch (Exception)
			//{
			//}

			//return 0;
		}

		public static DateTime GetLastWriteTime(string FullPath)
		{
			try
			{
				FileInfo f = new FileInfo(FullPath);
				if (!f.Exists)
					return DateTime.MinValue;

				return f.LastWriteTime;
			}
			catch (Exception)
			{
			}

			return DateTime.MinValue;
		}

		public static DateTime GetLastWriteDateOnly(string FullPath)
		{
			try
			{
				FileInfo f = new FileInfo(FullPath);
				return new DateTime(f.LastWriteTime.Year, f.LastWriteTime.Month, f.LastWriteTime.Day);
			}
			catch (Exception)
			{
			}

			return DateTime.MinValue;
		}

		public static IEnumerable<DriveInfo> GetDriveNames(DriveType Type)
		{
			foreach (DriveInfo di in DriveInfo.GetDrives())
			{
				if (di.DriveType == Type)
				{
					yield return di;
				}
			}
		}

		public static string GetHardDiskVolumeSerial(char Drive)
		{
			uint VolumeSerialNumberIs = 0;
			uint maxCompLen = 0;
			StringBuilder VolLabel = new StringBuilder(256); // Label
			UInt32 VolFlags = new UInt32();
			StringBuilder FSName = new StringBuilder(256); // File System Name
			string DriveLetter = Drive.ToString() + ":\\"; // fix up the passed-in drive letter for the API call
			long Ret = GetVolumeInformation(DriveLetter, VolLabel, (UInt32)VolLabel.Capacity, ref VolumeSerialNumberIs, ref maxCompLen, ref VolFlags, FSName, (UInt32)FSName.Capacity);

			return Convert.ToString(VolumeSerialNumberIs);
		}
		public static string GetSystemDrive()
		{
			return Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
		}
		public static string GetSystemHardDiskVolumeSerial()
		{
			char Drive = GetSystemDrive()[0];
			return GetHardDiskVolumeSerial(Drive);
		}

		public static int LineIndexOf(string FullPath, string Find, bool IgnoreCase)
		{
			StringComparison comparisonType = IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
			return LineIndexOf(FullPath, Find, comparisonType);
		}
		public static int LineIndexOf(string FullPath, string Find, StringComparison comparisonType)
		{
			using (TextReader tr = new StreamReader(FullPath))
			{
				string Line = "";
				int nLine = -1;
				while ((Line = tr.ReadLine()) != null)
				{
					nLine++;

					if (Line.IndexOf(Find, comparisonType) != -1)
						return nLine;
				}
			}

			return -1;
		}

		public static FileAttributes RemoveAttribute(FileAttributes Attributes, FileAttributes AttributesToRemove)
		{
			return Attributes & ~AttributesToRemove;
		}

		/// <summary>
		/// File.Move가 실제로는 파일을 삭제한 후 생성해서 시간이 걸리는 줄 알고 만들었으나 파일 날짜가 변경되지 않아 그렇지 않은 걸로 보여 File.Move 씀.
		/// </summary>
		/// <param name="FullPathSrc"></param>
		/// <param name="FullPathDest"></param>
		/// <returns></returns>
		//public static bool Rename(string FullPathSrc, string FullPathDest)
		//{
		//    ShellFileOperation fo = new ShellFileOperation();
		//    fo.SourceFiles = new string[] { FullPathSrc };
		//    fo.DestFiles = new string[] { FullPathDest };
		//    fo.Operation = ShellFileOperation.FileOperations.FO_RENAME;

		//    fo.OperationFlags 
		//        = ShellFileOperation.ShellFileOperationFlags.FOF_NOCONFIRMATION
		//        | ShellFileOperation.ShellFileOperationFlags.FOF_SILENT;

		//    bool Ret = fo.DoOperation();

		//    return Ret;
		//}
	}
}
