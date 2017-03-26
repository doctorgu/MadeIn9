using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Collections.Specialized;
using System.Text.RegularExpressions;


namespace DoctorGu
{
	public class CCscOptions
	{
		/// <summary>Wildcard 지원함(예: *.cs)</summary>
		public string CodeFullPath;
		public string[] ReferenceFullPathList;
		public string OutFullPath;
		public NameValueCollection ResourceFullPathAndIdentifier;

		public NameValueCollection RegionNameAndValue;
	}

	public class CCsc
	{
		public static void Compile(CCscOptions Options)
		{
			string CodeFullPath = Options.CodeFullPath;

			if (!CCollection.IsNullOr0Count(Options.RegionNameAndValue))
			{
				string Code = CFile.GetTextInFile(CodeFullPath);

				foreach (string Key in Options.RegionNameAndValue.AllKeys)
				{
					string Pattern = @"\#region\s" + Key + @"\s+(?<Value>.+)\n\s+\#endregion\s" + Key;
					Regex r = new Regex(Pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
					Code = r.Replace(Code, Options.RegionNameAndValue[Key]);
				}

				CodeFullPath = CFile.GetTempFileName(".cs");
				CFile.WriteTextToFile(CodeFullPath, Code);
			}

			string FullPathCsc = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "csc.exe");

			string Arguments = "";

			if (!CCollection.IsNullOr0Count<string>(Options.ReferenceFullPathList))
			{
				Arguments += " /reference:";

				string FullPathList = "";
				foreach (string FullPathCur in Options.ReferenceFullPathList)
				{
					FullPathList += ";\"" + FullPathCur + "\"";
				}
				FullPathList = FullPathList.Substring(1);

				Arguments += FullPathList;
			}

			if (!CCollection.IsNullOr0Count(Options.ResourceFullPathAndIdentifier))
			{
				foreach (string FullPathCur in Options.ResourceFullPathAndIdentifier)
				{
					Arguments += @" /resource:""" + FullPathCur + @"""";

					string Identifier = Options.ResourceFullPathAndIdentifier[FullPathCur];
					if (!string.IsNullOrEmpty(Identifier))
					{
						Arguments += "," + Identifier;
					}
				}
			}

			Arguments += " /out:\"" + Options.OutFullPath + "\"";

			Arguments += " \"" + CodeFullPath + "\"";

			Process p = Process.Start("\"" + FullPathCsc + "\"", Arguments);
		}
	}
}
