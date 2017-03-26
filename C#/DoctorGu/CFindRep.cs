using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace DoctorGu
{
	/// <summary>
	/// 특정 문자열의 위치를 가져오거나 바꾸기 위함.
	/// </summary>
	public class CFindRep
	{
		/// <summary>
		/// <paramref name="Value"/>가 숫자 형식이면 원래 값을 리턴하고, 아니면 0을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>숫자 형식이면 원래 값을 리턴하고, 아니면 0을 리턴함.</returns>
		/// <example>
		/// 다음은 POST 방식으로 전송된 txtQuantity의 값이 비어 있어도 에러를 내지 않고 0 값을 Quantity 변수에 저장합니다.
		/// <code>
		/// int Quantity = CFindRep.IfNotNumberThen0(Req.Form["txtQuantity"]);
		/// </code>
		/// </example>
		public static int IfNotNumberThen0(object Value)
		{
			int i = 0;

			if (Value == null)
				return i;
			
			//실패하면 i는 항상 0임.
			int.TryParse(Value.ToString(), out i);
			return i;
		}
		public static int IfNotNumberThenNegative1(object Value)
		{
			int i = -1;

			if (Value == null)
				return i;

			//실패하면 i는 항상 -1임.
			int.TryParse(Value.ToString(), out i);
			return i;
		}
		public static decimal IfNotNumberThen0Decimal(object Value)
		{
			decimal i = 0;

			if (Value == null)
				return i;
			
			decimal.TryParse(Value.ToString(), out i);
			return i;
		}
		public static double IfNotNumberThen0Double(object Value)
		{
			double i = 0;

			if (Value == null)
				return i;
			
			double.TryParse(Value.ToString(), out i);
			return i;
		}
		public static float IfNotNumberThen0Float(object Value)
		{
			float i = 0;

			if (Value == null)
				return i;

			float.TryParse(Value.ToString(), out i);
			return i;
		}
		/// <summary>
		/// <paramref name="Value"/>가 숫자 형식이면 원래 값을 리턴하고, 아니면 1을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>숫자 형식이면 원래 값을 리턴하고, 아니면 1을 리턴함.</returns>
		/// <example>
		/// 다음은 GET 방식으로 전송된 page 파라미터가 존재하지 않는 경우에도 에러를 내지 않고 1 값을 mPage 변수에 저장합니다.
		/// <code>
		/// this.mPage = CFindRep.IfNotNumberThen1(HttpContext.Current.Request.QueryString["page"]);
		/// </code>
		/// </example>
		public static int IfNotNumberThen1(object Value)
		{
			int i = 1;

			if (Value == null)
				return i;
			
			if (!int.TryParse(Value.ToString(), out i))
				return 1;
			else
				return i;
		}
		/// <summary>
		/// <paramref name="Value"/>가 숫자 형식이면 원래 값을 리턴하고, 아니면 DefaultValue를 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>숫자 형식이면 원래 값을 리턴하고, 아니면 null을 리턴함.</returns>
		public static int IfNotNumberThen(object Value, int DefaultValue)
		{
			int i;

			if (Value == null)
				return DefaultValue;
			
			if (!int.TryParse(Value.ToString(), out i))
				return DefaultValue;
			else
				return i;
		}
		/// <summary>
		/// <paramref name="Value"/>가 숫자 형식이면 원래 값을 리턴하고, 아니면 DefaultValue를 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>숫자 형식이면 원래 값을 리턴하고, 아니면 null을 리턴함.</returns>
		public static double IfNotNumberThen(object Value, double DefaultValue)
		{
			double i;

			if (Value == null)
				return DefaultValue;
			
			if (!double.TryParse(Value.ToString(), out i))
				return DefaultValue;
			else
				return i;
		}


		/// <summary>
		/// <paramref name="Value"/>의 길이가 0보다 크다면 원래 값을 리턴하고, 아니면 null을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>길이가 0보다 크다면 원래 값을 리턴하고, 아니면 null을 리턴함.</returns>
		public static string If0LengthThenNull(string Value)
		{
			if (Value == "")
			{
				return null;
			}
			else
			{
				return Value;
			}
		}
		public static string If0LengthThenSpace(string Value)
		{
			if (Value == "")
			{
				return " ";
			}
			else
			{
				return Value;
			}
		}
		/// <summary>
		/// <paramref name="Value"/>가 null이면 0 길이 문자열을 리턴하고, 아니면 원래 값을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>null이면 0 길이 문자열을 리턴하고, 아니면 원래 값을 리턴함.</returns>
		[Obsolete("Use IfNullThenEmpty")]
		public static string IfNullThen0Length(string Value)
		{
			return (Value != null) ? Value : "";
		}
		/// <summary>
		/// <paramref name="Value"/>가 null이면 0 길이 문자열을 리턴하고, 아니면 원래 값을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>null이면 0 길이 문자열을 리턴하고, 아니면 원래 값을 리턴함.</returns>
		[Obsolete("Use IfNullThenEmpty")]
		public static string IfNullThen0Length(object Value)
		{
			return (Value != null) ? Convert.ToString(Value) : "";
		}
		public static string IfNullThenEmpty(string Value)
		{
			return (Value != null) ? Value : string.Empty;
		}
		public static string IfNullThenEmpty(object Value)
		{
			return (Value != null) ? Convert.ToString(Value) : "";
		}
		public static object IfNullThenDBNull(object Value)
		{
			return (Value != null) ? Value : DBNull.Value;
		}

		/// <summary>
		/// <paramref name="Value"/>가 null이면 0을 리턴하고, 아니면 원래 값을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>null이면 0을 리턴하고, 아니면 원래 값을 리턴함.</returns>
		public static int IfNullThen0(object Value)
		{
			return (Value != null) ? Convert.ToInt32(Value) : 0;
		}

		public static bool IfNullThenFalse(object Value)
		{
			return (Value != null) ? Convert.ToBoolean(Value) : false;
		}

		/// <summary>
		/// <paramref name="Value"/>가 DBNull.Value이면 0을 리턴하고, 아니면 원래 값을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>DBNull.Value이면 0을 리턴하고, 아니면 원래 값을 리턴함.</returns>
		public static int IfDBNullThen0(object Value)
		{
			return (Value != DBNull.Value) ? Convert.ToInt32(Value) : 0;
		}
		public static object IfDBNullThenNull(object Value)
		{
			return (Value != DBNull.Value) ? Value : null;
		}
		public static string IfDBNullThenEmpty(object Value)
		{
			return (Value != DBNull.Value) ? Convert.ToString(Value) : string.Empty;
		}
		[Obsolete("Use IfDBNullThenEmpty")]
		public static string IfDBNullThen0Length(object Value)
		{
			return (Value != DBNull.Value) ? Convert.ToString(Value) : "";
		}
		public static object IfDBNullThen(object Value, object ValueIfDBNull)
		{
			return (Value != DBNull.Value) ? Value : ValueIfDBNull;
		}
		/// <summary>
		/// <paramref name="Value"/>가 null이면 <paramref name="ValueIfNull"/>을 리턴하고, 아니면 원래 값을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <param name="ValueIfNull">null인 경우 대체될 값</param>
		/// <returns>대체된 값 또는 원래 값</returns>
		public static object IfNullThen(object Value, object ValueIfNull)
		{
			return (Value != null) ? Value : ValueIfNull;
		}
		/// <summary>
		/// <paramref name="Value"/>가 0이면 null을 리턴하고, 아니면 원래 값을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>0이면 null을 리턴하고, 아니면 원래 값을 리턴함.</returns>
		public static object If0OrNotNumberThenNull(object Value)
		{
			int i = IfNotNumberThen0(Value);
			if (i == 0)
			{
				return null;
			}
			else
			{
				return Value;
			}
		}
		public static object If0OrNotNumberThenDBNull(object Value)
		{
			int i = IfNotNumberThen0(Value);
			if (i == 0)
			{
				return DBNull.Value;
			}
			else
			{
				return Value;
			}
		}
		public static int If0OrNotNumberThen(object Value, int DefaultValue)
		{
			int i = IfNotNumberThen0(Value);
			if (i == 0)
			{
				return DefaultValue;
			}
			else
			{
				return i;
			}
		}
		public static int If0Then(int Value, int DefaultValue)
		{
			return (Value == 0) ? DefaultValue : Value;
		}
		public static string If0ThenEmpty(int Value)
		{
			return (Value == 0) ? string.Empty : Value.ToString();
		}
		public static string If0ThenEmpty(decimal Value)
		{
			return (Value == 0) ? string.Empty : Value.ToString();
		}
		public static string IfDBNullOrNullOrEmptyThenEmpty(object Value)
		{
			return !CValid.IsDBNullOrNullOrEmpty(Value) ? Convert.ToString(Value) : string.Empty;
		}

		/// <summary>
		/// 클라이언트의 자바스크립트 문자열이 GET 방식으로 서버에 전송될 때 원래의 문자열 그대로
		/// 도착할 수 있도록 특수문자는 %로 시작하는 문자열로 인코딩해서 리턴함.
		/// </summary>
		/// <param name="Value">특수 문자가 포함될 수 있는 문자열</param>
		/// <returns>값</returns>
		/// <example>
		/// <code>
		/// ReplaceSpecialChar("#x@") --> "%23x%40"
		/// </code>
		/// </example>
		public static string ReplaceSpecialCharForGetMethod(string Value)
		{
			string Value2 = "";
			for (int i = 0, i2 = Value.Length; i < i2; i++)
			{
				string c = Value[i].ToString();
				if (c == "!") c = "%21";
				else if (c == "#") c = "%23";
				else if (c == "\"") c = "%22";
				else if (c == "$") c = "%24";
				else if (c == "%") c = "%25";
				else if (c == "&") c = "%26";
				else if (c == "(") c = "%28";
				else if (c == ")") c = "%29";
				else if (c == "@") c = "%40";
				else if (c == "+") c = "%2b";
				else if (c == ",") c = "%2c";
				else if (c == "-") c = "%2d";
				else if (c == ".") c = "%2e";
				else if (c == "/") c = "%2f";
				else if (c == ":") c = "%3a";
				else if (c == "<") c = "%3c";
				else if (c == "=") c = "%3d";
				else if (c == ">") c = "%3e";
				else if (c == "?") c = "%3f";
				else if (c == "[") c = "%5b";
				else if (c == "]") c = "%5d";
				else if (Convert.ToInt32(c[0]) == 13) c = "%0d";
				else if (Convert.ToInt32(c[0]) == 10) c = "%0a";
				else if ((Convert.ToInt32(c[0]) >= 0) && (Convert.ToInt32(c[0]) <= 5)) c = "%0" + Convert.ToInt32(c[0]);

				Value2 += c;
			}

			return Value2;
		}

		/// <summary>
		/// 숫자 1을 ①로 변환함.
		/// </summary>
		/// <param name="Value">원숫자를 포함하는 문자열</param>
		public static string GetCircledNumByRealNum(string Value)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < Value.Length; i++)
			{
				switch (Value[i])
				{
					case '0': sb.Append('ⓞ'); break;
					case '1': sb.Append('①'); break;
					case '2': sb.Append('②'); break;
					case '3': sb.Append('③'); break;
					case '4': sb.Append('④'); break;
					case '5': sb.Append('⑤'); break;
					case '6': sb.Append('⑥'); break;
					case '7': sb.Append('⑦'); break;
					case '8': sb.Append('⑧'); break;
					case '9': sb.Append('⑨'); break;
					default: sb.Append(Value[i]); break;
				}
			}

			return sb.ToString();
		}
		public static string GetRealNumByCircledNum(string Value)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < Value.Length; i++)
			{
				switch (Value[i])
				{
					case '①': sb.Append('1'); break;
					case '②': sb.Append('2'); break;
					case '③': sb.Append('3'); break;
					case '④': sb.Append('4'); break;
					case '⑤': sb.Append('5'); break;
					case '⑥': sb.Append('6'); break;
					case '⑦': sb.Append('7'); break;
					case '⑧': sb.Append('8'); break;
					case '⑨': sb.Append('9'); break;
					default: sb.Append(Value[i]); break;
				}
			}

			return sb.ToString();
		}

		public static string GetOX(bool Value)
		{
			return Value ? "○" : "×";
		}

		/// <summary>
		/// 열린 괄호에 해당하는 닫힌 괄호의 위치를 리턴함.
		/// </summary>
		/// <param name="Value">괄호를 포함하는 문자열</param>
		/// <param name="OpenIndex">열린 괄호의 위치</param>
		/// <example>
		/// Console.WriteLine(GetClosingPar("(1+(2*3))=x", 0)); //8
		/// Console.WriteLine(GetClosingPar("(1+(2*3))=x", 1)); //-1
		/// Console.WriteLine(GetClosingPar("(1+(2*3))=x", 3)); //7
		/// </example>
		public static int GetClosingPar(string Value, int OpenIndex)
		{
			//OpenIndex 위치에 여는 괄호가 없다면 빠져나감.
			if (Value[OpenIndex] != '(')
			{
				return -1;
			}

			//첫번째 여는 괄호에 대해 1을 증가시킴.
			int nPar = 1;

			//여는 괄호가 나오면 1 더하고 닫는 괄호가 나오면 1 빼서
			//0이 되면 첫번째 여는 괄호에 대해 닫힌 괄호를 찾은 것임.
			for (int i = (OpenIndex + 1); i < Value.Length; i++)
			{
				if (Value[i] == '(')
				{
					nPar++;
				}
				else if (Value[i] == ')')
				{
					nPar--;
				}

				if (nPar == 0)
				{
					return i;
				}
			}

			//닫는 괄호를 찾지 못했음.
			return -1;
		}
		/// <summary>
		/// 닫힌 괄호에 해당하는 열린 괄호의 위치를 리턴함.
		/// </summary>
		/// <param name="Value">괄호를 포함하는 문자열</param>
		/// <param name="CloseIndex">닫힌 괄호의 위치</param>
		/// <example>
		/// Console.WriteLine(GetOpenningPar("(1+(2*3))=x", 8)); //0
		/// Console.WriteLine(GetOpenningPar("(1+(2*3))=x", 7)); //3
		/// Console.WriteLine(GetOpenningPar("(1+(2*3))=x", 6)); //-1
		/// </example>
		public static int GetOpenningPar(string Value, int CloseIndex)
		{
			//OpenIndex 위치에 닫는 괄호가 없다면 빠져나감.
			if (Value[CloseIndex] != ')')
			{
				return -1;
			}

			//첫번째 닫는 괄호에 대해 1을 증가시킴.
			int nPar = 1;

			//닫는 괄호가 나오면 1 더하고 여는 괄호가 나오면 1 빼서
			//0이 되면 첫번째 닫는 괄호에 대해 열린 괄호를 찾은 것임.
			for (int i = (CloseIndex - 1); i >= 0; i--)
			{
				if (Value[i] == ')')
				{
					nPar++;
				}
				else if (Value[i] == '(')
				{
					nPar--;
				}

				if (nPar == 0)
				{
					return i;
				}
			}

			//여는 괄호를 찾지 못했음.
			return -1;
		}

		/// <summary>
		/// 문자열 안에서 nTH 번째 위치에 있는 string 형식의 특정 문자열의 위치를 리턴함.
		/// </summary>
		/// <param name="Value">찾을 문자열을 포함하는 문자열</param>
		/// <param name="ToFind">찾을 문자열</param>
		/// <param name="nTH">몇번째</param>
		/// <example>
		/// Console.WriteLine(IndexOfnTH("사랑,사랑,내사랑", "사랑", 0)); //0
		/// Console.WriteLine(IndexOfnTH("사랑,사랑,내사랑", "사랑", 1)); //3
		/// Console.WriteLine(IndexOfnTH("사랑,사랑,내사랑", "사랑", 2)); //7
		/// Console.WriteLine(IndexOfnTH("사랑,사랑,내사랑", "사랑", 3)); //-1
		/// </example>
		public static int IndexOfnTH(string Value, string ToFind, int nTH)
		{
			int Num = -1;

			//처음 찾을 문자열의 위치는 0이어야 함.
			//그런데 while 문 안에서 1을 항상 더하므로 -1로 만들어
			//처음 숫자가 0이 되게 함.
			int Pos = -1;

			//찾을 문자열을 찾았으면 그 다음 위치부터 다시 찾음.
			//더 이상 찾을 게 없도록 이런 과정을 반복하면서
			//반복 회수를 저장함.
			do
			{
				Pos = Value.IndexOf(ToFind, Pos + 1);
				Num++;
			} while ((Num < nTH) && (Pos != -1));

			return Pos;
		}
		/// <summary>
		/// 문자열 안에서 nTH 번째 위치에 있는 특정 문자열의 위치를 리턴함.
		/// </summary>
		/// <param name="Value">찾을 문자열을 포함하는 문자열</param>
		/// <param name="ToFind">찾을 문자열</param>
		/// <param name="nTH">몇번째</param>
		/// <example>
		/// Console.WriteLine(IndexOfnTH("0+0=0", '0', 0)); //0
		/// Console.WriteLine(IndexOfnTH("0+0=0", '0', 1)); //2
		/// Console.WriteLine(IndexOfnTH("0+0=0", '0', 2)); //4
		/// Console.WriteLine(IndexOfnTH("0+0=0", '0', 3)); //-1
		/// </example>
		public static int IndexOfnTH(string Value, char ToFind, int nTH)
		{
			int Num = -1;

			//처음 찾을 문자열의 위치는 0이어야 함.
			//그런데 while 문 안에서 1을 항상 더하므로 -1로 만들어
			//처음 숫자가 0이 되게 함.
			int Pos = -1;

			//찾을 문자열을 찾았으면 그 다음 위치부터 다시 찾음.
			//더 이상 찾을 게 없도록 이런 과정을 반복하면서
			//반복 회수를 저장함.
			do
			{
				Pos = Value.IndexOf(ToFind, Pos + 1);
				Num++;
			} while ((Num < nTH) && (Pos != -1));

			return Pos;
		}

		/// <summary>
		/// 문자열 안에서 string 형식의 특정 문자열이 몇번 반복되는 지 알아냄.
		/// </summary>
		/// <param name="Value"> </param>
		/// <param name="ToFind"> </param>
		/// <example>
		/// GetNumOccurred("ab%%c%%d%%", "%%") //3
		/// </example>
		public static int GetNumOccurred(string Value, string ToFind)
		{
			//찾지 못한 개수에 대해서도 1을 증가시키므로 
			//초기값을 -1로 만들어야 함.
			int Num = -1;

			//처음 찾을 문자열의 위치는 0이어야 함.
			//그런데 while 문 안에서 1을 항상 더하므로 -1로 만들어
			//처음 숫자가 0이 되게 함.
			int Pos = -1;

			//찾을 문자열을 찾았으면 그 다음 위치부터 다시 찾음.
			//더 이상 찾을 게 없도록 이런 과정을 반복하면서
			//반복 회수를 저장함.
			do
			{
				Pos = Value.IndexOf(ToFind, Pos + 1);
				Num++;
			} while (Pos != -1);

			return Num;
		}
		/// <summary>
		/// 문자열 안에서 char 형식의 특정 문자열이 몇번 반복되는 지 알아냄.
		/// </summary>
		/// <param name="Value"> </param>
		/// <param name="ToFind"> </param>
		/// <example>
		/// GetNumOccurred("ab%%c%%d%%", '%') //6
		/// </example>
		public static int GetNumOccurred(string Value, char ToFind)
		{
			//찾지 못한 개수에 대해서도 1을 증가시키므로 
			//초기값을 -1로 만들어야 함.
			int Num = -1;

			//처음 찾을 문자열의 위치는 0이어야 함.
			//그런데 while 문 안에서 1을 항상 더하므로 -1로 만들어
			//처음 숫자가 0이 되게 함.
			int Pos = -1;

			//찾을 문자열을 찾았으면 그 다음 위치부터 다시 찾음.
			//더 이상 찾을 게 없도록 이런 과정을 반복하면서
			//반복 회수를 저장함.
			do
			{
				Pos = Value.IndexOf(ToFind, Pos + 1);
				Num++;
			} while (Pos != -1);

			return Num;
		}


		/// <summary>
		/// 숫자를 한글로 읽는 문자열로 바꿈.
		/// </summary>
		/// <param name="Number"> </param>
		/// <example>
		/// Console.WriteLine(GetHangulFromNumber("2340001")); //이백삼십사만일
		/// </example>
		public static string GetHangulFromNumber(long Number, bool PrefixIsNumber)
		{
			if (Number == 0)
			{
				if (PrefixIsNumber)
				{
					return "0";
				}
				else
				{
					return "영";
				}
			}
			else if (Number < 0)
			{
				return "";
			}

			string sNum = Number.ToString().PadLeft(16, ' ');

			StringBuilder NewNum = new StringBuilder();
			if (PrefixIsNumber)
			{
				for (int i = 0; i < sNum.Length; i++)
				{
					switch (sNum[i])
					{
						case '0':
							NewNum.Append(' '); break;
						case '1':
							NewNum.Append('1'); break;
						case '2':
							NewNum.Append('2'); break;
						case '3':
							NewNum.Append('3'); break;
						case '4':
							NewNum.Append('4'); break;
						case '5':
							NewNum.Append('5'); break;
						case '6':
							NewNum.Append('6'); break;
						case '7':
							NewNum.Append('7'); break;
						case '8':
							NewNum.Append('8'); break;
						case '9':
							NewNum.Append('9'); break;
						default:
							NewNum.Append(' '); break;
					}
				}
			}
			else
			{
				for (int i = 0; i < sNum.Length; i++)
				{
					switch (sNum[i])
					{
						case '0':
							NewNum.Append(' '); break;
						case '1':
							NewNum.Append('일'); break;
						case '2':
							NewNum.Append('이'); break;
						case '3':
							NewNum.Append('삼'); break;
						case '4':
							NewNum.Append('사'); break;
						case '5':
							NewNum.Append('오'); break;
						case '6':
							NewNum.Append('육'); break;
						case '7':
							NewNum.Append('칠'); break;
						case '8':
							NewNum.Append('팔'); break;
						case '9':
							NewNum.Append('구'); break;
						default:
							NewNum.Append(' '); break;
					}
				}
			}


			string JoUnit = "", UkUnit = "", ManUnit = "", WonUnit = "";

			JoUnit = CFindRep.AddUnit(NewNum.ToString(NewNum.Length - 16, 4));
			UkUnit = CFindRep.AddUnit(NewNum.ToString(NewNum.Length - 12, 4));
			ManUnit = CFindRep.AddUnit(NewNum.ToString(NewNum.Length - 8, 4));
			WonUnit = CFindRep.AddUnit(NewNum.ToString(NewNum.Length - 4, 4));

			if (ManUnit != "")
			{
				ManUnit += "만";
			}
			if (UkUnit != "")
			{
				UkUnit += "억";
			}
			if (JoUnit != "")
			{
				JoUnit += "조";
			}

			return JoUnit + UkUnit + ManUnit + WonUnit;
		}
		public static string GetHangulFromNumber(long Number)
		{
			return GetHangulFromNumber(Number, false);
		}
		/// <summary>
		/// 숫자를 한자로 읽는 문자열로 바꿈.
		/// </summary>
		/// <param name="Number"> </param>
		/// <example>
		/// Console.WriteLine(GetHanjaFromNumber("2340001")); //貳百參拾四萬壹
		/// </example>
		/// <additional>
		/// GetHangulFromNumber method
		/// AddUnit method
		/// </additional>
		public static string GetHanjaFromNumber(long Number)
		{
			//壹貳參四五六七八九 拾百千 萬億兆

			string Hangul = CFindRep.GetHangulFromNumber(Number);

			//모든 한글을 확인해서 한자로 변환함.
			//각각의 경우에 해당하는 답이 하나만 있으므로 switch 함수를 썼음.
			StringBuilder NewNum = new StringBuilder();
			for (int i = 0; i < Hangul.Length; i++)
			{
				switch (Hangul[i])
				{
					case '일':
						NewNum.Append('壹'); break;
					case '이':
						NewNum.Append('貳'); break;
					case '삼':
						NewNum.Append('參'); break;
					case '사':
						NewNum.Append('四'); break;
					case '오':
						NewNum.Append('五'); break;
					case '육':
						NewNum.Append('六'); break;
					case '칠':
						NewNum.Append('七'); break;
					case '팔':
						NewNum.Append('八'); break;
					case '구':
						NewNum.Append('九'); break;
					case '십':
						NewNum.Append('拾'); break;
					case '백':
						NewNum.Append('百'); break;
					case '천':
						NewNum.Append('千'); break;
					case '만':
						NewNum.Append('萬'); break;
					case '억':
						NewNum.Append('億'); break;
					case '조':
						NewNum.Append('兆'); break;
					default:
						break;
				}
			}

			return NewNum.ToString();
		}
		/// <summary>
		/// NumToHangul 프로시저에서만 호출되며 천, 백 등의 단위를 끝에 붙임.
		/// </summary>
		/// <param name="Unit"> </param>
		private static string AddUnit(string Unit)
		{
			string One = "", Ten = "", Hundred = "", Thousand = "";

			if (Unit[0] != ' ')
			{
				Thousand = Unit[0].ToString() + "천";
			}
			if (Unit[1] != ' ')
			{
				Hundred = Unit[1].ToString() + "백";
			}
			if (Unit[2] != ' ')
			{
				Ten = Unit[2].ToString() + "십";
			}
			if (Unit[3] != ' ')
			{
				One = Unit[3].ToString();
			}

			return Thousand + Hundred + Ten + One;
		}

		/// <summary>
		/// 변수로 지정된 특정 문자열을 값으로 지정된 특정 문자열로 모두 바꿈.
		/// </summary>
		/// <param name="Value">변수를 하나 이상 가진 문자열</param>
		/// <param name="VarValueList">홀수번째는 변수 문자열이, 짝수번째는 값 문자열이 있음.</param>
		/// <example>
		/// string Value = "도서명: [[BookName]], 저자: [[Author]], 출판사: [[Publisher]]";
		/// string[] VarValueList = new String[6] {"[[BookName]]", "삼국지", 
		/// 										"[[Author]]", "이문열", 
		/// 										"[[Publisher]]", "민음사"};
		/// Value = ReplaceVariableWithValue(Value, VarValueList);
		/// Console.WriteLine(Value); // "도서명: 삼국지, 저자: 이문열, 출판사: 민음사"
		/// </example>
		public static string ReplaceVariableWithValue(string Value, string[] VarValueList)
		{
			//string의 Replace를 하기 위해 StringBuilder를 생성.
			StringBuilder sb = new StringBuilder(Value);

			for (int i = 0; i < VarValueList.Length; i += 2)
			{
				sb = sb.Replace(VarValueList[i], VarValueList[i + 1]);
			}

			return sb.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="SymbolStart"></param>
		/// <param name="SymbolEnd"></param>
		/// <param name="nvVar"></param>
		/// <returns></returns>
		/// <example>
		/// string s = "<a><b>";
		/// NameValueCollection nvVar = new NameValueCollection();
		/// nvVar.Add("a", "");
		/// nvVar.Add("b", "bbb");
		/// s = CFindRep.ReplaceVariableWithValue(s, "<", ">", nvVar); //bbb
		/// </example>
		public static string ReplaceVariableWithValue(string Value,
			string SymbolStart, string SymbolEnd,
			NameValueCollection nvVar)
		{
			for (int i = 0, i2 = nvVar.Count; i < i2; i++)
			{
				Value = Value.Replace(SymbolStart + nvVar.GetKey(i) + SymbolEnd, nvVar[i]);
			}

			return Value;
		}

		public static string TrimStartSpecific(string Value, string ToTrim, StringComparison comparisonType)
		{
			while (Value.StartsWith(ToTrim, comparisonType))
			{
				Value = Value.Substring(ToTrim.Length);
			}

			return Value;
		}
		public static string TrimEndSpecific(string Value, string ToTrim, StringComparison comparisonType)
		{
			while (Value.EndsWith(ToTrim, comparisonType))
			{
				Value = Value.Substring(0, (Value.Length - ToTrim.Length));
			}

			return Value;
		}
		public static string TrimSpecific(string Value, string[] aToTrim, StringComparison comparisonType)
		{
			string ValueOld = "";
			bool IsFound = true;
			while (IsFound == true)
			{
				IsFound = false;

				for (int i = 0; i < aToTrim.Length; i++)
				{
					int ToTrimLength = aToTrim[i].Length;

					if (ToTrimLength > Value.Length)
						continue;

					ValueOld = Value;
					Value = TrimStartSpecific(Value, aToTrim[i], comparisonType);
					if (ValueOld.Length != Value.Length)
						IsFound = true;

					if (ToTrimLength > Value.Length)
						continue;

					ValueOld = Value;
					Value = TrimEndSpecific(Value, aToTrim[i], comparisonType);
					if (ValueOld.Length != Value.Length)
						IsFound = true;
				}
			}

			return Value;
		}
		public static string TrimSpecific(string Value, string ToTrim, StringComparison comparisonType)
		{
			return TrimSpecific(Value, new string[] { ToTrim }, comparisonType);
		}

		public static string TrimNbspAndSpace(string Value)
		{
			return TrimSpecific(Value, "&nbsp;", StringComparison.CurrentCultureIgnoreCase).Trim();
		}

		public static string TrimWhiteBrPNbsp(string Value)
		{
			string[] aToTrim = new string[] { "\r", "\n", "\t", " ", "<br>", "<br/>", "<br />", "<p>", "</p>", "&nbsp;" };
			return TrimSpecific(Value, aToTrim, StringComparison.CurrentCultureIgnoreCase);
		}

		/// <summary>
		/// '\r', '\n', '\t', ' '를 모두 자름.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static string TrimWhiteSpace(string Value)
		{
			return Value.Trim('\r', '\n', '\t', ' ');
		}

		public static string RemoveWhiteSpace(string Value)
		{
			return Value
				.Replace("\r", "")
				.Replace("\n", "")
				.Replace("\t", "")
				.Replace(" ", "");
		}

		/// <summary>
		/// Except 문자열에 없는 문자는 Value에서 삭제한 후 리턴함.
		/// CFindRep.RemoveExcept("1abc234", "0123456789") -> "1234"
		/// </summary>
		public static string RemoveExcept(string Value, string Except)
		{
			string NewValue = "";

			for (int i = 0; i < Value.Length; i++)
			{
				char c = Value[i];
				if (Except.IndexOf(c) != -1)
				{
					NewValue += c;
				}
			}

			return NewValue;
		}

		/// <summary>
		/// <![CDATA[
		/// <br> <br/> 등의 여러 유형의 br 태그로 끝난다면 해당 br 태그를 모두 삭제한 값을 리턴함.
		/// ]]>
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static string TrimEndBr(string Value)
		{
			//만약 줄바꿈+<br>+탭+<br>의 경우 모두 삭제하려면 @"\s*" + CRegex.Pattern.BrTag + "$"을 사용해야 함.
			Regex r = new Regex(CRegex.Pattern.BrTag + "$", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline);
			while (r.IsMatch(Value))
			{
				Value = r.Replace(Value, "");
			}

			return Value;
		}

		private void x_()
		{
			//char[] cs = new char[3] {'나', '너', '아'};
			//byte[] bs = new Byte[2] {49, 50};
			//int len = Encoding.Unicode.GetByteCount(cs);
			//Console.WriteLine(len);

			/*
			StringBuilder sb = new StringBuilder("axbxcxd");
			sb = sb.Replace("x", "");
			Console.WriteLine(sb);
			*/

			/*
			StringBuilder sb = new StringBuilder("abcd");
			sb.Insert(1, "x");
			Console.WriteLine(sb);
			*/

			/*
			string s = "하하";
			s = s.PadLeft(3, 'x');
			Console.WriteLine(s);
			Console.ReadLine();
			*/

			//string s = "하a";
			//Console.WriteLine(s.Length.ToString());

			/*
			//가의 아스키코드는 44032임
			char c = '\uac00';
			Console.Write(c.ToInt32().ToString() + c.ToString()); //44032가
			Console.ReadLine();
			*/

			/*
			byte b = 'a'.ToByte();
			Console.WriteLine(b.ToChar());
			*/

		}

		/// <summary>
		/// 지정된 문자열을 제외한 모든 문자열을 삭제함.
		/// </summary>
		/// <param name="Value"> Except 문자열을 포함하는 문자열 </param>
		/// <param name="Except"> Value 문자열에서 지울 문자열 </param>
		/// <example>
		/// string Value = GetDeletedExcept("1,000원", "0123456789");
		/// Console.WriteLine(Value); // "1000"
		/// </example>
		public static string GetDeletedExcept(string Value, string Except)
		{
			string NewValue = "";

			for (int i = 0; i < Value.Length; i++)
			{
				//Except 문자열에 있는 문자열만 NewValue에 추가함.
				if (Except.IndexOf(Value[i]) != -1)
				{
					NewValue += Value[i];
				}
			}

			return NewValue;
		}

		/// <summary>
		/// 버전 정보를 숫자 형식으로 변환함.
		/// </summary>
		/// <param name="VersionString"> 1.0.0.35와 같은 버전 정보 </param>
		/// <example>
		/// long nVersionOld = GetNumberFromVersionInfo("1.0.0.12");
		/// long nVersionNew = GetNumberFromVersionInfo("1.0.0.2");
		/// 
		/// bool IsNeedToUpdate = (nVersionNew > nVersionOld);
		/// //1000000012, 1000000002, false
		/// Console.WriteLine("Old: {0}, New: {1}, 버전 교체 필요성: {2}", nVersionOld, nVersionNew, IsNeedToUpdate);
		/// </example>
		[Obsolete("2013-04-25 Use GetNumberFromVersionString")]
		public static long GetNumberFromVersionInfo(string VersionString)
		{
			Version Ver = GetVersionFromVersionString(VersionString);
			return GetNumberFromVersion(Ver);
		}
		[Obsolete("2013-04-25 Use GetVersionFromVersionString")]
		public static Version GetVersionFromVersionInfo(string VersionString)
		{
			////Version이 "5.3.0010.0 (xpclnt_qfe.020226-1835)"와 같이 읽혀질 수 있으므로 공백 전의 문자열만 가져옴.
			//int PosSpace = VersionInfo.IndexOf(" ");
			//if (PosSpace >= 1)
			//{
			//    VersionInfo = VersionInfo.Substring(0, PosSpace);
			//}

			int[] aNewVer = new int[4];

			//점으로 구분된 각 항목을 4자리 숫자로 만든 후 합침.
			string[] aVersionInfo = VersionString.Split(new char[] { '.' });

			//5.3.0010.0 (xpclnt_qfe.020226-1835)과 같이 숫자가 아닌 것 포함될 수 있으므로 4자리만 가져옴.
			int Max = Math.Min(4, aVersionInfo.Length);
			for (int i = 0; i < Max; i++)
			{
				string VersionCur = aVersionInfo[i];

				string VersionNew = "";
				for (int j = 0; j < VersionCur.Length; j++)
				{
					char c = VersionCur[j];

					//아래한글은 5. 7. 6. 3030 과 같이 공백이 있으므로 Trim
					if (!char.IsNumber(c) && (c != ' '))
						break;

					if (c == ' ')
						c = '0';

					VersionNew += c;
				}
				aNewVer[i] = Convert.ToInt32(VersionNew);
			}

			Version NewVer = new Version(aNewVer[0], aNewVer[1], aNewVer[2], aNewVer[3]);

			return NewVer;
		}
		public static long GetNumberFromVersionString(string VersionString)
		{
			Version Ver = GetVersionFromVersionString(VersionString);
			return GetNumberFromVersion(Ver);
		}
		public static Version GetVersionFromVersionString(string VersionString)
		{
			////Version이 "5.3.0010.0 (xpclnt_qfe.020226-1835)"와 같이 읽혀질 수 있으므로 공백 전의 문자열만 가져옴.
			//int PosSpace = VersionInfo.IndexOf(" ");
			//if (PosSpace >= 1)
			//{
			//    VersionInfo = VersionInfo.Substring(0, PosSpace);
			//}

			int[] aNewVer = new int[4];

			//점으로 구분된 각 항목을 4자리 숫자로 만든 후 합침.
			string[] aVersionInfo = VersionString.Split(new char[] { '.' });

			//5.3.0010.0 (xpclnt_qfe.020226-1835)과 같이 숫자가 아닌 것 포함될 수 있으므로 4자리만 가져옴.
			int Max = Math.Min(4, aVersionInfo.Length);
			for (int i = 0; i < Max; i++)
			{
				string VersionCur = aVersionInfo[i];

				string VersionNew = "";
				for (int j = 0; j < VersionCur.Length; j++)
				{
					char c = VersionCur[j];

					//아래한글은 5. 7. 6. 3030 과 같이 공백이 있으므로 Trim
					if (!char.IsNumber(c) && (c != ' '))
						break;

					if (c == ' ')
						c = '0';

					VersionNew += c;
				}
				aNewVer[i] = Convert.ToInt32(VersionNew);
			}

			Version NewVer = new Version(aNewVer[0], aNewVer[1], aNewVer[2], aNewVer[3]);

			return NewVer;
		}
		public static long GetNumberFromVersion(Version Ver)
		{
			string NewVersionInfo = "";

			for (int i = 0; i < 4; i++)
			{
				int VerCur = CLang.Choose(i, Ver.Major, Ver.Minor, Ver.Build, Ver.Revision);

				NewVersionInfo += CFindRep.Right(VerCur.ToString().PadLeft(4, '0'), 4);
			}

			return Convert.ToInt64(NewVersionInfo);			
		}
		
		public static string FormatPercent(float Value, int NumDigitsAfterDecimal)
		{
			return String.Format("{0:P" + NumDigitsAfterDecimal.ToString() + "}", Value);
		}
		public static string FormatPercent(double Value, int NumDigitsAfterDecimal)
		{
			return String.Format("{0:P" + NumDigitsAfterDecimal.ToString() + "}", Value);
		}
		public static string FormatNumber(float Value, int NumDigitsAfterDecimal)
		{
			return String.Format("{0:N" + NumDigitsAfterDecimal.ToString() + "}", Value);
		}

		/// <summary>
		/// 상품권 번호는 대부분 4자리 숫자로 나눠짐.
		/// 단, 컬쳐랜드의 경우는 마지막이 6자리이며, 첫번째 2자리는 알파벳인 경우도 있음.
		/// </summary>
		public static string FormatVoucherCode(string Value, int Unit)
		{
			Regex rDelete = new Regex(@"([^0-9|a-z|A-Z]+)");
			Value = rDelete.Replace(Value, "").ToUpper();

			Regex rDash = new Regex(string.Concat("(\\w{", Unit, "})(\\w{", Unit, "})"));
			while (rDash.IsMatch(Value))
			{
				Value = rDash.Replace(Value, "$1-$2");
			}

			return Value;
		}

		/// <summary>
		/// 전화번호 안에 자릿수에 맞게 -를 넣어 리턴함.
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// List<string> aValue = new List<string>();
		/// aValue.Add("121234");
		/// aValue.Add("12341234");
		/// aValue.Add("053-92-1234");
		/// aValue.Add("053-999-1234");
		/// aValue.Add("021231234");
		/// aValue.Add("0101231234");
		/// aValue.Add("01912341234");
		/// aValue.Add("0311231234");
		/// aValue.Add("82101231234");
		/// foreach (string Value in aValue)
		/// {
		/// 	Debug.WriteLine(CFindRep.FormatPhoneNumber(Value));
		/// }
		/// ]]>
		/// </example>
		public static string FormatPhoneNumber(string Value)
		{
			if (!CValid.IsDigit(Value))
			{
				Regex rDelete = new Regex(@"([^\d]+)");
				Value = rDelete.Replace(Value, "");

				//Value = CFindRep.RemoveExcept(Value, "0123456789");
			}

			if (Value.Length <= 4)
			{
				return Value;
			}

			if (Value.Length <= 8)
			{
				string Value2 = Value.Substring(Value.Length - 4);
				string Value1 = Value.Substring(0, Value.Length - Value2.Length);
				return Value1 + "-" + Value2;
			}

			string[] aFirst = new string[]
			{
				"010", "011", "012", "015", "016", "017", "018", "019",
				"030", "050", "060", "070", "080",
				"02", "031", "032", "033", "041", "042", "043", "044", "049", "051", "052", "053", "054", "055", "061", "062", "063", "064"
			};

			int IndexFirst = CArray.StartsWithAny(Value, aFirst);
			if (IndexFirst != -1)
			{
				string Value1 = aFirst[IndexFirst];

				Value = Value.Substring(Value1.Length);

				if (Value.Length <= 4)
				{
					return Value1 + "-" + Value;
				}
				else
				{
					string Value3 = Value.Substring(Value.Length - 4);
					string Value2 = Value.Substring(0, Value.Length - Value3.Length);

					return Value1 + "-" + Value2 + "-" + Value3;
				}
			}

			return Value;
		}

		/// <summary>
		/// 사업자번호 안에 자릿수에 맞게 -를 넣어 리턴함.
		/// </summary>
		public static string FormatSaupNo(string Value)
		{
			if (!CValid.IsDigit(Value))
			{
				Regex rDelete = new Regex(@"([^\d]+)");
				Value = rDelete.Replace(Value, "");
			}

			Regex rDash = new Regex("(\\w{3})(\\w{2})(\\w{5})");
			Value = rDash.Replace(Value, "$1-$2-$3");

			return Value;
		}

		/// <summary>
		/// 행은 \r로 구분되고 열은 \t로 구분되어 붙여넣기가 가능한 지를 알아냄.
		/// </summary>
		/// <param name="ClipData"></param>
		/// <returns></returns>
		public static bool IsPastable(string ClipData)
		{
			if (ClipData == "") return false;

			//최소한 \t으로 구분된 하나 이상의 열이 있어야 함.
			if (ClipData.IndexOf('\t') == -1) return false;

			string[] Rows = ClipData.Split('\r');

			//2행 이상이면 첫번째 행과 두번째 행의 열수는 같아야 함.
			if (Rows.Length > 1)
			{
				string[] RowFirst = Rows[0].Split('\t');
				string[] RowSecond = Rows[1].Split('\t');
				if (RowFirst.Length != RowSecond.Length)
				{
					return false;
				}
			}

			return true;
		}

		public static string Repeat(string Value, int Num)
		{
			string ValueList = "";
			for (int i = 0, i2 = Num; i < i2; i++)
			{
				ValueList += Value;
			}
			return ValueList;
		}
		public static string Repeat(char Value, int Num)
		{
			return Repeat(Value.ToString(), Num);
		}

		/// <summary>
		/// <paramref name="Value"/>의 오른쪽에서 <paramref name="Length"/> 길이만큼의 문자열을 리턴함.
		/// </summary>
		/// <param name="Value">전체 문자열</param>
		/// <param name="Length">오른쪽부터 가져올 문자열의 길이</param>
		/// <returns>
		/// <paramref name="Value"/>의 오른쪽에서 <paramref name="Length"/> 길이만큼의 문자열을 리턴함.
		/// </returns>
		public static string Right(string Value, int Length)
		{
			int Start = Value.Length - Length;
			if (Start < 0)
				Start = 0;

			return Value.Substring(Start);
		}
		/// <summary>
		/// <paramref name="Value"/>의 왼쪽에서 <paramref name="Length"/> 길이만큼의 문자열을 리턴함.
		/// </summary>
		/// <param name="Value">전체 문자열</param>
		/// <param name="Length">왼쪽부터 가져올 문자열의 길이</param>
		/// <returns>
		/// <paramref name="Value"/>의 왼쪽에서 <paramref name="Length"/> 길이만큼의 문자열을 리턴함.
		/// </returns>
		public static string Left(string Value, int Length)
		{
			if (Length > Value.Length)
				Length = Value.Length;

			return Value.Substring(0, Length);
		}
		public static string SubstringFromTo(string Value, int StartIndex, int EndIndex)
		{
			int Length = (EndIndex - StartIndex + 1);
			return Value.Substring(StartIndex, Length);
		}

		public static DateTime IfNotDateTimeThen1Year1Month1Day(string Value)
		{
			DateTime d;

			if (Value == null)
				return new DateTime(1, 1, 1);
			
			if (!DateTime.TryParse(Value, out d))
				return new DateTime(1, 1, 1);
			else
				return d;
		}

		public static DateTime IfNotDateTimeThen19000101(string Value)
		{
			DateTime d;

			if (Value == null)
				return CDateTime.DateMinValueForDb19000101;

			if (!DateTime.TryParse(Value, out d))
				return CDateTime.DateMinValueForDb19000101;
			else
				return d;
		}
		public static DateTime IfNotDateTimeThen29991231(string Value)
		{
			DateTime d;

			if (Value == null)
				return CDateTime.DateMaxValueForDb29991231;

			if (!DateTime.TryParse(Value, out d))
				return CDateTime.DateMaxValueForDb29991231;
			else
				return d;
		}
		
		public static DateTime IfNotDateTimeThenNow(string Value)
		{
			DateTime d;

			if (Value == null)
				return DateTime.Now;

			if (!DateTime.TryParse(Value, out d))
				return DateTime.Now;
			else
				return d;
		}
		public static DateTime IfNotDateTimeThen(string Value, DateTime DefaultValue)
		{
			DateTime d;

			if (Value == null)
				return DefaultValue;

			if (!DateTime.TryParse(Value, out d))
				return DefaultValue;
			else
				return d;
		}

		public static DateTime If19000101Then(DateTime Value, DateTime DefaultValue)
		{
			if (Value == CDateTime.DateMinValueForDb19000101)
				return DefaultValue;
			else
				return Value;
		}
		public static object If19000101ThenDBNull(DateTime Value)
		{
			if (Value == CDateTime.DateMinValueForDb19000101)
				return DBNull.Value;
			else
				return Value;
		}

		public static string If1Year1Month1DayThen0Length(DateTime Value, string Format)
		{
			if (Value == (new DateTime(1, 1, 1)))
			{
				return "";
			}
			else
			{
				return Value.ToString(Format);
			}
		}
		public static string If1Year1Month1DayThen0Length(DateTime Value)
		{
			return If1Year1Month1DayThen0Length(Value, "yyyy-MM-dd HH:mm:dd");
		}

		[Obsolete("use IfNullOrEmptyThen")]
		public static string If0LengthOrNullThen(string Value, string ValueNew)
		{
			return (!string.IsNullOrEmpty(Value)) ? Value : ValueNew;
		}
		public static string IfNullOrEmptyThen(string Value, string ValueNew)
		{
			return (!string.IsNullOrEmpty(Value)) ? Value : ValueNew;
		}

		public static string IfTrueThen1FalseThen0(bool Value)
		{
			return (Value == true) ? "1" : "0";
		}
		public static string IfTrueThenYeFalseThenAniyo(bool Value)
		{
			return (Value == true) ? "예" : "아니요";
		}
		public static string IfTrueThenYeFalseThenAniyo(bool? Value)
		{
			if (Value == null)
				return "";

			return IfTrueThenYeFalseThenAniyo(Value.Value);
		}
		public static bool If1ThenTrue0ThenFalse(object Value)
		{
			if (Value.ToString() == "1")
				return true;
			else if (Value.ToString() == "0")
				return false;
			else
				throw new Exception("'0', '1' 이외의 값입니다.");
		}

		public static string ReplaceByList(string Value, Hashtable htList)
		{
			string NewValue = "";
			for (int i = 0, i2 = Value.Length; i < i2; i++)
			{
				char c = Value[i];
				if (htList.ContainsKey(c))
				{
					NewValue += (char)htList[c];
				}
				else
				{
					NewValue += c;
				}
			}

			return NewValue;
		}
		public static string ReplaceByList(string Value, List<char> aList, string ValueToReplace)
		{
			string NewValue = "";
			for (int i = 0, i2 = Value.Length; i < i2; i++)
			{
				char c = Value[i];
				if (aList.IndexOf(c) != -1)
				{
					NewValue += ValueToReplace;
				}
				else
				{
					NewValue += c;
				}
			}

			return NewValue;
		}

		/// <summary>
		/// 스팸 방지를 위해 이메일 주소를 못 알아보게 변경함.
		/// </summary>
		/// <param name="Email"></param>
		/// <returns></returns>
		/// <example>
		/// <![CDATA[
		/// string s = "doctorgu@naver.com";
		/// Console.WriteLine(ReplaceEmailForSpam(s)); //do******@naver.com
		/// ]]>
		/// </example>
		public static string ReplaceEmailForSpam(string Email)
		{
			char[] aEmail = Email.ToCharArray();
			for (int i = 0, i2 = aEmail.Length; i < i2; i++)
			{
				if (aEmail[i] == '@')
					break;

				if (i >= 2)
					aEmail[i] = '*';
			}

			return new string(aEmail);
		}

		/// <summary>
		/// 대소문자를 무시하고 변경된 값을 리턴함.
		/// </summary>
		/// <param name="original"></param>
		/// <param name="pattern"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		public static string ReplaceNoCase(string original, string pattern, string replacement)
		{
			int count, position0, position1;
			count = position0 = position1 = 0;

			string upperString = original.ToUpper();
			string upperPattern = pattern.ToUpper();

			int inc = (original.Length / pattern.Length) *
					  (replacement.Length - pattern.Length);

			char[] chars = new char[original.Length + Math.Max(0, inc)];

			while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
			{
				for (int i = position0; i < position1; ++i)
				{
					chars[count++] = original[i];
				}
				for (int i = 0; i < replacement.Length; ++i)
				{
					chars[count++] = replacement[i];
				}
				position0 = position1 + pattern.Length;
			}

			if (position0 == 0)
				return original;

			for (int i = position0; i < original.Length; ++i)
			{
				chars[count++] = original[i];
			}

			return new string(chars, 0, count);
		}

		public static string GetPhoneNumberForDisplay(string Value1, string Value2, string Value3)
		{
			if (string.IsNullOrEmpty(Value1))
				return "";

			return Value1 + "-" + Value2 + "-" + Value3;
		}

		public static Tuple<string, string, string> SplitPhoneNumber(string NumberAll)
		{
			string Number1 = "";
			string Number2 = "";
			string Number3 = "";

			if (string.IsNullOrEmpty(NumberAll))
				return new Tuple<string, string, string>(Number1, Number2, Number3);


			if ((NumberAll.IndexOf('.') != -1) || (NumberAll.IndexOf('-') != -1) || (NumberAll.IndexOf(' ') != -1))
			{
				char Delim = ' ';
				if (NumberAll.IndexOf('.') != -1)
					Delim = '.';
				else if (NumberAll.IndexOf('-') != -1)
					Delim = '-';
				else if (NumberAll.IndexOf(' ') != -1)
					Delim = ' ';

				CArray.SplitToVariable(NumberAll, Delim, out Number1, out Number2, out Number3);
			}
			else if (NumberAll.Length == 11)
			{
				Number1 = NumberAll.Substring(0, 3);
				Number2 = NumberAll.Substring(3, 4);
				Number3 = NumberAll.Substring(7);
			}
			else if (NumberAll.Length == 10)
			{
				if (NumberAll.Substring(0, 2) == "02")
				{
					Number1 = NumberAll.Substring(0, 2);
					Number2 = NumberAll.Substring(2, 4);
					Number3 = NumberAll.Substring(6);
				}
				else
				{
					Number1 = NumberAll.Substring(0, 3);
					Number2 = NumberAll.Substring(3, 3);
					Number3 = NumberAll.Substring(6);
				}
			}
			else if (NumberAll.Length == 9)
			{
				if (NumberAll.Substring(0, 2) == "02")
				{
					Number1 = NumberAll.Substring(0, 2);
					Number2 = NumberAll.Substring(2, 3);
					Number3 = NumberAll.Substring(5);
				}
			}
			else if (NumberAll.Length == 8)
			{
				Number2 = NumberAll.Substring(0, 4);
				Number3 = NumberAll.Substring(4);
			}
			else if (NumberAll.Length == 7)
			{
				Number2 = NumberAll.Substring(0, 3);
				Number3 = NumberAll.Substring(3);
			}

			return new Tuple<string, string, string>(Number1, Number2, Number3);
		}

		/// <summary>
		/// <paramref name="Value"/>가 숫자 형식이면 원래 값을 리턴하고, 아니면 0을 리턴함.
		/// </summary>
		/// <param name="Value">값</param>
		/// <returns>숫자 형식이면 원래 값을 리턴하고, 아니면 0을 리턴함.</returns>
		public static double IfNaNThen0(double Value)
		{
			return double.IsNaN(Value) ? 0d : Value;
		}

		public static string TailDotDotDot(string Value, int Length)
		{
			if (string.IsNullOrEmpty(Value))
				return "";

			if (Value.Length > Length)
				return Value.Substring(0, Length - 3) + "...";
			else
				return Value;
		}

		//http://www.codeproject.com/KB/string/fastestcscaseinsstringrep.aspx
		public static string ReplaceIgnoreCase(string Original, string OldValue, string NewValue)
		{
			int Count, Position0, Position1;
			Count = Position0 = Position1 = 0;

			string UpperOriginal = Original.ToUpper();
			string UpperOldValue = OldValue.ToUpper();

			int Inc = (Original.Length / OldValue.Length) * (NewValue.Length - OldValue.Length);
			char[] aChar = new char[Original.Length + Math.Max(0, Inc)];

			while ((Position1 = UpperOriginal.IndexOf(UpperOldValue, Position0)) != -1)
			{
				for (int i = Position0; i < Position1; ++i)
					aChar[Count++] = Original[i];

				for (int i = 0; i < NewValue.Length; ++i)
					aChar[Count++] = NewValue[i];

				Position0 = Position1 + OldValue.Length;
			}

			if (Position0 == 0)
				return Original;

			for (int i = Position0; i < Original.Length; ++i)
				aChar[Count++] = Original[i];

			return new string(aChar, 0, Count);
		}

		/// <summary>
		/// "d   d     d" -> "d d d"
		/// </summary>
		/// <param name="Text"></param>
		/// <returns></returns>
		public static string ReplaceSpacesToOne(string Text)
		{
			while (Text.IndexOf("  ") != -1)
			{
				Text = Text.Replace("  ", " ");
			}

			return Text;
		}
		public static string ReplaceWhiteSpacesToOneSpace(string Value)
		{
			StringBuilder sb = new StringBuilder(Value.Length);

			bool IsWhiteOld = false, IsWhite = false;
			for (int i = 0; i < Value.Length; i++)
			{
				char c = Value[i];

				IsWhiteOld = IsWhite;
				
				IsWhite = false;
				switch (c)
				{
					case '\r':
					case '\n':
					case '\t':
					case ' ':
						IsWhite = true;
						if (!IsWhiteOld)
							sb.Append(' ');
						break;
					default:
						sb.Append(c);
						break;
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// 어떤 데이터는 행구분자가 \r만, 또는 \n만, 아니면 \r\n가 될 수 있으므로
		/// 이런 것을 모두 \n로 변경함.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static string ReplaceLineBreakToNewLine(string Value)
		{
			return Value
				.Replace("\r\n", "\n")
				.Replace("\r", "\n");
		}

		public static int GetWonFromHangulComma(string Value)
		{
			if (CValid.IsNumber(Value))
			{
				return Convert.ToInt32(Value);
			}
			else if (CValid.IsCurrencyWon(Value))
			{
				//123,456원 -> 123456
				Value = Value
					.Replace(",", "")
					.Replace("원", "");

				return Convert.ToInt32(Value);
			}
			else if (CArray.ContainsInAny(Value, "일", "십", "백", "천", "만", "억", "조") != -1)
			{
				//100백만원 -> 100 000 000
				Value = Value
					.Replace("백원", "00")
					.Replace("천원", "000")
					.Replace("만원", "0000")
					.Replace("십만원", "00000")
					.Replace("백만원", "000000")
					.Replace("천만원", "0000000")
					.Replace("억원", "00000000")
					.Replace("십억원", "000000000")
					.Replace("백억원", "0000000000")
					.Replace("천억원", "00000000000")
					.Replace("조원", "000000000000")
					.Replace("십조원", "0000000000000")
					.Replace("백조원", "00000000000000")
					.Replace("천조원", "000000000000000");

				//이만원 -> 이0000 -> 20000
				Value = Value
					.Replace("일", "1")
					.Replace("이", "2")
					.Replace("삼", "3")
					.Replace("사", "4")
					.Replace("오", "5")
					.Replace("육", "6")
					.Replace("칠", "7")
					.Replace("팔", "8")
					.Replace("구", "9");

				//만원 -> 0000 -> 10000
				if (Value.StartsWith("0"))
					Value = "1" + Value;

				return Convert.ToInt32(Value);
			}
			else
			{
				throw new Exception(string.Format("{0} is not Won type", Value));
			}
		}
	}
}
