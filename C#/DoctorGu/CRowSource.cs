using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using DoctorGu;
using System.Collections.Specialized;

namespace DoctorGu
{
	/// <summary>
	/// 액세스의 RowSource 형식과 유사하게 사용할 수 있도록 함.
	/// </summary>
	/// <example>
	/// string ValueList = "1;망치;2;못;3;톱;4;실";
	/// int ColumnCount = 2;
	/// string ColumnWidth = "2;5";
	/// RowSource rs = new RowSource(cbo, ValueList, ColumnCount, ColumnWidth);
	/// // 첫번째 필드의 두번째 항목인 2를 선택함.
	/// rs.SelectItem(2.ToString(), 0);
	/// //현재 선택된 행의 두번째 필드의 값인 '못'을 가져옴.
	/// this.Text = (string)RowSource.Column(cbo, 1);
	/// </example>
	public class CRowSource
	{
		private static ComboBox mComboBox = null;
		private static SourceType mSourceType = SourceType.TableQuery;
		private static string mRowSource = "";
		private static int mColumnCount = 0;
		private static string[] mColumnWidth = null;
		
		//다른 필드와 중복을 방지하기 위함.
		private const string drTextListFieldName = "TextList324821496734623";
		private const int drDefaultColumnWidth = 10;

		private CRowSource(){}

		public static void SetRowSource(ComboBox cbo, DataTable dt, int ColumnCount, string ColumnWidthSemiColon)
		{
			SetRowSource(cbo, dt, "", ColumnCount, ColumnWidthSemiColon);
		}
		public static void SetRowSource(ComboBox cbo, string ValueListSemiColon, int ColumnCount, string ColumnWidthSemiColon)
		{
			SetRowSource(cbo, null, ValueListSemiColon, ColumnCount, ColumnWidthSemiColon);
		}
		public static void SetRowSource(ComboBox cbo, NameValueCollection nvNameDesc, int ColumnCount, string ColumnWidthSemiColon)
		{
			string ValueList = "";
			for (int i = 0, i2 = nvNameDesc.Count; i < i2; i++)
			{
				ValueList += ";" + nvNameDesc.GetKey(i) + ";" + nvNameDesc[i];
			}
			if (ValueList != "")
			{
				ValueList = ValueList.Substring(1);
			}

			SetRowSource(cbo, null, ValueList, ColumnCount, ColumnWidthSemiColon);
		}
		private static void SetRowSource(ComboBox cbo, DataTable dt, 
			string ValueListSemiColon, int ColumnCount, string ColumnWidthSemiColon)
		{
			mComboBox = cbo;

			if (dt != null) 
			{
				mSourceType = SourceType.TableQuery;
			}
			else
			{
				mSourceType = SourceType.ValueList;
				mRowSource = ValueListSemiColon;
			}

			mColumnCount = ColumnCount;

			mColumnWidth = ColumnWidthSemiColon.Split(';');

			if (mSourceType == SourceType.ValueList)
			{
				dt = GetDataTableByValueList();
			}

			bool IsNoRecord = (dt.Rows.Count == 0);
			
			int[] aColumnWidth = GetColumnWidth();
			bool[] aIsLeftAlign = GetIsLeftAlign(dt);

			mComboBox.Items.Clear();

			if (IsNoRecord) return;

			dt.Columns.Add(drTextListFieldName, Type.GetType("System.String"));
			foreach (DataRow row in dt.Rows)
			{
				string TextList = "";
				for (int cl = 0, cl2 = mColumnCount; cl < cl2; cl++)
				{
					string Text = row[cl].ToString();
					if (aIsLeftAlign[cl])
					{
						TextList += " " + CTwoByte.PadRightH(Text, aColumnWidth[cl], ' ');
					}
					else
					{
						TextList += " " + CTwoByte.PadLeftH(Text, aColumnWidth[cl], ' ');
					}
				}
				TextList = TextList.Substring(1);

				row[drTextListFieldName] = TextList;
				mComboBox.Items.Add(TextList);
			}

			mComboBox.Tag = dt;
		}

		/// <summary>
		/// Value를 가진 행을 선택함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Column"></param>
		/// <example>
		/// //다음 코드는 첫번째 열의 값이 "2"인 행을 찾으므로 2번째 행을 선택함.
		/// RowSource.SelectItem(cbo, "2", 0);
		/// //다음 코드는 두번째 열의 값이 "3급"인 행을 찾으므로 3번째 행을 선택함.
		/// RowSource.SelectItem(cbo, "3급", 1);
		/// </example>
		public static void SelectItem(ComboBox cbo, object Value, object Column)
		{
			if (cbo.Tag == null)
			{
				throw new Exception("Tag 속성이 없으므로 값을 읽을 수 없습니다.");
			}

			DataTable dt = (DataTable)cbo.Tag;
			if (dt.Rows.Count == 0) return;

			int ColIndex = -1;
			string ColName = "";
			try
			{
				ColIndex = Convert.ToInt32(Column);
			}
			catch (Exception)
			{
				ColName = Column.ToString();
			}

			if (ColIndex != -1)
			{
				for (int rw = 0, rw2 = dt.Rows.Count; rw < rw2; rw++)
				{
					if (dt.Rows[rw][ColIndex].ToString() == Value.ToString())
					{
						cbo.SelectedIndex = rw;
						return;
					}
				}
			}
			else
			{
				for (int rw = 0, rw2 = dt.Rows.Count; rw < rw2; rw++)
				{
					if (dt.Rows[rw][ColName].ToString() == Value.ToString())
					{
						cbo.SelectedIndex = rw;
						return;
					}
				}
			}
		}

		private static DataTable GetDataTableByValueList()
		{
			DataTable dt = new DataTable();
			for (int i = 0, i2 = mColumnCount; i < i2; i++)
			{
				dt.Columns.Add("Field" + i.ToString(), Type.GetType("System.String"));
			}
			
			string[] aValueList = mRowSource.Split(';');

			int rw = -1;
			while ((rw + 1) < aValueList.Length)
			{
				object[] obj = new object[mColumnCount];
				for (int cl = 0, cl2 = mColumnCount; cl < cl2; cl++)
				{
					rw++;
					obj[cl] = aValueList[rw];
				}
				dt.Rows.Add(obj);
			}

			return dt;
		}

		private static bool[] GetIsLeftAlign(DataTable dt)
		{
			bool[] aIsLeftAlign = new bool[dt.Columns.Count];
			for (int i = 0, i2 = dt.Columns.Count; i < i2; i++)
			{
				aIsLeftAlign[i] = CType.GetIsLeftAlignByTypeName(dt.Columns[i].DataType);
			}

			return aIsLeftAlign;
		}

		private static int[] GetColumnWidth()
		{
			int[] aColWidth = new int[mColumnCount];
			for (int cl = 0, cl2 = mColumnCount; cl < cl2; cl++)
			{
				bool IsAutoSize = (mColumnWidth[cl] == "*") || (cl >= mColumnWidth.Length);

				if (!IsAutoSize)
				{
					aColWidth[cl] = Convert.ToInt32(mColumnWidth[cl]);
				}
				else
				{
					//원래는 모든 데이터에 대해 루핑을 돌면서 가장 긴 문자열을 찾아야 하나
					//속도 문제로 기본 크기로 설정함.
					aColWidth[cl] = drDefaultColumnWidth;
				}
			}

			return aColWidth;
		}

		public string ColumnWidth
		{
			get
			{
				string ColWidthList = "";
				for (int i = 0, i2 = mColumnWidth.Length; i < i2; i++)
				{
					ColWidthList += ";" + mColumnWidth[i].ToString();
				}
				ColWidthList = ColWidthList.Substring(1);

				return ColWidthList;
			}
		}

		public int ColumnCount
		{
			get{return mColumnCount;}
		}

		public static object Column(ComboBox cbo, object Column)
		{
			if (cbo.Items.Count == 0)
			{
				return null;
			}

			if (cbo.Tag == null)
			{
				throw new Exception("Tag 속성이 없으므로 값을 읽을 수 없습니다.");
			}

			int RowIndex = cbo.SelectedIndex;
			if (RowIndex == -1) return null;

			DataTable dt = (DataTable)cbo.Tag;
			if (dt.Rows.Count == 0) return null;

			int ColIndex = -1;
			string ColName = "";
			try
			{
				ColIndex = Convert.ToInt32(Column);
			}
			catch (Exception)
			{
				ColName = Column.ToString();
			}

			if (ColIndex != -1)
				return dt.Rows[RowIndex][ColIndex];
			else
				return dt.Rows[RowIndex][ColName];
		}
		public static string ColumnString(ComboBox cbo, object Column)
		{
			object Value = CRowSource.Column(cbo, Column);
			if (Value == null)
			{
				return "";
			}
			else
			{
				return Value.ToString();
			}
		}
		public static int ColumnInt32(ComboBox cbo, object Column)
		{
			return CFindRep.IfNotNumberThen0(CRowSource.Column(cbo, Column));
		}
	}

	public enum SourceType
	{
		TableQuery, ValueList
	}
}
