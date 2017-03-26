using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Xml;
using System.Linq;


namespace DoctorGu
{
	/// <summary>
	/// DataTable과 관련된 기능 구현.
	/// </summary>
	public class CDataTable
	{
		/// <summary>
		/// ADO의 GetString 함수와 같은 기능을 함.
		/// DataTable의 레코드를 구분자로 구분된 문자열로 변환해서 리턴함.
		/// </summary>
		/// <param name="dt">DataTable 개체</param>
		/// <param name="DelimRow">행 구분자</param>
		/// <param name="DelimCol">열 구분자</param>
		/// <param name="NullString">값이 Null일 때 대체할 문자열 값</param>
		/// <returns>구분자로 구분된 문자열</returns>
		/// <example>
		/// <code>
		/// DataTable dt = new DataTable();
		/// dt.Columns.Add("f1");
		/// dt.Columns.Add("f2");
		/// dt.Rows.Add(new object[] { "a1", "a2" });
		/// dt.Rows.Add(new object[] { "b1", "b2" });
		/// string s = CDataTable.ToString(dt, false, "\r\n", "\t", "");
		/// Console.WriteLine(s);
		/// <![CDATA[
		/// --s의 내용
		/// a1	  a2
		/// b1	  b2
		/// ]]>
		/// </code>
		/// </example>
		public static string ToString(System.Data.DataTable dt, bool IncludeColumnName, string DelimRow, string DelimCol, string NullString)
		{
			StringBuilder sb = new StringBuilder();

			bool IsFirstRow = true;

			if (IncludeColumnName)
			{
				bool IsFirstCol = true;
				foreach (DataColumn col in dt.Columns)
				{
					if (!IsFirstCol)
					{
						sb.Append(DelimCol);
					}
					else
					{
						IsFirstCol = false;
					}

					sb.Append(col.ColumnName);
				}

				IsFirstRow = false;
			}

			foreach (DataRow row in dt.Rows)
			{
				if (!IsFirstRow)
				{
					sb.Append(DelimRow);
				}
				else
				{
					IsFirstRow = false;
				}

				bool IsFirstCol = true;
				foreach (DataColumn col in dt.Columns)
				{
					if (!IsFirstCol)
					{
						sb.Append(DelimCol);
					}
					else
					{
						IsFirstCol = false;
					}

					object Value = row[col];
					string Text = CType.ConvertColumnValueToString(Value, NullString);

					sb.Append(Text.ToString());
				}
			}

			return sb.ToString();
		}
		public static string ToString(System.Data.DataTable dt, bool IncludeColumnName)
		{
			string DelimRow = "\r\n";
			string DelimCol = "\t";
			string NullString = CConst.NullString;

			return ToString(dt, IncludeColumnName, DelimRow, DelimCol, NullString);
		}
		public static string ToString(System.Data.DataTable dt)
		{
			bool IncludeColumnName = false;
			string DelimRow = "\r\n";
			string DelimCol = "\t";
			string NullString = CConst.NullString;

			return ToString(dt, IncludeColumnName, DelimRow, DelimCol, NullString);
		}

		///// <summary>
		///// <paramref name="PathFile"/>에서 지정된 텍스트 파일에 DataTable의 열 이름와 행의 내용을 저장함.
		///// </summary>
		///// <param name="dt">DataTable 개체</param>
		///// <param name="PathFile">생성될 텍스트 파일의 전체 경로. 이미 존재하면 에러 발생함.</param>
		///// <param name="DelimCol">열 구분자</param>
		///// <example>
		///// 다음은 C:\Test.txt에 DataTable의 열 이름과 열 구분자를 탭으로 하는 모든 행의 정보를 기록합니다.
		///// <code>
		///// CDataTable.OutputToText(dt, @"C:\Test.txt", "\t");
		///// </code>
		///// </example>
		//public static void OutputToText(System.Data.DataTable dt, string PathFile, string DelimCol)
		//{
			
		//    StreamWriter sw = new StreamWriter(PathFile, false, Encoding.UTF8);

		//    string Header = "";
		//    for (int i = 0, i2 = dt.Columns.Count; i < i2; i++)
		//    {
		//        Header += DelimCol + dt.Columns[i].ColumnName;
		//    }
		//    Header = Header.Substring(DelimCol.Length);
		//    sw.WriteLine(Header);


		//    foreach (DataRow row in dt.Rows)
		//    {
		//        string Value = "";

		//        foreach (DataColumn col in dt.Columns)
		//        {
		//            Value += DelimCol + row[col].ToString();
		//        }
		//        Value = Value.Substring(DelimCol.Length);

		//        sw.WriteLine(Value);
		//    }

		//    sw.Close();
		//}

#if !DotNet35
		public static List<dynamic> ConvertToDynamicList(DataTable dt)
		{
			List<dynamic> oList = new List<dynamic>();
			foreach (DataRow dr in dt.Rows)
			{
				dynamic row = GetDynamicRow(dt.Columns, dr);
				oList.Add(row);
			}

			return oList;
		}
		private static dynamic GetDynamicRow(DataColumnCollection cols, DataRow row)
		{
			dynamic Row = new CDynamicDictionary();

			foreach (DataColumn col in cols)
			{
				Row[col.ColumnName] = row[col];
			}

			return Row;
		}
#endif

		/// <summary>
		/// OracleDataReader 형식을 DataTable 형식으로 변환해서 리턴함.
		/// OracleDataReader는 Forward 전용이고, 열 이름으로 행을 가져올 수 없으므로
		/// DataTable 형식으로 변환하는 것이 편리한 경우가 있음.
		/// </summary>
		/// <param name="dr">OracleDataReader 개체</param>
		/// <returns>DataTable 개체</returns>
		/// <example>
		/// <code>
		/// DataTable dt = CDataTable.GetDataTableFromOracleDataReader(dr);
		/// </code>
		/// </example>
		public static DataTable GetDataTableFromOracleDataReader(OracleDataReader dr)
		{
			DataTable dt = new DataTable();
			for (int cl = 0, cl2 = dr.FieldCount; cl < cl2; cl++)
			{
				dt.Columns.Add(dr.GetName(cl));
			}

			while (dr.Read())
			{
				object[] o = new object[dr.FieldCount];
				for (int cl = 0, cl2 = dr.FieldCount; cl < cl2; cl++)
				{
					o[cl] = dr[cl];
				}
				dt.Rows.Add(o);
			}

			return dt;
		}

		/// <summary>
		/// DataTable의 특정 위치에 행의 내용을 삽입함.
		/// </summary>
		/// <param name="dt">DataTable 개체</param>
		/// <param name="aValue">값 목록</param>
		/// <param name="Index">삽입할 값 목록</param>
		/// <returns>값이 삽입된 DataTable 개체</returns>
		/// <example>
		/// 다음은 ComboBox의 첫번째에 테이블에 없는 값인 "(대지역)" 문자열을 표시하기 위해,
		/// 첫번째 열이 "", 두번째 열이 "(대지역)"인 내용을 첫번째 행에 삽입합니다.
		/// <code>
		/// string[] aKeyList = new string[] { "area", "off_line_category" };
		/// DataTable[] adt = CXmlClient.ExecSelectCustomCode(aKeyList);
		/// 
		/// DataTable dtArea = adt[0];
		/// dtArea = CDataTable.InsertAt(dtArea, new object[] { "", "(대지역)" }, 0);
		/// 
		/// CRowSource.SetRowSource(cboArea, dtArea, 2, "2;20");
		/// cboArea.SelectedIndex = 0;
		/// </code>
		/// </example>
		public static DataTable InsertAt(DataTable dt, object[] aValue, int Index)
		{
			DataRow dr = dt.NewRow();
			dr.ItemArray = aValue;
			dt.Rows.InsertAt(dr, Index);
			return dt;
		}

		/// <summary>
		/// DataTable의 DataColumn의 ReadOnly 속성을 설정함.
		/// </summary>
		/// <param name="dt">DataTable 개체</param>
		/// <param name="ReadOnly">읽기 전용으로 적용할 지 여부</param>
		/// <param name="ColumnNameExcept"><paramref name="ReadOnly"/>와 반대되는 값을 지정할 열이름</param>
		public static void SetColumnReadOnly(DataTable dt, bool ReadOnly, string ColumnNameExcept)
		{
			foreach (DataColumn dc in dt.Columns)
			{
				dc.ReadOnly = (dc.ColumnName == ColumnNameExcept) ? !ReadOnly : ReadOnly;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="ParentColumn"></param>
		/// <param name="ChildColumn"></param>
		/// <param name="RelationName"></param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// dt = CDataTable.GetHierarchicalDataTable(dt, ColumnNameId, ColumnNameParentId, "R");
		/// </code>
		/// </example>
		public static DataTable GetHierarchicalDataTable(DataTable dt,
			string ColumnNameId, string ColumnNameParentId, string RelationName)
		{
			DataSet ds = dt.DataSet;
			if (ds == null)
			{
				ds = new DataSet();
				ds.Tables.Add(dt);
			}

			DataRelation drel = new DataRelation(RelationName, dt.Columns[ColumnNameId], dt.Columns[ColumnNameParentId]);
			drel.Nested = true;
			ds.Relations.Add(drel);

			return dt;
		}

		public static DataTable RemoveAllRowsExceptOneRow(DataTable dt, DataRow RowExcept)
		{
			object[] aItem = RowExcept.ItemArray;

			dt.Rows.Clear();
			dt.Rows.Add(aItem);

			return dt;
		}
		public static DataTable RemoveAllRowsExceptOneRow(DataTable dt, int RowIndexExcept)
		{
			DataRow RowExcept = dt.Rows[RowIndexExcept];
			return RemoveAllRowsExceptOneRow(dt, RowExcept);
		}

		public static bool ColumnExists(DataTable dt, string ColumnName)
		{
			try { string Name = dt.Columns[ColumnName].ColumnName; }
			catch (Exception) { return false; }

			return true;
		}

		public static DataTable FilterRow(DataTable dt, string Filter, string Sort)
		{
			DataView dv = dt.DefaultView;
			dv.RowFilter = Filter;

			if (!string.IsNullOrEmpty(Sort))
				dv.Sort = Sort;
			
			return dv.ToTable();
		}
		public static DataTable FilterRow(DataTable dt, string Filter)
		{
			return FilterRow(dt, Filter, string.Empty);
		}

		/// <summary>
		/// DataTable이 가진 여러 개의 열 중, 원하는 열만을 가진 새로운 DataTable을 만들어 리턴함.
		/// </summary>
		/// <param name="dt">DataTable 개체</param>
		/// <param name="aColumnName">리턴되는 DataTable에 포함될 열 이름 목록</param>
		/// <returns><paramref name="aColumnName"/>에서 지정한 열만을 포함한 새로운 DataTable(행의 내용은 바뀌지 않음.)</returns>
		/// <example>
		/// 다음은 CRowSource.SetRowSource 함수를 호출하기 위해 kind, name 열만을 가진 DataTable로 만듭니다.
		/// <code>
		/// DataTable dt = CXmlClient.ExecSelectGet("pkg_manager.get_role_kind_list");
		/// dt = CDataTable.FilterColumn(dt, new string[] { "kind", "name" });
		/// CRowSource.SetRowSource(cboRoleKind, dt, 2, "3;50");
		/// </code>
		/// </example>
		public static DataTable FilterColumn(DataTable dt, params string[] aColumnName)
		{
			DataView dv = dt.DefaultView;
			return dv.ToTable(dt.TableName, false, aColumnName);
		}
		//public static DataTable FilterColumn(DataTable dt, string[] aColumnName)
		//{
		//    DataTable dtNew = new DataTable();
		//    for (int cl = 0, cl2 = aColumnName.Length; cl < cl2; cl++)
		//    {
		//        dtNew.Columns.Add(aColumnName[cl]);
		//    }

		//    foreach (DataRow dr in dt.Rows)
		//    {
		//        object[] o = new object[aColumnName.Length];
		//        for (int cl = 0, cl2 = aColumnName.Length; cl < cl2; cl++)
		//        {
		//            o[cl] = dr[aColumnName[cl]];
		//        }
		//        dtNew.Rows.Add(o);
		//    }

		//    return dtNew;
		//}

		/// <summary>
		/// DataTable에 중복된 행이 있다면 중복된 행을 제거한 행을 String의 배열 형식으로 리턴함.
		/// </summary>
		/// <param name="dt">DataTable 개체</param>
		/// <param name="ColumnName">값을 리턴한 열 이름</param>
		/// <returns>중복이 없는 String 배열</returns>
		/// <example>
		/// 다음은 f1 열에 a, a, b 행 중 유일한 값인 a, b만을 리턴합니다.
		/// <code>
		/// DataTable dt = new DataTable();
		/// dt.Columns.Add("f1");
		///
		/// dt.Rows.Add(new object[] { "a" });
		/// dt.Rows.Add(new object[] { "a" });
		/// dt.Rows.Add(new object[] { "b" });
		///
		/// DataTable dtDistinct = CDataTable.GetDistinctRow(dt, "f1");
		/// </code>
		/// </example>
		public static DataTable GetDistinctRow(DataTable dt, params string[] aColumnName)
		{
			return dt.DefaultView.ToTable(dt.TableName, true, aColumnName);
		}
		public static DataTable GetDistinctRow(DataTable dt)
		{
			string[] aColumnName = new string[dt.Columns.Count];
			for (int cl = 0; cl < dt.Columns.Count; cl++)
			{
				aColumnName[cl] = dt.Columns[cl].ColumnName;
			}

			return dt.DefaultView.ToTable(dt.TableName, true, aColumnName);
		}
		//public static string[] GetDistinctRow(DataTable dt, string ColumnName)
		//{
		//    List<string> aValue = new List<string>();
		//    for (int rw = 0, rw2 = dt.Rows.Count; rw < rw2; rw++)
		//    {
		//        string ValueCur = dt.Rows[rw][ColumnName].ToString();

		//        if (aValue.IndexOf(ValueCur) == -1)
		//        {
		//            aValue.Add(ValueCur);
		//        }
		//    }

		//    return aValue.ToArray();
		//}

		/// <summary>
		/// 1. DataTable의 ReadXml을 쓰면 xml:space="preserve"인 스페이스 값을 읽지 못해 DataSet의 ReadXml을 씀.
		/// 2. DataSet의 ReadXml을 썼을 때 에러나는 경우 있다면 DataTable의 ReadXml을 씀.
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="FullPathXml"></param>
		/// <returns></returns>
		public static DataTable ReadXml(TextReader tr)
		{
			DataTable dtNew = null;

			try
			{
				DataSet ds = new DataSet();
				ds.ReadXml(tr);
				dtNew = ds.Tables[0];
			}
			catch (Exception)
			{
				dtNew = new DataTable();
				dtNew.ReadXml(tr);
			}

			return dtNew;
		}
		public static DataTable ReadXml(string FullPathXml)
		{
			string Value = CFile.GetTextInFile(FullPathXml);
			return ReadXml(Value);
		}

		/// <summary>
		/// 테이블을 XML 문자열로 변경해서 리턴함.
		/// </summary>
		/// <param name="ds"></param>
		/// <param name="WriteMode">XmlWriteMode.IgnoreSchema를 쓰면 스키마 없는 간단한 XML 형식을 리턴함.</param>
		/// <returns></returns>
		public static string WriteXmlString(DataSet ds, XmlWriteMode WriteMode)
		{
			StringWriter sw = new StringWriter();
			ds.WriteXml(sw, WriteMode);
			return sw.ToString();
		}
		/// <summary>
		/// XML 문자열을 파싱해서 테이블 형식으로 변경해서 리턴함.
		/// </summary>
		/// <param name="XmlString"></param>
		/// <returns></returns>
		public static DataSet ReadXmlString(string XmlString)
		{
			DataSet ds = new DataSet();
			using (XmlTextReader xr = new XmlTextReader(new StringReader(XmlString)))
			{
				ds.ReadXml(xr);
			}

			return ds;
		}

		public static int UpDownRow(DataTable dt, int IndexSrc, bool IsUp)
		{
			if (dt.Rows.Count == 0)
				return IndexSrc;


			DataRow dr = dt.Rows[IndexSrc];
			DataRow dr2 = CopyRow(dt, IndexSrc, false);

			int IndexNew = IndexSrc;

			if (IsUp)
			{
				if (IndexSrc == 0)
					return IndexNew;

				IndexNew = IndexSrc - 1;

				//Unique가 true인 경우 중복값으로 에러날 수 있으므로 미리 제거함
				dt.Rows.Remove(dr);
				dt.Rows.InsertAt(dr2, IndexNew);
			}
			else
			{
				if ((IndexSrc + 1) == dt.Rows.Count)
					return IndexNew;

				IndexNew = IndexSrc + 1;

				//Unique가 true인 경우 중복값으로 에러날 수 있으므로 미리 제거함
				dt.Rows.Remove(dr);

				if (IndexNew == dt.Rows.Count)
					dt.Rows.Add(dr2);
				else
					dt.Rows.InsertAt(dr2, IndexNew);
			}

			return IndexNew;
		}

		public static int MoveRow(DataTable dt, int IndexSrc, int IndexDest)
		{
			if (dt.Rows.Count == 0)
				return -1;

			DataRow drOld = dt.Rows[IndexSrc];
			DataRow drNew = dt.NewRow();
			drNew.ItemArray = drOld.ItemArray;

			//Unique가 true인 경우 중복값으로 에러날 수 있으므로 미리 제거함
			dt.Rows.RemoveAt(IndexSrc);

			if (IndexSrc < IndexDest)
				IndexDest--;

			if (IndexDest == dt.Rows.Count)
				dt.Rows.Add(drNew);
			else
				dt.Rows.InsertAt(drNew, IndexDest);

			return dt.Rows.IndexOf(drNew);
		}

		public static DataRow CopyRow(DataTable dt, int RowIndex, bool NewValueIfUnique)
		{
			DataRow dr = dt.NewRow();
			for (int cl = 0, cl2 = dt.Columns.Count; cl < cl2; cl++)
			{
				dr[cl] = dt.Rows[RowIndex][cl];
			}

			if (NewValueIfUnique)
			{
				for (int cl = 0; cl < dt.Columns.Count; cl++)
				{
					if (dt.Columns[cl].Unique)
					{
						string ColumnName = dt.Columns[cl].ColumnName;

						Type t = dr[cl].GetType();
						if (t == typeof(string))
						{
							string Value = (string)dr[cl];

							string ValueNew = Value;
							int Index = 0;

							while (dt.Select(ColumnName + " = '" + ValueNew + "'").Length > 0)
							{
								ValueNew = string.Format("{0}({1})", Value, ++Index);
							}

							dr[cl] = ValueNew;
						}
						else
						{
							throw new Exception(string.Format("Wrong Type: {0}", t));
						}
					}
				}
			}

			return dr;
		}

		/// <summary>
		/// 행과 열을 서로 바꿈. 첫번째 열의 모든 행은 열이 될 것이므로 중복되면 안됨.
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// 
		/// SECTION_TYPE	COUNT_INPUT_ASSIGN	COUNT_INPUT_NO_INPUT
		/// L	135	103
		/// R	85	75
		/// S	11	7
		/// W	3	0
		/// LRSW	234	185
		/// ->
		/// SECTION_TYPE	L	R	S	W	LRSW
		/// COUNT_INPUT_ASSIGN	135	85	11	3	234
		/// COUNT_INPUT_NO_INPUT	103	75	7	0	185
		/// 
		/// DataTable dt = new DataTable();
		/// dt.Columns.Add("SECTION_TYPE", typeof(string));
		/// dt.Columns.Add("COUNT_INPUT_ASSIGN", typeof(int));
		/// dt.Columns.Add("COUNT_INPUT_NO_INPUT", typeof(int));
		/// dt.Rows.Add("L", 135, 103);
		/// dt.Rows.Add("R", 85, 75);
		/// dt.Rows.Add("S", 11, 7);
		/// dt.Rows.Add("W", 3, 0);
		/// dt.Rows.Add("LRSW", 234, 185);
		/// DataTable dtNew = CDataTable.Pivot(dt);
		/// string s = CDataTable.ToString(dtNew);
		/// ]]>
		/// </example>		
		public static DataTable Pivot(DataTable dt)
		{
			DataTable dtNew = new DataTable();

			//첫번째 열 제외한 나머지 열이 모두 같은 형식이면 새 DataTable도 그 형식의 열을 추가함.
			Type TypeOfColumns = dt.Columns[1].DataType;
			bool IsAllSameColumn = true;
			for (int cl = 2; cl < dt.Columns.Count; cl++)
			{
				Type TypeCur = dt.Columns[cl].DataType;
				if (TypeCur != TypeOfColumns)
				{
					IsAllSameColumn = false;
					break;
				}
			}

			DataRow drNew = null;
			dtNew.Columns.Add(new DataColumn(dt.Columns[0].ColumnName, typeof(String)));

			Type TypeOfRest = IsAllSameColumn ? TypeOfColumns : typeof(string);
			for (int rw = 0; rw < dt.Rows.Count; rw++)
			{
				dtNew.Columns.Add(new DataColumn(dt.Rows[rw][0].ToString(), TypeOfRest));
			}

			for (int cl = 1; cl < dt.Columns.Count; cl++)
			{
				drNew = dtNew.NewRow();

				//First column's data is from orininal column name.
				drNew[0] = dt.Columns[cl].ColumnName;

				for (int rw = 0; rw < dt.Rows.Count; rw++)
				{
					drNew[rw + 1] = dt.Rows[rw][cl];
				}

				dtNew.Rows.Add(drNew);
			}
			return dtNew;
		}

		public static DataSet Merge(params DataSet[] aDs)
		{
			DataSet dsNew = new DataSet();


			for (int cl = 0; cl < aDs[0].Tables.Count; cl++)
			{
				DataTable[] dtToMerge = new DataTable[aDs.Length];
				for (int rw = 0; rw < aDs.Length; rw++)
				{
					DataSet dsCur = aDs[rw];
					dtToMerge[rw] = dsCur.Tables[cl];
				}

				DataTable dtMerged = CDataTable.Merge(dtToMerge);
				dsNew.Tables.Add(dtMerged);
			}

			return dsNew;
		}
		public static DataTable Merge(params DataTable[] aDt)
		{
			DataTable dtNew = aDt[0].Clone();

			for (int i = 0; i < aDt.Length; i++)
			{
				dtNew.Merge(aDt[i]);
			}

			return dtNew;
		}

		/// <summary>
		/// DataTable이 이미 DataSet을 가지고 있다면 해당 DataSet에서 제거한 후 새로운 DataSet에 추가함.
		/// </summary>
		public static DataSet AddDataTables(params DataTable[] aDt)
		{
			DataSet ds = new DataSet();

			for (int i = 0; i < aDt.Length; i++)
			{
				DataTable dt = aDt[i];

				if (dt.DataSet != null)
					dt.DataSet.Tables.Remove(dt);

				dt.TableName = "Table" + i.ToString();

				ds.Tables.Add(dt);
			}

			return ds;
		}

		public static DataTable SortByRandom(Random Rnd, DataTable dt)
		{
			DataTable dtNew = dt.Clone();

			int[] aOrder = CRandomUnique.GenerateRandomlyOrdered(Rnd, 0, dt.Rows.Count);
			for (int i = 0; i < aOrder.Length; i++)
			{
				dtNew.ImportRow(dt.Rows[aOrder[i]]);
			}

			return dtNew;
		}
		public static DataTable SortByRandom(DataTable dt)
		{
			return SortByRandom(new Random(), dt);
		}

		public static void SortByRelation(DataTable dt, string ColumnNameId, string ColumnNameParentId)
		{
			DataRow[] adr = dt.Select(string.Format("{0} > ''", ColumnNameParentId));
			if (adr.Length == 0)
				return;

			object ParentId = adr[0][ColumnNameParentId];
			SqlColumnTypeSimple ColumnType = CSql.GetColumnTypeSimple(ParentId);
			if ((ColumnType == SqlColumnTypeSimple.DateTime) || (ColumnType == SqlColumnTypeSimple.Boolean))
				throw new Exception(string.Format("ColumnType: '{0}' is not allowed.", ColumnType));

			string Parenthesis = (ColumnType == SqlColumnTypeSimple.String) ? "'" : "";

			//하위 행을 추가할 때 원래의 순서를 유지하기 위해 거꾸로 루핑을 함
			for (int rw = (adr.Length - 1); rw >= 0; rw--)
			{
				DataRow dr = adr[rw];
				object ParentIdCur = dr[ColumnNameParentId];

				string Filter = string.Format("{0} = {1}{2}{1}", ColumnNameId, Parenthesis, ParentIdCur);
				DataRow[] adrParent = dt.Select(Filter);
				if (adrParent.Length == 0)
					continue;
				else if (adrParent.Length >= 2)
					throw new Exception(string.Format("{0} is not unique. Duplicated value: {1}", ColumnNameId, ParentIdCur));

				DataRow drParent = adrParent[0];
				CDataTable.MoveRow(dt, dt.Rows.IndexOf(dr), dt.Rows.IndexOf(drParent) + 1);
			}
		}
	}
}
