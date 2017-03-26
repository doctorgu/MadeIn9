using System;
using System.Diagnostics;
using System.Management;
using System.Linq;


namespace DoctorGu
{
	/*class Win32_Process : CIM_Process
	{
	  string   Caption;
	  string   CommandLine;
	  string   CreationClassName;
	  datetime CreationDate;
	  string   CSCreationClassName;
	  string   CSName;
	  string   Description;
	  string   ExecutablePath;
	  uint16   ExecutionState;
	  string   Handle;
	  uint32   HandleCount;
	  datetime InstallDate;
	  uint64   KernelModeTime;
	  uint32   MaximumWorkingSetSize;
	  uint32   MinimumWorkingSetSize;
	  string   Name;
	  string   OSCreationClassName;
	  string   OSName;
	  uint64   OtherOperationCount;
	  uint64   OtherTransferCount;
	  uint32   PageFaults;
	  uint32   PageFileUsage;
	  uint32   ParentProcessId;
	  uint32   PeakPageFileUsage;
	  uint64   PeakVirtualSize;
	  uint32   PeakWorkingSetSize;
	  uint32   Priority;
	  uint64   PrivatePageCount;
	  uint32   ProcessId;
	  uint32   QuotaNonPagedPoolUsage;
	  uint32   QuotaPagedPoolUsage;
	  uint32   QuotaPeakNonPagedPoolUsage;
	  uint32   QuotaPeakPagedPoolUsage;
	  uint64   ReadOperationCount;
	  uint64   ReadTransferCount;
	  uint32   SessionId;
	  string   Status;
	  datetime TerminationDate;
	  uint32   ThreadCount;
	  uint64   UserModeTime;
	  uint64   VirtualSize;
	  string   WindowsVersion;
	  uint64   WorkingSetSize;
	  uint64   WriteOperationCount;
	  uint64   WriteTransferCount;
	};
	*/


	/// <summary>
	/// 프로세스와 관련된 기능 구현.
	/// </summary>
	/// <example>
	/// 다음은 .Net의 Process.Start 만으로 연결된 프로그램을 실행하는 예이며,
	/// 이런 경우엔 따로 함수를 만들지 않았습니다.
	/// <code>
	/// //웹 페이지 탐색
	/// Process.Start("www.microsoft.com");
	/// //해당 폴더 열기
	/// Process.Start(@"c:\windows\fonts\");
	/// //연결 파일 실행
	/// Process.Start(@"c:\windows\setup.log");
	/// 
	/// //연결 파일 실행해서 인쇄
	/// ProcessStartInfo pinfo = new ProcessStartInfo();
	/// pinfo.FileName = @"c:\windows\setup.log";
	/// pinfo.WindowStyle = ProcessWindowStyle.Hidden;
	/// pinfo.Verb = "print";
	/// Process.Start(pinfo);
	/// </code>
	/// </example>
	public class CProc
	{
		public CProc()
		{
		}

		/// <summary>
		/// 현재 프로그램이 이미 실행되었는 지 여부를 알아냄.
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// //다음은 이미 실행 중이 아닐 때만 frmBoard 폼을 띄움.
		/// Process p = CProc.GetPreviousRunningApp();
		/// if (p != null)
		/// {
		///		return;
		/// }
		/// frmBoard f = new frmBoard();
		/// Application.Run(f);
		/// </example>
		public static Process GetPreviousRunningApp()
		{
			Process p = Process.GetCurrentProcess();
			string ProcName = p.ProcessName;
			int ProcId = p.Id;

			Process[] ps = Process.GetProcesses();
			foreach (Process pr in ps)
			{
				if ((pr.Id != ProcId) && (pr.ProcessName == ProcName))
					return pr;
			}

			return null;
		}

		/// <summary>
		/// 어떤 프로그램이 종료되기 전까지 계속해서 루프를 돌아
		/// 다음의 코드가 실행되지 않게 함.
		/// </summary>
		/// <param name="ProcId"></param>
		/// <param name="MaxSeconds"></param>
		/// <returns></returns>
		public static bool WaitUntilExitProc(int ProcId, int MaxSeconds)
		{
			long Max = MaxSeconds * 10000000;

			long Start = DateTime.Now.Ticks;
			while (true)
			{
				Process p = null;
				try
				{
					p = Process.GetProcessById(ProcId);
				}
				catch (Exception)
				{
					return true;
				}

				if ((DateTime.Now.Ticks - Start) > Max)
				{
					return false;
				}
			}
		}

		/// <summary>
		/// UseShellExecute가 true(Default)인 경우엔 일부 XP(다음 계정으로 실행 - 현재 사용자(줄기반2\햇살반이라고 표시됨)에서
		/// To run this application, you first must install one of the following versions of the .NET Framework: v4.0.30319
		/// 에러가 발생하는 경우 있어 해결하기 위함.
		/// </summary>
		/// <param name="FileName"></param>
		/// <param name="Arguments"></param>
		/// <returns></returns>
		public static int StartExecutableWithoutShellExecute(string FileName, string Arguments, bool WaitForExit)
		{
			ProcessStartInfo psi = new ProcessStartInfo(FileName, Arguments);
			psi.UseShellExecute = false;
			Process p = Process.Start(psi);

			if (!WaitForExit)
				return 0;

			p.WaitForExit();
			int ExitCode = p.ExitCode;
			return ExitCode;
		}

        /// <summary>
        /// Run cmd.exe invisble and run <paramref name="Command"/> and quit immediately
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static int StartCmdAndRunCommandAndQuit(string Command, bool WaitForExit)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.FileName = "cmd.exe";
            //"/C" is for quit after running
            psi.Arguments = "/C " + Command;
            Process p = Process.Start(psi);

            if (!WaitForExit)
                return 0;

            p.WaitForExit();
            int ExitCode = p.ExitCode;
            return ExitCode;
        }

		//http://stackoverflow.com/questions/9501771/how-to-avoid-a-win32-exception-when-accessing-process-mainmodule-filename-in-c
		//MainModule Access 에러 나는 경우 있어 사용
		public static string GetExecutablePathByProcessId(int ProcessId)
		{
			string wmiQueryString = "select ProcessId, ExecutablePath from Win32_Process where ProcessId = " + ProcessId;
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQueryString))
			{
				using (ManagementObjectCollection results = searcher.Get())
				{
					ManagementObject mo = results.OfType<ManagementObject>().FirstOrDefault();
					if (mo != null)
					{
						return (string)mo["ExecutablePath"];
					}
				}
			}

			return null;
		}
		public static int GetProcessIdByImageName(string ImageName)
		{
			string WmiQueryString = "select ProcessId, Name from Win32_Process where Name = '" + ImageName + "'";
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(WmiQueryString))
			{
				using (ManagementObjectCollection results = searcher.Get())
				{
					ManagementObject mo = results.OfType<ManagementObject>().FirstOrDefault();
					if (mo != null)
					{
						return Convert.ToInt32(mo["ProcessId"]);
					}
				}
			}

			return 0;
		}


		//Console.WriteLine(System.DateTime.Now.ToShortDateString());
		//Process.Start(@"DATE 2001-1-1");
		//Process p = Process.Start(@"C:\Program Files\Microsoft Visual Studio .NET 2003\Common7\IDE\devenv.exe");
		//p.WaitForInputIdle(3000);

		/*
'--추가코드
''Shell()함수에서 얻어온 프로세서ID를 OpenProcess()의
''dwProcessid 인수에 넘기면 dwDesiredaccess 인수에 지정한
''Flag값에 따른 정보를 리턴함.
'Private Declare Function OpenProcess Lib "kernel32" (ByVal dwDesiredaccess&, ByVal bInherithandle&, ByVal dwProcessid&) As Long
'
''GetExitCodeProcess()는 hProcess 인수에 해당하는 프로세서가
''종료되면 lpexitcode 인수는 STILL_ACTIVE(&H103)가 아닌 값을
''가짐.
'Private Declare Function GetExitCodeProcess Lib "kernel32" (ByVal hProcess As Long, lpexitcode As Long) As Long
	
	Dim HProc As Long
	Dim lExit As Long
	Dim StartTime As Long
	Dim CInfo As New CInfo
	Const STILL_ACTIVE = &H103
	Const PROCESS_QUERY_INFORMATION = &H400
	Const PORCESS_TERMINATE = &H1


	MaxSeconds = MaxSeconds * 1000
	
	If CInfo.IsWin32NTOrHigher() Then
		HProc = OpenProcess(PROCESS_QUERY_INFORMATION Or PORCESS_TERMINATE, False, ProcID)
	Else
		HProc = OpenProcess(PROCESS_QUERY_INFORMATION, False, ProcID)
	End If
	
	StartTime = GetTickCount()
	Do
		GetExitCodeProcess HProc, lExit
		DoEvents
		If (lExit <> STILL_ACTIVE) Then
			WaitUntilExitProc = True
			Exit Function
		End If
		If (GetTickCount() - StartTime) > MaxSeconds Then
			WaitUntilExitProc = False
			Exit Function
		End If
	Loop
End Function
*/
	}
}
