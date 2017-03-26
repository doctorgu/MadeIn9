using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.OracleClient;
using System.IO;
using System.Text;

namespace DoctorGu
{
	/// <summary>
	/// 데이터베이스와 관련된 SQL 명령어들을 단순하게 실행할 수 있도록 함.
	/// </summary>
	/// <example>
	/// 다음은 테이블의 생성, 이름 변경, 테이블 존재 여부 확인, 테이블 삭제를 순서대로 실행합니다.
	/// <code>
	/// SqlConnection conn = new SqlConnection("Data Source=localhost;Initial Catalog=Common;Persist Security Info=True;User ID=id;Password=password");
	/// conn.Open();
	/// 
	/// string TableName = "MemberForTest";
	/// 
	/// //테이블 생성
	/// CDB.CreateTable(conn, TableName,
	///	 new SqlColumnInfo[]{
	///		 new SqlColumnInfo("Name", SqlDbType.NVarChar, 50),
	///		 new SqlColumnInfo("Age", SqlDbType.Int),
	///		 new SqlColumnInfo("InsertDateTime", SqlDbType.DateTime)
	///	 });
	/// 
	/// //테이블 이름 변경
	/// string TableNameNew = "MemberForTestNew";
	/// CDB.RenameTable(conn, TableName, TableNameNew);
	/// 
	/// //테이블 존재 여부 확인
	/// if (CDB.IsTableExist(conn, TableNameNew))
	/// {
	///	 //테이블 삭제
	///	 CDB.DropTable(conn, TableNameNew);
	/// }
	/// </code>
	/// </example>
	public class CDB
	{
		/// <summary>
		/// 특정 테이블이 존재하는 지 여부를 리턴함.
		/// </summary>
		/// <param name="conn">Connection 개체</param>
		/// <param name="TableName">테이블 이름</param>
		/// <returns>테이블의 존재 여부</returns>
		public static bool IsTableExist(SqlConnection conn, string TableName)
		{
			string Sql = "select 1 from dbo.sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
			DataTable dt = CStoredProc.ExecDataTableSql(conn, Sql);
			return (dt.Rows.Count != 0);
		}

		/// <summary>
		/// 특정 테이블을 삭제함.
		/// </summary>
		/// <param name="conn">Connection 개체</param>
		/// <param name="TableName">테이블 이름</param>
		public static void DropTable(SqlConnection conn, string TableName)
		{
			string Sql = "drop table [" + TableName + "]";
			CStoredProc.ExecUpdateSql(conn, Sql);
		}
		/// <summary>
		/// 특정 테이블의 이름을 변경함.
		/// </summary>
		/// <param name="conn">Connection 개체</param>
		/// <param name="TableNameOld">현재 테이블 이름</param>
		/// <param name="TableNameNew">변경될 테이블 이름</param>
		public static void RenameTable(SqlConnection conn, string TableNameOld, string TableNameNew)
		{
			string Sql = "sp_rename '[" + TableNameOld.Replace("[", "[[").Replace("]", "]]") + "]', '" + TableNameNew + "'";
			CStoredProc.ExecUpdateSql(conn, Sql);
		}
		/// <summary>
		/// 특정 테이블을 만듦.
		/// </summary>
		/// <param name="conn">Connection 개체</param>
		/// <param name="TableName">만들어질 테이블 이름</param>
		/// <param name="aSqlColumnInfo">만들어질 테이블의 열 정보</param>
		public static void CreateTable(SqlConnection conn, string TableName, SqlColumnInfo[] aSqlColumnInfo)
		{
			string Sql = "create table [" + TableName + "] (\r\n";
			string SqlBody = "";
			string DataTypeCur = "";
			for (int cl = 0, cl2 = aSqlColumnInfo.Length; cl < cl2; cl++)
			{
				DataTypeCur = aSqlColumnInfo[cl].DataType.ToString();
				if (aSqlColumnInfo[cl].Length != 0)
				{
					DataTypeCur += "(" + aSqlColumnInfo[cl].Length + ")";
				}

				SqlBody += ",\r\n[" + aSqlColumnInfo[cl].Name + "] " + DataTypeCur;
			}
			SqlBody = SqlBody.Substring(3);
			Sql = Sql + SqlBody + ")";

			CStoredProc.ExecUpdateSql(conn, Sql);
		}
	}

	/// <summary>
	/// 테이블의 열에 대한 상세 정보.
	/// CreateTable 함수를 호출할 때 쓰임.
	/// </summary>
	public class SqlColumnInfo
	{
		private string mName;
		private SqlDbType mDataType;
		private int mLength;
 
		/// <summary>
		/// 열에 대한 정보를 지정함.
		/// </summary>
		/// <param name="Name">열 이름</param>
		/// <param name="DataType">데이터 형식</param>
		public SqlColumnInfo(string Name, SqlDbType DataType)
		{
			this.mName = Name;
			this.mDataType = DataType;
		}
		/// <summary>
		/// 열에 대한 정보를 지정함.
		/// </summary>
		/// <param name="Name">열 이름</param>
		/// <param name="DataType">데이터 형식</param>
		/// <param name="Length">데이터 길이</param>
		public SqlColumnInfo(string Name, SqlDbType DataType, int Length)
		{
			this.mName = Name;
			this.mDataType = DataType;
			this.mLength = Length;
		}

		/// <summary>
		/// 열 이름을 리턴함.
		/// </summary>
		public string Name
		{
			get { return mName; }
		}
		/// <summary>
		/// 데이터 형식을 리턴함.
		/// </summary>
		public SqlDbType DataType
		{
			get { return mDataType; }
		}
		/// <summary>
		/// 데이터 길이를 리턴함.
		/// </summary>
		public int Length
		{
			get { return mLength; }
		}
	}
}