using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace DoctorGu.Encryption
{
	public class CMd5
	{
		/// <summary>
		/// Md5는 암호화만 되고 복호화는 안되는 방식임.
		/// PHP와 호환되려면 PHP에서 사용된 Encoding과 같은 Encoding을 사용해야만 같은 결과를 얻을 수 있음.
		/// 한글 사용 PHP는 대부분 Encoding.GetEncoding("ks_c_5601-1987") 사용하면 됨.
		/// SQL Server는 Unicode = utf-16 = nvarchar이므로 nvarchar일 경우 Encoding.Unicode 사용해야 함.
		/// </summary>
		/// <example>
		/// <![CDATA[
		/// PHP: echo(readurl("http://www.a.b/a.aspx?p=" . md5("안녕괢!@#$%^&*()_+|1234")));
		/// C#: string s = Encryption.CMd5.Encrypt("안녕괢!@#$%^&*()_+|1234", Encoding.GetEncoding("ks_c_5601-1987"));
		/// ]]>
		/// </example>
		/// <param name="hashMe"></param>
		/// <param name="encoder"></param>
		/// <returns></returns>
		public static string Encrypt(string hashMe, Encoding encoder)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			//UTF7Encoding encoder = new UTF7Encoding();
			Byte[] encStringBytes;

			encStringBytes = encoder.GetBytes(hashMe);
			encStringBytes = md5.ComputeHash(encStringBytes);

			string strHex = string.Empty;
			foreach (byte b in encStringBytes)
			{
				strHex += String.Format("{0:x2}", b);
			}

			return strHex;
		}
	}
}
