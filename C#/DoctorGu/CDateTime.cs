using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace DoctorGu
{
	public enum DateInterval
	{
		Day,
		DayOfYear,
		Hour,
		Minute,
		Second,
		Millisecond,
		Weekday,
		WeekOfYear,
		Month,
		Quarter,
		Year
	}

	/// <summary>
	/// 날짜, 시간 관련 기능 구현.
	/// </summary>
	public class CDateTime
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool SetSystemTime([In] ref SYSTEMTIME st);

		private struct SYSTEMTIME
		{
			public short wYear;
			public short wMonth;
			public short wDayOfWeek;
			public short wDay;
			public short wHour;
			public short wMinute;
			public short wSecond;
			public short wMilliseconds;
		}

		public static void SetSystemTimeLocal(DateTime dt)
		{
			TimeSpan ts = DateTime.Now.Subtract(DateTime.UtcNow);
			DateTime dtUtc = dt.Subtract(ts);
			SetSystemTimeUtc(dtUtc);
		}
		public static void SetSystemTimeUtc(DateTime dtUtc)
		{
			SYSTEMTIME s = new SYSTEMTIME();
			s.wYear = (short)dtUtc.Year;
			s.wMonth = (short)dtUtc.Month;
			s.wDayOfWeek = (short)dtUtc.DayOfWeek;
			s.wDay = (short)dtUtc.Day;
			s.wHour = (short)dtUtc.Hour;
			s.wMinute = (short)dtUtc.Minute;
			s.wSecond = (short)dtUtc.Second;
			s.wMilliseconds = (short)dtUtc.Millisecond;
			SetSystemTime(ref s);
		}

		/// <summary>
		/// SQL Server에서 가장 작은 날짜는 1753-1-1 이고, Oracle에서 가장 작은 날짜는 1900-1-1임
		/// SqlDateTime overflow. Must be between 1/1/1753 12:00:00 AM and 12/31/9999 11:59:59 PM. 
		/// </summary>
		public static DateTime DateMinValueForDb19000101 = new DateTime(1900, 1, 1);
		public static DateTime DateMaxValueForDb29991231 = new DateTime(2999, 12, 31);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern int GetTickCount();

		/// <summary>
		/// 문자열이 날짜 형식으로 변환될 수 있는 지 여부를 리턴함.
		/// </summary>
		/// <param name="Value">확인할 문자열</param>
		[Obsolete("Use CValid.IsDateTime")]
		public static bool IsDateTime(string Value)
		{
			try
			{
				DateTime.Parse(Value);
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 날짜 형식을 표시형식에 맞는 문자열 형식으로 변환해서 리턴함.
		/// DateTime.ToString()을 사용하는 게 더 나음.
		/// DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") //2003-04-07 18:54:17로 표시됨. 
		/// MSDN의 Custom DateTime Format Strings 참고할 것.
		/// http://msdn2.microsoft.com/en-us/library/8kb3ddd4(VS.80,d=ide).aspx
		/// </summary>
		/// <param name="dt">DateTime 개체</param>
		/// <param name="Format">형식</param>
		/// <returns></returns>
		/// <example>
		/// Console.WriteLine(GetDateTimeFormat(DateTime.Parse("2002-2-2"), "YY-MM-DD")); //-> 02-02-02
		/// </example>
		public static string GetDateTimeFormat(DateTime dt, string Format)
		{
			string Year = dt.Year.ToString();
			string Month = dt.Month.ToString();
			string Day = dt.Day.ToString();
			string Hour = dt.Hour.ToString();
			string Minute = dt.Minute.ToString();
			string Second = dt.Second.ToString();

			string Year00 = Year.Substring(2, 2);
			string Month00 = Month.PadLeft(2, '0');
			string Day00 = Day.PadLeft(2, '0');
			string Hour00 = Hour.PadLeft(2, '0');
			string Minute00 = Minute.PadLeft(2, '0');
			string Second00 = Second.PadLeft(2, '0');

			string s = "";
			switch(Format.ToLower())
			{	
				case "yyyy-mm-dd":
					s = Year + "-" + Month00 + "-" + Day00;
					break;
				case "yy-mm-dd":
					s = Year00 + "-" + Month00 + "-" + Day00;
					break;
				case "yyyy-mm-dd hh:nn:ss":
					s = Year + "-" + Month00 + "-" + Day00 
						+ " " + Hour00 + ":" + Minute00 + ":" + Second00;
					break;
				case "yyyymmdd":
					s = Year + Month00 + Day00;
					break;
				case "hh:nn:ss":
					s = Hour00 + ":" + Minute00 + ":" + Second00;
					break;
				case "yyyymmddhhnnss":
					s = Year + Month00 + Day00 + Hour00 + Minute00 + Second00;
					break;
				case "yymmddhhnnss":
					s = Year00 + Month00 + Day00 + Hour00 + Minute00 + Second00;
					break;
				case "yy-mmdd":
					s = Year00 + "-" + Month00 + Day00;
					break;
				case "yyyy년 mm월 dd일":
					s = Year + "년 " + Month00 + "월 " + Day00 + "일";
					break;
				case "yyyy년mm월dd일":
					s = Year + "년" + Month00 + "월" + Day00 + "일";
					break;
				case "yyyy년m월d일":
					s = Year + "년" + Month + "월" + Day + "일";
					break;
				case "yyyy년 m월 d일":
					s = Year + "년 " + Month + "월 " + Day + "일";
					break;
				default:
					throw new Exception(Format + " 값이 없습니다.");
			}

			return s;
		}

		/// <summary>
		/// VB의 DateSerial과 같이 연, 월, 일로 DateTime을 리턴함.
		/// new DateTime(Year, Month, Day)는 Month나 Day의 형식이 틀리면 에러를 내므로
		/// DateSerial 함수를 만듦.
		/// </summary>
		/// <param name="Year">연</param>
		/// <param name="Month">월</param>
		/// <param name="Day">일</param>
		/// <returns>DateTime 개체</returns>
		/// <example>
		/// <code>
		/// Console.WriteLine(CDateTime.DateSerial(2002, 2, 95)); //-> 2002-05-06
		/// </code>
		/// </example>
		public static DateTime DateSerial(int Year, int Month, int Days)
		{
			DateTime d = new DateTime(1, 1, 1);
			d = d.AddYears(Year - 1);
			d = d.AddMonths(Month - 1);
			d = d.AddDays(Days - 1);
			return d;
		}

		/// <summary>
		/// 20001231150314 형식의 문자열을 DateTime 형식으로 변환해서 리턴함.
		/// </summary>
		/// <param name="YYYYMMDDHHNNSS">연월일시분초(예: 20001231150314)</param>
		/// <returns>변환된 DateTime 개체</returns>
		/// <example>
		/// Console.WriteLine(CDateTime.GetDateFrom_yyyyMMddHHmmss("20001231010203").ToString("yyyy-MM-dd")); //-> 2000-12-31
		/// </example>
		public static DateTime GetDateFrom_yyyyMMddHHmmss(string yyyyMMddHHmmss)
		{		
			if (yyyyMMddHHmmss.Length != 14)
			{
				throw new Exception("14자리만 가능합니다");
			}
			
			DateTime d = new DateTime(
				Convert.ToInt32(yyyyMMddHHmmss.Substring(0, 4)),
				Convert.ToInt32(yyyyMMddHHmmss.Substring(4, 2)),
				Convert.ToInt32(yyyyMMddHHmmss.Substring(6, 2)),
				Convert.ToInt32(yyyyMMddHHmmss.Substring(8, 2)),
				Convert.ToInt32(yyyyMMddHHmmss.Substring(10, 2)),
				Convert.ToInt32(yyyyMMddHHmmss.Substring(12, 2)));

			return d;
		}

		/// <summary>
		/// 2000-12-31 15:03:14 형식의 문자열을 DateTime 형식으로 변환해서 리턴함.
		/// </summary>
		/// <param name="YYYY_MM_DD_HH_NN_SS">연-월-일 시:분:초(예: 2000-12-31 15:03:14)</param>
		/// <returns>변환된 DateTime 개체</returns>
		/// <example>
		/// Console.WriteLine(CDateTime.GetDateFrom_yyyyMMddHHmmss("2000-12-31 15:03:14)").ToString("yyyy-MM-dd")); //-> 2000-12-31
		/// </example>
		public static DateTime GetDateFromYYYY_MM_DD_HH_NN_SS(string YYYY_MM_DD_HH_NN_SS)
		{
			if (YYYY_MM_DD_HH_NN_SS.Length != 19)
			{
				throw new Exception("19자리만 가능합니다");
			}

			DateTime d = new DateTime(
				Convert.ToInt32(YYYY_MM_DD_HH_NN_SS.Substring(0, 4)),
				Convert.ToInt32(YYYY_MM_DD_HH_NN_SS.Substring(5, 2)),
				Convert.ToInt32(YYYY_MM_DD_HH_NN_SS.Substring(8, 2)),
				Convert.ToInt32(YYYY_MM_DD_HH_NN_SS.Substring(11, 2)),
				Convert.ToInt32(YYYY_MM_DD_HH_NN_SS.Substring(14, 2)),
				Convert.ToInt32(YYYY_MM_DD_HH_NN_SS.Substring(17, 2)));

			return d;
		}

		public static DateTime GetDateFromYYYY_MM_DD(string YYYY_MM_DD)
		{
			if (YYYY_MM_DD.Length != 10)
			{
				throw new Exception("10자리만 가능합니다");
			}

			DateTime d = new DateTime(
				Convert.ToInt32(YYYY_MM_DD.Substring(0, 4)),
				Convert.ToInt32(YYYY_MM_DD.Substring(5, 2)),
				Convert.ToInt32(YYYY_MM_DD.Substring(8, 2))
				);

			return d;
		}

		/// <summary>
		/// 20051231 형식의 문자열을 DateTime 형식으로 변환해서 리턴함.
		/// </summary>
		/// <param name="YYYYMMDD">연월일(예: 20061231)</param>
		/// <returns>변환된 DateTime 개체</returns>
		/// <example>
		/// Console.WriteLine(CDateTime.GetDateFromYYYYMMDD("20001231").ToString("yyyy-MM-dd")); //-> 2000-12-31
		/// </example>
		public static DateTime GetDateFromYYYYMMDD(string YYYYMMDD)
		{		
			if (YYYYMMDD.Length != 8)
			{
				throw new Exception("8자리만 가능합니다");
			}
			
			DateTime d = new DateTime(
				Convert.ToInt32(YYYYMMDD.Substring(0, 4)),
				Convert.ToInt32(YYYYMMDD.Substring(4, 2)),
				Convert.ToInt32(YYYYMMDD.Substring(6, 2)));

			return d;
		}

		/// <summary>
		/// 윈도우가 시작된 후부터 경과된 시간을 DateTime 형식으로 리턴함.
		/// </summary>
		/// <returns>경과된 시간 정보를 가진 DateTime 개체</returns>
		/// <example>
		/// 다음은 현재 시간이 오후 5시 30분이고, 컴퓨터를 켠 시간 9시 40분 정도인 경우입니다.
		/// <code>
		/// DateTime dt = CDateTime.GetHowLongFromWindowStart();
		/// Console.WriteLine(dt); //-> 0001-01-01 오전 7:50:53
		/// </code>
		/// </example>
		public static DateTime GetHowLongFromWindowStart()
		{
			int Tick = GetTickCount();
			int d = (int)Math.Round((double)(Tick / 86400000), 0);
			int h = (int)Math.Round((double)((Tick % 86400000) / 3600000), 0);
			int m = (int)Math.Round((double)(((Tick % 86400000) % 3600000) / 60000), 0);
			int s = (int)Math.Round((double)((((Tick % 86400000) % 3600000) % 60000) / 1000), 0);
			DateTime dt = new DateTime(0);
			dt = dt.AddDays(d);
			dt = dt.AddHours(h);
			dt = dt.AddMinutes(m);
			dt = dt.AddSeconds(s);
			return dt;
		}
		/// <summary>
		/// 윈도우가 시작된 후부터 경과된 시간을 분 단위로 리턴함.
		/// </summary>
		/// <returns>분이 단위인 경과된 시간</returns>
		/// <example>
		/// 다음은 현재 시간이 오후 5시 34분이고, 컴퓨터를 켠 시간 9시 40분 정도인 경우입니다.
		/// <code>
		/// int Min = CDateTime.GetHowLongMinutesFromWindowStart();
		/// Console.WriteLine(Min); //-> 474
		/// </code>
		/// </example>
		public static int GetHowLongMinutesFromWindowStart()
		{
			int Tick = GetTickCount();
			return (int)Math.Round((double)(Tick / 60000), 0);
		}

		/// <summary>
		/// 유일한 값으로 이용하기 위해 현재 시간을 가장 상세하게 표시한 값을 리턴함.
		/// </summary>
		/// <returns>연부터 초의 1 / 1,000,000 까지 표현된 문자열</returns>
		/// <example>
		/// <code>
		/// 다음은 연속적으로 출력된 값이지만 시간의 차이로 다른 결과로 표시됩니다.
		/// <code>
		/// Console.WriteLine(CDateTime.GetUniqueIdByNow()); //200606021740266093750
		/// Console.WriteLine(CDateTime.GetUniqueIdByNow()); //200606021740267031250
		/// </code>
		/// </code>
		/// </example>
		public static string GetUniqueIdByNow()
		{
			string s = DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
			return s;
		}

		/// <summary>
		/// 특정 날짜와 날짜 사이의 경과된 일수를 리턴함.
		/// </summary>
		/// <param name="DateSmall">빠른 날짜</param>
		/// <param name="DateLarge">느린 날짜</param>
		/// <returns>경과된 일수</returns>
		/// <example>
		/// int Days = CDateTime.DaysBetween(new DateTime(2006, 1, 2), new DateTime(2007, 1, 2));
		/// Console.WriteLine(Days); //365
		/// </example>
		public static int DaysBetween(DateTime DateSmall, DateTime DateLarge)
		{
			TimeSpan TS = new TimeSpan(DateLarge.Ticks - DateSmall.Ticks);
			return (int)TS.TotalDays;
		}

		/* 제대로 동작하지 않아 주석.
		public static int DateDiff(string Interval, System.DateTime Date1, System.DateTime Date2)
		{
			double diff = 0;

			System.TimeSpan TS = new System.TimeSpan(Date2.Ticks - Date1.Ticks);
			switch (Interval.ToLower())
			{
				case "yyyy":
					diff = Convert.ToDouble(TS.TotalDays / 365);
					break;
				case "q":
					diff = Convert.ToDouble(TS.TotalDays / 365) * 4;
					break;
				case "m":
					diff = Convert.ToDouble(TS.TotalDays / 365) * 12;
					break;
				case "d":
					diff = Convert.ToDouble(TS.TotalDays);
					break;
				case "h":
					diff = Convert.ToDouble(TS.TotalHours);
					break;
				case "n":
					diff = Convert.ToDouble(TS.TotalMinutes);
					break;
				case "s":
					diff = Convert.ToDouble(TS.TotalSeconds);
					break;
				case "t":
					diff = Convert.ToDouble(TS.Ticks);
					break;
				case "mm":
					diff = Convert.ToDouble(TS.TotalMilliseconds);
					break;
				default:
					throw new Exception(Interval + "은 허용되지 않습니다.");
			}

			return (int)CMath.RoundDown(diff, 0);
		}
		*/

		/// <summary>
		/// 태어난 날짜와 현재 날짜 사이의 경과 기간을 계산해서 만 나이를 리턴함.
		/// </summary>
		/// <param name="Birth">태어난 날짜</param>
		/// <returns>만으로 계산된 나이</returns>
		/// <example>
		/// 다음은 현재 날짜가 2006년 6월 2일일 경우 2005년 6월 3일 생과 2005년 6월 4일 생의 나이를 각각 출력합니다.
		/// <code>
		/// Console.WriteLine(CDateTime.GetAge(new DateTime(2005, 6, 3))); //1
		/// Console.WriteLine(CDateTime.GetAge(new DateTime(2005, 6, 4))); //0
		/// </code>
		/// </example>
		public static int GetAge(DateTime Birth)
		{
			int YearFrom = Birth.Year;
			int YearTo = DateTime.Now.Year;
			int YearDiff = YearTo - YearFrom;

			if (Birth.Month > DateTime.Now.Month)
			{
				YearDiff--;
			}
			else if (Birth.Month == DateTime.Now.Month)
			{
				if ((Birth.Day - 1) > DateTime.Now.Day)
				{
					YearDiff--;
				}
			}

			return YearDiff;
		}

		/// <summary>
		/// 주민등록번호를 분석해서 생년월일을 리턴함.
		/// </summary>
		/// <param name="Jumin">주민등록번호</param>
		/// <returns>DateTime 형식의 생년월일</returns>
		/// <example>
		/// 다음은 생년월일이 1999년 12월 31일, 2000년 1월 1일인 경우의 생년월일을 각각 출력합니다.
		/// <code>
		/// Console.WriteLine(CDateTime.GetDateFromJumin("9912311234567")); //1999-12-31 오전 12:00:00
		/// Console.WriteLine(CDateTime.GetDateFromJumin("0001013234567")); //2000-01-01 오전 12:00:00
		/// </code>
		/// </example>
		public static DateTime GetDateFromJumin(string Jumin)
		{
			if (Jumin.IndexOf("-") != -1)
			{
				Jumin = Jumin.Replace("-", "");
			}

			int i = Convert.ToInt32(Jumin.Substring(6, 1));
			string yy = (i <= 2) ? "19" : "20";
			int y = Convert.ToInt32(yy + Jumin.Substring(0, 2));
			int m = Convert.ToInt32(Jumin.Substring(2, 2));
			int d = Convert.ToInt32(Jumin.Substring(4, 2));

			return new DateTime(y, m, d);
		}

		/// <summary>
		/// 주민등록번호의 나이가 현재 날짜를 기준으로 만 <paramref name="Age"/>세 미만인 지 여부를 리턴함.
		/// </summary>
		/// <param name="Jumin">주민등록번호</param>
		/// <param name="Age">만 기준 나이</param>
		/// <returns><paramref name="Age"/>세 미만인 지 여부</returns>
		/// <example>
		/// <code>
		/// 다음은 현재 날짜가 2006년 6월 2일인 경우엔 "14세 미만"을 출력하고,
		/// 현재 날짜가 2006년 6월 1일인 경우엔 "14세 이상"을 출력합니다.
		/// bool IsLess14 = CDateTime.IsJuminYearLessThan("9206041234567", 14);
		/// if (IsLess14)
		/// {
		///	 Console.WriteLine("14세 미만");
		/// }
		/// else
		/// {
		///	 Console.WriteLine("14세 이상");
		/// }
		/// </code>
		/// </example>
		public static bool IsJuminYearLessThan(string Jumin, int Age)
		{
			return (GetAgeFromJumin(Jumin) < Age);
		}

		/// <summary>
		/// 주민등록번호를 분석해서 현재 날짜를 기준으로 만 나이를 계산해서 리턴함.
		/// </summary>
		/// <param name="Jumin">주민등록번호</param>
		/// <returns>만으로 계산된 나이</returns>
		/// <example>
		/// <code>
		/// 다음은 현재 날짜가 2006년 6월 2일인 경우에 현재 날짜를 기준으로 계산된 만 나이를 리턴함.
		/// Console.WriteLine(CDateTime.GetAgeFromJumin("9006031234567")); //16
		/// Console.WriteLine(CDateTime.GetAgeFromJumin("9006041234567")); //15
		/// </code>
		/// </example>
		public static int GetAgeFromJumin(string Jumin)
		{
			DateTime Birth = GetDateFromJumin(Jumin);
			return GetAge(Birth);
		}

		public static string GetWeekNameHangul(DayOfWeek w, bool IsFull)
		{
			string s = "";
			switch (w)
			{
				case DayOfWeek.Monday:
					s = "월";
					break;
				case DayOfWeek.Tuesday:
					s = "화";
					break;
				case DayOfWeek.Wednesday:
					s = "수";
					break;
				case DayOfWeek.Thursday:
					s = "목";
					break;
				case DayOfWeek.Friday:
					s = "금";
					break;
				case DayOfWeek.Saturday:
					s = "토";
					break;
				case DayOfWeek.Sunday:
					s = "일";
					break;
				default:
					break;
			}

			if (IsFull)
				s += "요일";

			return s;
		}
		public static string GetWeekNameHangul(DateTime dt, bool IsFull)
		{
			return GetWeekNameHangul(dt.DayOfWeek, IsFull);
		}
		public static string GetWeekNameHangul(DateTime dt)
		{
			bool IsFull = false;
			return GetWeekNameHangul(dt.DayOfWeek, IsFull);
		}

		public static DateTime DateAdd(DateInterval interval, int val, DateTime dt)
        {
			if (interval == DateInterval.Year)
				return dt.AddYears(val);
			else if (interval == DateInterval.Month)
				return dt.AddMonths(val);
			else if (interval == DateInterval.Day)
				return dt.AddDays(val);
			else if (interval == DateInterval.Hour)
				return dt.AddHours(val);
			else if (interval == DateInterval.Minute)
				return dt.AddMinutes(val);
			else if (interval == DateInterval.Second)
				return dt.AddSeconds(val);
			else if (interval == DateInterval.Millisecond)
				return dt.AddMilliseconds(val);
			else if (interval == DateInterval.Quarter)
				return dt.AddMonths(val * 3);
			else
				return dt;
        }
		/// <summary>
		/// VB의 DateAdd와 같이 일수를 더한 값을 DateTime 형식으로 리턴함.
		/// </summary>
		/// <param name="Days">더할 일수</param>
		/// <param name="Date">더할 대상 날짜</param>
		/// <returns><paramref name="Days"/>가 더해진 날짜</returns>
		/// <example>
		/// <code>
		/// DateTime d = new DateTime(2006, 12, 31);
		/// d = CDateTime.DateAdd(31, d); //2007년 1월 31일 오전 12:00:00
		/// Console.WriteLine(d.ToString());
		/// </code>
		/// </example>
		public static DateTime DateAdd(int Days, DateTime dt)
		{
			//TimeSpan ts = new TimeSpan(Days, 0, 0, 0);
			//return Date + ts;

			return DateAdd(DateInterval.Day, Days, dt);
		}

        public static int DateDiff(DateInterval interval, DateTime dt1, DateTime dt2, DayOfWeek eFirstDayOfWeek)
        {
            if (interval == DateInterval.Year)
                return dt2.Year - dt1.Year;
			else if (interval == DateInterval.Month)
                return (dt2.Month - dt1.Month) + (12 * (dt2.Year - dt1.Year));


            TimeSpan ts = dt2 - dt1;

			switch (interval)
			{
				case DateInterval.Day:
				case DateInterval.DayOfYear:
					return CMath.RoundDownIgnoreSign(ts.TotalDays);
				case DateInterval.Hour:
					return CMath.RoundDownIgnoreSign(ts.TotalHours);
				case DateInterval.Minute:
					return CMath.RoundDownIgnoreSign(ts.TotalMinutes);
				case DateInterval.Second:
					return CMath.RoundDownIgnoreSign(ts.TotalSeconds);
				case DateInterval.Millisecond:
					return CMath.RoundDownIgnoreSign(ts.TotalMilliseconds);

				case DateInterval.Weekday:
					return CMath.RoundDownIgnoreSign(ts.TotalDays / 7.0);
				case DateInterval.WeekOfYear:
					while (dt2.DayOfWeek != eFirstDayOfWeek)
						dt2 = dt2.AddDays(-1);
					while (dt1.DayOfWeek != eFirstDayOfWeek)
						dt1 = dt1.AddDays(-1);
					ts = dt2 - dt1;
					return CMath.RoundDownIgnoreSign(ts.TotalDays / 7.0);

				case DateInterval.Month:
					throw new Exception("절대 이 줄 실행 안됨.");
				case DateInterval.Quarter:
					double d1Quarter = GetQuarter(dt1.Month);
					double d2Quarter = GetQuarter(dt2.Month);
					double d1 = d2Quarter - d1Quarter;
					double d2 = (4 * (dt2.Year - dt1.Year));
					return CMath.RoundDownIgnoreSign(d1 + d2);
				case DateInterval.Year:
					throw new Exception("절대 이 줄 실행 안됨.");
			}
            
            return 0;
        }
		public static int DateDiff(DateInterval interval, DateTime dt1, DateTime dt2)
		{
			return DateDiff(interval, dt1, dt2, System.Globalization.DateTimeFormatInfo.CurrentInfo.FirstDayOfWeek);
		}

		private static int GetQuarter(int nMonth)
		{
			if (nMonth <= 3)
				return 1;
			if (nMonth <= 6)
				return 2;
			if (nMonth <= 9)
				return 3;
			return 4;
		}

		/// <summary>
		/// bool 값이 true가 될 때까지 기다림.
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// private static bool _b;
		/// 
		/// static void Main()
		/// {
		///		Thread th = new Thread(ThreadTrue);
		///		th.Start();
		///		WaitUntilTrue(2000, ref _b);
		///	}
		/// 
		/// private static void ThreadTrue()
		/// {
		///		Thread.Sleep(5000);
		///		_b = true;
		///	}
		/// ]]>
		/// </example>
		public static void WaitUntilTrue(int MaxMilliseconds, ref bool ValueToBeTrue)
		{
			int nWaited = 0;
			while (!ValueToBeTrue && (nWaited < MaxMilliseconds))
			{
				nWaited += 100;
				Thread.Sleep(100);
			}
		}

		public static void WaitUntilHasValue(int MaxMilliseconds, ref string ValueToBeHasValue)
		{
			int nWaited = 0;
			while (string.IsNullOrEmpty(ValueToBeHasValue) && (nWaited < MaxMilliseconds))
			{
				nWaited += 100;
				Thread.Sleep(100);
			}
		}

		public static void WaitUntilTrueOrHasValue(int MaxMilliseconds, ref bool ValueToBeTrue, ref string ValueToBeHasValue)
		{
			int nWaited = 0;
			while (!ValueToBeTrue && string.IsNullOrEmpty(ValueToBeHasValue) && (nWaited < MaxMilliseconds))
			{
				nWaited += 100;
				Thread.Sleep(100);
			}
		}
	}
}
