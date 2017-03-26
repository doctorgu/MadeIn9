using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;
using System.Data.OracleClient;
using System.Data.Common;

namespace DoctorGu
{
	public struct SDotNetTypeNameSimple
	{
		public const string SystemString = "System.String";
		public const string SystemInt32 = "System.Int32";
		public const string SystemDouble = "System.Double";
		public const string SystemDateTime = "System.DateTime";
		public const string SystemBoolean = "System.Boolean";

		private static Dictionary<string, string> mdicTypeDescription;

		static SDotNetTypeNameSimple()
		{
			mdicTypeDescription = new Dictionary<string, string>();
			mdicTypeDescription.Add(SDotNetTypeNameSimple.SystemString, "문자열");
			mdicTypeDescription.Add(SDotNetTypeNameSimple.SystemInt32, "숫자(정수형)");
			mdicTypeDescription.Add(SDotNetTypeNameSimple.SystemDouble, "숫자(실수형)");
			mdicTypeDescription.Add(SDotNetTypeNameSimple.SystemDateTime, "날짜");
			mdicTypeDescription.Add(SDotNetTypeNameSimple.SystemBoolean, "예/아니요");
		}

		public static Dictionary<string, string> TypeDescription
		{
			get { return mdicTypeDescription; }
		}
	}

	public class CType
	{
		public static bool GetIsLeftAlignByTypeName(Type Type)
		{
			if (
				(Type == typeof(System.String))
				|| (Type == typeof(System.Char))
				|| (Type == typeof(System.DateTime))
				)
			{
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Convert.ChangeType은 Enum 형식을 제대로 변환하지 못하므로 Enum 형식에 대한 변환을 추가함.
		/// 문자열 "0", "1"의 경우 bool로 바로 변경하면 에러나므로 int로 변경후 bool로 변환을 추가함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="ConversionType"></param>
		/// <returns></returns>
		public static object ChangeType(string Value, Type ConversionType)
		{
			if (ConversionType.IsEnum)
			{
				return CReflection.GetEnumValueByInt32(ConversionType, Convert.ToInt32(Value));
			}
			else if (ConversionType == typeof(bool))
			{
				return Convert.ToBoolean(Convert.ToInt32(Value));
			}
			else
			{
				return Convert.ChangeType(Value, ConversionType);
			}
		}

		public static Type ConvertSqlDbTypeToDotNetType(SqlDbType DbType)
		{
			Type t = null;
			
			switch (DbType)
			{
				case SqlDbType.BigInt:
					t = typeof(Int64);
					break;
				case SqlDbType.Binary:
				case SqlDbType.Image:
				case SqlDbType.Timestamp:
				case SqlDbType.VarBinary:
					t = typeof(Byte[]);
					break;
				case SqlDbType.Bit:
					t = typeof(Boolean);
					break;
				case SqlDbType.Char:
				case SqlDbType.NChar:
				case SqlDbType.NText:
				case SqlDbType.NVarChar:
				case SqlDbType.Text:
				case SqlDbType.VarChar:
					t = typeof(String);
					break;
				case SqlDbType.DateTime:
				case SqlDbType.SmallDateTime:
					t = typeof(DateTime);
					break;
				case SqlDbType.Decimal: //numeric=decimal 포함됨.
				case SqlDbType.Money:
				case SqlDbType.SmallMoney:
					t = typeof(Decimal);
					break;
				case SqlDbType.Float:
					t = typeof(Double);
					break;
				case SqlDbType.Int:
					t = typeof(Int32);
					break;
				case SqlDbType.Real:
					t = typeof(Single);
					break;
				case SqlDbType.SmallInt:
					t = typeof(Int16);
					break;
				case SqlDbType.TinyInt:
					t = typeof(Byte);
					break;
				case SqlDbType.Udt:
					throw new Exception("Udt: 변환할 수 없는 형식");
				case SqlDbType.UniqueIdentifier:
					t = typeof(Guid);
					break;
				case SqlDbType.Variant:
					t = typeof(Object);
					break;
				case SqlDbType.Xml:
					t = typeof(String);
					break;
				default:
					break;
			}

			return t;
		}
		public static Type ConvertOracleTypeToDotNetType(OracleType DbType)
		{
			Type t = null;
			
			switch (DbType)
			{
				case OracleType.Int16:
					t = typeof(Int16);
					break;
				case OracleType.Int32:
					t = typeof(Int32);
					break;
				case OracleType.Double:
					t = typeof(Double);
					break;
				case OracleType.Number:
					t = typeof(Decimal);
					break;
				case OracleType.DateTime:
				case OracleType.Timestamp:
				case OracleType.TimestampLocal:
				case OracleType.TimestampWithTZ:
					t = typeof(DateTime);
					break;
				case OracleType.Byte:
					t = typeof(Byte);
					break;
				case OracleType.BFile:
				case OracleType.Blob:
				case OracleType.LongRaw:
				case OracleType.Raw:
					t = typeof(Byte[]);
					break;
				case OracleType.Clob:
				case OracleType.LongVarChar:
				case OracleType.Char:
				case OracleType.NChar:
				case OracleType.NVarChar:
				case OracleType.NClob:
				case OracleType.VarChar:
					t = typeof(String);
					break;
			}

			return t;
		}
		public static SqlDbType ConvertDotNetTypeToSqlDbType(Type DotNetType)
		{
			SqlDbType t = SqlDbType.NVarChar;

			if (DotNetType == typeof(System.Int64))
				t = SqlDbType.BigInt;
			else if (DotNetType == typeof(System.Byte[]))
				t = SqlDbType.Binary;
			else if (DotNetType == typeof(System.Boolean))
				t = SqlDbType.Bit;
			else if (DotNetType == typeof(System.String))
				t = SqlDbType.NVarChar;
			else if (DotNetType == typeof(System.DateTime))
				t = SqlDbType.DateTime;
			else if (DotNetType == typeof(System.Decimal))
				t = SqlDbType.Decimal;
			else if (DotNetType == typeof(System.Double))
				t = SqlDbType.Float;
			else if (DotNetType == typeof(System.Int32))
				t = SqlDbType.Int;
			else if (DotNetType == typeof(System.Single))
				t = SqlDbType.Real;
			else if (DotNetType == typeof(System.Int16))
				t = SqlDbType.SmallInt;
			else if (DotNetType == typeof(System.Byte))
				t = SqlDbType.TinyInt;
			else if (DotNetType == typeof(System.Guid))
				t = SqlDbType.UniqueIdentifier;
			else if (DotNetType == typeof(System.Object))
				t = SqlDbType.Variant;

			return t;
		}

		public static string OracleTypeToDescription(OracleType Type, bool IntFloatToNumber)
		{
			string Desc = "";

			switch (Type)
			{
				case OracleType.DateTime: Desc = "DATE"; break;
				case OracleType.Cursor: Desc = "REFCURSOR"; break;
				case OracleType.Double: Desc = "FLOAT"; break;
				case OracleType.Float: Desc = (IntFloatToNumber ? "NUMBER" : "FLOAT"); break;
				case OracleType.Int16: Desc = (IntFloatToNumber ? "NUMBER" : "INTEGER"); break;
				case OracleType.Int32: Desc = (IntFloatToNumber ? "NUMBER" : "INTEGER"); break;
				case OracleType.IntervalDayToSecond: Desc = "INTERVAL DAY TO SECOND"; break;
				case OracleType.IntervalYearToMonth: Desc = "INTERVAL YEAR TO MONTH"; break;
				case OracleType.LongRaw: Desc = "LONG RAW"; break;
				case OracleType.LongVarChar: Desc = "LONG"; break;
				case OracleType.NVarChar: Desc = "NVARCHAR2"; break;
				case OracleType.SByte: Desc = (IntFloatToNumber ? "NUMBER" : "INTEGER"); break;
				case OracleType.TimestampLocal: Desc = "TIMESTAMP WITH LOCAL TIME ZONE"; break;
				case OracleType.TimestampWithTZ: Desc = "TIMESTAMP WITH TIME ZONE"; break;
				case OracleType.UInt16: Desc = (IntFloatToNumber ? "NUMBER" : "INTEGER"); break;
				case OracleType.UInt32: Desc = (IntFloatToNumber ? "NUMBER" : "INTEGER"); break;
				case OracleType.VarChar: Desc = "VARCHAR2"; break;
				default: Desc = Type.ToString(); break;
			}

			return Desc;
		}
		public static string OracleTypeToDescription(OracleType Type)
		{
			return OracleTypeToDescription(Type, false);
		}

		public static bool IsNumericType(Type DotNetType)
		{
			bool IsNumeric = false;

			if (DotNetType == typeof(System.Int64))
				IsNumeric = true;
			else if (DotNetType == typeof(System.Decimal))
				IsNumeric = true;
			else if (DotNetType == typeof(System.Double))
				IsNumeric = true;
			else if (DotNetType == typeof(System.Int32))
				IsNumeric = true;
			else if (DotNetType == typeof(System.Single))
				IsNumeric = true;
			else if (DotNetType == typeof(System.Int16))
				IsNumeric = true;
			else if (DotNetType == typeof(System.Byte))
				IsNumeric = true;

			return IsNumeric;
		}

		public static string ConvertColumnValueToString(object Value, string NullString)
		{
			if (Value == null)
				return NullString;

			Type t = Value.GetType();
			
			return ConvertValueToString(Value, t, NullString);
		}
		public static string ConvertParameterValueToString(DbParameter Param, string NullString)
		{
			object Value = null;
			Type t = null;

			if (Param.GetType() == typeof(SqlParameter))
			{
				Value = Param.Value;
				t = ConvertSqlDbTypeToDotNetType(((SqlParameter)Param).SqlDbType);
			}
			else if (Param.GetType() == typeof(OracleParameter))
			{
				Value = Param.Value;
				t = ConvertOracleTypeToDotNetType(((OracleParameter)Param).OracleType);
			}

			return ConvertValueToString(Value, t, NullString);
		}
		private static string ConvertValueToString(object Value, Type t, string NullString)
		{
			string Text = "";

			if (Value == DBNull.Value)
			{
				Text = NullString;
			}
			else if (t == typeof(DateTime))
			{
				DateTime dt = (DateTime)Value;
				//Text = dt.ToString("yyyy-MM-dd HH:mm:ss");
				Text = dt.ToString(CConst.Format_yyyy_MM_dd_HH_mm_ss_fff);
			}
			else if (t == typeof(bool))
			{
				//true, false를 1, 0으로 바꿈.
				Text = Convert.ToInt32(Value).ToString();
			}
			else
			{
				Text = Value.ToString();
			}

			return Text;
		}

		public static object ConvertStringToColumnValue(string Text, Type t, string NullString)
		{
			object Value = null;

			try
			{
				if ((string)Text == NullString)
				{
					Value = DBNull.Value;
				}
				else if (t == typeof(DateTime))
				{
					IFormatProvider culture = new CultureInfo("ko-KR");
					Value = DateTime.Parse(Text, culture);
				}
				else if (t == typeof(bool))
				{
					//bool의 경우 값이 1, 0이라도 문자열에서 바로 bool 형식으로 변환할 수 없음.
					Value = Convert.ToBoolean(Convert.ToInt32(Text));
				}
				else
				{
					Value = Convert.ChangeType(Text, t);
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("Text: {0}, Type: {1}, NullString: {2}, Message: {3}", Text, t.ToString(), NullString, ex.Message), ex);
			}

			return Value;
		}

		public static decimal ConvertPercentToDecimal(string Value)
		{
			if (Value.EndsWith("%"))
			{
				return Convert.ToDecimal(Value.Substring(0, (Value.Length - 1))) / 100;
			}
			else
			{
				return Convert.ToDecimal(Value);
			}
		}
		public static double ConvertPercentToDouble(string Value)
		{
			if (Value.EndsWith("%"))
			{
				return Convert.ToDouble(Value.Substring(0, (Value.Length - 1))) / 100;
			}
			else
			{
				return Convert.ToDouble(Value);
			}
		}

		public static int ConvertHexaToInt(string hexString)
		{
			int iHex = (int)Convert.ToUInt32(hexString, 16);
			return iHex;
		}

		public static string ConvertIntToHexa(int number)
		{
			return string.Format("{0:X}", number);
		}

		public static bool IsNullableType(Type t)
		{
			return 
				(t.IsGenericType
				&& t.GetGenericTypeDefinition().Equals(typeof(Nullable<>))
				);
		}
	}
}
