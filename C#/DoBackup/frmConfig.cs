using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using DoctorGu;

namespace DoBackup
{
	public partial class frmConfig : Form
	{
		private string mFullPath;

		private struct SInfo
		{
			public string Name;
			public string RootFolderSrc;
			public string[] aRootFolderDest;
			public CFtpInfoSync FtpInfo;
			public int IntervalMinutes;
			public SyncTypes SyncType;
			public bool MinifyJs;
            public bool EmptyFolderHasNoFile;
			public string[] aFullPathReferencingJs;
			public string[] aJsFullPathRefered;
			public DateTime DateTimeAfter;
			public string DisallowedExt;
			public string AllowedOnlyExt;
			public string DisallowedFolder;
			public string AllowedOnlyFolder;
			public string LogFolder;
		}

		public frmConfig()
		{
			InitializeComponent();
		}

		private void frmConfig_Load(object sender, EventArgs e)
		{
			//DataTable dt = GetTable();
			//if (dt.Rows.Count == 0)
			//{
			//    dt.Rows.Add(dt.NewRow());
			//}

			this.Text += " " + Application.ProductVersion;

			DataTable dt = CEnum.GetDataTableByValueNameDescription<SyncTypes>();
			cboSyncType.ValueMember = "Name";
			cboSyncType.DisplayMember = "Name";
			cboSyncType.DataSource = dt;

			RestoreConfig();

			//grvList.Columns["FtpPassword"].Visible = false;
		}
		private void frmConfig_FormClosing(object sender, FormClosingEventArgs e)
		{
			SaveConfig();
		}

		private void tlsbtnOpen_Click(object sender, EventArgs e)
		{
			DialogResult dret = dlgOpen.ShowDialog(this);
			if (dret != DialogResult.OK)
				return;

			this.mFullPath = dlgOpen.FileName;
			sttlblFullPath.Text = this.mFullPath;

			DataTable dt = GetTable();
			dt.Rows.Clear();
			dt.ReadXml(this.mFullPath);
		}
		private void tlsbtnSave_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(this.mFullPath))
			{
				tlsbtnSaveAs.PerformClick();
				return;
			}

			DataTable dt = GetTable();
			dt.WriteXml(this.mFullPath, XmlWriteMode.WriteSchema);
		}
		private void tlsbtnSaveAs_Click(object sender, EventArgs e)
		{
			DialogResult dret = dlgSave.ShowDialog(this);
			if (dret != DialogResult.OK)
				return;

			this.mFullPath = dlgSave.FileName;
			sttlblFullPath.Text = this.mFullPath;

			DataTable dt = GetTable();
			dt.WriteXml(this.mFullPath, XmlWriteMode.WriteSchema);
		}

		private void tlsbtnDelete_Click(object sender, EventArgs e)
		{
			int Index = grvList.SelectedCells[0].RowIndex;
			CDataGridView.DeleteRow(grvList, Index);
		}

		private void tlsbtnMoveUp_Click(object sender, EventArgs e)
		{
			CDataGridView.UpDownSelectedRowOfDataTable(grvList, true);
		}
		private void tlsbtnMoveDown_Click(object sender, EventArgs e)
		{
			CDataGridView.UpDownSelectedRowOfDataTable(grvList, false);
		}

		private int _RowIndexCopied = -1;
		private int _RowIndexCutted = -1;
		private void tlsbtnCopy_Click(object sender, EventArgs e)
		{
			_RowIndexCopied = grvList.SelectedCells[0].RowIndex;
			_RowIndexCutted = -1;
		}
		private void tlsbtnCut_Click(object sender, EventArgs e)
		{
			_RowIndexCopied = -1;
			_RowIndexCutted = grvList.SelectedCells[0].RowIndex;
		}
		private void tlsbtnPaste_Click(object sender, EventArgs e)
		{
			int IndexTo = grvList.SelectedCells[0].RowIndex;
			if (_RowIndexCopied != -1)
				CDataGridView.CopyRow(grvList, _RowIndexCopied, IndexTo);
			else if (_RowIndexCutted != -1)
				CDataGridView.MoveRow(grvList, _RowIndexCutted, IndexTo);
		}

		private void tlsbtnHelp_Click(object sender, EventArgs e)
		{
			string Msg =
@"Only will copy from Source Folder to Destination Folder.

If file in Source Folder deleted, file in Destination Folder will not deleted.

Adding, editing, deleting from Destination Folder will not affect to Source Folder's file.";

			MessageBox.Show(Msg, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		private void tlsbtnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void txtDisallowedExt_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtDisallowedExt.Text))
			{
				radDisallowedExt.Checked = true;
				txtAllowedOnlyExt.Text = "";
			}
		}
		private void txtAllowedOnlyExt_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtAllowedOnlyExt.Text))
			{
				radAllowedOnlyExt.Checked = true;
				txtDisallowedExt.Text = "";
			}
		}

		private void txtDisallowedFolder_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtDisallowedFolder.Text))
			{
				radDisallowedFolder.Checked = true;
				txtAllowedOnlyFolder.Text = "";
			}
		}
		private void txtAllowedOnlyFolder_TextChanged(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(txtAllowedOnlyFolder.Text))
			{
				radAllowedOnlyFolder.Checked = true;
				txtDisallowedFolder.Text = "";
			}
		}

		private void btnOpenRootFolderSrc_Click(object sender, EventArgs e)
		{
			string Folder = txtRootFolderSrc.Text.Trim();
			if (Directory.Exists(Folder))
			{
				Process.Start(Folder);
			}
		}

		private void btnOpenRootFolderDest_Click(object sender, EventArgs e)
		{
			string Folder = txtRootFolderDest.Text.Trim();
			if (Directory.Exists(Folder))
			{
				Process.Start(Folder);
			}
		}

		private void btnRootFolderDest_Click(object sender, EventArgs e)
		{
			using (frmInputMultiLine f = new frmInputMultiLine())
			{
				f.TextSemicolon = txtRootFolderDest.Text;
				f.StartPosition = FormStartPosition.CenterParent;
				DialogResult dret = f.ShowDialog(this);
				if (dret != DialogResult.OK)
					return;

				txtRootFolderDest.Text = f.TextSemicolon;
			}
		}

		private void btnDeleteEmptyRootFolderDest_Click(object sender, EventArgs e)
		{
			string Folder = txtRootFolderDest.Text.Trim();
            string Msg = string.Format("Do you want to empty following foler(s) that has no file(s)?\n\n{0}", Folder);
			DialogResult dret = MessageBox.Show(Msg, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (dret != System.Windows.Forms.DialogResult.OK)
				return;

            string[] aRootFolderDest = txtRootFolderDest.Text.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string RootFolderDest in aRootFolderDest)
            {
                CFile.EmptyFolderHasNoFile(RootFolderDest);
            }

			MessageBox.Show(this, "Empty succeeded.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
		
		private void btnNow_Click(object sender, EventArgs e)
		{
			dtpDateTimeAfter.Value = DateTime.Now;
		}

		private void btnFullPathReferencingJs_Click(object sender, EventArgs e)
		{
			using (frmInputMultiLine f = new frmInputMultiLine())
			{
				f.TextSemicolon = txtFullPathReferencingJs.Text;
				f.StartPosition = FormStartPosition.CenterParent;
				DialogResult dret = f.ShowDialog(this);
				if (dret != DialogResult.OK)
					return;

				txtFullPathReferencingJs.Text = f.TextSemicolon;
			}
		}

		private void btnJsFullPathRefered_Click(object sender, EventArgs e)
		{
			using (frmInputMultiLine f = new frmInputMultiLine())
			{
				f.TextSemicolon = txtJsFullPathRefered.Text;
				f.StartPosition = FormStartPosition.CenterParent;
				DialogResult dret = f.ShowDialog(this);
				if (dret != DialogResult.OK)
					return;

				txtJsFullPathRefered.Text = f.TextSemicolon;
			}
		}

		private void btnDisallowedFolder_Click(object sender, EventArgs e)
		{
			using (frmInputMultiLine f = new frmInputMultiLine())
			{
				f.TextSemicolon = txtDisallowedFolder.Text;
				f.StartPosition = FormStartPosition.CenterParent;
				DialogResult dret = f.ShowDialog(this);
				if (dret != DialogResult.OK)
					return;

				txtDisallowedFolder.Text = f.TextSemicolon;
			}
		}

		private void btnAllowedOnlyFolder_Click(object sender, EventArgs e)
		{
			using (frmInputMultiLine f = new frmInputMultiLine())
			{
				f.TextSemicolon = txtAllowedOnlyFolder.Text;
				f.StartPosition = FormStartPosition.CenterParent;
				DialogResult dret = f.ShowDialog(this);
				if (dret != DialogResult.OK)
					return;

				txtAllowedOnlyFolder.Text = f.TextSemicolon;
			}
		}

		private void btnLogFolder_Click(object sender, EventArgs e)
		{
			string Folder = txtLogFolder.Text.Trim();
			if (Directory.Exists(Folder))
			{
				Process.Start(Folder);
			}
		}

		private void btnSyncNow_Click(object sender, EventArgs e)
		{
			using (new CWaitCursor(this, btnSyncNow))
			{
				SInfo InfoIs;
				string ErrMsgIs;
				if (!IsValid(out InfoIs, out ErrMsgIs))
				{
					MessageBox.Show(ErrMsgIs, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
					return;
				}

				CSyncFile SyncFile = null;
				if (InfoIs.FtpInfo == null)
				{
					SyncFile = new CSyncFile(InfoIs.RootFolderSrc, InfoIs.aRootFolderDest, InfoIs.SyncType, InfoIs.MinifyJs, InfoIs.aFullPathReferencingJs, InfoIs.aJsFullPathRefered, InfoIs.DateTimeAfter, InfoIs.LogFolder);
				}
				else
				{
					SyncFile = new CSyncFile(InfoIs.RootFolderSrc, new CFtpInfoSync[] { InfoIs.FtpInfo }, InfoIs.SyncType, InfoIs.MinifyJs, InfoIs.aFullPathReferencingJs, InfoIs.aJsFullPathRefered, InfoIs.DateTimeAfter, InfoIs.LogFolder);
				}

				if (radDisallowedExt.Checked)
					SyncFile.DisallowedExt = InfoIs.DisallowedExt.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				else
					SyncFile.AllowedOnlyExt = InfoIs.AllowedOnlyExt.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

				if (radDisallowedFolder.Checked)
					SyncFile.DisallowedFolder = InfoIs.DisallowedFolder.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				else
					SyncFile.AllowedOnlyFolder = InfoIs.AllowedOnlyFolder.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

				SyncFile.CopyAll();

                if (InfoIs.EmptyFolderHasNoFile)
                {
                    foreach (string RootFolderDest in InfoIs.aRootFolderDest)
                    {
                        CFile.EmptyFolderHasNoFile(RootFolderDest);
                    }
                }

				if (SyncFile.CountFailed > 0)
				{
					MessageBox.Show(string.Format("{0} count of file failed.", SyncFile.CountFailed), "Information", MessageBoxButtons.OK);
					return;
				}

				if (InfoIs.SyncType == SyncTypes.AfterSrcTime)
				{
					//다음번에 이 시간을 기준할 수 있게 함.
					DataTable dt = GetTable();
					DataGridViewCell dgv = grvList.SelectedCells[0];
					dt.Rows[dgv.OwningRow.Index]["DateTimeAfter"] = DateTime.Now;
					tlsbtnSave.PerformClick();
				}
				sttlblMsg.Text = "Synchronization completed at " + DateTime.Now.ToString();
			}
		}

		#region Method
		private bool IsValid(out SInfo InfoIs, out string ErrMsgIs)
		{
			InfoIs = new SInfo();
			ErrMsgIs = "";

			DataTable dt = GetTable();
			InfoIs.Name = txtName.Text.Trim();
			if (InfoIs.Name == "")
			{
				ErrMsgIs = "Name not entered.";
				return false;
			}

			InfoIs.RootFolderSrc = txtRootFolderSrc.Text.Trim();
			if (InfoIs.RootFolderSrc == "")
			{
				ErrMsgIs = "Source Folder not entered.";
				return false;
			}
			if (!Directory.Exists(InfoIs.RootFolderSrc))
			{
				ErrMsgIs = "Source Folder: " + InfoIs.RootFolderSrc + " is not exists.";
				return false;
			}

			InfoIs.aRootFolderDest = txtRootFolderDest.Text.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string RootFolderDest in InfoIs.aRootFolderDest)
			{
				if (RootFolderDest != "")
				{
					if (!Directory.Exists(RootFolderDest))
					{
						ErrMsgIs = "Destination Folder: " + RootFolderDest + " is not exists.";
						return false;
					}
				}
			}
			
			if (txtFtpHost.Text.Trim() != "")
			{
				InfoIs.FtpInfo = new CFtpInfoSync();
				InfoIs.FtpInfo.Host = txtFtpHost.Text.Trim();
				InfoIs.FtpInfo.Folder = txtFtpFolder.Text.Trim();
				InfoIs.FtpInfo.Port = Convert.ToInt32(nudFtpPort.Value);
				InfoIs.FtpInfo.UsePassive = chkFtpUsePassive.Checked;
				InfoIs.FtpInfo.UserId = txtFtpId.Text.Trim();
				InfoIs.FtpInfo.Password = txtFtpPassword.Text.Trim();
			}

			if ((InfoIs.aRootFolderDest.Length == 0) && (InfoIs.FtpInfo == null))
			{
				ErrMsgIs = "Both of Destination Folder and FTP Info is empty.";
				return false;
			}
			else if ((InfoIs.aRootFolderDest.Length != 0) && (InfoIs.FtpInfo != null))
			{
				ErrMsgIs = "Both of Destination Folder and FTP Info is not empty.";
				return false;
			}

			InfoIs.IntervalMinutes = Convert.ToInt32(nudIntervalMinutes.Value);
			InfoIs.SyncType = CEnum.GetValueByName<SyncTypes>((string)cboSyncType.SelectedValue);
			InfoIs.MinifyJs = chkMinifyJs.Checked;
            InfoIs.EmptyFolderHasNoFile = chkEmptyFolderHasNoFile.Checked;
			InfoIs.aFullPathReferencingJs = txtFullPathReferencingJs.Text.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			InfoIs.aJsFullPathRefered = txtJsFullPathRefered.Text.Trim().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			InfoIs.DateTimeAfter = dtpDateTimeAfter.Value;
			InfoIs.DisallowedExt = txtDisallowedExt.Text.Trim();
			InfoIs.AllowedOnlyExt = txtAllowedOnlyExt.Text.Trim();
			InfoIs.DisallowedFolder = txtDisallowedFolder.Text.Trim();
			InfoIs.AllowedOnlyFolder = txtAllowedOnlyFolder.Text.Trim();
			
			InfoIs.LogFolder = txtLogFolder.Text.Trim();
			if (InfoIs.LogFolder == "")
			{
				ErrMsgIs = "Log Folder not entered.";
				return false;
			}
			if (!Directory.Exists(InfoIs.LogFolder))
			{
				ErrMsgIs = "Log Folder: " + InfoIs.LogFolder + " is not exists.";
				return false;
			}

			return true;
		}

		private DataTable GetTable()
		{
			DataSet ds = (DataSet)grvList.DataSource;
			DataTable dt = ds.Tables[0];
			//DataTable dt = dsConfig.Tables[0];
			return dt;
		}

		private void RestoreConfig()
		{
			CXmlConfig xc = new CXmlConfig(CCommon.Const.ConfigXmlFullPath);
			string FullPath = xc.GetSetting("FullPath", "");
			if (FullPath == "")
				return;

			if (!File.Exists(FullPath))
				return;

			this.mFullPath = FullPath;
			sttlblFullPath.Text = this.mFullPath;

			DataTable dt = GetTable();
			dt.Rows.Clear();
			dt.ReadXml(this.mFullPath);
		}
		private void SaveConfig()
		{
			if (string.IsNullOrEmpty(this.mFullPath))
				return;

			if (!File.Exists(this.mFullPath))
				return;

			CXmlConfig xc = new CXmlConfig(CCommon.Const.ConfigXmlFullPath);
			xc.SaveSetting("FullPath", this.mFullPath);
		}

		#endregion
	}
}



