using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;

namespace DoctorGu
{
	/// <summary>
	/// 주민번호, 사업자등록번호 등의 유효성을 확인함.
	/// </summary>
	public class CValid
	{
		//public const string DisallowedListForFileName = @"\/:*?""<>|";
		public static readonly string DisallowedListForFileName = new string(Path.GetInvalidFileNameChars());

		public static void Main(string[] args)
		{
			string ErrMsgIs;

			Console.WriteLine(GetBarcodeCheckDigit("978898912508", out ErrMsgIs)); //"2"
			Console.WriteLine(GetBarcodeCheckDigit("978895090425", out ErrMsgIs)); //"8"
			Console.WriteLine(GetBarcodeCheckDigit("a78898912508", out ErrMsgIs)); //""
			Console.ReadLine();
		}

		/// <summary>
		/// 주민등록번호가 올바른 형식인 지 확인함.
		/// </summary>
		/// <param name="Value">확인할 주민번호</param>
		/// <param name="ErrMsgIs">리턴값이 false일 때 설정되는 에러 메세지</param>
		/// <example>
		/// string ErrMsgIs;
		/// 
		/// if (!IsJumin("710131-1121513", out ErrMsgIs))
		/// {
		/// 	Console.WriteLine(ErrMsgIs);
		/// }
		/// else 
		/// {
		/// 	Console.WriteLine("올바른 주민번호입니다");
		/// }
		/// </example>
		public static bool IsJumin(string Value, bool IsCheckDigit, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			int PosDash = Value.IndexOf('-');
			if (PosDash != -1)
			{
				Value = Value.Substring(0, PosDash) + Value.Substring(PosDash + 1);
			}

			if (!CValid.IsDigit(Value))
			{
				ErrMsgIs = "주민등록번호는 숫자만 가능합니다.";
				return false;
			}

			if (Value.Length != 13)
			{
				ErrMsgIs = "주민등록번호는 13자리여야만 합니다.";
				return false;
			}

			string DateCheck = Value.Substring(0, 2)
							+ "-" + Value.Substring(2, 2)
							+ "-" + Value.Substring(4, 2);
			if (!CValid.IsDateTime(DateCheck))
			{
				ErrMsgIs = "주민등록번호의 생년월일 부분은 날짜 형식이어야만 합니다.";
				return false;
			}

			int Sex = Convert.ToInt32(Value.Substring(6, 1));
			if ((Sex < 1) || (Sex > 4))
			{
				ErrMsgIs = "주민등록번호의 두번째 부분의 첫번째 자리는 성별을"
							+ " 나타내므로 1, 2, 3, 4만 가능합니다.";
				return false;
			}

			if (IsCheckDigit)
			{
				/*
				다음과 같은 방법으로 주민 번호의 마지막 번호가 만들어짐.
				1. 각 문자열에 2부터 1씩 증가시켜 9까지 순서대로 더함.
				2. 이 값들을 모두 더함.
				3. 모두 더한 값에서 11로 나눈 나머지 값을 구함.
				4. 11에서 그 나머지 값을 뺀 뒤, 그것을 10으로 나눈 나머지 값을 구함.
				*/
				int SumOfNum = (Convert.ToInt32(Convert.ToString(Value[0])) * 2)
							+ (Convert.ToInt32(Convert.ToString(Value[1])) * 3)
							+ (Convert.ToInt32(Convert.ToString(Value[2])) * 4)
							+ (Convert.ToInt32(Convert.ToString(Value[3])) * 5)
							+ (Convert.ToInt32(Convert.ToString(Value[4])) * 6)
							+ (Convert.ToInt32(Convert.ToString(Value[5])) * 7)
							+ (Convert.ToInt32(Convert.ToString(Value[6])) * 8)
							+ (Convert.ToInt32(Convert.ToString(Value[7])) * 9)
							+ (Convert.ToInt32(Convert.ToString(Value[8])) * 2)
							+ (Convert.ToInt32(Convert.ToString(Value[9])) * 3)
							+ (Convert.ToInt32(Convert.ToString(Value[10])) * 4)
							+ (Convert.ToInt32(Convert.ToString(Value[11])) * 5);

				int CheckDigit = (11 - (SumOfNum % 11)) % 10;
				if (Convert.ToInt32(Value.Substring(12, 1)) != CheckDigit)
				{
					ErrMsgIs = "올바른 형식의 주민등록번호가 아닙니다.";
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 주민번호의 체크디지트를 생성함.
		/// </summary>
		/// <param name="YYMMDD">YYMMDD 형식의 년월일</param>
		/// <param name="SixDigit">두번째 부분의 여섯자리</param>
		/// <param name="ErrMsgIs">리턴값이 ""일 때 설정되는 에러 메세지</param>
		/// <example>
		/// string ErrMsgIs;
		/// 
		/// Console.WriteLine(GenerateJuminNo("710131", "999999", out ErrMsgIs)); //""
		/// Console.WriteLine(GenerateJuminNo("710131", "abcdef", out ErrMsgIs)); //""
		/// Console.WriteLine(GenerateJuminNo("710131", "111111", out ErrMsgIs)); //0
		/// Console.WriteLine(GenerateJuminNo("710131", "112151", out ErrMsgIs)); //3
		/// </example>
		public static string GenerateJuminNo(string YYMMDD, string SixDigit,
										out string ErrMsgIs)
		{
			ErrMsgIs = "";

			string Value = YYMMDD + SixDigit;

			if (!CValid.IsDigit(Value))
			{
				ErrMsgIs = "주민등록번호는 숫자만 가능합니다.";
				return "";
			}

			if (Value.Length != 12)
			{
				ErrMsgIs = "마지막 숫자를 제외한 주민등록번호는 12자리여야만 합니다.";
				return "";
			}

			string DateCheck = Value.Substring(0, 2)
							+ "-" + Value.Substring(2, 2)
							+ "-" + Value.Substring(4, 2);
			if (!CValid.IsDateTime(DateCheck))
			{
				ErrMsgIs = "주민등록번호의 생년월일 부분은 날짜 형식이어야만 합니다.";
				return "";
			}

			int Sex = Convert.ToInt32(Value.Substring(6, 1));
			if ((Sex < 1) || (Sex > 4))
			{
				ErrMsgIs = "주민등록번호의 두번째 부분의 첫번째 자리는 성별을"
							+ " 나타내므로 1, 2, 3, 4만 가능합니다.";
				return "";
			}

			/*
			다음과 같은 방법으로 주민 번호의 마지막 번호가 만들어짐.
			1. 각 문자열에 2부터 1씩 증가시켜 9까지 순서대로 더함.
			2. 이 값들을 모두 더함.
			3. 모두 더한 값에서 11로 나눈 나머지 값을 구함.
			4. 11에서 그 나머지 값을 뺀 뒤, 그것을 10으로 나눈 나머지 값을 구함.
			*/
			int SumOfNum = (Convert.ToInt32(Convert.ToString(Value[0])) * 2)
				+ (Convert.ToInt32(Convert.ToString(Value[1])) * 3)
				+ (Convert.ToInt32(Convert.ToString(Value[2])) * 4)
				+ (Convert.ToInt32(Convert.ToString(Value[3])) * 5)
				+ (Convert.ToInt32(Convert.ToString(Value[4])) * 6)
				+ (Convert.ToInt32(Convert.ToString(Value[5])) * 7)
				+ (Convert.ToInt32(Convert.ToString(Value[6])) * 8)
				+ (Convert.ToInt32(Convert.ToString(Value[7])) * 9)
				+ (Convert.ToInt32(Convert.ToString(Value[8])) * 2)
				+ (Convert.ToInt32(Convert.ToString(Value[9])) * 3)
				+ (Convert.ToInt32(Convert.ToString(Value[10])) * 4)
				+ (Convert.ToInt32(Convert.ToString(Value[11])) * 5);

			int CheckDigit = (11 - (SumOfNum % 11)) % 10;

			return Value + CheckDigit;
		}

		/// <summary>
		/// 사업자번호가 올바른 형식인 지 확인함.
		/// </summary>
		/// <param name="Value">확인할 사업자번호</param>
		/// <param name="ErrMsgIs">리턴값이 false일 때 설정되는 에러 메세지</param>
		/// <example>
		/// string ErrMsgIs;
		/// 
		/// Console.WriteLine(IsSaupNo("1168135609", out ErrMsgIs)); //마니텔레콤
		/// Console.WriteLine(IsSaupNo("2168110233", out ErrMsgIs)); //(주)탐경
		/// Console.WriteLine(IsSaupNo("216-81-10233", out ErrMsgIs)); //(주)탐경
		/// Console.WriteLine(IsSaupNo("2168110230", out ErrMsgIs)); //false
		/// </example>
		public static bool IsSaupNo(string Value, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			//-를 삭제함.
			int PosDash = Value.IndexOf('-');
			if (PosDash != -1)
			{
				StringBuilder sb = new StringBuilder(Value);
				Value = sb.Replace("-", "").ToString();
			}

			if (!CValid.IsDigit(Value))
			{
				ErrMsgIs = "사업자등록번호가 숫자 형식이 아닙니다.";
				return false;
			}

			if (Value.Length != 10)
			{
				ErrMsgIs = "사업자등록번호가 10자리가 아닙니다.";
				return false;
			}

			/*
			다음과 같은 방법으로 사업자 번호의 마지막 번호가 만들어짐.
			1. 9번째 자리에 5를 곱함. 이때 한자리 수가 나오면 끝에 0을 붙임.
			2. 다음 a, b, c를 모두 더한 값을 구함.
			  a : 1의 결과에서 1, 4, 7번째의 숫자를 더함.
			  b : 2, 5, 8번째의 숫자를 더하고 3을 곱함.
			  c : 3, 6번째의 숫자를 더하고 7을 곱함.
			3. 구해진 값의 2번째 자리의 숫자를 구함.
			4. 9에서 구해진 값을 뺀 후, 10을 나눈 나머지 값을 구함.
			*/
			int[] aNum = new int[10];
			for (int i = 0; i < 10; i++)
			{
				aNum[i] = Convert.ToInt32(Value.Substring(i, 1));
			}

			string s = (aNum[8] * 5).ToString();
			if (s.Length == 1)
			{
				s += s + "0";
			}

			s = (
				(Convert.ToInt32(s) + aNum[0] + aNum[3] + aNum[6])
				+ ((aNum[1] + aNum[4] + aNum[7]) * 3)
				+ ((aNum[2] + aNum[5]) * 7)
				).ToString();

			s = s.Substring(1, 1);

			int CheckDigit = (9 - Convert.ToInt32(s)) % 10;
			if (CheckDigit != aNum[9])
			{
				ErrMsgIs = "올바른 사업자등록번호가 아닙니다.";
				return false;
			}

			return true;
		}

		[Obsolete("2013-12-10 use GetIsbnCheckDigit")]
		public static string GenerateISBNCheckDigit(string ISBN9, out string ErrMsgIs)
		{
			return GetIsbnCheckDigit(ISBN9, out ErrMsgIs);
		}

		/// <summary>
		/// ISBN 번호의 체크디지트를 생성함.
		/// </summary>
		/// <param name="Isbn9">ISBN 번호 중 마지막 자리를 제외한 아홉자리</param>
		/// <param name="ErrMsgIs">리턴값이 false일 때 설정되는 에러 메세지</param>
		/// <example>
		/// string ErrMsgIs;
		/// 
		/// Console.WriteLine(GetIsbnCheckDigit("898941800", out ErrMsgIs)); //3
		/// Console.WriteLine(GetIsbnCheckDigit("898524727", out ErrMsgIs)); //1
		/// Console.WriteLine(GetIsbnCheckDigit("898524728", out ErrMsgIs)); //X
		/// </example>
		public static string GetIsbnCheckDigit(string Isbn9, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			int nDigit = -1;
			int[] aDigit = new int[10];

			//숫자와 X(10을 의미)인 처음 9자리 문자열을 가져옴.
			for (int i = 0; i < Isbn9.Length; i++)
			{
				if (Isbn9[i] == '0'
					|| Isbn9[i] == '1'
					|| Isbn9[i] == '2'
					|| Isbn9[i] == '3'
					|| Isbn9[i] == '4'
					|| Isbn9[i] == '5'
					|| Isbn9[i] == '6'
					|| Isbn9[i] == '7'
					|| Isbn9[i] == '8'
					|| Isbn9[i] == '9')
				{
					nDigit++;
					if (nDigit < 9)
					{
						aDigit[nDigit] = Convert.ToInt32(Convert.ToString(Isbn9[i]));
					}
				}
				else if (Isbn9[i] == 'x'
					|| Isbn9[i] == 'X')
				{
					nDigit++;
					if (nDigit < 9)
					{
						aDigit[nDigit] = 10;
					}
				}
			}

			if (nDigit != 8)
			{
				ErrMsgIs = "ISBN9 인수의 숫자와 X를 합한 개수가 9자리가 아닙니다.";
				return "";
			}


			//ISBN9의 CheckDigit는 마지막 값까지 합해야 구해지므로
			//0~10까지 루핑을 하면서 마지막 값을 일일이 대입해서 맞는 값을 찾아내야 함.
			int nTotal = 0;
			for (int i = 0; i < 11; i++)
			{
				aDigit[9] = i;

				nTotal = 0;
				for (int j = 0; j < 10; j++)
				{
					nTotal += aDigit[j] * (10 - j);
				}

				if ((nTotal % 11) == 0)
				{
					if (i == 10)
					{
						return "X";
					}
					else
					{
						return i.ToString();
					}
				}
			}

			return "";
		}

		[Obsolete("2013-12-10 use IsIsbn")]
		public static bool IsISBN(string Value, out string ErrMsgIs)
		{
			return IsIsbn(Value, out ErrMsgIs);
		}

		/// <summary>
		/// ISBN 번호가 올바른 형식인 지 확인함.
		/// </summary>
		/// <param name="Value">ISBN 번호</param>
		/// <param name="ErrMsgIs">리턴값이 false일 때 설정되는 에러 메세지</param>
		/// <example>
		/// string ErrMsgIs;
		/// 
		/// if (!IsISBN("898524728x", out ErrMsgIs)) //true
		/// {
		/// 	Console.WriteLine(ErrMsgIs);
		/// }
		/// if (!IsISBN("898524728X", out ErrMsgIs)) //true
		/// {
		/// 	Console.WriteLine(ErrMsgIs);
		/// }
		/// </example>
		public static bool IsIsbn(string Value, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			string ValueExcept = Value.Substring(0, Value.Length - 1);
			string CheckDigitRight = GetIsbnCheckDigit(ValueExcept, out ErrMsgIs);
			if (string.IsNullOrEmpty(CheckDigitRight))
				return false;

			string CheckDigit = Value.Substring(Value.Length - 1, 1);
			return (CheckDigit == CheckDigitRight);
		}

		/// <summary>
		/// 휴대폰 번호가 올바른 형식인 지 확인함.
		/// </summary>
		/// <param name="Value">휴대폰 번호</param>
		/// <param name="ErrMsgIs">리턴값이 false일 때 설정되는 에러 메세지</param>
		/// <example>
		/// string ErrMsgIs;
		/// 
		/// if (!IsMobile("016)234-3748", out ErrMsgIs)) //true
		/// {
		/// 	Console.WriteLine(ErrMsgIs);
		/// }
		/// if (!IsMobile("0152343748", out ErrMsgIs)) //false
		/// {
		/// 	Console.WriteLine(ErrMsgIs);
		/// }
		/// if (!IsMobile("011 222 3333", out ErrMsgIs)) //false
		/// {
		/// 	Console.WriteLine(ErrMsgIs);
		/// }
		/// </example>
		public static bool IsMobile(string Value, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			const string drTextAllowed = "0123456789-)";
			const string drCompanyNum = "010,011,016,017,018,019";
			const int drMinLength = 10;
			const int drMaxLength = 11;

			string NewValue = "";
			for (int i = 0; i < Value.Length; i++)
			{
				if (drTextAllowed.IndexOf(Value[i]) == -1)
				{
					ErrMsgIs = "휴대폰 번호는 다음 문자열만 허용됩니다.\n"
								+ drTextAllowed;
					return false;
				}
				else
				{
					//-) 기호를 제외한 숫자만을 추가함.
					if ((Value[i] != '-') && Value[i] != ')')
					{
						NewValue += Value[i];
					}
				}
			}

			if (NewValue.Length < drMinLength)
			{
				ErrMsgIs = string.Format("휴대폰 번호는 최소한 {0}자리 이상의 숫자로 구성되어야 합니다.", drMinLength);
				return false;
			}
			else if (NewValue.Length > drMaxLength)
			{
				ErrMsgIs = string.Format("휴대폰 번호는 최대한 {1}자리 이하의 숫자로 구성되어야 합니다.", drMaxLength);
				return false;
			}

			string CompanyNum = NewValue.Substring(0, 3);
			if (drCompanyNum.IndexOf(CompanyNum, 0) == -1)
			{
				ErrMsgIs = "휴대폰 번호는 다음 문자열로 시작해야 합니다.\n"
							+ drCompanyNum;
				return false;
			}

			return true;
		}

		/// <summary>
		/// 전화번호가 형식에 맞게 제대로 입력되었는 지 확인하기 위함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="ErrMsgIs"></param>
		/// <returns></returns>
		public static bool IsPhone(string Value, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			const string drTextAllowed = "0123456789-)";
			const int drMinLength = 7;
			const int drMaxLength = 12;

			for (int i = 0; i <= Value.Length - 1; i++)
			{
				char c = Value[i];
				if (drTextAllowed.IndexOf(c) == -1)
				{
					ErrMsgIs = "전화번호는 다음 문자열만 허용됩니다\n" + drTextAllowed;
					return false;
				}
			}

			Value = CFindRep.RemoveExcept(Value, "0123456789");
			if (Value.Length < drMinLength)
			{
				ErrMsgIs = "전화번호는 최소한 " + drMinLength + "자리 이상의 숫자로 구성되어야 합니다";
				return false;
			}
			else if (Value.Length > drMaxLength)
			{
				ErrMsgIs = "전화번호는 최대한 " + drMaxLength + "자리 이하의 숫자로 구성되어야 합니다";
				return false;
			}

			return true;
		}

		/// <summary>
		/// 우편번호가 올바른 형식인 지 확인함.
		/// </summary>
		/// <param name="Value">우편번호</param>
		/// <param name="ErrMsgIs">리턴값이 false일 때 설정되는 에러 메세지</param>
		/// <example>
		/// Console.WriteLine(IsZipCode("435050", out ErrMsgIs)); //true
		/// Console.WriteLine(IsZipCode("435-050", out ErrMsgIs)); //true
		/// Console.WriteLine(IsZipCode("43550", out ErrMsgIs)); //false
		/// Console.WriteLine(IsZipCode("43-5050", out ErrMsgIs)); //false
		/// </example>
		public static bool IsZipCode(string Value, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			//435-050의 -를 뺌.
			int PosDash = Value.IndexOf('-');
			if (PosDash == 3)
			{
				Value = Value.Substring(0, 3) + Value.Substring(4, 3);
			}

			if (!CValid.IsDigit(Value))
			{
				ErrMsgIs = "우편번호가 숫자형식이 아닙니다.";
				return false;
			}

			if (Value.Length != 6)
			{
				ErrMsgIs = "우편번호가 6자리가 아닙니다.";
				return false;
			}

			return true;
		}

		/// <summary>
		/// 바코드의 CheckDigit를 생성함.
		/// </summary>
		/// <param name="ValueExceptCheckDigit">마지막 자리를 제외한 바코드</param>
		/// <param name="ErrMsgIs">리턴값이 false일 때 설정되는 에러 메세지</param>
		/// <example>
		/// string ErrMsgIs;
		/// 
		/// Console.WriteLine(GetBarcodeCheckDigit("978898912508", out ErrMsgIs)); //"2"
		/// Console.WriteLine(GetBarcodeCheckDigit("978895090425", out ErrMsgIs)); //"8"
		/// Console.WriteLine(GetBarcodeCheckDigit("a78898912508", out ErrMsgIs)); //""
		/// </example>
		//http://stackoverflow.com/a/10143593/2958717
		public static string GetBarcodeCheckDigit(string ValueExceptCheckDigit, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			if (ValueExceptCheckDigit != (new Regex("[^0-9]")).Replace(ValueExceptCheckDigit, ""))
			{
				ErrMsgIs = "숫자 형식이 아닙니다.";
				return "";
			}

			// pad with zeros to lengthen to 13 digits
			switch (ValueExceptCheckDigit.Length)
			{
				case 7:
					ValueExceptCheckDigit = "000000" + ValueExceptCheckDigit;
					break;
				case 11:
					ValueExceptCheckDigit = "00" + ValueExceptCheckDigit;
					break;
				case 12:
					ValueExceptCheckDigit = "0" + ValueExceptCheckDigit;
					break;
				case 13:
					break;
				default:
					ErrMsgIs = string.Format("잘못된 길이:{0}입니다.", ValueExceptCheckDigit.Length);
					return "";
			}

			// calculate check digit
			int[] a = new int[13];
			a[0] = int.Parse(ValueExceptCheckDigit[0].ToString()) * 3;
			a[1] = int.Parse(ValueExceptCheckDigit[1].ToString());
			a[2] = int.Parse(ValueExceptCheckDigit[2].ToString()) * 3;
			a[3] = int.Parse(ValueExceptCheckDigit[3].ToString());
			a[4] = int.Parse(ValueExceptCheckDigit[4].ToString()) * 3;
			a[5] = int.Parse(ValueExceptCheckDigit[5].ToString());
			a[6] = int.Parse(ValueExceptCheckDigit[6].ToString()) * 3;
			a[7] = int.Parse(ValueExceptCheckDigit[7].ToString());
			a[8] = int.Parse(ValueExceptCheckDigit[8].ToString()) * 3;
			a[9] = int.Parse(ValueExceptCheckDigit[9].ToString());
			a[10] = int.Parse(ValueExceptCheckDigit[10].ToString()) * 3;
			a[11] = int.Parse(ValueExceptCheckDigit[11].ToString());
			a[12] = int.Parse(ValueExceptCheckDigit[12].ToString()) * 3;
			int Sum = a[0] + a[1] + a[2] + a[3] + a[4] + a[5] + a[6] + a[7] + a[8] + a[9] + a[10] + a[11] + a[12];
			int CheckDigit = (10 - (Sum % 10)) % 10;

			return CheckDigit.ToString();
		}
		public static string GetBarcodeCheckDigit(string ValueExceptCheckDigit)
		{
			string ErrMsgIs;
			return GetBarcodeCheckDigit(ValueExceptCheckDigit, out ErrMsgIs);
		}

		public static bool IsBarcode(string Value)
		{
			if (string.IsNullOrEmpty(Value))
				return false;

			string ValueExcept = Value.Substring(0, Value.Length - 1);
			string CheckDigit = Value[Value.Length - 1].ToString();

			string ErrMsgIs;
			string CheckDigitRight = CValid.GetBarcodeCheckDigit(ValueExcept, out ErrMsgIs);
			if (string.IsNullOrEmpty(CheckDigitRight))
				return false;

			return (CheckDigit == CheckDigitRight.ToString());
		}


		/// <summary>
		/// string 배열의 모든 항목이 값이 있는 지를 검사함.
		/// </summary>
		/// <param name="aValue"></param>
		/// <param name="IndexHasNoValue"></param>
		/// <param name="ErrMsgIs"></param>
		/// <returns></returns>
		public static bool IsAllHasValue(string[] aValue,
			out int IndexHasNoValue, out string ErrMsgIs)
		{
			IndexHasNoValue = -1;
			ErrMsgIs = "";

			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				if (string.IsNullOrEmpty(aValue[i]))
				{
					if (i == 0)
					{
						ErrMsgIs = "첫번째 값이 비었습니다.";
					}
					else
					{
						ErrMsgIs = (i + 1).ToString() + "번째 값이 비었습니다.(" + aValue[i - 1] + " 값 다음 값)";
					}

					IndexHasNoValue = -1;
					return false;
				}
			}

			return true;
		}

		public static bool IsEmail(string Email, string[] aDisallowedServer, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			int PosApos = Email.IndexOf('@');
			if (PosApos == -1)
			{
				ErrMsgIs = "@ 기호가 없으므로 잘못된 Email 형식입니다.";
				return false;
			}

			int Pos2nd = PosApos + 1;

			int PosDot = Email.IndexOf('.', Pos2nd);
			if (PosDot == -1)
			{
				ErrMsgIs = "Email의 '아이디@서버주소.도메인형식'의 서버주소와 도메인형식 사이에는 점(.)이 있어야 합니다";
				return false;
			}

			int Pos3rd = PosDot + 1;

			string Word1st = Email.Substring(0, PosApos);
			string Word2nd = Email.Substring(Pos2nd, Pos3rd - Pos2nd - 1);
			string Word3rd = Email.Substring(Pos3rd);

			if (Word1st.Length == 0)
			{
				ErrMsgIs = "Email의 '아이디@서버주소.도메인형식'의 아이디가 비었습니다.";
				return false;
			}
			if (Word2nd.Length == 0)
			{
				ErrMsgIs = "Email의 '아이디@서버주소.도메인형식'의 서버주소가 비었습니다.";
				return false;
			}
			if (Word3rd.Length == 0)
			{
				ErrMsgIs = "Email의 '아이디@서버주소.도메인형식'의 도메인형식이 비었습니다.";
				return false;
			}

			if (Email.IndexOf("@@") != -1)
			{
				ErrMsgIs = "Email에 '@' 문자열이 연속으로 2개 이상 있습니다.";
				return false;
			}
			else if (Email.IndexOf("..") != -1)
			{
				ErrMsgIs = "Email에 '.' 문자열이 연속으로 2개 이상 있습니다.";
				return false;
			}

			if ((aDisallowedServer != null) && (aDisallowedServer.Length > 0))
			{
				int Idx = CArray.IndexOf(aDisallowedServer, Word2nd + "." + Word3rd, true);
				if (Idx != -1)
				{
					ErrMsgIs = aDisallowedServer[Idx] + " 서버는 메일 주소로 사용할 수 없습니다.";
					return false;
				}
			}

			return true;
		}

		public static bool IsUrl(string Url)
		{
			//뒤에 파라미터 붙은 주소는 false를 리턴해 주석(예:http://www.testinter.net/tool/download.aspx?UploadType=Good_ImageNamePreview&Key=717)
			//string Pattern = "^(https?://)"
			//+ "?(([0-9a-z_!~*'().&=+$%-]+: )?[0-9a-z_!~*'().&=+$%-]+@)?" //user@ 
			//+ @"(([0-9]{1,3}\.){3}[0-9]{1,3}" // IP- 199.194.52.184 
			//+ "|" // allows either IP or domain 
			//+ @"([0-9a-z_!~*'()-]+\.)*" // tertiary domain(s)- www. 
			//+ @"([0-9a-z][0-9a-z-]{0,61})?[0-9a-z]\." // second level domain 
			//+ "[a-z]{2,6})" // first level domain- .com or .museum 
			//+ "(:[0-9]{1,4})?" // port number- :80 
			//+ "((/?)|" // a slash isn't required if there is no file name 
			//+ @"(/[^:\*\?""<>\|]+)+/?)$";
			////+ "(/[0-9a-z_!~*'().;?:@&=+$,%#-]+)+/?)$";
			//Regex re = new Regex(Pattern, RegexOptions.IgnoreCase);

			//return re.IsMatch(Url);

			return Uri.IsWellFormedUriString(Url, UriKind.Absolute);
		}

		public static bool IsFileFolderName(string FileName, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			for (int i = 0, i2 = FileName.Length; i < i2; i++)
			{
				char c = FileName[i];
				if (CValid.DisallowedListForFileName.IndexOf(c) != -1)
				{
					ErrMsgIs = (i + 1).ToString() + "번째 문자열(" + c.ToString() + ")은 파일/폴더 이름으로 허용되지 않습니다.";
					return false;
				}
			}

			//파일명, 폴더명에 마지막에 점은 허용되지 않음.
			if (FileName.EndsWith("."))
			{
				ErrMsgIs = "파일/폴더 이름으로 마지막에 점(.)은 허용되지 않습니다.";
				return false;
			}

			//파일명, 폴더명의 양쪽에 스페이스는 허용되지 않음.
			if (FileName.StartsWith(" "))
			{
				ErrMsgIs = "파일/폴더 이름은 공백으로 시작할 수 없습니다.";
				return false;
			}
			if (FileName.EndsWith(" "))
			{
				ErrMsgIs = "파일/폴더 이름은 공백으로 끝날 수 없습니다.";
				return false;
			}

			return true;
		}

		/// <summary>
		/// 문자열이 숫자 형식으로 변환될 수 있는 지 여부를 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static bool IsNumber(string Value)
		{
			Regex r = new Regex(CRegex.Pattern.Number, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
			bool b = r.IsMatch(Value);
			return b;
		}
		public static bool IsNumber(object Value)
		{
			if (Value == null)
				return false;

			string sValue = Value.ToString();
			return IsNumber(sValue);
		}

		[Obsolete("Use IsDigit")]
		public static bool IsIntegerUnsigned(string Value)
		{
			if (string.IsNullOrEmpty(Value))
				return false;

			for (var i = 0; i < Value.Length; i++)
			{
				if (!char.IsDigit(Value, i))
					return false;
			}

			return true;
		}
		[Obsolete("Use IsDigit")]
		public static bool IsIntegerUnsigned(object Value)
		{
			if (Value == null)
				return false;

			string sValue = Value.ToString();
			return IsIntegerUnsigned(sValue);
		}

		public static bool IsDigit(string Value)
		{
			foreach (char c in Value)
			{
				if (!char.IsDigit(c))
					return false;
			}

			return true;
		}
		public static bool IsLower(string Value)
		{
			foreach (char c in Value)
			{
				if (!char.IsLower(c))
					return false;
			}

			return true;
		}
		public static bool IsDigitOrLower(string Value)
		{
			foreach (char c in Value)
			{
				if (!char.IsDigit(c) && !char.IsLower(c))
					return false;
			}

			return true;
		}

		public static bool IsCurrencyWon(string Value)
		{
			Regex r = new Regex(CRegex.Pattern.CurrencyWon, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
			return r.IsMatch(Value);
		}

		/// <summary>
		/// 다음 규칙에 맞는 유효한 아이디인 지 확인함.
		/// 길이는 MinLength부터 MaxLength까지, 영어와 숫자만 가능, 첫자는 영어만 가능.
		/// </summary>
		/// <param name="Id"></param>
		/// <param name="MinLength"></param>
		/// <param name="MaxLength"></param>
		/// <param name="IsAllowHangul"></param>
		/// <param name="ErrMsgIs"></param>
		/// <returns></returns>
		public static bool IsId(string Id, int MinLength, int MaxLength, bool IsAllowHangulHanja, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			const string AlphaList = "abcdefghijklmnopqrstuvwxyz";
			const string NumList = "1234567890";
			const string AlphaNumList = AlphaList + NumList;

			if (string.IsNullOrEmpty(Id))
			{
				ErrMsgIs = "아이디를 입력하지 않았습니다.";
				return false;
			}

			int Len = CTwoByte.LenH(Id);

			if ((Len < MinLength) || (Len > MaxLength))
			{
				ErrMsgIs = "아이디는 " + MinLength + " ~ " + MaxLength + "자 사이만 가능합니다";
				return false;
			}


			char FirstChar = Id[0];

			if (IsAllowHangulHanja)
			{
				bool IsHangul = CValid.IsHangul(FirstChar);
				bool IsHanja = CValid.IsHanja(FirstChar);
				bool IsAlpha = (AlphaList.IndexOf(FirstChar) != -1);
				if (!IsHangul && !IsHanja && !IsAlpha)
				{
					ErrMsgIs = "아이디의 첫번째는 한글, 한자, 영문만 가능합니다.";
					return false;
				}

				for (int i = 0, i2 = Id.Length; i < i2; i++)
				{
					char c = Id[i];

					bool IsHangulCur = CValid.IsHangul(c);
					bool IsHanjaCur = CValid.IsHanja(c);
					bool IsAlphaOrNumCur = (AlphaNumList.IndexOf(c) != -1);

					if (!IsHangulCur && !IsHanja && !IsAlphaOrNumCur)
					{
						ErrMsgIs = "아이디는 한글, 한자, 영문, 숫자만 가능합니다.";
						return false;
					}
				}
			}
			else
			{
				if (AlphaList.IndexOf(FirstChar) == -1)
				{
					ErrMsgIs = "아이디의 첫번째는 영문만 가능합니다";
					return false;
				}

				for (int i = 0, i2 = Id.Length; i < i2; i++)
				{
					char c = Id[i];

					bool IsAlphaOrNumCur = (AlphaNumList.IndexOf(c) != -1);

					if (!IsAlphaOrNumCur)
					{
						ErrMsgIs = "아이디는 영문, 숫자만 가능합니다";
						return false;
					}
				}
			}

			switch (Id)
			{
				case "sys":
				case "system":
				case "admin":
				case "administrator":
				case "master":
				case "webmaster":
				case "developer":
				case "dev":
				case "helper":
				case "관리자":
				case "운영자":
				case "개발자":
				case "도우미":
					ErrMsgIs = Id + "는 시스템에서 사용되므로 허용되지 않는 아이디입니다.";
					return false;
			}

			return true;
		}
		public static bool IsNickName(string NickName, int MinLength, int MaxLength, bool IsAllowHangulHanja, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			if (!IsId(NickName, MinLength, MaxLength, IsAllowHangulHanja, out ErrMsgIs))
			{
				ErrMsgIs = ErrMsgIs
					.Replace("아이디는", "별명은")
					.Replace("아이디를", "별명을")
					.Replace("아이디", "별명");
				return false;
			}

			return true;
		}

		public static bool IsHanja(char c)
		{
			//㐀(13312) ~ 䶵(19893): 문자표에는 없음
			//一(19968) ~ 龥(40869): CJK Unified Ideograph
			//豈(63744) ~ 鶴(64045): CJK Compatibility Ideograph

			int i = (int)c;

			return (((i >= 13312) && (i <= 19893))
				|| ((i >= 19968) && (i <= 40869))
				|| ((i >= 63744) && (i <= 64045)));
		}
		public static bool IsHanja(string Value)
		{
			foreach (char c in Value)
			{
				if (!IsHanja(c))
					return false;
			}

			return true;
		}

		public static bool IsHangul(char c)
		{
			int i = (int)c;
			//가(44032)
			//힣(55203)

			return ((i >= 44032) && (i <= 55203));
		}
		public static bool IsHangul(string Value)
		{
			foreach (char c in Value)
			{
				if (!IsHangul(c))
					return false;
			}

			return true;
		}

		public static bool IsAlpha(char c)
		{
			int i = (int)c;
			//'A'	65
			//'Z'	90
			//'a'	97
			//'z'	122

			return ((i >= 65) && (i <= 90))
				|| ((i >= 97) && (i <= 122));
		}
		public static bool IsAlpha(string Value)
		{
			foreach (char c in Value)
			{
				if (!IsAlpha(c))
					return false;
			}

			return true;
		}

		/// <summary>
		/// 키보드로 입력할 수 있는 특수문자인 경우만 true를 리턴함.
		/// </summary>
		public static bool IsSpecialCharInKeyboard(char c)
		{
			string List = "~!@#$%^&*()_+|`-=\\{}:\"<>?[];',./ \t\r\n";
			return (List.IndexOf(c) != -1);
		}
		public static bool IsSpecialCharInKeyboard(string Value)
		{
			foreach (char c in Value)
			{
				if (!IsSpecialCharInKeyboard(c))
					return false;
			}

			return true;
		}

		public static bool IsDateTime(string Value)
		{
			DateTime d;

			if (Value == null)
				return false;

			return DateTime.TryParse(Value, out d);
		}

		public static bool IsIpAddress(string Value)
		{
			Regex re = new Regex(CRegex.Pattern.IpAddress, RegexOptions.IgnoreCase);
			return re.IsMatch(Value);
		}

		public static bool IsDBNullOrNullOrEmpty(object Value)
		{
			if (Value == DBNull.Value)
				return true;
			else if (Value == null)
				return true;
			else if (Value.ToString() == string.Empty)
				return true;

			return false;
		}

		/// <summary>
		/// 허용된 값만 있는 지 확인함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="AllowedList"></param>
		/// <example>
		/// <![CDATA[
		/// //다음은 숫자와 콤마만 허용하는 경우임.
		/// bool IsValid = false;
		/// IsValid = CValid.HasOnlyAllowedValue("1234", "1234567890,"); //true
		/// IsValid = CValid.HasOnlyAllowedValue("1234,", "1234567890,"); //true
		/// IsValid = CValid.HasOnlyAllowedValue("1234.", "1234567890,"); //false
		/// ]]>
		/// </example>
		public static bool HasOnlyAllowedValue(string Value, string AllowedList)
		{
			char[] aValue = Value.ToCharArray();
			bool HasDisallowed = aValue.Any(delegate(char c)
			{
				return (AllowedList.IndexOf(c) == -1);
			});

			return !HasDisallowed;
		}
	}
}



