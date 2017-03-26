using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Collections;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DoctorGu
{
	/// <summary>
	/// ERwin 프로그램에서 Physical View와 Logical View를 SQL문 스크립트를 읽어서 자동으로 표시하는 데 필요한 기능 구현.
	/// </summary>
	/// <remarks>
	/// 이렇게 변경하는 이유는 TOAD에서 자동 생성된 SQL문을 읽어 ERwin에서 Physical View를 만들 수는 있지만,
	/// 한글 설명이 표시된 Logical View를 자동으로 만들 수 없기 때문에 COMMENT를 이용해서 Logical View를 자동으로 만들기 위함.
	/// </remarks>
	/// <example>
	/// <![CDATA[
	/// 1. Toad, SQL Navigator 등의 툴에서 테이블 스크립트를 생성.(table.sql)
	///    (옵션 중에서 Schema name이 체크되면 "CREATE TABLE ESHOP.TBL_SHOP_TEST"와 같이 생성되므로 Schema name의 체크 해제할 것)
	///
	/// 2. ER Win에서 table.sql을 다음 옵션으로 연 후, table.xml로 저장
	///    New Model Type: Logical/Physical 선택, Database: Oracle, Version: 9x 선택
	///
	/// 3. table.sql, table.xml 파일의 경로를 입력 후, [XML 내용 변경] 단추 누름.
	/// string SqlFullPath = txtSqlFullPath.Text;
	/// if (!File.Exists(SqlFullPath))
	/// {
	///     MessageBox.Show(lblSqlFullPath.Text + " " + SqlFullPath + " 파일이 없습니다.");
	///     return;
	/// }
	///
	/// string XmlFullPath = txtXmlFullPath.Text;
	/// if (!File.Exists(XmlFullPath))
	/// {
	///     MessageBox.Show(lblXmlFullPath.Text + " " + XmlFullPath + " 파일이 없습니다.");
	///     return;
	/// }
	///
	/// string Script = CFile.GetTextInFile(SqlFullPath, Encoding.Default);
	/// Dictionary<string, string> dicTable;
	/// Dictionary<string, Dictionary<string, string>> dicColumn;
	/// CErWin.GetTableColumnAndComment(Script, out dicTable, out dicColumn);
	/// CErWin.ReplaceNameWithComment(XmlFullPath, dicTable, dicColumn);
	/// CErWin.ChangeFontNameSize(XmlFullPath, "맑은 고딕", 9);
	///
	/// MessageBox.Show(XmlFullPath + " 변경 완료");
	/// ]]>
	/// </example>
	public class CErWin
	{
		public static void GetTableColumnAndComment(DbServerType DbType, string Script,
			out Dictionary<string, string> dicTableIs,
			out Dictionary<string, Dictionary<string, string>> dicColumnIs)
		{
			dicTableIs = new Dictionary<string, string>();
			dicColumnIs = new Dictionary<string, Dictionary<string, string>>();

			string PatternTable = "";
			string PatternColumn = "";

			switch (DbType)
			{
				case DbServerType.SQLServer:
					string PatternCommon = @"sp_addextendedproperty.*@value=N'(?<Description>[^']*)'.*@level1name=N'(?<Table>[^']*)'";
					PatternTable = PatternCommon + @"[\r\n]";
					PatternColumn = PatternCommon + @".*@level2name=N'(?<Column>[^']*)'";
					break;
				case DbServerType.Oracle:
					PatternTable = @"COMMENT\sON\sTABLE\s(?<Table>\w+)\sIS\s'(?<Description>[^']+)';";
					PatternColumn = @"COMMENT\sON\sCOLUMN\s(?<Table>\w+)\.(?<Column>\w+)\sIS\s'(?<Description>[^']+)';";
					break;
				default:
					throw new Exception(string.Format("{0} is not supported.", DbType));
			}

			{
				Regex r = new Regex(PatternTable, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
				foreach (Match m in r.Matches(Script))
				{
					string Table = m.Groups["Table"].Value;
					string Description = m.Groups["Description"].Value;
					dicTableIs.Add(Table, Description);
				}
			}

			{
				Regex r = new Regex(PatternColumn, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
				foreach (Match m in r.Matches(Script))
				{
					string Table = m.Groups["Table"].Value;
					string Column = m.Groups["Column"].Value;
					string Description = m.Groups["Description"].Value;

					if (dicColumnIs.ContainsKey(Table))
					{
						dicColumnIs[Table].Add(Column, Description);
					}
					else
					{
						Dictionary<string, string> dicSub = new Dictionary<string, string>();
						dicSub.Add(Column, Description);
						dicColumnIs.Add(Table, dicSub);
					}
				}
			}
		}

		//private struct SWordInfo
		//{
		//    public int Index, Length;
		//    public string Value, ValueNew;
		//}

		///// <summary>
		///// SQL 스크립트를 읽어 테이블 이름과 테이블 설명, 열 이름과 열 설명을 가진 NameValueCollection 개체를 리턴함.
		///// ReplaceNameWithComment 함수를 호출하기 위해 사용됨.
		///// </summary>
		///// <param name="Script">SQL 스크립트</param>
		///// <returns>NameValueCollection 개체</returns>
		//public static void GetTableColumnAndComment(string Script,
		//    out Dictionary<string, string> dicTable, out Dictionary<string, Dictionary<string, string>> dicColumn)
		//{
		//    dicTable = new Dictionary<string, string>();
		//    dicColumn = new Dictionary<string, Dictionary<string, string>>();

		//    SWordInfo[] awi = GetWordInfo(Script);

		//    for (int n = 0, n2 = awi.Length; n < n2; n++)
		//    {
		//        switch (awi[n].Value.ToLower())
		//        {
		//            case "comment":
		//                bool IsTableIs;
		//                string TableIs, TableCommentIs, ColumnIs, ColumnCommentIs;
		//                if (GetTableOrColumnComment(Script, awi, n, out IsTableIs,
		//                    out TableIs, out TableCommentIs,
		//                    out ColumnIs, out ColumnCommentIs))
		//                {
		//                    if (IsTableIs)
		//                    {
		//                        dicTable.Add(TableIs, TableCommentIs);
		//                    }
		//                    else
		//                    {
		//                        if (dicColumn.ContainsKey(TableIs))
		//                        {
		//                            dicColumn[TableIs].Add(ColumnIs, ColumnCommentIs);
		//                        }
		//                        else
		//                        {
		//                            Dictionary<string, string> dicSub = new Dictionary<string,string>();
		//                            dicSub.Add(ColumnIs, ColumnCommentIs);
		//                            dicColumn.Add(TableIs, dicSub);
		//                        }
		//                    }
		//                }
		//                break;
		//        }
		//    }
		//}

		//private static SWordInfo[] GetWordInfo(string Script, params char[] ValueToIncludeAsWord)
		//{
		//    List<SWordInfo> awi = new List<SWordInfo>();

		//    string Pattern = @"\w+";
		//    if (ValueToIncludeAsWord != null)
		//    {
		//        for (int i = 0, i2 = ValueToIncludeAsWord.Length; i < i2; i++)
		//        {
		//            Pattern += "|\\" + ValueToIncludeAsWord[i].ToString();
		//        }
		//    }

		//    Regex r = new Regex(Pattern, RegexOptions.Compiled);
		//    for (Match m = r.Match(Script); m.Success; m = m.NextMatch())
		//    {
		//        SWordInfo wi = new SWordInfo();
		//        wi.Index = m.Index;
		//        wi.Length = m.Length;
		//        wi.Value = m.Value;
		//        awi.Add(wi);
		//    }

		//    return awi.ToArray();
		//}
		//private static SWordInfo[] GetWordInfo(string Script)
		//{
		//    return GetWordInfo(Script, null);
		//}

		//private static bool GetTableOrColumnComment(string Script, SWordInfo[] awi, int Idx,
		//    out bool IsTable,
		//    out string TableIs, out string TableCommentIs,
		//    out string ColumnIs, out string ColumnCommentIs)
		//{
		//    IsTable = false;
		//    TableIs = "";
		//    TableCommentIs = "";
		//    ColumnIs = "";
		//    ColumnCommentIs = "";

		//    if (awi.Length < (Idx + 3))
		//    {
		//        return false;
		//    }

		//    if (awi[Idx + 1].Value.ToLower() != "on")
		//    {
		//        return false;
		//    }

		//    string ObjType = awi[Idx + 2].Value;
		//    if ((ObjType.ToLower() != "table")
		//        && (ObjType.ToLower() != "column"))
		//    {
		//        return false;
		//    }

		//    IsTable = (ObjType.ToLower() == "table");
		//    int IndexIs = 0;
		//    if (IsTable)
		//    {
		//        TableIs = awi[Idx + 3].Value;
		//        IndexIs = Script.IndexOf("is", awi[Idx + 3].Index + 1, StringComparison.CurrentCultureIgnoreCase);
		//    }
		//    else
		//    {
		//        TableIs = awi[Idx + 3].Value;
		//        ColumnIs = awi[Idx + 4].Value;
		//        IndexIs = Script.IndexOf("is", awi[Idx + 4].Index + 1, StringComparison.CurrentCultureIgnoreCase);
		//    }

		//    if (IndexIs == -1)
		//    {
		//        return false;
		//    }

		//    string Comment = CDelim.GetFirstQuotedValue(Script, IndexIs);

		//    if (IsTable)
		//    {
		//        TableCommentIs = Comment;
		//    }
		//    else
		//    {
		//        ColumnCommentIs = Comment;
		//    }

		//    return true;
		//}

		//private static bool SetTableValue(SWordInfo[] awi, int Idx, string Table, string TableComment)
		//{
		//    bool IsFound = false;
		//    for (int i = 0, i2 = awi.Length; i < i2; i++)
		//    {
		//        if (String.Compare(awi[i].Value, Table, true) == 0)
		//        {
		//            if (awi[i - 1].Value.ToLower() == "table")
		//            {
		//                if ((awi[i - 2].Value.ToLower() == "create") || (awi[i - 2].Value.ToLower() == "alter"))
		//                {
		//                    IsFound = true;
		//                    awi[i].ValueNew = "\"" + TableComment + "\"";
		//                }
		//            }
		//            else if (awi[i - 1].Value.ToLower() == "references")
		//            {
		//                IsFound = true;
		//                awi[i].ValueNew = "\"" + TableComment + "\"";
		//            }
		//            else if ((awi[i - 1].Value.ToLower() == "on") && (awi[i - 3].Value.ToLower() == "index"))
		//            {
		//                IsFound = true;
		//                awi[i].ValueNew = "\"" + TableComment + "\"";
		//            }
		//        }
		//    }

		//    return IsFound;
		//}

		//private static bool SetColumnValue(SWordInfo[] awi, int Idx, string Table, string Column, string ColumnComment)
		//{
		//    bool IsFound = false;
		//    for (int i = 0, i2 = awi.Length; i < i2; i++)
		//    {
		//        if (string.Compare(awi[i].Value, Table, true) == 0)
		//        {
		//            if (awi[i - 1].Value.ToLower() == "table")
		//            {
		//                if ((awi[i - 2].Value.ToLower() == "create") || (awi[i - 2].Value.ToLower() == "alter"))
		//                {
		//                    if (SetColumnValueFromPos(i, awi, Column, ColumnComment))
		//                    {
		//                        IsFound = true;
		//                    }
		//                }
		//            }
		//            else if (awi[i - 1].Value.ToLower() == "references")
		//            {
		//                if (SetColumnValueFromPos(i, awi, Column, ColumnComment))
		//                {
		//                    IsFound = true;
		//                }
		//            }
		//            else if ((awi[i - 1].Value.ToLower() == "on") && (awi[i - 3].Value.ToLower() == "index"))
		//            {
		//                if (SetColumnValueFromPos(i, awi, Column, ColumnComment))
		//                {
		//                    IsFound = true;
		//                }
		//            }
		//        }
		//    }

		//    return IsFound;
		//}
		//private static bool SetColumnValueFromPos(int PosFrom, SWordInfo[] awi,
		//    string Column, string ColumnComment)
		//{
		//    bool IsFound = false;
		//    bool IsQuoteStarted = false;
		//    for (int j = (PosFrom + 1), j2 = awi.Length; j < j2; j++)
		//    {
		//        if (awi[j].Value == "\"")
		//        {
		//            IsQuoteStarted = !IsQuoteStarted;
		//        }

		//        if (IsQuoteStarted) continue;
				
		//        if (string.Compare(awi[j].Value, Column, true) == 0)
		//        {
		//            awi[j].ValueNew = "\"" + ColumnComment + "\"";
		//            IsFound = true;
		//            //break를 하지 않으면 다른 테이블의 같은 Column까지 찾게 됨.
		//            break;
		//        }
		//        else if (awi[j].Value == ";")
		//        {
		//            break;
		//        }
		//    }

		//    return IsFound;
		//}

		//버전 7에서 속성이 많이 바뀌어 사용 안함.
		///// <summary>
		///// ERwin에서 저장한 XML 파일의 내용 중 글꼴이름, 글꼴크기 관련 값을 새로운 값으로 변경함.
		///// </summary>
		///// <param name="XmlFullPath">ERwin에서 저장한 XML 파일</param>
		///// <param name="FontName">새 글꼴 이름</param>
		///// <param name="FontSize">새 글꼴 크기</param>
		///// <returns>글꼴 정보가 변경되었는 지 여부</returns>
		//public static bool ChangeFontNameSize(string XmlFullPath, string FontName, int FontSize)
		//{
		//    XmlDocument XDoc = new XmlDocument();
		//    XDoc.Load(XmlFullPath);

		//    string NameText = GetNameTextByVersion(XDoc);

		//    string NewValue = FontName + "-" + FontSize.ToString() + "-0-0-0-0";
		//    bool IsChanged = false;

		//    string XPath = "//Font[@" + NameText + "]";
		//    XmlNodeList List = XDoc.SelectNodes(XPath);
		//    foreach (XmlNode NodeCur in List)
		//    {
		//        if (NodeCur.Attributes[NameText].Value != NewValue)
		//        {
		//            NodeCur.Attributes[NameText].Value = NewValue;
		//            IsChanged = true;
		//        }

		//        XPath = "FontProps/" + NameText;
		//        XmlElement ElemName = (XmlElement)NodeCur.SelectSingleNode(XPath);
		//        if (ElemName.InnerText != NewValue)
		//        {
		//            ElemName.InnerText = NewValue;
		//            IsChanged = true;
		//        }

		//        XPath = "FontProps/Font_Name";
		//        XmlElement ElemFontName = (XmlElement)NodeCur.SelectSingleNode(XPath);
		//        if (ElemFontName.InnerText != FontName)
		//        {
		//            ElemFontName.InnerText = FontName;
		//            IsChanged = true;
		//        }

		//        XPath = "FontProps/Font_Size";
		//        XmlElement ElemFontSize = (XmlElement)NodeCur.SelectSingleNode(XPath);
		//        if (ElemFontSize.InnerText != FontSize.ToString())
		//        {
		//            ElemFontSize.InnerText = FontSize.ToString();
		//            IsChanged = true;
		//        }
		//    }

		//    if (IsChanged)
		//    {
		//        XDoc.Save(XmlFullPath);
		//    }

		//    return IsChanged;
		//}

		public static void ReplaceNameWithComment(DbServerType DbType, string XmlFullPath,
			Dictionary<string, string> dicTable,
			Dictionary<string, Dictionary<string, string>> dicColumn)
		{
			XmlDocument XDoc = new XmlDocument();
			XDoc.Load(XmlFullPath);

			Version v = GetVersion(XDoc);

			// 7 버전에서는 Physical을 추가하면 Logical의 필드 양쪽에 _가 붙는 현상 있어 포기.
			if (v.Major >= 7)
				throw new Exception("7 버전은 지원하지 않습니다.");

			XmlNamespaceManager ns = (v.Major >= 7) ? CXml.AddNamespace(XDoc) : null;
			string Prefix = (v.Major >= 7) ? "EMX:" : "";

			string NameText = "";
			switch (DbType)
			{
				case DbServerType.SQLServer:
					NameText = "name";
					break;
				case DbServerType.Oracle:
					NameText = "Name";
					break;
				default:
					throw new Exception(string.Format("Wrong DbType:{0}", DbType));
			}

			foreach (KeyValuePair<string, string> kvTable in dicTable)
			{
				XmlNode nodEntity = XDoc.SelectSingleNode("//" + Prefix + "Entity[@" + NameText + "='" + kvTable.Key + "']", ns);
				XmlNode nodEntityPropsName = nodEntity.SelectSingleNode(Prefix + "EntityProps/" + Prefix + "Name", ns);

				nodEntity.Attributes[NameText].Value = kvTable.Value;
				nodEntityPropsName.ChildNodes[0].Value = kvTable.Value;

				if (v.Major >= 7)
				{
					//XmlNode nodModelProps = XDoc.SelectSingleNode("//" + Prefix + "ModelProps", ns);
					//XmlNode nodStandards = nodModelProps.SelectSingleNode(Prefix + "Suspend_Naming_Standards", ns);
					//if (nodStandards == null)
					//{
					//    nodModelProps.AppendChild(XDoc.CreateElement("Suspend_Naming_Standards")).InnerText = "false";
					//}

					//XmlNode nodPhysicalOnly = nodEntityPropsName.SelectSingleNode(Prefix + "Physical_Only", ns);
					//if (nodPhysicalOnly == null)
					//{
					//    nodEntityPropsName.AppendChild(XDoc.CreateElement("Physical_Only")).InnerText = "false";
					//    nodEntityPropsName.AppendChild(XDoc.CreateElement("Physical_Name")).InnerText = kvTable.Key;
					//}

					XmlNode nodDrawingObjectEntity = XDoc.SelectSingleNode("//" + Prefix + "Drawing_Object_Entity_Groups/" + Prefix + "Drawing_Object_Entity[@" + NameText + "='" + kvTable.Key + "']", ns);
					if (nodDrawingObjectEntity != null)
						nodDrawingObjectEntity.Attributes[NameText].Value = kvTable.Value;
				}


				foreach (KeyValuePair<string, Dictionary<string, string>> kvColumn in dicColumn)
				{
					if (kvColumn.Key != kvTable.Key)
						continue;

					foreach (KeyValuePair<string, string> kvColumnSub in kvColumn.Value)
					{
						XmlNode nodAttribute = nodEntity.SelectSingleNode(Prefix + "Attribute_Groups/" + Prefix + "Attribute[@" + NameText + "='" + kvColumnSub.Key + "']", ns);
						XmlNode nodAttributePropsName = nodAttribute.SelectSingleNode(Prefix + "AttributeProps/" + Prefix + "Name", ns);

						nodAttribute.Attributes[NameText].Value = kvColumnSub.Value;
						nodAttributePropsName.ChildNodes[0].Value = kvColumnSub.Value;
					}
				}
			}

			XDoc.PreserveWhitespace = true;
			XDoc.Save(XmlFullPath);
		}

		private static Version GetVersion(XmlDocument XDoc)
		{
			XmlAttribute Attr = XDoc.DocumentElement.Attributes["FileVersion"];
			if (Attr == null)
				return new Version();

			int Version = CFindRep.IfNotNumberThen0(Attr.Value);
			if (Version > 0)
			{
				//4255 -> 4.2.5.5
				string[] aVersion = CArray.SplitByLength(Attr.Value, 1);
				return new Version(Convert.ToInt32(aVersion[0]), Convert.ToInt32(aVersion[1]), Convert.ToInt32(aVersion[2]), Convert.ToInt32(aVersion[3]));
			}
			else
			{
				return new Version(Attr.Value);
			}
		}
	}
}
