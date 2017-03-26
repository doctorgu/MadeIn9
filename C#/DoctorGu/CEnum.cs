using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;

namespace DoctorGu
{
	public class CEnum
	{
		/// <summary>
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// foreach (FieldInfo fi in GetFields<GoodTrackingVar>())
		/// {
		/// 	Debug.WriteLine(fi.Name);
		/// }
		/// ]]>
		/// </example>
		private static IEnumerable<FieldInfo> GetFields<T>()
		{
			Type TypeOfEnum = typeof(T);
			return TypeOfEnum.GetFields().Where(field => field.IsLiteral);
		}
		private static IEnumerable<KeyValuePair<string, string>> GetNameDescriptionHasDescription<T>()
		{
			foreach (FieldInfo fi in GetFields<T>())
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (attributes.Length == 0)
					continue;

				yield return new KeyValuePair<string, string>(fi.Name, attributes[0].Description);
			}
		}

		/// <summary>
		/// Silverlight 4에는 Enum.GetValues 메쏘드가 존재하지 않아 GetFields 메쏘드를 사용해 같은 기능을 구현함.
		/// </summary>
		public static IEnumerable<T> GetAllValues<T>()
		{
			IEnumerable<FieldInfo> aFi = GetFields<T>();
			return aFi.Select(field => field.GetValue(typeof(T))).Select(value => (T)value);
		}

		public static string GetDescriptionByValue<T>(T EnumValue)
		{
			FieldInfo fi = typeof(T).GetField(EnumValue.ToString());
			DescriptionAttribute[] attributes =
			  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : EnumValue.ToString();
		}

		/// <summary>
		/// Enum의 문자열 값으로 Enum의 Description을 리턴함.
		/// </summary>
		/// <param name="TypeOfEnum">Enum의 Type(typeof(SeatType)과 같이 사용할 수 있음)</param>
		/// <param name="EnumName"></param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// SeatType st = SeatType.Window;
		/// string s = CReflection.GetEnumDescriptionByName(st, "AnythingExceptSeatNearBathroom"); //"Anything Except Seat Near Bathroom"
		/// </code>
		/// </example>
		public static string GetDescriptionByName<T>(string EnumName, string DefaultValue)
		{
			foreach (FieldInfo fi in CEnum.GetFields<T>())
			{
				if (fi.Name == EnumName)
				{
					DescriptionAttribute[] attributes =
					  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
					if (attributes.Length > 0)
						return attributes[0].Description;
				}
			}

			return DefaultValue;
		}
		public static string GetDescriptionByName<T>(string EnumName)
		{
			return GetDescriptionByName<T>(EnumName, string.Empty);
		}
		

		/// <example>
		/// <code>
		/// SeatType st = (SeatType)CReflection.GetEnumValue(typeof(SeatType), "Aisle");
		/// Console.WriteLine(st.ToString()); //"Aisle"
		/// </code>
		/// </example>
		public static T GetValueByName<T>(string EnumName, T DefaultValue)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), EnumName, false);
			}
			catch (Exception)
			{
				return DefaultValue;
			}
		}
		public static T GetValueByName<T>(string EnumName)
		{
			return (T)Enum.Parse(typeof(T), EnumName, false);
		}

		/// <summary>
		/// Int32 값을 Enum 값으로 변환해서 리턴함.
		/// 실제로 (SiteKinds)1과 같은 방법으로 쓰면 되나, Enum 형식을 DB에서 불러올 때
		/// 어떤 형식인 지 알 수 없으므로 자동화시킬 수 없고,
		/// Convert.ChangeType으로는 에러 나서 만듦.
		/// </summary>
		/// <param name="TypeOfEnum"></param>
		/// <param name="EnumValue"></param>
		/// <returns></returns>
		public static T GetValueByInt32<T>(int EnumValue, T DefaultValue)
		{
			if (Enum.IsDefined(typeof(T), EnumValue))
				return (T)Enum.Parse(typeof(T), EnumValue.ToString(), false);

			return DefaultValue;
		}
		public static T GetValueByInt32<T>(int EnumValue)
		{
			if (Enum.IsDefined(typeof(T), EnumValue))
				return (T)Enum.Parse(typeof(T), EnumValue.ToString(), false);

			throw new Exception(string.Format("Wrong EnumValue:{0}", EnumValue));
		}

		public static T GetValueByNameOrInt32<T>(string NameOrInt32, T DefaultValue)
		{
			try
			{
				return (T)Enum.Parse(typeof(T), NameOrInt32, false);
			}
			catch (Exception)
			{
				return DefaultValue;
			}
		}
		public static T GetValueByNameOrInt32<T>(string NameOrInt32)
		{
			return (T)Enum.Parse(typeof(T), NameOrInt32, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TypeOfEnum"></param>
		/// <param name="Description"></param>
		/// <example>
		/// <code>
		/// SeatType st = (SeatType)CReflection.GetEnumValueByDescription(typeof(SeatType), "Anything Except Seat Near Bathroom");
		/// Console.WriteLine(st.ToString()); //"AnythingExceptSeatNearBathroom"
		/// </code>
		/// </example>
		/// <returns></returns>
		public static T GetValueByDescription<T>(string Description, T DefaultValue)
		{
			foreach (KeyValuePair<string, string> kv in GetNameDescriptionHasDescription<T>())
			{
				string NameCur = kv.Key;
				string DescriptionCur = kv.Value;
				if (DescriptionCur == Description)
					return GetValueByName<T>(NameCur);
			}

			return DefaultValue;
		}
		public static T GetValueByDescription<T>(string Description)
		{
			foreach (KeyValuePair<string, string> kv in GetNameDescriptionHasDescription<T>())
			{
				string NameCur = kv.Key;
				string DescriptionCur = kv.Value;
				if (DescriptionCur == Description)
					return GetValueByName<T>(NameCur);
			}

			throw new Exception(string.Format("Wrong Description:{0}", Description));
		}

		/// <summary>
		/// Enum의 모든 Name을 배열 형식으로 리턴함.
		/// </summary>
		/// <param name="TypeOfEnum">Enum의 Type(typeof(SeatType)과 같이 사용할 수 있음)</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// SeatType st = SeatType.Window;
		/// string[] a = CReflection.GetAllEnumName(st);
		/// Console.WriteLine(a[1]); // "Aisle"
		/// </code>
		/// </example>
		public static IEnumerable<string> GetAllName<T>()
		{
			foreach (FieldInfo fi in GetFields<T>())
			{
				yield return fi.Name;
			}
		}

		/// <summary>
		/// Enum의 모든 Description을 배열 형식으로 리턴함.
		/// </summary>
		/// <param name="TypeOfEnum">Enum의 Type(typeof(SeatType)과 같이 사용할 수 있음)</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// SeatType st = SeatType.Window;
		/// string[] a = CReflection.GetAllEnumDescription(st);
		/// Console.WriteLine(a[2]); // "Anything Except Seat Near Bathroom"
		/// </code>
		/// </example>
		public static IEnumerable<string> GetAllDescription<T>()
		{
			foreach (FieldInfo fi in GetFields<T>())
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				yield return (attributes.Length > 0) ? attributes[0].Description : fi.Name;
			}
		}

		public static IEnumerable<KeyValuePair<string, string>> GetAllNameDescription<T>()
		{
			Dictionary<string, string> dicNameDesc = new Dictionary<string, string>();

			foreach (FieldInfo fi in GetFields<T>())
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				string Name = fi.Name;
				string Desc = (attributes.Length > 0) ? attributes[0].Description : Name;

				yield return new KeyValuePair<string, string>(Name, Desc);
			}
		}

		/// <summary>
		/// Enum 형식의 Value, Name, Description를 각각 "Value", "Name", "Description" 필드를 가진
		/// DataTable을 리턴함.
		/// </summary>
		/// <param name="TypeOfEnum">Enum의 Type(typeof(SeatType)과 같이 사용할 수 있음)</param>
		/// <returns>"Value", "Name", "Description" 필드를 가진 DataTable</returns>
		/// <example>
		/// 다음은 SeatType 형식을 DataTable로 바꾸어 모든 필드와 값을 출력합니다. 
		/// <code>
		/// <![CDATA[
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// DataTable dt = CEnum.GetDataTableByValueNameDescription<SeatType>();
		/// string s = CDataTable.ToString(dt, "\r\n", ",", "(null)");
		/// Console.WriteLine(s);
		/// --결과
		/// 1,Window,Window
		/// 2,Aisle,Aisle
		/// 3,AnythingExceptSeatNearBathroom,Anything Except Seat Near Bathroom
		/// ]]>
		/// </code>
		/// </example>
		public static DataTable GetDataTableByValueNameDescription<T>()
		{
			List<int> aValue = new List<int>();
			List<string> aName = new List<string>();
			List<string> aDescription = new List<string>();

			
			DataTable dt = new DataTable();
			dt.Columns.Add("Value", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("Description", typeof(string));

			foreach (FieldInfo fi in GetFields<T>())
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				string Name = fi.Name;
				string Desc = (attributes.Length > 0) ? attributes[0].Description : Name;
				
				//값에 -1, 0, 1이 있다면 -1, 0, 1 순이 아닌 0, 1, -1 순으로 정렬되므로
				//이곳에서 값을 추가함.
				foreach (T EnumValue in CEnum.GetAllValues<T>())
				{
					if (EnumValue.ToString() == Name)
					{
						aValue.Add(Convert.ToInt32(EnumValue));
						break;
					}
				}

				aName.Add(Name);
				aDescription.Add(Desc);
			}

			for (int i = 0, i2 = aValue.Count; i < i2; i++)
			{
				dt.Rows.Add(aValue[i], aName[i], aDescription[i]);
			}

			return dt;
		}

		/// <summary>
		/// Enum 형식의 Enum, Int32, Name, Description를 SEnumValueNameDescription의 배열로 리턴함.
		/// </summary>
		public static IEnumerable<Tuple<T, int, string, string>> GetAllEnumInt32NameDescription<T>()
		{
			foreach (FieldInfo fi in GetFields<T>())
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				T Value = (T)fi.GetValue(typeof(T));
				int ValueInt32 = Convert.ToInt32(Value);
				string Name = fi.Name;
				string Description = (attributes.Length > 0) ? attributes[0].Description : Name;

				yield return new Tuple<T, int, string, string>(Value, ValueInt32, Name, Description);
			}
		}
	}
}
