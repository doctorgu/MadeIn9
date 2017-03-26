using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.ComponentModel;
#if !DotNet35
using Microsoft.Ajax.Utilities;
using System.Text.RegularExpressions;
#endif

//!!!
//파일 변경, 삭제 등 반영하기

namespace DoctorGu
{
	public class CFtpInfoSync : CFtpInfo
	{
		public string Folder;
	}

	public enum SyncTypes
	{
		[Description("원본이 특정 시간 이후")]
		AfterSrcTime,
		[Description("원본과 대상본의 시간, 크기 비교")]
		CompareTimeBetweenSrcAndDest,
	}

	/// <summary>
	/// 서로 다른 폴더 간의 파일을 동기화하는 기능 구현.
	/// </summary>
	/// <remarks>
	/// 다음의 2가지 기능을 구현함.
	/// 1. 원본 폴더의 파일과 대상 폴더의 파일을 비교해서 수정일자나 파일크기가 틀리면 대상 폴더의 같은 위치에 원본 파일을 복사함.
	/// 2. 실시간으로 원본 폴더의 파일 작업을 감시해서 파일의 생성, 변경, 삭제, 복사, 이동 등이 일어나면 대상 폴더에도 같은 작업을 실행해서 동기화를 유지함.
	/// </remarks>
	/// <example>
	/// 다음은 C:\temp\sync_src 폴더를 기준으로 C:\temp\sync_dest1, C:\temp\sync_dest2 폴더를 동기화시킴.
	/// <code>
	/// //원본 폴더
	/// string RootFolderSrc = "C:\\temp\\sync_src";
	/// //대상 폴더
	/// string[] aRootFolderDest = new string[] { "C:\\temp\\sync_dest1", "C:\\temp\\sync_dest2" };
	/// //복사 기록을 남길 폴더
	/// string LogFolder = "C:\\Temp\\log";
	/// //생성자 생성
	/// CSyncFile bf = new CSyncFile(RootFolderSrc, aRootFolderDest, LogFolder);
	/// //복사에서 제외될 확장자
	/// bf.DisallowedExt = new string[] { ".sln", ".suo", ".config", ".log" };
	/// //복사에서 제외될 폴더
	/// bf.DisallowedFolder = new string[] { "C:\\RECYCLER", "C:\\Temp" };
	/// 
	/// //C:\temp\sync_src 폴더의 전체 파일을 C:\temp\sync_dest1, C:\temp\sync_dest2 폴더로 복사
	/// bf.CopyAll();
	/// 
	/// //실시간 감시해서 파일 동기화함. 
	/// //즉, C:\temp\sync_src 폴더에서 발생한 파일 관련 작업은 C:\temp\sync_dest1, C:\temp\sync_dest2 폴더에 실시간으로 반영됨.
	/// bf.EnableWatcher = true;
	/// //실시간 감시 중지
	/// bf.EnableWatcher = false;
	/// </code>
	/// </example>
	public class CSyncFile
	{
		private string[] _aFullPathSrc;
		private string _RootFolderSrc;

		private CFtpInfoSync[] _aFtpInfoDest;
		private string[] _aRootFolderDest;
		private SyncTypes _SyncType = SyncTypes.CompareTimeBetweenSrcAndDest;
		private bool _MinifyJs;
		private string[] _aFullPathReferencingJs;
		private string[] _aJsFullPathRefered;
		private DateTime _DateTimeAfter = DateTime.Now.AddYears(100);
		private string[] _DisallowedExt = null;
		private string[] _AllowedOnlyExt = null;
		private string[] _DisallowedFolder = null;
		private string[] _AllowedOnlyFolder = null;
		private string _LogFolder = "";
		private CLog _Logging = new CLog();

		private CFile _File = new CFile();
		private CFtp2[] _aFtp = null;

		//private const string _PatternUrlAndVersion = @"<((script)|(link))[^>]+((src)|(href))\s*=\s*[""'](?<Url>[^""']+)\?v=(?<Version>\d+)[""'][^>]*>";
		private const string _PatternUrlSpecific = @"<((script)|(link))[^>]+((src)|(href))\s*=\s*[""'](?<Url>[^""']*{0}[^""']*)[""'][^>]*>";
		private const string _PatternUrl = @"<((script)|(link))[^>]+((src)|(href))\s*=\s*[""'](?<Url>[^""']*)[""'][^>]*>";

		private FileSystemWatcher mWatcher = new FileSystemWatcher();

		private enum LogTypes
		{
			Copy, Delete, Rename,
			FailCopy, FailDelete, FailRename
		}

		private enum DestTypes
		{
			FileSystem,
			Ftp
		}
		private DestTypes _DestType = DestTypes.FileSystem;

		/// <summary>
		/// </summary>
		/// <param name="RootFolderSrc">원본 폴더</param>
		/// <param name="aRootFolderDest">대상 폴더</param>
		/// <param name="LogFolder">복사 기록을 남길 폴더</param>
		public CSyncFile(string RootFolderSrc, string[] aRootFolderDest, SyncTypes SyncType, bool MinifyJs, string[] aFullPathReferencingJs, string[] aJsFullPathRefered, DateTime DateTimeAfter, string LogFolder)
			: this(null, RootFolderSrc, aRootFolderDest, SyncType, MinifyJs, aFullPathReferencingJs, aJsFullPathRefered, DateTimeAfter, LogFolder)
		{
		}
		public CSyncFile(string[] aFullPathSrc, string RootFolderSrc, string[] aRootFolderDest, SyncTypes SyncType,
			bool MinifyJs, string[] aFullPathReferencingJs, string[] aJsFullPathRefered, DateTime DateTimeAfter, string LogFolder)
		{
			this._aFullPathSrc = aFullPathSrc;
			this._RootFolderSrc = RootFolderSrc;
			this._aRootFolderDest = aRootFolderDest;
			this._SyncType = SyncType;
			this._MinifyJs = MinifyJs;

			if (aFullPathReferencingJs.Length > 0)
				this._aFullPathReferencingJs = aFullPathReferencingJs;
			if (aJsFullPathRefered.Length > 0)
				this._aJsFullPathRefered = aJsFullPathRefered;

			this._DateTimeAfter = DateTimeAfter;
			this._LogFolder = LogFolder;

			//{
			this.mWatcher.IncludeSubdirectories = true;
			this.mWatcher.EnableRaisingEvents = this.mEnableWatcher;
			this.mWatcher.NotifyFilter
				= NotifyFilters.DirectoryName
				| NotifyFilters.FileName
				| NotifyFilters.CreationTime
				| NotifyFilters.LastWrite;

			this.mWatcher.Path = RootFolderSrc;

			this.mWatcher.Changed += new FileSystemEventHandler(mWatcher_Changed);
			this.mWatcher.Created += new FileSystemEventHandler(mWatcher_Created);
			this.mWatcher.Renamed += new RenamedEventHandler(mWatcher_Renamed);
			this.mWatcher.Deleted += new FileSystemEventHandler(mWatcher_Deleted);
			//}

			_File.BeforeDirectoryCopy += new EventHandler<CBeforeDirectoryCopyEventArgs>(_File_BeforeDirectoryCopy);
			_File.BeforeFileCopy += new EventHandler<CBeforeFileCopyEventArgs>(f_BeforeFileCopy);
			_File.AfterFileCopy += new EventHandler<CAfterFileCopyEventArgs>(f_AfterFileCopy);

			_DestType = DestTypes.FileSystem;
		}
		public CSyncFile(string RootFolderSrc, CFtpInfoSync[] aFtpInfo, SyncTypes SyncType, bool MinifyJs, string[] aFullPathReferencingJs, string[] aJsFullPathRefered, DateTime DateTimeAfter, string LogFolder)
			: this(null, RootFolderSrc, aFtpInfo, SyncType, MinifyJs, aFullPathReferencingJs, aJsFullPathRefered, DateTimeAfter, LogFolder)
		{
		}
		public CSyncFile(string[] aFullPathSrc, string RootFolderSrc, CFtpInfoSync[] aFtpInfo, SyncTypes SyncType,
			bool MinifyJs, string[] aFullPathReferencingJs, string[] aJsFullPathRefered, DateTime DateTimeAfter, string LogFolder)
		{
			this._aFullPathSrc = aFullPathSrc;

			this._RootFolderSrc = RootFolderSrc;
			this._aFtpInfoDest = aFtpInfo;
			this._SyncType = SyncType;
			this._MinifyJs = MinifyJs;

			if (aFullPathReferencingJs.Length > 0)
				this._aFullPathReferencingJs = aFullPathReferencingJs;
			if (aJsFullPathRefered.Length > 0)
				this._aJsFullPathRefered = aJsFullPathRefered;

			this._DateTimeAfter = DateTimeAfter;
			this._LogFolder = LogFolder;

			//{
			this.mWatcher.IncludeSubdirectories = true;
			this.mWatcher.EnableRaisingEvents = this.mEnableWatcher;
			this.mWatcher.NotifyFilter
				= NotifyFilters.DirectoryName
				| NotifyFilters.FileName
				| NotifyFilters.CreationTime
				| NotifyFilters.LastWrite;

			this.mWatcher.Path = RootFolderSrc;

			this.mWatcher.Changed += new FileSystemEventHandler(mWatcher_Changed);
			this.mWatcher.Created += new FileSystemEventHandler(mWatcher_Created);
			this.mWatcher.Renamed += new RenamedEventHandler(mWatcher_Renamed);
			this.mWatcher.Deleted += new FileSystemEventHandler(mWatcher_Deleted);
			//}

			_aFtp = new CFtp2[aFtpInfo.Length];
			for (int i = 0; i < _aFtp.Length; i++)
			{
				_aFtp[i] = new CFtp2(aFtpInfo[i]);
				_aFtp[i].BeforeDirectoryUpload += new EventHandler<CBeforeDirectoryUploadEventArgs>(ftp_BeforeDirectoryUpload);
				_aFtp[i].BeforeFileUpload += new EventHandler<CBeforeFileUploadEventArgs>(ftp_BeforeFileUpload);
				_aFtp[i].AfterFileUpload += new EventHandler<CAfterFileUploadEventArgs>(ftp_AfterFileUpload);
				_aFtp[i].FileUploadFailed += new EventHandler<CFileUploadFailedEventArgs>(ftp_FileUploadFailed);
			}

			_DestType = DestTypes.Ftp;
		}

		void mWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.FullPath);
			if ((fiSrc.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
			{
				return;
			}

			//Changed 이벤트가 2번씩 일어나므로 실제로 복사 작업도 2번 하게 되는 문제
			//있으나 해결 못함.
			Copy(fiSrc);
		}
		void mWatcher_Created(object sender, FileSystemEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.FullPath);
			Copy(fiSrc);
		}
		void mWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.FullPath);
			Rename(e.OldFullPath, fiSrc);
		}
		void mWatcher_Deleted(object sender, FileSystemEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.FullPath);
			Delete(fiSrc);
		}

		/// <summary>
		/// 설정된 조건에 따라 원본 폴더의 모든 파일을 대상 폴더에 복사함.
		/// </summary>
		public void CopyAll()
		{
			string FolderSrc = this._RootFolderSrc;

			if (_DestType == DestTypes.FileSystem)
			{
				List<string> aDirectoryChecked = new List<string>();

				for (int i = 0, i2 = this._aRootFolderDest.Length; i < i2; i++)
				{
					if (this._aFullPathSrc != null)
					{
						for (int j = 0; j < this._aFullPathSrc.Length; j++)
						{
							string FullPathSrc = this._aFullPathSrc[j];
							string FullPathDest = CPath.GetFullPathDest(FolderSrc, this._aRootFolderDest[i], FullPathSrc);

							string FolderDest = CPath.GetFolderName(FullPathDest);
							if (aDirectoryChecked.IndexOf(FolderDest) == -1)
							{
								if (!Directory.Exists(FolderDest))
									Directory.CreateDirectory(FolderDest);

								aDirectoryChecked.Add(FolderDest);
							}

							_File.CopyFile(FullPathSrc, FullPathDest);
						}
					}
					else
					{
						_File.CopyDirectory(FolderSrc, this._aRootFolderDest[i]);
					}
				}
			}
			else if (_DestType == DestTypes.Ftp)
			{
				for (int i = 0, i2 = this._aFtpInfoDest.Length; i < i2; i++)
				{
					CFtp2 FtpCur = _aFtp[i];
					CFtpInfoSync InfoSync = (CFtpInfoSync)FtpCur.Info;

					List<string> aDirectoryChecked = new List<string>();

					if (this._aFullPathSrc != null)
					{
						for (int j = 0; j < this._aFullPathSrc.Length; j++)
						{
							string FullPathSrc = this._aFullPathSrc[j];
							string FullPathDest = CUrl.GetFullUrlDest(FolderSrc, InfoSync.Folder, FullPathSrc);

							string FolderDest = CUrl.GetDirectoryName(FullPathDest);
							if (aDirectoryChecked.IndexOf(FolderDest) == -1)
							{
								if (!FtpCur.DirectoryExists(FolderDest))
									FtpCur.CreateDirectory(FolderDest);

								aDirectoryChecked.Add(FolderDest);
							}

							FtpCur.UploadFile(FullPathSrc, FullPathDest);
						}
					}
					else
					{
						FtpCur.UploadDirectory(FolderSrc, InfoSync.Folder);
					}
				}
			}
		}

		private void ftp_BeforeDirectoryUpload(object sender, CBeforeDirectoryUploadEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.LocalFolder);
			//FTP에 있는 파일은 원래 파일 날짜가 아닌 업로드된 파일의 날짜 정보를 가지므로
			//날짜를 비교할 수 없으므로 원본파일의 날짜만 기준이 되므로 null로 설정함.
			FileInfo fiDest = null;

			string FullPathSrcNewIs;

			if (!IsValidForSync(fiSrc, fiDest, true, out FullPathSrcNewIs))
				e.Cancel = true;
		}
		private void ftp_BeforeFileUpload(object sender, CBeforeFileUploadEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.LocalFullPath);
			//FTP에 있는 파일은 원래 파일 날짜가 아닌 업로드된 파일의 날짜 정보를 가지므로
			//날짜를 비교할 수 없으므로 원본파일의 날짜만 기준이 되므로 null로 설정함.
			FileInfo fiDest = null;

			string FullPathSrcNewIs;

			if (!IsValidForSync(fiSrc, fiDest, true, out FullPathSrcNewIs))
				e.Cancel = true;

			if (!string.IsNullOrEmpty(FullPathSrcNewIs))
				e.LocalFullPathNew = FullPathSrcNewIs;
		}
		private void ftp_AfterFileUpload(object sender, CAfterFileUploadEventArgs e)
		{
			this.mCountSucceeded++;
			WriteLog(LogTypes.Copy, e);
		}
		private void ftp_FileUploadFailed(object sender, CFileUploadFailedEventArgs e)
		{
			this.mCountFailed++;
			WriteLog(LogTypes.Copy, e);
		}

		private void _File_BeforeDirectoryCopy(object sender, CBeforeDirectoryCopyEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.FolderSrc);
			FileInfo fiDest = (File.Exists(e.FolderDest) ? new FileInfo(e.FolderDest) : null);

			string FullPathSrcNewIs;

			if (!IsValidForSync(fiSrc, fiDest, false, out FullPathSrcNewIs))
				e.Cancel = true;
		}

		private void f_BeforeFileCopy(object sender, CBeforeFileCopyEventArgs e)
		{
			FileInfo fiSrc = new FileInfo(e.FullPathSrc);
			FileInfo fiDest = (File.Exists(e.FullPathDest) ? new FileInfo(e.FullPathDest) : null);

			if (fiDest != null)
			{
				//Remove ReadOnly attribute from destination to block error when copying.
				if ((fiDest.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					FileAttributes AttrRemoved = CFile.RemoveAttribute(fiDest.Attributes, FileAttributes.ReadOnly);
					fiDest.Attributes = AttrRemoved;
				}
			}

			string FullPathSrcNewIs;
			if (!IsValidForSync(fiSrc, fiDest, false, out FullPathSrcNewIs))
				e.Cancel = true;

			if (!string.IsNullOrEmpty(FullPathSrcNewIs))
				e.FullPathSrcNew = FullPathSrcNewIs;
		}
		private void f_AfterFileCopy(object sender, CAfterFileCopyEventArgs e)
		{
			this.mCountSucceeded++;
			WriteLog(LogTypes.Copy, e);
		}

		//!!!
		//현재는 쓰이지 않으나 쓰게 되면 CopyAll의 것으로 수정하고 Copy, CopyAll을 공통으로 쓰게 해야 함.
		private void Copy(FileInfo fiSrc)
		{
			bool IsFile = !CFile.GetIsFolder(fiSrc);

			if (_DestType == DestTypes.FileSystem)
			{
				for (int i = 0, i2 = this._aRootFolderDest.Length; i < i2; i++)
				{
					string FullPathDest = this._aRootFolderDest[i] + fiSrc.FullName.Substring(this._RootFolderSrc.Length);

					if (IsFile)
						_File.CopyFile(fiSrc.FullName, FullPathDest);
					else
						_File.CopyDirectory(fiSrc.FullName, FullPathDest);
				}
			}
			else if (_DestType == DestTypes.Ftp)
			{
				for (int i = 0, i2 = this._aFtpInfoDest.Length; i < i2; i++)
				{
					CFtpInfoSync InfoSync = (CFtpInfoSync)_aFtp[i].Info;

					if (IsFile)
					{
						string FullPathSrc = fiSrc.FullName;
						string FullPathDest = CPath.GetFullPathDest(fiSrc.DirectoryName, InfoSync.Folder, FullPathSrc, '/');
						//FullPathDest의 폴더가 없다면 에러 나나 속도 때문에 무시함.
						_aFtp[i].UploadFile(FullPathSrc, FullPathDest);
					}
					else
					{
						_aFtp[i].UploadDirectory(fiSrc.DirectoryName, InfoSync.Folder);
					}
				}
			}
		}

		private void Delete(FileInfo fiSrc)
		{
			for (int i = 0, i2 = this._aRootFolderDest.Length; i < i2; i++)
			{
				string FullPathDest = this._aRootFolderDest[i] + fiSrc.FullName.Substring(this._RootFolderSrc.Length);

				try
				{
					if (File.Exists(FullPathDest))
					{
						File.Delete(FullPathDest);
					}
					else
					{
						Directory.Delete(FullPathDest, true);
					}

					WriteLog(LogTypes.Delete, fiSrc.FullName, "", FullPathDest, null);
				}
				catch (Exception ex)
				{
					WriteLog(LogTypes.FailDelete, fiSrc.FullName, "", FullPathDest, ex);
				}
			}
		}

		private void Rename(string FullPathSrcOld, FileInfo fiSrc)
		{
			bool IsFile = !CFile.GetIsFolder(fiSrc);

			for (int i = 0, i2 = this._aRootFolderDest.Length; i < i2; i++)
			{
				string FullPathDestOld = this._aRootFolderDest[i] + FullPathSrcOld.Substring(this._RootFolderSrc.Length);
				FileInfo fiDestOld = null;
				if (IsFile)
				{
					if (File.Exists(FullPathDestOld))
					{
						fiDestOld = new FileInfo(FullPathDestOld);
					}
				}
				else
				{
					if (Directory.Exists(FullPathDestOld))
					{
						fiDestOld = new FileInfo(FullPathDestOld);
					}
				}

				if (IsFile)
				{
					if (!IsValidExtension(fiSrc))
						continue;
				}
				else
				{
					if (!IsValidFolder(fiSrc))
						continue;
				}

				string FullPathDestNew = this._aRootFolderDest[i] + fiSrc.FullName.Substring(this._RootFolderSrc.Length);

				if (fiDestOld == null)
				{
					try
					{
						string Folder = "";
						if (IsFile)
							Folder = Path.GetDirectoryName(FullPathDestNew);
						else
							Folder = FullPathDestNew;

						if (!Directory.Exists(Folder))
						{
							Directory.CreateDirectory(Folder);
						}

						if (IsFile)
						{
							File.Copy(fiSrc.FullName, FullPathDestNew, true);
						}
						else
						{
							CFile f = new CFile();
							f.BeforeFileCopy += new EventHandler<CBeforeFileCopyEventArgs>(f_BeforeFileCopy);
							f.CopyDirectory(fiSrc.FullName, FullPathDestNew);
						}

						WriteLog(LogTypes.Copy, fiSrc.FullName, "", FullPathDestNew, null);
					}
					catch (Exception ex)
					{
						WriteLog(LogTypes.FailCopy, fiSrc.FullName, "", FullPathDestNew, ex);
					}
				}
				else
				{
					try
					{
						fiDestOld.MoveTo(FullPathDestNew);

						WriteLog(LogTypes.Rename, FullPathDestOld, "", FullPathDestNew, null);
					}
					catch (Exception ex)
					{
						WriteLog(LogTypes.FailRename, FullPathDestOld, "", FullPathDestNew, ex);
					}
				}
			}
		}

		private bool IsValidForSync(FileInfo fiSrc, FileInfo fiDest, bool IsFtp, out string FullPathSrcNewIs)
		{
			FullPathSrcNewIs = "";

			bool IsFile = !CFile.GetIsFolder(fiSrc);

			bool IsValid = true;

			if (IsFile)
			{
				if (IsValid)
					IsValid = IsValidExtension(fiSrc);

				if (IsFtp)
				{
					if (_SyncType == SyncTypes.CompareTimeBetweenSrcAndDest)
						throw new Exception(string.Format("SyncType:{0} disallowed in Ftp.", _SyncType));

					//숨김 속성인 경우 new FileStream 사용할 때 읽지 못함.
					if (IsValid)
						IsValid = ((fiSrc.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden);

					if (IsValid)
						IsValid = (fiSrc.LastWriteTime > _DateTimeAfter);
				}
				else
				{
					if (_SyncType == SyncTypes.CompareTimeBetweenSrcAndDest)
					{
						if (fiDest != null)
						{
							DateTime DateSrc = fiSrc.LastWriteTime;
							DateTime DateDest = fiDest.LastWriteTime;

							long SizeSrc = fiSrc.Length;
							long SizeDest = fiDest.Length;

							if (IsValid)
								IsValid = ((DateSrc != DateDest) || (SizeSrc != SizeDest));
						}
					}
					else
					{
						if (IsValid)
							IsValid = (fiSrc.LastWriteTime > _DateTimeAfter);
					}
				}
			}
			else
			{
				if (IsValid)
					IsValid = IsValidFolder(fiSrc);
			}

			if (IsValid)
			{
				if (this._MinifyJs && (string.Compare(fiSrc.Extension, ".js", true) == 0))
				{
#if !DotNet35
					string JsSource = CFile.GetTextInFile(fiSrc.FullName);

					Minifier mf = new Minifier();
					string JsSourceMinified = mf.MinifyJavaScript(JsSource);

					FullPathSrcNewIs = CFile.GetTempFileName(fiSrc.Extension);
					CFile.WriteTextToFile(FullPathSrcNewIs, JsSourceMinified);
#else
					throw new Exception(".Net 3.5 does not support Minifier.");
#endif
				}

				if ((this._aJsFullPathRefered != null) && (this._aJsFullPathRefered.Length > 0))
				{
					if (CArray.IndexOf(this._aJsFullPathRefered, fiSrc.FullName, true) != -1)
					{
						List<Tuple<string, string, CFtpInfoSync>> tpPathAndHtml = GetPathAndHtmlAndFtpInfoDest(this._aFullPathReferencingJs, fiSrc);

						foreach (Tuple<string, string, CFtpInfoSync> tp in tpPathAndHtml)
						{
							string FullPathDest = tp.Item1;
							string Html = tp.Item2;
							CFtpInfoSync FtpInfo = tp.Item3;

							if (_DestType == DestTypes.FileSystem)
							{
								CFile.WriteTextToFile(FullPathDest, Html);
							}
							else
							{
								string TmpFullPath = CFile.GetTempFileName();
								CFile.WriteTextToFile(TmpFullPath, Html);

								CFtp2 Ftp = new CFtp2(FtpInfo);
								Ftp.UploadFile(TmpFullPath, FullPathDest);
							}
						}
					}
					else if (CArray.IndexOf(this._aFullPathReferencingJs, fiSrc.FullName, true) != -1)
					{
						for (int i = 0; i < this._aRootFolderDest.Length; i++)
						{
							string HtmlSrc = CFile.GetTextInFile(fiSrc.FullName);

							string HtmlDest = "";

							string FullPathDest = "";
							string FullUrlDest = "";
							if (_DestType == DestTypes.FileSystem)
							{
								FullPathDest = CPath.GetFullPathDest(this._RootFolderSrc, this._aRootFolderDest[i], fiSrc.FullName);
								HtmlDest = CFile.GetTextInFile(FullPathDest);
							}
							else
							{
								CFtp2 Ftp = new CFtp2(this._aFtpInfoDest[i]);
								FullUrlDest = CUrl.GetFullUrlDest(this._RootFolderSrc, this._aRootFolderDest[i], fiSrc.FullName);
								HtmlDest = Ftp.GetText(FullUrlDest);
							}

							string HtmlSrcNew = GetHtmlVersionReplaced(HtmlSrc, HtmlDest);
							if (string.IsNullOrEmpty(HtmlSrcNew))
								continue;


							FullPathSrcNewIs = CFile.GetTempFileName(fiSrc.Extension);
							CFile.WriteTextToFile(FullPathSrcNewIs, HtmlSrcNew);
						}
					}
				}
			}

			return IsValid;
		}

		private string GetHtmlVersionReplaced(string HtmlSrc, string HtmlDest)
		{
			Dictionary<string, string> dicUrlAndVer = new Dictionary<string, string>();

			Regex rDest = new Regex(_PatternUrl, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
			foreach (Match m in rDest.Matches(HtmlDest))
			{
				string Url = m.Groups["Url"].Value;
				CQueryString qs = new CQueryString(Url);
				if (string.IsNullOrEmpty(qs["v"]))
					continue;

				dicUrlAndVer.Add(qs.PathOnly, qs["v"]);
			}


			StringBuilder sbHtml = new StringBuilder();

			List<string> aUrl = new List<string>();

			bool IsFound = false;
			Regex rSrc = new Regex(_PatternUrl, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
			foreach (CMatchInfo mi in CRegex.GetMatchResult(rSrc, HtmlSrc))
			{
				sbHtml.Append(mi.ValueBeforeMatch);

				if (mi.Match == null)
					break;

				string Url = mi.Match.Groups["Url"].Value;

				CQueryString qs = new CQueryString(Url);
				string VersionIs;
				dicUrlAndVer.TryGetValue(qs.PathOnly, out VersionIs);

				if (string.IsNullOrEmpty(VersionIs))
				{
					sbHtml.Append(mi.Match.Value);
				}
				else
				{
					IsFound = true;
					sbHtml.Append(mi.Match.Value.Replace(mi.Match.Groups["Url"].Value, qs.PathOnly + "?v=" + VersionIs));
				}
			}

			if (!IsFound)
				return "";


			return sbHtml.ToString();
		}

		private List<Tuple<string, string, CFtpInfoSync>> GetPathAndHtmlAndFtpInfoDest(string[] aFullPathReferencingJs, FileInfo fiSrc)
		{
			List<Tuple<string, string, CFtpInfoSync>> tpPathAndHtml = new List<Tuple<string, string, CFtpInfoSync>>();

			if (aFullPathReferencingJs.Length == 0)
				return tpPathAndHtml;


			List<Tuple<string, string, CFtpInfoSync>> tpFullPathReferencingDest = GetFullPathReferencingDest(aFullPathReferencingJs);
			if (tpFullPathReferencingDest.Count == 0)
				return tpPathAndHtml;


			foreach (Tuple<string, string, CFtpInfoSync> kv in tpFullPathReferencingDest)
			{
				string FullPathSrc = kv.Item1;
				string FullPathDest = kv.Item2;
				CFtpInfoSync FtpInfo = kv.Item3;

				string Html = "";
				if (_DestType == DestTypes.FileSystem)
				{
					if (!File.Exists(FullPathDest))
						continue;

					Html = CFile.GetTextInFile(FullPathDest);
				}
				else if (_DestType == DestTypes.Ftp)
				{
					CFtp2 Ftp = new CFtp2(FtpInfo);
					if (!Ftp.FileExists(FullPathDest))
						continue;

					string TmpFullPath = CFile.GetTempFileName();
					Ftp.DownloadFile(TmpFullPath, FullPathDest);
					Html = CFile.GetTextInFile(TmpFullPath);
				}

				List<string> aHtmlNew = new List<string>();
				string Pattern = string.Format(_PatternUrlSpecific, fiSrc.Name.Replace(".", "\\."));
				Regex r = new Regex(Pattern, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);

				bool IsFound = false;
				foreach (CMatchInfo mi in CRegex.GetMatchResult(r, Html))
				{
					aHtmlNew.Add(mi.ValueBeforeMatch);
					Match m = mi.Match;
					if (m == null)
						break;

					string Url = m.Groups["Url"].Value;
					CQueryString qs = new CQueryString(Url);
					//Commented because v parameter can be setted to empty when referencing file uploaded alone.
					//When that situation, v parameter will get value of 1 again.
					//qs["v"] = (CFindRep.IfNotNumberThen0(qs["v"]) + 1).ToString();
					qs["v"] = DateTime.Now.ToString(CConst.Format_yyyyMMddHHmmss);
					aHtmlNew.Add(m.Value.Replace(m.Groups["Url"].Value, qs.PathAndQuery));
					IsFound = true;
				}
				if (IsFound)
				{
					string HtmlNew = string.Join("", aHtmlNew);

					tpPathAndHtml.Add(new Tuple<string, string, CFtpInfoSync>(FullPathDest, HtmlNew, FtpInfo));
				}
			}

			return tpPathAndHtml;
		}
		private List<Tuple<string, string, CFtpInfoSync>> GetFullPathReferencingDest(string[] aFullPathReferencing)
		{
			List<Tuple<string, string, CFtpInfoSync>> tpFullPathDest = new List<Tuple<string, string, CFtpInfoSync>>();

			foreach (string FullPath in aFullPathReferencing)
			{
				for (int i = 0; i < this._aRootFolderDest.Length; i++)
				{
					if (_DestType == DestTypes.FileSystem)
						tpFullPathDest.Add(new Tuple<string, string, CFtpInfoSync>(FullPath, CPath.GetFullPathDest(this._RootFolderSrc, this._aRootFolderDest[i], FullPath), null));
					else
						tpFullPathDest.Add(new Tuple<string, string, CFtpInfoSync>(FullPath, CUrl.GetFullUrlDest(this._RootFolderSrc, this._aRootFolderDest[i], FullPath), this._aFtpInfoDest[i]));
				}
			}

			return tpFullPathDest;
		}

		private bool IsValidFolder(FileInfo fi)
		{
			if (!CFile.GetIsFolder(fi))
				throw new Exception(fi.FullName + "은 폴더가 아닌 파일입니다.");


			bool HasDisallowedFolder = ((this._DisallowedFolder != null) && (this._DisallowedFolder.Length > 0));
			bool HasAllowedOnlyFolder = ((this._AllowedOnlyFolder != null) && (this._AllowedOnlyFolder.Length > 0));

			if (!HasDisallowedFolder && !HasAllowedOnlyFolder)
				return true;

			if (HasDisallowedFolder && HasAllowedOnlyFolder)
				throw new Exception("IsDisallowedFolder와 IsAllowedOnlyFolder는 둘 중 하나만 값이 있어야 합니다.");


			string Folder = fi.FullName;

			if (HasDisallowedFolder)
			{
				for (int i = 0, i2 = this._DisallowedFolder.Length; i < i2; i++)
				{
					if (Folder.StartsWith(this._DisallowedFolder[i], StringComparison.CurrentCultureIgnoreCase))
					{
						return false;
					}
				}

				return true;
			}
			else
			{
				for (int i = 0, i2 = this._AllowedOnlyFolder.Length; i < i2; i++)
				{
					if (Folder.StartsWith(this._AllowedOnlyFolder[i], StringComparison.CurrentCultureIgnoreCase))
					{
						return true;
					}
				}

				return false;
			}
		}
		private bool IsValidExtension(FileInfo fi)
		{
			if (CFile.GetIsFolder(fi))
				return true;

			bool HasDisallowedExt = ((this._DisallowedExt != null) && (this._DisallowedExt.Length > 0));
			bool HasAllowedOnlyExt = ((this._AllowedOnlyExt != null) && (this._AllowedOnlyExt.Length > 0));

			if (!HasDisallowedExt && !HasAllowedOnlyExt)
				return true;

			if (HasDisallowedExt && HasAllowedOnlyExt)
				throw new Exception("IsDisallowedExt와 IsAllowedOnlyExt는 둘 중 하나만 값이 있어야 합니다.");


			string Ext = Path.GetExtension(fi.FullName);

			if (HasDisallowedExt)
			{
				if (string.IsNullOrEmpty(Ext))
				{
					return true;
				}

				return (CArray.IndexOf(this._DisallowedExt, Ext, true) == -1);
			}
			else if (HasAllowedOnlyExt)
			{
				if (string.IsNullOrEmpty(Ext))
				{
					return false;
				}

				return (CArray.IndexOf(this._AllowedOnlyExt, Ext, true) != -1);
			}

			return false;
		}

		private void WriteLog(LogTypes LogType, string FullPathSrc, string FullPathSrcNew, string FullPathDest, Exception ex)
		{
			string FullPath = Path.Combine(this._LogFolder, LogType.ToString() + DateTime.Now.ToString("yyyyMMdd") + ".log");
			string Log = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
						+ "\t" + FullPathSrc
						+ (!string.IsNullOrEmpty(FullPathSrcNew) ? "(" + FullPathSrcNew + ")" : "")
						+ "\t" + FullPathDest
						+ "\t" + ((ex != null) ? CInfo.GetExceptionText(ex) : "");
			_Logging.WriteLog(FullPath, Log);
		}
		private void WriteLog(LogTypes LogType, CAfterFileCopyEventArgs e)
		{
			WriteLog(LogType, e.FullPathSrc, e.FullPathSrcNew, e.FullPathDest, null);
		}
		private void WriteLog(LogTypes LogType, CAfterFileUploadEventArgs e)
		{
			WriteLog(LogType, e.LocalFullPath, e.LocalFullPathNew, e.RemoteFullUrl, null);
		}
		private void WriteLog(LogTypes LogType, CFileUploadFailedEventArgs e)
		{
			WriteLog(LogType, e.LocalFullPath, e.LocalFullPathNew, e.RemoteFullUrl, e.ex);
		}

		/// <summary>
		/// 복사에서 제외될 확장자
		/// </summary>
		public string[] DisallowedExt
		{
			set
			{
				string[] aExt = new string[value.Length];
				for (int i = 0, i2 = value.Length; i < i2; i++)
				{
					if (!value[i].StartsWith("."))
					{
						aExt[i] = "." + value[i];
					}
					else
					{
						aExt[i] = value[i];
					}
				}

				this._DisallowedExt = aExt;
			}
			get
			{
				return this._DisallowedExt;
			}
		}

		/// <summary>
		/// 복사에서 포함할 확장자
		/// </summary>
		public string[] AllowedOnlyExt
		{
			set
			{
				string[] aExt = new string[value.Length];
				for (int i = 0, i2 = value.Length; i < i2; i++)
				{
					if (!value[i].StartsWith("."))
					{
						aExt[i] = "." + value[i];
					}
					else
					{
						aExt[i] = value[i];
					}
				}

				this._AllowedOnlyExt = aExt;
			}
			get
			{
				return this._AllowedOnlyExt;
			}
		}

		/// <summary>
		/// 복사에서 제외될 폴더
		/// </summary>
		public string[] DisallowedFolder
		{
			set
			{
				string[] aFolder = new string[value.Length];
				for (int i = 0, i2 = value.Length; i < i2; i++)
				{
					if (value[i].EndsWith("\\"))
					{
						aFolder[i] = value[i].Substring(0, value[i].Length - 1);
					}
					else
					{
						aFolder[i] = value[i];
					}
				}

				this._DisallowedFolder = aFolder;
			}
			get
			{
				return this._DisallowedFolder;
			}
		}

		/// <summary>
		/// 유일하게 복사할 폴더
		/// </summary>
		public string[] AllowedOnlyFolder
		{
			set
			{
				string[] aFolder = new string[value.Length];
				for (int i = 0, i2 = value.Length; i < i2; i++)
				{
					if (value[i].EndsWith("\\"))
					{
						aFolder[i] = value[i].Substring(0, value[i].Length - 1);
					}
					else
					{
						aFolder[i] = value[i];
					}
				}

				this._AllowedOnlyFolder = aFolder;
			}
			get
			{
				return this._AllowedOnlyFolder;
			}
		}

		private bool mEnableWatcher = false;
		/// <summary>
		/// 파일이 변경되면 실시간으로 동기화를 실행할 지 여부를 설정하거나 리턴함.
		/// </summary>
		public bool EnableWatcher
		{
			get { return this.mEnableWatcher; }
			set
			{
				this.mWatcher.EnableRaisingEvents = value;
			}
		}

		private int mCountSucceeded;
		public int CountSucceeded
		{
			get { return this.mCountSucceeded; }
		}

		private int mCountFailed;
		public int CountFailed
		{
			get { return this.mCountFailed; }
		}
	}
}
