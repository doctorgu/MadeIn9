using System;
using System.Collections.Generic;
using System.Text;
using ADODB;
using System.Data;
using System.Xml;

namespace DoctorGu
{
	public class CAdo
	{
		public static bool GetQuoteNeededByFieldType(DataTypeEnum FieldType)
		{
			switch (FieldType)
			{
				case DataTypeEnum.adDate:
				case DataTypeEnum.adDBDate:
				case DataTypeEnum.adDBTimeStamp:
				case DataTypeEnum.adChar:
				case DataTypeEnum.adWChar:
				case DataTypeEnum.adVarChar:
				case DataTypeEnum.adVarWChar:
				case DataTypeEnum.adLongVarChar:
				case DataTypeEnum.adLongVarWChar:
				case DataTypeEnum.adLongVarBinary:
					return true;
				default:
					return false;
			}
		}

		public static void GetFileFromField(ADODB.Field Fld, string FullPath)
		{
			byte[] aChunks = new byte[Fld.ActualSize];
			aChunks = (byte[])Fld.GetChunk(Fld.ActualSize);

			CFile.WriteByteToFile(FullPath, aChunks);
		}

		public static DataTable GetDataTableFromAdoXml(string FullPathRsXml)
		{
			using (XmlReader XReader = XmlReader.Create(FullPathRsXml))
			{
				return GetDataTableFromAdoXml(XReader);
			}
		}
		private static DataTable GetDataTableFromAdoXml(XmlReader XReader)
		{
			DataTable dt = null;

			string ColName = "";
			Type DataType = typeof(string);
			
			while (XReader.Read())
			{
				switch (XReader.NodeType)
				{
					case XmlNodeType.Element:
						switch (XReader.Name)
						{
							case "s:ElementType":
								dt = new DataTable();
								break;

							case "s:AttributeType":
								while (XReader.MoveToNextAttribute())
								{
									if (XReader.Name == "name")
									{
										ColName = XReader.Value;
										break;
									}
								}
								break;
							case "s:datatype":
								while (XReader.MoveToNextAttribute())
								{
									if (XReader.Name == "dt:type")
									{
										DataType = GetTypeByAdoDtType(XReader.Value);
										break;
									}
								}

								DataColumn ColNew = dt.Columns.Add(ColName, DataType);

								break;
							case "z:row":
								DataRow dr = dt.NewRow();
								while (XReader.MoveToNextAttribute())
								{
									if (!dt.Columns.Contains(XReader.Name))
										continue;

									Type TypeCur = dt.Columns[XReader.Name].DataType;
									object ValueCur = CType.ConvertStringToColumnValue(XReader.Value, TypeCur, "");
									dr[XReader.Name] = ValueCur;
								}
								dt.Rows.Add(dr);

								break;
							default:
								break;
						}

						break;
				}
			} //XReader.Read()

			return dt;
		}

		private static Type GetTypeByAdoDtType(string Value)
		{
			Type t = null;

			switch (Value)
			{
				case "string": t = typeof(string); break;
				case "int": t = typeof(int); break;
				case "number": t = typeof(double); break;
			}

			return t;
		}

		public static ADODB.DataTypeEnum ConvertDotNetTypeToAdoType(Type DotNetType)
		{
			switch (DotNetType.UnderlyingSystemType.ToString())
			{
				case "System.Boolean":
					return ADODB.DataTypeEnum.adBoolean;

				case "System.Byte":
					return ADODB.DataTypeEnum.adUnsignedTinyInt;

				case "System.Char":
					return ADODB.DataTypeEnum.adChar;

				case "System.DateTime":
					return ADODB.DataTypeEnum.adDate;

				case "System.Decimal":
					return ADODB.DataTypeEnum.adCurrency;

				case "System.Double":
					return ADODB.DataTypeEnum.adDouble;

				case "System.Int16":
					return ADODB.DataTypeEnum.adSmallInt;

				case "System.Int32":
					return ADODB.DataTypeEnum.adInteger;

				case "System.Int64":
					return ADODB.DataTypeEnum.adBigInt;

				case "System.SByte":
					return ADODB.DataTypeEnum.adTinyInt;

				case "System.Single":
					return ADODB.DataTypeEnum.adSingle;

				case "System.UInt16":
					return ADODB.DataTypeEnum.adUnsignedSmallInt;

				case "System.UInt32":
					return ADODB.DataTypeEnum.adUnsignedInt;

				case "System.UInt64":
					return ADODB.DataTypeEnum.adUnsignedBigInt;

				case "System.String":
				default:
					return ADODB.DataTypeEnum.adVarChar;
			}
		}

		public static ADODB.DataTypeEnum ConvertSqlDbTypeToAdoType(SqlDbType DbType)
		{
			switch (DbType)
			{
				case SqlDbType.BigInt:
					return ADODB.DataTypeEnum.adBigInt;
				case SqlDbType.Binary:
					return ADODB.DataTypeEnum.adBinary;
				case SqlDbType.Bit:
					return ADODB.DataTypeEnum.adBoolean;
				case SqlDbType.Char:
					return ADODB.DataTypeEnum.adChar;
				case SqlDbType.DateTime:
					return ADODB.DataTypeEnum.adDBTimeStamp;
				case SqlDbType.Decimal:
					return ADODB.DataTypeEnum.adNumeric;
				case SqlDbType.Float:
					return ADODB.DataTypeEnum.adDouble;
				case SqlDbType.Image:
					return ADODB.DataTypeEnum.adBinary;
				case SqlDbType.Int:
					return ADODB.DataTypeEnum.adInteger;
				case SqlDbType.Money:
					return ADODB.DataTypeEnum.adCurrency;
				case SqlDbType.NChar:
					return ADODB.DataTypeEnum.adWChar;
				case SqlDbType.NText:
					return ADODB.DataTypeEnum.adLongVarWChar;
				case SqlDbType.NVarChar:
					return ADODB.DataTypeEnum.adVarWChar;
				case SqlDbType.Real:
					return ADODB.DataTypeEnum.adSingle;
				case SqlDbType.SmallDateTime:
					return ADODB.DataTypeEnum.adDBTimeStamp;
				case SqlDbType.SmallInt:
					return ADODB.DataTypeEnum.adSmallInt;
				case SqlDbType.SmallMoney:
					return ADODB.DataTypeEnum.adCurrency;
				case SqlDbType.Text:
					return ADODB.DataTypeEnum.adLongVarChar;
				case SqlDbType.Timestamp:
					return ADODB.DataTypeEnum.adDBTimeStamp;
				case SqlDbType.TinyInt:
					return ADODB.DataTypeEnum.adUnsignedTinyInt;
				case SqlDbType.UniqueIdentifier:
					return ADODB.DataTypeEnum.adGUID;
				case SqlDbType.VarBinary:
					return ADODB.DataTypeEnum.adVarBinary;
				case SqlDbType.VarChar:
					return ADODB.DataTypeEnum.adVarChar;
				case SqlDbType.Variant:
					return ADODB.DataTypeEnum.adVariant;
				case SqlDbType.Xml:
					return ADODB.DataTypeEnum.adVarChar;
				default:
					return ADODB.DataTypeEnum.adVarChar;
			}
		}

		public static SqlDbType ConvertAdoTypeToSqlDbType(ADODB.DataTypeEnum AdoType)
		{
			switch (AdoType)
			{
				case ADODB.DataTypeEnum.adBigInt:
					return SqlDbType.BigInt;
				case ADODB.DataTypeEnum.adBinary:
					return SqlDbType.Binary;
				case ADODB.DataTypeEnum.adBoolean:
					return SqlDbType.Bit;
				case ADODB.DataTypeEnum.adChar:
					return SqlDbType.Char;
				case ADODB.DataTypeEnum.adDBTimeStamp:
					return SqlDbType.DateTime;
				case ADODB.DataTypeEnum.adNumeric:
					return SqlDbType.Decimal;
				case ADODB.DataTypeEnum.adDouble:
					return SqlDbType.Float;
				case ADODB.DataTypeEnum.adInteger:
					return SqlDbType.Int;
				case ADODB.DataTypeEnum.adCurrency:
					return SqlDbType.Money;
				case ADODB.DataTypeEnum.adWChar:
					return SqlDbType.NChar;
				case ADODB.DataTypeEnum.adLongVarWChar:
					return SqlDbType.NText;
				case ADODB.DataTypeEnum.adVarWChar:
					return SqlDbType.NVarChar;
				case ADODB.DataTypeEnum.adSingle:
					return SqlDbType.Real;
				case ADODB.DataTypeEnum.adSmallInt:
					return SqlDbType.SmallInt;
				case ADODB.DataTypeEnum.adLongVarChar:
					return SqlDbType.Text;
				case ADODB.DataTypeEnum.adUnsignedTinyInt:
					return SqlDbType.TinyInt;
				case ADODB.DataTypeEnum.adGUID:
					return SqlDbType.UniqueIdentifier;
				case ADODB.DataTypeEnum.adVarBinary:
					return SqlDbType.VarBinary;
				case ADODB.DataTypeEnum.adVarChar:
					return SqlDbType.VarChar;
				case ADODB.DataTypeEnum.adVariant:
					return SqlDbType.Variant;
				default:
					return SqlDbType.VarChar;
			}
		}

		public static ADODB.Recordset ConvertToRecordset(DataTable dt)
		{
			ADODB.Recordset rs = new ADODB.Recordset();
			rs.CursorLocation = ADODB.CursorLocationEnum.adUseClient;

			ADODB.Fields RsFld = rs.Fields;
			System.Data.DataColumnCollection DtCols = dt.Columns;

			foreach (DataColumn DtCol in DtCols)
			{
				RsFld.Append(DtCol.ColumnName
					, CAdo.ConvertDotNetTypeToAdoType(DtCol.DataType)
					, DtCol.MaxLength
					, DtCol.AllowDBNull ? ADODB.FieldAttributeEnum.adFldIsNullable :
											 ADODB.FieldAttributeEnum.adFldUnspecified
					, null);
			}

			rs.Open(System.Reflection.Missing.Value
					, System.Reflection.Missing.Value
					, ADODB.CursorTypeEnum.adOpenStatic
					, ADODB.LockTypeEnum.adLockOptimistic, 0);

			foreach (DataRow dr in dt.Rows)
			{
				rs.AddNew(System.Reflection.Missing.Value,
							  System.Reflection.Missing.Value);

				for (int cl = 0; cl < DtCols.Count; cl++)
				{
					RsFld[cl].Value = dr[cl];
				}
			}

			return rs;
		}

	}
}
