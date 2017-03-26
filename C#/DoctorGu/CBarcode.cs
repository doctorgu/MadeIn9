using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public enum BarcodeEanType
	{
		Ean8,
		Ean13,
	}

	public class CBarcode
	{
		//http://www.jtbarton.com/Barcodes/BarcodeStringBuilderExample.aspx
		/// <summary>
		/// Converts an input string to the equivilant string, that need to be produced using the 'Code 128' font.
		/// </summary>
		/// <param name="value">String to be encoded</param>
		/// <returns>Encoded string start/stop and checksum characters included</returns>
		public static string ToCode128(string value)
		{
			// Parameters : a string
			// Return     : a string which give the bar code when it is dispayed with CODE128.TTF font
			// 			 : an empty string if the supplied parameter is no good
			int charPos, minCharPos;
			int currentChar, checksum;
			bool isTableB = true, isValid = true;
			string returnValue = string.Empty;

			if (value.Length > 0)
			{

				// Check for valid characters
				for (int charCount = 0; charCount < value.Length; charCount++)
				{
					//currentChar = char.GetNumericValue(value, charPos);
					currentChar = (int)char.Parse(value.Substring(charCount, 1));
					if (!(currentChar >= 32 && currentChar <= 126))
					{
						isValid = false;
						break;
					}
				}

				// Barcode is full of ascii characters, we can now process it
				if (isValid)
				{
					charPos = 0;
					while (charPos < value.Length)
					{
						if (isTableB)
						{
							// See if interesting to switch to table C
							// yes for 4 digits at start or end, else if 6 digits
							if (charPos == 0 || charPos + 4 == value.Length)
								minCharPos = 4;
							else
								minCharPos = 6;


							minCharPos = IsNumber(value, charPos, minCharPos);

							if (minCharPos < 0)
							{
								// Choice table C
								if (charPos == 0)
								{
									// Starting with table C
									returnValue = ((char)205).ToString(); // char.ConvertFromUtf32(205);
								}
								else
								{
									// Switch to table C
									returnValue = returnValue + ((char)199).ToString();
								}
								isTableB = false;
							}
							else
							{
								if (charPos == 0)
								{
									// Starting with table B
									returnValue = ((char)204).ToString(); // char.ConvertFromUtf32(204);
								}

							}
						}

						if (!isTableB)
						{
							// We are on table C, try to process 2 digits
							minCharPos = 2;
							minCharPos = IsNumber(value, charPos, minCharPos);
							if (minCharPos < 0) // OK for 2 digits, process it
							{
								currentChar = int.Parse(value.Substring(charPos, 2));
								currentChar = currentChar < 95 ? currentChar + 32 : currentChar + 100;
								returnValue = returnValue + ((char)currentChar).ToString();
								charPos += 2;
							}
							else
							{
								// We haven't 2 digits, switch to table B
								returnValue = returnValue + ((char)200).ToString();
								isTableB = true;
							}
						}
						if (isTableB)
						{
							// Process 1 digit with table B
							returnValue = returnValue + value.Substring(charPos, 1);
							charPos++;
						}
					}

					// Calculation of the checksum
					checksum = 0;
					for (int loop = 0; loop < returnValue.Length; loop++)
					{
						currentChar = (int)char.Parse(returnValue.Substring(loop, 1));
						currentChar = currentChar < 127 ? currentChar - 32 : currentChar - 100;
						if (loop == 0)
							checksum = currentChar;
						else
							checksum = (checksum + (loop * currentChar)) % 103;
					}

					// Calculation of the checksum ASCII code
					checksum = checksum < 95 ? checksum + 32 : checksum + 100;
					// Add the checksum and the STOP
					returnValue = returnValue +
						((char)checksum).ToString() +
						((char)206).ToString();
				}
			}

			return returnValue;
		}
		private static int IsNumber(string InputValue, int CharPos, int MinCharPos)
		{
			// if the MinCharPos characters from CharPos are numeric, then MinCharPos = -1
			MinCharPos--;
			if (CharPos + MinCharPos < InputValue.Length)
			{
				while (MinCharPos >= 0)
				{
					if ((int)char.Parse(InputValue.Substring(CharPos + MinCharPos, 1)) < 48
						|| (int)char.Parse(InputValue.Substring(CharPos + MinCharPos, 1)) > 57)
					{
						break;
					}
					MinCharPos--;
				}
			}
			return MinCharPos;
		}


		//http://www.jtbarton.com/Barcodes/BarcodeStringBuilderExample.aspx
		/// <summary>
		/// Converts an input string to the equivilant string, that need to be produced using the 'Code 3 de 9' font.
		/// </summary>
		/// <param name="value">String to be encoded</param>
		/// <param name="addChecksum">Is checksum to be added</param>
		/// <returns>Encoded string start/stop and checksum characters included</returns>
		public static string ToCode39(string value, bool addChecksum)
		{
			// Parameters : a string
			// Return     : a string which give the bar code when it is dispayed with CODE128.TTF font
			// 			 : an empty string if the supplied parameter is no good
			bool isValid = true;
			char currentChar;
			string returnValue = string.Empty;
			int checksum = 0;
			if (value.Length > 0)
			{

				//Check for valid characters
				for (int CharPos = 0; CharPos < value.Length; CharPos++)
				{
					currentChar = char.Parse(value.Substring(CharPos, 1));
					if (!((currentChar >= '0' && currentChar <= '9') || (currentChar >= 'A' && currentChar <= 'Z') ||
						currentChar == ' ' || currentChar == '-' || currentChar == '.' || currentChar == '$' ||
						currentChar == '/' || currentChar == '+' || currentChar == '%'))
					{
						isValid = false;
						break;
					}
				}
				if (isValid)
				{
					// Add start char
					returnValue = "*";
					// Add other chars, and calc checksum
					for (int CharPos = 0; CharPos < value.Length; CharPos++)
					{
						currentChar = char.Parse(value.Substring(CharPos, 1));
						returnValue += currentChar.ToString();
						if (currentChar >= '0' && currentChar <= '9')
						{
							checksum = checksum + (int)currentChar - 48;
						}
						else if (currentChar >= 'A' && currentChar <= 'Z')
						{
							checksum = checksum + (int)currentChar - 55;
						}
						else
						{
							switch (currentChar)
							{
								case '-':
									checksum = checksum + (int)currentChar - 9;
									break;
								case '.':
									checksum = checksum + (int)currentChar - 9;
									break;
								case '$':
									checksum = checksum + (int)currentChar + 3;
									break;
								case '/':
									checksum = checksum + (int)currentChar - 7;
									break;
								case '+':
									checksum = checksum + (int)currentChar - 2;
									break;
								case '%':
									checksum = checksum + (int)currentChar + 5;
									break;
								case ' ':
									checksum = checksum + (int)currentChar + 6;
									break;
							}
						}
					}
					// Calculation of the checksum ASCII code
					if (addChecksum)
					{
						checksum = checksum % 43;
						if (checksum >= 0 && checksum <= 9)
						{
							returnValue += ((char)(checksum + 48)).ToString();
						}
						else if (checksum >= 10 && checksum <= 35)
						{
							returnValue += ((char)(checksum + 55)).ToString();
						}
						else
						{
							switch (checksum)
							{
								case 36:
									returnValue += "-";
									break;
								case 37:
									returnValue += ".";
									break;
								case 38:
									returnValue += " ";
									break;
								case 39:
									returnValue += "$";
									break;
								case 40:
									returnValue += "/";
									break;
								case 41:
									returnValue += "+";
									break;
								case 42:
									returnValue += "%";
									break;
							}
						}
					}
					// Add stop char
					returnValue += "*";
				}
			}
			return returnValue;
		}
		/// <summary>
		/// Converts an input string to the equivilant string, that need to be produced using the 'Code 3 de 9' font.
		/// </summary>
		/// <param name="value">String to be encoded</param>
		/// <returns>Encoded string start/stop characters included</returns>
		public static string ToCode39(string value)
		{
			return ToCode39(value, false);
		}

		//http://www.codeproject.com/Articles/10162/Creating-EAN-13-Barcodes-with-C
		/// <summary>
		/// EAN-8, EAN-13 형식으로 변환함.
		/// ean13.ttf 글꼴 필요
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="EanType"></param>
		/// <returns></returns>
		public static string ToEan(string Value, BarcodeEanType EanType)
		{
			string ErrMsgIs;
			if (!IsValidEan(Value, EanType, out ErrMsgIs))
			{
				throw new Exception(ErrMsgIs);
			}

			string NewValue = "";

			if (EanType == BarcodeEanType.Ean8)
			{
				NewValue = ":";

				for (int i = 1; i <= 4; i++)
				{
					NewValue = NewValue + Convert.ToChar(65 + Convert.ToInt32(Value.Substring(i - 1, 1)));
				}
				NewValue = NewValue + "*";
				for (int i = 5; i <= 8; i++)
				{
					NewValue = NewValue + Convert.ToChar(97 + Convert.ToInt32(Value.Substring(i - 1, 1)));
				}
				NewValue = NewValue + "+";
			}
			else if (EanType == BarcodeEanType.Ean13)
			{
				NewValue = Value.Substring(0, 1) + Convert.ToChar(65 + Convert.ToInt32((Value.Substring(1, 1))));
				int First = Convert.ToInt32(Value.Substring(0, 1));
				bool IsTableA;

				for (int i = 3; i <= 7; i++)
				{
					IsTableA = false;
					switch (i)
					{
						case 3:
							switch (First)
							{
								case 0:
								case 1:
								case 2:
								case 3:
									IsTableA = true;
									break;
							}
							break;
						case 4:
							switch (First)
							{
								case 0:
								case 4:
								case 7:
								case 8:
									IsTableA = true;
									break;
							}
							break;
						case 5:
							switch (First)
							{
								case 0:
								case 1:
								case 4:
								case 5:
								case 9:
									IsTableA = true;
									break;
							}
							break;
						case 6:
							switch (First)
							{
								case 0:
								case 2:
								case 5:
								case 6:
								case 7:
									IsTableA = true;
									break;
							}
							break;
						case 7:
							switch (First)
							{
								case 0:
								case 3:
								case 6:
								case 8:
								case 9:
									IsTableA = true;
									break;
							}
							break;
					}
					if (IsTableA)
					{
						NewValue = NewValue + Convert.ToChar(65 + Convert.ToInt32(Value.Substring(i - 1, 1)));
					}
					else
					{
						NewValue = NewValue + Convert.ToChar(75 + Convert.ToInt32(Value.Substring(i - 1, 1)));
					}
				}
				NewValue = NewValue + "*";
				for (int i = 8; i <= 13; i++)
				{
					NewValue = NewValue + Convert.ToChar(97 + Convert.ToInt32(Value.Substring(i - 1, 1)));
				}
				NewValue = NewValue + "+";
			}


			return NewValue;
		}

		public static bool IsValidEan(string Value, BarcodeEanType EanType, out string ErrMsgIs)
		{
			ErrMsgIs = "";

			int FixedLength = 0;
			if (EanType == BarcodeEanType.Ean8)
			{
				FixedLength = 8;
			}
			else if (EanType == BarcodeEanType.Ean13)
			{
				FixedLength = 13;
			}
			else
			{
				ErrMsgIs = string.Format("잘못된 EanType:{0}입니다.", EanType);
				return false;
			}

			if (Value.Length != FixedLength)
			{
				ErrMsgIs = string.Format("길이는 {0}자리만 허용됩니다.", FixedLength);
				return false;
			}

			string ValueWithoutCheck = Value.Substring(0, FixedLength - 1);

			string CheckDigitRight = CValid.GetBarcodeCheckDigit(ValueWithoutCheck, out ErrMsgIs);
			if (string.IsNullOrEmpty(CheckDigitRight))
				return false;

			string CheckDigit = Value.Substring(FixedLength - 1, 1);
			if (CheckDigitRight != CheckDigit)
			{
				ErrMsgIs = string.Format("잘못된 검증번호:{0}", CheckDigit);
				return false;
			}


			return true;
		}

		//public static string GetEanCheckDigit(string Value, BarcodeEanType EanType)
		//{
		//    double CheckDigit = 0;

		//    int Start1 = 0, Start2 = 0;
		//    if (EanType == BarcodeEanType.Ean8)
		//    {
		//        Start1 = 7;
		//        Start2 = 6;
		//    }
		//    else if (EanType == BarcodeEanType.Ean13)
		//    {
		//        Start1 = 12;
		//        Start2 = 11;
		//    }
		//    else
		//    {
		//        throw new Exception(string.Format("Wrong EanType:{0}", EanType));
		//    }

		//    for (int i = Start1; i > 0; i -= 2)
		//    {
		//        CheckDigit = CheckDigit + Convert.ToInt32(Value.Substring(i - 1, 1));
		//    }
		//    CheckDigit = CheckDigit * 3;

		//    for (int i = Start2; i > 0; i -= 2)
		//    {
		//        CheckDigit = CheckDigit + Convert.ToInt32(Value.Substring(i - 1, 1));
		//    }

		//    CheckDigit = (10 - CheckDigit % 10) % 10;

		//    return CheckDigit.ToString();
		//}
	}
}
