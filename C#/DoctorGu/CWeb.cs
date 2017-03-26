using System;
using System.Collections.Generic;
using System.Text;
using DoctorGu;
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
using System.Text.RegularExpressions;

namespace DoctorGu
{
	public class CWeb
	{
		public CWeb()
		{
			
		}

		/// <summary>
		/// 현재 페이지를 호출한 페이지가 같은 서버 안에 있는 지 여부를 리턴함.
		/// </summary>
		/// <param name="ErrMsgIs"></param>
		/// <returns></returns>
		/// <example>
		/// <![CDATA[
		/// 다음은 현재 페이지를 호출한 페이지가 다른 사이트의 페이지이면 에러 메세지를 출력하고 더 이상 진행하지 않음.
		/// if (!IsRefererInSameServer(out ErrMsgIs)
		/// {
		///		Response.Write(ErrMsgIs);
		///		Response.End();
		/// }
		/// ]]>
		/// </example>
		public static bool IsRefererInSameServer(out string ErrMsgIs)
		{
			ErrMsgIs = "";

			string Referer = CFindRep.IfNullThenEmpty(HttpContext.Current.Request.ServerVariables.Get("HTTP_REFERER"));
			if (Referer == "")
				Referer = CWeb.GetRemoteIpAddress(HttpContext.Current);

			Referer = CPath.GetServerUrl(Referer).ToLower();

			string Server = HttpContext.Current.Request.ServerVariables.Get("LOCAL_ADDR");
			Server = CPath.GetServerUrl(Server).ToLower();

			if (Referer == Server)
			{
				return true;
			}

			Server = HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST");
			Server = CPath.GetServerUrl(Server).ToLower();

			if (Referer == Server)
			{
				return true;
			}

			ErrMsgIs = "서버명이 다음과 같이 일치하지 않습니다.\r\n클라이언트: " + Referer + ", 서버: " + Server;
			return false;
		}

		public static bool IsCallerInSameServer(out string ErrMsgIs)
		{
			ErrMsgIs = "";

			string RemoteAddr = CWeb.GetRemoteIpAddress(HttpContext.Current);
			RemoteAddr = CPath.GetServerUrl(RemoteAddr).ToLower();

			string Server = HttpContext.Current.Request.ServerVariables.Get("LOCAL_ADDR");
			Server = CPath.GetServerUrl(Server).ToLower();

			if (RemoteAddr == Server)
			{
				return true;
			}

			Server = HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST");
			Server = CPath.GetServerUrl(Server).ToLower();

			if (RemoteAddr == Server)
			{
				return true;
			}

			ErrMsgIs = "서버명이 다음과 같이 일치하지 않습니다.\r\n클라이언트: " + RemoteAddr + ", 서버: " + Server;
			return false;
		}

		/// <summary>
		/// POST 방식으로 전송된 모든 데이터의 Name, Value를 리턴함.
		/// </summary>
		public static string GetFormValueList(HttpContext ctx, string DelimCol, string DelimRow)
		{
			NameValueCollection nvForm = ctx.Request.Form;
			string s = "";
			for (int i = 0, i2 = nvForm.Count; i < i2; i++)
			{
				s += DelimRow + nvForm.GetKey(i) + DelimCol + nvForm[i];
			}
			if (s != "")
				s = s.Substring(DelimRow.Length);

			return s;
		}
		public static string GetFormValueList(string DelimCol, string DelimRow)
		{
			return GetFormValueList(HttpContext.Current, DelimCol, DelimRow);
		}
		public static string GetFormValueList(HttpContext ctx)
		{
			return GetFormValueList(ctx, "\t", "\r\n");
		}
		public static string GetFormValueList()
		{
			return GetFormValueList(HttpContext.Current);
		}

		/// <summary>
		/// 현재 가지고 있는 모든 Session 정보를 리턴함.
		/// </summary>
		public static string GetSessionList(string DelimCol, string DelimRow)
		{
			HttpSessionState Sess = HttpContext.Current.Session;

			string s = "";

			if (Sess != null)
			{
				foreach (string Key in Sess.Contents)
				{
					s += Key + DelimCol + Sess.Contents[Key] + DelimRow;
				}
			}

			return s;
		}

		/// <summary>
		/// 현재 ServerVariables의 모든 정보를 리턴함.
		/// </summary>
		public static string GetServerVariableList(string DelimCol, string DelimRow)
		{
			HttpRequest Req = HttpContext.Current.Request;

			string s = "";
			foreach (string Key in Req.ServerVariables)
			{
				string Value = (Req.ServerVariables[Key] + "");
				if (Value == "") continue;
				if ((Key == "ALL_HTTP") || (Key == "ALL_RAW")) continue;

				s += Key + DelimCol + Value + DelimRow;
			}

			return s;
		}

		public static string GetServerVariablesInHtml()
		{
			HttpRequest Req = HttpContext.Current.Request;

			StringBuilder sb = new StringBuilder();
			sb.Append("<table border=1 cellpadding=0 cellspacing=0>");
			foreach (string Key in Req.ServerVariables)
			{
				sb.Append("<tr><td>" + (string)Key + "</td><td>" + CFindRep.IfNullOrEmptyThen(Req.ServerVariables.Get(Key), "&nbsp;") + "</td></tr>");
			}
			sb.Append("</table>");

			return sb.ToString();
		}

		/// <summary>
		/// 클라이언트에서 캐쉬된 페이지를 불러오지 않도록 함.
		/// </summary>
		public static void SetNoCache()
		{
			HttpResponse Response = HttpContext.Current.Response;

			//Rep.CacheControl = "no-cache"; //이 코드를 실행하면 JScript 에러시 디버깅 창에서 스크립트 코드가 보이지 않아 주석.
			//Response.AddHeader("Pragma", "no-cache"); //depricated 되어 주석
			//Response.Expires = -1; //SetExpires가 대신할 거라 생각해 주석.

			Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.Cache.SetNoStore();
		}

		/// <summary>
		/// 현재 페이지 경로 앞에 Full URL을 덧붙인 값을 리턴함.
		/// 예를 들어 현재 페이지 경로가 "/test.aspx"이면, "http://www.myurl.com/test.aspx"로 변경함.
		/// </summary>
		public static string PrefixHttpToCurrentUrl()
		{
			HttpRequest Req = HttpContext.Current.Request;
			string Url = "http://"
					+ Req.ServerVariables["HTTP_HOST"]
					+ Req.ServerVariables["PATH_INFO"];
			string Param = Req.ServerVariables["QUERY_STRING"];
			string UrlParam = CQueryString.AppendQueryString(Url, Param);
			return UrlParam;
		}

		public static string ConvertRelativeToAbsoluteImgSrc(string UrlDirectory, string Html)
		{
			StringBuilder sb = new StringBuilder();

			Regex r = new Regex(CRegex.Pattern.ExtractUrlFromImgTag, RegexOptions.IgnoreCase);
			int PosStart = 0, PosEnd = 0;
			for (Match m = r.Match(Html); m.Success; m = m.NextMatch())
			{
				PosEnd = (m.Index - 1);
				sb.Append(Html.Substring(PosStart, (PosEnd - PosStart + 1)));
				PosStart = (m.Index + m.Length);

				string Url = m.Groups["Url"].Value;
				string UrlNew = CPath.ConvertRelativeToAbsolute(UrlDirectory, Url);
				sb.Append(m.Value.Replace(Url, UrlNew));
			}

			if ((PosStart + 1) <= Html.Length)
			{
				sb.Append(Html.Substring(PosStart));
			}

			return sb.ToString();
		}

		/// <summary>
		/// WebRequest를 이용해 Ftp 기능을 이용할 때 C# 폴더는 C로 읽혀지는 경우 있어 방지하기 위함.
		/// Uri.EscapeDataString 사용하면 /까지 전부 바뀌어 문제가 있는 기호만 변경함.
		/// </summary>
		/// <param name="Url"></param>
		/// <returns></returns>
		public static string EncodeUrlForWebRequest(string Url)
		{
			string Url2 = "";

			string[] aToFind = new string[] { 
				"~", "!", "@", "#", "$", 
				"%", "^", "&", "(", ")", 
				"_", "+", "`", "-", "=", 
				"{", "}", "[", "]", ";", 
				"'" };
			for (int i = 0; i < Url.Length; i++)
			{
				char c = Url[i];
				string s = c.ToString();
				if (Array.IndexOf(aToFind, s) != -1)
				{
					s = CMath.Hex(c, true);
				}

				Url2 += s;
			}

			return Url2;
		}

		public static void WriteLine(string Value, bool UseXmpTag)
		{
			string TagLeft = UseXmpTag ? "<xmp>" : "";
			string TagRight = UseXmpTag ? "</xmp>" : "";
			HttpContext.Current.Response.Write(TagLeft + Value + TagRight + "<br>\r\n");
		}
		public static void WriteLine(string Value)
		{
			WriteLine(Value, false);
		}

		public static double GetIeVersionFromUserAgent(HttpContext ctx)
		{
			string Ua = ctx.Request.ServerVariables["HTTP_USER_AGENT"];
			Regex r = new Regex(CRegex.Pattern.ExtractIeVersionFromUserAgent, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
			Match m = r.Match(Ua);
			if (m.Success)
				return Convert.ToDouble(m.Groups["Version"].Value);
			else
				return 0d;
		}

		public static string GetRemoteIpAddress(HttpContext ctx)
		{
			string Ip = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

			if (!string.IsNullOrEmpty(Ip))
			{
				string[] aIp = Ip.Split(',');
				if (aIp.Length != 0)
				{
					return aIp[0];
				}
			}

			return ctx.Request.ServerVariables["REMOTE_ADDR"];
		}
	}
}
