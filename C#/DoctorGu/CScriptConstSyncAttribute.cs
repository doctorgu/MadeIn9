using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace DoctorGu
{
	[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum)]
	public class CScriptConstSyncAttribute : Attribute
	{
		public enum SyncResults
		{
			[Description("변경없음")]
			NoChange,
			[Description("수정")]
			Update,
			[Description("추가")]
			Append
		}

		public class CSyncReturn
		{
			public SyncResults SyncResult;
			public string FullPathOfJavaScript;
			public string FullPathOfJavaScriptBackup;
		}

		private string _FullPathOfJavaScript;
		public string FullPathOfJavaScript
		{
			get { return _FullPathOfJavaScript; }
		}

		public CScriptConstSyncAttribute(string FullPathOfJavaScript)
		{
			_FullPathOfJavaScript = FullPathOfJavaScript;
		}

		public static CScriptConstSyncAttribute GetAttribute(Type T)
		{
			CScriptConstSyncAttribute[] attributes = (CScriptConstSyncAttribute[])T.GetCustomAttributes(typeof(CScriptConstSyncAttribute), false);
			return attributes[0];
		}

		public static CSyncReturn Syncronize(Type T)
		{
			CScriptConstSyncAttribute[] attributes = (CScriptConstSyncAttribute[])T.GetCustomAttributes(typeof(CScriptConstSyncAttribute), false);

			string FullPath = attributes[0].FullPathOfJavaScript;
			if (FullPath.StartsWith("/"))
				FullPath = HttpContext.Current.Server.MapPath(FullPath);

			if (!File.Exists(FullPath))
				throw new Exception(string.Format("{0} is not exists.", FullPath));

			string JsGenerated = ConvertToJavaScript(T, 0);

			string FileContent = CFile.GetTextInFile(FullPath);
			string LogDateTime = DateTime.Now.ToString(CConst.Format_yyyyMMddHHmmss);
			string FullPathBackup = GetFullPathBackup(FullPath, LogDateTime);

			string Declare = "var " + T.Name + " ";

			SyncResults SyncResult = SyncResults.NoChange;

			string JsAlready = GetJavaScriptByDeclare(FileContent, Declare);
			if (!string.IsNullOrEmpty(JsAlready))
			{
				if (CFindRep.TrimWhiteSpace(JsGenerated) != CFindRep.TrimWhiteSpace(JsAlready))
				{
					SyncResult = SyncResults.Update;
				}
			}
			else
			{
				SyncResult = SyncResults.Append;
			}

			if (SyncResult != SyncResults.NoChange)
			{
				string Line = string.Format("//{0} {1} by CScriptConstSyncAttribute", LogDateTime, SyncResult);

				string FileContentNew = "";
				if (SyncResult == SyncResults.Update)
				{
					FileContentNew = FileContent.Replace(JsAlready, Line + "\r\n" + JsGenerated);
				}
				else if (SyncResult == SyncResults.Append)
				{
					FileContentNew = FileContent + "\r\n" + Line + "\r\n" + JsGenerated;
				}

				CFile.WriteTextToFile(FullPath, FileContentNew);
				CFile.WriteTextToFile(FullPathBackup, FileContent);
			}

			return new CSyncReturn()
			{
				SyncResult = SyncResult,
				FullPathOfJavaScript = FullPath,
				FullPathOfJavaScriptBackup = FullPathBackup
			};
		}

		private static string GetFullPathBackup(string FullPath, string LogDateTime)
		{
			string Folder = Path.GetDirectoryName(FullPath);
			string Name = Path.GetFileName(FullPath);
			return Path.Combine(Folder, Name + "." + LogDateTime + ".exclude");
		}

		private static string GetJavaScriptByDeclare(string FileContent, string Declare)
		{
			int IndexDeclare = FileContent.IndexOf(Declare);
			if (IndexDeclare == -1)
				return null;

			int IndexOpen = FileContent.IndexOf('{', IndexDeclare);
			int IndexClose = -1;

			//첫번째 여는 괄호에 대해 1을 증가시킴.
			int nPar = 1;

			//여는 괄호가 나오면 1 더하고 닫는 괄호가 나오면 1 빼서
			//0이 되면 첫번째 여는 괄호에 대해 닫힌 괄호를 찾은 것임.
			for (int i = (IndexOpen + 1); i < FileContent.Length; i++)
			{
				if (FileContent[i] == '{')
					nPar++;
				else if (FileContent[i] == '}')
				{
					nPar--;
				}

				if (nPar == 0)
				{
					IndexClose = i;
					break;
				}
			}

			if (IndexClose == -1)
				return null;

			if (((IndexClose + 1) <= FileContent.Length) && (FileContent[IndexClose + 1] == ';'))
			{
				IndexClose++;
			}

			return FileContent.Substring(IndexDeclare, IndexClose - IndexDeclare + 1);
		}

		private static string ConvertToJavaScript(Type T, int Depth)
		{
			string TmpAll = "";

			if (Depth == 0)
			{
				TmpAll =
@"var {{Name}} =
{
{{NameValue}}
};";
			}
			else
			{
				TmpAll =
@",
{{Tab}}{{Name}}:
{{Tab}}{
{{NameValue}}
{{Tab}}}";
			}

			string TmpNameValue = ",\r\n{{Tab}}\t{{Name}}: {{Value}}";
			string NameValue = "";

			MemberInfo[] aMi = T.GetMembers(BindingFlags.Public | BindingFlags.Static);
			foreach (MemberInfo mi in aMi)
			{
				if (mi.MemberType == MemberTypes.Field)
				{
					FieldInfo fi = T.GetField(mi.Name);
					Type FieldType = fi.FieldType;

					string Name = fi.Name;
					string Value = "";

					if (fi.IsLiteral)
					{
						object ConstValue = fi.GetRawConstantValue();

						if (FieldType.IsEnum)
						{
							DescriptionAttribute[] attributes =
							  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

							string Desc = (attributes.Length > 0) ? attributes[0].Description : null;

							if (!string.IsNullOrEmpty(Desc))
								Value = string.Format("new CVnd(\"{0}\", \"{1}\", \"{2}\")", ConstValue, Name, Desc);
							else
								Value = ConstValue.ToString();
						}
						else
						{
							if (FieldType == typeof(char))
								Value = string.Format("'{0}'", ConstValue);
							else if (FieldType == typeof(string))
								Value = string.Format("\"{0}\"", ConstValue);
							else
								Value = ConstValue.ToString();
						}
					}
					else if (fi.IsInitOnly)
					{
						if (FieldType == typeof(string[]))
						{
							string[] aValue = (string[])fi.GetValue(null);
							Value = "[ \"" + string.Join("\", \"", aValue) + "\" ]";
						}
					}

					NameValue += TmpNameValue
						.Replace("{{Tab}}", CFindRep.Repeat('\t', Depth))
						.Replace("{{Name}}", Name)
						.Replace("{{Value}}", Value.ToString());
				}
				else if (mi.MemberType == MemberTypes.NestedType)
				{
					Type Typ = T.GetNestedType(mi.Name);
					NameValue += ConvertToJavaScript(Typ, Depth + 1);
				}
			}

			if (!string.IsNullOrEmpty(NameValue))
				NameValue = NameValue.Substring(",\r\n".Length);

			string s = TmpAll
				.Replace("{{Tab}}", CFindRep.Repeat('\t', Depth))
				.Replace("{{Name}}", T.Name)
				.Replace("{{NameValue}}", NameValue);

			return s;
		}

	}

}
