using System;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace DoctorGu
{
	public enum HRootKeys
	{
		HKEY_CLASSES_ROOT,
		HKEY_CURRENT_CONFIG,
		HKEY_CURRENT_USER,
		HKEY_PERFORMANCE_DATA,
		HKEY_LOCAL_MACHINE,
		HKEY_USERS
	}

	public enum HwpVersion
	{
		_None_0 = 0,
		_2002_5 = 5,
		_2005_6 = 6,
		_2007_7 = 7,
		_2010_8 = 8,
	}

	/*
11001 (0x2AF9) Internet Explorer 11. Webpages are displayed in IE11 edge mode, regardless of the !DOCTYPE directive.  
11000 (0x2AF8) IE11. Webpages containing standards-based !DOCTYPE directives are displayed in IE11 edge mode. Default value for IE11. 

10001 (0x2711)	Internet Explorer 10. Webpages are displayed in IE10 Standards mode, regardless of the !DOCTYPE directive.
10000 (0x02710)	Internet Explorer 10. Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode. Default value for Internet Explorer 10.

9999 (0x270F)	Windows Internet Explorer 9. Webpages are displayed in IE9 Standards mode, regardless of the !DOCTYPE directive.
9000 (0x2328)	Internet Explorer 9. Webpages containing standards-based !DOCTYPE directives are displayed in IE9 mode. Default value for Internet Explorer 9.
Important  In Internet Explorer 10, Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.

8888 (0x22B8)	Webpages are displayed in IE8 Standards mode, regardless of the !DOCTYPE directive.
8000 (0x1F40)	Webpages containing standards-based !DOCTYPE directives are displayed in IE8 mode. Default value for Internet Explorer 8
Important  In Internet Explorer 10, Webpages containing standards-based !DOCTYPE directives are displayed in IE10 Standards mode.

7000 (0x1B58)	Webpages containing standards-based !DOCTYPE directives are displayed in IE7 Standards mode. Default value for applications hosting the WebBrowser Control.*/
	public enum BrowserEmulation
	{
		_11IgnoreDoctype = 11001,
		_11Doctype = 11000,
		_10IgnoreDoctype = 10001,
		_10Doctype = 10000,
		_09IgnoreDoctype = 9999,
		_09Doctype = 9000,
		_08IgnoreDoctype = 8888,
		_08Doctype = 8000,
		_07Doctype = 7000,
	}

	public struct IePageSetups
	{
		public MarginF MarginInche;
		public string Header;
		public string Footer;
		public string Font;
		public bool PrintBackground;
		public bool ShrinkToFit;
	}

	/// <summary>
	/// 레지스트리와 관련된 기능 구현
	/// </summary>
	public class CRegistry
	{
		public static void SaveSetting(HRootKeys RootKey, string Section,
			string Name, object Value, RegistryValueKind Kind)
		{
			RegistryKey KeyRoot = GetRootKey(RootKey);
			RegistryKey KeySub = KeyRoot.OpenSubKey(Section, true);
			if (KeySub == null)
			{
				KeySub = KeyRoot.CreateSubKey(Section);
			}

			KeySub.SetValue(Name, Value, Kind);
			KeySub.Close();
		}
		/// <summary>
		/// HKEY_CURRENT_USER\Software\DoctorGu 경로 안에 값을 설정함.
		/// </summary>
		/// <param name="Section"></param>
		/// <param name="Name"></param>
		/// <param name="Value"></param>
		public static void SaveSetting(string Section, string Name, object Value)
		{
			string SectionNew = @"Software\DoctorGu\" + Section.TrimStart('\\');

			SaveSetting(HRootKeys.HKEY_CURRENT_USER, SectionNew, Name, Value, RegistryValueKind.String);
		}

		public static object GetSetting(HRootKeys RootKey, string Section,
			string Name, object Default)
		{
			RegistryKey KeyRoot = GetRootKey(RootKey);
			RegistryKey KeySub = KeyRoot.OpenSubKey(Section, false);
			if (KeySub == null)
				return Default;

			object Value = KeySub.GetValue(Name, Default);
			KeySub.Close();

			return Value;
		}
		public static object GetSetting(string Section, string Name, object Default)
		{
			string SectionNew = @"Software\DoctorGu\" + Section.TrimStart('\\');

			return GetSetting(HRootKeys.HKEY_CURRENT_USER, SectionNew, Name, Default);
		}
		public static object GetSetting(string Section, string Name)
		{
			return GetSetting(Section, Name, "");
		}

		public static void SaveSettingVb6(string AppName, string Section, string Key, string Setting)
		{
			RegistryKey VbKey = GetVbKey();

			RegistryKey KeySub = VbKey.OpenSubKey(AppName + "\\" + Section, true);
			if (KeySub == null)
			{
				KeySub = VbKey.CreateSubKey(AppName + "\\" + Section);
			}

			KeySub.SetValue(Key, Setting);
			KeySub.Close();
		}
		public static string GetSettingVb6(string AppName, string Section, string Key, string Default)
		{
			RegistryKey VbKey = GetVbKey();

			RegistryKey KeySub = VbKey.OpenSubKey(AppName + "\\" + Section, false);
			if (KeySub == null) return Default;

			object Value = KeySub.GetValue(Key, Default);
			KeySub.Close();

			return Value.ToString();
		}

		public static void DeleteSetting(HRootKeys RootKey, string Section, bool IsTree)
		{
			RegistryKey KeyRoot = GetRootKey(RootKey);

			if (KeyRoot.OpenSubKey(Section) == null)
				return;

			if (IsTree)
				KeyRoot.DeleteSubKeyTree(Section);
			else
				KeyRoot.DeleteSubKey(Section);
		}
		public static void DeleteSetting(HRootKeys RootKey, string Section)
		{
			bool IsTree = false;
			DeleteSetting(RootKey, Section, IsTree);
		}
		public static void DeleteSetting(string Section)
		{
			string SectionNew = @"Software\DoctorGu\" + Section.TrimStart('\\');
			bool IsTree = false;
			DeleteSetting(HRootKeys.HKEY_CURRENT_USER, SectionNew, IsTree);
		}

		public static void DeleteSetting(HRootKeys RootKey, string Section, string Name)
		{
			RegistryKey KeyRoot = GetRootKey(RootKey);

			using (RegistryKey KeySub = KeyRoot.OpenSubKey(Section, true))
			{
				if (KeySub == null)
					return;

				if (KeySub.GetValue(Name) != null)
					KeySub.DeleteValue(Name);
			}
		}
		public static void DeleteSetting(string Section, string Name)
		{
			string SectionNew = @"Software\DoctorGu\" + Section.TrimStart('\\');
			DeleteSetting(HRootKeys.HKEY_CURRENT_USER, SectionNew, Name);
		}

		private static RegistryKey GetRootKey(HRootKeys RootKey)
		{
			RegistryKey KeyRoot = null;
			switch (RootKey)
			{
				case HRootKeys.HKEY_CLASSES_ROOT:
					KeyRoot = Registry.ClassesRoot;
					break;
				case HRootKeys.HKEY_CURRENT_CONFIG:
					KeyRoot = Registry.CurrentConfig;
					break;
				case HRootKeys.HKEY_CURRENT_USER:
					KeyRoot = Registry.CurrentUser;
					break;
				case HRootKeys.HKEY_PERFORMANCE_DATA:
					KeyRoot = Registry.PerformanceData;
					break;
				case HRootKeys.HKEY_LOCAL_MACHINE:
					KeyRoot = Registry.LocalMachine;
					break;
				case HRootKeys.HKEY_USERS:
					KeyRoot = Registry.Users;
					break;
			}

			return KeyRoot;
		}
		private static RegistryKey GetDefaultKey()
		{
			RegistryKey DefaultKey = Registry.CurrentUser.OpenSubKey(@"Software\DoctorGu", true);
			if (DefaultKey == null)
			{
				DefaultKey = Registry.CurrentUser.CreateSubKey(@"Software\DoctorGu");
			}

			return DefaultKey;
		}
		private static RegistryKey GetVbKey()
		{
			RegistryKey VbKey = Registry.CurrentUser.OpenSubKey(@"Software\VB and VBA Program Settings");
			return VbKey;
		}

		public static string GetAutoStartFullPath(string AppName)
		{
			string Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
			RegistryKey rk = Registry.LocalMachine.OpenSubKey(Path, true);
			if (rk == null)
			{
				rk = Registry.LocalMachine.CreateSubKey(Path);
			}
			return (string)rk.GetValue(AppName, "");
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="AppName"></param>
		/// <param name="PathFile"></param>
		/// <example>
		/// string AutoStartFullPathExact = Path.GetDirectoryName(Application.ExecutablePath) 
		///	 + "\\AutoUpdate.exe /UpdateUrl http://10.65.21.1/SQLGame /MainProgramFullPath SQLGame.exe";
		/// string AutoStartFullPath = CRegistry.GetAutoStartFullPath(Application.ProductName);
		/// if (AutoStartFullPath != AutoStartFullPathExact)
		/// {
		///		CRegistry.RegisterAsAutoStart(Application.ProductName, AutoStartFullPathExact);
		/// }
		/// </example>
		public static void RegisterAsAutoStart(string AppName, string PathFile)
		{
			string Path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
			RegistryKey rk = Registry.LocalMachine.OpenSubKey(Path, true);
			if (rk == null)
			{
				rk = Registry.LocalMachine.CreateSubKey(Path);
			}
			rk.SetValue(AppName, PathFile);
			rk.Close();
		}

		public static string GetWinwordFullPath()
		{
			string Section = @"Applications\Winword.exe\shell\edit\command";
			object oFullPath = CRegistry.GetSetting(HRootKeys.HKEY_CLASSES_ROOT, Section, "", "");
			if (oFullPath == null)
				return "";

			string FullPath = GetExeFullPath((string)oFullPath);

			return FullPath;
		}

		/// <summary>
		/// 아래한글 버전 중 가장 높은 버전을 리턴함.
		/// </summary>
		/// <returns></returns>
		public static HwpVersion GetHwpVersionHighest()
		{
			HwpVersion Ver = HwpVersion._None_0;

			int Max = (int)HwpVersion._2010_8;
			int Min = (int)HwpVersion._2002_5;

			for (int i = Max; i >= Min; i--)
			{
				string FullPath = GetHwpFullPath((HwpVersion)i);
				if (!string.IsNullOrEmpty(FullPath))
					return (HwpVersion)i;
			}

			return Ver;
		}
		public static string GetHwpFullPathHighest(HwpVersion Version)
		{
			int Max = (Version == 0) ? (int)HwpVersion._2010_8 : (int)Version;
			int Min = (Version == 0) ? (int)HwpVersion._2002_5 - 1 : (int)Version;

			for (int i = Max; i >= Min; i--)
			{
				string FullPath = "";

				List<string> aSectionCur = new List<string>();
				if (i == Min)
				{
					FullPath = GetHwpFullPath(HwpVersion._None_0);
				}
				else
				{
					FullPath = GetHwpFullPath((HwpVersion)i);
				}

				if (!string.IsNullOrEmpty(FullPath))
					return FullPath;
			}

			return "";
		}
		public static string GetHwpFullPath()
		{
			return GetHwpFullPathHighest(HwpVersion._None_0);
		}
		private static string GetHwpFullPath(HwpVersion Version)
		{
			List<string> aSectionCur = new List<string>();

			if (Version == HwpVersion._None_0)
			{
				//버전은 알 수 없어도 한글 파일 경로를 리턴하기 위함.
				aSectionCur.Add(@"Hwp.Document\shell\Open\command");
			}
			else
			{
				aSectionCur.Add(@"Hwp.Document." + ((int)Version).ToString() + @"\shell\Open\command");
				aSectionCur.Add(@"Hwp.Document." + ((int)Version).ToString() + @"\shell\DefaultIcon");
				aSectionCur.Add(@"Hwp.Document." + ((int)Version).ToString() + @"\DefaultIcon");
			}

			for (int j = 0; j < aSectionCur.Count; j++)
			{
				object oFullPathCur = CRegistry.GetSetting(HRootKeys.HKEY_CLASSES_ROOT, aSectionCur[j], "", null);
				if (oFullPathCur == null)
					continue;

				string FullPathCur = GetExeFullPath((string)oFullPathCur);

				//HwpView.exe인 경우도 있음.
				if (FullPathCur.IndexOf("Hwp.exe", StringComparison.CurrentCultureIgnoreCase) == -1)
					continue;

				if (File.Exists(FullPathCur))
				{
					return FullPathCur;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// "C:\Test\Test.exe %1" -> C:\Test\Test.exe
		/// </summary>
		/// <param name="FullPath"></param>
		/// <returns></returns>
		private static string GetExeFullPath(string FullPath)
		{
			int PosExe = FullPath.IndexOf(".exe", StringComparison.CurrentCultureIgnoreCase);
			if (PosExe == -1)
				return "";

			int PosLast = PosExe + ".exe".Length - 1;

			FullPath = FullPath.Substring(0, PosLast + 1);
			if (FullPath.StartsWith("\""))
				FullPath = FullPath.Substring(1);

			return FullPath;
		}

		/// <summary>
		/// scrrun.dll이 등록되지 않은 컴퓨터가 있는 경우, 강제로 등록함.
		/// </summary>
		/// <param name="ErrMsgIs"></param>
		/// <returns></returns>
		public static void RegisterScriptingRuntime()
		{
			try
			{
				Type t = Type.GetTypeFromProgID("Scripting.FileSystemObject");
			}
			catch (Exception)
			{
				string System32Folder = Environment.GetFolderPath(Environment.SpecialFolder.System);
				string FullPath = Path.Combine(System32Folder, "scrrun.dll");
				CRegComponent.Register(FullPath);
			}
		}

		public static int GetIeVersionMajor()
		{
			// HKLM\SOFTWARE\Microsoft\Internet Explorer\Version
			string Section = @"SOFTWARE\Microsoft\Internet Explorer";
			object oVersion = CRegistry.GetSetting(HRootKeys.HKEY_LOCAL_MACHINE, Section, "Version", null);
			if (oVersion == null)
				return 0;

			string[] aVersion = ((string)oVersion).Split('.');
			int Version = Convert.ToInt32(aVersion[0]);

			//10 버전이면 Version은 9로 시작하고 svcVersion이 10으로 시작함.
			if (Version >= 9)
			{
				oVersion = CRegistry.GetSetting(HRootKeys.HKEY_LOCAL_MACHINE, Section, "svcVersion", null);
				if (oVersion == null)
					return Version;

				aVersion = ((string)oVersion).Split('.');
				Version = Convert.ToInt32(aVersion[0]);
			}

			return Version;
		}

		//http://stackoverflow.com/questions/17922308/use-latest-version-of-ie-in-webbrowser-control
		public static void SetWebBrowserControlVersion(string ProcessNameWithExtension, int IeVersionMajor)
		{
			int nBrowserEmulationNew = (int)BrowserEmulation._07Doctype;
			if (IeVersionMajor >= 11)
				nBrowserEmulationNew = (int)BrowserEmulation._11IgnoreDoctype;
			else if (IeVersionMajor >= 10)
				nBrowserEmulationNew = (int)BrowserEmulation._10IgnoreDoctype;
			else if (IeVersionMajor >= 9)
				nBrowserEmulationNew = (int)BrowserEmulation._09IgnoreDoctype;
			else if (IeVersionMajor >= 8)
				nBrowserEmulationNew = (int)BrowserEmulation._08IgnoreDoctype;
			else if (IeVersionMajor >= 7)
				nBrowserEmulationNew = (int)BrowserEmulation._07Doctype;

			//HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION
			//(When 64bit OS, the value will save to HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\... node.)
			string Section = @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION";
			object oValue = CRegistry.GetSetting(HRootKeys.HKEY_LOCAL_MACHINE, Section, ProcessNameWithExtension, (int)BrowserEmulation._07Doctype);
			int nBrowserEmulationOld = Convert.ToInt32(oValue);

			if (nBrowserEmulationNew != nBrowserEmulationOld)
				CRegistry.SaveSetting(HRootKeys.HKEY_LOCAL_MACHINE, Section, ProcessNameWithExtension, nBrowserEmulationNew, RegistryValueKind.DWord);
		}

		/// <summary>
		/// 특정 확장자와 특정 파일을 연결시키거나 연결을 해제함.
		/// </summary>
		private static void LinkOrNotExtensionWithExecutable(string Extension, string ExecutableName, string ExecutableFullPath, bool Unlink)
		{
			//.dr 확장자 가진 파일을 C:\My\DoctorGu.exe와 연결시키기

			HRootKeys[] aRootKey = new HRootKeys[] { HRootKeys.HKEY_CURRENT_USER, HRootKeys.HKEY_CLASSES_ROOT };
			string[] aSectionRoot = new string[] { @"SOFTWARE\Classes\", "" };

			for (int i = 0; i < aRootKey.Length; i++)
			{
				HRootKeys RootKey = aRootKey[i];
				string SectionRoot = aSectionRoot[i];

				//[HKEY_CLASSES_ROOT\.dr]
				//@="DoctorGu"
				if (!Unlink)
					SaveSetting(RootKey, SectionRoot + Extension, "", ExecutableName, RegistryValueKind.String);
				else
					DeleteSetting(RootKey, SectionRoot + Extension);

				//[HKEY_CLASSES_ROOT\DoctorGu]
				//@="DoctorGu File"
				//
				//[HKEY_CLASSES_ROOT\DoctorGu\shell\open\command]
				//@="C:\\My\\DoctorGu.exe \"%1\""
				{
					string Section1 = SectionRoot + ExecutableName;
					string Section2 = SectionRoot + ExecutableName + @"\shell\open\command";

					if (!Unlink)
					{
						SaveSetting(RootKey, Section1, "", ExecutableName + " File", RegistryValueKind.String);
						SaveSetting(RootKey, Section2, "", string.Format(@"{0} ""%1""", ExecutableFullPath), RegistryValueKind.String);
					}
					else
					{
						DeleteSetting(RootKey, Section1, true);
					}
				}
			}
		}
		public static void UnlinkExtensionWithExecutable(string Extension, string ExecutableName, string ExecutableFullPath)
		{
			bool Unlink = true;
			LinkOrNotExtensionWithExecutable(Extension, ExecutableName, ExecutableFullPath, Unlink);
		}
		public static void LinkExtensionWithExecutable(string Extension, string ExecutableName, string ExecutableFullPath)
		{
			bool Unlink = false;
			LinkOrNotExtensionWithExecutable(Extension, ExecutableName, ExecutableFullPath, Unlink);
		}

		//http://support.microsoft.com/kb/236777
		private static string SectionIePageSetup = @"Software\Microsoft\Internet Explorer\PageSetup";
		public static IePageSetups IePageSetup
		{
			get
			{
				IePageSetups setup = new IePageSetups();

				const float DefaultMargin = 0.75f;

				RegistryKey KeyRoot = GetRootKey(HRootKeys.HKEY_CURRENT_USER);
				RegistryKey KeySub = KeyRoot.OpenSubKey(SectionIePageSetup, false);
				if (KeySub == null)
				{
					setup.MarginInche = new MarginF(DefaultMargin, DefaultMargin, DefaultMargin, DefaultMargin);
					setup.Header = "";
					setup.Footer = "";
					setup.Font = "";
					setup.PrintBackground = false;
					setup.ShrinkToFit = true;
				}
				else
				{
					float MarginLeft = Convert.ToSingle(KeySub.GetValue("margin_left", DefaultMargin));
					float MarginRight = Convert.ToSingle(KeySub.GetValue("margin_right", DefaultMargin));
					float MarginTop = Convert.ToSingle(KeySub.GetValue("margin_top", DefaultMargin));
					float MarginBottom = Convert.ToSingle(KeySub.GetValue("margin_bottom", DefaultMargin));

					setup.MarginInche = new MarginF(MarginLeft, MarginRight, MarginTop, MarginBottom);
					setup.Header = (string)KeySub.GetValue("header", "");
					setup.Footer = (string)KeySub.GetValue("footer", "");
					setup.Font = (string)KeySub.GetValue("font", "");
					setup.PrintBackground = ((string)KeySub.GetValue("Print_Background", "") == "yes");
					setup.ShrinkToFit = ((string)KeySub.GetValue("Shrink_To_Fit", "") == "yes");
				}

				KeySub.Close();

				return setup;
			}
			set
			{
				RegistryKey KeyRoot = GetRootKey(HRootKeys.HKEY_CURRENT_USER);
				RegistryKey KeySub = KeyRoot.OpenSubKey(SectionIePageSetup, true);
				if (KeySub == null)
				{
					KeySub = KeyRoot.CreateSubKey(SectionIePageSetup);
				}

				KeySub.SetValue("margin_left", value.MarginInche.Left, RegistryValueKind.String);
				KeySub.SetValue("margin_right", value.MarginInche.Right, RegistryValueKind.String);
				KeySub.SetValue("margin_top", value.MarginInche.Top, RegistryValueKind.String);
				KeySub.SetValue("margin_bottom", value.MarginInche.Bottom, RegistryValueKind.String);
				KeySub.SetValue("header", value.Header, RegistryValueKind.String);
				KeySub.SetValue("footer", value.Footer, RegistryValueKind.String);
				KeySub.SetValue("font", value.Font, RegistryValueKind.String);
				KeySub.SetValue("Print_Background", (value.PrintBackground ? "yes" : "no"), RegistryValueKind.String);
				KeySub.SetValue("Shrink_To_Fit", (value.ShrinkToFit ? "yes" : "no"), RegistryValueKind.String);

				KeySub.Close();
			}
		}

	}
}
