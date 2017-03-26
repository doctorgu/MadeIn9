using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Xml;
using System.Text;
using System.IO;
using System.Data.Common;
using System.Linq;


namespace DoctorGu
{
	/// <summary>
	/// Stored procedure를 호출하기 위한 클래스
	/// </summary>
	/// <example>
	/// 다음은 이미지 파일을 DB에 저장하고 불러오는 과정임.
	/// 
	/// create table ImageTest (
	/// 	Id int identity(1, 1) not null,
	/// 	Face image not null
	/// )
	/// GO
	/// 
	/// create procedure [GetImage]
	/// 	@Id int
	/// as
	/// 	select	Face
	/// 	from	ImageTest
	/// 	where	Id = @Id
	/// GO
	/// 
	/// create procedure [InsertImage]
	/// 	@Face image
	/// as
	/// 	insert into ImageTest
	/// 		(Face)
	/// 	values	(@Face)
	/// 
	/// 	return @@identity
	/// GO
	/// 
	/// SqlConnection conn = new SqlConnection("Data Source = DOCTORGU; Initial Catalog = Northwind; Uid=sa;Pwd=");
	/// conn.Open();
	/// //DB에 저장
	/// byte[] aFile = CFile.GetByteFromFile(@"C:\Test.bmp");
	/// int Id = CStoredProc.ExecUpdate(conn, "InsertImage", new SqlParameter("@Face", aFile));
	/// 
	/// //DB에서 가져오기
	/// DataTable dt = CStoredProc.ExecDataSet(conn, "GetImage", new SqlParameter("@Id", Id)).Tables[0];
	/// byte[] aFile2 = (byte[])dt.Rows[0]["Face"];
	/// if (aFile2.Length != 0)
	/// {
	/// 	BinaryWriter bw = new BinaryWriter(new FileStream(@"C:\Test2.bmp", FileMode.CreateNew, FileAccess.Write, FileShare.None));
	/// 	bw.Write(aFile2, 0, aFile2.Length);
	/// }
	/// </example>
	public class CStoredProc
	{
		#region Common
		/// <summary>
		/// Output 파라미터의 값이 여러 개인 경우, 이름으로 파라미터 값을 가져올 수 있도록
		/// 파라미터 이름을 Key로, 파라미터 값을 Value로 하는 Dictionary를 리턴함.
		/// </summary>
		/// <param name="aParam">DbParameter 배열</param>
		/// <returns>파라미터 이름을 Key로, 파라미터 값을 Value로 하는 Dictionary</returns>
		public static Dictionary<string, object> DbParameterArrayToDictionary(DbParameter[] aParam)
		{
			Dictionary<string, object> dic = new Dictionary<string, object>();
			foreach (DbParameter Param in aParam)
			{
				dic.Add(Param.ParameterName, Param.Value);
			}

			return dic;
		}

		public static DbParameter GetParameterByName(DbParameter[] aParam, string ParamName)
		{
			foreach (DbParameter Param in aParam)
			{
				string NameCur = Param.ParameterName;

				if (string.Compare(NameCur, ParamName, true) == 0)
					return Param;
			}

			return null;
		}

		/// <summary>
		/// ParameterName, Value가 같은 지 비교.(개수, DbType이 틀린 경우는 체크하지 않음.)
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// SqlParameter[] aParam1 =
		///     new SqlParameter[]
		///             {
		///                 new SqlParameter("@a", "aaa"),
		///                 new SqlParameter("@b", 123),
		///                 new SqlParameter("@c", new DateTime(2012, 12, 31, 23, 59, 59)),
		///                 new SqlParameter("@d", DBNull.Value)
		///             };
		/// SqlParameter[] aParam2 =
		///     new SqlParameter[]
		///             {
		///                 new SqlParameter("@a", "aaa"),
		///                 new SqlParameter("@b", 123),
		///                 new SqlParameter("@c", new DateTime(2012, 12, 31, 23, 59, 59)),
		///                 new SqlParameter("@d", DBNull.Value)
		///             };
		/// bool b = CStoredProc.Equals(aParam1, aParam2);
		/// ]]>
		/// </example>
		public static bool Equals(IEnumerable<DbParameter> aParam1, IEnumerable<DbParameter> aParam2)
		{
			foreach (DbParameter Param1 in aParam1)
			{
				if (aParam2.FirstOrDefault(p =>
					(p.ParameterName == Param1.ParameterName)
					&&
					((p.Value == null) ? (Param1.Value == null) : p.Value.Equals(Param1.Value))
					) == null)

					return false;
			}

			return true;
		}
		#endregion Common

		#region SQLServer
		/// <summary>
		/// 데이터를 가져오기 위함.
		/// </summary>
		/// <param name="SpName">SP 이름</param>
		/// <param name="SqlParam">SP의 파라미터</param>
		/// <returns>선택한 데이터의 내용</returns>
		/// <example>
		/// SqlConnection conn = new SqlConnection("Data Source = DOCTORGU; Initial Catalog = Shop; Uid=sa;Pwd=wlrvks2002");
		/// conn.Open();
		/// SqlParameter[] aSqlParam = new SqlParameter[]{new SqlParameter("@MemberID", "doctorgu")};
		/// SqlDataReader sdr = CStoredProc.ExecReader(conn, "MSGgetFaxHistoryByMemberID", aSqlParam);
		/// while (sdr.Read())
		/// {
		/// 	Console.WriteLine(sdr.GetString(0));
		/// }
		/// </example>
		public static SqlDataReader ExecReader(SqlConnection conn, string SpName, SqlParameter[] aParamIn)
		{
			SqlCommand cmd = new SqlCommand();
			cmd.Connection = conn;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SpName;

			for (int i = 0; i < aParamIn.Length; i++)
			{
				cmd.Parameters.Add(aParamIn[i]);
			}

			return cmd.ExecuteReader();
		}
		public static SqlDataReader ExecReader(SqlConnection conn, string SpName, SqlParameter ParamIn)
		{
			return ExecReader(conn, SpName, new SqlParameter[] { ParamIn });
		}
		public static SqlDataReader ExecReader(SqlConnection conn, string SpName)
		{
			return ExecReader(conn, SpName, new SqlParameter[] { });
		}

		/// <summary>
		/// 데이터를 가져오기 위함.
		/// </summary>
		/// <param name="SpName">SP 이름</param>
		/// <param name="SqlParam">SP의 파라미터</param>
		/// <returns>선택한 데이터의 내용</returns>
		/// <example>
		/// SqlConnection conn = new SqlConnection("Data Source = DOCTORGU; Initial Catalog = Logics; Uid=sa;Pwd=wlrvks2002");
		/// conn.Open();
		/// SqlParameter[] aSqlParam = new SqlParameter[]{new SqlParameter("@GoodVerify", 1)};
		/// DataSet ds = CStoredProc.ExecDataSet(conn, "MDgetDefaultInfo", aSqlParam);
		/// DataTable dt = ds.Tables[0];
		/// for (int i = 0, j = dt.Rows.Count; i < j; i++)
		/// {
		/// 	DataRow C = dt.Rows[i];
		/// 	Console.WriteLine("To: {0}, Fax: {1}", C["To"].ToString(), C["Fax"].ToString());
		/// }
		/// Console.ReadLine();
		/// </example>
		public static DataSet ExecDataSet(SqlConnection conn, string SpName,
			SqlParameter[] aParamIn, ref SqlParameter[] aParamOut, SqlTransaction tran)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				if (tran != null)
					cmd.Transaction = tran;

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = SpName;

				if (aParamIn != null)
				{
					for (int i = 0; i < aParamIn.Length; i++)
					{
						cmd.Parameters.Add(aParamIn[i]);
					}
				}

				if (aParamOut != null)
				{
					for (int i = 0; i < aParamOut.Length; i++)
					{
						aParamOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(aParamOut[i]);
					}
				}

				SqlParameter ParamReturn = new SqlParameter("ReturnValue", SqlDbType.Int);
				ParamReturn.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add(ParamReturn);

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);

				//int nReturn = Convert.ToInt32(ParamReturn.Value);

				return ds;
			}
		}
		public static DataSet ExecDataSet(SqlConnection conn, string SpName, SqlParameter[] aParamIn, ref SqlParameter[] aParamOut)
		{
			SqlTransaction tran = null;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataSet ExecDataSet(SqlConnection conn, string SpName, SqlParameter[] aParamIn)
		{
			SqlParameter[] aParamOut = null;
			SqlTransaction tran = null;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataSet ExecDataSet(SqlConnection conn, string SpName, SqlParameter ParamIn)
		{
			SqlParameter[] aParamIn = new SqlParameter[] { ParamIn };
			SqlParameter[] aParamOut = null;
			SqlTransaction tran = null;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataSet ExecDataSet(SqlConnection conn, string SpName)
		{
			SqlParameter[] aParamIn = new SqlParameter[] { };
			SqlParameter[] aParamOut = null;
			SqlTransaction tran = null;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}

		public static DataTable ExecDataTable(SqlConnection conn, string SpName,
			SqlParameter[] aParamIn, ref SqlParameter[] aParamOut,
			SqlTransaction tran)
		{
			DataSet ds = ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
			if ((ds == null) || (ds.Tables.Count == 0))
				return null;

			return ds.Tables[0];
		}
		public static DataTable ExecDataTable(SqlConnection conn, string SpName,
			SqlParameter[] aParamIn, ref SqlParameter[] aParamOut)
		{
			SqlTransaction tran = null;
			return ExecDataTable(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataTable ExecDataTable(SqlConnection conn, string SpName, SqlParameter[] aParamIn)
		{
			SqlParameter[] aParamOut = null;
			SqlTransaction tran = null;
			return ExecDataTable(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataTable ExecDataTable(SqlConnection conn, string SpName, SqlParameter ParamIn)
		{
			SqlParameter[] aParamIn = new SqlParameter[] { ParamIn };
			SqlParameter[] aParamOut = null;
			SqlTransaction tran = null;
			return ExecDataTable(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataTable ExecDataTable(SqlConnection conn, string SpName)
		{
			SqlParameter[] aParamIn = new SqlParameter[] { };
			SqlParameter[] aParamOut = null;
			SqlTransaction tran = null;
			return ExecDataTable(conn, SpName, aParamIn, ref aParamOut, tran);
		}

		/// <summary>
		/// 데이터를 추가, 수정, 삭제하기 위함.
		/// </summary>
		/// <param name="SpName">SP 이름</param>
		/// <param name="aParamIn">SP의 파라미터의 이름과 값 정보</param>
		/// <param name="aParamOut">SP의 output 형식 파라미터의 이름과 값 정보</param>
		/// <param name="NoInternalTransaction">트랜잭션을 사용하지 않음. DB 백업시엔 트랜잭션을 쓰지 않아야 하므로 true로 설정함.</param>
		/// <returns></returns>
		/// <example>
		/// SqlConnection conn = new SqlConnection("Data Source = DOCTORGU; Initial Catalog = Shop; Uid=sa;Pwd=wlrvks2002");
		/// conn.Open();
		/// CStoredProc sp = new CStoredProc(conn);
		/// SqlParameter[] aSqlParamIn = new SqlParameter[]{new SqlParameter("@SeriesCD",  1),
		/// 												   new SqlParameter("@SeriesName", "시리즈명"),
		/// 												   new SqlParameter("@SeriesISBN", "")
		/// 											   };
		/// int nReturn = sp.ExecUpdate("OPupdateSeries", aSqlParamIn);
		/// Console.WriteLine(nReturn.ToString());
		/// </example>
		public static int ExecUpdate(SqlConnection conn, string SpName,
			SqlParameter[] aParamIn, ref SqlParameter[] aParamOut, SqlTransaction tran)
		{
			//도중에 에러가 나면 트랜잭션이 중첩되는 경우가 있어 확실하게 트랜잭션을 종료함.
			//-> 코드 중복 막기 위해 안씀.
			//using (SqlTransaction tran = conn.BeginTransaction()) 

			int nReturn = 0;

			using (SqlCommand cmd = new SqlCommand())
			{
				if (tran != null)
					cmd.Transaction = tran;

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = SpName;

				if (aParamIn != null)
				{
					for (int i = 0; i < aParamIn.Length; i++)
					{
						cmd.Parameters.Add(aParamIn[i]);
					}
				}

				if (aParamOut != null)
				{
					for (int i = 0; i < aParamOut.Length; i++)
					{
						aParamOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(aParamOut[i]);
					}
				}

				SqlParameter ParamReturn = new SqlParameter("ReturnValue", SqlDbType.Int);
				ParamReturn.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add(ParamReturn);

				cmd.ExecuteNonQuery();

				nReturn = Convert.ToInt32(ParamReturn.Value);
			}

			return nReturn;
		}
		public static int ExecUpdate(SqlConnection conn, string SpName, SqlParameter[] aParamIn, ref SqlParameter[] aParamOut)
		{
			using (SqlTransaction tran = conn.BeginTransaction())
			{
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}
		public static int ExecUpdate(SqlConnection conn, string SpName, SqlParameter[] aParamIn)
		{
			using (SqlTransaction tran = conn.BeginTransaction())
			{
				SqlParameter[] aParamOut = null;
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}
		public static int ExecUpdate(SqlConnection conn, string SpName, SqlParameter ParamIn)
		{
			using (SqlTransaction tran = conn.BeginTransaction())
			{
				SqlParameter[] aParamIn = new SqlParameter[] { ParamIn };
				SqlParameter[] aParamOut = null;
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}
		public static int ExecUpdate(SqlConnection conn, string SpName)
		{
			using (SqlTransaction tran = conn.BeginTransaction())
			{
				SqlParameter[] aParamIn = null;
				SqlParameter[] aParamOut = null;
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}

		public static DataSet ExecDataSetSql(SqlConnection conn, string Sql)
		{
			using (SqlCommand cmd = new SqlCommand(Sql, conn))
			{
				cmd.CommandType = CommandType.Text;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);

				return ds;
			}
		}
		public static DataTable ExecDataTableSql(SqlConnection conn, string Sql)
		{
			return ExecDataSetSql(conn, Sql).Tables[0];
		}

		public static int ExecUpdateSql(SqlConnection conn, string Sql, SqlTransaction tran)
		{
			using (SqlCommand cmd = new SqlCommand())
			{
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = Sql;
				if (tran != null)
					cmd.Transaction = tran;
				return cmd.ExecuteNonQuery();
			}
		}
		public static int ExecUpdateSql(SqlConnection conn, string Sql)
		{
			return ExecUpdateSql(conn, Sql, null);
		}

		/// <summary>
		/// "SPName @Param1 = 'test', @Param2 = 123"과 같은 형식의 문자열을 만들어 리턴함.
		/// 이 문자열을 복사해서 쿼리 입력기에서 직접 실행하기 위함.
		/// </summary>
		/// <param name="cmd">SqlCommand 개체</param>
		/// <returns>프로시저를 실행하는 문자열</returns>
		public static string GetSpTextForDebug(SqlCommand cmd)
		{
			string ReturnName = "";
			string ParamList = "";
			foreach (SqlParameter Param in cmd.Parameters)
			{
				string Quot = "";
				switch (Param.SqlDbType)
				{
					case SqlDbType.DateTime:
					case SqlDbType.SmallDateTime:
					case SqlDbType.Timestamp:
					case SqlDbType.Binary:
					case SqlDbType.Image:
					case SqlDbType.NChar:
					case SqlDbType.NText:
					case SqlDbType.Text:
					case SqlDbType.VarBinary:
					case SqlDbType.Char:
					case SqlDbType.NVarChar:
					case SqlDbType.VarChar:
					case SqlDbType.Xml:
						Quot = "'";
						break;
				}

				string Dir = "";
				switch (Param.Direction)
				{
					case ParameterDirection.Input:
						Dir = "";
						break;
					case ParameterDirection.InputOutput:
					case ParameterDirection.Output:
						Dir = " output";
						break;
					case ParameterDirection.ReturnValue:
						ReturnName = Param.ParameterName;
						continue;
				}

				string Value = Param.Value.ToString();
				if (Quot != "")
					Value = Value.Replace(Quot, Quot + Quot);

				ParamList += ", " + Quot + Value + Quot + Dir;
			}
			if (ParamList != "")
			{
				ParamList = ParamList.Substring(2);
			}

			string Stmt = "";
			if (ReturnName != "")
			{
				Stmt += ReturnName + " = ";
			}
			Stmt += cmd.CommandText + " " + ParamList;

			return Stmt;
		}

		public static string GetSpTextForDebug(string SpName, SqlParameter[] aSqlParam)
		{
			return GetSpTextForDebug(SpName, aSqlParam, null);
		}
		public static string GetSpTextForDebug(string SpName, SqlParameter[] aParamIn, SqlParameter[] aParamOut)
		{
			string Declare = "";
			string Params = "";

			if (aParamIn != null)
			{
				foreach (SqlParameter p in aParamIn)
				{
					Params += "\r\n\t, " + GetParamValueForDebug(p);
				}
			}

			if (aParamOut != null)
			{
				foreach (SqlParameter p in aParamOut)
				{
					p.Direction = ParameterDirection.Output;

					Declare += GetOutputParamDeclareForDebug(p) + "\r\n";
					Params += "\r\n\t, " + GetParamValueForDebug(p);
				}
			}

			if (Params.Length > 0)
				Params = Params.Substring("\r\n\t, ".Length);

			return Declare
				+ "exec " + SpName + "\r\n\t" + Params;
		}

		private static string GetOutputParamDeclareForDebug(SqlParameter p)
		{
			string s = "";

			if ((p.Direction != ParameterDirection.InputOutput)
				&& (p.Direction != ParameterDirection.Output))
				throw new Exception(string.Format("Wrong Direction: {0}", p.Direction));

			s += "declare " + p.ParameterName + " " + p.SqlDbType;
			if (CLang.In(p.SqlDbType, SqlDbType.Char, SqlDbType.NChar, SqlDbType.VarChar, SqlDbType.NVarChar))
			{
				string Size = (p.Size == -1) ? "max" : p.Size.ToString();
				s += "(" + Size + ")";
			}

			return s;
		}

		private static string GetParamValueForDebug(SqlParameter p)
		{
			string s = "";

			if ((p.Direction == ParameterDirection.InputOutput)
				|| (p.Direction == ParameterDirection.Output))
			{
				return p.ParameterName + " = " + p.ParameterName + " output";
			}

			if (p.Value == null)
				return "";

			s = p.ParameterName + " = " + GetParamValueForDebug(p.Value);

			return s;
		}
		private static string GetParamValueForDebug(OracleParameter p)
		{
			if (p.Value == null)
				return "null";


			string s = GetParamValueForDebug(p.Value);

			return s;
		}
		private static string GetParamValueForDebug(object Value)
		{
			string s = "";

			Type t = Value.GetType();
			if (Value == DBNull.Value)
			{
				s = "null";
			}
			else if ((t == typeof(System.String))
				|| (t == typeof(System.Char)))
			{
				s = "'" + Value.ToString().Replace("'", "''") + "'";
			}
			else if (t == typeof(System.DateTime))
			{
				s = "'" + ((DateTime)Value).ToString("yyyy-MM-dd HH:mm:ss") + "'";
			}
			else if (t == typeof(System.Boolean))
			{
				s = ((((bool)Value) == true) ? "1" : "0");
			}
			else
			{
				s = Value.ToString();
			}

			return s;
		}

		public static string SqlParameterArrayToXml(SqlParameter[] aParam)
		{
			//<?xml version=\"1.0\" encoding=\"utf-8\"?>가 포함되면 Insert 시 에러 나므로 주석.
			using (MemoryStream ms = new MemoryStream())
			{
				using (XmlTextWriter xw = new XmlTextWriter(ms, Encoding.UTF8))
				{
					xw.Formatting = Formatting.Indented;
					xw.IndentChar = '\t';

					xw.WriteStartDocument();
					xw.WriteStartElement("rows");

					foreach (SqlParameter Param in aParam)
					{
						xw.WriteStartElement("row");
						xw.WriteAttributeString("ParameterName", Param.ParameterName);
						xw.WriteAttributeString("SqlDbType", Param.SqlDbType.ToString());
						xw.WriteAttributeString("Size", Param.Size.ToString());
						xw.WriteAttributeString("Value", Param.Value.ToString());
						xw.WriteEndElement();
					}

					xw.WriteEndElement();
					xw.WriteEndDocument();

					xw.Flush();

					ms.Position = 0;
					StreamReader sr = new StreamReader(ms, Encoding.UTF8);
					string Xml = sr.ReadToEnd();
					return Xml;
				}
			}
		}
		public static SqlParameter[] XmlToSqlParameterArray(string Xml)
		{
			List<SqlParameter> aParam = new List<SqlParameter>();

			using (XmlReader xr = XmlReader.Create(new StringReader(Xml)))
			{
				while (xr.Read())
				{
					if (xr.NodeType != XmlNodeType.Element)
						continue;

					if (xr.Name != "row")
						continue;


					SqlParameter Param = new SqlParameter();

					while (xr.MoveToNextAttribute())
					{
						switch (xr.Name)
						{
							case "ParameterName":
								Param.ParameterName = xr.Value;
								break;
							case "SqlDbType":
								Param.SqlDbType = (SqlDbType)CReflection.GetFieldOrPropertyValue(Param, "SqlDbType");
								break;
							case "Size":
								Param.Size = (int)CReflection.GetFieldOrPropertyValue(Param, "Size");
								break;
							case "Value":
								Param.Value = (object)CReflection.GetFieldOrPropertyValue(Param, "Value");
								break;
						}
					}

					aParam.Add(Param);
				} //xr.Read
			}

			return aParam.ToArray();
		}

		public static SqlParameter[] DbParameterArrayToSqlParameterArray(DbParameter[] oParam)
		{
			if (oParam == null)
				return null;


			SqlParameter[] aParamNew = new SqlParameter[oParam.Length];

			for (int i = 0; i < oParam.Length; i++)
			{
				aParamNew[i] = (SqlParameter)oParam[i];
			}

			return aParamNew;
		}
		#endregion SQLServer


		#region Oracle
		/// <summary>
		/// 데이터를 가져오기 위함.
		/// </summary>
		/// <param name="SpName">SP 이름</param>
		/// <param name="OracleParam">SP의 파라미터</param>
		/// <returns>선택한 데이터의 내용</returns>
		/// <example>
		/// OracleConnection conn = new OracleConnection("Data Source = DOCTORGU; Initial Catalog = Shop; Uid=sa;Pwd=wlrvks2002");
		/// conn.Open();
		/// OracleParameter[] aParamIn = new OracleParameter[]{new OracleParameter("v_member_id", "doctorgu")};
		/// OracleDataReader odr = CStoredProc.ExecReader(conn, "MSGgetFaxHistoryByMemberID", aParamIn);
		/// while (odr.Read())
		/// {
		/// 	Console.WriteLine(odr.GetString(0));
		/// }
		/// </example>
		public static OracleDataReader ExecReader(OracleConnection conn, string SpName, OracleParameter[] aParamIn)
		{
			OracleCommand cmd = new OracleCommand();
			cmd.Connection = conn;
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = SpName;

			for (int i = 0; i < aParamIn.Length; i++)
			{
				cmd.Parameters.Add(aParamIn[i]);
			}

			return cmd.ExecuteReader();
		}
		public static OracleDataReader ExecReader(OracleConnection conn, string SpName, OracleParameter ParamIn)
		{
			return ExecReader(conn, SpName, new OracleParameter[] { ParamIn });
		}
		public static OracleDataReader ExecReader(OracleConnection conn, string SpName)
		{
			return ExecReader(conn, SpName, new OracleParameter[] { });
		}

		/// <summary>
		/// Call procedure and return DataSet as a result.
		/// </summary>
		/// <param name="conn">OracleConnection object already openned</param>
		/// <param name="SpName">Procedure name</param>
		/// <param name="aParamIn">Input parameter list</param>
		/// <param name="aParamOut">Output parameter list</param>
		/// <param name="tran">OracleTransaction object already started</param>
		/// <returns>DataSet contains one or more DataTables</returns>
		/// <example>
		/// <code>
		/// using (OracleConnection conn = CConn.GetConnection())
		/// {
		///		using (OracleTransaction tran = conn.BeginTransaction())
		///		{
		///		    OracleParameter[] aParamIn = new OracleParameter[]
		///		    {
		///		        new OracleParameter("v_p_id", "test_id")
		///		    };
		///		    OracleParameter[] aParamOut = new OracleParameter[]
		///		    {
		///		        new OracleParameter("cv_1", OracleType.Cursor),
		///		        new OracleParameter("cv_2", OracleType.Cursor)
		///		    };
		///		    DataSet ds = CStoredProc.ExecDataSet(conn, "PKG_MSSPM12002.Q", aParamIn, ref aParamOut, tran);
		///		}
		/// }
		/// </code>
		/// </example>
		public static DataSet ExecDataSet(OracleConnection conn, string SpName, OracleParameter[] aParamIn, ref OracleParameter[] aParamOut, OracleTransaction tran)
		{
			using (OracleCommand cmd = new OracleCommand())
			{
				if (tran != null)
					cmd.Transaction = tran;

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = SpName;

				if (aParamIn != null)
				{
					for (int i = 0; i < aParamIn.Length; i++)
					{
						cmd.Parameters.Add(aParamIn[i]);
					}
				}

				if (aParamOut != null)
				{
					for (int i = 0; i < aParamOut.Length; i++)
					{
						aParamOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(aParamOut[i]);
					}
				}

				OracleDataAdapter da = new OracleDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);

				//OracleDataReader dr = cmd.ExecuteReader();
				//DataTable dt = CDataTable.GetDataTableFromOracleDataReader(dr);

				return ds;
			}
		}
		public static DataSet ExecDataSet(OracleConnection conn, string SpName, OracleParameter[] aParamIn, ref OracleParameter[] aParamOut)
		{
			OracleTransaction tran = null;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataSet ExecDataSet(OracleConnection conn, string SpName, OracleParameter[] aParamIn)
		{
			OracleParameter[] aParamOut = null;
			OracleTransaction tran = null;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataSet ExecDataSet(OracleConnection conn, string SpName, OracleParameter ParamIn)
		{
			OracleParameter[] aParamIn = new OracleParameter[] { ParamIn };
			OracleParameter[] aParamOut = null;
			OracleTransaction tran = null; ;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}
		public static DataSet ExecDataSet(OracleConnection conn, string SpName)
		{
			OracleParameter[] aParamIn = new OracleParameter[] { };
			OracleParameter[] aParamOut = null;
			OracleTransaction tran = null; ;
			return ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
		}

		/// <summary>
		/// Call procedure and return DataTable as a result.
		/// </summary>
		/// <param name="conn">OracleConnection object already openned</param>
		/// <param name="SpName">Procedure name</param>
		/// <param name="aParamIn">Input parameter list</param>
		/// <param name="aParamOut">Output parameter list</param>
		/// <param name="tran">OracleTransaction object already started</param>
		/// <returns>DataTable contains columns and rows information.</returns>
		/// <example>
		/// <code>
		/// using (OracleConnection conn = CConn.GetConnection())
		/// {
		///		using (OracleTransaction tran = conn.BeginTransaction())
		///		{
		///		    OracleParameter[] aParamIn = new OracleParameter[]
		///		    {
		///		        new OracleParameter("v_p_id", "test_id")
		///		    };
		///		    OracleParameter[] aParamOut = new OracleParameter[]
		///		    {
		///		        new OracleParameter("cv_1", OracleType.Cursor),
		///		    };
		///		    DataTable dt = CStoredProc.ExecDataTable(conn, "PKG_MSSPM12002.Q", aParamIn, ref aParamOut, tran);
		///		}
		/// }
		/// </code>
		/// </example>
		public static DataTable ExecDataTable(OracleConnection conn, string SpName, OracleParameter[] aParamIn, ref OracleParameter[] aParamOut, OracleTransaction tran)
		{
			DataSet ds = ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
			if ((ds == null) || (ds.Tables.Count == 0))
				return null;

			return ds.Tables[0];
		}
		public static DataTable ExecDataTable(OracleConnection conn, string SpName, OracleParameter[] aParamIn, ref OracleParameter[] aParamOut)
		{
			OracleTransaction tran = null;
			DataSet ds = ExecDataSet(conn, SpName, aParamIn, ref aParamOut, tran);
			if ((ds == null) || (ds.Tables.Count == 0))
				return null;

			return ds.Tables[0];
		}
		public static DataTable ExecDataTable(OracleConnection conn, string SpName, OracleParameter[] aParamIn)
		{
			OracleParameter[] aParamOut = null;
			return ExecDataTable(conn, SpName, aParamIn, ref aParamOut);
		}
		public static DataTable ExecDataTable(OracleConnection conn, string SpName, OracleParameter ParamIn)
		{
			OracleParameter[] aParamIn = new OracleParameter[] { ParamIn };
			OracleParameter[] aParamOut = null;
			return ExecDataTable(conn, SpName, aParamIn, ref aParamOut);
		}
		public static DataTable ExecDataTable(OracleConnection conn, string SpName)
		{
			OracleParameter[] aParamIn = new OracleParameter[] { };
			OracleParameter[] aParamOut = null;
			return ExecDataTable(conn, SpName, aParamIn, ref aParamOut);
		}

		/// <summary>
		/// Execute raw SQL and return DataSet as a result.
		/// </summary>
		/// <param name="conn">OracleConnection object already openned</param>
		/// <param name="Sql">SQL statement</param>
		/// <returns>DataSet contains one or more DataTables</returns>
		/// <example>
		/// <code>
		/// using (OracleConnection conn = CConn.GetConnection())
		/// {
		///		string Sql = "SELECT 1 as COL FROM dual UNION ALL SELECT 2 as COL FROM";
		///		DataSet ds = CStoredProc.ExecDataSetSql(conn, Sql);
		/// }
		/// </code>
		/// </example>
		public static DataSet ExecDataSetSql(OracleConnection conn, string Sql)
		{
			using (OracleCommand cmd = new OracleCommand(Sql, conn))
			{
				cmd.CommandType = CommandType.Text;

				OracleDataAdapter da = new OracleDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);

				return ds;
			}
		}
		/// <summary>
		/// Execute raw SQL and return DataTable as a result.
		/// </summary>
		/// <param name="conn">OracleConnection object already openned</param>
		/// <param name="Sql">SQL statement</param>
		/// <returns>DataTable contains columns and rows information.</returns>
		/// <example>
		/// <code>
		/// using (OracleConnection conn = CConn.GetConnection())
		/// {
		///		string Sql = "SELECT 1 as COL FROM dual";
		///		DataTable dt = CStoredProc.ExecDataTableSql(conn, Sql);
		/// }
		/// </code>
		/// </example>
		public static DataTable ExecDataTableSql(OracleConnection conn, string Sql)
		{
			return ExecDataSetSql(conn, Sql).Tables[0];
		}

		/// <summary>
		/// Call procedure and return affected record count.
		/// </summary>
		/// <param name="conn">OracleConnection object already openned</param>
		///	<param name="SpName">Procedure name</param>
		///	<param name="aParamIn">Input parameter list</param>
		///	<param name="aParamOut">Output parameter list</param>
		/// <param name="tran">OracleTransaction object already started</param>
		/// <returns>Record count affected</returns>
		/// <example>
		/// <code>
		/// //If P_TEST_2 fails, P_TEST_1 is also invalid because same transaction was used.
		/// using (OracleConnection conn = CConn.GetConnection())
		/// {
		///		using (OracleTransaction tran = conn.BeginTransaction())
		///		{
		///			OracleParameter[] aParamIn = new OracleParameter[]
		///			{
		///				new OracleParameter("v_p_id", "test_id"),
		///				new OracleParameter("v_p_name", "test_name")
		///			};
		///
		///			OracleParameter[] aParamOut = new OracleParameter[]
		///			{
		///				new OracleParameter("v_p_name_out", OracleType.Varchar, 50)
		///			};
		///			int RowsAffected1 = CStoredProc.ExecUpdate(conn, "P_TEST_1", aParamIn, ref aParamOut, tran);
		///
		///			int RowsAffected2 = CStoredProc.ExecUpdateSql(conn, "P_TEST_2", aParamIn, ref aParamOut, tran);
		///			
		///			tran.Commit();
		///		}
		///	}
		/// </code>
		/// </example>
		public static int ExecUpdate(OracleConnection conn, string SpName, OracleParameter[] aParamIn, ref OracleParameter[] aParamOut, OracleTransaction tran)
		{
			int nReturn = 0;

			using (OracleCommand cmd = new OracleCommand())
			{
				if (tran != null)
					cmd.Transaction = tran;

				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = SpName;

				if (aParamIn != null)
				{
					for (int i = 0; i < aParamIn.Length; i++)
					{
						cmd.Parameters.Add(aParamIn[i]);
					}
				}

				if (aParamOut != null)
				{
					for (int i = 0; i < aParamOut.Length; i++)
					{
						aParamOut[i].Direction = ParameterDirection.Output;
						cmd.Parameters.Add(aParamOut[i]);
					}
				}

				/* ReturnValue 형식을 추가하면 파라미터 개수가 틀려져서 프로시저가 없다고 나옴.
				OracleParameter ParamReturn = new OracleParameter("ReturnValue", OracleType.Int32);
				ParamReturn.Direction = ParameterDirection.ReturnValue;
				cmd.Parameters.Add(ParamReturn);
				*/

				int RowsAffected = cmd.ExecuteNonQuery();

				//nReturn = Convert.ToInt32(ParamReturn.Value);
				nReturn = RowsAffected;
			}

			return nReturn;
		}
		public static int ExecUpdate(OracleConnection conn, string SpName, OracleParameter[] aParamIn, ref OracleParameter[] aParamOut)
		{
			using (OracleTransaction tran = conn.BeginTransaction())
			{
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}
		public static int ExecUpdate(OracleConnection conn, string SpName, OracleParameter[] aParamIn)
		{
			using (OracleTransaction tran = conn.BeginTransaction())
			{
				OracleParameter[] aParamOut = null;
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}
		public static int ExecUpdate(OracleConnection conn, string SpName, OracleParameter ParamIn)
		{
			using (OracleTransaction tran = conn.BeginTransaction())
			{
				OracleParameter[] aParamIn = new OracleParameter[] { ParamIn };
				OracleParameter[] aParamOut = null;
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}
		public static int ExecUpdate(OracleConnection conn, string SpName)
		{
			using (OracleTransaction tran = conn.BeginTransaction())
			{
				OracleParameter[] aParamIn = null;
				OracleParameter[] aParamOut = null;
				int Ret = ExecUpdate(conn, SpName, aParamIn, ref aParamOut, tran);
				tran.Commit();

				return Ret;
			}
		}

		/// <summary>
		/// Execute raw SQL and return affected record count.
		/// </summary>
		/// <param name="conn">OracleConnection object already openned</param>
		/// <param name="Sql">SQL statement</param>
		/// <param name="tran">OracleTransaction object already started</param>
		/// <returns>Record count affected</returns>
		/// <example>
		/// <code>
		/// //If Sql2 fails, Sql1 is also invalid because same transaction was used.
		/// using (OracleConnection conn = CConn.GetConnection())
		/// {
		///		using (OracleTransaction tran = conn.BeginTransaction())
		///		{
		///			string Sql1 = "INSERT INTO T_TEST VALUES(1);";
		///			int RowsAffected1 = CStoredProc.ExecUpdateSql(conn, Sql, tran);
		///
		///			string Sql2 = "INSERT INTO T_TEST VALUES(2);";
		///			int RowsAffected2 = CStoredProc.ExecUpdateSql(conn, Sql, tran);
		///			
		///			tran.Commit();
		///		}
		///	}
		/// </code>
		/// </example>
		public static int ExecUpdateSql(OracleConnection conn, string Sql, OracleTransaction tran)
		{
			using (OracleCommand cmd = new OracleCommand())
			{
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = Sql;
				if (tran != null)
					cmd.Transaction = tran;
				return cmd.ExecuteNonQuery();
			}
		}
		/// <summary>
		/// Execute raw SQL and return affected record count.
		/// </summary>
		/// <param name="conn">OracleConnection object already openned</param>
		/// <param name="Sql">SQL statement</param>
		/// <returns>Record count affected</returns>
		/// <example>
		/// <code>
		/// //If Sql2 fails, Sql1 is still valid because different transaction used for each call.
		/// using (OracleConnection conn = CConn.GetConnection())
		/// {
		///			string Sql1 = "INSERT INTO T_TEST VALUES(1);";
		///			int RowsAffected1 = CStoredProc.ExecUpdateSql(conn, Sql, tran);
		///
		///			string Sql2 = "INSERT INTO T_TEST VALUES(2);";
		///			int RowsAffected2 = CStoredProc.ExecUpdateSql(conn, Sql, tran);
		///	}
		/// </code>
		/// </example>
		public static int ExecUpdateSql(OracleConnection conn, string Sql)
		{
			return ExecUpdateSql(conn, Sql, null);
		}

		/// <summary>
		/// Return text for calling procedure and print output parameter to use at TOAD
		/// </summary>
		/// <param name="SpName">Procedure name</param>
		/// <param name="aParamIn">Input parameter list</param>
		/// <param name="aParamOut">Output parameter list</param>
		/// <returns>All SQL statement needed for calling procedure and printing output</returns>
		/// <example>
		/// <code>
		/// OracleParameter[] aParamIn = new OracleParameter[]
		/// {
		///     new OracleParameter("v_p_id", Id),
		///     new OracleParameter("v_p_name", Name)
		/// };
		/// //int Ret = CSpDb.ExecUpdate(conn, "P_TEST_S_USER_NAME", aParamIn);
		/// CCommon.LogWeb.WriteLog(LogTypes.Test, CSpDb.GetSpTextForDebug("P_TEST_S_USER_NAME", false, aParamIn));
		/// 
		/// //Output:
		/// declare
		///     v_p_id	VARCHAR2(7):='test_id';
		///     v_p_name	VARCHAR2(9):='test_name';
		///     v_p_error_code	VARCHAR2(500);
		///     v_p_row_count	Number;
		///     v_p_error_note	VARCHAR2(500);
		///     v_p_return_str	VARCHAR2(500);
		///     v_p_error_str	VARCHAR2(500);
		///     v_errorstate	VARCHAR2(500);
		///     v_errorprocedure	VARCHAR2(500);
		/// begin
		///     P_TEST_S_USER_NAME (v_p_id => v_p_id
		///     , v_p_name => v_p_name
		///     , v_p_error_code => v_p_error_code
		///     , v_p_row_count => v_p_row_count
		///     , v_p_error_note => v_p_error_note
		///     , v_p_return_str => v_p_return_str
		///     , v_p_error_str => v_p_error_str
		///     , v_errorstate => v_errorstate
		///     , v_errorprocedure => v_errorprocedure);
		/// end;
		/// </code>
		/// </example>
		public static string GetSpTextForDebug(string SpName, OracleParameter[] aParamIn, OracleParameter[] aParamOut)
		{
			string Template = @"
{{DeclareOuter}}

declare
	{{DeclareInner}}
begin
	{{SpName}} ({{ParamList}});
end;

{{PrintCursor}}
";
			string DeclareOuter = "";
			string DeclareInner = "";
			string ParamList = "";
			string PrintCursor = "";

			if (aParamIn != null)
			{
				foreach (OracleParameter p in aParamIn)
				{
					string TypeDesc = GetTypeDesc(p, false);

					DeclareInner += "\r\n\t" + p.ParameterName + "\t" + TypeDesc + ":=" + GetParamValueForDebug(p) + ";";

					ParamList += "\r\n\t, " + p.ParameterName + " => " + p.ParameterName;
				}
			}

			if (aParamOut != null)
			{
				foreach (OracleParameter p in aParamOut)
				{
					string TypeDesc = GetTypeDesc(p, true);

					DeclareOuter += "\r\nvar " + p.ParameterName + "\t" + TypeDesc + ";";

					PrintCursor += "\r\nprint " + p.ParameterName + ";";

					ParamList += "\r\n\t, " + p.ParameterName + " => " + ":" + p.ParameterName;
				}
			}

			if (!string.IsNullOrEmpty(ParamList))
				ParamList = ParamList.Substring("\r\n\t, ".Length);

			return Template
				.Replace("{{DeclareOuter}}", DeclareOuter)
				.Replace("{{DeclareInner}}", DeclareInner)
				.Replace("{{SpName}}", SpName)
				.Replace("{{ParamList}}", ParamList)
				.Replace("{{PrintCursor}}", PrintCursor);
		}
		private static string GetTypeDesc(OracleParameter Param, bool ForOutOfDeclare)
		{
			bool IntFloatToNumber = ForOutOfDeclare;
			string TypeDesc = CType.OracleTypeToDescription(Param.OracleType, IntFloatToNumber);

			if (ForOutOfDeclare && (TypeDesc == "DATE"))
			{
				TypeDesc = "VARCHAR2";
			}

			if (CLang.In(Param.OracleType, OracleType.Char, OracleType.NChar, OracleType.VarChar, OracleType.NVarChar))
			{
				int Size = Math.Max(Param.Size, 50);
				TypeDesc += "(" + Size.ToString() + ")";
			}

			return TypeDesc;
		}
		public static string GetSpTextForDebug(string SpName, OracleParameter[] aParamIn)
		{
			return GetSpTextForDebug(SpName, aParamIn, null);
		}

		public static OracleParameter[] DbParameterArrayToOracleParameterArray(DbParameter[] oParam)
		{
			if (oParam == null)
				return null;

			OracleParameter[] aParamNew = new OracleParameter[oParam.Length];

			for (int i = 0; i < oParam.Length; i++)
			{
				aParamNew[i] = (OracleParameter)oParam[i];
			}

			return aParamNew;
		}
		#endregion Oracle


		#region Odbc
		public static DataTable ExecDataTable(OdbcConnection conn, string SpName, OdbcParameter[] aOdbcParam)
		{
			using (OdbcCommand cmd = new OdbcCommand())
			{
				cmd.Connection = conn;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = SpName;

				//for (int i = 0; i < aOdbcParam.Length; i++)
				//{
				//	cmd.Parameters.Add(aOdbcParam[i]);
				//}

				Dictionary<string, object> dicParam = CStoredProc.DbParameterArrayToDictionary(aOdbcParam);

				//오라클의 procedure는 데이터를 리턴하지 않으므로 t_cursor type의 v_cursor 인수가 꼭 있는 것으로
				//규칙을 정했으므로 무조건 추가함.
				//-> Odbc에는 RefCursor Type이 없어 주석.
				//cmd.Parameters.Add(new OdbcParameter("v_cursor", OdbcType.RefCursor, ParameterDirection.Output));

				//-> Odbc에는 RefCursor Type이 없어 다른 Type으로 대체해보려 했으나 SQL문이 부적절합니다. 에러 발생.
				//cmd.Parameters.Add("v_cursor", OdbcType.Image);
				//cmd.Parameters[cmd.Parameters.Count - 1].Direction = ParameterDirection.Input;

				//Odbc의 경우 RefCursor Type이 없으므로 자동으로 가져와야 함.
				//-> 자동으로 가져와도 RefCursor Type은 못 가져와서 주석.
				OdbcCommandBuilder.DeriveParameters(cmd);

				foreach (OdbcParameter Param in cmd.Parameters)
				{
					if (Param.Direction == ParameterDirection.Input)
					{
						switch (Param.OdbcType)
						{
							case OdbcType.BigInt:
							case OdbcType.Bit:
							case OdbcType.Int:
							case OdbcType.SmallInt:
							case OdbcType.TinyInt:
								Param.Value = Convert.ToInt64(dicParam[Param.ParameterName]);
								break;
							case OdbcType.Decimal:
							case OdbcType.Double:
							case OdbcType.Real:
								Param.Value = Convert.ToDecimal(dicParam[Param.ParameterName]);
								break;
							case OdbcType.Date:
							case OdbcType.DateTime:
							case OdbcType.SmallDateTime:
							case OdbcType.Time:
							case OdbcType.Timestamp:
								Param.Value = Convert.ToDateTime(dicParam[Param.ParameterName]);
								break;
							case OdbcType.Char:
							case OdbcType.Image:
							case OdbcType.NChar:
							case OdbcType.NText:
							case OdbcType.NVarChar:
							case OdbcType.Text:
							case OdbcType.UniqueIdentifier:
							case OdbcType.VarBinary:
							case OdbcType.VarChar:
								Param.Value = dicParam[Param.ParameterName];
								break;
						}
					}
					else
					{
						Param.Value = DBNull.Value;
					}
				}

				OdbcDataAdapter da = new OdbcDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				DataTable dt = ds.Tables[0];

				return dt;
			}
		}
		public static DataTable ExecDataTable(OdbcConnection conn, string SpName, OdbcParameter OdbcParam)
		{
			return ExecDataTable(conn, SpName, new OdbcParameter[] { OdbcParam });
		}
		public static DataTable ExecDataTable(OdbcConnection conn, string SpName)
		{
			return ExecDataTable(conn, SpName, new OdbcParameter[] { });
		}

		public static DataTable ExecDataTableSql(OdbcConnection conn, string Sql)
		{
			using (OdbcCommand cmd = new OdbcCommand())
			{
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = Sql;

				OdbcDataAdapter da = new OdbcDataAdapter(cmd);
				DataSet ds = new DataSet();
				da.Fill(ds);
				if ((ds == null) || (ds.Tables.Count == 0))
					return null;

				return ds.Tables[0];
			}
		}

		/// <summary>
		/// 데이터를 추가, 수정, 삭제하기 위함.
		/// </summary>
		/// <param name="SpName">SP 이름</param>
		/// <param name="aParamIn">SP의 파라미터의 이름과 값 정보</param>
		/// <param name="aParamOut">SP의 output 형식 파라미터의 이름과 값 정보</param>
		/// <returns></returns>
		/// <example>
		/// OdbcConnection conn = new OdbcConnection("Data Source = DOCTORGU; Initial Catalog = Shop; Uid=sa;Pwd=wlrvks2002");
		/// conn.Open();
		/// CStoredProc sp = new CStoredProc(conn);
		/// OdbcParameter[] aParamIn = new OdbcParameter[]{new OdbcParameter("@SeriesCD",  1),
		/// 												   new OdbcParameter("@SeriesName", "시리즈명"),
		/// 												   new OdbcParameter("@SeriesISBN", "")
		/// 											   };
		/// int nReturn = sp.ExecUpdate("OPupdateSeries", aParamIn);
		/// Console.WriteLine(nReturn.ToString());
		/// </example>
		public static int ExecUpdate(OdbcConnection conn, string SpName, OdbcParameter[] aParamIn, ref OdbcParameter[] aParamOut)
		{
			//도중에 에러가 나면 트랜잭션이 중첩되는 경우가 있어 확실하게 트랜잭션을 종료함.
			using (OdbcTransaction tran = conn.BeginTransaction())
			{
				using (OdbcCommand cmd = new OdbcCommand())
				{
					cmd.Connection = conn;
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = SpName;

					for (int i = 0; i < aParamIn.Length; i++)
					{
						cmd.Parameters.Add(aParamIn[i]);
					}

					if (aParamOut != null)
					{
						for (int i = 0; i < aParamOut.Length; i++)
						{
							aParamOut[i].Direction = ParameterDirection.Output;
							cmd.Parameters.Add(aParamOut[i]);
						}
					}

					/* ReturnValue 형식이라 추가하면 파라미터 개수가 틀려져서 프로시저가 없다고 나옴.
					OdbcParameter ParamReturn = new OdbcParameter("ReturnValue", OdbcDbType.Int32);
					ParamReturn.Direction = ParameterDirection.ReturnValue;
					cmd.Parameters.Add(ParamReturn);
					*/

					cmd.ExecuteNonQuery();
				}

				tran.Commit();

				return 0;
			}
		}
		public static int ExecUpdate(OdbcConnection conn, string SpName, OdbcParameter[] aParamIn)
		{
			OdbcParameter[] aParamOut = null;
			return ExecUpdate(conn, SpName, aParamIn, ref aParamOut);
		}
		public static int ExecUpdate(OdbcConnection conn, string SpName, OdbcParameter ParamIn)
		{
			OdbcParameter[] aParamOut = null;
			return ExecUpdate(conn, SpName, new OdbcParameter[] { ParamIn }, ref aParamOut);
		}
		public static int ExecUpdate(OdbcConnection conn, string SpName)
		{
			OdbcParameter[] aParamOut = null;
			return ExecUpdate(conn, SpName, new OdbcParameter[] { }, ref aParamOut);
		}

		public static int ExecUpdateSql(OdbcConnection conn, string Sql)
		{
			using (OdbcCommand cmd = new OdbcCommand())
			{
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = Sql;
				return cmd.ExecuteNonQuery();
			}
		}

		public static string GetSpTextForDebug(OdbcCommand cmd)
		{
			string ReturnName = "";
			string ParamList = "";
			foreach (OdbcParameter Param in cmd.Parameters)
			{
				string Quot = "";
				switch (Param.OdbcType)
				{
					case OdbcType.Binary:
					case OdbcType.Char:
					case OdbcType.Date:
					case OdbcType.DateTime:
					case OdbcType.Image:
					case OdbcType.NChar:
					case OdbcType.NText:
					case OdbcType.NVarChar:
					case OdbcType.SmallDateTime:
					case OdbcType.Text:
					case OdbcType.Time:
					case OdbcType.Timestamp:
					case OdbcType.UniqueIdentifier:
					case OdbcType.VarBinary:
					case OdbcType.VarChar:
						Quot = "'";
						break;
				}

				string Dir = "";
				switch (Param.Direction)
				{
					case ParameterDirection.Input:
						Dir = "";
						break;
					case ParameterDirection.InputOutput:
					case ParameterDirection.Output:
						Dir = " output";
						break;
					case ParameterDirection.ReturnValue:
						ReturnName = Param.ParameterName;
						continue;
				}

				string Value = Param.Value.ToString();
				if (Quot != "")
					Value = Value.Replace("'", "''");

				ParamList += ", " + Quot + Value + Quot + Dir;
			}
			if (ParamList != "")
			{
				ParamList = ParamList.Substring(2);
			}

			string Stmt = "";
			if (ReturnName != "")
			{
				Stmt += ReturnName + " = ";
			}
			Stmt += cmd.CommandText + " " + ParamList;

			return Stmt;
		}
		public static string GetSpTextForDebug(string SpName, OdbcParameter[] aParamIn)
		{
			return GetSpTextForDebug(SpName, aParamIn, (OdbcParameter[])null);
		}
		public static string GetSpTextForDebug(string SpName, OdbcParameter[] aParamIn, OdbcParameter[] aParamOut)
		{
			string Params = "";

			if (aParamIn != null)
			{
				foreach (OdbcParameter p in aParamIn)
				{
					//Parameter의 형식을 정확히 알 수는 없으므로 숫자형식이면 숫자로 일단 인식함.
					if (CValid.IsNumber(p.Value.ToString()))
					{
						Params += "\r\n\t, " + p.ParameterName + " = " + p.Value;
					}
					else
					{
						Params += "\r\n\t, '" + p.ParameterName + " = " + p.Value + "'";
					}
				}
			}

			if (aParamOut != null)
			{
				foreach (OdbcParameter p in aParamOut)
				{
					Params += "\r\n\t, " + p.ParameterName + " output";
				}
			}

			if (Params.Length > 0)
				Params = Params.Substring(5);

			return SpName + "\t" + Params;
		}
		#endregion Odbc
	}
}
