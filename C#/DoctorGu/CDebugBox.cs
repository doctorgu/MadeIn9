using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DoctorGu;

namespace EngCommon
{
	public partial class CDebugBox : Form
	{
		public CDebugBox()
		{
			InitializeComponent();
		}

		private void CDebugBox_Load(object sender, EventArgs e)
		{
			CEvent.ApplyKeyToTextBox(ControlShortcutKeys.ControlA, txtLog);
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			ClearLog();
		}

		//=========================================
		private static CDebugBox _DebugBox;
		public static void AddLog(string Log)
		{
			CDebugBox DebugBox = GetDebugBox();
			DebugBox.txtLog.AppendText(DateTime.Now.ToString(CConst.Format_HH_mm_ss_fff) + " " + Log + "\r\n");
		}

		public static void ClearLog()
		{
			CDebugBox DebugBox = GetDebugBox();
			DebugBox.txtLog.Text = "";
		}

		private static CDebugBox GetDebugBox()
		{
			if ((_DebugBox == null) || !_DebugBox.Visible)
			{
				_DebugBox = new CDebugBox();
				_DebugBox.Show();
			}

			return _DebugBox;
		}
	}
}
