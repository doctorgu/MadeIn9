using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Security.Cryptography;

namespace DoctorGu.Encryption
{
	/// <summary>
	/// Data Protection API
	/// </summary>
	/// <example>
	/// <![CDATA[
	/// string Encrypted = CDPApi.EncryptString(CDPApi.ToSecureString("1234"));
	/// string Decrypted = CDPApi.ToInsecureString(CDPApi.DecryptString(Encrypted));
	/// ]]>
	/// </example>
	public class CDPApi
	{
		private static byte[] entropy = Encoding.Unicode.GetBytes("소금은비밀번호가아니다");

		public static string EncryptString(SecureString input)
		{
			byte[] encryptedData = ProtectedData.Protect(
				Encoding.Unicode.GetBytes(ToInsecureString(input)),
				entropy,
				DataProtectionScope.CurrentUser);
			return Convert.ToBase64String(encryptedData);
		}
		public static SecureString DecryptString(string encryptedData)
		{
			try
			{
				byte[] decryptedData = ProtectedData.Unprotect(
					Convert.FromBase64String(encryptedData), entropy,
					DataProtectionScope.CurrentUser);
				return ToSecureString(Encoding.Unicode.GetString(decryptedData));
			}
			catch { return new SecureString(); }
		}
		public static SecureString ToSecureString(string input)
		{
			SecureString secure = new SecureString(); foreach (char c in input)
			{
				secure.AppendChar(c);
			} secure.MakeReadOnly(); return secure;
		}
		public static string ToInsecureString(SecureString input)
		{
			string returnValue = string.Empty;
			IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
			try
			{
				returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
			}
			finally
			{
				System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
			}
			return returnValue;
		}
	}
}
