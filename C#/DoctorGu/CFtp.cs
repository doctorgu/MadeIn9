using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace DoctorGu
{
	/// <summary>
	/// FTP와 관련된 기능 구현.
	/// CFtp 클래스는 Win32 API를 사용한 버전이며, CFtp2 클래스는 .Net Framework만을 이용한 버전임.
	/// </summary>
	/// <example>
	/// CFtp ftp = new CFtp(true);
	/// ftp.Connect("doctorgu.ddns.co.kr", "ftponly", "ftponly");
	/// ftp.PutFile("C:\\Test.txt", "Test.txt");
	/// ftp.Disconnect();
	/// </example>
	public class CFtp : IDisposable
	{
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern int InternetOpen(string sAgent, int lAccessType, string sProxyName, string sProxyBypass, int lFlags);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern int InternetCloseHandle(int hInet);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern int InternetConnect(int hInternetSession, string sServerName, int nServerPort, string sUsername, string sPassword, int lService, int lFlags, int lContext);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool InternetGetLastResponseInfo(IntPtr lpdwError, StringBuilder lpszErrorBuffer, int lpdwErrorBufferLength);

		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpGetFile(int hFtpSession, string lpszRemoteFile, string lpszNewFile, Boolean fFailIfExists, int dwFlagsAndAttributes, int dwFlags, int dwContext);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpPutFile(int hFtpSession, string lpszLocalFile, string lpszRemoteFile, int dwFlags, int dwContext);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpDeleteFile(int hFtpSession, string lpszFileName);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpRenameFile(int hFtpSession, string lpszOldName, string lpszNewName);

		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpGetCurrentDirectory(int hFtpSession, StringBuilder lpszCurrentDirectory, int lpdwCurrentDirectory);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpSetCurrentDirectory(int hFtpSession, string lpszDirectory);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpCreateDirectory(int hFtpSession, string lpszName);
		[DllImport("wininet.dll", SetLastError = true)]
		private static extern bool FtpRemoveDirectory(int hFtpSession, string lpszName);

		private const int ERROR_NO_MORE_FILES = 18;
		private const int INTERNET_OPEN_TYPE_PRECONFIG = 0;
		private const int INTERNET_INVALID_PORT_NUMBER = 0;
		private const int INTERNET_SERVICE_FTP = 1;

		private const int INTERNET_FLAG_RELOAD = -2147483648;

		private const int FILE_ATTRIBUTE_READONLY = 1;
		private const int FILE_ATTRIBUTE_HIDDEN = 2;
		private const int FILE_ATTRIBUTE_SYSTEM = 4;
		private const int FILE_ATTRIBUTE_DIRECTORY = 16;
		private const int FILE_ATTRIBUTE_ARCHIVE = 32;
		private const int FILE_ATTRIBUTE_NORMAL = 128;
		private const int FILE_ATTRIBUTE_TEMPORARY = 256;
		private const int FILE_ATTRIBUTE_COMPRESSED = 2048;
		private const int FILE_ATTRIBUTE_OFFLINE = 4096;

		private const int FTP_TRANSFER_TYPE_BINARY = 2;
		private const int FTP_TRANSFER_TYPE_ASCII = 1;

		private const int INTERNET_FLAG_ACTIVE = 0;
		private const int INTERNET_FLAG_PASSIVE = 134217728;

		//private const int BUFFERSIZE = 255;

		private int hOpen = 0;
		private int hConnection = 0;
		//private int hFile = 0;

		private int mTransferMode;
		private int mConnectionMode;

		/// <summary>
		/// 생성자
		/// </summary>
		/// <param name="IsPassive">Passive 모드를 설정할 지 여부</param>
		public CFtp(bool IsPassive)
		{
			hOpen = InternetOpen("FTP Client", INTERNET_OPEN_TYPE_PRECONFIG, "", "", 0);
			if (hOpen == 0)
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
			
			mTransferMode = FTP_TRANSFER_TYPE_ASCII;
			mConnectionMode = IsPassive ? INTERNET_FLAG_PASSIVE : INTERNET_FLAG_ACTIVE;
			hConnection = 0;
		}
		/// <summary>
		/// 개체가 삭제될 때 GC에서 호출됨.
		/// </summary>
		public void Dispose()
		{
			InternetCloseHandle(hOpen);
		}

		/// <summary>
		/// FTP 서버에 연결함.
		/// </summary>
		/// <param name="sServer">서버 주소</param>
		/// <param name="Port">FTP 포트</param>
		/// <param name="sUser">아이디</param>
		/// <param name="sPassword">패스워드</param>
		public void Connect(string sServer, int Port, string sUser, string sPassword)
		{
			if (hConnection != 0)
			{
				InternetCloseHandle(hConnection);
			}
			
			if (Port == 0) Port = CConst.Port21Ftp;
			
			hConnection = InternetConnect(hOpen, sServer, Port, sUser, sPassword, INTERNET_SERVICE_FTP, mConnectionMode, 0);
			if (hConnection == 0)
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
		}
		/// <summary>
		/// FTP 서버에 연결함.
		/// </summary>
		/// <param name="sServer">서버 주소</param>
		/// <param name="sUser">아이디</param>
		/// <param name="sPassword">패스워드</param>
		public void Connect(string sServer, string sUser, string sPassword)
		{
			Connect(sServer, 0, sUser, sPassword);
		}
		/// <summary>
		/// FTP 서버에 연결함.
		/// </summary>
		/// <param name="sServer">서버 주소</param>
		/// <param name="Port">FTP 포트</param>
		public void Connect(string sServer, int Port)
		{
			Connect(sServer, Port, "", "");
		}
		/// <summary>
		/// FTP 서버에 연결함.
		/// </summary>
		/// <param name="sServer">서버 주소</param>
		public void Connect(string sServer)
		{
			Connect(sServer, 0, "", "");
		}

		/// <summary>
		/// FTP 서버와 연결을 끊음.
		/// </summary>
		public void Disconnect()
		{
			if (hConnection != 0)
			{
				InternetCloseHandle(hConnection);
			}
			
			hConnection = 0;
		}

		/// <summary>
		/// FTP 서버의 파일을 다운 받음.
		/// </summary>
		/// <param name="sLocal">다운 받을 경로</param>
		/// <param name="sRemote">가져올 서버의 경로</param>
		public void GetFile(string sLocal, string sRemote)
		{
			//Local caching을 막으려면 dwFlags에 INTERNET_FLAG_NO_CACHE_WRITE(0x04000000)를 추가할 것.
			if (!FtpGetFile(hConnection, sRemote, sLocal, false, FILE_ATTRIBUTE_NORMAL, mTransferMode + INTERNET_FLAG_RELOAD, 0))
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
		}
		/// <summary>
		/// FTP 서버의 텍스트 파일의 내용을 리턴함.
		/// </summary>
		/// <param name="sRemote">서버에 있는 텍스트 파일의 경로</param>
		/// <returns>텍스트 파일의 내용</returns>
		public string GetText(string sRemote)
		{
			string TempPathFile = CFile.GetTempFileName();
			GetFile(TempPathFile, sRemote);

			StreamReader sr = new StreamReader(TempPathFile, Encoding.UTF8);
			string s = sr.ReadToEnd();
			sr.Close();

			return s;
		}

		/// <summary>
		/// 파일을 업로드함.
		/// </summary>
		/// <param name="sLocal">업로드할 파일의 경로</param>
		/// <param name="sRemote">업로드될 서버의 경로</param>
		public void PutFile(string sLocal, string sRemote)
		{
			if (!FtpPutFile(hConnection, sLocal, sRemote, mTransferMode, 0))
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
		}

		/// <summary>
		/// 서버의 파일을 삭제함.
		/// </summary>
		/// <param name="sRemote">삭제할 파일이 있는 서버의 경로</param>
		public void DeleteFile(string sRemote)
		{
			if (!FtpDeleteFile(hConnection, sRemote))
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
		}

		/// <summary>
		/// 서버의 파일 이름을 변경함.
		/// </summary>
		/// <param name="sExisting">원래 이름</param>
		/// <param name="sNewName">새 이름</param>
		public void RenameFile(string sExisting, string sNewName)
		{
			if (!FtpRenameFile(hConnection, sExisting, sNewName))
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
		}

		/// <summary>
		/// 현재 경로를 설정하거나 리턴함.
		/// </summary>
		public string CurrentDirectory
		{
			get
			{
				StringBuilder szDir = new StringBuilder(1024);
				if (!FtpGetCurrentDirectory(hConnection, szDir, szDir.Capacity))
				{
					throw new Exception(CUtil.GetLastWin32ErrorInfo());
				}

				return szDir.ToString();
			}

			set
			{
				if (!FtpSetCurrentDirectory(hConnection, (string)value))
				{
					throw new Exception(CUtil.GetLastWin32ErrorInfo());
				}
			}
		}

		/// <summary>
		/// 서버에 폴더를 만듦.
		/// </summary>
		/// <param name="sDirectory">만들어질 폴더 이름</param>
		public void CreateDirectory(string sDirectory)
		{
			if (!FtpCreateDirectory(hConnection, sDirectory))
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
		}
		/// <summary>
		/// 서버에 있는 폴더를 삭제함.
		/// </summary>
		/// <param name="sDirectory">삭제할 폴더 이름</param>
		public void RemoveDirectory(string sDirectory)
		{
			if (!FtpRemoveDirectory(hConnection, sDirectory))
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}
		}
	}
}
