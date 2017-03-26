using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using ADODB;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Data;

namespace DoctorGu
{
	public class CCacheToFile
	{
		private string mCacheFolder;
		private bool mUseAdo;

		private const int drUnitForFile100 = 100;

		public CCacheToFile(string CacheFolder, bool UseAdo)
		{
			this.mCacheFolder = CacheFolder;
			this.mUseAdo = UseAdo;
		}

		public void SaveParamByProcToCache(string Key, List<SqlParameter> aParamToSave)
		{
			object data = null;
			if (this.mUseAdo)
			{
				data = ConvertToAdoRecordset(aParamToSave);
			}
			else
			{
				data = ConvertToDataTable(aParamToSave);
			}
			SaveDataByProcToCache(Key, data);
		}
		public void SaveParamByProcToCache(string DbName, string ProcName, string ParamList, List<SqlParameter> aParamToSave)
		{
			string Key = DbName + "@" + ProcName + "@" + ParamList;
			SaveParamByProcToCache(Key, aParamToSave);
		}

		public void SaveDataByProcToCache(string Key, object DataToSave)
		{
			string FullPathTemp = CFile.GetTempFileName();

			if (this.mUseAdo)
			{
				Recordset RsToSave = DataToSave as Recordset;
				RsToSave.Save(FullPathTemp, PersistFormatEnum.adPersistXML);
				SaveToCacheRs(Key, FullPathTemp);
			}
			else
			{
				DataTable DtToSave = DataToSave as DataTable;
				DtToSave.TableName = "Content";
				if (DtToSave.DataSet == null)
				{
					DtToSave.WriteXml(FullPathTemp, XmlWriteMode.WriteSchema);
				}
				else
				{
					DtToSave.DataSet.WriteXml(FullPathTemp, XmlWriteMode.WriteSchema);
				}
				SaveToCacheDt(Key, FullPathTemp);
			}
		}
		public void SaveDataByProcToCache(string DbName, string ProcName, string ParamList, object DataToSave)
		{
			string Key = DbName + "@" + ProcName + "@" + ParamList;
			SaveDataByProcToCache(Key, DataToSave);
		}

		public void SaveInternetFileToCache(string Url)
		{
			string FullPathTemp = CFile.GetTempFileName();
			CNet Net = new CNet();
			Net.DownloadFile(Url, FullPathTemp, true);

			if (this.mUseAdo)
			{
				SaveToCacheRs(Url, FullPathTemp);
			}
			else
			{
				SaveToCacheDt(Url, FullPathTemp);
			}
		}
		public string GetCachedInternetFullPath(string Url)
		{
			return GetFullPathCacheContent(Url);
		}


		public object GetCachedData(string Key)
		{
			string FullPathContent = GetFullPathCacheContent(Key);
			if (FullPathContent == "")
				return null;

			if (this.mUseAdo)
			{
				Recordset Rs = new Recordset();
				Rs.Open(FullPathContent, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);

				return Rs;
			}
			else
			{
				DataTable dt = null;
				try
				{
					DataSet ds = new DataSet();
					ds.ReadXml(FullPathContent);
					dt = ds.Tables[0];
				}
				catch (Exception)
				{
					dt = new DataTable();
					dt.ReadXml(FullPathContent);
				}

				return dt;
			}
		}
		public object GetCachedData(string DbName, string ProcName, string ParamList)
		{
			string Key = DbName + "@" + ProcName + "@" + ParamList;
			return GetCachedData(Key);
		}

		public SqlParameter[] GetCachedParam(string Key)
		{
			SqlParameter[] aParam = null;
			if (this.mUseAdo)
			{
				Recordset rs = (Recordset)GetCachedData(Key);
				if (rs == null)
					return null;

				aParam = ConvertRsToSqlParameter(rs);
			}
			else
			{
				DataTable dt = (DataTable)GetCachedData(Key);
				if (dt == null)
					return null;

				aParam = ConvertDtToSqlParameter(dt);
			}

			return aParam;
		}
		public SqlParameter[] GetCachedParam(string DbName, string ProcName, string ParamList)
		{
			string Key = DbName + "@" + ProcName + "@" + ParamList;
			return GetCachedParam(Key);
		}

		public void Remove(string Key)
		{
			string Criteria = "Key = '" + Key.Replace("'", "''") + "'";

			if (mUseAdo)
			{
				string FullPathIndex = mCacheFolder + "\\CacheIndex.rs";
				if (!File.Exists(FullPathIndex))
					throw new Exception(FullPathIndex + " 파일이 없습니다.");

				Recordset RsIndex = new Recordset();
				RsIndex.Open(FullPathIndex, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);

				RsIndex.Find(Criteria, 0, SearchDirectionEnum.adSearchForward, Missing.Value);
				if (RsIndex.EOF)
					throw new Exception(Criteria + " 조건에 해당하는 행이 " + FullPathIndex + " 파일에 없습니다.");

				string FileName = (string)RsIndex.Fields["FileName"].Value;
				string FullPathCache = mCacheFolder + "\\" + FileName;


				Recordset rs = new Recordset();
				rs.Open(FullPathCache, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);

				rs.Find(Criteria, 0, SearchDirectionEnum.adSearchForward, Missing.Value);
				if (rs.EOF)
					throw new Exception(Criteria + " 조건에 해당하는 행이 " + FullPathCache + " 파일에 없습니다.");

				rs.Delete(AffectEnum.adAffectCurrent);
				if (rs.RecordCount > 0)
				{
					rs.Save(FullPathCache, PersistFormatEnum.adPersistXML);
					rs.Close();
				}
				else
				{
					rs.Close();
					File.Delete(FullPathCache);
				}
				rs = null;

				RsIndex.Delete(AffectEnum.adAffectCurrent);
				RsIndex.Save(FullPathIndex, PersistFormatEnum.adPersistXML);
				RsIndex.Close();
				RsIndex = null;
			}
			else
			{
				string FullPathIndex = mCacheFolder + "\\CacheIndex.dt";
				if (!File.Exists(FullPathIndex))
					throw new Exception(FullPathIndex + " 파일이 없습니다.");

				DataTable DtIndex = new DataTable();
				DtIndex.ReadXml(FullPathIndex);

				DataRow[] adrIndex = DtIndex.Select(Criteria);
				if (adrIndex.Length == 0)
					throw new Exception(Criteria + " 조건에 해당하는 행이 " + FullPathIndex + " 파일에 없습니다.");

				string FileName = (string)adrIndex[0]["FileName"];
				string FullPathCache = mCacheFolder + "\\" + FileName;


				DataTable dt = new DataTable();
				dt.ReadXml(FullPathCache);

				DataRow[] adrCache = dt.Select(Criteria);
				if (adrCache.Length == 0)
					throw new Exception(Criteria + " 조건에 해당하는 행이 " + FullPathCache + " 파일에 없습니다.");

				dt.Rows.Remove(adrCache[0]);
				if (dt.Rows.Count > 0)
				{
					dt.WriteXml(FullPathCache, XmlWriteMode.WriteSchema);
				}
				else
				{
					File.Delete(FullPathCache);
				}

				DtIndex.Rows.Remove(adrIndex[0]);
				DtIndex.WriteXml(FullPathIndex, XmlWriteMode.WriteSchema);
			}
		}
		public void Remove(string DbName, string ProcName, string ParamList)
		{
			string Key = DbName + "@" + ProcName + "@" + ParamList;
			Remove(Key);
		}

		private Recordset ConvertToAdoRecordset(List<SqlParameter> aParam)
		{
			Recordset Rs = new Recordset();

			foreach (SqlParameter Param in aParam)
			{
				string Name = Param.ParameterName;
				if (Name.StartsWith("@"))
					Name = Name.Substring(1);

				DataTypeEnum DataType = CType.ConvertSqlDbTypeToAdoType(Param.SqlDbType);
				switch (DataType)
				{
					case DataTypeEnum.adVarChar:
					case DataTypeEnum.adVarWChar:
					case DataTypeEnum.adChar:
					case DataTypeEnum.adWChar:
						Rs.Fields.Append(Name, DataType, 4000, FieldAttributeEnum.adFldIsNullable, null);
						break;
					default:
						Rs.Fields.Append(Name, DataType, -1, FieldAttributeEnum.adFldIsNullable, null);
						break;
				}
			}

			Rs.Open(Missing.Value, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);
			Rs.AddNew(Missing.Value, Missing.Value);

			for (int i = 0, i2 = aParam.Count; i < i2; i++)
			{
				Rs.Fields[i].Value = aParam[i].Value;
			}

			return Rs;
		}
		private DataTable ConvertToDataTable(List<SqlParameter> aParam)
		{
			DataTable dt = new DataTable();

			foreach (SqlParameter Param in aParam)
			{
				string Name = Param.ParameterName;
				if (Name.StartsWith("@"))
					Name = Name.Substring(1);

				Type DataType = CType.ConvertSqlDbTypeToDotNetType(Param.SqlDbType);
				dt.Columns.Add(Name, DataType);
			}

			DataRow dr = dt.NewRow();
			for (int i = 0, i2 = aParam.Count; i < i2; i++)
			{
				dr[i] = aParam[i].Value;
			}
			dt.Rows.Add(dr);

			return dt;
		}

		private SqlParameter[] ConvertRsToSqlParameter(Recordset Rs)
		{
			SqlParameter[] aParam = new SqlParameter[Rs.Fields.Count];

			for (int cl = 0, cl2 = aParam.Length; cl < cl2; cl++)
			{
				SqlDbType DbType = CType.ConvertAdoTypeToSqlDbType(Rs.Fields[cl].Type);
				switch (DbType)
				{
					case SqlDbType.Char:
					case SqlDbType.NChar:
					case SqlDbType.NVarChar:
					case SqlDbType.VarChar:
						aParam[cl] = new SqlParameter(Rs.Fields[cl].Name, DbType, 4000);
						break;
					default:
						aParam[cl] = new SqlParameter(Rs.Fields[cl].Name, DbType);
						break;
				}

				aParam[cl].Value = Rs.Fields[cl].Value;
			}

			return aParam;
		}
		private SqlParameter[] ConvertDtToSqlParameter(DataTable dt)
		{
			SqlParameter[] aParam = new SqlParameter[dt.Columns.Count];

			for (int cl = 0, cl2 = aParam.Length; cl < cl2; cl++)
			{
				SqlDbType DbType = CType.ConvertDotNetTypeToSqlDbType(dt.Columns[cl].DataType);
				switch (DbType)
				{
					case SqlDbType.Char:
					case SqlDbType.NChar:
					case SqlDbType.NVarChar:
					case SqlDbType.VarChar:
						aParam[cl] = new SqlParameter(dt.Columns[cl].ColumnName, DbType, 4000);
						break;
					default:
						aParam[cl] = new SqlParameter(dt.Columns[cl].ColumnName, DbType);
						break;
				}

				aParam[cl].Value = dt.Rows[0][cl];
			}

			return aParam;
		}

		private void SaveToCacheRs(string Key, string FullPathToSave)
		{
			CreateCacheFolder();

			string FullPathCache = GetFullPathCacheRs(Key);
			if (!File.Exists(FullPathCache))
			{
				CreateRs(FullPathCache, false);
			}

			Recordset Rs = new Recordset();
			Rs.Open(FullPathCache, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);

			string Criteria = "Key = '" + Key.Replace("'", "''") + "'";
			Rs.Find(Criteria, 0, SearchDirectionEnum.adSearchForward, Missing.Value);

			if (!Rs.EOF)
			{
				Rs.Fields["Content"].AppendChunk(CFile.GetByteFromFile(FullPathToSave));
			}
			else
			{
				Rs.AddNew(Missing.Value, Missing.Value);
				Rs.Fields["Key"].Value = Key;
				Rs.Fields["Content"].AppendChunk(CFile.GetByteFromFile(FullPathToSave));
			}

			Rs.Save(FullPathCache, PersistFormatEnum.adPersistXML);
			Rs.Close();
			Rs = null;
		}
		private void SaveToCacheDt(string Key, string FullPathToSave)
		{
			CreateCacheFolder();

			string FullPathCache = GetFullPathCacheDt(Key);
			if (!File.Exists(FullPathCache))
			{
				CreateDt(FullPathCache, false);
			}

			DataTable dt = new DataTable();
			dt.ReadXml(FullPathCache);

			string Criteria = "Key = '" + Key.Replace("'", "''") + "'";
			DataRow[] adr = dt.Select(Criteria);

			if (adr.Length > 0)
			{
				adr[0]["Content"] = CFile.GetByteFromFile(FullPathToSave);
			}
			else
			{
				DataRow dr = dt.NewRow();
				dr["Key"] = Key;
				dr["Content"] = CFile.GetByteFromFile(FullPathToSave);
				dt.Rows.Add(dr);
			}

			dt.WriteXml(FullPathCache, XmlWriteMode.WriteSchema);
			dt = null;
		}

		private void CreateCacheFolder()
		{
			if (string.IsNullOrEmpty(this.mCacheFolder))
			{
				throw new Exception("CacheFolder 값이 없습니다");
			}

			if (!Directory.Exists(this.mCacheFolder))
			{
				Directory.CreateDirectory(this.mCacheFolder);
			}
		}

		private string GetFullPathCacheRs(string Key)
		{
			string FullPathIndex = mCacheFolder + "\\CacheIndex.rs";
			if (!File.Exists(FullPathIndex))
			{
				CreateRs(FullPathIndex, true);
			}

			Recordset RsIndex = new Recordset();
			RsIndex.Open(FullPathIndex, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);

			string FileName = "";
			int Seq = 0;

			string Criteria = "Key = '" + Key.Replace("'", "''") + "'";
			RsIndex.Find(Criteria, 0, SearchDirectionEnum.adSearchForward, Missing.Value);
			if (!RsIndex.EOF)
			{
				FileName = (string)RsIndex.Fields["FileName"].Value;
			}
			else
			{
				if (RsIndex.RecordCount > 0)
				{
					RsIndex.MoveLast();

					Seq = ((int)RsIndex.Fields["Seq"].Value + 1);

					FileName = (string)RsIndex.Fields["FileName"].Value;
					if (((Seq - 1) % drUnitForFile100) == 0)
					{
						FileName = GetNextFileName(FileName);
					}
				}
				else
				{
					Seq = 1;
					FileName = "Cache.rs";
				}

				RsIndex.AddNew(Missing.Value, Missing.Value);
				RsIndex.Fields["Seq"].Value = Seq;
				RsIndex.Fields["Key"].Value = Key;
				RsIndex.Fields["FileName"].Value = FileName;

				RsIndex.Save(FullPathIndex, PersistFormatEnum.adPersistXML);
				RsIndex.Close();
				RsIndex = null;
			}

			return mCacheFolder + "\\" + FileName;
		}
		private string GetFullPathCacheDt(string Key)
		{
			string FullPathIndex = mCacheFolder + "\\CacheIndex.dt";
			if (!File.Exists(FullPathIndex))
			{
				CreateDt(FullPathIndex, true);
			}

			DataTable DtIndex = new DataTable();
			DtIndex.ReadXml(FullPathIndex);

			string FileName = "";
			int Seq = 0;

			string Criteria = "Key = '" + Key.Replace("'", "''") + "'";
			DataRow[] adr = DtIndex.Select(Criteria);
			if (adr.Length != 0)
			{
				FileName = (string)adr[0]["FileName"];
			}
			else
			{
				if (DtIndex.Rows.Count > 0)
				{
					DataRow drLast = DtIndex.Rows[DtIndex.Rows.Count - 1];

					Seq = ((int)drLast["Seq"] + 1);

					FileName = (string)drLast["FileName"];
					if (((Seq - 1) % drUnitForFile100) == 0)
					{
						FileName = GetNextFileName(FileName);
					}
				}
				else
				{
					Seq = 1;
					FileName = "Cache.dt";
				}

				DataRow drNew = DtIndex.NewRow();
				drNew["Seq"] = Seq;
				drNew["Key"] = Key;
				drNew["FileName"] = FileName;
				DtIndex.Rows.Add(drNew);

				DtIndex.WriteXml(FullPathIndex, XmlWriteMode.WriteSchema);
				DtIndex = null;
			}

			return mCacheFolder + "\\" + FileName;
		}

		private string GetNextFileName(string FileName)
		{
			string File = Path.GetFileNameWithoutExtension(FileName);
			string Ext = Path.GetExtension(FileName);
			int Num = 0;

			if (File.IndexOf('_') != -1)
			{
				string[] aFileNum = File.Split('_');

				File = aFileNum[0];
				Num = Convert.ToInt32(aFileNum[1]);
			}

			return string.Concat(File, "_", (Num + 1), Ext);
		}

		private void CreateRs(string FullPathRs, bool IsIndex)
		{
			Recordset Rs = new Recordset();

			if (IsIndex)
			{
				Rs.Fields.Append("Seq", DataTypeEnum.adInteger, -1, FieldAttributeEnum.adFldIsNullable, null);
				Rs.Fields.Append("Key", DataTypeEnum.adVarChar, 4000, FieldAttributeEnum.adFldKeyColumn, null);
				Rs.Fields.Append("FileName", DataTypeEnum.adVarChar, 256, FieldAttributeEnum.adFldIsNullable, null);
			}
			else
			{
				Rs.Fields.Append("Key", DataTypeEnum.adVarChar, 4000, FieldAttributeEnum.adFldKeyColumn, null);
				Rs.Fields.Append("Content", DataTypeEnum.adLongVarBinary, -1, FieldAttributeEnum.adFldIsNullable, null);
			}

			Rs.Open(Missing.Value, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);

			Rs.Save(FullPathRs, PersistFormatEnum.adPersistXML);
			Rs.Close();
			Rs = null;
		}
		private void CreateDt(string FullPathDt, bool IsIndex)
		{
			DataTable dt = new DataTable();

			if (IsIndex)
			{
				dt.TableName = "Index";
				dt.Columns.Add("Seq", typeof(int));
				dt.Columns.Add("Key", typeof(string));
				dt.Columns.Add("FileName", typeof(string));
			}
			else
			{
				dt.TableName = "Content";
				dt.Columns.Add("Key", typeof(string));
				dt.Columns.Add("Content", typeof(byte[]));
			}

			dt.WriteXml(FullPathDt, XmlWriteMode.WriteSchema);
			dt = null;
		}

		private string GetFullPathCacheContent(string Key)
		{
			if (string.IsNullOrEmpty(this.mCacheFolder))
			{
				throw new Exception("CacheFolder 값이 없습니다");
			}

			string FullPathTemp = "";
			if (this.mUseAdo)
			{
				string FullPathCache = GetFullPathCacheRs(Key);
				//Remove 메쏘드로 삭제하다 더 이상 행이 없으면 파일 자체가 삭제됨
				if (!File.Exists(FullPathCache))
					return "";

				Recordset Rs = new Recordset();
				Rs.Open(FullPathCache, Missing.Value, CursorTypeEnum.adOpenStatic, LockTypeEnum.adLockPessimistic, -1);

				string Criteria = "Key = '" + Key.Replace("'", "''") + "'";
				Rs.Find(Criteria, 0, SearchDirectionEnum.adSearchForward, Missing.Value);
				if (Rs.EOF)
				{
					Rs.Close();
					return "";
				}

				FullPathTemp = CFile.GetTempFileName();
				CAdo.GetFileFromField(Rs.Fields["Content"], FullPathTemp);
			}
			else
			{
				string FullPathCache = GetFullPathCacheDt(Key);
				//Remove 메쏘드로 삭제하다 더 이상 행이 없으면 파일 자체가 삭제됨
				if (!File.Exists(FullPathCache))
					return "";

				DataTable dt = new DataTable();
				dt.ReadXml(FullPathCache);

				string Criteria = "Key = '" + Key.Replace("'", "''") + "'";
				DataRow[] adr = dt.Select(Criteria);
				if (adr.Length == 0)
				{
					dt = null;
					return "";
				}
				DataRow dr = adr[0];

				FullPathTemp = CFile.GetTempFileName();
				CFile.WriteByteToFile(FullPathTemp, (byte[])dr["Content"]);
			}

			return FullPathTemp;
		}
	}
}