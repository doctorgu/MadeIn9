using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class CSqlParser
	{
		public static string ConvertSql(string Sql, Dictionary<string, string[]> dicPrimaryKey,
			DbServerType DbServerFrom, DbServerType DbServerTo)
		{
			Sql = ReplaceFunction(DbServerFrom, DbServerTo, Sql);

			List<string> aSqlNew = new List<string>();

			Dictionary<int, string> dicIdxSql = SplitSqlBySemicolon(Sql);
			foreach (KeyValuePair<int, string> kv in dicIdxSql)
			{
				string SqlCur = kv.Value;

				CParagraph p = new CParagraph(CParagraph.DelimWord.NoUnderbar);
				List<string> aWord = p.GetWords(SqlCur, false);
				string Sql2 = string.Join(" ", aWord.ToArray());

				if (DbServerFrom == DbServerType.SQLite)
				{
					bool IsInsertOrReplace = (Sql2.IndexOf("insert or replace") != -1);
					if (IsInsertOrReplace)
					{
						CSqlInfoInsert InfoInsert = GetSqlInfoInsert(DbServerFrom, SqlCur);
						if (dicPrimaryKey.ContainsKey(InfoInsert.Table))
							InfoInsert.PrimaryKey = dicPrimaryKey[InfoInsert.Table];

						SqlCur = MakeInsertOrUpdate(DbServerTo, InfoInsert);
						aSqlNew.Add(SqlCur);
					}
					else
					{
						aSqlNew.Add(SqlCur);
					}
				}
			}

			return string.Join(";", aSqlNew.ToArray());
		}

		private static string ReplaceFunction(DbServerType DbServerFrom, DbServerType DbServerTo, string Sql)
		{
			Dictionary<string, string> dicFromTo = new Dictionary<string, string>();

			if ((DbServerFrom == DbServerType.SQLite) && (DbServerTo == DbServerType.SQLServer))
			{
				dicFromTo.Add("datetime('now')", "getdate()");
			}

			bool IsStartQuot = false;

			StringBuilder sbNew = new StringBuilder();
			for (int i = 0; i < Sql.Length; i++)
			{
				char c = Sql[i];
				if (c == '\'')
				{
					IsStartQuot = !IsStartQuot;
				}

				bool IsFound = false;
				foreach (KeyValuePair<string, string> kv in dicFromTo)
				{
					string From = kv.Key;
					string To = kv.Value;

					if ((i + From.Length) > Sql.Length)
						continue;

					if (Sql.Substring(i, From.Length) != From)
						continue;

					IsFound = true;
					sbNew.Append(To);
					i += From.Length - 1;
				}

				if (IsFound)
					continue;

				sbNew.Append(c);
			}

			return sbNew.ToString();
		}

		private static Dictionary<int, string> SplitSqlBySemicolon(string Sql)
		{
			List<int> aIndex = new List<int>();
			Dictionary<int, string> dicIdxSql = new Dictionary<int, string>();

			bool IsStartQuot = false;

			for (int i = 0; i < Sql.Length; i++)
			{
				char c = Sql[i];
				if (c == '\'')
				{
					IsStartQuot = !IsStartQuot;
				}
				else if (c == ';')
				{
					if (!IsStartQuot)
						aIndex.Add(i);
				}
			}

			int IndexStart = 0;
			int IndexEnd = -2;
			for (int i = 0; i < aIndex.Count; i++)
			{
				IndexStart = IndexEnd + 2;
				IndexEnd = aIndex[i] - 1;
				string SqlCur = Sql.Substring(IndexStart, (IndexEnd - IndexStart + 1));

				if (!string.IsNullOrEmpty(CFindRep.TrimWhiteSpace(SqlCur)))
					dicIdxSql.Add(IndexStart, SqlCur);
			}

			{
				IndexStart = IndexEnd + 2;
				IndexEnd = Sql.Length - 1;
				string SqlCur = Sql.Substring(IndexStart, (IndexEnd - IndexStart + 1));

				if (!string.IsNullOrEmpty(CFindRep.TrimWhiteSpace(SqlCur)))
					dicIdxSql.Add(IndexStart, SqlCur);
			}

			return dicIdxSql;
		}

		private static CSqlInfoInsert GetSqlInfoInsert(DbServerType DbServer, string Sql)
		{
			CSqlInfoInsert Info = new CSqlInfoInsert();

			CParagraph p = new CParagraph(CParagraph.DelimWord.NoUnderbar);
			p.WordSolo = new char[] { '(', ')', ',', '\'', '#' };
			Dictionary<int, string> dicWordDelim = p.GetIndexAndWords(Sql, true);
			List<int> aIndex = dicWordDelim.Keys.ToList();
			List<string> aWord = dicWordDelim.Values.ToList();

			SFromTo ftIndex = new SFromTo();


			ftIndex = IndexOf(aWord, 0, true, "insert", null, "into");
			if (ftIndex.From == -1)
			{
				if (DbServer == DbServerType.SQLite)
					ftIndex = IndexOf(aWord, 0, true, "insert", null, "or", null, "replace", null, "into");
			}

			if (ftIndex.From == -1)
				return null;

			int IndexTable = ftIndex.To + 2;
			if ((IndexTable + 1) > aWord.Count)
				return null;

			Info.Table = aWord[IndexTable];


			ftIndex = IndexOf(aWord, IndexTable + 1, true, "(");
			if (ftIndex.From == -1)
				return null;

			List<string> aField = new List<string>();
			int IndexFieldClose = -1;
			bool IsFirstField = true;
			int IndexFieldStart = (ftIndex.To + 2); // (Name, Age)에서 "," 위치부터 검사
			for (int i = IndexFieldStart; i < aWord.Count; i++)
			{
				string WordCur = aWord[i];

				if ((WordCur == ",")
					|| (!IsFirstField && (WordCur == ")")))
				{
					aField.Add(aWord[i - 1]);

					if (WordCur == ")")
					{
						IndexFieldClose = i;
						break;
					}
				}

				IsFirstField = false;
			}

			if (IndexFieldClose == -1)
				return null;

			Info.Field = aField.ToArray();


			ftIndex = IndexOf(aWord, IndexFieldClose + 1, true, "values");
			if (ftIndex.From == -1)
				return null;

			List<string> aDelim = new List<string>();
			List<string> aValue = new List<string>();
			int IndexValueClose = -1;
			bool IsFirstValue = true;
			int IndexValueStart = (ftIndex.To + 2); // (1, '홍길동', 65)에서 "," 위치부터 검사
			for (int i = IndexValueStart; i < aWord.Count; i++)
			{
				string WordCur = aWord[i];

				if ((WordCur == ",")
					|| (!IsFirstValue && (WordCur == ")")))
				{
					if (CLang.In(aWord[i - 1], "'", "#"))
					{
						string Delim = aWord[i - 1];
						int IndexDelimEnd = i - 1;

						ftIndex = LastIndexOf(aWord, (IndexDelimEnd - 1), true, Delim);
						if (ftIndex.From == -1)
							return null;

						aDelim.Add(aWord[IndexDelimEnd]);

						int IndexDelimStart = ftIndex.From;

						aValue.Add(string.Join("", aWord.ToArray(), (IndexDelimStart + 1), (IndexDelimEnd - IndexDelimStart - 1)));
					}
					else
					{
						aDelim.Add("");
						aValue.Add(aWord[i - 1]);
					}

					if (aWord[i] == ")")
					{
						IndexValueClose = i;
						break;
					}
				}

				IsFirstValue = false;
			}

			if (IndexValueClose == -1)
				return null;

			Info.Delim = aDelim.ToArray();
			Info.Value = aValue.ToArray();

			return Info;
		}

		private static string MakeInsertOrUpdate(DbServerType DbServer, CSqlInfoInsert Info)
		{
			string Sql = "";

			if (DbServer == DbServerType.SQLServer)
			{
				string Table = Info.Table;

				string FieldValue = "";
				for (int i = 0; i < Info.Value.Length; i++)
				{
					if (!Info.IsPrimaryKey[i])
						FieldValue += ", " + Info.Field[i] + " = " + Info.Delim[i] + Info.Value[i] + Info.Delim[i];
				}
				FieldValue = FieldValue.Substring(2);

				string Where = Info.WhereByPrimaryKey;

				string FieldList = string.Join(", ", Info.Field);

				string ValueList = "";
				for (int i = 0; i < Info.Value.Length; i++)
				{
					ValueList += ", " + Info.Delim[i] + Info.Value[i] + Info.Delim[i];
				}
				ValueList = ValueList.Substring(2);

				Sql =
@"
update	{{Table}}
set	{{FieldValue}}
where	{{Where}};
if (@@rowcount = 0)
	insert into {{Table}}
		({{FieldList}})
	values
		({{ValueList}})".Replace("{{Table}}", Table)
						.Replace("{{FieldValue}}", FieldValue)
						.Replace("{{Where}}", Where)
						.Replace("{{FieldList}}", FieldList)
						.Replace("{{ValueList}}", ValueList);
			}


			return Sql;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="aValueAll"></param>
		/// <param name="StartIndex"></param>
		/// <param name="IgnoreCase"></param>
		/// <param name="aValue">null이면 와일드카드(*)의 역할을 함. 즉 모든 문자열을 허용함.</param>
		/// <returns></returns>
		private static SFromTo IndexOf(List<string> aValueAll, int StartIndex, bool IgnoreCase, params string[] aValue)
		{
			int IndexFirst = -1;
			int IndexLast = -1;
			string ValueAllCur = "";

			for (int i = StartIndex; i < aValueAll.Count; i++)
			{
				ValueAllCur = aValueAll[i];
				if ((aValue[0] != null) && (string.Compare(aValue[0], ValueAllCur, IgnoreCase) != 0))
					continue;

				IndexFirst = i;

				if ((i + aValue.Length) > aValueAll.Count)
					continue;

				bool IsFound = true;
				for (int j = 1; j < aValue.Length; j++)
				{
					ValueAllCur = aValueAll[i + j];
					if ((aValue[j] != null) && (string.Compare(aValue[j], ValueAllCur, IgnoreCase) != 0))
					{
						IsFound = false;
						break;
					}
				}
				if (!IsFound)
					continue;

				IndexLast = IndexFirst + aValue.Length - 1;

				return new SFromTo(IndexFirst, IndexLast);
			}

			return new SFromTo(-1, -1);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="aValueAll"></param>
		/// <param name="StartIndex"></param>
		/// <param name="IgnoreCase"></param>
		/// <param name="aValue">null이면 와일드카드(*)의 역할을 함. 즉 모든 문자열을 허용함.</param>
		/// <returns></returns>
		private static SFromTo LastIndexOf(List<string> aValueAll, int StartIndex, bool IgnoreCase, params string[] aValue)
		{
			int IndexFirst = -1;
			int IndexLast = -1;
			string ValueAllCur = "";

			for (int i = StartIndex; i >= 0; i--)
			{
				ValueAllCur = aValueAll[i];
				if ((aValue[aValue.Length - 1] != null) && (string.Compare(aValue[aValue.Length - 1], ValueAllCur, IgnoreCase) != 0))
					continue;

				IndexLast = i;

				bool IsFound = true;
				for (int j = (aValue.Length - 2); j >= 0; j--)
				{
					ValueAllCur = aValueAll[i - j];
					if ((aValue[j] != null) && (string.Compare(aValue[j], ValueAllCur, IgnoreCase) != 0))
					{
						IsFound = false;
						break;
					}
				}
				if (!IsFound)
					continue;

				IndexFirst = IndexLast - aValue.Length + 1;

				return new SFromTo(IndexFirst, IndexLast);
			}

			return new SFromTo(-1, -1);
		}
	}
}
