using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;

namespace DoctorGu
{
	/// <summary>
	/// 배열과 관련된 기능 구현
	/// </summary>
	public class CArray
	{
		public CArray()
		{
		}

		/// <summary>
		/// <paramref name="Value"/>가 <paramref name="aList"/> 중 한 항목으로라도 시작된다면 
		/// 그 항목의 Index를 리턴함.
		/// </summary>
		/// <param name="Value">검사할 값</param>
		/// <param name="aList">시작값을 가진 배열</param>
		/// <returns>찾은 Index, 못 찾았을 때는 -1</returns>
		/// <example>
		/// <code>
		/// string[] aList = new string[]{"가자", "가고", "가니"};
		/// Console.WriteLine(CArray.StartsWithAny("가고파", aList)); //1
		/// Console.WriteLine(CArray.StartsWithAny("가지마", aList)); //-1
		/// </code>
		/// </example>
		public static int StartsWithAny(string Value, StringComparison comparisonType, params string[] aList)
		{
			for (int i = 0, i2 = aList.Length; i < i2; i++)
			{
				if (Value.StartsWith(aList[i], comparisonType))
				{
					return i;
				}
			}

			return -1;
		}
		public static int StartsWithAny(string Value, params string[] aList)
		{
			return StartsWithAny(Value, StringComparison.CurrentCulture, aList);
		}

		[Obsolete("Use IndexOfAny 20150505")]
		public static int ContainsInAny(string Value, StringComparison comparisonType, params string[] aList)
		{
			for (int i = 0, i2 = aList.Length; i < i2; i++)
			{
				if (Value.IndexOf(aList[i], comparisonType) != -1)
				{
					return i;
				}
			}

			return -1;
		}
		[Obsolete("Use IndexOfAny 20150505")]
		public static int ContainsInAny(string Value, params string[] aList)
		{
			return ContainsInAny(Value, StringComparison.CurrentCulture, aList);
		}

		public static int IndexOfAny(string Value, StringComparison comparisonType, params string[] aList)
		{
			for (int i = 0, i2 = aList.Length; i < i2; i++)
			{
				if (Value.IndexOf(aList[i], comparisonType) != -1)
				{
					return i;
				}
			}

			return -1;
		}
		public static int IndexOfAny(string Value, params string[] aList)
		{
			return IndexOfAny(Value, StringComparison.CurrentCulture, aList);
		}

		/// <summary>
		/// 구분자가 콤마일 때, 값에 콤마가 있어서 콤마 두 개를 입력한 경우,
		/// 해당 값을 제대로 가져오기 위함.
		/// </summary>
		/// <param name="Expression">콤마로 구분된 값</param>
		/// <returns>콤마를 구분자로 나눠진 결과 배열</returns>
		/// <example>
		/// <code>
		/// string a = "a,,b,c";
		/// string[] aa = CArray.SplitComma(a);
		/// Console.WriteLine(aa[0]); //a,b
		/// Console.WriteLine(aa[1]); //c
		/// </code>
		/// </example>
		public static string[] SplitComma(string Expression)
		{
			return Split2(Expression, ',');
		}

		/// <summary>
		/// 구분자로 구별되는 항목 자체에 구분자를 표현하기 위해 두 개의 연속된 구분자를 입력한 경우,
		/// 두 개의 연속된 구분자는 항목으로 취급하기 위함.
		/// </summary>
		/// <param name="Value">구분자가 포함된 값 목록</param>
		/// <param name="Delim">구분자</param>
		/// <returns>구분자로 나눠진 결과 배열</returns>
		/// <example>
		/// <code>
		/// string s = "서울/부산/울릉도//독도";
		/// string[] a = CArray.Split2(s, '/');
		/// Console.WriteLine(a[0]); //서울
		/// Console.WriteLine(a[1]); //부산
		/// Console.WriteLine(a[2]); //울릉도/독도
		/// </code>
		/// </example>
		public static string[] Split2(string Value, char Delim)
		{
			const char drDelim = (char)4;

			if (Value.IndexOf((Delim.ToString() + Delim.ToString())) == -1)
			{
				return Value.Split(Delim);
			}

			if (Value.IndexOf(drDelim) != -1)
				Value = Value.Replace(drDelim.ToString(), "");

			string NewValue = Value.Replace((Delim.ToString() + Delim.ToString()), drDelim.ToString());
			string[] aValue = NewValue.Split(Delim);
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				if (aValue[i].IndexOf(drDelim) != -1)
				{
					aValue[i] = aValue[i].Replace(drDelim.ToString(), Delim.ToString());
				}
			}

			return aValue;
		}

		/// <summary>
		/// 구분자로 구별되는 항목 자체에 구분자를 표현하기 위해 두 개의 연속된 구분자를 입력한 경우,
		/// 두 개의 연속된 구분자는 항목으로 취급하기 위함.
		/// </summary>
		public static string Join2(char Separator, string[] aValue)
		{
			string Found = aValue.FirstOrDefault(s => s.IndexOf(Separator) != -1);
			if (Found == null)
			{
				return string.Join(Separator.ToString(), aValue);
			}
			else
			{
				string[] aValue2 = new string[aValue.Length];
				aValue.CopyTo(aValue2, 0);

				for (int i = 0; i < aValue2.Length; i++)
				{
					if (aValue2[i].IndexOf(Separator) != -1)
						aValue2[i] = aValue2[i].Replace(Separator.ToString(), string.Concat(Separator, Separator));
				}

				return string.Join(Separator.ToString(), aValue2);
			}
		}

		/// <summary>
		/// 배열의 값이 null인 경우엔 Length 속성을 읽을 때 에러가 나므로
		/// 그런 경우에도 에러를 나지 않도록 하기 위함.
		/// </summary>
		/// <param name="a">배열</param>
		/// <returns>배열의 길이, 배열이 null이거나 배열 형식이 아닌 경우엔 -1</returns>
		/// <example>
		/// <code>
		/// string[] a = null;
		/// string[] b = new string[] { "가", "나" };
		/// Console.WriteLine(CArray.SafeLength(a)); //-1
		/// Console.WriteLine(CArray.SafeLength(b)); //2
		/// </code>
		/// </example>
		public static int SafeLength(Array a)
		{
			int Len = -1;
			try { Len = a.Length; }
			catch (Exception) { }

			return Len;
		}

		/// <summary>
		/// 배열의 항목 중 첫번째 항목을 삭제함.
		/// </summary>
		/// <param name="aValue">배열</param>
		/// <returns>첫번째 항목이 삭제된 배열</returns>
		/// <example>
		/// <code>
		/// string[] a = new string[] { "a", "b", "c" };
		/// string[] b = CArray.RemoveFirst(a);
		/// Console.WriteLine(a[0]); //a
		/// Console.WriteLine(b[0]); //b
		/// </code>
		/// </example>
		public static string[] RemoveFirst(string[] aValue)
		{
			string[] aValueNew = new string[aValue.Length - 1];
			Array.ConstrainedCopy(aValue, 1, aValueNew, 0, aValue.Length - 1);
			return aValueNew;
		}

		/// <summary>
		/// 구분자로 구분된 항목들을 변수에 차례대로 설정함.
		/// </summary>
		/// <param name="ValueList">구분자로 구분된 항목의 목록</param>
		/// <param name="Delim">구분자</param>
		/// <param name="Value1Is">1번째 항목 값을 가지는 변수</param>
		/// <param name="Value2Is">2번째 항목 값을 가지는 변수</param>
		/// <param name="Value3Is">3번째 항목 값을 가지는 변수</param>
		/// <param name="Value4Is">4번째 항목 값을 가지는 변수</param>
		/// <example>
		/// <code>
		/// string List = "a,bc";
		/// string a, b;
		/// CArray.SplitToVariable(List, ',', out a, out b);
		/// Console.WriteLine(a); //a
		/// Console.WriteLine(b); //bc		
		/// </code>
		/// </example>
		public static void SplitToVariable(string ValueList, char Delim,
			out string Value1Is, out string Value2Is, out string Value3Is, out string Value4Is)
		{
			Value1Is = "";
			Value2Is = "";
			Value3Is = "";
			Value4Is = "";

			string[] aValue = ValueList.Split(Delim);
			if (aValue.Length >= 1)
			{
				Value1Is = aValue[0];
			}
			if (aValue.Length >= 2)
			{
				Value2Is = aValue[1];
			}
			if (aValue.Length >= 3)
			{
				Value3Is = aValue[2];
			}
			if (aValue.Length >= 4)
			{
				Value4Is = aValue[3];
			}
		}
		public static void SplitToVariable(string ValueList, char Delim,
			out string Value1Is, out string Value2Is, out string Value3Is)
		{
			Value1Is = "";
			Value2Is = "";
			Value3Is = "";
			string Value4Is;

			SplitToVariable(ValueList, Delim, out Value1Is, out Value2Is, out Value3Is, out Value4Is);
		}
		public static void SplitToVariable(string ValueList, char Delim, 
			out string Value1Is, out string Value2Is)
		{
			Value1Is = "";
			Value2Is = "";
			string Value3Is;
			string Value4Is;

			SplitToVariable(ValueList, Delim, out Value1Is, out Value2Is, out Value3Is, out Value4Is);
		}
		public static void SplitToVariable(string ValueList, char Delim,
			out int Value1Is, out int Value2Is, out int Value3Is, out int Value4Is)
		{
			Value1Is = 0;
			Value2Is = 0;
			Value3Is = 0;
			Value4Is = 0;

			string sValue1Is;
			string sValue2Is;
			string sValue3Is;
			string sValue4Is;
			SplitToVariable(ValueList, Delim, out sValue1Is, out sValue2Is, out sValue3Is, out sValue4Is);

			Value1Is = CFindRep.IfNotNumberThen0(sValue1Is);
			Value2Is = CFindRep.IfNotNumberThen0(sValue2Is);
			Value3Is = CFindRep.IfNotNumberThen0(sValue3Is);
			Value4Is = CFindRep.IfNotNumberThen0(sValue4Is);
		}
		public static void SplitToVariable(string ValueList, char Delim,
			out int Value1Is, out int Value2Is, out int Value3Is)
		{
			Value1Is = 0;
			Value2Is = 0;
			Value3Is = 0;
			int Value4Is;

			SplitToVariable(ValueList, Delim, out Value1Is, out Value2Is, out Value3Is, out Value4Is);
		}
		public static void SplitToVariable(string ValueList, char Delim,
			out int Value1Is, out int Value2Is)
		{
			Value1Is = 0;
			Value2Is = 0;
			int Value3Is;
			int Value4Is;

			SplitToVariable(ValueList, Delim, out Value1Is, out Value2Is, out Value3Is, out Value4Is);
		}
		public static void SplitToVariable(string ValueList, char Delim,
			out string Value1Is, out int Value2Is, out int Value3Is)
		{
			Value1Is = "";
			Value2Is = 0;
			Value3Is = 0;
			
			string sValue2Is;
			string sValue3Is;
			string sValue4Is;
			SplitToVariable(ValueList, Delim, out Value1Is, out sValue2Is, out sValue3Is, out sValue4Is);

			Value2Is = CFindRep.IfNotNumberThen0(sValue2Is);
			Value3Is = CFindRep.IfNotNumberThen0(sValue3Is);
		}

		/// <summary>
		/// <paramref name="Value"/>의 문자열을 특정 길이만큼 잘라서 배열을 만듦.
		/// </summary>
		/// <param name="Value">대상 문자열</param>
		/// <param name="Length">자를 길이</param>
		/// <param name="IsTwoByte">2바이트 문자열의 길이를 2로 취급할 지 여부</param>
		/// <returns>각 항목이 특정 길이를 넘지 않는 배열</returns>
		/// <example>
		/// <code>
		/// string Value = "a01나03";
		///
		/// string[] aValue1 = CArray.SplitByLength(Value, 3, false);
		/// string[] aValue2 = CArray.SplitByLength(Value, 3, true);
		///
		/// Console.WriteLine(string.Join(",", aValue1)); //a01,나03
		/// Console.WriteLine(string.Join(",", aValue2)); //a01,나0,3		
		/// </code>
		/// </example>
		public static string[] SplitByLength(string Value, int Length, bool IsTwoByte)
		{
			List<string> aValue = new List<string>();

			bool IsEnd = false;
			//처음엔 (Value != "") 로 검사했으니 마지막에 ModIs가 1이 되면 무한 루프
			//발생하여 IsEnd로 체크함.
			int LenCur = 0;
			if (!IsTwoByte)
			{
				while (!IsEnd)
				{
					LenCur = Value.Length;
					if (LenCur > Length)
					{
						LenCur = Length;
					}
					else
					{
						IsEnd = true;
					}

					aValue.Add(Value.Substring(0, LenCur));
					Value = Value.Substring(LenCur);
				}
			}
			else
			{
				while (!IsEnd)
				{
					LenCur = CTwoByte.LenH(Value);
					if (LenCur > Length)
					{
						LenCur = Length;
					}
					else
					{
						IsEnd = true;
					}

					int ModIs;
					aValue.Add(CTwoByte.SubstringH(Value, 0, LenCur, out ModIs));

					Value = CTwoByte.SubstringH(Value, (LenCur - ModIs));
				}
			}

			//return (string[])aValue.ToArray(typeof(System.String));
			return aValue.ToArray();
		}
		/// <summary>
		/// <paramref name="Value"/>의 문자열을 특정 길이만큼 잘라서 배열을 만듦.
		/// </summary>
		/// <param name="Value">대상 문자열</param>
		/// <param name="Length">자를 길이</param>
		/// <returns>각 항목이 특정 길이를 넘지 않는 배열</returns>
		/// <example>
		/// <code>
		/// string Value = "a01나03";
		/// string[] aValue1 = CArray.SplitByLength(Value, 3);
		/// Console.WriteLine(string.Join(",", aValue1)); //a01,나03
		/// </code>
		/// </example>
		public static string[] SplitByLength(string Value, int Length)
		{
			return SplitByLength(Value, Length, false);
		}

		/// <summary>
		/// Array.IndexOf, Array.BinarySearch는 Case Sensitive 형식이므로
		/// Case Insensitive 형식으로도 찾을 수 있도록 함.
		/// </summary>
		/// <param name="aValue"></param>
		/// <param name="Find"></param>
		/// <param name="comparisonType"></param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// int i = CArray.IndexOf(new string[] { "a", "b" }, "B", true);
		/// Console.WriteLine(i.ToString()); // 1
		/// 
		/// int i2 = CArray.IndexOf(new string[] { "a", "b" }, "B", false);
		/// Console.WriteLine(i2.ToString()); // -1
		/// </code>
		/// </example>
		public static int IndexOf(string[] aValue, string Find, bool IgnoreCase)
		{
			StringComparison comparisonType = IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
			return IndexOf(aValue, Find, comparisonType);
		}
		public static int IndexOf(string[] aValue, string Find, StringComparison comparisonType)
		{
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				if (string.Compare(aValue[i], Find, comparisonType) == 0)
				{
					return i;
				}
			}

			return -1;
		}
		public static int IndexOf(char[] aValue, char Find)
		{
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				if (aValue[i] == Find)
				{
					return i;
				}
			}

			return -1;
		}
		public static int IndexOf(int[] aValue, int Find)
		{
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				if (aValue[i] == Find)
				{
					return i;
				}
			}

			return -1;
		}

		public static string ToHtml(string[] aValue)
		{
			string s = "";

			s += "<table border=1 cellpadding=0 cellspacing=0>\r\n";
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				s += "<tr><td>" + i.ToString() + "</td><td>" + aValue[i] + "</td></tr>\r\n";
			}
			s += "</table>";

			return s;
		}

		/// <summary>
		/// 줄바꿈에는 \r\n과 \n이 둘다 허용되므로 모든 경우를 줄바꿈으로 인정해서 Split하기 위함.
		/// </summary>
		/// <param name="Value"></param>
		/// <example>
		/// string s = "a\n\nb\nc";
		/// string[] a = CArray.SplitLineBreak(s, StringSplitOptions.None); //'a', '', 'b', 'c'
		/// string[] a = CArray.SplitLineBreak(s, StringSplitOptions.RemoveEmptyEntries); //'a', 'b', 'c'
		///
		/// string s2 = "";
		/// string[] a2 = CArray.SplitLineBreak(s, StringSplitOptions.None); //a2.Length = 0
		/// string[] a2 = CArray.SplitLineBreak(s, StringSplitOptions.RemoveEmptyEntries); //a2.Length = 1, a2[0] = ""
		/// </example>
		/// <returns></returns>
		public static string[] SplitLineBreak(string Value, StringSplitOptions Options)
		{
			Value = CFindRep.ReplaceLineBreakToNewLine(Value);
			return Value.Split(new char[] { '\n' }, Options);

			//\r로 구분된 Silverlight TextBox 경우 Split 안되어 주석.
			//List<string> aValue = new List<string>();

			//string Pattern = "^.*$";
			//Regex r = new Regex(Pattern, RegexOptions.Multiline);
			//for (Match m = r.Match(Value); m.Success; m = m.NextMatch())
			//{
			//    if ((m.Value == "") && (Options == StringSplitOptions.RemoveEmptyEntries))
			//        continue;

			//    // 줄바꿈이 \r\n인 경우, \r은 남게 되므로 삭제함.
			//    aValue.Add(m.Value.Trim('\r'));
			//}

			//return aValue.ToArray();
		}

		/// <summary>
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// List<string> a = new List<string>();
		/// a.Add("a");
		/// a.Add("b");
		/// a.Add("c");
		/// 
		/// List<string> b = new List<string>();
		/// b.Add("b");
		/// b.Add("c");
		/// b.Add("d");
		/// 
		/// bool IsSame = CArray.IsAllItemIsSame(a, b); //false
		/// ]]>
		/// </example>
		/// <param name="aValue1"></param>
		/// <param name="aValue2"></param>
		/// <returns></returns>
		public static bool Equals(List<string> aValue1, List<string> aValue2)
		{
			if (aValue1.Count != aValue2.Count)
				return false;

			int Count = (from t1 in aValue1
						 join t2 in aValue2
						 on t1 equals t2
						 select new { t1 }).Count();

			if (Count != aValue1.Count)
				return false;

			return true;
		}

		//<=로 하면 Item이 하나도 없어도 true를 리턴하므로 ==으로 함.
		public static bool IsAllItemIsSame(IEnumerable<object> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<string> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<int> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<int?> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<double> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<double?> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<Color> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<Color?> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<bool> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		public static bool IsAllItemIsSame(IEnumerable<bool?> aValue)
		{
			return (aValue.Distinct().Count() == 1);
		}
		//특정 컴퓨터(Win XP였음)에선 aShadowEmbossEngrave.Cast<Enum>() 구문이 System.InvalidCastException 에러를 발생시키므로 주석.
		//public static bool IsAllItemIsSame(IEnumerable<Enum> aValue)
		//{
		//    return (aValue.Distinct().Count() == 1);
		//}

		/// <summary>
		/// string 형식 외의 배열도 Join하기 위함.
		/// </summary>
		/// <param name="Sep"></param>
		/// <param name="aValue"></param>
		/// <returns></returns>
		public static string Join<T>(string Separator, IEnumerable<T> aValue, Func<T, int, string> ConvertFunc)
		{
			int Index = -1;
			string s = "";
			foreach (T Value in aValue)
			{
				s += Separator + ConvertFunc(Value, ++Index);
			}
			if (s != "")
				s = s.Substring(Separator.Length);

			return s;
		}
		public static string Join<T>(string Separator, IEnumerable<T> aValue)
		{
			return Join<T>(Separator, aValue, (v, i) => v.ToString());
		}

		public static T[] Split<T>(string Value, char Delim, Func<string, T> ConvertFunc)
		{
			string[] aValue = Value.Split(Delim);
			T[] aValueNew = new T[aValue.Length];
			for (int Index = 0; Index < aValue.Length; Index++)
			{
				aValueNew[Index] = ConvertFunc(aValue[Index]);
			}

			return aValueNew;
		}
		public static T[] Split<T>(string Value, char Delim)
		{
			return Split<T>(Value, Delim, (v) => (T)Convert.ChangeType(v, typeof(T)));
		}

		/// <summary>
		/// 숫자로만 구성된 문자열 목록을 int[] 형식으로 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Delim"></param>
		/// <returns></returns>
		/// <example>
		/// int[] aNum = CArray.SplitInt("1,2,3", ',');
		/// </example>
		public static int[] SplitInt(string Value, char Delim)
		{
			return Split<int>(Value, Delim, (s) => string.IsNullOrEmpty(s) ? 0 : Convert.ToInt32(s));
		}

		/// <summary>
		/// 문자열 목록을 bool[] 형식으로 리턴함. 1이 아니면 모두 false로 처리함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Delim"></param>
		/// <returns></returns>
		/// <example>
		/// bool[] aBool = CArray.SplitBool("0,1,2", ','); // false, true, false
		/// </example>
		public static bool[] SplitBool(string Value, char Delim)
		{
			return Split<bool>(Value, Delim, (s) => s == "1");
		}

		//http://stackoverflow.com/questions/6539571/how-to-resize-multidimensional-2d-array-in-c
		public static T[,] ResizeArray<T>(T[,] aOriginal, int Rows, int Columns)
		{
			var aNew = new T[Rows, Columns];

			int MinRows = Math.Min(Rows, aOriginal.GetLength(0));
			int MinCols = Math.Min(Columns, aOriginal.GetLength(1));

			for (int i = 0; i < MinRows; i++)
				for (int j = 0; j < MinCols; j++)
					aNew[i, j] = aOriginal[i, j];

			return aNew;
		}

		/// <example>
		/// <![CDATA[
		/// string[] aCustom = new string[] 
		/// {
		///     Path.Combine(ParentFolder, "실전모의고사"),
		///     Path.Combine(ParentFolder, "최신기출문제"),
		///     Path.Combine(ParentFolder, "직전대비모의고사")
		/// };
		/// aFolder = CArray.OrderByCustom<string>(aFolder, aCustom).ToArray();
		/// ]]>
		/// </example>
		public static IEnumerable<T> OrderByCustom<T>(IEnumerable<T> aOriginal, T[] aCustom)
		{
			return 
				aOriginal
				.OrderBy(delegate(T t)
				{
					int Index = aCustom.ToList().FindIndex(t2 => t2.Equals(t));
					if (Index != -1)
						return Index - aCustom.Length;
					else
						return 0;
				})
				.ThenBy(t => t);
		}

		/// <summary>
		/// 배열의 중간에 다른 배열을 삽입함.
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// int[] aOri = new int[] { 1, 2, 3 };

		/// List<int> aNew = new List<int>();
		/// aNew.Add(11);
		/// aNew.Add(12);

		/// int IndexToInsert = 0;

		/// int[] aInserted = null;
		/// if ((IndexToInsert + 1) > aOri.Length)
		///     aInserted = aOri.Concat(aNew).ToArray();
		/// else
		///     aInserted = CArray.InsertAt<int>(aOri, IndexToInsert, aNew).ToArray();
		/// ]]>
		/// </example>
		public static IEnumerable<T> InsertRange<T>(IEnumerable<T> aOriginal, int Index, IEnumerable<T> aNew)
		{
			int i = -1;
			foreach (T Original in aOriginal)
			{
				i++;

				if (i == Index)
				{
					foreach (T New in aNew)
						yield return New;
				}

				yield return Original;
			}

			if (Index == (i + 1))
			{
				foreach (T New in aNew)
					yield return New;
			}
			else if (Index > (i + 1))
			{
				throw new ArgumentOutOfRangeException();
			}
		}
		public static IEnumerable<T> Insert<T>(IEnumerable<T> aOriginal, int Index, T New)
		{
			return InsertRange<T>(aOriginal, Index, new T[] { New });
		}

		public static IEnumerable<T> FillDefaultValue<T>(int Count)
		{
			for (int i = 0; i < Count; i++)
			{
				yield return default(T);
			}
		}

		public static IEnumerable<T> Repeat<T>(T Value, int Num)
		{
			for (int i = 0, i2 = Num; i < i2; i++)
			{
				yield return Value;
			}
		}
	}
}
