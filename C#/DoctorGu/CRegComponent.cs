using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace DoctorGu
{
	/// <summary>
	/// ActiveX DLL, ActiveX EXE를 레지스트리에 등록하거나 삭제함.
	/// </summary>
	/// <example>
	/// CRegComponent.Register(@"C:\My\MadeIn9\VB\HappyFTP\HappyFTP.dll");
	/// CRegComponent.Unregister(@"C:\My\MadeIn9\VB\HappyFTP\HappyFTP.dll");
	/// </example>
	public class CRegComponent
	{
		[DllImport("kernel32", SetLastError = true)]
		private static extern int LoadLibraryA(string lpLibFileName);
		[DllImport("kernel32")]
		private static extern int FreeLibrary(int hLibModule);
		[DllImport("kernel32")]
		private static extern int CloseHandle(int hObject);
		[DllImport("kernel32", SetLastError = true)]
		private static extern int GetProcAddress(int hModule, string lpProcName);
		[DllImport("kernel32", SetLastError = true)]
		private static extern int CreateThread(int lpThreadAttributes, int dwStackSize, int lpStartAddress, int lpParameter, int dwCreationFlags, int lpThreadID);
		[DllImport("kernel32")]
		private static extern int WaitForSingleObject(int hHandle, int dwMilliseconds);
		[DllImport("kernel32")]
		private static extern int GetExitCodeThread(int hThread, int lpExitCode);
		[DllImport("kernel32")]
		private static extern int ExitThread(int dwExitCode);

		private const int STATUS_WAIT_0 = 0x0;
		private const int WAIT_OBJECT_0 = ((STATUS_WAIT_0) + 0);


		public static void Register(string FileName)
		{
			RegComponent(FileName, true);
		}
		public static void Unregister(string FileName)
		{
			RegComponent(FileName, false);
		}
		private static void RegComponent(string FileName, bool IsRegister)
		{
			string RegsvrFullPath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.System), "regsvr32.exe");

			Process p = new Process();

			ProcessStartInfo si = new ProcessStartInfo(RegsvrFullPath);
			si.Arguments = (!IsRegister ? " /u": "") + " /s \"" + FileName + "\"";
			si.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo = si;

			p.Start();
			p.WaitForExit(3000);
		}

		public static void RegisterByApi(string FileName)
		{
			RegComponentByApi(FileName, true);
		}
		public static void UnregisterByApi(string FileName)
		{
			RegComponentByApi(FileName, false);
		}
		private static void RegComponentByApi(string FileName, bool IsRegister)
		{
			//2011-06-07 등록 안되는 Windows 7 컴퓨터 있어 주석.
			int nLib = LoadLibraryA(FileName);
			if (nLib == 0)
			{
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}

			string ProcName = IsRegister ? "DllRegisterServer" : "DllUnregisterServer";
			int nProcAddress = GetProcAddress(nLib, ProcName);
			if (nProcAddress == 0)
			{
				FreeLibrary(nLib);
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}

			int hThread = CreateThread(0, 0, nProcAddress, 0, 0, 0);
			if (hThread == 0)
			{
				FreeLibrary(nLib);
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}

			bool Success = (WaitForSingleObject(hThread, 10000) == WAIT_OBJECT_0);
			if (!Success)
			{
				int dwExitCode = 0;
				GetExitCodeThread(hThread, dwExitCode);
				ExitThread(dwExitCode);
				FreeLibrary(nLib);
				throw new Exception(CUtil.GetLastWin32ErrorInfo());
			}

			CloseHandle(hThread);
			FreeLibrary(nLib);
		}

		public static void RegisterNet(FrameworkVersion Ver, string FullPath)
		{
			string RegasmFullPath = Path.Combine(CPath.GetFrameworkFolder(Ver), "regasm.exe");

			Process p = new Process();
			
			ProcessStartInfo si = new ProcessStartInfo(RegasmFullPath);
			string TlbName = Path.GetFileNameWithoutExtension(FullPath) + ".tlb";
			si.Arguments = "\"" + FullPath + "\"" + " /codebase /tlb:" + TlbName;
			si.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo = si;

			p.Start();
			p.WaitForExit(3000);
			//C:\Windows\Microsoft.NET\Framework\v4.0.30319>regasm D:\My\MadeIn9\C#\ComhwalChecker\bin\Release\ComhwalChecker.dll /codebase /tlb:ComhwalChecker.tlb
		}
		public static void UnregisterNet(FrameworkVersion Ver, string FullPath)
		{
			string RegasmFullPath = Path.Combine(CPath.GetFrameworkFolder(Ver), "regasm.exe");

			Process p = new Process();

			ProcessStartInfo si = new ProcessStartInfo(RegasmFullPath);
			si.Arguments = "\"" + FullPath + "\"" + " /unregister";
			si.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo = si;

			p.Start();
			p.WaitForExit(3000);
		}
	}
}
