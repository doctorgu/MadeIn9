using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DoctorGu;
using System.Timers;
using System.Reflection;
using System.IO;
using DoctorGu.Encryption;

namespace DoBackup
{
	public partial class SvcBackup : ServiceBase
	{
		private Timer tmr;
		//private string mAppPath = CInfo.GetAppFolder(Assembly.GetExecutingAssembly());
		private string mAppPath = CAssembly.GetFolder();
		private bool mRunning;

		public SvcBackup()
		{
			InitializeComponent();

			this.tmr = new Timer();
			
			CXmlConfig xc = GetXmlConfig();
			//60 = 1분
			int Interval = CFindRep.IfNotNumberThen(xc.GetSetting("IntervalMinutes", 60), 60);
			this.tmr.Interval = Interval * 1000;
			//this.tmr.Interval = 15000;
			this.tmr.Enabled = true;
			this.tmr.Elapsed += new ElapsedEventHandler(tmr_Elapsed);

			//처음엔 무조건 실행
			//tmr_Elapsed(this.tmr, null);
		}

		void tmr_Elapsed(object sender, ElapsedEventArgs e)
		{
			const char Delim = '─';
			
			if (this.mRunning)
				return;
			else
				this.mRunning = true;

			string LogFullPath = GetLogFullPath();
			string LogFolderForSync = Path.GetDirectoryName(LogFullPath);
			try
			{
				CXmlConfig xc = GetXmlConfig();

				string[] aRootPathSrc = xc.GetSetting("RootPathSrc", "").Split(Delim);

				string[] aFtpHost = xc.GetSetting("FtpHost", "").Split(Delim);
				string[] aFtpId = xc.GetSetting("FtpId", "").Split(Delim);

				string[] aFtpPassword = xc.GetSetting("FtpPassword", "").Split(Delim);
				for (int i = 0; i < aFtpPassword.Length; i++)
				{
					aFtpPassword[i] = CEncrypt.DecryptPassword(aFtpPassword[i]);
				}

				string[] aFtpFolder = xc.GetSetting("FtpFolder", "").Split(Delim);

				string[] aSyncType = xc.GetSetting("SyncType", "").Split(Delim);

				string[] aMinifyJs = xc.GetSetting("MinifyJs", "").Split(Delim);

				string[] aFileNameToAppendParam = xc.GetSetting("FileNameToAppendParam", "").Split(Delim);

				string[] asDateTimeAfter = xc.GetSetting("DateTimeAfter", "").Split(Delim);
				DateTime[] aDateTimeAfter = new DateTime[asDateTimeAfter.Length];
				for (int i = 0; i < asDateTimeAfter.Length; i++)
				{
					aDateTimeAfter[i] = CFindRep.IfNotDateTimeThen19000101(asDateTimeAfter[i]);
				}


				for (int i = 0; i < aRootPathSrc.Length; i++)
				{
					CFtpInfoSync[] aFtpInfo = new CFtpInfoSync[]
					{
						new CFtpInfoSync()
						{
							Host = aFtpHost[i],
							UserId = aFtpId[i],
							Password = aFtpPassword[i],
							Folder = aFtpFolder[i]
						}
					};

					CSyncFile sf = new CSyncFile(aRootPathSrc[i], aFtpInfo, CEnum.GetValueByName<SyncTypes>(aSyncType[i]), (aMinifyJs[i] == "1"), aFileNameToAppendParam, aDateTimeAfter[i], LogFolderForSync);
					sf.DisallowedFolder = new string[] { LogFolderForSync };
					sf.CopyAll();

					aDateTimeAfter[i] = DateTime.Now;
				}


				for (int i = 0; i < aDateTimeAfter.Length; i++)
				{
					asDateTimeAfter[i] = aDateTimeAfter[i].ToString(CConst.Format_yyyy_MM_dd_HH_mm_ss);
				}

				xc.SaveSetting("DateTimeAfter", string.Join(Delim.ToString(), asDateTimeAfter));
			}
			catch (Exception ex)
			{
				CFile.AppendTextToFile(LogFullPath, "Error " + DateTime.Now.ToString()
					+ "\r\n" + ex.Message
					+ "\r\n" + ex.StackTrace
					+ "\r\n" + ex.Source);
			}

			this.mRunning = false;
		}

		protected override void OnStart(string[] args)                                                             
		{
			string FullPath = GetLogFullPath();
			CFile.AppendTextToFile(FullPath, "Start " + DateTime.Now.ToString());
		}

		protected override void OnStop()
		{
			string FullPath = GetLogFullPath();
			CFile.AppendTextToFile(FullPath, "Stop " + DateTime.Now.ToString());
		}

		private string GetLogFullPath()
		{
			return this.mAppPath + "\\DoBackup.log";
		}

		private CXmlConfig GetXmlConfig()
		{
			return new CXmlConfig(this.mAppPath + "\\Config.xml"); ;
		}
	}
}
