using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;

namespace DoctorGu
{
	public delegate bool DelegateFilterHtmlElement(HtmlElement el);

	public class CWebBrowser
	{
		/// <summary>
		/// WebBrowser 컨트롤에 HTML 내용을 표시함
		/// </summary>
		/// <remarks>
		/// 폼의 Load 이벤트에서 다음 코드를 미리 실행해야만 이 함수를 호출할 때 에러가 발생하지 않음.
		/// webContent.Navigate("about:blank");
		/// </remarks>
		/// <param name="web">WebBrowser 컨트롤</param>
		/// <param name="Html">표시할 HTML</param>
		/// <param name="IsAllowHtmlTag">HTML 태그가 적용되게 할 것인지, <![CDATA[<, >]]> 등의 문자열 자체가 그대로 보이게 할 것인 지 여부.</param>
		/// <param name="Style">표시할 HTML에 적용될 스타일</param>
		/// <example>
		/// 다음은 radHtml, radText Radio 컨트롤, txtContent TextBox 컨트롤, webContent WebBrowser 컨트롤이 있는 경우,
		/// radHtml, radText를 선택하거나, txtContent에 새로운 내용을 입력할 때마다 webContent에 HTML 결과를 표시하는 예입니다.
		/// <code>
		/// private void frmBoardManage_Load(object sender, EventArgs e)
		/// {
		///	 CCommon.ContentStyle = "font-family: 돋움,돋움체,Gulim,Arial; font-size: 12px; color:#000000; line-height:17px;  margin:0 0 0 0;";
		/// 	webContent.Navigate("about:blank");
		/// }
		/// private void radText_CheckedChanged(object sender, EventArgs e)
		/// {
		/// 	CHtml.ShowHtml(webContent, txtContent.Text, radHtml.Checked, CCommon.ContentStyle);
		/// }
		/// private void radHtml_CheckedChanged(object sender, EventArgs e)
		/// {
		/// 	CHtml.ShowHtml(webContent, txtContent.Text, radHtml.Checked, CCommon.ContentStyle);
		/// }
		/// private void txtContent_TextChanged(object sender, EventArgs e)
		/// {
		/// 	CHtml.ShowHtml(webContent, txtContent.Text, radHtml.Checked, CCommon.ContentStyle);
		/// }
		/// </code>
		/// </example>
		public static void ShowHtml(WebBrowser web, string Html, bool IsAllowHtmlTag, string Style)
		{
			Html = CHtml.ReplaceHtmlToShow(Html, IsAllowHtmlTag);
			ShowHtml(web, Html, Style);
		}
		private static void ShowHtml(WebBrowser web, string Html, string Style)
		{
			HtmlDocument Doc = web.Document;
			if (Doc == null)
			{
				return;
			}
			if (Doc.Body == null)
			{
				return;
			}
			HtmlElement Elem = null;
			if (Doc.All["__spnContainer"] != null)
			{
				Elem = Doc.All["__spnContainer"];
			}
			else
			{
				Elem = Doc.CreateElement("span");
				Elem.Id = "__spnContainer";
				Doc.Body.AppendChild(Elem);
			}

			if (Style != "")
			{
				Elem.Style = Style;
			}

			Elem.InnerHtml = Html;
		}

		/// <summary>
		/// HTML을 임시파일에 저장하고, 그 파일을 Navigate해서 내용을 표시함.
		/// </summary>
		/// <remarks>
		/// 내용이 많으면서 IMG의 SRC 속성, A의 HREF 속성에 전체 경로를 가진 이메일의 내용을 표시할 때 유용함.
		/// </remarks>
		/// <param name="web">WebBrowser 컨트롤</param>
		/// <param name="Html">파일로 저장해서 보여질 HTML 내용</param>
		/// <example>
		/// 다음은 이메일의 내용을 표시합니다.
		/// <code>
		/// EmailBody = GetEmailBody();
		/// CHtml.ShowHtmlByNavigate(webContent, EmailBody);
		/// </code>
		/// </example>
		public static void ShowHtmlByNavigate(WebBrowser web, string Html)
		{
			string TempFile = CFile.GetTempFileName();
			TempFile = Path.GetDirectoryName(TempFile) + Path.GetFileNameWithoutExtension(TempFile) + ".htm";
			CFile.WriteTextToFile(TempFile, Html);
			web.Navigate(TempFile);
		}

		/// <summary>
		/// 브라우저가 로딩을 끝냈는 지 여부를 리턴함.
		/// </summary>
		/// <param name="web"></param>
		/// <param name="IsAllowInteractive">
		/// 서버에서 이미지를 주기 위해 Response.OutputStream을 사용하고 Response.End를 하지 않은 경우에 readyState가 "interactive"로 남아있게 되므로
		/// 이럴 땐 IsAllowInteractive를 true로 해야만 함.
		/// </param>
		/// <returns></returns>
		public static bool IsCompleted(WebBrowser web, bool IsAllowInteractive)
		{
			if ((web.ReadyState != WebBrowserReadyState.Complete)
				&& (IsAllowInteractive && (web.ReadyState != WebBrowserReadyState.Interactive)))
				return false;

			HtmlDocument Doc = web.Document;
			if (Doc == null)
				return false;

			HtmlElement Body = Doc.Body;
			if (Body != null)
			{
				string Html = Body.OuterHtml;

				if (!Html.StartsWith("<frameset ", StringComparison.CurrentCultureIgnoreCase))
				{
					if (Html.IndexOf("</body>", StringComparison.CurrentCultureIgnoreCase) == -1)
						return false;
				}
			}

			string readyState = (string)Doc.InvokeScript("eval", new object[] { "document.readyState" });
			if (IsAllowInteractive)
			{
				if ((readyState != "interactive") && (readyState != "complete"))
					return false;
			}
			else
			{
				if (readyState != "complete")
					return false;
			}

			return true;
		}
		public static bool IsCompleted(WebBrowser web)
		{
			bool IsAllowInteractive = false;
			return IsCompleted(web, IsAllowInteractive);
		}

		public static IEnumerable<HtmlElement> Select(HtmlElement elParent, string TagName, DelegateFilterHtmlElement Filter)
		{
			if (elParent != null)
			{
				foreach (HtmlElement elCur in elParent.GetElementsByTagName(TagName))
				{
					if (Filter(elCur))
						yield return elCur;
				}
			}
		}
		public static IEnumerable<HtmlElement> Select(HtmlElement elParent, string TagName, string AttrName, string AttrValue, bool IgnoreCase)
		{
			return Select(elParent, TagName, el => (string.Compare(el.GetAttribute(AttrName), AttrValue, IgnoreCase) == 0));
		}
		public static HtmlElement FirstOrDefault(HtmlElement elParent, string TagName, DelegateFilterHtmlElement Filter)
		{
			foreach (HtmlElement elCur in Select(elParent, TagName, Filter))
			{
				return elCur;
			}

			return null;
		}
		public static HtmlElement FirstOrDefault(HtmlElement elParent, string TagName, string AttrName, string AttrValue, bool IgnoreCase)
		{
			foreach (HtmlElement elCur in Select(elParent, TagName, AttrName, AttrValue, IgnoreCase))
			{
				return elCur;
			}

			return null;
		}

		public static HtmlElement SelectParent(HtmlElement elChild, string TagName, DelegateFilterHtmlElement Filter)
		{
			HtmlElement elParent = elChild.Parent;
			while (elParent != null)
			{
				elParent = elParent.Parent;
				if (string.Compare(elParent.TagName, TagName, true) != 0)
					continue;

				if (Filter(elParent))
					return elParent;
			}

			return null;
		}
		public static HtmlElement SelectParent(HtmlElement elChild, string TagName, string AttrName, string AttrValue, bool IgnoreCase)
		{
			return SelectParent(elChild, TagName, el => (string.Compare(el.GetAttribute(AttrName), AttrValue, IgnoreCase) == 0));
		}
		public static HtmlElement SelectParent(HtmlElement elChild, string TagName)
		{
			return SelectParent(elChild, TagName, el => true);
		}

		public static HtmlDocument GetDocumentOfFrame(WebBrowser Web, string FrameName)
		{
			HtmlDocument Doc = Web.Document;
			if (Doc == null)
				return null;

			if (Doc.Window.Frames.Count == 0)
				return null;

			HtmlWindow Fra = Web.Document.Window.Frames[FrameName];
			if (Fra == null)
				return null;

			//네이버 카페의 cafe_main 프레임을 로딩 중에는 "액세스가 거부되었습니다." 에러 나는 순간이 있음.
			HtmlDocument FraDoc = null;
			try
			{
				FraDoc = Fra.Document;
			}
			catch (Exception)
			{
				return null;
			}

			if (FraDoc == null)
				return null;

			return FraDoc;
		}

		public static int SelectOptionByText(HtmlElement elSelect, string Text)
		{
			int SelectedIndex = -1;
			foreach (HtmlElement elOption in elSelect.Children)
			{
				if (string.Compare(elOption.TagName, "option", true) != 0)
					continue;

				SelectedIndex++;
				if (elOption.InnerText != Text)
					continue;

				elOption.SetAttribute("selected", "selected");
				return SelectedIndex;
			}

			return SelectedIndex;
		}
	}
}
