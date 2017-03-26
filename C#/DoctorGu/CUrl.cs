using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Runtime.InteropServices;


namespace DoctorGu
{
    public class CUrl
    {
        /// <summary>
        /// Url의 경우 마지막의 /까지 폴더 경로에 포함해야 하므로 따로 만듦.
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string GetDirectoryName(string Url)
        {
            int Pos = Url.LastIndexOf('/');
            return (Pos != -1 ? Url.Substring(0, Pos + 1) : "");
        }
        public static string GetFileName(string Url)
        {
            //오른쪽에서 왼쪽으로 첫번째 \ 기호를 찾음.
            //그 뒤의 모든 문자열은 파일이므로 그것을 리턴함.
            int Pos = Url.LastIndexOf('/');
            return (Pos != -1 ? Url.Substring(Pos + 1) : Url);
        }

        public static string GetParentDirectory(string Url)
        {
            //{오른쪽에서 왼쪽 방향으로 \ 기호를 찾고,
            //찾아진 위치의 왼쪽에 있는 모든 문자열을 리턴함.
            for (int i = (Url.Length - 1); i >= 0; i--)
            {
                char c = Url[i];
                if (c == '/')
                {
                    string UrlNew = Url.Substring(0, i + 1);
                    return UrlNew;
                }
            }

            return "";
        }

        public static string GetLastDirectory(string FullUrl)
        {
            if (FullUrl == "")
                return "";

            bool EndsWithSep = (FullUrl.Substring(FullUrl.Length - 1)[0] == '/');

            int PosStart = EndsWithSep ? FullUrl.Substring(0, FullUrl.Length - 1).LastIndexOf('/') : FullUrl.LastIndexOf('/');
            if (PosStart == -1)
                return "";

            return FullUrl.Substring(PosStart + 1);
        }

        public static string Combine(string Url1, string Url2)
        {
			if (string.IsNullOrEmpty(Url1))
				return Url2;
			else if (string.IsNullOrEmpty(Url2))
				return Url1;

            return (new Uri(new Uri(Url1), Url2)).ToString();
        }
        public static string Combine(string Url1, string Url2, string Url3)
        {
			if (string.IsNullOrEmpty(Url1))
				return Combine(Url2, Url3);
			else if (string.IsNullOrEmpty(Url2))
				return Combine(Url1, Url3);
			else if (string.IsNullOrEmpty(Url3))
				return Combine(Url1, Url2);

            return (new Uri(new Uri(new Uri(Url1), Url2), Url3)).ToString();
        }

        /// <summary>
        /// UrlParent가 '/good/manager/'이고 UrlChild가 '../../images/star.gif'인 경우
        /// '/images/star.gif'를 리턴함.
        /// </summary>
        /// <param name="UrlParent"></param>
        /// <param name="UrlChild"></param>
        /// <returns></returns>
        public static string ConvertRelativeToAbsolute(string UrlParent, string UrlChild)
        {
            if (UrlChild.StartsWith("http://")
                || UrlChild.StartsWith("ftp://"))
            {
                return UrlChild;
            }

            // http://www.site.com은 제거
            bool IsError = false;
            Uri u = null;
            string Scheme = "";
            string Authority = "";
            try 
            {
                u = new Uri(UrlParent, UriKind.Absolute);
                Scheme = u.Scheme;
                Authority = u.Authority;
            }
            catch (Exception) 
            {
                IsError = true;
            }
            if (!IsError)
            {
                UrlParent = u.AbsolutePath;
            }

            List<string> aUrlParent = new List<string>(UrlParent.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
            List<string> aUrlChild = new List<string>(UrlChild.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries));

            //파일은 경로에서 삭제
            if (aUrlParent[aUrlParent.Count - 1].IndexOf('.') != -1)
            {
                aUrlParent.RemoveAt(aUrlParent.Count - 1);
            }

            if (UrlChild.StartsWith("/"))
            {
                aUrlParent.Clear();
            }
            else
            {
                for (int i = 0; i < aUrlChild.Count; i++)
                {
                    if (aUrlChild[i] == "..")
                    {
                        aUrlParent.RemoveAt(aUrlParent.Count - 1);
                        aUrlChild.RemoveAt(0);
                        i--;
                    }
                }
            }

            foreach (string UrlChildCur in aUrlChild)
            {
				aUrlParent.Add(UrlChildCur);
            }

            string UrlNew = string.Join("/", aUrlParent.ToArray());
            if (!string.IsNullOrEmpty(Authority))
				UrlNew = CUrl.Combine(Scheme + "//" + Authority, UrlNew);

            return UrlNew;
        }

        /// <summary>
        /// 2차 도메인(gilbut.testinter.net)이 아닌 상태에서 www.로 시작하지 않으면 true를 리턴함.
        /// </summary>
        /// <example>
        /// <![CDATA[
        /// Url = "http://testinter.net";
        /// Debug.WriteLine(CPath.StartsWithNoWwwAndNotSubDomain(Url)); //true
        /// 
        /// Url = "http://www.testinter.net";
        /// Debug.WriteLine(CPath.StartsWithNoWwwAndNotSubDomain(Url)); //false
        ///
        /// Url = "gilbut.testinter.net";
        /// Debug.WriteLine(CPath.StartsWithNoWwwAndNotSubDomain(Url)); //false
        /// ]]>
        /// </example>
        /// <param name="ServerUrl"></param>
        /// <returns></returns>
        public static bool StartsWithNoWwwAndNotSubDomain(string ServerUrl)
        {
            if (ServerUrl.StartsWith("http://"))
                ServerUrl = ServerUrl.Substring("http://".Length);

            if (ServerUrl.EndsWith("/"))
                ServerUrl = ServerUrl.TrimEnd('/');

            string[] aKind = new string[] { ".com", ".net", ".co.kr", ".org", ".biz", ".info", ".kr", ".cn", ".cc", ".tv", ".jp", ".in", ".tw", ".de", ".co", ".tel", ".asia", ".eu", ".mobi", ".name", ".com", ".net", ".kr", ".org", ".biz", ".info" };
            string KindFound = aKind.FirstOrDefault(s => ServerUrl.EndsWith(s));
            if (!string.IsNullOrEmpty(KindFound))
            {
                ServerUrl = ServerUrl.Substring(0, ServerUrl.Length - KindFound.Length);
            }

            bool NotSubDomain = (ServerUrl.IndexOf(".") == -1);
            bool StartsWithWww = ServerUrl.StartsWith("www.");

            return !StartsWithWww && NotSubDomain;
        }

        //gilbut.testinter.net인 경우 www.gilbut.testinter.net으로 바껴서
        //이미지를 못가져오는 경우 발생해서 주석.
        //public static string PrefixHttpWww(string Url)
        //{
        //	if (Url.StartsWith("http://www.", StringComparison.CurrentCultureIgnoreCase))
        //	{
        //		return Url;
        //	}

        //	if (Url.StartsWith("localhost", StringComparison.CurrentCultureIgnoreCase))
        //	{
        //		return "http://" + Url;
        //	}

        //	if (Url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
        //	{
        //		Url = Url.Substring("http://".Length);
        //	}
        //	if (Url.StartsWith("www.", StringComparison.CurrentCultureIgnoreCase))
        //	{
        //		Url = Url.Substring("www.".Length);
        //	}

        //	return "http://www." + Url;
        //}
        public static string PrefixHttp(string Url)
        {
            if (Url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
            {
                return Url;
            }

            return "http://" + Url;
        }
        public static string PrefixWww(string Url)
        {
            bool StartsWithHttp = Url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase);

            if (StartsWithHttp)
                Url = Url.Substring("http://".Length);

            bool StartsWithWww = Url.StartsWith("www.", StringComparison.CurrentCultureIgnoreCase);
            if (!StartsWithWww)
                Url = "www." + Url;

            if (StartsWithHttp)
                Url = "http://" + Url;

            return Url;
        }

        public static string PostfixSlash(string Url)
        {
            if (string.IsNullOrEmpty(Url))
                return Url;
            else if (!Url.EndsWith("/"))
                return Url + "/";
            else
                return Url;
        }

        /// <summary>

        public static CFullPathFullUrl GetNumberedFullPathFullUrl(HttpContext ctx, string FullUrl)
        {
            CFullPathFullUrl PathUrl = new CFullPathFullUrl();

            string FileName = CUrl.GetFileName(FullUrl);
            string UrlFolder = CUrl.GetParentDirectory(FullUrl);
            string FullPath = ctx.Server.MapPath(FullUrl);

            string FullPathNew = CPath.GetNumberedFullPath(FullPath);
            if (FullPath != FullPathNew)
            {
                FileName = Path.GetFileName(FullPathNew);
                FullUrl = Combine(UrlFolder, FileName);
                FullPath = FullPathNew;
            }

            PathUrl.FullUrl = FullUrl;
            PathUrl.FullPath = FullPath;
            return PathUrl;
        }

        /// <summary>
        /// Url1과 Url2가 같은 주소인 지 확인.
        /// http://와 마지막의 /를 빼고 비교함.
        /// </summary>
        public static bool CompareUrl(string Url1, string Url2)
        {
            string Url1New = Url1.ToLower();
            string Url2New = Url2.ToLower();

            if (Url1New.StartsWith("http://"))
                Url1New = Url1New.Substring("http://".Length);
            if (Url2New.StartsWith("http://"))
                Url2New = Url2New.Substring("http://".Length);

            if (Url1New.EndsWith("/"))
                Url1New = Url1New.Substring(0, Url1New.Length - "/".Length);
            if (Url2New.EndsWith("/"))
                Url2New = Url2New.Substring(0, Url2New.Length - "/".Length);

            return (Url1New == Url2New);
        }

		public static string GetFullUrlDest(string FolderSrc, string FolderDest, string FullPathSrc)
		{
			if (!FullPathSrc.StartsWith(FolderSrc))
				return "";

			string FullPathRest = FullPathSrc.Substring(FolderSrc.Length);
			if (FullPathRest.StartsWith("/"))
				FullPathRest = FullPathRest.Substring(1);

			string FullPathDest = CUrl.Combine(FolderDest, FullPathRest);
			return FullPathDest;
		}
    }
}

