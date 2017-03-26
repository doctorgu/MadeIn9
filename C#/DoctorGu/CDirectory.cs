using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DoctorGu
{
	public class CDirectory
	{
		/// <summary>
		/// Pattern(*, ? 와일드카드 포함)에 해당하는 모든 폴더명을 리턴함. 단, 시스템 폴더는 제외됨.
		/// </summary>
		/// <param name="StartFolder">찾기 시작할 폴더명</param>
		/// <param name="Pattern">*, ? 등을 포함하는 표현식(예:My*를 입력하면 My로 시작하는 모든 폴더를 찾음.</param>
		/// <param name="IsIncludeSubFolder">Folder 폴더 안에 하위 폴더가 존재할 경우 하위 폴더 안의 폴더까지 찾는 지 여부.</param>
		/// <param name="IsIncludeStartFolder">리턴되는 결과에 <paramref name="StartFolder"/>도 포함할 지 여부</param>
		/// <returns>찾아진 모든 폴더에 해당하는 DirectoryInfo 배열</returns>
		/// <example>
		/// 다음은 C:\Windows 폴더와 C:\Windows 폴더의 하위 폴더에서 win으로 시작하는 모든 폴더명을 출력합니다.
		/// IsIncludeStartFolder가 true이므로 C:\Windows 폴더도 포함됩니다.
		/// <code>
		/// bool IsIncludeSubFolder = true;
		/// bool IsIncludeStartFolder = true;
		/// DirectoryInfo[] adi = CFile.FindFolders(@"C:\Windows", "win*", IsIncludeSubFolder, IsIncludeStartFolder);
		/// 
		/// foreach (DirectoryInfo di in adi)
		/// {
		///	 Console.WriteLine(di.FullName);
		/// }
		/// </code>
		/// </example>
		public static DirectoryInfo[] FindFolders(string StartFolder, string Pattern,
			bool IsIncludeSubFolder, bool IsIncludeStartFolder)
		{
			List<DirectoryInfo> aFolders = new List<DirectoryInfo>();

			if (IsIncludeStartFolder)
			{
				aFolders.Add(new DirectoryInfo(StartFolder));
			}

			if (IsIncludeSubFolder)
			{
				GetAllFolders(new DirectoryInfo(StartFolder), Pattern, ref aFolders);
			}
			else
			{
				DirectoryInfo di = new DirectoryInfo(StartFolder);
				foreach (DirectoryInfo d in di.GetDirectories(Pattern, SearchOption.TopDirectoryOnly))
				{
					//루트의 System Volume Information 폴더를 포함시키면
					//Access to the path 'D:\System Volume Information' is denied. 에러가 발생하는 경우 무시하기 위함.
					if ((d.Attributes & FileAttributes.System) == FileAttributes.System)
						continue;

					aFolders.Add(d);
				}
			}

			return aFolders.ToArray();
		}

		/// <summary>
		/// Pattern에 해당하는 현재 폴더와 하위폴더의 폴더명을 배열에 저장함.
		/// </summary>
		/// <param name="di">루트 경로</param>
		/// <param name="Pattern">*, ? 등을 포함하는 표현식</param>
		/// <param name="aFolders">폴더명을 항목으로 하는 배열</param>
		private static void GetAllFolders(DirectoryInfo di, string Pattern, ref List<DirectoryInfo> aFolders)
		{
			List<string> aDirSys = new List<string>();

			foreach (DirectoryInfo d1 in di.GetDirectories(Pattern))
			{
				//루트의 System Volume Information 폴더를 포함시키면
				//Access to the path 'D:\System Volume Information' is denied. 에러가 발생하는 경우 무시하기 위함.
				if ((d1.Attributes & FileAttributes.System) == FileAttributes.System)
				{
					aDirSys.Add(d1.Name);
					continue;
				}

				aFolders.Add(d1);
			}

			//Pattern에 해당하는 폴더의 하위폴더를 찾게 되면
			//빠뜨려지는 게 있으므로 하위 폴더에 대해 재귀호출을 할 때는
			//모든 폴더의 하위폴더를 대상으로 해야 함.
			foreach (DirectoryInfo d2 in di.GetDirectories())
			{
				if (aDirSys.IndexOf(d2.Name) != -1)
					continue;

				GetAllFolders(d2, Pattern, ref aFolders);
			}
		}

		public static bool Exists(string PathHasPattern)
		{
			string LastPath = CPath.GetLastFolder(PathHasPattern);
			if ((LastPath.IndexOf('*') != -1)
				|| (LastPath.IndexOf('?') != -1))
			{
				string ParentPath = CPath.GetParentPath(PathHasPattern);
				if (!Directory.Exists(ParentPath))
					return false;

				DirectoryInfo di = new DirectoryInfo(ParentPath);
				
				DirectoryInfo[] adi = di.GetDirectories(LastPath, SearchOption.TopDirectoryOnly);
				return (adi.Length > 0);
			}
			else
			{
				return Directory.Exists(PathHasPattern);
			}
		}

		public static void Delete(string PathHasPattern, bool Recursive)
		{
			string LastPath = CPath.GetLastFolder(PathHasPattern);
			if ((LastPath.IndexOf('*') != -1)
				|| (LastPath.IndexOf('?') != -1))
			{
				string ParentPath = CPath.GetParentPath(PathHasPattern);
				if (!Directory.Exists(ParentPath))
					throw new FileNotFoundException(ParentPath + " Not exists", ParentPath);

				DirectoryInfo di = new DirectoryInfo(ParentPath);

				foreach (DirectoryInfo dSub in di.GetDirectories(LastPath, SearchOption.TopDirectoryOnly))
				{
					dSub.Delete(Recursive);
				}
			}
			else
			{
				Directory.Delete(PathHasPattern, Recursive);
			}
		}
		public static void Delete(string PathHasPattern)
		{
			Delete(PathHasPattern, false);
		}
	}
}
