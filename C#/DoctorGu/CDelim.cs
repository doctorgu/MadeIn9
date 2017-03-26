using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace DoctorGu
{
	/// <summary>
	/// 콤마, 줄바꿈, 따옴표 등의 구분자로 구분된 문자열들을 만들거나 분석하는 기능 구현
	/// </summary>
	public class CDelim
	{
		private NameValueCollection _nvNameValue = new NameValueCollection();

		/// <summary>
		/// 행 구분자, 열 구분자로 구분된 문자열을 파싱함.
		/// </summary>
		/// <param name="Params"></param>
		/// <param name="DelimRow"></param>
		/// <param name="DelimCol"></param>
		/// <example>
		/// string Params = "Description=네이버한자,Url=http://hanja.naver.com/hanja?q={{q}}";
		/// CDelim Delim = new CDelim(Params, ',', '=');
		/// string Url = Delim["Url"]; //http://hanja.naver.com/hanja?q={{q}}
		/// </example>
		public CDelim(string Params, char DelimRow, char DelimCol)
		{
			string[] aParams = Params.Split(DelimRow);
			foreach (string Param in aParams)
			{
				string[] aNameValue = Param.Split(DelimCol);
				string Name = String.Empty, Value = string.Empty;
				for (int i = 0; i < aNameValue.Length; i++)
				{
					if (i == 0)
						Name = aNameValue[i];
					else if (i == 1)
						Value = aNameValue[i];
					else
						//"Description=네이버한자,Url=http://hanja.naver.com/hanja?q={{q}}"
						//와 같은 경우는 Url에 =이 또 붙으므로 포함시킴.
						Value += DelimCol + aNameValue[i];
				}

				_nvNameValue.Add(Name, Value);
			}
		}

		public string this[int Index]
		{
			get
			{
				return _nvNameValue[Index];
			}
		}
		public string this[string Name]
		{
			get
			{
				return _nvNameValue[Name];
			}
		}

		/// <summary>
		/// 명령줄 인수의 값을 분석해서 Key를 이용해 Value를 얻을 수 있도록 함.
		/// </summary>
		/// <param name="Command">"/i gu /you sky"와 같은 명령줄 인수 문자열</param>
		/// <param name="Delim">구분자</param>
		/// <returns>Key와 Value가 쌍으로 구성된 NameValueCollection 개체</returns>
		/// <example>
		/// <code>
		/// NameValueCollection CmdParam;
		/// string[] a = null;
		/// 
		/// //일반적인 /가 키에 대한 구분자인 경우
		/// a = new string[]{"/i", "gu", "/you", "sky"};
		/// CmdParam = CDelim.GetCommandParameter(a);
		/// Console.WriteLine("i: {0}, you: {1}", CmdParam["i"], CmdParam["you"]); //"i: gu, you: sky"
		/// 
		/// //키에 대한 구분자가 -인 경우
		/// a = new string[]{"-i", "gu", "-you", "sky"};
		/// CmdParam = CDelim.GetCommandParameter(a, "-");
		/// Console.WriteLine("i: {0}, you: {1}", CmdParam["i"], CmdParam["you"]); //"i: gu, you: sky"
		/// 
		/// //키에 대한 구분자가 -와 /가 같이 사용되면 제대로 해석할 수 없음.
		/// a = new string[]{"-i", "gu", "/you", "sky"};
		/// CmdParam = CDelim.GetCommandParameter(a, "-");
		/// Console.WriteLine("i: {0}, you: {1}", CmdParam["i"], CmdParam["you"]); //"i: gu /you sky, you:"
		/// 
		/// //값이 없으므로 리턴값은 null임
		/// CmdParam = CDelim.GetCommandParameter(new string[] { "" });
		/// Console.WriteLine(CmdParam == null); //True
		/// 
		/// //값에 구분자 문자열이 없으므로 리턴값은 null임
		/// CmdParam = CDelim.GetCommandParameter(new string[] { "no slash" });
		/// Console.WriteLine(CmdParam == null); //True
		/// </code>
		/// </example>
		[Obsolete("Use GetCommandLineArgs")]
		public static NameValueCollection GetCommandParameter(string Command, string Delim)
		{
			NameValueCollection nv = new NameValueCollection();

			if (Command == "")
				return nv;

			//스페이스 + 구분자로 나눌 것이므로 
			if (Command.Substring(0, 1) != " ")
			{
				Command = " " + Command;
			}

			//Delim(주로 '/')를 경계로 Key와 Value 쌍을 배열의 항목으로 나눔.
			string[] aCommand = Regex.Split(Command, " " + Delim);

			//Command에 0길이 문자열이거나 / 기호가 없으면 Length는 1이 됨.
			if (aCommand.Length <= 1)
			{
				return nv;
			}

			//스페이스를 경계로 왼쪽의 Key와 오른쪽의 Value를 가져옴.
			//만약 스페이스가 없다면 Value는 0길이 문자열이 됨.
			int PosSpace = 0;
			string Key = "", Value = "";
			for (int i = 1; i < aCommand.Length; i++)
			{
				PosSpace = aCommand[i].IndexOf(" ");
				if (PosSpace == -1)
				{
					Key = aCommand[i];
					Value = "";
				}
				else
				{
					Key = aCommand[i].Substring(0, PosSpace);
					Value = aCommand[i].Substring(PosSpace).Trim();
				}

				nv.Add(Key, Value);
			}

			return nv;
		}
		/// <summary>
		/// 명령줄 인수의 값을 분석해서 Key를 이용해 Value를 얻을 수 있도록 함.
		/// </summary>
		/// <param name="Command">new string[]{"/i", "gu", "/you", "sky"}와 같은 명령줄 인수 문자열</param>
		/// <param name="Delim">구분자</param>
		/// <returns>Key와 Value가 쌍으로 구성된 NameValueCollection 개체</returns>
		[Obsolete("Use GetCommandLineArgs")]
		public static NameValueCollection GetCommandParameter(string[] Command, string Delim)
		{
			return GetCommandParameter(String.Join(" ", Command), Delim);
		}
		/// <summary>
		/// 명령줄 인수의 값을 분석해서 Key를 이용해 Value를 얻을 수 있도록 함.
		/// 구분자는 "/"임
		/// </summary>
		/// <param name="Command">new string[]{"/i", "gu", "/you", "sky"}와 같은 명령줄 인수 문자열</param>
		/// <returns>Key와 Value가 쌍으로 구성된 NameValueCollection 개체</returns>
		[Obsolete("Use GetCommandLineArgs")]
		public static NameValueCollection GetCommandParameter(string[] Command)
		{
			return GetCommandParameter(Command, "/");
		}
		/// <summary>
		/// 명령줄 인수의 값을 분석해서 Key를 이용해 Value를 얻을 수 있도록 함.
		/// 구분자는 "/"임
		/// </summary>
		/// <param name="Command">"/i gu /you sky"와 같은 명령줄 인수 문자열</param>
		/// <returns>Key와 Value가 쌍으로 구성된 NameValueCollection 개체</returns>
		[Obsolete("Use GetCommandLineArgs")]
		public static NameValueCollection GetCommandParameter(string Command)
		{
			return GetCommandParameter(Command, "/");
		}

		public static NameValueCollection GetCommandLineArgs(string[] Args, string Delim)
		{
			NameValueCollection nv = new NameValueCollection();

			string Name = "";
			for (int i = 0; i < Args.Length; i++)
			{
				var Arg = Args[i];
				if (Arg.StartsWith(Delim))
				{
					Name = Arg.Substring(Delim.Length);
					nv.Add(Name, null);
				}
				else
				{
					if (string.IsNullOrEmpty(nv[Name]))
						nv[Name] = Arg;
					else
						nv[Name] += " " + Arg;
				}
			}

			return nv;
		}


		/// <summary>
		/// 텍스트 파일의 행 구분자를 리턴함.
		/// 일반적으로 텍스트 파일이 Window 형식이면 \r\n, Unix 형식이면 \n이 행 구분자가 됨.
		/// </summary>
		/// <param name="Value">행 구분자가 있는 문자열</param>
		/// <example>
		/// 다음은 C:\Windows\setuplog.txt 파일의 행 구분자를 리턴합니다.
		/// <code>
		/// string s = CFile.GetTextInFile(@"C:\Windows\setuplog.txt");
		/// string LineSep = CDelim.GetLineSeparator(s);
		/// if (LineSep == "\r\n")
		/// {
		///	 Console.WriteLine("Window 형식");
		/// }
		/// else if (LineSep == "\n")
		/// {
		///	 Console.WriteLine("Unix 형식");
		/// }
		/// </code>
		/// </example>
		public static string GetLineSeparator(string Value)
		{
			int PosLf = Value.IndexOf("\n");
			if (PosLf == -1)
			{
				return "";
			}

			//다음의 else if에서 PosLf - 1로 검사하므로 에러가 발생하지 않도록
			//Line Feed의 위치가 첫번째인지 검사함.
			if (PosLf == 0)
			{
				return "\n";
			}
			else if (Value.Substring(PosLf - 1, 1) == "\r")
			{
				return "\r\n";
			}
			else
			{
				return "\n";
			}
		}

		/// <summary>
		/// 변수를 가진 문자열에서 변수 자체를 삭제함.
		/// </summary>
		/// <param name="TextHasVar">변수를 가진 문자열</param>
		/// <param name="SymbolStart">변수 시작 문자열</param>
		/// <param name="SymbolEnd">변수 종료 문자열</param>
		/// <returns>변수가 삭제된 문자열</returns>
		/// <example>
		/// 다음은 변수인 [[Name]]과 [[Age]] 문자열을 삭제함.
		/// <code>
		/// string Value = "Name: [[Name]], Age: [[Age]]";
		/// string Res = CDelim.EraseAllVariables(Value, "[[", "]]");
		/// Console.WriteLine(Res); //"Name: , Age:"
		/// </code>
		/// </example>
		public static string EraseAllVariables(string TextHasVar, string SymbolStart, string SymbolEnd)
		{
			while (true)
			{
				int PosStart = TextHasVar.IndexOf(SymbolStart);
				if (PosStart == -1)
					break;

				int PosEnd = GetIndexSymbolClose(TextHasVar, SymbolStart, SymbolEnd, PosStart);
				if (PosEnd == -1)
					break;
				
				//int PosEnd = TextHasVar.IndexOf(SymbolEnd, PosStart + 1) + (SymbolEnd.Length - 1);
				//if ((PosEnd - (SymbolEnd.Length - 1)) == -1) break;

				PosEnd += SymbolEnd.Length - 1;
				TextHasVar = TextHasVar.Substring(0, PosStart) + TextHasVar.Substring(PosEnd + 1);
			}

			return TextHasVar;
		}

		/// <summary>
		/// 변수를 가진 문자열에서 변수가 포함된 행 전체를 삭제함.
		/// </summary>
		/// <param name="TextHasVar">변수를 가진 문자열</param>
		/// <param name="SymbolStart">변수 시작 문자열</param>
		/// <param name="SymbolEnd">변수 종료 문자열</param>
		/// <returns>변수를 가진 행이 삭제된 문자열</returns>
		/// <example>
		/// <code>
		/// string Value = "Name: [[Name]],\r\nAge: 45";
		/// string Res = CDelim.EraseAllVariablesLine(Value, "[[", "]]");
		/// Console.WriteLine(Res); //"Age: 45"
		/// </code>
		/// </example>
		public static string EraseAllVariablesLine(string TextHasVar, string SymbolStart, string SymbolEnd)
		{
			string LineSep = GetLineSeparator(TextHasVar);
			if (LineSep == "") LineSep = "\n";

			while (true)
			{
				int PosStart = TextHasVar.IndexOf(SymbolStart);
				if (PosStart == -1) break;

				int PosEnd = TextHasVar.IndexOf(SymbolEnd, PosStart + 1) + (SymbolEnd.Length - 1);
				if ((PosEnd - (SymbolEnd.Length - 1)) == -1) break;

				int PosStartOfLine = TextHasVar.LastIndexOf(LineSep, PosStart) + LineSep.Length;
				if ((PosStartOfLine - LineSep.Length) == - 1)
				{
					PosStartOfLine = 0;
				}

				int PosEndOfLine = TextHasVar.IndexOf(LineSep, PosEnd) + (LineSep.Length - 1);
				if ((PosEndOfLine - (LineSep.Length - 1)) == -1)
				{
					PosEndOfLine = TextHasVar.Length - 1;
				}

				TextHasVar = TextHasVar.Substring(0, PosStartOfLine) + TextHasVar.Substring(PosEndOfLine + 1);
			}

			return TextHasVar;
		}

		/// <summary>
		/// 여러 개의 파일을 선택해서 끌어놓거나 엑셀의 영역을 복사하거나 끌어놓을 때 다음과 같이 이중따옴표가 쓰여짐.
		/// 파일의 경우: 파일 이름에 스페이스가 있으면 파일이름의 양쪽을 이중따옴표로 묶음.
		/// 엑셀의 경우: 특정 셀에 줄바꿈(char(10))이 있으면 셀 내용의 양쪽을 이중따옴표로 묶음.
		/// 이런 경우를 예상해서 각 항목을 컬렉션에 추가함.
		/// </summary>
		/// <param name="TextList">문자열 목록</param>
		/// <param name="DelimCol">열 구분자</param>
		/// <param name="DelimRow">행 구분자</param>
		/// <param name="PosStart"><paramref name="TextList"/>에서 파싱을 시작할 위치</param>
		/// <param name="PosNextIs"></param>
		/// <returns>배열로 구분된 문자열</returns>
		/// <example>
		/// 다음은 "b,c"가 한 항목이므로 따옴표를 묶어 하나의 항목임을 표시한 경우임.
		/// <code>
		/// string TextList = @"a,""b,c"",d,e";
		/// string[] aItem = CDelim.GetItemByDelimChar(TextList, ",", ",");
		/// Console.WriteLine(string.Join("+", aItem)); //"a+b,c+d+e"
		/// </code>
		/// 다음은 엑셀에서 데이터를 Drag해서 DataGridView에 Drop한 경우,
		/// 해당 데이터를 읽어 DataTable로 변환한 후, DataGridView에 표시합니다.
		/// <code>
		/// <![CDATA[
		/// private void dataGridView1_DragEnter(object sender, DragEventArgs e)
		/// {
		///	 e.Effect = DragDropEffects.Copy;
		/// }
		///
		/// private void dataGridView1_DragDrop(object sender, DragEventArgs e)
		/// {
		///	 string s = e.Data.GetData(typeof(System.String)).ToString();
		///	 int PosNextIs = -1;
		///
		///	 string[] aHeader = CDelim.GetItemByDelimChar(s, "\t", "\r\n", 0, out PosNextIs);
		///
		///	 DataTable dt = new DataTable();
		///	 for (int i = 0, i2 = aHeader.Length; i < i2; i++)
		///	 {
		///		 dt.Columns.Add(aHeader[i]);
		///	 }
		///
		///	 while (PosNextIs != -1)
		///	 {
		///		 string[] aRows = CDelim.GetItemByDelimChar(s, "\t", "\r\n", PosNextIs, out PosNextIs);
		///		 dt.Rows.Add(aRows);
		///	 }
		///
		///	 dataGridView1.DataSource = dt;
		/// }
		/// ]]>
		/// </code>
		/// </example>
		public static string[] GetItemByDelimChar(string TextList,
									string DelimCol, string DelimRow,
									int PosStart, 
									out int PosNextIs)
		{
			PosNextIs = -1;

			if (TextList.EndsWith(DelimRow))
			{
				TextList = TextList.TrimEnd(DelimRow.ToCharArray());
			}

			if (TextList.EndsWith(DelimCol))
			{
				TextList = TextList.TrimEnd(DelimCol.ToCharArray());
			}
			TextList += DelimCol + DelimRow;

			bool IsFirstQuoteFound = false;
			int PosEnd = -1;
			string Text = "";
			List<string> aTextList = new List<string>();
			for (int i = PosStart, i2 = TextList.Length; i < i2; i++)
			{
				char c = TextList[i];
				string c2 = "";
				if ((i + 1) < i2)
				{
					c2 = TextList.Substring(i, 2);
				}

				if (c == '"')
				{
					if (IsFirstQuoteFound)
					{
						if (c2 == "\"\"")
						{
							//내용을 따옴표로 묶었는 데 내용 자체에 두개의 따옴표가 있다면
							//따옴표를 표현하는 것이므로 이 따옴표는 무시함.
							i++;
						}
						else
						{
							PosEnd = i - 1;
							
							Text = TextList.Substring(PosStart, PosEnd - PosStart + 1);
							if (Text != "")
							{
								aTextList.Add(Text);
							}
							
							//"와 스페이스 다음의 위치이므로 2을 더함.
							PosStart = i + 2;
							
							//다음엔 확실히 스페이스가 있을 것이므로 Skip함
							i++;
							
							IsFirstQuoteFound = false;
						}
					}
					else
					{
						PosStart = i + 1;
						IsFirstQuoteFound = true;
					}
				}
				else if (c.ToString() == DelimCol)
				{
					if (!IsFirstQuoteFound)
					{
						PosEnd = i - 1;
						
						Text = TextList.Substring(PosStart, PosEnd - PosStart + 1);
						if (Text != "")
						{
							aTextList.Add(Text);
						}
						
						//DelimCol 다음의 위치이므로 1을 더함.
						PosStart = i + 1;
					}
				}
				//행구분자인 경우 \r\n이 되면 2자가 됨.
				else if (c2 == DelimRow)
				{
					if (!IsFirstQuoteFound)
					{
						PosEnd = i - 1;
						
						Text = TextList.Substring(PosStart, PosEnd - PosStart + 1);
						if (Text != "")
						{
							aTextList.Add(Text);
						}
						
						PosNextIs = i + DelimRow.Length;
						if ((PosNextIs + 1) > TextList.Length)
						{
							PosNextIs = -1;
						}
						
						break;
					}
				}
			}

			return aTextList.ToArray();
		}

		/// <summary>
		/// 여러 개의 파일을 선택해서 끌어놓거나 엑셀의 영역을 복사하거나 끌어놓을 때 다음과 같이 이중따옴표가 쓰여짐.
		/// 파일의 경우: 파일 이름에 스페이스가 있으면 파일이름의 양쪽을 이중따옴표로 묶음. -> 닷넷의 경우 이중따옴표를 자동으로 인식하고 args 배열에 들어오므로 해당 안됨.
		/// 엑셀의 경우: 특정 셀에 줄바꿈(char(10))이 있으면 셀 내용의 양쪽을 이중따옴표로 묶음.
		/// 이런 경우를 예상해서 각 항목을 컬렉션에 추가함.
		/// </summary>
		/// <param name="aTextList">문자열 목록</param>
		/// <param name="DelimCol">열 구분자</param>
		/// <returns>배열로 구분된 문자열</returns>
		public static string[] GetItemByDelimChar(string[] aTextList, string DelimCol)
		{
			string TextList = String.Join(" ", aTextList);
			int PosNextIs;
			return GetItemByDelimChar(TextList, DelimCol, DelimCol, 0, out PosNextIs);
		}
		/// <summary>
		/// 여러 개의 파일을 선택해서 끌어놓거나 엑셀의 영역을 복사하거나 끌어놓을 때 다음과 같이 이중따옴표가 쓰여짐.
		/// 파일의 경우: 파일 이름에 스페이스가 있으면 파일이름의 양쪽을 이중따옴표로 묶음.
		/// 엑셀의 경우: 특정 셀에 줄바꿈(char(10))이 있으면 셀 내용의 양쪽을 이중따옴표로 묶음.
		/// 이런 경우를 예상해서 각 항목을 컬렉션에 추가함.
		/// </summary>
		/// <param name="TextList">문자열 목록</param>
		/// <param name="DelimCol">열 구분자</param>
		/// <returns>배열로 구분된 문자열</returns>
		public static string[] GetItemByDelimChar(string TextList, string DelimCol)
		{
			int PosNextIs;
			return GetItemByDelimChar(TextList, DelimCol, DelimCol, 0, out PosNextIs);
		}

		/// <summary>
		/// 배열의 값을 콤마(,)가 구분자인 문자열로 변환해서 리턴함. 값에 콤마가 있다면 두개의 콤마로 변경해줌.
		/// </summary>
		/// <param name="aValue">문자열로 변환할 배열</param>
		/// <returns>콤마(,)로 구분된 문자열</returns>
		/// <example>
		/// 다음은 콤마로 구분된 두번째 항목이 "b,c"인 문자열을 배열로 변환하고, 다시 콤마로 구분된 문자열로 변환합니다.
		/// <code>
		/// string s = "a,b,,c,d";
		/// string[] a = CArray.SplitComma(s);
		/// Console.WriteLine(a[1]); //b,c
		///
		/// string s2 = CDelim.CombineComma(a);
		/// Console.WriteLine(s2); //"a,b,,c,d"
		/// </code>
		/// </example>
		public static string CombineComma(params string[] aValue)
		{
			string Value = "";
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				Value += "," + aValue[i].Replace(",", ",,");
			}
			if (Value != "") Value = Value.Substring(1);

			return Value;
		}

		/// <summary>
		/// 큰 따옴표(")가 있는 경우엔 하나의 항목으로 취급해서 Split함
		/// </summary>
		/// <param name="Args"></param>
		/// <returns></returns>
		public static string[] SplitCommandLineArgs(string Args)
		{
			char Delim = (char)4;

			char[] aArgs = Args.ToCharArray();
			bool IsQuoteStarted = false;
			for (int i = 0; i < aArgs.Length; i++)
			{
				if (aArgs[i] == '"')
				{
					IsQuoteStarted = !IsQuoteStarted;
					aArgs[i] = Delim;
				}

				if (!IsQuoteStarted && aArgs[i] == ' ')
					aArgs[i] = Delim;
			}
			return (new string(aArgs)).Split(new char[] { Delim }, StringSplitOptions.RemoveEmptyEntries);
		}
		/// <summary>
		/// 스페이스( )가 있는 경우엔 큰 따옴표(")로 묶어 Join함
		/// </summary>
		/// <param name="aArgs"></param>
		/// <returns></returns>
		public static string JoinCommandLineArgs(string[] aArgs)
		{
			for (int i = 0; i < aArgs.Length; i++)
			{
				if (aArgs[i].IndexOf(' ') != -1)
					aArgs[i] = "\"" + aArgs[i] + "\"";
			}

			return string.Join(" ", aArgs);
		}

		/// <summary>
		/// 첫번째로 찾아진 작은 따옴표(')로 묶은 문자열을 리턴함.
		/// </summary>
		/// <param name="Value">따옴표(')로 묶인 문자열을 가진 전체 문자열</param>
		/// <param name="IndexStart"><paramref name="Value"/> 문자열 내에서 찾기 시작할 위치</param>
		/// <returns>따옴표(')로 묶인 첫번째 문자열(따옴표(')는 제외됨)</returns>
		/// <example>
		/// <code>
		/// string s =
		/// @"COMMENT ON TABLE WWW_BOARD IS '게시판';
		/// COMMENT ON COLUMN WWW_BOARD.KIND IS '게시판 종류';";
		/// string s2 = CDelim.GetFirstQuotedValue(s, 0);
		/// Console.WriteLine(s2); //"게시판"
		/// </code>
		/// </example>
		public static string GetFirstQuotedValue(string Value, int IndexStart)
		{
			int PosStart = -1;
			for (int i = IndexStart, i2 = (Value.Length - 1); i < i2; i++)
			{
				char c = Value[i];
				char c2 = Value[i + 1];

				if ((c == '\'') && (c2 != '\''))
				{
					if (PosStart == -1)
					{
						PosStart = i + 1;
					}
					else
					{
						int PosEnd = i - 1;
						return Value.Substring(PosStart, PosEnd - PosStart + 1);
					}
				}
			}

			return "";
		}

		/// <summary>
		/// <paramref name="Index"/> 위치 이후에 <paramref name="EndChar"/> 값이 있는 위치를 리턴함.
		/// 시작 따옴표에 해당하는 마지막 따옴표의 위치를 알기 위함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Index"></param>
		/// <param name="EndChar"></param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// int i = CDelim.GetQuoteEnd("'''1'", 0, '\'');
		/// Console.WriteLine(i.ToString()); //4
		///
		/// int i2 = CDelim.GetQuoteEnd("'1'", 0, '\'');
		/// Console.WriteLine(i2.ToString()); //2		/// </code>
		/// </example>
		public static int GetQuoteEnd(string Value, int Index, char EndChar)
		{
			while (true)
			{
				int PosEnd = Value.IndexOf(EndChar, Index + 1);
				if (PosEnd == -1)
				{
					return -1;
				}

				if (((PosEnd + 1) < Value.Length) && (Value.Substring(PosEnd + 1, 1)[0] == EndChar))
				{
					Index = PosEnd + 1;
				}
				else
				{
					return PosEnd;
				}
			}

			//for (int i = (Index + 1), j = Value.Length; i < j; i++)
			//{
			//	char Cur = Value[i];
			//	if (Cur == '\\')
			//	{
			//		i++;
			//		continue;
			//	}
			//	else if (Cur == EndChar)
			//	{
			//		return i;
			//	}
			//}

			//return -1;
		}

		public static List<string> GetAllVariables(string ValueHasVar, string DelimStart, string DelimEnd)
		{
			List<string> aVar = new List<string>();

			int StartIndexNextIs;
			int StartIndex = 0;
			while (StartIndex >= 0)
			{
				string Var = FindVariable(ValueHasVar, DelimStart, DelimEnd, StartIndex, out StartIndexNextIs);
				if (!string.IsNullOrEmpty(Var))
				{
					if (aVar.IndexOf(Var) == -1)
					{
						aVar.Add(Var);
					}
				}

				StartIndex = StartIndexNextIs;
			}

			return aVar;
		}
		public static string FindVariable(string ValueHasVar, string DelimStart, string DelimEnd)
		{
			int StartIndex = 0;
			int StartIndexNextIs;
			return FindVariable(ValueHasVar, DelimStart, DelimEnd, StartIndex, out StartIndexNextIs);
		}
		public static string FindVariable(string ValueHasVar, string DelimStart, string DelimEnd, int StartIndex, out int StartIndexNextIs)
		{
			StartIndexNextIs = -1;

			int PosStart = ValueHasVar.IndexOf(DelimStart, StartIndex);
			if (PosStart == -1)
				return "";
			PosStart += DelimStart.Length;

			int PosEnd = ValueHasVar.IndexOf(DelimEnd, (PosStart + 1));
			if (PosEnd == -1)
				return "";
			PosEnd--;

			StartIndexNextIs = (PosEnd + 1 + DelimEnd.Length);

			return CFindRep.SubstringFromTo(ValueHasVar, PosStart, PosEnd);
		}

		/// <summary>
		/// 열린 기호에 해당하는 닫힌 괄호를 찾음.
		/// </summary>
		public static int GetIndexSymbolClose(string Value, string SymbolStart, string SymbolEnd, int IndexStart)
		{
			int ParNum = 0;

			int LenSymbolStart = SymbolStart.Length;
			int LenSymbolEnd = SymbolEnd.Length;
			//여는 기호가 나오면 1 더하고 닫는 기호가 나오면 1 빼서
			//0이 되면 첫번째 여는 기호에 대해 닫힌 기호를 찾은 것임.
			for (int i = IndexStart, i2 = Value.Length; i < i2; i++)
			{
				string SymbolStartCur = Value.Substring(i, Math.Min(LenSymbolStart, (i2 - i + 1)));
				string SymbolEndCur = Value.Substring(i, Math.Min(LenSymbolEnd, (i2 - i + 1)));

				if (SymbolStartCur == SymbolStart)
					ParNum++;
				else if (SymbolEndCur == SymbolEnd)
					ParNum--;

				if (ParNum == 0)
					return i;
			}

			return -1;
		}
	}
}
