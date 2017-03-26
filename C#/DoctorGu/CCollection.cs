using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace DoctorGu
{
	/// <summary>
	/// Collection 개체와 관련된 기능 구현.
	/// </summary>
	public class CCollection
	{
		/// <summary>
		/// NameValueCollection 개체에 특정 Name이 존재하는 지 여부를 리턴함.
		/// </summary>
		/// <param name="nv">NameValueCollection 개체</param>
		/// <param name="Key">찾을 Name</param>
		/// <returns><paramref name="Key"/>의 존재 여부</returns>
		public static bool HasKey(NameValueCollection nv, string Key)
		{
			return (nv[Key] != null);
			/*
			string Test = nv[Key];
			return (Test != null);
			*/
			/*
			bool IsKeyExist = true;
			string Test = "";
			try 
			{
				Test = nv[Key];
			}
			catch (Exception)
			{
				IsKeyExist = false; 
			}

			return IsKeyExist;
			*/
		}

		/// <summary>
		/// Name의 목록을 구분자로 구분해서 리턴함.
		/// </summary>
		/// <param name="nv">NameValueCollection 개체</param>
		/// <param name="Sep">구분자</param>
		/// <returns>구분자로 구분된 모든 Name의 목록</returns>
		/// <example>
		/// <code>
		/// NameValueCollection nv = new NameValueCollection();
		/// nv.Add("a", "1234");
		/// nv.Add("b", "5678");
		/// string KeyList = CCollection.Keys(nv, "+");
		/// Console.WriteLine(KeyList); //a+b
		/// </code>
		/// </example>
		public static string Keys(NameValueCollection nv, string Sep)
		{
			if (nv == null) return "";
			if (nv.Count == 0) return "";
			
			string KeyList = "";
			foreach (string Key in nv)
			{
				KeyList += Sep + Key;
			}
			KeyList = KeyList.Substring(Sep.Length);

			return KeyList;
		}

		public static string Join(Dictionary<string, string> dic, char DelimRow, char DelimCol, char ValueWrapper, char EscapeCharOfValueWrapper)
		{
			if ((dic == null) || (dic.Count == 0))
				return "";

			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<string, string> kv in dic)
			{
				string Key = kv.Key;

				string Value = kv.Value;
				if (Value.IndexOf(ValueWrapper) != -1)
					Value = Value.Replace(ValueWrapper.ToString(), string.Concat(EscapeCharOfValueWrapper, ValueWrapper));

				Value = string.Concat(ValueWrapper, Value, ValueWrapper);

				sb.Append(string.Concat(DelimRow, Key, DelimCol, Value));
			}

			return sb.ToString().Substring(1);
		}

		/// <summary>
		/// NameValueCollection의 모든 Name과 Value를 HTML 테이블 형식으로 변환해서 리턴함.
		/// </summary>
		/// <param name="nv">NameValueCollection 개체</param>
		/// <returns>HTML 테이블 형식으로 변환된 문자열</returns>
		/// <example>
		/// <code>
		/// NameValueCollection nv = new NameValueCollection();
		/// nv.Add("a", "1234");
		/// nv.Add("b", "5678");
		/// string Html = CCollection.ToHtml(nv);
		/// Console.WriteLine(Html);
		/// <![CDATA[//Html 내용
		/// <table border=1 cellpadding=0 cellspacing=0>
		/// <tr><td>a</td><td>1234</td></tr>
		/// <tr><td>b</td><td>5678</td></tr>
		/// </table>
		/// ]]>
		/// </code>
		/// </example>
		public static string ToHtml(NameValueCollection nv)
		{
			string s = "";

			s += "<table border=1 cellpadding=0 cellspacing=0>\r\n";
			foreach (string Key in nv)
			{
				s += "<tr><td>" + Key + "</td><td>" + nv[Key] + "</td></tr>\r\n";
			}
			s += "</table>";

			return s;
		}

		/// <summary>
		/// NameValueCollection의 모든 Name과 Value를 탭과 줄바꿈으로 구분된 문자열 형식으로 변환해서 리턴함.
		/// </summary>
		/// <param name="nv">NameValueCollection 개체</param>
		/// <returns>탭과 줄바꿈으로 구분된 문자열</returns>
		/// <example>
		/// NameValueCollection nv = new NameValueCollection();
		/// nv.Add("a", "1234");
		/// nv.Add("b", "5678");
		/// string Text = CCollection.ToString(nv);
		/// Console.WriteLine(Text);
		/// <![CDATA[
		/// --Text 내용
		/// a	   1234
		/// b	   5678
		/// ]]>
		/// </example>
		public static string ToString(NameValueCollection nv)
		{
			string s = "";

			foreach (string Key in nv)
			{
				s += "\r\n" + Key + "\t" + nv[Key];
			}

			if (s != "")
			{
				s = s.Substring(2);
			}

			return s;
		}

		public static string ItemToString(NameValueCollection nv, string Delim)
		{
			string s = "";

			foreach (string Key in nv)
			{
				s += Delim + nv[Key];
			}

			if (s != "")
			{
				s = s.Substring(Delim.Length);
			}

			return s;
		}

		/// <example>
		/// <![CDATA[
		/// Dictionary<string, string> dic1 = new Dictionary<string, string>();
		/// Dictionary<int, string> dic2 = new Dictionary<int, string>();
		/// dic2.Add(1, "a");
		/// bool b1 = IsNullOr0Count(dic1); //true
		/// bool b2 = IsNullOr0Count(dic2); //false
		/// ]]>
		/// </example>
		public static bool IsNullOr0Count(ICollection Value)
		{
			return ((Value == null) || (Value.Count == 0));
		}
		public static bool IsNullOr0Count<T>(T[] Value)
		{
			return ((Value == null) || (Value.Length == 0));
		}
	}
}
