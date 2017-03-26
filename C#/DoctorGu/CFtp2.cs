using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace DoctorGu
{
	public enum FtpServerType
	{
		SFtp,
		Ftp
	}
	public class CFtpInfo
	{
		public FtpServerType ServerType;
		public string Host;
		public int Port;
		public string UserId;
		public string Password;
		public bool UsePassive;
		//상속 받는 클래스에서 사용하게 했으므로 주석.
		//public string Folder;
	}

	public class CBeforeDirectoryUploadEventArgs : EventArgs
	{
		public string LocalFolder;
		public bool Cancel;
	}

	public class CBeforeFileUploadEventArgs : EventArgs
	{
		public string LocalFullPath;
		public string LocalFullPathNew;
		public bool Cancel;
	}

	public class CAfterFileUploadEventArgs : EventArgs
	{
		public string LocalFullPath;
		public string LocalFullPathNew;
		public string RemoteFullUrl;
	}
	
	public class CFileUploadFailedEventArgs : EventArgs
	{
		public string LocalFullPath;
		public string LocalFullPathNew;
		public string RemoteFullUrl;
		public Exception ex;
	}

	/// <summary>
	/// FTP와 관련된 기능 구현.
	/// CFtp 클래스는 Win32 API를 사용한 버전이며, CFtp2 클래스는 .Net Framework만을 이용한 버전임.
	/// </summary>
	/// <remarks>
	/// 다음과 같은 방법을 쓰면 happymoney 사용자가 로그인할 때 happymoney 폴더가 기본이 되어야 하는 데 그렇게 안됨.<br/>
	/// FtpReq.Credentials = new NetworkCredential(this.mUserId, this.mPassword);<br/>
	/// 그러므로 Url 안에 id와 password를 입력하는 방법을 사용함.
	/// </remarks>
	/// <example>
	/// 다음은 FTP 서버에 폴더의 생성, 폴더의 삭제, 텍스트 파일을 업로드, 다운로드, 삭제하는 과정을 보여줍니다.
	/// <code>
	/// //생성자
	/// CFtp2 ftp = new CFtp2(false, "yourftpurl.com", 0, "yourid", "yourpassword");
	///
	/// //루트에 test 폴더 생성
	/// ftp.CreateDirectory("/test");
	///
	/// //"FTP 테스트" 문자열을 가진 텍스트 파일 생성
	/// string Value = "FTP 테스트";
	/// string FullPath = CFile.GetTempFileName();
	/// string FileName = Path.GetFileName(FullPath);
	/// CFile.WriteTextToFile(FullPath, Value);
	///
	/// //텍스트 파일을 업로드
	/// ftp.UploadFile(FullPath, "/test/" + FileName);
	///
	/// //업로드된 텍스트 파일의 내용을 가져옴.
	/// string Value2 = ftp.GetText("/test/" + FileName);
	///
	/// //업로드할 때 사용된 텍스트와 다운로드된 텍스트를 비교
	/// Console.WriteLine(Value); //FTP 테스트
	/// Console.WriteLine(Value2); //FTP 테스트
	///
	/// //업로드된 텍스트 파일 삭제
	/// ftp.DeleteFile("/test/" + FileName);
	///
	/// //루트에서 생성된 test 폴더 삭제
	/// ftp.RemoveDirectory("/test");
	/// </code>
	/// </example>
	public class CFtp2
	{
		//Uri u = new Uri("ftp://user:password@testinter.net:24/Test/Test.aspx?a=1234");
		//AbsolutePath: /Test/Test.aspx%3Fa=1234
		//AbsoluteUri: ftp://user:password@testinter.net:24/Test/Test.aspx%3Fa=1234
		//Authority: testinter.net:24
		//DnsSafeHost: testinter.net
		//Host: testinter.net
		//LocalPath: /Test/Test.aspx?a=1234
		//OriginalString: ftp://user:password@testinter.net:24/Test/Test.aspx?a=1234
		//PathAndQuery: /Test/Test.aspx%3Fa=1234
		//Port: 24
		//Scheme: ftp
		//Segments[0]: /
		//Segments[1]: Test/
		//Segments[2]: Test.aspx%3Fa=1234
		//UserInfo: user:password

		private CFtpInfo _Info;

		public event EventHandler<CBeforeDirectoryUploadEventArgs> BeforeDirectoryUpload;
		public event EventHandler<CBeforeFileUploadEventArgs> BeforeFileUpload;
		public event EventHandler<CAfterFileUploadEventArgs> AfterFileUpload;
		public event EventHandler<CFileUploadFailedEventArgs> FileUploadFailed;

		/// <summary>
		/// 생성자
		/// </summary>
		/// <param name="UsePassive">Passive 모드 적용 여부</param>
		/// <param name="Url">FTP 주소</param>
		/// <param name="Port">포트(0이면 FTP 기본 포트인 21번 포트가 사용됨.)</param>
		/// <param name="UserId">사용자 아이디</param>
		/// <param name="Password">비밀번호</param>
		public CFtp2(CFtpInfo Info)
		{
			_Info = Info;
		}

		public CFtp2(string Url, FtpServerType ServerType, bool UsePassive)
		{
			Uri u = new Uri(Url);
			
			CFtpInfo finfo = new CFtpInfo();
			finfo.ServerType = ServerType;
			finfo.UsePassive = UsePassive;

			finfo.Host = u.Host;
			finfo.Port = u.Port;

			if (!string.IsNullOrEmpty(u.UserInfo))
			{
				string[] aUserPwd = u.UserInfo.Split(':');
				finfo.UserId = aUserPwd[0];
				
				if (aUserPwd.Length > 1)
				{
					finfo.Password = aUserPwd[1];
				}
			}

			_Info = finfo;
		}

		public CFtpInfo Info
		{
			get { return _Info; }
		}

		public string Url
		{
			get
			{
				string UrlNew = "ftp://";
				//NetworkCredencial 사용하므로 주석.
				//if (!string.IsNullOrEmpty(Info.UserId))
				//{
				//	UrlNew += Info.UserId + ":" + Info.Password + "@";
				//}

				UrlNew += _Info.Host;

				if ((_Info.Port != 0) && (_Info.Port != CConst.Port21Ftp))
				{
					UrlNew += ":" + _Info.Port.ToString();
				}

				return UrlNew;
			}
		}

		/// <summary>
		/// FTP 서버의 파일을 삭제함.
		/// </summary>
		/// <param name="RemoteFolder">서버의 삭제할 폴더 위치</param>
		/// <param name="RemoteFile">서버의 삭제할 파일 이름</param>
		public void DeleteFile(string RemoteFolder, string RemoteFile)
		{
			Uri RemoteUri = GetRemoteUri(RemoteFolder, RemoteFile);

			FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
			if (!string.IsNullOrEmpty(this._Info.UserId))
			{
				FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
			}
			FtpReq.UsePassive = this._Info.UsePassive;

			FtpReq.Method = WebRequestMethods.Ftp.DeleteFile;

			FtpWebResponse FtpResp = (FtpWebResponse)FtpReq.GetResponse();
			FtpResp.Close();
		}
		/// <summary>
		/// FTP 서버의 파일을 삭제함.
		/// </summary>
		/// <param name="RemoteFullPath">서버에 있는 삭제할 파일의 전체 경로</param>
		public void DeleteFile(string RemoteFullPath)
		{
			string Folder = CPath.GetFolderName(RemoteFullPath, '/');
			string File = CPath.GetFileName(RemoteFullPath, '/');

			DeleteFile(Folder, File);
		}

		/// <summary>
		/// FTP 서버에 있는 파일을 클라이언트 컴퓨터로 다운로드함.
		/// </summary>
		/// <param name="LocalFullPath">다운로드할 파일의 클라이언트 전체 경로</param>
		/// <param name="RemoteFolder">서버의 폴더 위치</param>
		/// <param name="RemoteFile">서버의 파일 이름</param>
		public void DownloadFile(string LocalFullPath, string RemoteFolder, string RemoteFile)
		{
			Uri RemoteUri = GetRemoteUri(RemoteFolder, RemoteFile);

			FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
			if (!string.IsNullOrEmpty(this._Info.UserId))
			{
				FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
			}
			FtpReq.UsePassive = this._Info.UsePassive;
			FtpReq.Method = WebRequestMethods.Ftp.DownloadFile;

			FtpWebResponse FtpResp = (FtpWebResponse)FtpReq.GetResponse();

			Stream readStream = FtpResp.GetResponseStream();
			FileStream writeStream = new FileStream(LocalFullPath, FileMode.Create, FileAccess.Write);

			int Length = 256;
			Byte[] buffer = new Byte[Length];
			int bytesRead = readStream.Read(buffer, 0, Length);

			while (bytesRead > 0)
			{
				writeStream.Write(buffer, 0, bytesRead);
				bytesRead = readStream.Read(buffer, 0, Length);
			}

			readStream.Close();
			writeStream.Close();
			FtpResp.Close();
		}
		/// <summary>
		/// FTP 서버에 있는 파일을 클라이언트 컴퓨터로 다운로드함. 
		/// </summary>
		/// <param name="LocalFullPath">다운로드할 파일의 클라이언트 전체 경로</param>
		/// <param name="RemoteFullPath">서버에 있는 파일의 전체 경로</param>
		public void DownloadFile(string LocalFullPath, string RemoteFullPath)
		{
			string Folder = CPath.GetFolderName(RemoteFullPath, '/');
			string File = CPath.GetFileName(RemoteFullPath, '/');

			DownloadFile(LocalFullPath, Folder, File);
		}

		/// <summary>
		/// FTP 서버에 있는 텍스트 파일의 내용을 리턴함.
		/// </summary>
		/// <param name="RemoteFolder">서버의 폴더 위치</param>
		/// <param name="RemoteFile">서버의 텍스트 파일 이름</param>
		/// <returns>텍스트 파일의 내용</returns>
		public string GetText(string RemoteFolder, string RemoteFile)
		{
			Uri RemoteUri = GetRemoteUri(RemoteFolder, RemoteFile);

			FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
			if (!string.IsNullOrEmpty(this._Info.UserId))
			{
				FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
			}
			FtpReq.UsePassive = this._Info.UsePassive;

			FtpReq.Method = WebRequestMethods.Ftp.DownloadFile;

			FtpWebResponse FtpResp = (FtpWebResponse)FtpReq.GetResponse();

			StreamReader sr = new StreamReader(FtpResp.GetResponseStream(), System.Text.Encoding.UTF8);
			string s = sr.ReadToEnd();

			sr.Close();
			FtpResp.Close();

			return s;
		}
		/// <summary>
		/// FTP 서버에 있는 텍스트 파일의 내용을 리턴함.
		/// </summary>
		/// <param name="RemoteFullPath">서버에 있는 텍스트 파일의 전체 경로</param>
		/// <returns>텍스트 파일의 내용</returns>
		public string GetText(string RemoteFullPath)
		{
			string Folder = CPath.GetFolderName(RemoteFullPath, '/');
			string File = CPath.GetFileName(RemoteFullPath, '/');

			return GetText(Folder, File);
		}

		/// <summary>
		/// FTP 서버에 클라이언트 컴퓨터의 파일을 업로드함.
		/// </summary>
		/// <param name="LocalFullPath">클라이언트 컴퓨터의 파일 전체 경로</param>
		/// <param name="RemoteFolder">서버의 폴더 위치</param>
		/// <param name="RemoteFile">서버의 파일 이름</param>
		public void UploadFile(string LocalFullPath, string RemoteFolder, string RemoteFile)
		{
			string LocalFullPathNew = "";

			if (this.BeforeFileUpload != null)
			{
				CBeforeFileUploadEventArgs e = new CBeforeFileUploadEventArgs()
				{
					LocalFullPath = LocalFullPath
				};
				this.BeforeFileUpload(this, e);

				if (e.Cancel)
					return;

				if (!string.IsNullOrEmpty(e.LocalFullPathNew))
					LocalFullPathNew = e.LocalFullPathNew;
			}

			try
			{
				Uri RemoteUri = GetRemoteUri(RemoteFolder, RemoteFile);

				FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
				if (!string.IsNullOrEmpty(this._Info.UserId))
				{
					FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
				}
				FtpReq.UsePassive = this._Info.UsePassive;

				FtpReq.Method = WebRequestMethods.Ftp.UploadFile;

				using (FileStream readStream = new FileStream((!string.IsNullOrEmpty(LocalFullPathNew) ? LocalFullPathNew : LocalFullPath), FileMode.Open, FileAccess.Read))
				{
					using (Stream writeStream = FtpReq.GetRequestStream())
					{
						int Length = 256;
						Byte[] buffer = new Byte[Length];
						int bytesRead = readStream.Read(buffer, 0, Length);

						while (bytesRead > 0)
						{
							writeStream.Write(buffer, 0, bytesRead);
							bytesRead = readStream.Read(buffer, 0, Length);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (this.FileUploadFailed != null)
				{
					CFileUploadFailedEventArgs e = new CFileUploadFailedEventArgs()
					{
						LocalFullPath = LocalFullPath,
						LocalFullPathNew = LocalFullPathNew,
						RemoteFullUrl = CPath.CombineUrl(RemoteFolder, RemoteFile),
						ex = ex
					};
					this.FileUploadFailed(this, e);
					return;
				}
				else
				{
					throw new Exception(ex.Message, ex);
				}
			}

			if (this.AfterFileUpload != null)
			{
				CAfterFileUploadEventArgs e = new CAfterFileUploadEventArgs()
				{
					LocalFullPath = LocalFullPath,
					LocalFullPathNew = LocalFullPathNew,
					RemoteFullUrl = CPath.CombineUrl(RemoteFolder, RemoteFile)
				};
				this.AfterFileUpload(this, e);
			}
		}
		/// <summary>
		/// FTP 서버에 클라이언트 컴퓨터의 파일을 업로드함.
		/// </summary>
		/// <param name="LocalFullPath">클라이언트 컴퓨터의 파일 전체 경로</param>
		/// <param name="RemoteFullPath">서버의 파일 전체 경로</param>
		public void UploadFile(string LocalFullPath, string RemoteFullUrl)
		{
			string Folder = CPath.GetFolderName(RemoteFullUrl, '/');
			string File = CPath.GetFileName(RemoteFullUrl, '/');

			this.UploadFile(LocalFullPath, Folder, File);
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
		///	 f.BeforeFileUpload += new CFile.BeforeFileUploadEventHandler(f_BeforeFileUpload);
		///	 f.CopyDirectory(@"D:\Temp\log", @"D:\Temp2\log");
		/// }
		///
		/// private static void f_BeforeFileUpload(string FullPathSrc, ref bool Cancel)
		/// {
		///	 if (FullPathDest.EndsWith(".bat"))
		///	 {
		///		 //파일의 복사를 취소함.
		///		 Cancel = true;
		///	 }
		/// }
		/// </code>
		/// </example>
		public void UploadDirectory(string FolderLocal, string FolderRemote)
		{
			if (this.BeforeDirectoryUpload != null)
			{
				CBeforeDirectoryUploadEventArgs e = new CBeforeDirectoryUploadEventArgs()
				{
					LocalFolder = FolderLocal
				};
				this.BeforeDirectoryUpload(this, e);

				if (e.Cancel)
					return;
			}

			string[] aFiles = null;

			if (!FolderRemote.EndsWith("/"))
				FolderRemote += '/';

			try
			{
				//D:\System Volume Information의 경우는 액세스할 수 없다는 에러가 나므로 무시함.
				aFiles = Directory.GetFileSystemEntries(FolderLocal);
			}
			catch (Exception)
			{
				return;
			}

			bool IsDirectoryChecked = false;
			foreach (string LocalFullPath in aFiles)
			{
				if (Directory.Exists(LocalFullPath))
				{
					this.UploadDirectory(LocalFullPath, FolderRemote + Path.GetFileName(LocalFullPath));
				}
				else
				{
					if (!IsDirectoryChecked)
					{
						if (!this.DirectoryExists(FolderRemote))
							this.CreateDirectory(FolderRemote);

						IsDirectoryChecked = true;
					}

					string RemoteFullUrl = FolderRemote + Path.GetFileName(LocalFullPath);
					this.UploadFile(LocalFullPath, RemoteFullUrl);
				}
			}
		}

		/// <summary>
		/// FTP 서버에 메모리 상에 있는 Stream을 업로드함.
		/// </summary>
		/// <param name="str">Stream 개체</param>
		/// <param name="RemoteFolder">서버의 폴더 위치</param>
		/// <param name="RemoteFile">서버의 파일 이름</param>
		public void UploadStream(Stream str, string RemoteFolder, string RemoteFile)
		{
			Uri RemoteUri = GetRemoteUri(RemoteFolder, RemoteFile);

			FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
			if (!string.IsNullOrEmpty(this._Info.UserId))
			{
				FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
			}
			FtpReq.UsePassive = this._Info.UsePassive;

			FtpReq.Method = WebRequestMethods.Ftp.UploadFile;

			Stream writeStream = FtpReq.GetRequestStream();

			byte[] aByte = CFile.GetByteFromStream(str);
			writeStream.Write(aByte, 0, aByte.Length);

			writeStream.Close();

			return;
		}
		/// <summary>
		/// FTP 서버에 메모리 상에 있는 Stream을 업로드함.
		/// </summary>
		/// <param name="str">Stream 개체</param>
		/// <param name="RemoteFullPath">서버에 있는 파일의 전체 경로</param>
		public void UploadStream(Stream str, string RemoteFullPath)
		{
			string Folder = CPath.GetFolderName(RemoteFullPath, '/');
			string File = CPath.GetFileName(RemoteFullPath, '/');
			UploadStream(str, Folder, File);
		}

		/// <summary>
		/// FTP 서버의 특정 폴더를 만듦.
		/// </summary>
		/// <param name="FolderName">만들 폴더 위치</param>
		public void CreateDirectory(string FolderName)
		{
			string[] aPath = CPath.GetAllPathInArray(FolderName, '/', StringSplitOptions.RemoveEmptyEntries);
			string PathList = "";
			for (int i = 0, i2 = aPath.Length; i < i2; i++)
			{
				PathList += aPath[i] + "/";
				if (!this.DirectoryExists(PathList))
				{
					Uri RemoteUri = GetRemoteUri(PathList);

					FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
					if (!string.IsNullOrEmpty(this._Info.UserId))
					{
						FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
					}
					FtpReq.UsePassive = this._Info.UsePassive;

					FtpReq.Method = WebRequestMethods.Ftp.MakeDirectory;
					FtpWebResponse FtpResp = (FtpWebResponse)FtpReq.GetResponse();
					FtpResp.Close();
				}
			}
		}

		/// <summary>
		/// FTP 서버의 특정 폴더를 삭제함. 폴더 안에 파일이 존재하면 삭제되지 않음.
		/// </summary>
		/// <param name="FolderName">삭제할 폴더 위치</param>
		public void RemoveDirectory(string FolderName)
		{
			Uri RemoteUri = GetRemoteUri(FolderName);

			FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
			if (!string.IsNullOrEmpty(this._Info.UserId))
			{
				FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
			}
			FtpReq.UsePassive = this._Info.UsePassive;

			FtpReq.Method = WebRequestMethods.Ftp.RemoveDirectory;
			FtpWebResponse FtpResp = (FtpWebResponse)FtpReq.GetResponse();
			FtpResp.Close();
		}

		/// <summary>
		/// <paramref name="RemoteFolder"/> 폴더가 존재하는 지 여부를 리턴함.
		/// </summary>
		/// <param name="RemoteFolder">폴더</param>
		/// <returns><paramref name="RemoteFolder"/> 폴더가 존재하는 지 여부</returns>
		public bool DirectoryExists(string RemoteFolder)
		{
			string[] aFile = this.GetFileNames(RemoteFolder);
			if (aFile == null)
				return false;

			if ((aFile.Length == 1) && (aFile[0] == CPath.GetFileName(RemoteFolder, '/')))
				return false;

			return true;
		}
		/// <summary>
		/// <paramref name="RemoteFullPath"/> 파일이 존재하는 지 여부를 리턴함.
		/// </summary>
		/// <param name="RemoteFullPath">폴더</param>
		/// <returns><paramref name="RemoteFullPath"/> 파일이 존재하는 지 여부</returns>
		public bool FileExists(string RemoteFullPath)
		{
			string[] aFile = this.GetFileNames(RemoteFullPath);
			if ((aFile.Length == 1) && (aFile[0] == CPath.GetFileName(RemoteFullPath, '/')))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// <paramref name="RemoteFolder"/> 안의 모든 파일과 폴더의 이름을 리턴함.
		/// </summary>
		/// <param name="RemoteFolder">부모 폴더</param>
		/// <returns>
		/// 배열 형식의 모든 파일과 폴더의 이름.
		/// <paramref name="RemoteFolder"/> 폴더 안에 파일이 없다면 0 길이의 배열을 리턴함.
		/// <paramref name="RemoteFolder"/>가 존재하지 않으면 null을 리턴함.
		/// <paramref name="RemoteFolder"/>가 폴더가 아닌 파일의 전체 경로이면 해당 파일 이름을 리턴함.
		/// </returns>
		private string[] GetFileNames(string RemoteFolder)
		{
			string[] aFile = null;

			Uri RemoteUri = GetRemoteUri(RemoteFolder);
			FtpWebRequest FtpReq = (FtpWebRequest)WebRequest.Create(RemoteUri);
			if (!string.IsNullOrEmpty(this._Info.UserId))
			{
				FtpReq.Credentials = new NetworkCredential(this._Info.UserId, this._Info.Password);
			}
			FtpReq.UsePassive = this._Info.UsePassive;
			

			FtpReq.Method = WebRequestMethods.Ftp.ListDirectory;

			//여기서 에러가 났다면 "/f1/f2"와 같이 썼는 데 f1 폴더도 없고 f2 폴더도 없는 경우임.
			WebResponse response = null;
			try { response = FtpReq.GetResponse(); }
			catch (WebException)
			{
				return null;
			}

			using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
			{
				string FileList = "";
				try { FileList = reader.ReadToEnd(); }
				catch (Exception) { return aFile; }

				if (FileList != null)
				{
					aFile = FileList.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				}
			}

			return aFile;
		}

		private Uri GetRemoteUri(string FolderName, string FileName)
		{
			string Url = this.Url;
			if (!string.IsNullOrEmpty(FolderName))
			{
				Url = CPath.CombineUrl(Url, FolderName.Trim('/'));
				if (!Url.EndsWith("/"))
					Url = Url + "/";
			}
			if (!string.IsNullOrEmpty(FileName))
			{
				Url = CPath.CombineUrl(Url, FileName);
			}

			//C# 폴더가 C 폴더로 만들어지는 것을 방지
			Url = CWeb.EncodeUrlForWebRequest(Url);

			Uri RemoteUri = new Uri(Url);

			return RemoteUri;
		}
		private Uri GetRemoteUri(string FolderName)
		{
			return GetRemoteUri(FolderName, "");
		}
	}
}
