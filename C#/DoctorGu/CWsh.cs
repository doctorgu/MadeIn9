using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace DoctorGu
{
	public class CWsh
	{
		/// <summary>
		/// 특정 파일의 바로 가기를 만듦.
		/// </summary>
		/// <param name="TargetFullPath">바로 가기가 참조하는 원본 파일의 전체 경로</param>
		/// <param name="ShortcutFolder">바로 가기가 생성될 폴더 위치</param>
		/// <example>
		/// 다음은 바탕 화면에 메모장의 바로 가기를 만듭니다.
		/// <code>
		/// CFile.CreateShortcut(@"C:\Windows\Notepad.exe", @"C:\Documents and Settings\Administrator\바탕 화면");
		/// </code>
		/// </example>
		public static void CreateShortcut(string TargetFullPath, string ShortcutFolder)
		{
			string ShortcutFullPath = ShortcutFolder + "\\" + Path.GetFileNameWithoutExtension(TargetFullPath) + ".lnk";
			string IconLocation = TargetFullPath;
			string Arguments = "";
			string Description = "";

			CreateShortcutDetail(ShortcutFullPath, TargetFullPath, IconLocation, Arguments, Description);
		}
		/// <summary>
		/// 특정 파일의 바로 가기를 만듦.
		/// </summary>
		/// <param name="ShortcutFullPath">바로 가기 파일의 전체 경로</param>
		/// <param name="TargetFullPath">바로 가기가 참조하는 원본 파일의 전체 경로</param>
		/// <param name="IconLocation">바로 가기에 사용될 아이콘의 전체 경로</param>
		/// <param name="Arguments">바로 가기를 실행했을 때 원본 파일에 넘겨질 명령줄 인수</param>
		/// <param name="Description">바로 가기의 설명(풍선 도움말로 표시됨)</param>
		/// <param name="ErrMsgIs">바로 가기 생성이 실패한 경우, 에러 메세지</param>
		/// <returns>바로 가기 생성의 성공 여부</returns>
		/// <example>
		/// 다음은 바탕 화면에 메모장의 바로 가기를 만듭니다.
		/// <paramref name="Arguments"/>가 "C:\Windows\setup.log"이므로 실행하면 setup.log 파일이 열립니다.
		/// <code>
		/// string ShortcutFullPath = @"C:\Documents and Settings\Administrator\바탕 화면\메모장.lnk";
		/// 
		/// tring TargetFullPath = @"C:\Windows\Notepad.exe";
		/// string IconLocation = @"C:\Windows\Notepad.exe";
		/// string Arguments = @"C:\Windows\setup.log";
		/// string Description = "메모장-setup.log";
		/// string ErrMsgIs;
		/// CWsh.CreateShortcutDetail(ShortcutFullPath, TargetFullPath, IconLocation, Arguments, Description);
		/// Console.WriteLine(IsSuccess);
		/// </code>
		/// </example>
		public static void CreateShortcutDetail(string ShortcutFullPath,
			string TargetFullPath, string IconLocation, string Arguments, string Description)
		{
			string ShortcutFolder = Path.GetDirectoryName(ShortcutFullPath);
			if (!Directory.Exists(ShortcutFolder))
			{
				Directory.CreateDirectory(ShortcutFolder);
			}

			Type typWsh = Type.GetTypeFromProgID("WScript.Shell");
			object instWsh = Activator.CreateInstance(typWsh);

			object Shortcut = typWsh.InvokeMember(
				"CreateShortcut",
				BindingFlags.InvokeMethod | BindingFlags.Public,
				null,
				instWsh,
				new object[1] { ShortcutFullPath });

			Type typShortcut = Shortcut.GetType();
			typShortcut.InvokeMember(
				"TargetPath",
				BindingFlags.SetProperty | BindingFlags.Public,
				null,
				Shortcut,
				new object[1] { TargetFullPath });
			typShortcut.InvokeMember(
				"WorkingDirectory",
				BindingFlags.SetProperty | BindingFlags.Public,
				null,
				Shortcut,
				new object[1] { ShortcutFolder });
			typShortcut.InvokeMember(
				"IconLocation",
				BindingFlags.SetProperty | BindingFlags.Public,
				null,
				Shortcut,
				new object[1] { IconLocation });

			if (!string.IsNullOrEmpty(Arguments))
			{
				typShortcut.InvokeMember(
					"Arguments",
					BindingFlags.SetProperty | BindingFlags.Public,
					null,
					Shortcut,
					new object[1] { Arguments });
			}

			if (!string.IsNullOrEmpty(Description))
			{
				typShortcut.InvokeMember(
					"Description",
					BindingFlags.SetProperty | BindingFlags.Public,
					null,
					Shortcut,
					new object[1] { Description });
			}

			typShortcut.InvokeMember(
				"Save",
				BindingFlags.InvokeMethod | BindingFlags.Public,
				null,
				Shortcut,
				null);

			//Wsh.WshShell Shell = new Wsh.WshShellClass();
			//Wsh.WshShortcut Shortcut = (Wsh.WshShortcut)Shell.CreateShortcut(ShortcutFullPath);
			//Shortcut.TargetPath = TargetFullPath;
			//Shortcut.IconLocation = IconLocation;
			//Shortcut.Arguments = Arguments;
			//Shortcut.Description = Description;
			//Shortcut.Save();
			//Shortcut = null;
		}

		/// <summary>
		/// 단축아이콘의 아이콘을 실행파일의 아이콘으로 변경함.
		/// </summary>
		/// <param name="ExePath"></param>
		/// <param name="ExeFile"></param>
		public static void ChangeShortcutIconWithExeIcon(string ExePath, string ExeFile)
		{
			string ExeFileNoExt = CPath.GetFileNameWithoutExtension(ExeFile);

			string[] aPathFile =
				new String[]
				{
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Internet Explorer\Quick Launch",
					Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
					Environment.GetFolderPath(Environment.SpecialFolder.Programs),
					Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)
				};

			Type typWsh = Type.GetTypeFromProgID("WScript.Shell");
			object instWsh = Activator.CreateInstance(typWsh);

			for (int i = 0, i2 = aPathFile.Length; i < i2; i++)
			{
				FileInfo fi = CFile.FindFirstFile(aPathFile[i], ExeFileNoExt + ".lnk", true);
				if (fi != null)
				{
					string PathFile = (string)fi.FullName;

					object Shortcut = typWsh.InvokeMember(
						"CreateShortcut",
						BindingFlags.InvokeMethod | BindingFlags.Public,
						null,
						instWsh,
						new object[1] { PathFile });

					Type typShortcut = Shortcut.GetType();
					typShortcut.InvokeMember(
						"IconLocation",
						BindingFlags.SetProperty | BindingFlags.Public,
						null,
						Shortcut,
						new object[1] { ExePath + "\\" + ExeFile });

					typShortcut.InvokeMember(
						"Save",
						BindingFlags.InvokeMethod | BindingFlags.Public,
						null,
						Shortcut,
						null);
				}
			}
		}

		public static void CopyShortCut(string PathFileSrc, string PathFileDest, string TargetPath)
		{
			Type typWsh = Type.GetTypeFromProgID("WScript.Shell");
			object instWsh = Activator.CreateInstance(typWsh);

			object ShortcutSrc = typWsh.InvokeMember(
				"CreateShortcut",
				BindingFlags.InvokeMethod | BindingFlags.Public,
				null,
				instWsh,
				new object[1] { PathFileSrc });

			object ShortcutDest = typWsh.InvokeMember(
				"CreateShortcut",
				BindingFlags.InvokeMethod | BindingFlags.Public,
				null,
				instWsh,
				new object[1] { PathFileDest });

			Type typShortcutSrc = ShortcutSrc.GetType();
			Type typShortcutDest = ShortcutDest.GetType();

			string ArgumentsSrc = (string)typShortcutSrc.InvokeMember(
				"Arguments",
				BindingFlags.GetProperty | BindingFlags.Public,
				null,
				ShortcutSrc,
				null);
			string TargetPathSrc = (string)typShortcutSrc.InvokeMember(
				"TargetPath",
				BindingFlags.GetProperty | BindingFlags.Public,
				null,
				ShortcutSrc,
				null);
			string DescriptionSrc = (string)typShortcutSrc.InvokeMember(
				"Description",
				BindingFlags.GetProperty | BindingFlags.Public,
				null,
				ShortcutSrc,
				null);

			typShortcutDest.InvokeMember(
						"Arguments",
						BindingFlags.SetProperty | BindingFlags.Public,
						null,
						ShortcutDest,
						new object[1] { ArgumentsSrc });
			typShortcutDest.InvokeMember(
						"TargetPath",
						BindingFlags.SetProperty | BindingFlags.Public,
						null,
						ShortcutDest,
						new object[1] { TargetPathSrc });
			typShortcutDest.InvokeMember(
						"Description",
						BindingFlags.SetProperty | BindingFlags.Public,
						null,
						ShortcutDest,
						new object[1] { DescriptionSrc });
			typShortcutDest.InvokeMember(
						"Save",
						BindingFlags.InvokeMethod | BindingFlags.Public,
						null,
						ShortcutDest,
						null);
		}
	}
}
