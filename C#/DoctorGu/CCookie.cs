using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Web.SessionState;
using DoctorGu;

namespace DoctorGu
{
	/// <summary>
	/// 쿠키의 설정과 삭제 관련 기능 구현
	/// </summary>
	public class CCookie
	{
		private static Hashtable mhtCookieForShareHttp = new Hashtable();
		private static Hashtable mhtCookieForShareHttps = new Hashtable();

		public CCookie()
		{
		}

		/// <summary>
		/// https://pg.site.co.kr 과 http://www.site.co.kr 간의 Cookie를 공유하기 위해
		/// 한쪽의 쿠키 정보를 변수에 저장함.
		/// </summary>
		/// <param name="LogInId">로그인 아이디</param>
		public static void AddCookieForShareHttpsAndHttp(string LogInId)
		{
			HttpRequest Req = HttpContext.Current.Request;

			HttpCookieCollection CookiesCur = Req.Cookies;

			bool IsHttps = (Req.ServerVariables["HTTPS"] == "on");

			if (IsHttps)
			{
				mhtCookieForShareHttps[LogInId] = CookiesCur;
			}
			else
			{
				mhtCookieForShareHttp[LogInId] = CookiesCur;
			}
		}

		/// <summary>
		/// https://pg.site.co.kr 과 http://www.site.co.kr 간의 Cookie를 공유하기 위해
		/// 한쪽에서 저장된 쿠키 정보를 다른쪽에서 읽음.
		/// </summary>
		public static void ShareCookieWithHttpsAndHttp(string LogInId)
		{
			HttpRequest Req = HttpContext.Current.Request;

			HttpCookieCollection CookiesCur = Req.Cookies;

			bool IsHttps = (Req.ServerVariables["HTTPS"] == "on");

			if (IsHttps)
			{
				if (mhtCookieForShareHttp.ContainsKey(LogInId))
				{
					MergeCookie((HttpCookieCollection)mhtCookieForShareHttp[LogInId]);
					mhtCookieForShareHttp.Remove(LogInId);
				}
			}
			else
			{
				if (mhtCookieForShareHttps.ContainsKey(LogInId))
				{
					MergeCookie((HttpCookieCollection)mhtCookieForShareHttps[LogInId]);
					mhtCookieForShareHttps.Remove(LogInId);
				}
			}
		}
		/// <summary>
		/// 한쪽에서 저장된 쿠키 정보를 다른쪽에서 읽는 작업을 실제로 함.
		/// </summary>
		private static void MergeCookie(HttpCookieCollection CookToOverwrite)
		{
			HttpCookieCollection CookiesCur = HttpContext.Current.Request.Cookies;
			HttpCookieCollection CookiesWrite = HttpContext.Current.Response.Cookies;
			bool IsChanged = false;

			//현재 쿠키와 이전 쿠키에 일치하는 이름이 있다면 이전 쿠키 값을 덮어씀.
			for (int i = 0, i2 = CookiesCur.Count; i < i2; i++)
			{
				string Key = CookiesCur.GetKey(i);
				string Value = CookiesCur[i].Value;

				if (CookToOverwrite[Key] == null)
				{
					continue;
				}

				if (Value != CookToOverwrite[Key].Value)
				{
					CookiesWrite[Key].Value = CookToOverwrite[Key].Value;
					IsChanged = true;
				}
			}

			//이전 쿠키에서 추가된 값이 있다면 현재 쿠키에 추가함.
			for (int i = 0, i2 = CookToOverwrite.Count; i < i2; i++)
			{
				string Key = CookToOverwrite.GetKey(i);
				string Value = CookToOverwrite[i].Value;

				if (CookiesCur[Key] == null)
				{
					CookiesWrite.Add(new HttpCookie(Key, Value));
					IsChanged = true;
				}
			}

			if (IsChanged)
			{
				//클라이언트로 가기 전까지는 변경된 쿠키를 읽을 수 없으므로
				//현재 페이지를 다시 호출함.
				HttpRequest Req = HttpContext.Current.Request;
				HttpResponse Resp = HttpContext.Current.Response;
				string UrlParam = CQueryString.AppendQueryString(Req.ServerVariables["PATH_INFO"], Req.ServerVariables["QUERY_STRING"]);
				Resp.Redirect(UrlParam, true);
			}
		}

		/// <summary>
		/// 모든 쿠키의 정보를 리턴함.
		/// </summary>
		/// <param name="DelimCol">열 구분자</param>
		/// <param name="DelimRow">행 구분자</param>
		/// <returns>완성된 쿠키 정보 문자열</returns>
		public static string GetAllCookies(string DelimCol, string DelimRow)
		{
			HttpCookieCollection CookiesCur = HttpContext.Current.Request.Cookies;

			string s = "";
			for (int i = 0, i2 = CookiesCur.Count; i < i2; i++)
			{
				string Key = CookiesCur.GetKey(i);
				string Value = CookiesCur[i].Value;
				s += Key + DelimCol + Value + DelimRow;
			}

			return s;
		}

		/// <summary>
		/// 특정 키에 해당하는 쿠키 값을 리턴함. 값이 없으면 0길이 문자열을 리턴함.
		/// </summary>
		public static string GetCookie(string Key)
		{
			HttpCookie Cookie = HttpContext.Current.Request.Cookies[Key];

			if (Cookie == null)
			{
				return "";
			}
			else
			{
				return Cookie.Value;
			}
		}
		/// <summary>
		/// 특정 키에 해당하는 쿠키 값을 리턴함. 값이 없으면 0길이 문자열을 리턴함.
		/// </summary>
		public static string GetCookie(string Key1, string Key2)
		{
			HttpCookie Cookie = HttpContext.Current.Request.Cookies[Key1];
			if (Cookie == null)
				return "";

			string Value = Cookie[Key2];
			if (Value == null)
				return "";

			return Value;
		}
		/// <summary>
		/// 특정 키에 해당하는 쿠키 값을 설정함.
		/// </summary>
		public static void SetCookie(string Key, string Value, int ExpireDays)
		{
			HttpCookie Cookie = HttpContext.Current.Response.Cookies[Key];

			Cookie.Value = Value;

			if (ExpireDays > 0)
			{
				Cookie.Expires = DateTime.Now.AddDays(ExpireDays);
			}
		}
		public static void SetCookie(string Key, string Value)
		{
			SetCookie(Key, Value, 0);
		}
		/// <summary>
		/// 특정 키에 해당하는 쿠키 값을 삭제함.
		/// </summary>
		public static void RemoveCookie(string Key)
		{
			HttpContext.Current.Response.Cookies.Remove(Key);

			//원인을 알 수 없으나 Remove를 해도 남아 있어 0길이 문자열로 설정함.
			if (HttpContext.Current.Request.Cookies[Key] != null)
			{
				HttpContext.Current.Response.Cookies[Key].Value = "";
			}
		}
	}
}