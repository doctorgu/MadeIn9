using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace DoctorGu
{
	/// <summary>
	/// Flash의 Class 파일을 읽어 JScript 코드로 변환함.
	/// Flash에는 static 등의 JScript 코드에서는 쓸 수 없는 키워드가 있으므로 이런 것을 삭제함.
	/// </summary>
	public class CCode
	{
		/// <summary>
		/// Flash의 Class 파일을 읽어 JScript 코드로 변환해서 리턴함.
		/// </summary>
		/// <param name="PathFile">Flash 파일의 전체 경로</param>
		/// <returns>변환된 JScript 코드</returns>
		/// <example>
		/// 다음은 CSql.as 파일을 웹에서 쓸 수 있도록 JScript 파일로 변환합니다.
		/// <code>
		/// string FlashPathFile = @"D:\My\MadeIn9\Flash\DoctorGu\CSql.as";
		/// string FlashCode = CFile.GetTextInFile(FlashPathFile);
		/// Console.WriteLine(FlashCode);
		/// class CSql
		/// {
		///	 public function CSql()
		///	 {
		///	 }
		/// 
		///	 public static function GetSqlInByArray(aItem)
		///	 {
		///		 var SqlIn = "";
		///		 for (var i = 0, i2 = aItem.length; i &lt; i2; i++)
		///		 {
		///			 SqlIn += ",'" + aItem[i] + "'";
		///		 }
		///		 SqlIn = SqlIn.substr(1);
		/// 		
		///		 return SqlIn;
		///	 }
		/// }
		/// 
		/// string JScript = CCode.FlashFileToJScript(FlashPathFile);
		/// Console.WriteLine(JScript);
		/// var CSql = new CSql();
		/// function CSql()
		/// {
		///		 this.GetSqlInByArray = GetSqlInByArray;
		///		 return this;
		/// }
		/// 
		///		 function GetSqlInByArray(aItem)
		///		 {
		///				 var SqlIn = "";
		///				 for (var i = 0, i2 = aItem.length; i &lt; i2; i++)
		///				 {
		///						 SqlIn += ",'" + aItem[i] + "'";
		///				 }
		///				 SqlIn = SqlIn.substr(1);
		/// 
		///				 return SqlIn;
		///		 }
		/// </code>
		/// </example>
		public static string FlashFileToJScript(string PathFile)
		{
			StreamReader sr = new StreamReader(PathFile, Encoding.UTF8);
			string s = sr.ReadToEnd();
			return FlashToJScript(s);
		}

		/// <summary>
		/// Flash의 Class를 읽어 JScript 코드로 변환해서 리턴함.
		/// </summary>
		/// <param name="FlashCode">Flash의 Class 파일</param>
		/// <returns>변환된 JScript 코드</returns>
		public static string FlashToJScript(string FlashCode)
		{
			string[] aCode = FlashCode.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

			string ClassName = "";
			List<string> aFuncName = new List<string>();
			int PosStartFunc = -1, PosEndFunc = -1;
			string RecentFuncName = "";
			
			for (int nLine = 0; nLine < aCode.Length; nLine++)
			{
				string Line = aCode[nLine].Trim(new char[]{'\r', '\n', '\t', ' '});
				if (Line.StartsWith("class"))
				{
					int PosSpc = Line.IndexOf(' ');
					ClassName = Line.Substring(PosSpc + 1);
				}
				else if (Line.EndsWith("}"))
				{
					if (PosStartFunc == -1)
					{
						PosStartFunc = nLine + 1;
					}

					if ((nLine + 1) == aCode.Length)
					{
						PosEndFunc = nLine;
					}
				}
				else if (Line.StartsWith("public static function"))
				{
					RecentFuncName = Line.Substring("public static function".Length + 1).TrimStart(new char[] { '\t', ' ' });
					int PosOpen = RecentFuncName.IndexOf("(");
					RecentFuncName = RecentFuncName.Substring(0, PosOpen);

					aCode[nLine] = aCode[nLine].Replace("public static function ", "function ");

					aFuncName.Add(RecentFuncName);
				}
			}

			string s =
			"var " + ClassName + " = new " + ClassName + "();\r\n"
			+ "function " + ClassName + "()\r\n"
			+ "{\r\n";

			for (int i = 0, i2 = aFuncName.Count; i < i2; i++)
			{
				s += "\tthis." + (string)aFuncName[i] + " = " + (string)aFuncName[i] + ";\r\n";
			}
			s += "\treturn this;\r\n";

			s += "}\r\n"
				+ "\r\n";

			s += String.Join("\r\n", aCode, PosStartFunc, (PosEndFunc - PosStartFunc));

			return s;
		}
	}
}
