using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class CConst
	{
		public const int Port21Ftp = 21;
		//public const int Port25Smtp = 25;
		public const int Port587Smtp = 587;

		/// <summary>yyyyMMdd</summary>
		public const string Format_yyyyMMdd = "yyyyMMdd";
		/// <summary>yyyy-MM-dd</summary>
		public const string Format_yyyy_MM_dd = "yyyy-MM-dd";
		/// <summary>yyyyMMddHH</summary>
		public const string Format_yyyyMMddHH = "yyyyMMddHH";
		/// <summary>yyyy-MM-dd HH:mm:ss</summary>
		public const string Format_yyyy_MM_dd_HH_mm_ss = "yyyy-MM-dd HH:mm:ss";
		/// <summary>yyyy-MM-dd HH:mm:ss.fff</summary>
		public const string Format_yyyy_MM_dd_HH_mm_ss_fff = "yyyy-MM-dd HH:mm:ss.fff";
		/// <summary>yyyyMMddHHmmss</summary>
		public const string Format_yyyyMMddHHmmss = "yyyyMMddHHmmss";
		/// <summary>yyyyMMddHHmmssfff</summary>
		public const string Format_yyyyMMddHHmmssfff = "yyyyMMddHHmmssfff";
		/// <summary>HHmmss</summary>
		public const string Format_HHmmss = "HHmmss";
		/// <summary>HH:mm:ss</summary>
		public const string Format_HH_mm_ss = "HH:mm:ss";
		/// <summary>HH:mm:ss.fff</summary>
		public const string Format_HH_mm_ss_fff = "HH:mm:ss.fff";

		/// <summary>서버에서 이 메시지를 넘기면 CXmlClient, CAjaxClient에서 AuthenticationException 에러를 발생시키기 위함.</summary>
		public const string MsgLogInNeeded = "로그인이 필요합니다.";
		public const string MsgTimeoutExpired = "시간제한이 만료되었습니다.";

		/// <summary>이 값을 넘기면 서버에서 DbNull.Value로 변경함.</summary>
		public const string NullString = "<<null>>";

		public const string TextAllowedForVoucherCode = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>utf-8 형식의 XML을 생성하기 위함. XmlDocument를 만들려면 CXml.CreateUtf8XmlDocument를 사용할 것</summary>
		public const string XmlHeaderUtf8 = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";

        //public struct BooleanList
        //{
        //    public static readonly string[] TrueList = new string[] { "true", "True", "1", "-1" };
        //    public static readonly string[] FalseList = new string[] { "false", "False", "0" };
        //}

		public struct ServerVariableName
		{
			public const string Ip = "{{ServerVariableName.Ip}}";
			public const string MemberKey = "{{ServerVariableName.MemberKey}}";
		}
	}
}
