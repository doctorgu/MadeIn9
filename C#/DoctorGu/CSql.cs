using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;

namespace DoctorGu
{
	/// <summary>
	/// DB 서버 종류
	/// </summary>
	public enum DbServerType
	{
		SQLServer, Oracle, MSAccess, MySQL, Odbc, SQLite
	}

	public enum InsertUpdateDelete
	{
		Insert,
		Update,
		Delete,
	}

	public enum SqlColumnTypeSimple
	{
		String,
		DateTime,
		Numeric,
		Boolean
	}

	public class CSqlNameValueKeepValue
	{
		public readonly string Name;
		public readonly object Value;
		public readonly bool KeepValue;

		public CSqlNameValueKeepValue(string Name, object Value, bool KeepValue)
		{
			this.Name = Name;
			this.Value = Value;
			this.KeepValue = KeepValue;
		}
		public CSqlNameValueKeepValue(string Name, object Value)
		{
			this.Name = Name;
			this.Value = Value;
			this.KeepValue = false;
		}
	}

	/// <summary>
	///	Summary description for SQL.
	/// </summary>
	public class CSql
	{
		public CSql()
		{

		}

		public enum CaseTypes
		{
			PascalCase, UpperCaseUnderbar, LowerCaseUnderbar
		}

		/// <summary>
		/// 배열의 각 요소을 SQL문의 in 절에 들어갈 수 있도록 콤마로 구분해서 리턴함.
		/// </summary>
		/// <param name="aValue"> </param>
		/// <param name="IsNumberType">숫자형식의 필드인 지 여부 </param>
		/// <example>
		/// string SQL = "", SQLIn = "";
		/// 
		/// SQLIn = GetInList(new string[3]{"1", "2", "3"}, true);
		/// SQL = "select * from CodeList where Code in (" + SQLIn + ")";
		/// Console.WriteLine(SQL); //"select * from CodeList where Code in (1, 2, 3)"
		/// 
		/// SQLIn = GetInList(new string[3]{"학생", "교사", "프로그래머"}, false);
		/// SQL = "select * from JobList where JobName in (" + SQLIn + ")";
		/// Console.WriteLine(SQL); //"select * from JobList where JobName in ('학생', '교사', '프로그래머')"
		/// </example>
		public static string GetInList(string[] aValue, bool IsNumberType)
		{
			string ValueList = "";

			//숫자 형식이면 따옴표로 묶지 않아도 되므로 바로 Join을 사용함.
			if (IsNumberType)
			{
				ValueList = String.Join(",", aValue);
			}
			//문자열이나 날짜 형식이면 따옴표로 묶어야 하므로 모든 요소에 대해 루핑을
			//하면서 따옴표로 묶음.
			else
			{
				for (int i = 0; i < aValue.Length; i++)
				{
					if (aValue[i] != "")
					{
						ValueList += ",'" + aValue[i] + "'";
					}
				}

			}

			//양쪽에 콤마가 있으면 콤마를 제거함.
			if (ValueList.Substring(0, 1) == ",")
			{
				ValueList = ValueList.Substring(1);
			}
			if (ValueList.Substring(ValueList.Length - 1, 1) == ",")
			{
				ValueList = ValueList.Substring(0, ValueList.Length - 1);
			}

			return ValueList;
		}

		///// <summary>
		///// \r\n, \r, \n, \t 등 정상적으로 입력되지 않는 문자열이 구분자로 쓰였다면
		///// 해당 문자열들을 CD in (1, 2, 3)과 in 절에 넣을 수 있도록 "1, 2, 3"과 같이 변환해서 리턴함.
		///// </summary>
		///// <param name="Value"> </param>
		///// <param name="IsNumberType">숫자형식의 필드인 지 여부 </param>
		///// <param name="IsMultipleIs">in 절에 들어갈 수 있도록 콤마로 구분된 값으로 변환되었는 지 여부 </param>
		///// <example>
		///// bool IsMultipleIs;
		///// string SQL = "", SQLIn = "";
		///// 
		///// SQL = "select * from CodeList";
		///// SQLIn = GetInList("1\t2\t3", true, out IsMultipleIs);
		///// if (IsMultipleIs)
		///// {
		///// 	SQL += " where Code in (" + SQLIn + ")";
		///// }
		///// else
		///// {
		///// 	SQL += " where Code = " + SQLIn;
		///// }
		///// Console.WriteLine(SQL); //"select * from CodeList where Code in (1, 2, 3)"
		///// 
		///// SQL = "select * from JobList";
		///// SQLIn = GetInList("학생\t교사\t프로그래머", false, out IsMultipleIs);
		///// if (IsMultipleIs)
		///// {
		///// 	SQL += " where JobName in (" + SQLIn + ")";
		///// }
		///// else
		///// {
		///// 	SQL += " where JobName = " + SQLIn;
		///// }
		///// Console.WriteLine(SQL); //"select * from JobList where JobName in ('학생', '교사', '프로그래머')"
		///// </example>
		//public static string GetInList(string Value, bool IsNumberType,
		//                                out bool IsMultipleIs)
		//{
		//    char[] Delim;
		//    string ValueList = "";

		//    IsMultipleIs = false;

		//    Delim = GetDelimChar(Value);
		//    if (Delim.Length == 0)
		//    {
		//        return Value;
		//    }

		//    ValueList = GetInList(Value.Split(Delim), IsNumberType);

		//    IsMultipleIs = true;
		//    return ValueList;
		//}

		public static string GetInList<T>(T[] aValue)
		{
			string ValueList = "";

			SqlColumnTypeSimple TypeSimple = GetColumnTypeSimple(typeof(T));
			string Delim = CLang.In(TypeSimple, SqlColumnTypeSimple.DateTime, SqlColumnTypeSimple.String) ? "'" : "";

			for (int i = 0; i < aValue.Length; i++)
			{
				ValueList += "," + Delim + aValue[i] + Delim;
			}
			if (!string.IsNullOrEmpty(ValueList))
			{
				ValueList = ValueList.Substring(1);
			}

			return ValueList;
		}

		/// <summary>
		/// 문자열들을 구분하는 특정 문자열이 있다면 그 문자열을 리턴함.
		/// </summary>
		/// <param name="ValueList">구분자를 가진 문자열 전체</param>
		/// <returns>구분자</returns>
		/// <example>GetDelimChar("d1,d2") --> ","</example>
		public static char[] GetDelimChar(string Value)
		{
			char[] Delim;

			//배열로 변환하기 위해 구분자가 무엇인 지 찾아냄.
			if (Value.IndexOf("\r\n") != -1)
			{
				Delim = new char[2] { '\r', '\n' };
			}
			else if (Value.IndexOf("\r") != -1)
			{
				Delim = new char[1] { '\r' };
			}
			else if (Value.IndexOf("\n") != -1)
			{
				Delim = new char[1] { '\n' };
			}
			else if (Value.IndexOf("\t") != -1)
			{
				Delim = new char[1] { '\t' };
			}
			else if (Value.IndexOf(",") != -1)
			{
				Delim = new char[1] { ',' };
			}
			else if (Value.IndexOf(";") != -1)
			{
				Delim = new char[1] { ';' };
			}
			else
			{
				Delim = new char[0];
			}

			return Delim;
		}

		public static string AddSqlCriteria(string Sql, string Criteria, string Operator)
		{
			if (Criteria == "") return Sql;

			Sql = Sql.Replace("\r", " ");
			Sql = Sql.Replace("\n", " ");
			Sql = Sql.Replace("\t", " ");

			int PosEnd = Sql.ToLower().IndexOf("group by ") - 1;
			if (PosEnd == -2) PosEnd = Sql.ToLower().IndexOf("order by ") - 1;
			if (PosEnd == -2) PosEnd = Sql.ToLower().IndexOf("pivot ") - 1;
			if (PosEnd == -2) PosEnd = Sql.ToLower().IndexOf("union ") - 1;
			if (PosEnd == -2) PosEnd = Sql.Length - 1;

			int PosWhere = Sql.ToLower().IndexOf("where ");
			if (PosWhere == -1)
			{
				return Sql.Substring(0, PosEnd + 1)
					+ " where "
					+ Criteria
					+ " "
					+ Sql.Substring(PosEnd + 1);
			}
			else
			{
				return Sql.Substring(0, PosWhere + "where ".Length)
					+ Criteria
					+ " "
					+ Operator
					+ " "
					+ Sql.Substring(PosWhere + "where ".Length);
			}
		}

		public static string GetSqlInsert(DbServerType DSType, string TableName,
			List<CSqlNameValueKeepValue> aNameValueKeepValue)
		{
			string Sql = GetSqlTemplateInsert(DSType, TableName, aNameValueKeepValue);
			Sql = GetSqlTemplateReplaced(Sql, DSType, aNameValueKeepValue);
			return Sql;
		}

		public static string GetSqlInsertSelect(DbServerType DSType, string TableName,
			List<CSqlNameValueKeepValue> aNameValueKeepValue, string TableNameFrom, string Where)
		{
			string Sql = GetSqlTemplateInsertSelect(DSType, TableName, aNameValueKeepValue, TableNameFrom, Where);
			Sql = GetSqlTemplateReplaced(Sql, DSType, aNameValueKeepValue);
			return Sql;
		}

		public static string GetSqlUpdate(DbServerType DSType, string TableName,
			List<CSqlNameValueKeepValue> aNameValueKeepValue, string Where)
		{
			string Sql = GetSqlTemplateUpdate(DSType, TableName, aNameValueKeepValue, Where);
			Sql = GetSqlTemplateReplaced(Sql, DSType, aNameValueKeepValue);
			return Sql;
		}

		private static string GetSqlTemplateInsert(DbServerType DSType, string TableName,
			List<CSqlNameValueKeepValue> aNameValueKeepValue)
		{
			string SqlTmpHeader = "";
			switch (DSType)
			{
				case DbServerType.MySQL:
					SqlTmpHeader = "insert into " + TableName + " set ";
					break;
				default:
					SqlTmpHeader = "insert into " + TableName + "(";
					break;
			}

			string SqlTmpBody = "", SqlTmpBody2 = "";
			foreach (CSqlNameValueKeepValue nvk in aNameValueKeepValue)
			{
				string FieldName = nvk.Name;
				switch (DSType)
				{
					case DbServerType.MySQL:
						SqlTmpBody += ",\n" + FieldName + " = {{" + FieldName + "}}";
						break;
					default:
						SqlTmpBody += ",\n" + FieldName;
						SqlTmpBody2 += ",\n{{" + FieldName + "}}";
						break;
				}
			}

			string SqlTmp = "";
			switch (DSType)
			{
				case DbServerType.MySQL:
					SqlTmp = SqlTmpHeader + " " + SqlTmpBody.Substring(2);
					break;
				default:
					SqlTmp = SqlTmpHeader + SqlTmpBody.Substring(2) + ")"
						+ "\nvalues(" + SqlTmpBody2.Substring(2) + ")";
					break;
			}

			return SqlTmp;
		}

		private static string GetSqlTemplateInsertSelect(DbServerType DSType, string TableName,
			List<CSqlNameValueKeepValue> aNameValueKeepValue, string TableNameFrom, string Where)
		{
			string SqlTmpHeader = "insert into " + TableName + "(";

			string SqlTmpBody = "", SqlTmpBody2 = "";
			foreach (CSqlNameValueKeepValue nvk in aNameValueKeepValue)
			{
				string FieldName = nvk.Name;

				SqlTmpBody += ",\n" + FieldName;
				SqlTmpBody2 += ",\n{{" + FieldName + "}}";
			}

			string SqlTmp = SqlTmpHeader + SqlTmpBody.Substring(2) + ")"
						+ "\nselect\n" + SqlTmpBody2.Substring(2)
						+ "\nfrom " + TableNameFrom
						+ (!string.IsNullOrEmpty(Where) ? "\nwhere " + Where : "");

			return SqlTmp;
		}

		private static string GetSqlTemplateUpdate(DbServerType DSType, string TableName,
			List<CSqlNameValueKeepValue> aNameValueKeepValue, string Where)
		{
			string SqlTmpHeader = "update " + TableName + " set ";

			string SqlTmpBody = "";
			foreach (CSqlNameValueKeepValue nvk in aNameValueKeepValue)
			{
				string FieldName = nvk.Name;
				SqlTmpBody += ",\n" + FieldName + " = {{" + FieldName + "}}";
			}

			string SqlTmpWhere = "\nwhere " + Where;

			string SqlTmp = SqlTmpHeader + "\n" + SqlTmpBody.Substring(2) + "\n" + SqlTmpWhere;

			return SqlTmp;
		}

		private static string GetSqlTemplateReplaced(string Sql, DbServerType DSType, List<CSqlNameValueKeepValue> aNameValueKeepValue)
		{
			foreach (CSqlNameValueKeepValue nvk in aNameValueKeepValue)
			{
				string FieldName = nvk.Name;
				bool KeepValue = nvk.KeepValue;

				string Value = "";
				if (nvk.Value == null)
				{
					Value = "null";
				}
				else if (KeepValue)
				{
					//함수나 연산자가 사용된 경우엔 변형하지 않음.
					Value = nvk.Value.ToString();
				}
				else
				{
					SqlColumnTypeSimple ColumnType = GetColumnTypeSimple(nvk.Value);

					switch (ColumnType)
					{
						case SqlColumnTypeSimple.DateTime:
						case SqlColumnTypeSimple.String:
							Value = nvk.Value.ToString();

							Value = "'" + Value.Replace("'", "''");

							if (DSType == DbServerType.MySQL)
								Value = Value.Replace(@"\", @"\\");

							Value += "'";

							break;
						case SqlColumnTypeSimple.Boolean:
							Value = CFindRep.IfTrueThen1FalseThen0((bool)nvk.Value);
							break;
						case SqlColumnTypeSimple.Numeric:
							Value = nvk.Value.ToString();
							break;
					}
				}

				Sql = Sql.Replace("{{" + FieldName + "}}", Value);
			}

			return Sql;
		}

		public static SqlColumnTypeSimple GetColumnTypeSimple(object Value)
		{
			Type t = Value.GetType();
			return GetColumnTypeSimple(t);
		}
		public static SqlColumnTypeSimple GetColumnTypeSimple(Type t)
		{
			if (t == typeof(DateTime))
				return SqlColumnTypeSimple.DateTime;
			else if ((t == typeof(String)) || (t == typeof(Char)))
				return SqlColumnTypeSimple.String;
			else if (t == typeof(Boolean))
				return SqlColumnTypeSimple.Boolean;
			else if (CType.IsNumericType(t))
				return SqlColumnTypeSimple.Numeric;
			else
				throw new Exception(string.Format("Wrong Type:{0}", t));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Stmt"></param>
		/// <param name="aTableFieldName"></param>
		/// <param name="CaseFrom"></param>
		/// <param name="CaseTo"></param>
		/// <param name="IsReplaceOnlyInDoubleQuote"></param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// string[] aNamePascal = new string[] { "UserName", "UserId" };
		/// string s = "SELECT a.UserName, a.UserId, a.UserJumin FROM UserInfo AS a WHERE a.UserId LIKE 'UserId%'";
		///
		/// string s2 = CSql.GetTableFieldNameToCase(s, aNamePascal, CSql.CaseTypes.PascalCase, CSql.CaseTypes.UpperCaseUnderbar);
		/// Console.WriteLine(s2);
		/// //--결과
		/// //SELECT a.USER_NAME, a.USER_ID, a.UserJumin FROM UserInfo AS a WHERE a.USER_ID LIKE 'UserId%'
		///
		/// string[] aNameUpper = new string[] { "USER_NAME", "USER_ID" };
		/// string s3 = CSql.GetTableFieldNameToCase(s2, aNameUpper, CSql.CaseTypes.UpperCaseUnderbar, CSql.CaseTypes.PascalCase);
		/// Console.WriteLine(s3);
		/// //--결과
		/// //SELECT a.UserName, a.UserId, a.UserJumin FROM UserInfo AS a WHERE a.UserId LIKE 'UserId%'
		/// </code>
		/// </example>
		public static string GetTableFieldNameToCase(string Stmt, string[] aTableFieldName,
			CaseTypes CaseFrom, CaseTypes CaseTo, bool IsReplaceOnlyInDoubleQuote)
		{
			string[] aKeywords = GetSqlServerKeyword();

			SortedList slIndexValue = new SortedList();

			// \w+: 연속된(+) 글자(w), 즉 단어
			// ': 작은 따옴표
			// ": 큰 따옴표

			bool IsDoubleQuotStarted = false;
			int QuoteEnd = -1;
			Regex r = new Regex(@"\w+|'|""", RegexOptions.Compiled);
			for (Match m = r.Match(Stmt); m.Success; m = m.NextMatch())
			{
				if (m.Index <= QuoteEnd)
					continue;

				switch (m.Value)
				{
					case "\"":
						if (IsDoubleQuotStarted)
							IsDoubleQuotStarted = false;
						else
							IsDoubleQuotStarted = true;

						break;
					case "'":
						QuoteEnd = CDelim.GetQuoteEnd(Stmt, m.Index, '\'');
						break;
					default:
						bool IsReplace = true;
						//따옴표 안의 문자열만 변경하는 옵션일 때 따옴표 안에 있지 않거나,
						//현재 Value가 Table, Field 이름이 아닌 경우.
						if ((IsReplaceOnlyInDoubleQuote && !IsDoubleQuotStarted)
							|| (CArray.IndexOf(aTableFieldName, m.Value, true) == -1))
						{
							IsReplace = false;
						}

						if (IsReplace)
						{
							string Value = GetCaseConverted(m.Value, CaseFrom, CaseTo);
							slIndexValue.Add(m.Index, new string[] { m.Value, Value });
						}
						else
						{
							slIndexValue.Add(m.Index, new string[] { m.Value, m.Value });
						}
						break;
				}
			}

			string s = GetIndexValueMerged(Stmt, slIndexValue);
			return s;
		}
		private static string GetCaseConverted(string Value, CaseTypes CaseFrom, CaseTypes CaseTo)
		{
			StringBuilder sb = new StringBuilder();

			switch (CaseFrom)
			{
				case CaseTypes.PascalCase:
					for (int i = 0, i2 = Value.Length; i < i2; i++)
					{
						char c = Value[i];
						bool IsUpper = char.IsUpper(c);

						string s = "";
						switch (CaseTo)
						{
							case CaseTypes.LowerCaseUnderbar:
							case CaseTypes.UpperCaseUnderbar:
								if ((i != 0) && IsUpper)
									s = "_";

								if (CaseTo == CaseTypes.LowerCaseUnderbar)
								{
									s += c.ToString().ToLower();
								}
								else if (CaseTo == CaseTypes.UpperCaseUnderbar)
								{
									s += c.ToString().ToUpper();
								}

								break;
						}

						sb.Append(s);
					}

					break;

				case CaseTypes.LowerCaseUnderbar:
				case CaseTypes.UpperCaseUnderbar:
					bool IsUnder = false, IsUnderOld = false;
					for (int i = 0, i2 = Value.Length; i < i2; i++)
					{
						char c = Value[i];
						IsUnderOld = IsUnder;
						IsUnder = (c == '_');

						string s = "";
						switch (CaseTo)
						{
							case CaseTypes.PascalCase:
								//_는 뺌.
								if (IsUnder) continue;

								if ((i == 0) || (IsUnderOld))
									s = c.ToString().ToUpper();
								else
									s = c.ToString().ToLower();

								break;
						}

						sb.Append(s);
					}

					break;
			}

			return sb.ToString();
		}

		/// <summary>
		/// SQL 서버용 키워드는 소문자로 변경해서 리턴함.
		/// </summary>
		/// <param name="Stmt">SQL문 자체, 또는 SQL문을 포함하는 문자열</param>
		/// <returns>변환된 문자열</returns>
		/// <example>
		/// <code>
		/// string Sql = "SELECT * FROM mytable WHERE id = 'BETWEEN a TO b' AND NAME LIKE '박%'";
		/// string s = CSql.GetSqlServerKeywordToLcase(Sql);
		/// Console.WriteLine(s);
		/// //--결과
		/// //select * from mytable where id = 'BETWEEN a TO b' AND NAME LIKE '박%'";
		/// </code>
		/// </example>
		public static string GetSqlServerKeywordToLcase(string Stmt)
		{
			return GetSqlServerKeywordTo(Stmt, false);
		}
		/// <summary>
		/// SQL 서버용 키워드는 대문자로 변경해서 리턴함.
		/// </summary>
		/// <param name="Stmt">SQL문 자체, 또는 SQL문을 포함하는 문자열</param>
		/// <returns>변환된 문자열</returns>
		/// <example>
		/// <code>
		/// string Sql = "select * from mytable where id = 'between a to b' and name like '박%'";
		/// string s = CSql.GetSqlServerKeywordToUcase(Sql);
		/// Console.WriteLine(s);
		/// //--결과
		/// //SELECT * FROM mytable WHERE id = 'between a to b' AND name LIKE '박%'
		/// </code>
		/// </example>
		public static string GetSqlServerKeywordToUcase(string Stmt)
		{
			return GetSqlServerKeywordTo(Stmt, true);
		}
		/// <summary>
		/// SQL 서버용 키워드는 소문자 또는 대문자로 변경해서 리턴함.
		/// </summary>
		/// <param name="Stmt">SQL문 자체, 또는 SQL문을 포함하는 문자열</param>
		/// <returns>변환된 문자열</returns>
		/// <example>
		/// <code>
		/// string Sql = "select * from mytable where id = 'between a to b' and name like '박%'";
		/// string s = CSql.GetSqlServerKeywordTo(Sql, true);
		/// Console.WriteLine(s);
		/// //--결과
		/// //SELECT * FROM mytable WHERE id = 'between a to b' AND name LIKE '박%'
		/// </code>
		/// </example>
		private static string GetSqlServerKeywordTo(string Stmt, bool IsUcase)
		{
			string[] aKeywords = GetSqlServerKeyword();

			SortedList slIndexValue = new SortedList();

			// \w+: 연속된(+) 글자(w), 즉 단어
			// ': 작은 따옴표

			int QuoteEnd = -1;
			Regex r = new Regex(@"\w+|'", RegexOptions.Compiled);
			for (Match m = r.Match(Stmt); m.Success; m = m.NextMatch())
			{
				if (m.Index <= QuoteEnd) continue;

				switch (m.Value)
				{
					case "'":
						QuoteEnd = CDelim.GetQuoteEnd(Stmt, m.Index, '\'');
						break;
					default:
						if (CArray.IndexOf(aKeywords, m.Value, true) >= 0)
						{
							slIndexValue.Add(m.Index, new string[] { m.Value, (IsUcase ? m.Value.ToUpper() : m.Value.ToLower()) });
						}
						else
						{
							slIndexValue.Add(m.Index, new string[] { m.Value, m.Value });
						}
						break;
				}
			}

			string s = GetIndexValueMerged(Stmt, slIndexValue);
			return s;
		}
		private static string[] GetSqlServerKeyword()
		{
			string[] aKeyWord = new string[]
			{
			"ELSE",
			"IF",
			"BEGIN",
			"OUTPUT",
			"BREAK",
			"WHILE",
			"ALTER",
			"@@ERROR",
			"@@IDENTITY",
			"@@ROWCOUNT",
			"@@TRANCOUNT",
			"ABS",
			"ACOS",
			"ALL",
			"AND",
			"APP_NAME",
			"AS",
			"ASC",
			"ASCII",
			"ASIN",
			"ATAN",
			"ATN2",
			"BETWEEN",
			"BIGINT",
			"BINARY",
			"BIT",
			"BY",
			"CASE",
			"CAST",
			"CEILING",
			"CHAR",
			"CHARINDEX",
			"COALESCE",
			"COLLATIONPROPERTY",
			"CONSTRAINT",
			"COS",
			"COT",
			"COUNT",
			"CREATE",
			"CURRENT_TIMESTAMP",
			"CURRENT_USER",
			"CURSOR",
			"DATALENGTH",
			"DATEADD",
			"DATEDIFF",
			"DATENAME",
			"DATEPART",
			"DATETIME",
			"DAY",
			"DECIMAL",
			"DECLARE",
			"DEGREES",
			"DELETE",
			"DESC",
			"DIFFERENCE",
			"DISTINCT",
			"DROP",
			"EXEC",
			"EXECUTE",
			"EXISTS",
			"EXP",
			"FLOAT",
			"FLOOR",
			"FN_HELPCOLLATIONS",
			"FN_SERVERSHAREDDRIVES",
			"FN_VIRTUALFILESTATS",
			"FOR",
			"FORMATMESSAGE",
			"FROM",
			"FUNCTION",
			"GETANSINULL",
			"GETDATE",
			"GETUTCDATE",
			"GROUP",
			"HAVING",
			"HOST_ID",
			"HOST_NAME",
			"IDENT_CURRENT",
			"IDENT_INCR",
			"IDENT_SEED",
			"IDENTITY",
			"IMAGE",
			"INDEX",
			"INNER",
			"INSERT",
			"INT",
			"INTO",
			"IS",
			"ISDATE",
			"ISNULL",
			"ISNUMERIC",
			"JOIN",
			"KEY",
			"LEFT",
			"LEN",
			"LIKE",
			"LOG",
			"LOG10",
			"LOWER",
			"LTRIM",
			"MAX",
			"MIN",
			"MONEY",
			"MONTH",
			"NCHAR",
			"NEWID",
			"NOT",
			"NTEXT",
			"NULL",
			"NULLIF",
			"NUMERIC",
			"NVARCHAR",
			"ON",
			"OR",
			"ORDER",
			"OVER",
			"PARSENAME",
			"PATINDEX",
			"PERMISSIONS",
			"PI",
			"POWER",
			"PRIMARY",
			"PROC",
			"PROCEDURE",
			"QUOTENAME",
			"RADIANS",
			"RAISERROR",
			"RAND",
			"REAL",
			"REPLACE",
			"REPLICATE",
			"RETURN",
			"RETURNS",
			"REVERSE",
			"RIGHT",
			"ROUND",
			"ROWCOUNT_BIG",
			"ROW_NUMBER",
			"RTRIM",
			"SCOPE_IDENTITY",
			"SELECT",
			"SERVERPROPERTY",
			"SESSION_USER",
			"SESSIONPROPERTY",
			"SET",
			"SIGN",
			"SIN",
			"SMALLDATETIME",
			"SMALLINT",
			"SMALLMONEY",
			"SOUNDEX",
			"SPACE",
			"SQL_VARIANT",
			"SQRT",
			"SQUARE",
			"STATS_DATE",
			"STR",
			"STUFF",
			"SUBSTRING",
			"SUM",
			"SYSTEM_USER",
			"TABLE",
			"TAN",
			"TEXT",
			"TEXTPTR",
			"TEXTVALID",
			"THEN",
			"TIMESTAMP",
			"TINYINT",
			"UNICODE",
			"UNION",
			"UNIQUEIDENTIFIER",
			"UPDATE",
			"UPPER",
			"USER_NAME",
			"VALUES",
			"VARBINARY",
			"VARCHAR",
			"VIEW",
			"WITH",
			"WHERE",
			"XML",
			"YEAR",
			"WHEN",
			"END",
			"IN"
			};

			return aKeyWord;
		}

		private static string GetIndexValueMerged(string Value, SortedList slIndexValue)
		{
			StringBuilder sb = new StringBuilder();

			int PosStart = 0, PosEnd = 0;
			foreach (DictionaryEntry d in slIndexValue)
			{
				PosEnd = Convert.ToInt32(d.Key) - 1;

				string[] aValueOriNew = (string[])d.Value;
				string ValueOri = aValueOriNew[0];
				string ValueNew = aValueOriNew[1];

				string s = Value.Substring(PosStart, PosEnd - PosStart + 1);
				string s2 = ValueNew;

				sb.Append(s);
				sb.Append(s2);

				PosStart = PosEnd + ValueOri.Length + 1;
			}
			if (PosStart < Value.Length)
			{
				sb.Append(Value.Substring(PosStart));
			}

			return sb.ToString();
		}

		public static string GetOracleApplicationErrorMessage(int OraErrorNumber, string ExceptionMessage)
		{
			/*
			ORA-20000: 1지문2문항의 1번째 문제유형:L, 문항영역:2, 문항번호:17에 대한 권한이 없거나 허용 개수를 초과했습니다.
			ORA-06512: ""DB_CONTEST.PKG_EXAM_INFO"", 줄 330에서
			ORA-06512: 줄 1에서
			*/

			string Pattern = "ORA" + OraErrorNumber.ToString() + ":\\s(?<Message>.+?)ORA-";
			Regex r = new Regex(Pattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
			Match m = r.Match(ExceptionMessage);

			//If not found, return original message.
			if (!m.Success)
				return ExceptionMessage;

			return CFindRep.TrimWhiteSpace(m.Groups["Message"].Value);
		}
		public static string GetOracleApplicationErrorMessage(int OraErrorNumber, Exception ex)
		{
			return GetOracleApplicationErrorMessage(OraErrorNumber, ex.Message);
		}

		/// <summary>
		/// SQL Injection을 막기 위해 '{0}'와 같이 따옴표 안의 변수는 따옴표를 2개로 변경함.
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// int f1 = 99;
		/// string f2 = "꽃이' where seq = 1; update tt set f2 = '하하하";
		/// string Sql = string.Format("update t set f1 = {0}, f2 = '{1}' where seq = 1", f1, f2); //Injection 성공
		/// -> update t set f1 = 99, f2 = '꽃이' where seq = 1; update tt set f2 = '하하하' where seq = 1
		/// string SqlSafe = CSql.FormatSql("update t set f1 = {0}, f2 = '{1}' where seq = 1", f1, f2); //Injection 실패
		/// -> update t set f1 = 99, f2 = '꽃이'' where seq = 1; update tt set f2 = ''하하하' where seq = 1
		/// ]]>
		/// </example>
		public static string FormatSql(string Format, params object[] Args)
		{
			for (int i = 0; i < Args.Length; i++)
			{
				if (Args[i] == null)
					Args[i] = "";

				string PatternQuot = @"'\{" + i.ToString() + @"\}'";
				string PatternNoQuot = @"{" + i.ToString() + @"\}";

				//f2 = '{1}'와 같은 경우, 변수 값에 따옴표(')를 넣어 Injection 처리가 가능하므로 따옴표(')가 있으면 이중 따옴표('')로 변경
				Regex rQuot = new Regex(PatternQuot, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
				Format = rQuot.Replace(Format, "'" + Args[i].ToString().Replace("'", "''") + "'");

				//f1 = {0}와 같은 경우, 따옴표를 넣어도 에러가 발생하므로 변수 값을 그냥 대체함
				Regex rNoQuot = new Regex(PatternNoQuot, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
				Format = rNoQuot.Replace(Format, Args[i].ToString());
			}

			return Format;
		}
	}
}
