using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace DoctorGu
{
	/// <summary>
	/// 구문에 맞게 색을 넣음.
	/// </summary>
	/// <example>
	/// //클립보드에서 가져온 문자열에 컬러를 입히고 컬러를 입힌 문자열을 클립보드에 복사함.
	/// CSyntaxColor sc = new CSyntaxColor(Clipboard.GetDataObject().GetData("System.String").ToString());
	/// Clipboard.SetDataObject(sc.GetHtml(), true);
	/// </example>
	public class CSyntaxColor
	{
		private string mCode = "";
		private Color mBackColor = Color.Beige;

		private const string drCommentTagStart = "<font color=forestgreen>";
		private const string drKeywordTagStart = "<font color=blue>";
		private const string drQuoteTagStart = "<font color=dimgray>";
		private const string drErrTagStart = "<font color=red>";
		private const string drTagEnd = "</font>";

		private string[] aKeywords 
			= new string[]{"as", "auto",
							  "base", "break",
							  "case", "catch", "const", "continue",
							  "default", "do",
							  "else", "event", "explicit", "extern",
							  "finally", "fixed", "for", "foreach",
							  "goto",
							  "if", "implicit", "in", "internal",
							  "lock",
							  "namespace",
							  "operator", "out", "override",
							  "params", "partial", "private", "protected", "public",
							  "readonly", "ref", "return",
							  "sealed", "stackalloc", "static", "switch",
							  "this", "throw", "try",
							  "unsafe", "using",
							  "virtual", "void",
							  "while"};
		private string[] aDataTypes 
			= new string[]{"bool", "byte",
							  "char", "class",
							  "decimal", "delegate", "double",
							  "enum",
							  "float",
							  "int", "interface",
							  "long",
							  "object",
							  "sbyte", "short", "string", "struct",
							  "uint", "ulong", "ushort"};

		private string[] aOperators
			= new string[]{"as", "checked", "is", "new",
							  "sizeof", "stackalloc", "typeof", "unchecked"};

		private string[] aLiterals
			= new string[]{"true", "false", "null"};


		public CSyntaxColor(string Code)
		{
			this.mCode = Code;
		}
		public CSyntaxColor(string Code, Color BackColor)
		{
			this.mCode = Code;
			this.mBackColor = BackColor;
		}
		public CSyntaxColor()
		{
		}

		public string Code
		{
			get {return mCode;}
			set {mCode = value;}
		}
		public Color BackColor
		{
			get {return mBackColor;}
			set {mBackColor = value;}
		}

		public string GetHtml()
		{
			string Code = this.mCode;
			if (Code == "")
			{
				throw new Exception("Code가 0길이 문자열입니다.");
			}
			
			NameValueCollection nvPosAndTag = GetPosAndTag(Code);
			Code = GetNewCode(Code, nvPosAndTag);
			
			return "<table width='100%' border=0 bgcolor='"
				+ this.mBackColor.Name + "'>\r\n"
				+ "<tr><td width='100%'>\r\n"
				+ Code
				+ "</td></tr>\r\n"
				+ "</table>";

		}

		/// <summary>
		/// 태그를 삽입할 위치와 태그 문자열 정보를 NameValueCollection으로 리턴함.
		/// </summary>
		/// <param name="Code"></param>
		/// <returns></returns>
		private NameValueCollection GetPosAndTag(string Code)
		{
			NameValueCollection nv = new NameValueCollection();
			

			// \w+: 연속된(+) 글자(w), 즉 단어
			// //: 한줄 주석
			// /\*: 여러줄 주석 시작
			// @": @-quoted
			// ": quoted
			// ': single quoted

			int PosCur = 0;
			Regex r = new Regex(@"\w+|//|/\*|@""|""|'", RegexOptions.Compiled);
			for (Match m = r.Match(Code) /*1*/; m.Success /*2*/; m = m.NextMatch()) /*3*/
			{
				if (m.Index < PosCur) continue;

				switch (m.Value)
				{
					case @"//":
						PosCur = GetCommentEnd(Code, m.Index) + 1;
						if (PosCur != 0)
						{
							nv.Add(m.Index.ToString(), drCommentTagStart);
							nv.Add(PosCur.ToString(), drTagEnd);
						}
						else
						{
							return GetErrInfo(nv, m.Index, Code.Length);
						}
						break;
					case @"/*":
						PosCur = GetMultiCommentEnd(Code, m.Index) + 1;
						if (PosCur != 0)
						{
							nv.Add(m.Index.ToString(), drCommentTagStart);
							nv.Add(PosCur.ToString(), drTagEnd);
						}
						else
						{
							return GetErrInfo(nv, m.Index, Code.Length);
						}
						break;
					case @"@""":
						PosCur = GetAtQuoteEnd(Code, m.Index) + 1;
						if (PosCur != 0)
						{
							nv.Add(m.Index.ToString(), drQuoteTagStart);
							nv.Add(PosCur.ToString(), drTagEnd);
						}
						else
						{
							return GetErrInfo(nv, m.Index, Code.Length);
						}
						break;
					case @"""":
						PosCur = CDelim.GetQuoteEnd(Code, m.Index, '"') + 1;
						if (PosCur != 0)
						{
							nv.Add(m.Index.ToString(), drQuoteTagStart);
							nv.Add(PosCur.ToString(), drTagEnd);
						}
						else
						{
							return GetErrInfo(nv, m.Index, Code.Length);
						}
						break;
					case @"'":
						PosCur = CDelim.GetQuoteEnd(Code, m.Index, '\'') + 1;
						if (PosCur != 0)
						{
							nv.Add(m.Index.ToString(), drQuoteTagStart);
							nv.Add(PosCur.ToString(), drTagEnd);
						}
						else
						{
							return GetErrInfo(nv, m.Index, Code.Length);
						}
						break;
					default:
						// \w+
						if ((CArray.IndexOf(aKeywords, m.Value, true) >= 0)
							|| (CArray.IndexOf(aDataTypes, m.Value, true) >= 0)
							|| (CArray.IndexOf(aOperators, m.Value, true) >= 0)
							|| (CArray.IndexOf(aLiterals, m.Value, true) >= 0))
						{
							nv.Add(m.Index.ToString(), drKeywordTagStart);
							nv.Add((m.Index + m.Value.Length).ToString(), drTagEnd);
						}
						break;
				}
			}

			//마지막 문자열을 GetNewCode에서 읽을 수 있도록 위치만을 지정함.
			//if (!DrCollection.HasKey(nv, Code.Length.ToString()))
			if (nv[Code.Length.ToString()] == null)
				nv.Add(Code.Length.ToString(), "");

			return nv;
		}

		private NameValueCollection GetErrInfo(NameValueCollection nv, int Start, int End)
		{
			nv.Add(Start.ToString(), drErrTagStart);
			nv.Add(End.ToString(), drTagEnd);
			return nv;
		}

		private int GetCommentEnd(string Code, int Index)
		{
			for (int i = (Index + 2), j = Code.Length; i < j; i++)
			{
				char Cur = Code[i];
				if (Cur == 13)
				{
					if ((i + 1) < j)
					{
						char Next = Code[i + 1];
						if (Next == 10)
						{
							return i - 1;
						}
					}
				}
			}

			//줄바꿈이 없다면 한줄짜리 문장이므로 마지막 위치를 리턴함.
			return (Code.Length - 1);
		}
		private int GetMultiCommentEnd(string Code, int Index)
		{
			for (int i = (Index + 2), j = Code.Length; i < j; i++)
			{
				char Cur = Code[i];
				if (Cur == '*')
				{
					if ((i + 1) < j)
					{
						char Next = Code[i + 1];
						if (Next == '/')
						{
							return i + 1;
						}
					}
				}
			}

			return -1;
		}
		private int GetAtQuoteEnd(string Code, int Index)
		{
			for (int i = (Index + 2), j = Code.Length; i < j; i++)
			{
				char Cur = Code[i];
				if (Cur == '"')
				{
					if ((i + 1) < j)
					{
						char Next = Code[i + 1];
						if (Next == '"')
						{
							i++;
						}
						else
						{
							return i;
						}
					}
				}
			}
		
			return -1;
		}
		
		private string GetNewCode(string Code, NameValueCollection nv)
		{
			StringBuilder sb = new StringBuilder();
			
			int PosStart = 0, PosEnd = 0;
			for (int i = 0, j = nv.Count; i < j; i++)
			{
				PosEnd = Convert.ToInt32(nv.GetKey(i)) - 1;
				string s = Code.Substring(PosStart, PosEnd - PosStart + 1);
				//Html로 표시할 것이므로 <와 >를 안전한 문자열로 변환함.
				s = s.Replace("<", "&lt;").Replace(">", "&gt;");
				s = s.Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
				s = s.Replace("\r\n", "<br>\r\n");

				string Value = nv[i];

				sb.Append(s);
				sb.Append(Value);

				PosStart = PosEnd + 1;
			}


			return sb.ToString();
		}
	}
}
