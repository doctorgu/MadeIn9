using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DoctorGu
{
	public class CDataTableClientOption
	{
		/// <summary>클라이언트 코드에서 변수를 이미 선언한 경우에는 생성하는 스크립트에서 다시 변수를 선언할 필요 없으므로 이 값을 true로 설정해야 함.</summary>
		public bool DontDeclareVariable;

		public string VariableName;
		public bool ColumnNameToLower;
		public bool AddScriptTag;
	}

	public class CDataTableClient
	{
		private CDataTableClientOption _Option;
		private List<DataTable> _aDt = new List<DataTable>();
		private List<DataSet> _aDs = new List<DataSet>();

		public CDataTableClient(DataTable dt, CDataTableClientOption Option)
		{
			_aDt.Add(dt);
			_Option = Option;
		}
		public CDataTableClient(IEnumerable<DataTable> aDt, CDataTableClientOption Option)
		{
			_aDt.AddRange(aDt);
			_Option = Option;
		}
		public CDataTableClient(DataSet ds, CDataTableClientOption Option)
		{
			_aDs.Add(ds);
			_Option = Option;
		}
		public CDataTableClient(IEnumerable<DataSet> aDs, CDataTableClientOption Option)
		{
			_aDs.AddRange(aDs);
			_Option = Option;
		}

		/// <summary>
		/// 서버의 DataTable을 클라이언트의 자바스크립트에서 쓸 수 있도록 변경함.
		/// </summary>
		/// <remarks>오라클의 경우는 필드 이름이 전부 대문자로 리턴되므로 소문자로 변경하려면 ColumnNameToLower 인수를 true로 할 것.</remarks>
		/// <returns></returns>
		public string ToClientScript()
		{
			List<string> aStmt = new List<string>();

			if (_aDt.Count > 0)
			{
				if (!_Option.DontDeclareVariable)
				{
					if (_aDt.Count == 1)
						aStmt.Add("var " + _Option.VariableName + " = new CDataTable();");
					else
						aStmt.Add("var " + _Option.VariableName + " = [];");
				}

				for (int nTbl = 0; nTbl < _aDt.Count; nTbl++)
				{
					DataTable dt = _aDt[nTbl];

					string VarNameCur = (_aDt.Count != 1) ? _Option.VariableName + "[" + nTbl + "]" : _Option.VariableName;
					aStmt = GetClientDataTableCommon(aStmt, dt, VarNameCur, _Option.ColumnNameToLower);
				}
			}

			if (_aDs.Count > 0)
			{
				/*
var ads = [];

ads[0] = { Tables: [] };
ads[0].Tables[0] = new CDataTable();
ads[0].Tables[1] = new CDataTable();

ads[1] = { Tables: [] };
ads[1].Tables[0] = new CDataTable();
ads[1].Tables[1] = new CDataTable();


var ds = { Tables: [] };
ds.Tables[0] = new CDataTable();
ds.Tables[1] = new CDataTable();

*/
				if (!_Option.DontDeclareVariable)
				{
					if (_aDs.Count == 1)
						aStmt.Add("var " + _Option.VariableName + " = { Tables: [] };");
					else
						aStmt.Add("var " + _Option.VariableName + " = [];");
				}

				for (int nDs = 0; nDs < _aDs.Count; nDs++)
				{
					DataSet ds = _aDs[nDs];

					if (_aDs.Count != 1)
						aStmt.Add(_Option.VariableName + "[" + nDs + "] = { Tables: [] };");

					string VarNameCur =
						(_aDs.Count != 1)
						? _Option.VariableName + "[" + nDs + "].Tables"
						: _Option.VariableName + ".Tables";

					for (int nTbl = 0; nTbl < ds.Tables.Count; nTbl++)
					{
						DataTable dt = ds.Tables[nTbl];

						string VarNameCur2 = VarNameCur + "[" + nTbl + "]";
						aStmt = GetClientDataTableCommon(aStmt, dt, VarNameCur2, _Option.ColumnNameToLower);
					}
				}
			}


			string s = CScript.GetScript(aStmt, _Option.AddScriptTag);
			return s;
		}
		private List<string> GetClientDataTableCommon(List<string> aStmt, DataTable dt, string VarName, bool ColumnNameToLower)
		{
			aStmt.Add(VarName + " = new CDataTable();");

			string[] aColName = CDataTableClient.GetColumnNameInArray(dt, ColumnNameToLower);

			string ColumnNameList = "";
			for (int cl = 0; cl < aColName.Length; cl++)
			{
				ColumnNameList += ", \"" + aColName[cl] + "\"";
			}
			ColumnNameList = ColumnNameList.Substring(2);
			aStmt.Add(VarName + ".Columns = [ " + ColumnNameList + " ];");

			string RowList = CDataTableClient.GetClientDataRow(dt, aColName);
			aStmt.Add(VarName + ".Rows = " + RowList + ";");

			return aStmt;
		}

		public static string GetClientDataRow(DataTable dt, string[] aColName)
		{
			List<string> aRow = new List<string>();
			for (int rw = 0, rw2 = dt.Rows.Count; rw < rw2; rw++)
			{
				DataRow dr = dt.Rows[rw];

				string[] aNameValue = new string[dt.Columns.Count];
				for (int cl = 0, cl2 = dt.Columns.Count; cl < cl2; cl++)
				{
					object oValue = dr[cl];

					string Text = CScript.GetValueForJson(oValue);

					//JSON 형식을 리턴할 때 Name에 큰 따옴표를 붙여야만 에러를 내지 않음.
					aNameValue[cl] = "\"" + aColName[cl] + "\":" + Text;
				}
				string NameValueList = string.Join(", ", aNameValue);
				aRow.Add("{ " + NameValueList + " }");
			}

			string RowList = "[ " + string.Join(",\r\n", aRow.ToArray()) + " ]";
			return RowList;
		}
		public static string GetClientDataRow(DataTable dt)
		{
			string[] aColName = GetColumnNameInArray(dt);
			return GetClientDataRow(dt, aColName);
		}

		private static string[] GetColumnNameInArray(DataTable dt, bool ColumnNameToLower)
		{
			string[] aColName = new string[dt.Columns.Count];
			for (int cl = 0, cl2 = dt.Columns.Count; cl < cl2; cl++)
			{
				string ColumnName = dt.Columns[cl].ColumnName;
				if (ColumnNameToLower)
				{
					//오라클의 경우 필드 이름을 전부 대문자로 리턴하므로
					ColumnName = ColumnName.ToLower();
				}

				aColName[cl] = ColumnName;
			}

			return aColName;
		}
		private static string[] GetColumnNameInArray(DataTable dt)
		{
			bool ColumnNameToLower = false;
			return GetColumnNameInArray(dt, ColumnNameToLower);
		}
	}
}
