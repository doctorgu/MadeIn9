using System;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using System.Drawing.Printing;

namespace DoctorGu
{
	/// <summary>
	/// 단위 종류(서로 다른 단위들 간에 변환하기 위함.)
	/// </summary>
	public enum ScaleModeConstants
	{
		Twips,
		Points,
		Pixels,
		Inches,
		Millimeters,
		Centimeters
	}

	public struct MarginF
	{
		public readonly float Left, Right, Top, Bottom;
		public MarginF(float Left, float Right, float Top, float Bottom)
		{
			this.Left = Left;
			this.Right = Right;
			this.Top = Top;
			this.Bottom = Bottom;
		}

		public override string ToString()
		{
			return string.Concat(this.Left, ",", this.Right, ",", this.Top, ",", this.Bottom);
		}
	}

	/// <summary>
	/// Summary description for Math.
	/// </summary>
	public class CMath
	{
		public const int FileSize1Gb = 1073741824; //(int)Math.Pow(1024, 3);
		public const int FileSize1Mb = 1048576; //(int)Math.Pow(1024, 2);
		public const int FileSize1Kb = 1024;

		private static Dictionary<PaperKind, Size> _dicPaperSize = new Dictionary<PaperKind, Size>();

		/// <summary>
		/// n진수를 10진수로 변환함. 62진수까지 가능함.
		/// </summary>
		/// <param name="Value"><paramref name="nTH"/>에서 정한 n진수 값</param>
		/// <param name="nTH">현재 진수의 단위</param>
		/// <returns>10진수로 변환된 값</returns>
		/// <example>
		/// 다음은 2006이란 10진수를 30진수로 변환하고, 다시 10진수로 변환합니다.
		/// <code>
		/// string Num30 = CMath.GetNFrom10(2006, 30);
		/// Console.WriteLine(Num30); //"26Q"
		/// 
		/// int Num10 = (int)CMath.Get10FromN(Num30, 30); //2006
		/// Console.WriteLine(Num10);
		/// </code>
		/// </example>
		public static long Get10FromN(string Value, int nTH)
		{
			//0 ~ 9, A ~ Z, a ~ z의 개수만큼만 하기 위해 62개로 한정함.
			if ((nTH < 1) || (nTH > 62))
			{
				throw new Exception("1부터 62진수까지 가능합니다");
			}

			int NumPos = Value.Length;
			long AllNum = 0;
			for (int i = 0, i2 = Value.Length; i < i2; i++)
			{
				char c = Value[i];

				long NumC = Char.IsDigit(c) ? Convert.ToInt64(c.ToString()) : Get10(c);

				//현재 자리에 해당하는 10의 n승을 곱함.
				//(예를 들어 123의 1은 10의 2승을, 2는 10의 1승을 곱함)
				NumC *= Convert.ToInt64(Math.Pow(nTH, (NumPos - 1)));

				AllNum += NumC;
				NumPos--;
			}

			return AllNum;
		}
		/// <summary>
		/// 10진수의 자리수에서 표현하는 데 한계가 되는 값을 문자열을 이용해 10진수 이상의 진수로 표현하기 위함.
		/// <remarks>
		/// 10진수는 10번째를 초과하면 자리수를 증가시키지만 16진수는 16번째를 초과해야 자리수를 증가시키므로
		/// 11번째는 A, 12번째는 B와 같이 알파벳으로 현재 자리를 계속 채워나감.
		/// </remarks>
		/// </summary>
		/// <param name="NumStr">n진수 값</param>
		/// <returns>10진수 값</returns>
		private static long Get10(char NumStr)
		{
			long n = Convert.ToInt64(NumStr);
			long n10 = 0;

			if ((n >= 65) && (n <= 90)) //A ~ Z
			{
				//A의 아스키값은 65이므로 55를 빼면 10이 됨.
				n10 = (n - 55);
			}
			else if ((n >= 97) && (n <= 122)) //a ~ z
			{
				//Z와 a 사이에는 기호가 있으므로 기호 개수만큼 6을 더 뺌.
				n10 = (n - 61);
			}

			return n10;
		}
		/// <summary>
		/// 10진수를 n진수로 변환함. 62진수까지 가능함.
		/// </summary>
		/// <remarks>
		/// 다음과 같은 원리로 이루어짐.
		/// 1.처음 수를 nTH으로 나눈다.
		/// (이때 항상 나머지 값을 Stack에 저장한다.)
		/// 2.몫이 nTH보다 크거나 같다면 그 몫을 다시 nTH으로 나눈다.
		///   몫이 nTH보다 작다면 몫 + 여태껏 저장된 나머지 값을 리턴한다.
		/// </remarks>
		/// <param name="Num">10진수 값</param>
		/// <param name="nTH">변환할 진수 단위</param>
		/// <returns>n진수로 변환된 값</returns>
		/// <example>
		/// 다음은 2006이란 10진수를 30진수로 변환하고, 다시 10진수로 변환합니다.
		/// <code>
		/// string Num30 = CMath.GetNFrom10(2006, 30);
		/// Console.WriteLine(Num30); //"26Q"
		/// 
		/// int Num10 = (int)CMath.Get10FromN(Num30, 30); //2006
		/// Console.WriteLine(Num10);
		/// </code>
		/// </example>
		public static string GetNFrom10(long Num, int nTH)
		{
			string ModList = "";
			return GetNFrom10(Num, nTH, ref ModList);
		}
		/// <summary>
		/// 10진수를 n진수로 변환함. 62진수까지 가능함.
		/// </summary>
		/// <remarks>
		/// 다음과 같은 원리로 이루어짐.
		/// 1.처음 수를 nTH으로 나눈다.
		/// (이때 항상 나머지 값을 Stack에 저장한다.)
		/// 2.몫이 nTH보다 크거나 같다면 그 몫을 다시 nTH으로 나눈다.
		///   몫이 nTH보다 작다면 몫 + 여태껏 저장된 나머지 값을 리턴한다.
		/// </remarks>
		/// <param name="Num">10진수 값</param>
		/// <param name="nTH">변환할 진수 단위</param>
		/// <param name="ModList">n진수로 변환된 값(재귀호출할 때 값을 계속 쌓아나가기 위해 사용됨)</param>
		/// <returns>n진수로 변환된 값</returns>
		/// <example>
		/// 다음은 2006이란 10진수를 30진수로 변환하고, 다시 10진수로 변환합니다.
		/// <code>
		/// string Num30 = CMath.GetNFrom10(2006, 30);
		/// Console.WriteLine(Num30); //"26Q"
		/// 
		/// int Num10 = (long)CMath.Get10FromN(Num30, 30); //2006
		/// Console.WriteLine(Num10);
		/// </code>
		/// </example>
		private static string GetNFrom10(long Num, int nTH, ref string ModList)
		{
			//나머지
			long NumMod = Num % nTH;
			//몫
			Num = Convert.ToInt64(Math.Floor(Convert.ToDouble(Num / nTH)));

			//나머지는 자리수를 증가시키며 붙여나감.
			ModList = GetNTh(NumMod) + ModList;

			string NFrom10 = "";
			if (Num >= nTH)
			{
				//몫이 현재 자리에서 표현할 수 없을 정도로 크다면
				//그 몫을 다음 자리에서 표현할 수 있도록 재귀 호출함.
				NFrom10 = GetNFrom10(Num, nTH, ref ModList);
			}
			else
			{
				//몫이 현재 자리에서 표현이 가능하므로 여태껏 생성된 하위자리수의 값들 앞에 몫을 표시함.
				//이때 몫이 없다면 하위자리의 값들만 표시하면 끝남.
				if (Num == 0)
					NFrom10 = ModList;
				else
					NFrom10 = GetNTh(Num) + ModList;
			}

			return NFrom10;
		}
		/// <summary>
		/// 10보다 큰 값을 표현하는 알파벳인 경우엔 실제 값을 리턴하게 함.
		/// </summary>
		/// <param name="NumInt"></param>
		/// <returns></returns>
		private static char GetNTh(long NumInt)
		{
			char c = ' ';
			if ((NumInt >= 0) && (NumInt <= 9))
			{
				c = Convert.ToChar(NumInt.ToString());
			}
			else if ((NumInt >= 10) && (NumInt <= 35)) //A ~ Z
			{
				c = Convert.ToChar(NumInt + 55);
			}
			else if ((NumInt >= 36) && (NumInt <= 61)) //a ~ z
			{
				c = Convert.ToChar(NumInt + 61);
			}

			return c;
		}


		/// <summary>
		/// 일반적인 반올림을 함.
		/// </summary>
		/// <remarks>
		/// .Net의 Math.Round 함수는 다음과 같이 예상하지 못한 결과를 리턴하므로
		/// 일반적으로 예상한 데로의 반올림을 위해서 이 함수를 만듦.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // -1이 아닌 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 1이 아닌 0
		/// </code>
		/// </remarks>
		/// <param name="d">반올림할 값</param>
		/// <param name="digits">소수점 뒤에 표시될 자리수</param>
		/// <returns>반올림된 값</returns>
		/// <example>
		/// 다음은 .Net의 Math 클래스에서 리턴하는 예상치 못한 결과와
		/// CMath 클래스에서 리턴하는 예상된 결과의 차이를 보여줍니다.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 0
		/// 
		/// Console.WriteLine(CMath.Round(-0.5, 0)); // -1
		/// Console.WriteLine(CMath.Round(0.5, 0)); // 1
		/// </code>
		/// </example>
		public static decimal Round(decimal d, int digits)
		{
			//-0.5, 0.5와 같이 마지막이 5인 숫자를 반올림할 경우
			//-1, 1을 리턴해야 하나 0을 리턴하는 문제 있음.
			//이를 해결하기 위해 반올림될 숫자까지를 정수로 만들고,
			//10으로 나눈 나머지가 5일 경우 반올림될 숫자에 1을 더함.
			//(예: 0.5이면 0.1을 더하고, 0.05이면 0.01을 더함)
			decimal d2 = d * (decimal)Math.Pow(10, digits + 1);
			if (Math.Abs(d2 % 10) == 5)
			{
				d = (d > 0) ? d + (decimal)Math.Pow(10, -digits - 1) : d - (decimal)Math.Pow(10, -digits - 1);
			}

			return Math.Round(d, digits);
		}
		/// <summary>
		/// 일반적인 반올림을 함.
		/// </summary>
		/// <remarks>
		/// .Net의 Math.Round 함수는 다음과 같이 예상하지 못한 결과를 리턴하므로
		/// 일반적으로 예상한 데로의 반올림을 위해서 이 함수를 만듦.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // -1이 아닌 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 1이 아닌 0
		/// </code>
		/// </remarks>
		/// <param name="d">반올림할 값</param>
		/// <returns>반올림된 값</returns>
		/// <example>
		/// 다음은 .Net의 Math 클래스에서 리턴하는 예상치 못한 결과와
		/// CMath 클래스에서 리턴하는 예상된 결과의 차이를 보여줍니다.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 0
		/// 
		/// Console.WriteLine(CMath.Round(-0.5, 0)); // -1
		/// Console.WriteLine(CMath.Round(0.5, 0)); // 1
		/// </code>
		/// </example>
		public static decimal Round(decimal d)
		{
			return Round(d, 0);
		}
		/// <summary>
		/// 올림을 함.
		/// </summary>
		/// <remarks>
		/// 수학적으로는 2.0103을 소수점 2자리까지 올림하면 3째자리가 0이므로 2.01이 맞으나,
		/// 실무적으로는 고객 위주로 계산하면 2.02로 하는 것이 맞으므로 2.02를 리턴함.
		/// </remarks>
		/// <param name="d">올림할 값</param>
		/// <param name="digits">소수점 뒤에 표시될 자리수</param>
		/// <returns>올림된 값</returns>
		/// <example>
		/// <code>
		/// Console.WriteLine(CMath.RoundUp((double)2.0103, 2)); // 2.02
		/// Console.WriteLine(CMath.RoundUp((double)2.0113, 2)); // 2.02
		/// </code>
		/// </example>
		public static decimal RoundUp(decimal d, int digits)
		{
			decimal d2 = d * (decimal)Math.Pow(10, digits);
			if ((d2 % 1) == 0) return d;

			decimal Add = (decimal)Math.Pow(10, -digits) / 2;
			if (d < 0) Add = -Add;

			return Round(d + Add, digits);
		}
		/// <summary>
		/// 올림을 함.
		/// </summary>
		/// <remarks>
		/// 수학적으로는 2.0103을 소수점 2자리까지 올림하면 3째자리가 0이므로 2.01이 맞으나,
		/// 실무적으로는 고객 위주로 계산하면 2.02로 하는 것이 맞으므로 2.02를 리턴함.
		/// </remarks>
		/// <param name="d">올림할 값</param>
		/// <returns>올림된 값</returns>
		/// <example>
		/// <code>
		/// Console.WriteLine(CMath.RoundUp((double)2.0103, 2)); // 2.02
		/// Console.WriteLine(CMath.RoundUp((double)2.0113, 2)); // 2.02
		/// </code>
		/// </example>
		public static decimal RoundUp(decimal d)
		{
			return RoundUp(d, 0);
		}

		/// <summary>
		/// 일반적인 반올림을 함.
		/// </summary>
		/// <remarks>
		/// .Net의 Math.Round 함수는 다음과 같이 예상하지 못한 결과를 리턴하므로
		/// 일반적으로 예상한 데로의 반올림을 위해서 이 함수를 만듦.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // -1이 아닌 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 1이 아닌 0
		/// </code>
		/// </remarks>
		/// <param name="d">반올림할 값</param>
		/// <param name="digits">소수점 뒤에 표시될 자리수</param>
		/// <returns>반올림된 값</returns>
		/// <example>
		/// 다음은 .Net의 Math 클래스에서 리턴하는 예상치 못한 결과와
		/// CMath 클래스에서 리턴하는 예상된 결과의 차이를 보여줍니다.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 0
		/// 
		/// Console.WriteLine(CMath.Round(-0.5, 0)); // -1
		/// Console.WriteLine(CMath.Round(0.5, 0)); // 1
		/// </code>
		/// </example>
		public static double Round(double d, int digits)
		{
			//-0.5, 0.5와 같이 마지막이 5인 숫자를 반올림할 경우
			//-1, 1을 리턴해야 하나 0을 리턴하는 문제 있음.
			//이를 해결하기 위해 반올림될 숫자까지를 정수로 만들고,
			//10으로 나눈 나머지가 5일 경우 반올림될 숫자에 1을 더함.
			//(예: 0.5이면 0.1을 더하고, 0.05이면 0.01을 더함)
			double d2 = d * Math.Pow(10, digits + 1);
			if (Math.Abs(d2 % 10) == 5)
			{
				d = (d > 0) ? d + Math.Pow(10, -digits - 1) : d - Math.Pow(10, -digits - 1);
			}

			return Math.Round(d, digits);
		}
		/// <summary>
		/// 일반적인 반올림을 함.
		/// </summary>
		/// <remarks>
		/// .Net의 Math.Round 함수는 다음과 같이 예상하지 못한 결과를 리턴하므로
		/// 일반적으로 예상한 데로의 반올림을 위해서 이 함수를 만듦.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // -1이 아닌 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 1이 아닌 0
		/// </code>
		/// </remarks>
		/// <param name="d">반올림할 값</param>
		/// <returns>반올림된 값</returns>
		/// <example>
		/// 다음은 .Net의 Math 클래스에서 리턴하는 예상치 못한 결과와
		/// CMath 클래스에서 리턴하는 예상된 결과의 차이를 보여줍니다.
		/// <code>
		/// Console.WriteLine(Math.Round(-0.5, 0)); // 0
		/// Console.WriteLine(Math.Round(0.5, 0)); // 0
		/// 
		/// Console.WriteLine(CMath.Round(-0.5, 0)); // -1
		/// Console.WriteLine(CMath.Round(0.5, 0)); // 1
		/// </code>
		/// </example>
		public static double Round(double d)
		{
			return Round(d, 0);
		}
		/// <summary>
		/// 올림을 함.
		/// </summary>
		/// <remarks>
		/// 수학적으로는 2.0103을 소수점 2자리까지 올림하면 3째자리가 0이므로 2.01이 맞으나,
		/// 실무적으로는 고객 위주로 계산하면 2.02로 하는 것이 맞으므로 2.02를 리턴함.
		/// </remarks>
		/// <param name="d">올림할 값</param>
		/// <param name="digits">소수점 뒤에 표시될 자리수</param>
		/// <returns>올림된 값</returns>
		/// <example>
		/// <code>
		/// Console.WriteLine(CMath.RoundUp((double)2.0103, 2)); // 2.02
		/// Console.WriteLine(CMath.RoundUp((double)2.0113, 2)); // 2.02
		/// </code>
		/// </example>
		public static double RoundUp(double d, int digits)
		{
			//올림의 경우는 끝단위가 0이면 올릴 수 없음.(예: 1.1 -> 2, 1.0 -> 1)
			//그러므로 끝단위가 0인지 확인하기 위해 다음의 방법을 사용함.
			//올려질 자리수 앞까지 정수로 만들고, 1로 나눠서 나머지 값이 없다면 끝단위가 0임.
			//(예: 10 % 1 -> 0, 10.1 % 1 -> 0.1)
			double d2 = d * Math.Pow(10, digits);
			if ((d2 % 1) == 0) return d;

			double Add = Math.Pow(10, -digits) / 2;
			if (d < 0) Add = -Add;

			return Round(d + Add, digits);
		}
		/// <summary>
		/// 올림을 함.
		/// </summary>
		/// <remarks>
		/// 수학적으로는 2.0103을 소수점 2자리까지 올림하면 3째자리가 0이므로 2.01이 맞으나,
		/// 실무적으로는 고객 위주로 계산하면 2.02로 하는 것이 맞으므로 2.02를 리턴함.
		/// </remarks>
		/// <param name="d">올림할 값</param>
		/// <returns>올림된 값</returns>
		/// <example>
		/// <code>
		/// Console.WriteLine(CMath.RoundUp((double)2.0103, 2)); // 2.02
		/// Console.WriteLine(CMath.RoundUp((double)2.0113, 2)); // 2.02
		/// </code>
		/// </example>
		public static double RoundUp(double d)
		{
			return RoundUp(d, 0);
		}

		/// <summary>
		/// 내림을 함.
		/// </summary>
		/// <param name="d">내림할 값</param>
		/// <param name="digits">소수점 뒤에 표시될 자리수</param>
		/// <returns>내림된 값</returns>
		/// <example>
		/// <code>
		/// Console.WriteLine(CMath.RoundDown((double)2.0103, 2)); // 2.01
		/// Console.WriteLine(CMath.RoundDown((double)2.0193, 2)); // 2.01
		/// </code>
		/// </example>
		/// <remarks>
		/// Type을 double로 하면 CMath.RoundDown((double)2.0103, 2)의 결과가 2.010300000001 과 같이
		/// 나와서 decimal로 Type을 정함.
		/// </remarks>
		public static decimal RoundDown(decimal d, int digits)
		{
			decimal Unit = (decimal)Math.Pow(10, -digits);
			return (Math.Floor(d / Unit) * Unit);
		}
		public static decimal RoundDown(decimal d)
		{
			return RoundDown(d, 0);
		}
		/// <summary>
		/// Math.Floor는 낮은 값으로만 내려감. 즉, -1.5일 경우 -2를 리턴함.
		/// -1.5일 때는 -1을 리턴하기 위함.
		/// </summary>
		/// <param name="dVal"></param>
		/// <returns></returns>
		public static int RoundDownIgnoreSign(double dVal)
		{
			if (dVal >= 0)
				return (int)Math.Floor(dVal);

			return (int)Math.Ceiling(dVal);
		}
		//public static decimal RoundDown(decimal d, int digits)
		//{
		//	decimal Add = (decimal)(Math.Pow(10, -digits) / 2);
		//	if (d < 0) Add = -Add;

		//	return Round(d - Add, digits);
		//}
		//public static decimal RoundDown(decimal d)
		//{
		//	return RoundDown(d, 0);
		//}
		//public static double RoundDown(double d, int digits)
		//{
		//	double Add = (Math.Pow(10, -digits) / 2);
		//	if (d < 0) Add = -Add;

		//	return Round(d - Add, digits);
		//}
		//public static double RoundDown(double d)
		//{
		//	return RoundDown(d, 0);
		//}

		/// <summary>
		/// 정수만 무조건 올림함
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Digits"></param>
		/// <returns></returns>
		/// <example>
		/// CMath.RoundUp(1234, -1); // 1240
		/// CMath.RoundUp(1234, -2); // 1300
		/// CMath.RoundUp(1234, -3); // 2000
		/// CMath.RoundUp(1234, -4); // 에러
		/// </example>
		public static int RoundUp(int Value, int Digits)
		{
			double Unit = Math.Pow(10, -Digits);
			//Digits가 Value 자리수보다 큰 경우에 Divide by zero 에러 내기 위함.
			if (Unit > Value)
				Unit = 0;

			int ValueNew = Convert.ToInt32(Math.Ceiling((double)Value / Unit) * Unit);

			return ValueNew;
		}

		/// <summary>
		/// 전체 행수, 페이지 크기에 의해 계산된 총 페이지 수를 리턴함.
		/// </summary>
		/// <param name="RowCount">전체 행수</param>
		/// <param name="PageSize">페이지 크기</param>
		/// <param name="RowCountOfLastPageIs">마지막 페이지의 행수</param>
		/// <returns>총 페이지 수</returns>
		/// <example>
		/// 다음은 한 페이지에 3행 씩 표현될 때 10행을 인쇄하는 데 필요한 총 페이지 개수를 리턴합니다.
		/// <code>
		/// int RowCountRest;
		/// int PageCount = CMath.GetPageCount(10, 3, out RowCountRest);
		/// Console.WriteLine(PageCount); // 4
		/// Console.WriteLine(RowCountRest); // 1
		/// </code>
		/// </example>
		public static int GetPageCount(int RowCount, int PageSize, out int RowCountOfLastPageIs)
		{
			RowCountOfLastPageIs = 0;
			if (RowCount < 1) return 0;

			//총 페이지수는  총 행수에서 1페이지당 행수를 나눈 몫.
			//이때 나머지 값이 있다면 총 쪽수에 1을 더함.
			RowCountOfLastPageIs = RowCount % PageSize;
			int PageCount = (int)(RowCount / PageSize);
			return PageCount + (RowCountOfLastPageIs > 0 ? 1 : 0);
		}
		/// <summary>
		/// 전체 행수, 페이지 크기에 의해 계산된 총 페이지 수를 리턴함.
		/// </summary>
		/// <param name="RowCount">전체 행수</param>
		/// <param name="PageSize">페이지 크기</param>
		/// <returns>총 페이지 수</returns>
		/// <example>
		/// 다음은 한 페이지에 3행 씩 표현될 때 10행을 인쇄하는 데 필요한 총 페이지 개수를 리턴합니다.
		/// <code>
		/// int PageCount = CMath.GetPageCount(10, 3);
		/// Console.WriteLine(PageCount); // 4
		/// </code>
		/// </example>
		public static int GetPageCount(int RowCount, int PageSize)
		{
			int RowCountOfLastPage;
			return GetPageCount(RowCount, PageSize, out RowCountOfLastPage);
		}

		/// <summary>
		/// 페이지 크기, 현재 페이지에 의해 계산된 시작 행과 끝 행을 리턴함.
		/// </summary>
		/// <param name="PageSize">페이키 크기</param>
		/// <param name="Page">현재 페이지</param>
		/// <param name="RowFromIs">시작 행 번호</param>
		/// <param name="RowToIs">끝 행 번호</param>
		public static void GetRowFromTo(int PageSize, int Page, out int RowFromIs, out int RowToIs)
		{
			RowFromIs = ((Page - 1) * PageSize) + 1;
			RowToIs = RowFromIs + PageSize - 1;
		}

		/// <summary>
		/// 현재 행 번호로 계산된 현재 페이지 번호를 리턴함.
		/// </summary>
		/// <param name="PageSize">페이지 크기</param>
		/// <param name="CurrentRow">현재 행</param>
		/// <returns></returns>
		/// <example>
		/// int Page1 = CMath.GetCurrentPage(10, 10); // 1
		/// int Page2 = CMath.GetCurrentPage(10, 11); // 2
		/// </example>
		public static int GetCurrentPage(int PageSize, int CurrentRow)
		{
			return Convert.ToInt32(Math.Ceiling(((CurrentRow + 0d) / PageSize)));
		}

		public static int GetRandomizedNum(int From, int To)
		{
			Random random = new Random();
			return random.Next(From, To);
		}

		/// <summary>
		/// 1을 제외하고 나눌 수 있는 모든 가능한 숫자를 리턴함.
		/// 예를 들어 6은 2, 3을, 14는 7을 리턴함.
		/// </summary>
		/// <param name="Num"></param>
		/// <returns></returns>
		public static int[] GetDividableNumbers(int Num)
		{
			List<int> aDiv = new List<int>();
			for (int i = (int)(Num / 2), i2 = 2; i >= i2; i--)
			{
				if ((Num % i) == 0)
				{
					int Div = (Num / i);
					aDiv.Add(Div);
				}
			}


			return aDiv.ToArray();
		}

		public static string Hex(int i, bool PrefixPercent)
		{
			return (PrefixPercent ? "%" : "") + i.ToString("x2");
		}
		public static string Hex(long i, bool PrefixPercent)
		{
			return (PrefixPercent ? "%" : "") + i.ToString("x2");
		}
		public static string Hex(char c, bool PrefixPercent)
		{
			return Hex((int)c, PrefixPercent);
		}
		public static string Hex(string s, bool PrefixPercent)
		{
			string s2 = "";
			for (int i = 0, i2 = s.Length; i < i2; i++)
			{
				s2 += Hex(s[i], PrefixPercent);
			}

			return s2;
		}

		public static int Dec(string s)
		{
			return Int32.Parse(s, NumberStyles.AllowHexSpecifier);
		}

		/// <summary>
		/// 다음의 기준에 따라 변환된 값을 리턴함.
		/// 1440 Twips : 1 Inch
		/// 567 Twips : 1 Centimeter
		/// 72 Points : 1 Inch
		/// 2.54 Centemeters : 1 Inch
		/// 10 Millimeters : 1 Centimeter
		/// </summary>
		/// <param name="FromMode">원래 단위</param>
		/// <param name="FromValue">원래 값</param>
		/// <param name="ToMode">변환할 단위</param>
		/// <returns>변환된 값</returns>
		public static float ConvertUnit(ScaleModeConstants FromMode, float FromValue, ScaleModeConstants ToMode,
			float DpiXOrY)
		{
			float NewValue = 0;

			//VB에서 테스트 결과 해상도를 바꿔도 항상 15를 리턴하므로 15로 고정시킴.
			float TwipsPerPixel = 15;
			const float drMilliPerInch25_4 = 25.4f;
			const float drCentiPerInch2_54 = 2.54f;

			switch (FromMode)
			{
				case ScaleModeConstants.Twips:
					switch (ToMode)
					{
						case ScaleModeConstants.Twips:
							NewValue = FromValue;
							break;
						case ScaleModeConstants.Points:
							NewValue = (FromValue / 1440) * 72;
							break;
						case ScaleModeConstants.Pixels:
							NewValue = FromValue / TwipsPerPixel;
							break;
						case ScaleModeConstants.Inches:
							NewValue = FromValue / 1440;
							break;
						case ScaleModeConstants.Millimeters:
							NewValue = FromValue / 5670;
							break;
						case ScaleModeConstants.Centimeters:
							NewValue = FromValue / 567;
							break;
					}
					break;
				case ScaleModeConstants.Points:
					switch (ToMode)
					{
						case ScaleModeConstants.Twips:
							NewValue = (FromValue / 72) * 1440;
							break;
						case ScaleModeConstants.Points:
							NewValue = FromValue;
							break;
						case ScaleModeConstants.Pixels:
							NewValue = ((FromValue / 72) * 1440) / TwipsPerPixel;
							break;
						case ScaleModeConstants.Inches:
							NewValue = FromValue / 72;
							break;
						case ScaleModeConstants.Millimeters:
							NewValue = (FromValue / 72) * drMilliPerInch25_4;
							break;
						case ScaleModeConstants.Centimeters:
							NewValue = (FromValue / 72) * drCentiPerInch2_54;
							break;
					}
					break;
				case ScaleModeConstants.Pixels:
					switch (ToMode)
					{
						case ScaleModeConstants.Twips:
							NewValue = FromValue * TwipsPerPixel;
							break;
						case ScaleModeConstants.Points:
							NewValue = ((FromValue * TwipsPerPixel) / 1440) * 72;
							break;
						case ScaleModeConstants.Pixels:
							NewValue = FromValue;
							break;
						case ScaleModeConstants.Inches:
							NewValue = (FromValue * TwipsPerPixel) / 1440;
							break;
						case ScaleModeConstants.Millimeters:
							//NewValue = (FromValue * TwipsPerPixel) / 5670;
							NewValue = FromValue / (DpiXOrY / drMilliPerInch25_4);
							break;
						case ScaleModeConstants.Centimeters:
							//NewValue = (FromValue * TwipsPerPixel) / 567;
							NewValue = FromValue / (DpiXOrY / drCentiPerInch2_54);
							break;
					}
					break;
				case ScaleModeConstants.Inches:
					switch (ToMode)
					{
						case ScaleModeConstants.Twips:
							NewValue = FromValue * 1440;
							break;
						case ScaleModeConstants.Points:
							NewValue = FromValue * 72;
							break;
						case ScaleModeConstants.Pixels:
							NewValue = (FromValue * 1440) / TwipsPerPixel;
							break;
						case ScaleModeConstants.Inches:
							NewValue = FromValue;
							break;
						case ScaleModeConstants.Millimeters:
							NewValue = FromValue * drMilliPerInch25_4;
							break;
						case ScaleModeConstants.Centimeters:
							NewValue = FromValue * drCentiPerInch2_54;
							break;
					}
					break;
				case ScaleModeConstants.Millimeters:
					switch (ToMode)
					{
						case ScaleModeConstants.Twips:
							NewValue = FromValue * 5670;
							break;
						case ScaleModeConstants.Points:
							NewValue = (FromValue / drMilliPerInch25_4) * 72;
							break;
						case ScaleModeConstants.Pixels:
							//NewValue = (FromValue * 5670) / TwipsPerPixel;
							NewValue = (DpiXOrY / drMilliPerInch25_4) * FromValue;
							break;
						case ScaleModeConstants.Inches:
							NewValue = FromValue / drMilliPerInch25_4;
							break;
						case ScaleModeConstants.Millimeters:
							NewValue = FromValue;
							break;
						case ScaleModeConstants.Centimeters:
							NewValue = FromValue / 10;
							break;
					}
					break;
				case ScaleModeConstants.Centimeters:
					switch (ToMode)
					{
						case ScaleModeConstants.Twips:
							NewValue = FromValue * 567;
							break;
						case ScaleModeConstants.Points:
							NewValue = (FromValue / drCentiPerInch2_54) * 72;
							break;
						case ScaleModeConstants.Pixels:
							//NewValue = (FromValue * 567) / TwipsPerPixel;
							NewValue = (DpiXOrY / drCentiPerInch2_54) * FromValue;
							break;
						case ScaleModeConstants.Inches:
							NewValue = FromValue / drCentiPerInch2_54;
							break;
						case ScaleModeConstants.Millimeters:
							NewValue = FromValue * 10;
							break;
						case ScaleModeConstants.Centimeters:
							NewValue = FromValue;
							break;
					}
					break;
				default:
					break;
			}

			return NewValue;
		}
		public static float ConvertUnit(ScaleModeConstants FromMode, float FromValue, ScaleModeConstants ToMode)
		{
			//C#에서 테스트 결과 해상도를 바꿔도 항상 96이므로 고정함.
			int DpiXOrY = 96;
			return ConvertUnit(FromMode, FromValue, ToMode, DpiXOrY);
		}

		public static SizeF GetPaperSize(PaperKind Kind, ScaleModeConstants Mode)
		{
			#region GetValues
			if (_dicPaperSize.Count == 0)
			{
				//기본 프린터가 Microsoft XPS Document Writer인 상태에서 다음 코드로 생성함.
				//기본 프린터가 다른 경우엔 종류가 적어지고 값이 최대 10 정도 달라짐.
				//string s = "";
				//PrinterSettings settings = new PrinterSettings();
				//foreach (PaperSize size in settings.PaperSizes)
				//    s += string.Format("_dicPaperSize.Add(PaperKind.{0}, new Size({1}, {2}));\r\n", size.Kind, size.Width, size.Height); 

				_dicPaperSize.Add(PaperKind.Letter, new Size(850, 1100));
				_dicPaperSize.Add(PaperKind.LetterSmall, new Size(850, 1100));
				_dicPaperSize.Add(PaperKind.Tabloid, new Size(1100, 1700));
				_dicPaperSize.Add(PaperKind.Ledger, new Size(1700, 1100));
				_dicPaperSize.Add(PaperKind.Legal, new Size(850, 1400));
				_dicPaperSize.Add(PaperKind.Statement, new Size(550, 850));
				_dicPaperSize.Add(PaperKind.Executive, new Size(725, 1050));
				_dicPaperSize.Add(PaperKind.A3, new Size(1169, 1654));
				_dicPaperSize.Add(PaperKind.A4, new Size(827, 1169));
				_dicPaperSize.Add(PaperKind.A4Small, new Size(827, 1169));
				_dicPaperSize.Add(PaperKind.A5, new Size(583, 827));
				_dicPaperSize.Add(PaperKind.B4, new Size(1012, 1433));
				_dicPaperSize.Add(PaperKind.B5, new Size(717, 1012));
				_dicPaperSize.Add(PaperKind.Folio, new Size(850, 1300));
				_dicPaperSize.Add(PaperKind.Quarto, new Size(846, 1083));
				_dicPaperSize.Add(PaperKind.Standard10x14, new Size(1000, 1400));
				_dicPaperSize.Add(PaperKind.Standard11x17, new Size(1100, 1700));
				_dicPaperSize.Add(PaperKind.Note, new Size(850, 1100));
				_dicPaperSize.Add(PaperKind.Number9Envelope, new Size(387, 887));
				_dicPaperSize.Add(PaperKind.Number10Envelope, new Size(412, 950));
				_dicPaperSize.Add(PaperKind.Number11Envelope, new Size(450, 1037));
				_dicPaperSize.Add(PaperKind.Number12Envelope, new Size(475, 1100));
				_dicPaperSize.Add(PaperKind.Number14Envelope, new Size(500, 1150));
				_dicPaperSize.Add(PaperKind.CSheet, new Size(1700, 2200));
				_dicPaperSize.Add(PaperKind.DSheet, new Size(2200, 3400));
				_dicPaperSize.Add(PaperKind.ESheet, new Size(3400, 4400));
				_dicPaperSize.Add(PaperKind.DLEnvelope, new Size(433, 866));
				_dicPaperSize.Add(PaperKind.C5Envelope, new Size(638, 902));
				_dicPaperSize.Add(PaperKind.C3Envelope, new Size(1276, 1803));
				_dicPaperSize.Add(PaperKind.C4Envelope, new Size(902, 1276));
				_dicPaperSize.Add(PaperKind.C6Envelope, new Size(449, 638));
				_dicPaperSize.Add(PaperKind.C65Envelope, new Size(449, 902));
				_dicPaperSize.Add(PaperKind.B4Envelope, new Size(984, 1390));
				_dicPaperSize.Add(PaperKind.B5Envelope, new Size(693, 984));
				_dicPaperSize.Add(PaperKind.B6Envelope, new Size(693, 492));
				_dicPaperSize.Add(PaperKind.ItalyEnvelope, new Size(433, 906));
				_dicPaperSize.Add(PaperKind.MonarchEnvelope, new Size(387, 750));
				_dicPaperSize.Add(PaperKind.PersonalEnvelope, new Size(362, 650));
				_dicPaperSize.Add(PaperKind.USStandardFanfold, new Size(1487, 1100));
				_dicPaperSize.Add(PaperKind.GermanStandardFanfold, new Size(850, 1200));
				_dicPaperSize.Add(PaperKind.GermanLegalFanfold, new Size(850, 1300));
				_dicPaperSize.Add(PaperKind.IsoB4, new Size(984, 1390));
				_dicPaperSize.Add(PaperKind.JapanesePostcard, new Size(394, 583));
				_dicPaperSize.Add(PaperKind.Standard9x11, new Size(900, 1100));
				_dicPaperSize.Add(PaperKind.Standard10x11, new Size(1000, 1100));
				_dicPaperSize.Add(PaperKind.Standard15x11, new Size(1500, 1100));
				_dicPaperSize.Add(PaperKind.InviteEnvelope, new Size(866, 866));
				_dicPaperSize.Add(PaperKind.LetterExtra, new Size(950, 1200));
				_dicPaperSize.Add(PaperKind.LegalExtra, new Size(950, 1500));
				_dicPaperSize.Add(PaperKind.A4Extra, new Size(927, 1269));
				_dicPaperSize.Add(PaperKind.LetterTransverse, new Size(850, 1100));
				_dicPaperSize.Add(PaperKind.A4Transverse, new Size(827, 1169));
				_dicPaperSize.Add(PaperKind.LetterExtraTransverse, new Size(950, 1200));
				_dicPaperSize.Add(PaperKind.APlus, new Size(894, 1402));
				_dicPaperSize.Add(PaperKind.BPlus, new Size(1201, 1917));
				_dicPaperSize.Add(PaperKind.LetterPlus, new Size(850, 1269));
				_dicPaperSize.Add(PaperKind.A4Plus, new Size(827, 1299));
				_dicPaperSize.Add(PaperKind.A5Transverse, new Size(583, 827));
				_dicPaperSize.Add(PaperKind.B5Transverse, new Size(717, 1012));
				_dicPaperSize.Add(PaperKind.A3Extra, new Size(1268, 1752));
				_dicPaperSize.Add(PaperKind.A5Extra, new Size(685, 925));
				_dicPaperSize.Add(PaperKind.B5Extra, new Size(791, 1087));
				_dicPaperSize.Add(PaperKind.A2, new Size(1654, 2339));
				_dicPaperSize.Add(PaperKind.A3Transverse, new Size(1169, 1654));
				_dicPaperSize.Add(PaperKind.A3ExtraTransverse, new Size(1268, 1752));
				_dicPaperSize.Add(PaperKind.JapaneseDoublePostcard, new Size(787, 583));
				_dicPaperSize.Add(PaperKind.A6, new Size(413, 583));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeKakuNumber2, new Size(945, 1307));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeKakuNumber3, new Size(850, 1091));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeChouNumber3, new Size(472, 925));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeChouNumber4, new Size(354, 807));
				_dicPaperSize.Add(PaperKind.LetterRotated, new Size(1100, 850));
				_dicPaperSize.Add(PaperKind.A3Rotated, new Size(1654, 1169));
				_dicPaperSize.Add(PaperKind.A4Rotated, new Size(1169, 827));
				_dicPaperSize.Add(PaperKind.A5Rotated, new Size(827, 583));
				_dicPaperSize.Add(PaperKind.B4JisRotated, new Size(1433, 1012));
				_dicPaperSize.Add(PaperKind.B5JisRotated, new Size(1012, 717));
				_dicPaperSize.Add(PaperKind.JapanesePostcardRotated, new Size(583, 394));
				_dicPaperSize.Add(PaperKind.JapaneseDoublePostcardRotated, new Size(583, 787));
				_dicPaperSize.Add(PaperKind.A6Rotated, new Size(583, 413));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeKakuNumber2Rotated, new Size(1307, 945));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeKakuNumber3Rotated, new Size(1091, 850));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeChouNumber3Rotated, new Size(925, 472));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeChouNumber4Rotated, new Size(807, 354));
				_dicPaperSize.Add(PaperKind.B6Jis, new Size(504, 717));
				_dicPaperSize.Add(PaperKind.B6JisRotated, new Size(717, 504));
				_dicPaperSize.Add(PaperKind.Standard12x11, new Size(1200, 1100));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeYouNumber4, new Size(413, 925));
				_dicPaperSize.Add(PaperKind.JapaneseEnvelopeYouNumber4Rotated, new Size(925, 413));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber1, new Size(402, 650));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber3, new Size(492, 693));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber4, new Size(433, 819));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber5, new Size(433, 866));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber6, new Size(472, 906));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber7, new Size(630, 906));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber8, new Size(472, 1217));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber9, new Size(902, 1276));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber10, new Size(1276, 1803));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber1Rotated, new Size(650, 402));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber3Rotated, new Size(693, 492));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber4Rotated, new Size(819, 433));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber5Rotated, new Size(866, 433));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber6Rotated, new Size(906, 472));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber7Rotated, new Size(906, 630));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber8Rotated, new Size(1217, 472));
				_dicPaperSize.Add(PaperKind.PrcEnvelopeNumber9Rotated, new Size(1276, 902));
			}
			#endregion GetValues

			Size szInches100 = _dicPaperSize[Kind];
			SizeF szNew = new SizeF(
				CMath.ConvertUnit(ScaleModeConstants.Inches, szInches100.Width / 100f, Mode),
				CMath.ConvertUnit(ScaleModeConstants.Inches, szInches100.Height / 100f, Mode));

			return szNew;
		}
		public static SizeF GetPaperSize(PaperKind Kind, bool Rotated, ScaleModeConstants Mode)
		{
			if (Kind.ToString().EndsWith("Rotated"))
				throw new Exception(string.Format("Kind:{0} has 'Rotated' value", Kind));

			PaperKind KindNew = Kind;

			if (Rotated)
			{
				switch (Kind)
				{
					case PaperKind.Letter: KindNew = PaperKind.LetterRotated; break;
					case PaperKind.B4: KindNew = PaperKind.B4JisRotated; break;
					case PaperKind.B5: KindNew = PaperKind.B5JisRotated; break;
					case PaperKind.A3: KindNew = PaperKind.A3Rotated; break;
					case PaperKind.A4: KindNew = PaperKind.A4Rotated; break;
					case PaperKind.A5: KindNew = PaperKind.A5Rotated; break;
					case PaperKind.A6: KindNew = PaperKind.A6Rotated; break;
					default:
						throw new Exception(string.Format("Wrong PaperKind:{0}", Kind));
				}
			}

			return GetPaperSize(KindNew, Mode);
		}
		/// <summary>
		/// IE의 페이지 설정 대화상자의 용지 크기 목록에 표시되는 이름을 리턴함.
		/// </summary>
		public static string GetPaperSizeName(PaperKind Kind)
		{
			switch (Kind)
			{
				case PaperKind.Letter: return "Letter";
				case PaperKind.LetterRotated: return "Letter Rotated";
				case PaperKind.B4: return "B4"; //"B4 (ISO)"
				case PaperKind.B4JisRotated: return "B4 Rotated"; //"B4 (JIS) Rotated"
				case PaperKind.B5: return "B5"; //"B5 (JIS)"
				case PaperKind.B5JisRotated: return "B5 Rotated"; // "B5 (JIS) Rotated";
				case PaperKind.A3: return "A3";
				case PaperKind.A3Rotated: return "A3 Rotated";
				case PaperKind.A4: return "A4";
				case PaperKind.A4Rotated: return "A4 Rotated";
				case PaperKind.A5: return "A5";
				case PaperKind.A5Rotated: return "A5 Rotated";
				case PaperKind.A6: return "A6";
				case PaperKind.A6Rotated: return "A6 Rotated";
				default:
					throw new Exception(string.Format("Wrong PaperKind:{0}", Kind));
			}
		}
		public static void GetPaperKindAndRotated(PaperKind Kind, out PaperKind KindNotRotatedIs, out bool RotatedIs)
		{
			RotatedIs = Kind.ToString().EndsWith("Rotated");

			if (!RotatedIs)
			{
				KindNotRotatedIs = Kind;
			}
			else
			{
				switch (Kind)
				{
					case PaperKind.LetterRotated: KindNotRotatedIs = PaperKind.Letter; break;
					case PaperKind.B4JisRotated: KindNotRotatedIs = PaperKind.B4; break;
					case PaperKind.B5JisRotated: KindNotRotatedIs = PaperKind.B5; break;
					case PaperKind.A3Rotated: KindNotRotatedIs = PaperKind.A3; break;
					case PaperKind.A4Rotated: KindNotRotatedIs = PaperKind.A4; break;
					case PaperKind.A5Rotated: KindNotRotatedIs = PaperKind.A5; break;
					case PaperKind.A6Rotated: KindNotRotatedIs = PaperKind.A6; break;
					default:
						throw new Exception(string.Format("Wrong PaperKind:{0}", Kind));
				}
			}
		}

		/// <summary>
		/// 윈도우 폼, 아래한글 글꼴 크기에서 10pt를 지정해도 0.75 단위로 증가하는 9.75를 리턴하므로 그 값을 리턴함.
		/// </summary>
		/// <param name="FontSize"></param>
		/// <returns></returns>
		public static float ConvertFontSizeTo75Unit(float FontSize)
		{
			if (FontSize <= 0)
				throw new Exception("Wrong Value: 0");
			else if (FontSize == 1) //1은 0.75가 가까운 값이나 1.5를 리턴하는 예외 있음.
				return 1.5f;

			float Unit = 0.75f;
			float Size1 = FontSize - (FontSize % Unit);
			float Size2 = Size1 + Unit;
			if ((FontSize - Size1) < (Size2 - FontSize))
				return Size1;
			else
				return Size2;
		}

		/// <summary>
		/// Table 서버 컨트롤의 Width 값을 주면 ASP.Net에서 변환하는 방식을 흉내냄.
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// string FuncValue = ConvertUnitToHtmlSize(Unit.Pixel(300)); // "300px"
		/// string FuncValue = ConvertUnitToHtmlSize(Unit.Percentage(100)); // "100%"
		/// string FuncValue = ConvertUnitToHtmlSize(Unit.Empty); // ""
		/// ]]>
		/// </example>
		/// <param name="u"></param>
		/// <returns></returns>
		public static string ConvertUnitToHtmlSize(Unit u)
		{
			string s;

			if (u.IsEmpty)
				s = "";
			else
			{
				switch (u.Type)
				{
					case UnitType.Percentage:
						s = string.Concat(u.Value, "%");
						break;
					case UnitType.Pixel:
						s = string.Concat(u.Value, "px");
						break;
					default:
						throw new Exception("Pixel, Percentage 외에는 허용하지 않습니다.");
				}
			}

			return s;
		}

		public static string ConvertByteToGbMbKb(long SizeInByte)
		{
			string sSize = "";
			if (SizeInByte >= CMath.FileSize1Gb)
				sSize = (SizeInByte / CMath.FileSize1Gb).ToString("#,##0") + "GB";
			else if (SizeInByte >= CMath.FileSize1Mb)
				sSize = (SizeInByte / CMath.FileSize1Mb).ToString("#,##0") + "MB";
			else if (SizeInByte >= CMath.FileSize1Kb)
				sSize = (SizeInByte / CMath.FileSize1Kb).ToString("#,##0") + "KB";
			else
				sSize = "1KB";

			return sSize;
		}
		public static long ConvertGbMbKbToByte(string Value)
		{
			long Size = 0;

			if (Value.IndexOf(",") != -1)
				Value = Value.Replace(",", "");

			if (Value.EndsWith("GB", StringComparison.CurrentCultureIgnoreCase))
				Size = Convert.ToInt64(Value.Substring(0, Value.Length - 2)) * CMath.FileSize1Gb;
			else if (Value.EndsWith("MB", StringComparison.CurrentCultureIgnoreCase))
				Size = Convert.ToInt64(Value.Substring(0, Value.Length - 2)) * CMath.FileSize1Mb;
			else if (Value.EndsWith("KB", StringComparison.CurrentCultureIgnoreCase))
				Size = Convert.ToInt64(Value.Substring(0, Value.Length - 2)) * CMath.FileSize1Kb;
			else
				Size = Convert.ToInt64(Value.Substring(0, Value.Length - 2));

			return Size;
		}

		public static DateTime Max(DateTime Value1, DateTime Value2)
		{
			return (Value1 > Value2) ? Value1 : Value2;
		}
		public static DateTime Min(DateTime Value1, DateTime Value2)
		{
			return (Value1 < Value2) ? Value1 : Value2;
		}

		public static int Min(params int[] Value)
		{
			int ValueMin = int.MaxValue;

			foreach (int ValueCur in Value)
			{
				if (ValueCur < ValueMin)
					ValueMin = ValueCur;
			}

			return ValueMin;
		}

		/// <summary>
		/// A(100*400) 이미지를 B(200*400) 이미지로 대체하는 경우에 A 이미지의 너비에 맞게 B 이미지를 변경하면
		/// 100*200 크기로 조절하게 되어 이런 경우 B 이미지가 너무 작아지는 경우 발생함.
		/// 그러므로 A 이미지의 면적인 40,000에 맞게 B 이미지의 크기를 141*282로 변경함.
		/// </summary>
		/// <param name="SizeSrc">원본 크기</param>
		/// <param name="AreaDest">변경하려는 면적</param>
		/// <param name="Tolerance">변경하려는 면적과 정확히 일치하지 않더라도 지정값 만큼만 차이나면 만족함</param>
		/// <param name="Step">변경하기 위해 크기를 점진적으로 줄이거나 늘릴 값</param>
		/// <returns></returns>
		private static SizeF GetSizeByArea(SizeF SizeSrc, float AreaDest, float Tolerance, float Step)
		{
			//무한루프 도는 것 방지
			if (SizeSrc.Width <= 0)
				throw new Exception(string.Format("SizeSrc.Width:{0} is less than or equal to 0.", SizeSrc.Width));
			if (SizeSrc.Height <= 0)
				throw new Exception(string.Format("SizeSrc.Height:{0} is less than or equal to 0.", SizeSrc.Height));
			if (AreaDest <= 0)
				throw new Exception(string.Format("AreaDest:{0} is less than or equal to 0.", AreaDest));
			if (Tolerance < 1)
				throw new Exception(string.Format("Tolerance:{0} is less than 1.", Tolerance));
			if (Step <= 0)
				throw new Exception(string.Format("Step:{0} is less than or equal to 0.", Step));

			SizeF SizeNew = new SizeF(SizeSrc.Width, SizeSrc.Height);
			float AreaSrc = SizeSrc.Width * SizeSrc.Height;

			float Zoom = 1f;

			if (AreaSrc < AreaDest)
			{
				while (AreaSrc < AreaDest)
				{
					Zoom += Step;
					SizeNew = new SizeF((SizeSrc.Width * Zoom), (SizeSrc.Height * Zoom));
					AreaSrc = SizeNew.Width * SizeNew.Height;
				}
			}
			else
			{
				while (AreaSrc > AreaDest)
				{
					Zoom -= Step;
					SizeNew = new SizeF((SizeSrc.Width * Zoom), (SizeSrc.Height * Zoom));
					AreaSrc = SizeNew.Width * SizeNew.Height;
				}
			}

			if (Math.Abs(AreaSrc - AreaDest) > Tolerance)
			{
				SizeNew = GetSizeByArea(SizeNew, AreaDest, Tolerance, Step / 2);
			}

			return new SizeF(SizeNew.Width, SizeNew.Height);
		}
		public static SizeF GetSizeByArea(SizeF SizeSrc, SizeF SizeDest, float Tolerance)
		{
			float AreaSrc = SizeSrc.Width * SizeSrc.Height;
			float AreaDest = SizeDest.Width * SizeDest.Height;
			float Step = Math.Min(Math.Abs(AreaSrc / AreaDest), Math.Abs(AreaDest / AreaSrc));
			return GetSizeByArea(SizeSrc, AreaDest, Tolerance, Step);
		}
	}
}
