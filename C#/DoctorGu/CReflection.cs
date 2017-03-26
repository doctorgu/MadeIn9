using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Data;
using System.Collections.Specialized;
using System.Collections;

namespace DoctorGu
{
	public class CEnumValueNameDescription
	{
		public Enum Value { get; set; }
		public int ValueInt32 { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	}

	public class CValueNameType
	{
		public object Value { get; set; }
		public string Name { get; set; }
		public Type Type { get; set; }

		public override string ToString()
		{
			return string.Format("{0}:{1}={2}", this.Type, this.Name, this.Value);
		}
	}

	public class CReflection
	{
		/// <summary>
		/// Enum 형식의 Description을 리턴함.
		/// </summary>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// SeatType st = SeatType.AnythingExceptSeatNearBathroom;
		/// string s = CReflection.GetEnumDescriptionByValue(st); //"Anything Except Seat Near Bathroom"
		/// </code>
		/// </example>
		public static string GetEnumDescriptionByValue(Enum EnumValue)
		{
			if (EnumValue == null)
				return "";

			FieldInfo fi = EnumValue.GetType().GetField(EnumValue.ToString());
			DescriptionAttribute[] attributes =
			  (DescriptionAttribute[])fi.GetCustomAttributes(
			  typeof(DescriptionAttribute), false);
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
		public static string GetEnumDescriptionByName(Type TypeOfEnum, string EnumName)
		{
			FieldInfo[] afi = TypeOfEnum.GetFields();
			for (int i = 0, i2 = afi.Length; i < i2; i++)
			{
				if (afi[i].Name == EnumName)
				{
					DescriptionAttribute[] attributes =
					  (DescriptionAttribute[])afi[i].GetCustomAttributes(
					  typeof(DescriptionAttribute), false);
					return (attributes.Length > 0) ? attributes[0].Description : EnumName;
				}
			}

			return "";
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TypeOfEnum"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// SeatType st = (SeatType)CReflection.GetEnumValue(typeof(SeatType), "Aisle");
		/// Console.WriteLine(st.ToString()); //"Aisle"
		/// </code>
		/// </example>
		public static Enum GetEnumValueByName(Type TypeOfEnum, string Name)
		{
			Enum en = null;
			try
			{
				en = (Enum)Enum.Parse(TypeOfEnum, Name, false);
			}
			catch (Exception)
			{
				return null;
			}

			return en;
		}

		/// <summary>
		/// Int32 값을 Enum 값으로 변환해서 리턴함.
		/// 실제로 (SiteKinds)1과 같은 방법으로 쓰면 되나, Enum 형식을 DB에서 불러올 때
		/// 어떤 형식인 지 알 수 없으므로 자동화시킬 수 없고,
		/// Convert.ChangeType으로는 에러 나서 만듦.
		/// </summary>
		/// <param name="TypeOfEnum"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static Enum GetEnumValueByInt32(Type TypeOfEnum, int Value)
		{
			//잘못된 값이 넘어와도 에러는 나지 않으므로 try, catch 사용 안함.
			Enum en = (Enum)Enum.Parse(TypeOfEnum, Value.ToString(), false);
			if (Enum.IsDefined(TypeOfEnum, en))
			{
				return en;
			}
			else
			{
				return null;
			}
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
		public static Enum GetEnumValueByDescription(Type TypeOfEnum, string Description)
		{
			FieldInfo[] afi = TypeOfEnum.GetFields();

			//첫번째는 특수정보이고, 두번째부터 값이 있으므로 1번째부터 루핑
			for (int i = 1, i2 = afi.Length; i < i2; i++)
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])afi[i].GetCustomAttributes(typeof(DescriptionAttribute), false);

				if (attributes.Length == 0) continue;

				string DescCur = attributes[0].Description;
				string NameCur = afi[i].Name;

				if (DescCur == Description)
				{
					return GetEnumValueByName(TypeOfEnum, NameCur);
				}
			}

			return null;
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
		public static string[] GetAllEnumName(Type TypeOfEnum)
		{
			FieldInfo[] afi = TypeOfEnum.GetFields();
			string[] aName = new string[afi.Length - 1];

			//첫번째는 특수정보이고, 두번째부터 값이 있으므로 1번째부터 루핑
			for (int i = 1, i2 = afi.Length; i < i2; i++)
			{
				aName[i - 1] = afi[i].Name;
			}

			return aName;
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
		public static string[] GetAllEnumDescription(Type TypeOfEnum)
		{
			FieldInfo[] afi = TypeOfEnum.GetFields();
			string[] aDesc = new string[afi.Length - 1];

			//첫번째는 특수정보이고, 두번째부터 값이 있으므로 1번째부터 루핑
			for (int i = 1, i2 = afi.Length; i < i2; i++)
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])afi[i].GetCustomAttributes(
				  typeof(DescriptionAttribute), false);

				aDesc[i - 1] = (attributes.Length > 0) ? attributes[0].Description : afi[i].Name;
			}

			return aDesc;
		}

		public static Dictionary<string, string> GetAllEnumNameDescription(Type TypeOfEnum)
		{
			FieldInfo[] afi = TypeOfEnum.GetFields();
			Dictionary<string, string> dicNameDesc = new Dictionary<string, string>();

			//첫번째는 특수정보이고, 두번째부터 값이 있으므로 1번째부터 루핑
			for (int i = 1, i2 = afi.Length; i < i2; i++)
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])afi[i].GetCustomAttributes(
				  typeof(DescriptionAttribute), false);

				string Name = afi[i].Name;
				string Desc = (attributes.Length > 0) ? attributes[0].Description : Name;

				dicNameDesc.Add(Name, Desc);
			}

			return dicNameDesc;
		}

		/// <summary>
		/// System.Enum 형식의 모든 값을 배열로 리턴함.
		/// </summary>
		/// <param name="TypeOfEnum">Enum의 Type(typeof(SeatType)과 같이 사용할 수 있음)</param>
		/// <returns>System.Enum 형식의 모든 값</returns>
		/// <example>
		/// 다음은 SeatType의 모든 값(1, 2, 3)을 가져온 후, String 형식으로 변환해서 출력합니다.
		/// <code>
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// 
		/// Enum[] ai = CReflection.GetAllEnumValues(typeof(SeatType));
		/// foreach (Enum i in ai)
		/// {
		///	 SeatType Cur = (SeatType)i;
		///	 Console.WriteLine(Cur.ToString());
		/// }
		/// --결과
		/// Window
		/// Aisle
		/// AnythingExceptSeatNearBathroom
		/// </code>
		/// </example>
		public static Enum[] GetAllEnumValues(Type TypeOfEnum)
		{
			List<Enum> aValue = new List<Enum>();

			foreach (Enum en in Enum.GetValues(TypeOfEnum))
			{
				aValue.Add(en);
			}

			return aValue.ToArray();
		}
		//http://devlicio.us/blogs/joe_niland/archive/2006/10/10/Generic-Enum-to-List_3C00_T_3E00_-converter.aspx
		/// <summary>
		/// 
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// List<StatusType> a = CReflection.GetAllEnumValues<StatusType>();
		/// ]]>
		/// </example>
		public static List<T> GetAllEnumValues<T>()
		{
			Type EnumType = typeof(T);

			// Can't use type constraints on value types, so have to do check like this
			if (EnumType.BaseType != typeof(Enum))
				throw new ArgumentException("T must be of type System.Enum");

			return new List<T>(Enum.GetValues(EnumType) as IEnumerable<T>);
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
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// DataTable dt = CReflection.GetDataTableByEnumValueNameDescription(typeof(SeatType));
		/// string s= CDataTable.GetString(dt, "\r\n", ",", "(null)");
		/// Console.WriteLine(s);
		/// --결과
		/// 1,Window,Window
		/// 2,Aisle,Aisle
		/// 3,AnythingExceptSeatNearBathroom,Anything Except Seat Near Bathroom
		/// </code>
		/// </example>
		public static DataTable GetDataTableByEnumValueNameDescription(Type TypeOfEnum)
		{
			List<int> aValue = new List<int>();
			List<string> aName = new List<string>();
			List<string> aDescription = new List<string>();


			DataTable dt = new DataTable();
			dt.Columns.Add("Value", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("Description", typeof(string));

			FieldInfo[] afi = TypeOfEnum.GetFields();
			//첫번째는 특수정보이고, 두번째부터 값이 있으므로 1번째부터 루핑
			for (int i = 1, i2 = afi.Length; i < i2; i++)
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])afi[i].GetCustomAttributes(
				  typeof(DescriptionAttribute), false);

				string Name = afi[i].Name;
				string Desc = (attributes.Length > 0) ? attributes[0].Description : Name;

				//값에 -1, 0, 1이 있다면 -1, 0, 1 순이 아닌 0, 1, -1 순으로 정렬되므로
				//이곳에서 값을 추가함.
				foreach (Enum en in Enum.GetValues(TypeOfEnum))
				{
					if (en.ToString() == Name)
					{
						aValue.Add(Convert.ToInt32(en));
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
		/// 특정 개체의 모든 Public Member의 이름과 값을 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static string GetPublicNameValueListInHtml(object Value)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("<table border=1 cellpadding=0 cellspacing=0>");

			FieldInfo[] afi = Value.GetType().GetFields();
			foreach (FieldInfo fi in afi)
			{
				if (!fi.IsPublic)
					continue;

				object ValueCur = fi.GetValue(Value);
				if (ValueCur == null)
				{
					ValueCur = "(null)";
				}

				sb.Append("<tr><td>" + fi.Name + "</td><td>" + ValueCur.ToString() + "</tr>");
			}
			sb.Append("</table>");

			return sb.ToString();
		}
		public static string GetPropertiesInHtml(object Value)
		{
			if (Value == null)
				return "(null)";

			StringBuilder sb = new StringBuilder();
			sb.Append("<table border=1 cellpadding=0 cellspacing=0>");

			PropertyInfo[] api = Value.GetType().GetProperties();
			foreach (PropertyInfo pi in api)
			{
				object oValueCur = pi.GetValue(Value, null);
				string ValueCur = "";
				if (oValueCur == null)
				{
					ValueCur = "(null)";
				}
				if (oValueCur.GetType() == typeof(string[]))
				{
					ValueCur = CArray.ToHtml((string[])oValueCur);
				}
				else
				{
					ValueCur = oValueCur.ToString();
				}

				if (ValueCur == "") ValueCur = "&nbsp;";

				sb.Append("<tr><td>" + pi.Name + "</td><td>" + ValueCur + "</tr>");
			}
			sb.Append("</table>");

			return sb.ToString();
		}

		/// <summary>
		/// 특정 개체의 모든 Public Member의 Description 속성의 값과 실제 값을 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static string GetPublicFieldDescriptionValueList(object Value)
		{
			string s = "";

			FieldInfo[] afi = Value.GetType().GetFields();
			foreach (FieldInfo fi in afi)
			{
				if (!fi.IsPublic)
					continue;

				object ValueCur = fi.GetValue(Value);
				if (ValueCur == null)
				{
					ValueCur = "(null)";
				}

				DescriptionAttribute[] AttrCur =
				(DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

				string Desc = (AttrCur.Length > 0) ? AttrCur[0].Description : fi.Name;

				s += Desc + ": " + ValueCur.ToString() + "\r\n";
			}

			return s;
		}

		public static List<string> GetPublicFieldPropertyNameList(Type TypeOfObject)
		{
			List<string> aName = new List<string>();

			FieldInfo[] afi = TypeOfObject.GetFields();
			foreach (FieldInfo fi in afi)
			{
				if (!fi.IsPublic)
					continue;

				aName.Add(fi.Name);
			}

			PropertyInfo[] api = TypeOfObject.GetProperties();
			//마지막엔 내부에서만 쓰이는 Item이므로 제외함.
			for (int i = 0; i < (api.Length - 1); i++)
			{
				aName.Add(api[i].Name);
			}

			return aName;
		}
		public static Dictionary<string, Type> GetPublicFieldPropertyNameTypeDictionary(Type TypeOfObject)
		{
			Dictionary<string, Type> dicNameType = new Dictionary<string, Type>();

			FieldInfo[] afi = TypeOfObject.GetFields();
			foreach (FieldInfo fi in afi)
			{
				if (!fi.IsPublic)
					continue;

				string Name = fi.Name;
				Type Type = fi.FieldType;

				dicNameType.Add(Name, Type);
			}

			PropertyInfo[] api = TypeOfObject.GetProperties();
			//마지막엔 내부에서만 쓰이는 Item이므로 제외함.
			for (int i = 0; i < (api.Length - 1); i++)
			{
				PropertyInfo pi = api[i];

				string Name = pi.Name;
				Type Type = pi.PropertyType;

				dicNameType.Add(Name, Type);
			}

			return dicNameType;
		}

		public static Dictionary<string, object> GetPublicFieldPropertyNameValueDictionary(object Obj)
		{
			Dictionary<string, object> dicNameValue = new Dictionary<string, object>();

			FieldInfo[] afi = Obj.GetType().GetFields();
			foreach (FieldInfo fi in afi)
			{
				if (!fi.IsPublic)
					continue;

				string Name = fi.Name;
				object Value = fi.GetValue(Obj);

				dicNameValue.Add(Name, Value);
			}

			PropertyInfo[] api = Obj.GetType().GetProperties();
			//마지막엔 내부에서만 쓰이는 Item이므로 제외함.
			for (int i = 0; i < (api.Length - 1); i++)
			{
				PropertyInfo pi = api[i];

				string Name = pi.Name;
				object Value = pi.GetValue(Obj, null);

				dicNameValue.Add(Name, Value);
			}

			return dicNameValue;
		}

		public static void SetPublicFieldPropertyValue(object Obj, string Name, object Value)
		{
			FieldInfo[] afi = Obj.GetType().GetFields();
			foreach (FieldInfo fi in afi)
			{
				if (!fi.IsPublic)
					continue;

				if (fi.Name != Name)
					continue;

				fi.SetValue(Obj, Value);
				return;
			}

			PropertyInfo[] api = Obj.GetType().GetProperties();
			foreach (PropertyInfo pi in api)
			{
				if (pi.Name != Name)
					continue;

				pi.SetValue(Obj, Value, null);
				return;
			}
		}

		/// <summary>
		/// <paramref name="EnumValueOrText"/>가 <paramref name="TypeOfEnum"/>의 Value 또는 Name인 경우엔
		/// 해당 Value를 리턴하고, 그렇지 않으면 DefaultValue를 리턴함.
		/// </summary>
		/// <remarks>
		/// Parse 메쏘드 사용시 목록에 없는 문자열일 경우 에러 발생하나
		/// 목록에 없는 숫자일 경우 int 범위 안에만 있으면 에러 발생하지 않음.
		/// <code>
		/// Enum.Parse(TypeOfEnum, "-999"); // -> -999 리턴
		/// Enum.Parse(TypeOfEnum, "뷃"); // -> 에러 발생
		/// </code>
		/// </remarks>
		/// <param name="EnumValue"></param>
		/// <param name="TypeOfEnum"></param>
		/// <param name="DefaultValue"></param>
		/// <returns></returns>
		public static Enum IfNotValueOrTextThen(string EnumValueOrText, Type TypeOfEnum, Enum DefaultValue)
		{
			object tmp = null;
			try
			{
				tmp = Enum.Parse(TypeOfEnum, EnumValueOrText, false);
			}
			catch (Exception)
			{
				return DefaultValue;
			}

			if (!Enum.IsDefined(TypeOfEnum, tmp))
				return DefaultValue;

			return (Enum)tmp;
		}

		/// <summary>
		/// Parameter가 있는 첫번째 Constructor의 정보를 리턴함.
		/// </summary>
		/// <param name="TypeOfClass">Class의 형식</param>
		/// <param name="aTypeIs">각 Parameter의 Type</param>
		/// <param name="aDescriptionIs">각 Parameter의 Description(없으면 Name)</param>
		/// <returns>Parameter가 있는 Constructor가 없으면 false를 리턴하고, 아니면 true를 리턴함.</returns>
		/// <example>
		/// 다음은 ConTest 클래스의 Constructor 정보를 출력합니다.
		/// <code>
		/// List&lt;Type&gt; aTypeIs;
		/// List&lt;string&gt; aNameIs;
		/// List&lt;string&gt; aDescriptionIs;
		///
		/// if (GetFirstConstructorParamInfo(typeof(ConTest), out aTypeIs, out aNameIs, out aDescriptionIs))
		/// {
		///	 for (int i = 0; i &lt; aTypeIs.Count; i++)
		///	 {
		///		 Console.WriteLine("Type: {0}, Name: {1}, Description: {2}",  aTypeIs[i].ToString(), aNameIs[i], aDescriptionIs[i]);
		///	 }
		/// }
		/// 
		/// public class ConTest
		/// {
		/// 	public ConTest()
		/// 	{
		/// 	}
		/// 	public ConTest(
		/// 		[Description("숫자")]
		/// 		int i,
		/// 		[Description("문자열 이야")]
		/// 		string s,
		/// 		[Description("개체(Object)")]
		/// 		object o)
		/// 	{
		/// 
		/// 	}
		/// }		
		/// </code>
		/// </example>
		public static bool GetFirstConstructorParamInfo(Type TypeOfClass,
			out List<Type> aTypeIs, out List<string> aNameIs, out List<string> aDescriptionIs)
		{
			aTypeIs = new List<Type>();
			aNameIs = new List<string>();
			aDescriptionIs = new List<string>();

			ConstructorInfo[] aConstInfo = TypeOfClass.GetConstructors();
			ParameterInfo[] aPInfo = null;
			foreach (ConstructorInfo Info in aConstInfo)
			{
				aPInfo = Info.GetParameters();
				if (aPInfo.Length > 0)
					break;
			}

			if (aPInfo.Length == 0)
				return false;

			foreach (ParameterInfo PInfo in aPInfo)
			{
				aTypeIs.Add(PInfo.ParameterType);
				aNameIs.Add(PInfo.Name);

				DescriptionAttribute[] aAttr = (DescriptionAttribute[])PInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
				string Description = (aAttr.Length > 0) ? aAttr[0].Description : PInfo.Name;

				aDescriptionIs.Add(Description);
			}

			return true;
		}

		public static string GetDescription(Type TypeOfObject)
		{
			DescriptionAttribute[] aDescAttr = (DescriptionAttribute[])TypeOfObject.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return (aDescAttr.Length > 0) ? aDescAttr[0].Description : TypeOfObject.Name;
		}

		/// <summary>
		/// Field나 Property의 값을 리턴함.
		/// </summary>
		/// <param name="Obj"></param>
		/// <param name="Name"></param>
		/// <returns></returns>
		/// <example>
		/// string Text = (string)CReflection.GetFieldOrPropertyValue(button1, "Text"); //button1
		/// </example>
		public static object GetFieldOrPropertyValue(object Obj, string Name)
		{
			FieldInfo fi = Obj.GetType().GetField(Name);
			if (fi != null)
				return fi.GetValue(Obj);

			PropertyInfo pi = Obj.GetType().GetProperty(Name);
			if (pi != null)
				return pi.GetValue(Obj, null);

			throw new Exception("Name: " + Name + "이 잘못되었습니다.");
		}

		public static void SetFieldOrPropertyValue(object Obj, string Name, object Value)
		{
			FieldInfo fi = Obj.GetType().GetField(Name);
			if (fi != null)
			{
				fi.SetValue(Obj, Value);
				return;
			}

			PropertyInfo pi = Obj.GetType().GetProperty(Name);
			if (pi != null)
			{
				pi.SetValue(Obj, Value, null);
				return;
			}

			throw new Exception("Name: " + Name + "이 잘못되었습니다.");
		}

		public static IEnumerable<CValueNameType> GetAllFieldOrPropertyNameValueList(object Obj)
		{
			List<CValueNameType> aVnt = new List<CValueNameType>();

			FieldInfo[] aFi = Obj.GetType().GetFields();
			foreach (FieldInfo fi in aFi)
			{
				yield return (new CValueNameType()
				{
					Value = fi.GetValue(Obj),
					Name = fi.Name,
					Type = fi.FieldType
				});
			}

			PropertyInfo[] aPi = Obj.GetType().GetProperties();
			foreach (PropertyInfo pi in aPi)
			{
				yield return (new CValueNameType()
				{
					Value = pi.GetValue(Obj, null),
					Name = pi.Name,
					Type = pi.PropertyType
				});
			}
		}

		/// <summary>
		/// Enum 형식의 Value, Name, Description를 SEnumValueNameDescription의 배열로 리턴함.
		/// </summary>
		/// <param name="TypeOfEnum">Enum의 Type(typeof(SeatType)과 같이 사용할 수 있음)</param>
		/// <returns>Value, Name, Description 속성을 가진 SEnumValueNameDescription</returns>
		/// <example>
		/// 다음은 SeatType 형식을 SEnumValueNameDescription로 바꾸어 모든 속성과 값을 출력합니다. 
		/// <code>
		/// <![CDATA[
		/// public enum SeatType
		/// {
		///		[Description("Window")] Window = 1,
		/// 	[Description("Aisle")] Aisle = 2,
		/// 	[Description("Anything Except Seat Near Bathroom")] AnythingExceptSeatNearBathroom
		/// }
		/// List<SEnumValueNameDescription> a = CReflection.GetAllEnumValueNameDescription(typeof(SeatType));
		/// foreach (SEnumValueNameDescription e in a)
		/// {
		///		Console.WriteLine("{0},{1},{2}", e.Value.ToString(), e.Name, e.Description);
		///	}
		/// --결과
		/// 1,Window,Window
		/// 2,Aisle,Aisle
		/// 3,AnythingExceptSeatNearBathroom,Anything Except Seat Near Bathroom
		/// ]]>
		/// </code>
		/// </example>
		public static List<CEnumValueNameDescription> GetAllEnumValueNameDescription(Type TypeOfEnum)
		{
			List<CEnumValueNameDescription> aInfo = new List<CEnumValueNameDescription>();

			FieldInfo[] afi = TypeOfEnum.GetFields();
			for (int i = 0, i2 = afi.Length; i < i2; i++)
			{
				DescriptionAttribute[] attributes =
				  (DescriptionAttribute[])afi[i].GetCustomAttributes(
				  typeof(DescriptionAttribute), false);

				//보통은 첫번째가 특수정보임. 개수가 아주 많은 경우 8번째가 특수정보인 경우 있었음.
				if (afi[i].IsSpecialName)
					continue;

				Enum Value = (Enum)afi[i].GetValue(TypeOfEnum);
				int ValueInt32 = Convert.ToInt32(Value);
				string Name = afi[i].Name;
				string Description = (attributes.Length > 0) ? attributes[0].Description : Name;

				////값에 -1, 0, 1이 있다면 -1, 0, 1 순이 아닌 0, 1, -1 순으로 정렬되므로
				////이곳에서 값을 추가함.
				//foreach (Enum en in Enum.GetValues(TypeOfEnum))
				//{
				//    if (en.ToString() == Name)
				//    {
				//        aValue.Add(Convert.ToInt32(en));
				//        break;
				//    }
				//}
				aInfo.Add(new CEnumValueNameDescription()
				{
					Value = Value,
					ValueInt32 = ValueInt32,
					Name = Name,
					Description = Description
				});
			}

			return aInfo;
		}

		/// <summary>
		/// List 안에 있는 Class나 Structure의 특정 속성의 값들을 <paramref name="DelimCol"/>로 구분한 목록으로 리턴함.
		/// </summary>
		/// <param name="aObj"></param>
		/// <param name="PropertyName"></param>
		/// <param name="DelimCol"></param>
		/// <returns></returns>
		/// <example>
		/// <![CDATA[
		/// private struct STest
		/// {
		///     public string s;
		/// }
		/// 
		/// List<STest> aTest = new List<STest>();			
		/// aTest.Add(new STest() { s = "abc" });
		/// aTest.Add(new STest() { s = "def" });
		/// 
		/// string List = CReflection.GetListOfValueOfEnumerable(aTest.Cast<object>(), "s", ','); //abc,def
		/// ]]>
		/// </example>
		public static string GetListOfValueOfEnumerable(IEnumerable<object> aObj, string PropertyName, char DelimCol, Func<Type, object, string> Converter)
		{
			string[] aPropertyName = PropertyName.Split('.');

			bool IsFound = false;
			bool IsCountMoreThan0 = false;

			string ValueList = "";
			foreach (object Obj in aObj)
			{
				IsCountMoreThan0 = true;

				FieldInfo[] afi = Obj.GetType().GetFields();
				foreach (FieldInfo fi in afi)
				{
					if (fi.Name == aPropertyName[0])
					{
						object Value = fi.GetValue(Obj);

						if (aPropertyName.Length == 1)
						{
							IsFound = true;
							ValueList += string.Concat(DelimCol, (Converter == null) ? Value : Converter(fi.FieldType, Value));

							break;
						}
						else
						{
							FieldInfo[] afi2 = Value.GetType().GetFields();
							foreach (FieldInfo fi2 in afi2)
							{
								if (fi2.Name == aPropertyName[1])
								{
									object Value2 = fi2.GetValue(Value);

									IsFound = true;
									ValueList += string.Concat(DelimCol, (Converter == null) ? Value2 : Converter(fi2.FieldType, Value2));

									break;
								}
							}
						}
					}
				}
			}

			if (IsCountMoreThan0 && !IsFound)
			{
				throw new Exception(PropertyName + " 속성을 찾을 수 없습니다.");
			}

			if (ValueList == "")
				return "";

			return ValueList.Substring(1);
		}
		public static string GetListOfValueOfEnumerable(IEnumerable<object> aObj, string PropertyName, char DelimCol)
		{
			return GetListOfValueOfEnumerable(aObj, PropertyName, DelimCol, null);
		}
	}
}
