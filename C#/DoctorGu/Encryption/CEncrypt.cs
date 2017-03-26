using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

namespace DoctorGu.Encryption
{
	/// <summary>
	/// 문자열의 암호화, 복호화 관련 기능
	/// </summary>
	public class CEncrypt
	{
		/// <summary>
		/// 암호에 쓰이는 문자열(키보드 상에 있는 영어, 숫자, 기호)을 간단한 방식으로 암호화함.
		/// 한글이나 특수문자의 경우 제대로 작동하지 않음.
		/// </summary>
		/// <param name="DecryptedPassword">암호화할 문자열</param>
		/// <returns>암호화된 문자열</returns>
		/// <example>
		/// 다음은 암호화, 복호화하는 과정입니다.
		/// <code>
		/// string s = CEncrypt.EncryptPassword("It'sMyPassword!@#");
		/// s = CEncrypt.DecryptPassword(s);
		/// Console.WriteLine(s); //"It'sMyPassword!@#"
		/// </code>
		/// </example>
		public static string EncryptPassword(string DecryptedPassword)
		{
			string Password = "";
			int Number = DecryptedPassword.Length % 10;

			for (int i = 0, i2 = DecryptedPassword.Length; i < i2; i++)
			{
				int Temp = (int)DecryptedPassword[i];
				if ((i % 2) == 1)
					Temp -= Number;
				else
					Temp += Number;

				Temp = Temp ^ (10 - Number);
				Password += (char)Temp;
			}

			return Password;
		}

		/// <summary>
		/// 암호에 쓰이는 문자열(키보드 상에 있는 영어, 숫자, 기호)을 간단한 방식으로 복호화함.
		/// 한글이나 특수문자의 경우 제대로 작동하지 않음.
		/// </summary>
		/// <param name="EncryptedPassword">복호화할 문자열</param>
		/// <returns>복호화된 문자열</returns>
		/// <example>
		/// 다음은 암호화, 복호화하는 과정입니다.
		/// <code>
		/// string s = CEncrypt.EncryptPassword("It'sMyPassword!@#");
		/// s = CEncrypt.DecryptPassword(s);
		/// Console.WriteLine(s); //"It'sMyPassword!@#"
		/// </code>
		/// </example>
		public static string DecryptPassword(string EncryptedPassword)
		{
			string Password = "";
			int Number = EncryptedPassword.Length % 10;

			for (int i = 0, i2 = EncryptedPassword.Length; i < i2; i++)
			{
				int Temp = ((int)EncryptedPassword[i]) ^ (10 - Number);
				if ((i % 2) == 1)
					Temp += Number;
				else
					Temp -= Number;

				Password += (char)Temp;
			}

			return Password;
		}

		public static string Encrypt(string Decrypted, int EncryptionKey)
		{
			string Password = "";
			int Number = Decrypted.Length % EncryptionKey;

			for (int i = 0, i2 = Decrypted.Length; i < i2; i++)
			{
				int Temp = (int)Decrypted[i];
				if ((i % 2) == 1)
					Temp -= Number;
				else
					Temp += Number;

				Temp = Temp ^ (EncryptionKey - Number);
				Password += (char)Temp;
			}

			return Password;
		}
		public static string Decrypt(string Encrypted, int EncryptionKey)
		{
			string Password = "";
			int Number = Encrypted.Length % EncryptionKey;

			for (int i = 0, i2 = Encrypted.Length; i < i2; i++)
			{
				int Temp = ((int)Encrypted[i]) ^ (EncryptionKey - Number);
				if ((i % 2) == 1)
					Temp += Number;
				else
					Temp -= Number;

				Password += (char)Temp;
			}

			return Password;
		}

		public static string EncryptForSilverlightInitParams(string Value)
		{
			if (Value == null)
				return Value;

			return Value.Replace("=", "&eq;").Replace(",", "&cm;");
		}
		public static string DecryptForSilverlightInitParams(string Value)
		{
			if (Value == null)
				return Value;

			return Value.Replace("&eq;", "=").Replace("&cm;", ",");
		}

		/// <remarks>동적 Compile 시에 사용되므로 변경하지 말 것</remarks>
		public static string EncryptForExecutableArguments(string Value)
		{
			if (Value == null)
				return Value;

			return Value.Replace("/", "&#47;").Replace(" ", "&#32;");
		}
		/// <remarks>동적 Compile 시에 사용되므로 변경하지 말 것</remarks>
		public static string DecryptForExecutableArguments(string Value)
		{
			if (Value == null)
				return Value;

			return Value.Replace("&#47;", "/").Replace("&#32;", " ");
		}

		/// <summary>
		/// <![CDATA[
		/// \u003c -> <
		/// ]]>
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string DecryptUnicode(string value)
		{
			return Regex.Replace(
				value,
				@"\\u(?<Value>[a-zA-Z0-9]{4})",
				m =>
				{
					return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
				});
		}

		public static string EncryptNumber(long Decrypted)
		{
			int[] aKey = GetKeyForEncryptNumber();
			int Antilog = GetAntilogForEncryptNumber();

			string sDecrypted = Decrypted.ToString(CFindRep.Repeat('0', aKey.Length));
			string[] aDecrypted = CArray.SplitByLength(sDecrypted, 1);

			for (int i = 0; i < aDecrypted.Length; i++)
			{
				aDecrypted[i] = CMath.GetNFrom10((Convert.ToInt32(aDecrypted[i]) ^ aKey[i % 10]), Antilog);
			}

			return string.Join("", aDecrypted);
		}
		public static long DecryptNumber(string Encrypted)
		{
			int[] aKey = GetKeyForEncryptNumber();
			int Antilog = GetAntilogForEncryptNumber();

			string[] aEncrypted = CArray.SplitByLength(Encrypted, 1);

			for (int i = 0; i < aEncrypted.Length; i++)
			{
				aEncrypted[i] = (CMath.Get10FromN(aEncrypted[i], Antilog) ^ aKey[i % 10]).ToString();
			}

			return Convert.ToInt64(string.Join("", aEncrypted));
		}
		private static int[] GetKeyForEncryptNumber()
		{
			return new int[10] { 1, 3, 9, 4, 6, 8, 2, 7, 0, 5 };
		}
		//XOR하면 10을 넘을 수 있으므로 10을 넘지 않게 하기 위함.
		private static int GetAntilogForEncryptNumber()
		{
			return 32;
		}
	}
}
