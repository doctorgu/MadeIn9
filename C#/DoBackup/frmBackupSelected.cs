using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DoctorGu;
using System.IO;

namespace DoBackup
{
	public partial class frmBackupSelected : Form
	{
		private struct SInfoBackup
		{
			public string Name;
			public string RootFolderSrc;
			public string RootFolderDest;
			public CFtpInfoSync FtpInfo;
			public bool MinifyJs;
			public string[] aFullPathReferencingJs;
			public string[] aJsFullPathRefered;
			public string LogFolder;
		}

		private struct SInfo
		{
			public string FullPath;
		}

		private DataTable mDt = null;
		private string[] maFullPathFromCmd;

		public frmBackupSelected(string[] aFullPathFromCmd)
		{
			this.maFullPathFromCmd = aFullPathFromCmd;

			InitializeComponent();
		}

		private void frmBackupSelected_Load(object sender, EventArgs e)
		{
			SInfo InfoIs;
			string ErrMsgIs;
			if (!IsValidForShow(out InfoIs, out ErrMsgIs))
			{
				MessageBox.Show(ErrMsgIs, "Information", MessageBoxButtons.OK);
				this.Close();
			}

			this.mDt = new DataTable();
			this.mDt.ReadXml(InfoIs.FullPath);

			ShowNameList(this.mDt);
		}

		private void btnBackup_Click(object sender, EventArgs e)
		{
			if (lstName.CheckedItems.Count == 0)
			{
				MessageBox.Show("Not selected anything.", "Information", MessageBoxButtons.OK);
				return;
			}

			List<string> aMsg = new List<string>();

			using (new CWaitCursor(this, btnBackup))
			{
				for (int i = 0; i < lstName.CheckedItems.Count; i++)
				{
					string NameCur = (string)lstName.CheckedItems[i];

					SInfoBackup InfoIs;
					string ErrMsgIs;
					if (!IsValidForBackup(NameCur, out InfoIs, out ErrMsgIs))
					{
						MessageBox.Show(ErrMsgIs, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
						return;
					}

					CSyncFile SyncFile = null;
					if (InfoIs.FtpInfo == null)
					{
						SyncFile = new CSyncFile(this.maFullPathFromCmd, InfoIs.RootFolderSrc, new string[] { InfoIs.RootFolderDest }, SyncTypes.AfterSrcTime, InfoIs.MinifyJs, InfoIs.aFullPathReferencingJs, InfoIs.aJsFullPathRefered, DateTime.MinValue, InfoIs.LogFolder);
					}
					else
					{
						SyncFile = new CSyncFile(this.maFullPathFromCmd, InfoIs.RootFolderSrc, new CFtpInfoSync[] { InfoIs.FtpInfo }, SyncTypes.AfterSrcTime, InfoIs.MinifyJs, InfoIs.aFullPathReferencingJs, InfoIs.aJsFullPathRefered, DateTime.MinValue, InfoIs.LogFolder);
					}
					
					//선택한 파일은 무조건 복사해야 해서 주석
					//SyncFile.DisallowedExt = InfoIs.DisallowedExt.Split(';');
					//if (InfoIs.DisallowedFolder != "")
					//{
					//    SyncFile.DisallowedFolder = InfoIs.DisallowedFolder.Split(';');
					//}
					SyncFile.CopyAll();

					if (SyncFile.CountFailed > 0)
						aMsg.Add(string.Format("There's {0} count failed from {1}", SyncFile.CountFailed, NameCur));
				}
			}

			aMsg.Add("Completed.");
			MessageBox.Show(string.Join("\r\n\r\n", aMsg.ToArray()), "Information", MessageBoxButtons.OK);
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		//=========================================
		private bool IsValidForShow(out SInfo InfoIs, out string ErrMsgIs)
		{
			InfoIs = new SInfo();
			ErrMsgIs = "";

			CXmlConfig xc = new CXmlConfig(CCommon.Const.ConfigXmlFullPath);
			InfoIs.FullPath = xc.GetSetting("FullPath", "");
			if (InfoIs.FullPath == "")
			{
				ErrMsgIs = "Config not saved.";
				return false;
			}

			if (!File.Exists(InfoIs.FullPath))
			{
				ErrMsgIs = InfoIs.FullPath + " file not exists.";
				return false;
			}


			return true;
		}

		private bool IsValidForBackup(string Name, out SInfoBackup InfoIs, out string ErrMsgIs)
		{
			InfoIs = new SInfoBackup();
			ErrMsgIs = "";

			
			DataRow dr = this.mDt.Select("Name = '" + Name + "'")[0];

			InfoIs.Name = (string)dr["Name"];
			if (InfoIs.Name == "")
			{
				ErrMsgIs = "Name not entered.";
				return false;
			}

			InfoIs.RootFolderSrc = (string)dr["RootFolderSrc"];
			if (InfoIs.RootFolderSrc == "")
			{
				ErrMsgIs = "Source folder not entered.";
				return false;
			}
			if (!Directory.Exists(InfoIs.RootFolderSrc))
			{
				ErrMsgIs = "Source Folder: " + InfoIs.RootFolderSrc + " is not exists.";
				return false;
			}

			InfoIs.RootFolderDest = (string)dr["RootFolderDest"];
			if (InfoIs.RootFolderDest != "")
			{
				if (!Directory.Exists(InfoIs.RootFolderDest))
				{
					ErrMsgIs = "Destination Folder: " + InfoIs.RootFolderDest + " is not exists.";
					return false;
				}
			}

			string FtpHost = (string)dr["FtpHost"];
			if (FtpHost.Trim() != "")
			{
				InfoIs.FtpInfo = new CFtpInfoSync();
				InfoIs.FtpInfo.Host = (string)dr["FtpHost"];
				InfoIs.FtpInfo.Folder = (string)dr["FtpFolder"];
				InfoIs.FtpInfo.Port = Convert.ToInt32(dr["FtpPort"]);
				InfoIs.FtpInfo.UsePassive = Convert.ToBoolean(dr["FtpUsePassive"]);
				InfoIs.FtpInfo.UserId = (string)dr["FtpId"];
				InfoIs.FtpInfo.Password = (string)dr["FtpPassword"];
			}

			if ((InfoIs.RootFolderDest == "") && (InfoIs.FtpInfo == null))
			{
				ErrMsgIs = "Both Destination Folder and FTP Info is empty.";
				return false;
			}
			else if ((InfoIs.RootFolderDest != "") && (InfoIs.FtpInfo != null))
			{
				ErrMsgIs = "Both Destination Folder and FTP Info is not empty.";
				return false;
			}

			InfoIs.MinifyJs = (bool)dr["MinifyJs"];

			InfoIs.aFullPathReferencingJs = ((string)dr["FullPathReferencingJs"]).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			InfoIs.aJsFullPathRefered = ((string)dr["JsFullPathRefered"]).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

			InfoIs.LogFolder = (string)dr["LogFolder"];
			if (InfoIs.LogFolder == "")
			{
				ErrMsgIs = "Log Folder is not entered.";
				return false;
			}
			if (!Directory.Exists(InfoIs.LogFolder))
			{
				ErrMsgIs = "Log Folder: " + InfoIs.LogFolder + " is not exists.";
				return false;
			}

			return true;
		}

		private void ShowNameList(DataTable dt)
		{
			foreach (DataRow dr in dt.Rows)
			{
				lstName.Items.Add((string)dr["Name"]);
			}
		}
	}
}
