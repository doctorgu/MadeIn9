using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using DoctorGu;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace DoBackup
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				MessageBox.Show("Cannot execute alone. Contact to system manager.", "Information", MessageBoxButtons.OK);
				return;
			}

			string SendToPath = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
			//바로가기가 이미 삭제되어 없는 실행파일의 경로를 참조할 수도 있으므로 무조건 만듦.
			CWsh.CreateShortcut(Application.ExecutablePath, SendToPath);

			NameValueCollection cmd = CDelim.GetCommandLineArgs(args, "/");
			if (cmd.GetKey(0) == "auto")
			{
				frmConfig f = new frmConfig();
				f.ShowDialog();
			}
			else
			{
				frmSyncSelected f = new frmSyncSelected(args);
				f.ShowDialog();
			}

			//else
			//{
			//    ServiceBase[] ServicesToRun;
			//    ServicesToRun = new ServiceBase[] { new SvcBackup() };
			//    ServiceBase.Run(ServicesToRun);
			//}
		}
	}
}