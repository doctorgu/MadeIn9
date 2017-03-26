using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Collections;
using System.Reflection;

namespace DoctorGu
{
	public class CUtil
	{
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll")]
		private static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr Arguments);

		public static string GetLastWin32ErrorInfo()
		{
			const int FORMAT_MESSAGE_FROM_HMODULE = 2048;

			StringBuilder szString = new StringBuilder(256);
			
			int dwError = Marshal.GetLastWin32Error();

			int dwRet = FormatMessage(FORMAT_MESSAGE_FROM_HMODULE,
				GetModuleHandle("KERNEL32.DLL"), dwError, 0,
				szString, szString.Capacity, IntPtr.Zero);
			
			string ErrorMessage = "error code: " + dwError + " Message: " + szString.ToString();
			return ErrorMessage;
		}

		/// <summary>
		/// 주의: 현재 시점에서는 참조하는 파일이 없을 수 있으므로 이곳에서는 참조하는 외부 파일을 이용하면 안됨.
		/// </summary>
		/// <param name="htRefs"></param>
		public static void WriteReferencedToResources(Hashtable htRefs)
		{
			string ResPathFile = @"..\..\" + Application.ProductName + ".resources";
			ResourceWriter rw = new ResourceWriter(ResPathFile);

			foreach (DictionaryEntry d in htRefs)
			{
				string Name = (string)d.Key;
				string PathFile = (string)d.Value;
				if (!System.IO.File.Exists(PathFile)) continue;

				FileStream fs = new FileStream(PathFile, FileMode.Open, FileAccess.Read, FileShare.None);
				int len = Convert.ToInt32(fs.Length);
				byte[] byt = new byte[len];
				fs.Read(byt, 0, len);
				fs.Close();

				rw.AddResource(Name, byt);
			}

			rw.Close();
		}
		/// <summary>
		/// 주의: 현재 시점에서는 참조하는 파일이 없을 수 있으므로 이곳에서는 참조하는 외부 파일을 이용하면 안됨.
		/// </summary>
		/// <param name="htRefs"></param>
		public static void SaveResourcesToFiles(Hashtable htRefs)
		{
			string ResName = "AutoUpdate." + Application.ProductName;
			ResourceManager rm = new ResourceManager(ResName, CAssembly.GetEntryOrExecuting());

			foreach (DictionaryEntry d in htRefs)
			{
				string Name = (string)d.Key;
				string PathFile = (string)d.Value;

				string NewPathFile = Path.GetDirectoryName(Application.ExecutablePath)
									+ "\\" + CPath.GetFileName(PathFile);

				if (System.IO.File.Exists(NewPathFile)) continue;

				byte[] byt = (byte[])rm.GetObject(Name);
				FileStream fs = new FileStream(NewPathFile, FileMode.Create, FileAccess.Write, FileShare.Write);
				fs.Write(byt, 0, byt.Length);
				fs.Close();

				//DoctorGu.dll이 없을 수도 있으므로
				//CFile.WaitForFileCreation(NewPathFile)을 사용하지 않음.
				for (int i = 0; i < 10; i++)
				{
					if (System.IO.File.Exists(NewPathFile))
					{
						break;
					}

					System.Threading.Thread.Sleep(1000);
				}
			}
		}
	}

	/// <summary>
	/// 작업 도중에만 커서 모양을 모래시계로 변경하기 위함.
	/// </summary>
	/// <example>
	/// //바로 Dispose 메쏘드가 호출될 수 있도록 using을 사용함.
	/// using (new WaitCursor(this))
	/// {
	/// 	System.Threading.Thread.Sleep(1000);
	/// 	this.Text = DateTime.Now.ToString();
	/// }
	/// </example>
	public class CWaitCursor : IDisposable
	{
		private Form f;
		private Control mCtlToDisable;
		public CWaitCursor(Form f)
		{
			this.f = f;
			this.f.Cursor = Cursors.WaitCursor;
		}
		public CWaitCursor(Form f, Control CtlToDisable)
		{
			this.f = f;
			this.f.Cursor = Cursors.WaitCursor;

			this.mCtlToDisable = CtlToDisable;
			this.mCtlToDisable.Enabled = false;
		}

		~CWaitCursor()
		{
			Dispose();
		}

		public void Dispose()
		{
			try
			{
				this.f.Cursor = Cursors.Default;
				if (this.mCtlToDisable != null)
				{
					this.mCtlToDisable.Enabled = true;
				}
			}
			catch (Exception) { }
		}
	}

	/// <summary>
	/// 현재 어떤 함수를 실행 중이면 같은 함수를 호출했을 때 실행되지 않도록 하기 위함.
	/// </summary>
	/// <example>
	/// <![CDATA[
	/// private CRunWhenNotRunning mRunWhenNotRunningWpLog = new CRunWhenNotRunning();
	/// 
	/// void tmrWpLog_Elapsed(object sender, ElapsedEventArgs e)
	/// {
	///		if (this.mRunWhenNotRunningWpLog.Running)
	///			return;
	///			
	///		using (this.mRunWhenNotRunningWpLog = new CRunWhenNotRunning())
	///		{
	///			this.mRunWhenNotRunningWpLog.Running = true;
	///			
	///			...코드
	///		}
	///	}
	/// ]]>
	/// </example>
	public class CRunWhenNotRunning : IDisposable
	{
		private bool _Running = false;
		private object _Lock = new object();

		~CRunWhenNotRunning()
		{
			Dispose();
		}

		public bool Running
		{
			get { return this._Running; }
			set 
			{
				lock (_Lock)
					this._Running = value;
			}
		}

		public void Dispose()
		{
			lock (_Lock)
				this._Running = false;
		}
	}
}
