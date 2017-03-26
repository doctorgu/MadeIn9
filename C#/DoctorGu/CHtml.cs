using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Globalization;
using System.Xml;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DoctorGu
{
	/// <summary>
	/// GetTagInfo 함수에서 사용되며 태그의 정보를 저장함.
	/// </summary>
	public struct TagInfos
	{
		/// <summary>태그 이름</summary>
		public string TagName;
		/// <summary>태그 내의 값</summary>
		public string Value;
		/// <summary>태그의 시작 위치</summary>
		public int PosTagStart;
		/// <summary>태그의 종료 위치</summary>
		public int PosTagEnd;
		/// <summary>태그 내의 값의 시작 위치</summary>
		public int PosValueStart;
		/// <summary>태그 내의 값의 종료 위치</summary>
		public int PosValueEnd;
		/// <summary>태그에서 가진 모든 속성 이름과 값</summary>
		public Hashtable htAttributes;
	}

	public enum CodePageName
	{
		[Description("utf-8")]
		utf_8 = 65001,
		[Description("euc-kr")]
		euc_kr = 51949,
		[Description("ks_c_5601-1987")]
		ks_c_5601_1987 = 949
	}

	/// <summary>HTML 태그 변환 형식</summary>
	[Flags]
	public enum ReplaceHtmlTypes
	{
		/// <summary><![CDATA[<, > 태그를 &lt;, &gt;로 변경]]></summary>
		LtGtTag = 1,
		/// <summary><![CDATA[작은 따옴표('), 큰 따옴표(")를 &#39;, &quot;로 변경]]></summary>
		SingleAndDoubleQuot = 2,
		/// <summary><![CDATA[<script를 &lt;script로 변경]]></summary>
		ScriptTag = 4,
		/// <summary><![CDATA[<xmp를 &lt;xmp로 변경]]></summary>
		XmpTag = 8,
		///// <summary><![CDATA[\r\n, \n을 <br />로 변경]]></summary>
		//NewLineToBr = 2,
	}

	[Flags]
	public enum ExtractUrlTypes
	{
		UrlFromImgTag = 1,
		SwfUrlFromParamTag = 2,
		UrlFromBackground = 4,
		UrlFromStyleBackground = 8,

		UrlFromATag = 16,
		UrlFromNewUri = 32,

		ImgOrSwfUrl = UrlFromImgTag | SwfUrlFromParamTag | UrlFromBackground | UrlFromStyleBackground
	}

	/// <summary>
	/// HTML 태그를 분석해서 가공하거나, 원하는 결과만을 가져오는 기능 구현.
	/// </summary>
	public class CHtml
	{
		/// <summary>
		/// HTML 문서 내에서 PosStart 위치 다음의 첫번째 태그의 정보를 가져옴.
		/// </summary>
		/// <param name="PosStart"></param>
		/// <param name="Html"></param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string s = "<a name=\"abc\" age='3'>안녕</a>";
		/// 
		/// TagInfos ti = CHtml.GetTagInfo(0, s);
		/// 
		/// Console.WriteLine(s);
		/// foreach (DictionaryEntry d in ti.htAttributes)
		/// {
		/// 	Console.WriteLine("Key:{0}, Html:{1}", d.Key, d.Value);
		/// }
		/// Console.WriteLine("PosTagEnd:{0}\n PosTagStart:{1}\n PosValueEnd:{2}"
		/// 	+ "\n PosValueStart:{3}\n TagName:{4}\n Value:{5}",
		/// 	ti.PosTagEnd.ToString(), ti.PosTagStart.ToString(), ti.PosValueEnd.ToString(),
		/// 	ti.PosValueStart.ToString(), ti.TagName, ti.Value);
		/// 
		/// Console.ReadLine();
		/// ]]>
		/// </code>
		/// </example>
		public static TagInfos GetTagInfo(int PosStart, string Html)
		{
			TagInfos ti = new TagInfos();

			string[] aNotAllowedForTagNameList = new string[] { " ", "\t", "\r", "\n" };
			int PosTagNameEnd = -1, PosTagNameEnd2 = -1;

			while (true)
			{
				//시작 태그가 없으면 빠져나감.
				int Pos = PosStart;
				while (Pos != -1)
				{
					Pos = Html.IndexOf("<", Pos);
					if (Pos != -1)
					{
						if ((Pos < (Html.Length - 1)) && (Html[Pos + 1] != '/'))
						{
							break;
						}
						else
						{
							Pos++;
						}
					}
				}
				ti.PosTagStart = Html.IndexOf("<", Pos);
				if (ti.PosTagStart == -1) return ti;

				PosTagNameEnd = Html.IndexOf(">", ti.PosTagStart);
				if (PosTagNameEnd == -1) return ti;

				PosTagNameEnd2 = Html.IndexOf(" ", ti.PosTagStart);
				if (PosTagNameEnd2 < PosTagNameEnd)
				{
					PosTagNameEnd = PosTagNameEnd2;
				}

				ti.TagName = Html.Substring((ti.PosTagStart + 1), ((PosTagNameEnd - 1) - (ti.PosTagStart + 1) + 1));
				if (ti.TagName.Trim() == "")
				{
					PosStart = PosTagNameEnd;
					continue;
				}

				bool IsNotAllowedFound = false;
				for (int i = 0; i < aNotAllowedForTagNameList.Length; i++)
				{
					if (ti.TagName.IndexOf(aNotAllowedForTagNameList[i]) != -1)
					{
						PosStart = PosTagNameEnd;
						IsNotAllowedFound = true;
						break;
					}
				}
				if (IsNotAllowedFound) continue;

				//이곳까지 왔다면 정상적인 태그 이름이므로 빠져나감.
				break;
			}

			ti.PosTagEnd = Html.IndexOf(">", PosTagNameEnd);
			if (ti.PosTagEnd == -1) return ti;

			string OpenTagText = Html.Substring(ti.PosTagStart, ti.PosTagEnd - ti.PosTagStart + 1);
			ti.htAttributes = GetAttributes(OpenTagText, ti.TagName);

			ti.PosValueStart = ti.PosTagEnd + 1;
			if (ti.PosValueStart == 0) return ti;

			//마지막 태그가 없으면 빠져나감.
			ti.PosValueEnd = Html.IndexOf("</" + ti.TagName + ">", ti.PosValueStart) - 1;
			if (ti.PosValueEnd == -2) return ti;

			ti.PosTagEnd = ti.PosValueEnd + ("</" + ti.TagName + ">").Length;

			ti.Value = Html.Substring(ti.PosValueStart, ti.PosValueEnd - ti.PosValueStart + 1);

			return ti;
		}

		/// <summary>
		/// HTML 태그에서 특정 태그의 모든 속성 이름과 값을 리턴함.
		/// </summary>
		/// <param name="Value">HTML 태그로 구성된 문자열</param>
		/// <param name="TagName">태그 이름</param>
		/// <returns>속성의 이름을 Key로, 속성의 값을 Value로 하는 Hashtable </returns>
		/// <example>
		/// 다음은 font 태그의 color, size 속성의 이름과 값을 출력합니다.
		/// <code>
		/// <![CDATA[
		/// string Tag = "<font color=\"red\" size='3'>빨강</font>";
		/// Hashtable htAttrs = CHtml.GetAttributes(Tag, "font");
		/// foreach (DictionaryEntry d in htAttrs)
		/// {
		///	 Console.WriteLine(d.Key.ToString() + ":" + d.Value.ToString());
		/// }
		/// //--결과
		/// //size:3
		/// //color:red
		/// ]]>
		/// </code>
		/// </example>
		public static Hashtable GetAttributes(string Value, string TagName)
		{
			Hashtable htAttributes = new Hashtable();

			int PosStart = Value.IndexOf("<" + TagName);
			int PosEnd = -1;
			if (PosStart != -1)
			{
				PosEnd = Value.IndexOf(">", PosStart);
			}

			string Tag = Value.Substring(PosStart, PosEnd - PosStart + 1);

			int PosStartAttr = -1, PosEndAttr = -1, PosEqual = -1;
			int PosStartQuote = -1, PosEndQuote = -1;
			string AttrName = "", AttrValue = "";
			while (true)
			{
				PosStartAttr = Tag.IndexOf(" ", PosStartAttr + 1) + 1;
				if (PosStartAttr == 0) break;

				PosEndAttr = Tag.IndexOf("=", PosStartAttr + 1) - 1;
				if (PosEndAttr == -2) break;

				AttrName = Tag.Substring(PosStartAttr, (PosEndAttr - PosStartAttr + 1));

				PosEqual = PosEndAttr + 1;
				if (PosEqual == -1) break;

				PosStartQuote = Tag.IndexOf("\"", PosEqual);
				if (PosStartQuote == -1)
				{
					PosStartQuote = Tag.IndexOf("'", PosEqual);
					if (PosStartQuote == -1) continue;

					PosEndQuote = Tag.IndexOf("'", PosStartQuote + 1);
					if (PosEndQuote == -1) continue;
				}
				else
				{
					PosEndQuote = Tag.IndexOf("\"", PosStartQuote + 1);
					if (PosEndQuote == -1) continue;
				}

				AttrValue = Tag.Substring(PosStartQuote + 1, (PosEndQuote - 1) - (PosStartQuote + 1) + 1);

				htAttributes.Add(AttrName, AttrValue);
			}

			return htAttributes;
		}

		/// <summary>
		/// HTML 태그와 함께 쓰이면 에러 발생의 소지가 있는 문자열을 모두 안전하게 변경함.
		/// textarea 안에 내용을 표시할 때 등에 사용됨.
		/// </summary>
		/// <param name="Value">HTML 태그가 포함된 문자열</param>
		/// <returns>안전하게 변경된 문자열</returns>
		/// <example>
		/// <![CDATA[
		/// Console.WriteLine(CHtml.ReplaceHtmlChar("<a></a>")); // "&lt;a&gt;&lt;/a&gt;"
		/// ]]>
		/// </example>
		public static string ReplaceHtmlChar(string Value)
		{
			Value = Value.Replace("<", "&lt;");
			Value = Value.Replace(">", "&gt;");
			Value = Value.Replace("\"", "&quot;");
			Value = Value.Replace("'", "&#39;");

			return Value;
		}

		/// <summary>
		/// HTML 태그와 함께 쓰이면 에러 발생의 소지가 있는 문자열을 모두 안전하게 변경함.
		/// textarea 안에 내용을 표시할 때 등에 사용됨.
		/// </summary>
		/// <param name="Value">HTML 태그가 포함된 문자열</param>
		/// <returns>안전하게 변경된 문자열</returns>
		/// <example>
		/// <![CDATA[
		/// Console.WriteLine(CHtml.ReplaceHtmlChar("<a></a>", ReplaceHtmlType.LtGtTag)); // "&lt;a&gt;&lt;/a&gt;"
		/// ]]>
		/// </example>
		public static string ReplaceHtmlChar(string Value, ReplaceHtmlTypes Type)
		{
			if ((Type & ReplaceHtmlTypes.LtGtTag) == ReplaceHtmlTypes.LtGtTag)
			{
				Value = Value
					.Replace("<", "&lt;")
					.Replace(">", "&gt;");
			}
			//if ((Type & ReplaceHtmlType.NewLineToBr) == ReplaceHtmlType.NewLineToBr)
			//{
			//    Value = Value
			//        .Replace("\r\n", "<br />")
			//        .Replace("\n", "<br />")
			//        .Replace("\r", ""); //\Regex 사용해서 r만 남은 경우는 삭제함.
			//}
			if ((Type & ReplaceHtmlTypes.SingleAndDoubleQuot) == ReplaceHtmlTypes.SingleAndDoubleQuot)
			{
				Value = Value
					.Replace("\"", "&quot;")
					.Replace("'", "&#39;");
			}
			if ((Type & ReplaceHtmlTypes.ScriptTag) == ReplaceHtmlTypes.ScriptTag)
			{
				Value = Value
					.Replace("<script", "&lt;script");
			}
			if ((Type & ReplaceHtmlTypes.XmpTag) == ReplaceHtmlTypes.XmpTag)
			{
				Value = Value
					.Replace("<xmp", "&lt;xmp");
			}

			return Value;
		}

		/// <summary>
		/// 줄바꿈, <![CDATA[<, >, <script]]> 문자열을 HTML 형식으로 화면에 표시하기 위해 HTML 태그로 변경함.
		/// </summary>
		/// <param name="Value">텍스트 형식의 문자열</param>
		/// <param name="IsAllowHtmlTagAndDisallowScriptXmp">HTML 태그를 허용할 지 여부. 단, script, xmp 태그는 위험하므로 모두 허용하지 않음.</param>
		/// <returns>HTML 형식으로 변경된 문자열</returns>
		/// <example>
		/// 다음은 HTML 태그를 적용하는 것과 적용하지 않은 경우의 차이점을 보여줍니다.
		/// <code>
		/// <![CDATA[
		/// string Value = "<b> 태그는 글자를 굵게 합니다.\r\n<script>alert('안녕');</script>";
		/// 
		/// string HtmlAllowTag = CHtml.ReplaceHtmlToShow(Value, true);
		/// Console.WriteLine(HtmlAllowTag);
		/// //--결과
		/// //<b> 태그는 글자를 굵게 합니다.
		/// //&lt;script>alert('안녕');</script>
		/// 
		/// string HtmlDisallowTag = CHtml.ReplaceHtmlToShow(Value, false);
		/// Console.WriteLine(HtmlDisallowTag);
		/// //--결과
		/// //&lt;b&gt; 태그는 글자를 굵게 합니다.<br>&lt;script&gt;alert('안녕');&lt;/script&gt;
		/// ]]>
		/// </code>
		/// </example>
		public static string ReplaceHtmlToShow(string Value, bool IsAllowHtmlTagAndDisallowScriptXmp)
		{
			if (IsAllowHtmlTagAndDisallowScriptXmp)
			{
				return ReplaceHtmlChar(Value, ReplaceHtmlTypes.ScriptTag | ReplaceHtmlTypes.XmpTag);
			}
			else
			{
				return ReplaceHtmlChar(Value, ReplaceHtmlTypes.LtGtTag);
			}
		}
		public static string ReplaceHtmlToShow(string Value)
		{
			//return ReplaceHtmlChar(Value, ReplaceHtmlType.LtGtTag | ReplaceHtmlType.NewLineToBr);
			return ReplaceHtmlToShow(Value, false);
		}

		/// <summary>
		/// !!! <![CDATA[<p>제목</p> -> <p/>제목</p>로 변환되므로 완벽하지 않음.]]>
		/// HTML을 XML 형식으로 변경하기 위해 닫는 태그가 없는 HTML 태그인 <![CDATA[<p>]]> 태그 등을 <![CDATA[<p/>]]> 태그와 같이 변경함.
		/// </summary>
		/// <param name="Html">HTML 문자열</param>
		/// <param name="IsAddProcessingInstruction">"<![CDATA[<?xml version="1.0" encoding="UTF-8"?>]]>" 문자열을 추가할 지 여부</param>
		/// <returns>XML 형식으로 변환된 문자열</returns>
		/// <example>
		/// <code>
		/// </code>
		/// </example>
		public static string ConvertHtmlToParsableXml(string Html, bool IsAddProcessingInstruction)
		{
			Html = CHtml.ReplaceLtGtAmpersand(Html);

			string[] aTag = new string[] { "<img", "<p", "<br", "<hr", "<input" };

			//에러 나지 않게 3 Space를 넣음.
			for (int i = 0, i2 = Html.Length; i < i2; i++)
			{
				for (int j = 0, j2 = aTag.Length; j < j2; j++)
				{
					string TagCur = aTag[j];

					string s = Html.Substring(i, Math.Min(i2 - i, TagCur.Length));
					if (string.Compare(s, TagCur, true) == 0)
					{
						int Pos = Html.IndexOf(">", i + TagCur.Length);
						if (Pos != -1)
						{
							if (Html.Substring(Pos - 1, 1) != "/")
							{
								Html = Html.Substring(0, Pos) + "/" + Html.Substring(Pos);
								//이미 시작 태그에 해당하는 마지막 태그를 고쳤으므로 
								//마지막 태그 다음으로 검사 위치를 옮김.
								i = Pos + 2;
								//"/"를 넣어 총길이가 하나 늘었으므로 i2를 하나 늘림.
								i2++;
							}
						}
					}
				}
			}

			if (IsAddProcessingInstruction)
			{
				Html = CConst.XmlHeaderUtf8 + "\r\n" + Html;
			}

			return Html;
		}

		/// <summary>
		/// A 태그 안에 IMG 태그가 있는 HTML 태그를 만듦.
		/// </summary>
		/// <param name="AHref">A 태그의 HREF 속성</param>
		/// <param name="ATarget">A 태그의 TARGET 속성</param>
		/// <param name="ImgSrc">IMG 태그의 SRC 속성</param>
		/// <returns>인수에 의해 조합된 HTML 태그</returns>
		/// <example>
		/// 다음은 AHref 값이 있을 때와 없을 때의 차이를 보여줍니다.
		/// AHref 값이 없으면 A 태그 전체가 생기지 않는 것을 알 수 있습니다.
		/// <code>
		/// <![CDATA[
		/// string AHref = "www.testsite.com";
		/// string ATarget = "_blank";
		/// string ImgSrc = "/image/img.gif";
		/// 
		/// string Html = CHtml.GetHtmlLinkImage(AHref, ATarget, ImgSrc);
		/// Console.WriteLine(Html);
		/// //--결과
		/// //<a href="http://www.testsite.com" target="_blank"><img src="/image/img.gif" align="absmiddle"></a>
		/// 
		/// string Html2 = CHtml.GetHtmlLinkImage("", ATarget, ImgSrc);
		/// Console.WriteLine(Html2);
		/// //--결과
		/// //<img src="/image/img.gif" align="absmiddle">
		/// ]]>
		/// </code>
		/// </example>
		public static string GetHtmlLinkImage(string AHref, string ATarget, string ImgSrc)
		{
			string s = "";
			if (AHref != "")
			{
				if (AHref.StartsWith("www."))
				{
					AHref = "http://" + AHref;
				}

				s += "<a href=\"" + AHref + "\"";
				if (ATarget != "")
				{
					s += " target=\"" + ATarget + "\"";
				}
				s += ">";
			}

			if (ImgSrc != "")
			{
				s += GetHtmlImage(ImgSrc, "");
			}

			if (AHref != "")
			{
				s += "</a>";
			}

			return s;
		}

		public static string GetHtmlLink(string AHref, string ATarget, string Text, string Attribute)
		{
			string s = "";
			if (AHref != "")
			{
				if (AHref.StartsWith("www."))
				{
					AHref = "http://" + AHref;
				}

				s += "<a href=\"" + AHref + "\"";
				if (ATarget != "")
				{
					s += " target=\"" + ATarget + "\"";
				}
				if (Attribute != "")
				{
					s += " " + Attribute;
				}
				s += ">";
			}

			if (Text != "")
			{
				s += Text;
			}

			if (AHref != "")
			{
				s += "</a>";
			}

			return s;
		}
		public static string GetHtmlLink(string AHref, string ATarget, string Text)
		{
			return GetHtmlLink(AHref, ATarget, Text, "");
		}
		public static string GetHtmlLink(string AHref, string ATarget)
		{
			return GetHtmlLink(AHref, ATarget, AHref, "");
		}

		public static string GetHtmlImage(string ImageUrl, string Alt)
		{
			return "<img src=\"" + ImageUrl + "\" align=\"absmiddle\" alt=\"" + Alt + "\" />";
		}

		/// <summary>
		/// 이미지를 표시하고, 이미지를 클릭하면 새 창 또는 지정한 창에서 다른 URL로 이동하는 태그와 스크립트를 생성함.
		/// </summary>
		/// <param name="Href">이동할 URL</param>
		/// <param name="ATarget">Target Window의 이름(_blank: 새창)</param>
		/// <param name="ImgSrc">표시할 IMG 태그의 SRC 속성. 클릭하면 이동함.</param>
		/// <param name="Width">새 창의 너비</param>
		/// <param name="Height">새 창의 높이</param>
		/// <returns>인수에 의해 조합된 HTML 태그</returns>
		/// <example>
		/// 다음은 현재 창에 btn_game.gif 이미지를 표시하고, 
		/// 이미지를 클릭하면 스크립트를 이용해 www.testsite.com을 새창을 띄우는 문자열을 만듭니다.
		/// <code>
		/// <![CDATA[
		/// string Href = "www.testsite.com";
		/// string ImgSrc = "/images/park/btn_game.gif";
		/// string Html = CHtml.GetHtmlWindowOpenImage(Href, "_blank", ImgSrc, 870, 670);
		/// Console.WriteLine(Html);
		/// //--결과
		/// //<span style="cursor:hand;"
		/// // onclick="window.open('http://www.testsite.com', '_blank', 'width=870,height=670,toolbar=no,menubar=no,resizable=yes')">
		/// //<img src="/images/park/btn_game.gif" align="absmiddle">
		/// //</span>
		/// ]]>
		/// </code>
		/// </example>
		public static string GetHtmlWindowOpenImage(string Href, string ATarget, string ImgSrc, int Width, int Height)
		{
			string s = "";
			if (Href != "")
			{
				if (Href.StartsWith("www."))
				{
					Href = "http://" + Href;
				}

				//<a href="javascript:window.open 으로 하면 호출 페이지가 바뀌면서 [object]란 
				//문자열이 찍혀서 span의 onclick 이벤트에서 호출하는 것으로 변경
				string Param = "width=" + Width.ToString()
					+ ",height=" + Height.ToString()
					+ ",toolbar=no"
					+ ",menubar=no"
					+ ",resizable=yes";
				s += "<span style=\"cursor:hand;\" onclick=\"window.open('" + Href + "', '" + ATarget + "', '" + Param + "')\">";
			}

			if (ImgSrc != "")
			{
				s += "<img src=\"" + ImgSrc + "\" align=\"absmiddle\">";
			}

			if (Href != "")
			{
				s += "</span>";
			}

			return s;
		}

		/// <summary>
		/// 처음 몇줄만 표시하는 페이지에서 사용되며, 처음 RowCount 행만을 리턴함.
		/// </summary>
		/// <param name="Value">전체 문자열</param>
		/// <param name="MaxColumnCount">한글을 2Byte로 취급하는 최대 열너비(이 값을 초과한 문자열은 같은 줄에 있어도 다음 줄로 인식됨)</param>
		/// <param name="RowCount">표시할 처음의 n행</param>
		/// <returns><paramref name="RowCount"/>행 까지의 문자열</returns>
		/// <example>
		/// <code>
		/// string s =
		/// @"1번줄
		/// 2번줄
		/// 3번줄";
		/// 
		/// Console.WriteLine(CHtml.GetTopRow(s, 10, 2));
		/// //--결과
		/// //1번줄
		/// //2번줄
		/// 
		/// //최대 열너비를 넘었으므로 첫번째 줄에는 "1번"까지만 출력됨.
		/// Console.WriteLine(CHtml.GetTopRow(s, 4, 2));
		/// //1번
		/// //줄
		/// </code>
		/// </example>
		public static string GetTopRow(string Value, int MaxColumnCount, int RowCount)
		{
			bool IsHtmlTagUsed = GetHtmlTagUsed(Value);
			string[] aValue = SplitLine(Value, MaxColumnCount, IsHtmlTagUsed);
			if (RowCount > aValue.Length)
			{
				RowCount = aValue.Length;
			}
			if (IsHtmlTagUsed)
			{
				//실제로는 <P> 태그를 썼을 수도 있으나 <BR> 태그로 함.
				return String.Join("<br>", aValue, 0, RowCount);
			}
			else
			{
				return String.Join("\r\n", aValue, 0, RowCount);
			}
		}

		/// <summary>
		/// HTML 태그로 구성된 문자열은 BR, P 등을 행의 구분자로,
		/// 일반 텍스트 문자열은 줄바꿈을 구분자로 취급해서 행의 개수를 리턴함.
		/// </summary>
		/// <param name="Value">문자열</param>
		/// <returns>행의 총 개수</returns>
		/// <example>
		/// 다음은 HTML 문자열과 텍스트 문자열의 행 개수를 각각 출력합니다.
		/// <code>
		/// <![CDATA[
		/// string Html = "1줄<br>2줄<p>3줄";
		/// Console.WriteLine(CHtml.GetLineCount(Html)); // 3
		/// 
		/// string Text = "1줄\r\n2줄\n3줄";
		/// 
		/// Console.WriteLine(CHtml.GetLineCount(Text)); // 3
		/// ]]>
		/// </code>
		/// </example>
		public static int GetLineCount(string Value)
		{
			if ((Value + "") == "")
			{
				return 0;
			}

			bool IsHtmlTagUsed = GetHtmlTagUsed(Value);
			string[] aValue = SplitLine(Value, IsHtmlTagUsed);
			return aValue.Length;
		}

		/// <summary>
		/// HTML 태그로 구성된 문자열은 BR, P 등을 행의 구분자로,
		/// 일반 텍스트 문자열은 줄바꿈을 구분자로 취급해서 각 행을 항목으로 하는 배열을 리턴함.
		/// </summary>
		/// <param name="Value">문자열</param>
		/// <param name="MaxColumnCount">한글을 2Byte로 취급하는 최대 열너비(이 값을 초과한 문자열은 같은 줄에 있어도 다음 줄로 인식됨)</param>
		/// <param name="Include_BR_P_AsSeparator">BR, P 태그를 행 구분자로 취급할 지 여부</param>
		/// <returns>각 행을 항목으로 하는 배열</returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string Html = "1줄<br>2줄<p>3줄";
		/// 
		/// string[] aHtml = CHtml.SplitLine(Html, 2, true);
		/// Console.WriteLine(aHtml[0]); // "1"
		/// 
		/// string[] aHtml2 = CHtml.SplitLine(Html, 3, true);
		/// Console.WriteLine(aHtml2[0]); // "1줄"
		/// ]]>
		/// </code>
		/// </example>
		public static string[] SplitLine(string Value, int MaxColumnCount, bool Include_BR_P_AsSeparator)
		{
			List<string> aValueList = new List<string>();

			string[] aLineSep = null;
			if (Include_BR_P_AsSeparator)
			{
				aLineSep = new string[] { "<p>", "<p/>", "<p />", "<br>", "<br/>", "<br />" };
			}
			else
			{
				Value = Value.Replace("\r", "");
				aLineSep = new string[] { "\n" };
			}

			string[] aValue = Value.Split(aLineSep, StringSplitOptions.None);
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				if (MaxColumnCount != 0)
				{
					string[] aValueInner = CArray.SplitByLength(aValue[i], MaxColumnCount, true);
					if (aValueInner.Length > 1)
					{
						for (int j = 0, j2 = aValueInner.Length; j < j2; j++)
						{
							aValueList.Add(aValueInner[j]);
						}
					}
					else
					{
						aValueList.Add(aValue[i]);
					}
				}
				else
				{
					aValueList.Add(aValue[i]);
				}
			}

			return aValueList.ToArray();
		}
		/// <summary>
		/// HTML 태그로 구성된 문자열은 BR, P 등을 행의 구분자로,
		/// 일반 텍스트 문자열은 줄바꿈을 구분자로 취급해서 각 행을 항목으로 하는 배열을 리턴함.
		/// </summary>
		/// <param name="Value">문자열</param>
		/// <param name="Include_BR_P_AsSeparator">BR, P 태그를 행 구분자로 취급할 지 여부</param>
		/// <returns>각 행을 항목으로 하는 배열</returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string Html = "1줄<br>2줄<p>3줄";
		/// 
		/// string[] aHtml = CHtml.SplitLine(Html, true);
		/// Console.WriteLine(aHtml[1]); // "2줄"
		///
		/// string[] aHtml2 = CHtml.SplitLine(Html, false);
		/// Console.WriteLine(aHtml2[0]); // "1줄<br>2줄<p>3줄"
		/// ]]>
		/// </code>
		/// </example>
		public static string[] SplitLine(string Value, bool Include_BR_P_AsSeparator)
		{
			return SplitLine(Value, 0, Include_BR_P_AsSeparator);
		}

		/// <summary>
		/// HTML 태그가 사용되었는 지 여부를 리턴함.
		/// </summary>
		/// <param name="Value">문자열</param>
		/// <returns>HTML 태그가 사용되었는 지 여부</returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string Html = "<b>굵게</b>";
		/// string Text = "<굵게>";
		/// 
		/// Console.WriteLine(CHtml.GetHtmlTagUsed(Html)); // True
		/// Console.WriteLine(CHtml.GetHtmlTagUsed(Text)); // False
		/// ]]>
		/// </code>
		/// </example>
		public static bool GetHtmlTagUsed(string Value)
		{
			int PosOpen = Value.IndexOf("<");
			if (PosOpen == -1)
			{
				return false;
			}

			//<b>와 같은 최소한 길이 3보다 작으면 태그 아님.
			if (Value.Length < (PosOpen + 3))
				return false;

			//< 뒤에 알파벳이 없으면 태그 아님.
			UnicodeCategory uc = char.GetUnicodeCategory(Value[PosOpen + 1]);
			if ((uc != UnicodeCategory.LowercaseLetter)
				&& (uc != UnicodeCategory.UppercaseLetter))
			{
				return false;
			}

			//<, 알파벳 뒤에 > 태그가 없으면 태그 아님.
			int PosClose = Value.IndexOf(">", PosOpen + 1);
			if (PosClose == -1)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// 넘겨진 인수를 조합해서 INPUT 태그를 완성해서 리턴함.
		/// </summary>
		/// <param name="type">INPUT 태그의 type 속성의 값</param>
		/// <param name="Name">INPUT 태그의 name 속성의 값</param>
		/// <param name="Value">INPUT 태그의 value 속성의 값</param>
		/// <returns>완성된 INPUT 태그</returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string s = CHtml.GetHtmlInput("text", "txtId", "doctorgu");
		/// Console.WriteLine(s); // <input type="text" name="txtId" value="doctorgu">
		/// ]]>
		/// </code>
		/// </example>
		public static string GetHtmlInput(string type, string Name, object Value)
		{
			return "<input type=\"" + type + "\" name=\"" + Name + "\" value=\"" + Value.ToString().Replace("\"", "\\\"") + "\">";
		}

		/// <summary>
		/// OPTION 태그의 VALUE 속성과 값을 인수로 받아 OPTION 태그를 완성해서 리턴함.
		/// </summary>
		/// <param name="aValueText">OPTION 태그의 VALUE 속성과 값이 번갈아가며 순서대로 나열된 배열</param>
		/// <returns>완성된 OPTION 태그 목록</returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string[] aOption = new string[] {"nownuri.net", "나우누리", "naver.com", "네이버"};
		/// Console.WriteLine(CHtml.GetOptionList(aOption));
		/// 
		/// //--결과
		/// //<option value="nownuri.net">나우누리</option>
		/// //<option value="naver.com">네이버</option>
		/// ]]>
		/// </code>
		/// </example>
		public static string GetOptionList(string[] aValueText)
		{
			string s = "";
			for (int i = 0, i2 = aValueText.Length; i < i2; i += 2)
			{
				s += "<option value=\"" + aValueText[i] + "\">" + aValueText[i + 1] + "</option>\r\n";
			}
			return s.TrimEnd('\r', '\n');
		}
		/// <summary>
		/// OPTION 태그의 VALUE 속성과 값을 인수로 받아 OPTION 태그를 완성해서 리턴함.
		/// </summary>
		/// <param name="aValue">OPTION 태그의 VALUE 속성</param>
		/// <param name="aText">OPTION 태그의 값</param>
		/// <param name="ValueToSelect">SELECTED로 표시할 OPTION의 VALUE 속성 값</param>
		/// <returns>완성된 OPTION 태그 목록</returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string[] aValue = new string[] { "1", "2", "3" };
		/// string[] aText = new string[] { "First", "Second", "Third" };
		/// Console.WriteLine(CHtml.GetOptionList(aValue, aText, "2"));
		/// 
		/// //<option value="1">First</option>
		/// //<option selected value="2">Second</option>
		/// //<option value="3">Third</option>
		/// ]]>
		/// </code>
		/// </example>
		public static string GetOptionList(string[] aValue, string[] aText, string ValueToSelect)
		{
			string s = "";
			for (int i = 0, i2 = aValue.Length; i < i2; i++)
			{
				if (aValue[i] == ValueToSelect)
				{
					s += "<option selected value=\"" + aValue[i] + "\">" + aText[i] + "</option>\r\n";
				}
				else
				{
					s += "<option value=\"" + aValue[i] + "\">" + aText[i] + "</option>\r\n";
				}
			}
			return s.TrimEnd('\r', '\n');
		}
		/// <summary>
		/// OPTION 태그의 VALUE 속성과 값을 인수로 받아 OPTION 태그를 완성해서 리턴함.
		/// </summary>
		/// <param name="aValue">OPTION 태그의 VALUE 속성</param>
		/// <param name="aText">OPTION 태그의 값</param>
		/// <returns>완성된 OPTION 태그 목록</returns>
		/// <example>
		/// <code>
		/// <![CDATA[
		/// string[] aValue = new string[] { "1", "2", "3" };
		/// string[] aText = new string[] { "First", "Second", "Third" };
		/// Console.WriteLine(CHtml.GetOptionList(aValue, aText));
		/// 
		/// //<option value="1">First</option>
		/// //<option value="2">Second</option>
		/// //<option value="3">Third</option>
		/// ]]>
		/// </code>
		/// </example>
		public static string GetOptionList(string[] aValue, string[] aText)
		{
			return GetOptionList(aValue, aText, "");
		}

		/// <summary>
		/// 속성값을 설정함.
		/// </summary>
		/// <param name="Html">HTML 문자열</param>
		/// <param name="AttrName">속성 이름</param>
		/// <param name="AttrValue">속성 값</param>
		/// <returns>속성값이 설정된 HTML 문자열</returns>
		public static string SetAttribute(string Html, string AttrName, string AttrValue)
		{
			string Xml = CHtml.ConvertHtmlToParsableXml(Html, true);
			XmlDocument XDoc = new XmlDocument();
			XDoc.LoadXml(Xml);
			XDoc.DocumentElement.SetAttribute(AttrName, AttrValue);
			return XDoc.DocumentElement.OuterXml;
		}

		public static string GetCssRolloverImage(string ImageUrl, string CssClassName, int ImageWidth, int ImageHeight)
		{
			if ((ImageWidth == 0) || (ImageHeight == 0))
			{
				return "<a href=\"/\"><img src=\"" + ImageUrl + "\"></a>";
			}

			List<string> aStmt = new List<string>();

			//IE에선 a 태그에 background-image 사용하면 깜박거려서 해결하기 위해 common.js에서
			//document.execCommand("BackgroundImageCache", false, true) 명령 실행함.

			aStmt.Add("<style type=\"text/css\">");
			aStmt.Add("a." + CssClassName + ", a." + CssClassName + ":link, a." + CssClassName + ":visited"); //, a." + CssClassName + ":active"
			aStmt.Add("{");
			aStmt.Add("	display: block;");
			aStmt.Add("	width: " + ImageWidth.ToString() + "px;");
			aStmt.Add("	height: " + ImageHeight.ToString() + "px;");
			aStmt.Add("	background-image: url(" + ImageUrl + ");");
			aStmt.Add("	background-repeat: repeat-y;");
			aStmt.Add("	background-position: 0px 0px;");
			aStmt.Add("	overflow: hidden;");
			aStmt.Add("	border: none;");
			aStmt.Add("	float: left;"); /* eliminate float for vertical display */
			aStmt.Add("}");

			aStmt.Add("a." + CssClassName + ":hover");
			aStmt.Add("{");
			aStmt.Add("	background-position: 0px " + ImageHeight.ToString() + "px;");
			aStmt.Add("}");
			aStmt.Add("</style>");

			aStmt.Add("<a href=\"/\" class=\"" + CssClassName + "\"></a>");

			return string.Join("\r\n", aStmt.ToArray());
		}

		/// <summary>
		/// 현재 문장을 감싼 p 태그가 하나 뿐이라면 삭제함.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static string TrimPTagIfOne(string Value)
		{
			string ValueNew = Value.Trim();

			if (ValueNew.StartsWith("<p>", StringComparison.CurrentCultureIgnoreCase)
				&& ValueNew.EndsWith("</p>", StringComparison.CurrentCultureIgnoreCase)
				&& (ValueNew.IndexOf("<p>", 3, StringComparison.CurrentCultureIgnoreCase) == -1)
				)
			{
				ValueNew = CFindRep.SubstringFromTo(ValueNew, "<p>".Length, ValueNew.Length - ("</p>".Length + 1));
				return ValueNew;
			}

			return Value;
		}

		/// <summary>
		/// <![CDATA[
		/// <b>bold</b> -> bold
		/// ]]>
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Tag"></param>
		/// <returns></returns>
		public static string TrimTag(string Value, string Tag)
		{
			string ValueNew = Value.Trim();

			string TagStart = "<" + Tag + ">";
			string TagEnd = "</" + Tag + ">";

			if (ValueNew.StartsWith(TagStart, StringComparison.CurrentCultureIgnoreCase)
				&& ValueNew.EndsWith(TagEnd, StringComparison.CurrentCultureIgnoreCase))
			{
				ValueNew = ValueNew.Substring(TagStart.Length);
				ValueNew = ValueNew.Substring(0, ValueNew.Length - TagEnd.Length);
			}

			return ValueNew;
		}

		/// <summary>
		/// <![CDATA[
		/// Unhide <div style="display:none;">this</div>. -> Unhide this.
		/// ]]>
		/// </summary>
		public static string RemoveTag(string Value, string[] aTag, bool RemoveValueToo)
		{
			string ValueNew = "<x>" + Value + "</x>";
			XmlDocument XDoc = new XmlDocument();
			XDoc.LoadXml(ValueNew);

			//            XmlReaderSettings settings = new XmlReaderSettings();
			//            //settings.DtdProcessing = DtdProcessing.Parse;
			//            settings.ProhibitDtd = false;
			//            string DTD = @"<!DOCTYPE doc [
			//<!ENTITY % iso-lat1 PUBLIC ""ISO 8879:1986//ENTITIES Added Latin 1//EN//XML""
			//""http://www.oasis-open.org/docbook/xmlcharent/0.3/iso-lat1.ent"">
			//%iso-lat1;
			//]> ";
			//            //Value = string.Concat(DTD, "<xml><x>" + Value + "</x></xml>");
			//            Value = string.Concat(DTD, "<xml><txt>&rsquo;ren&eacute;</txt></xml>"); 
			//            XmlDocument XDoc = new XmlDocument();
			//            XDoc.Load(XmlReader.Create(new MemoryStream(UTF8Encoding.UTF8.GetBytes(Value)), settings)); 

			for (int i = 0; i < aTag.Length; i++)
			{
				XmlNodeList ndl = XDoc.SelectNodes("//" + aTag[i]);
				foreach (XmlNode nod in ndl)
				{
					if (RemoveValueToo)
					{
						nod.ParentNode.RemoveChild(nod);
					}
					else
					{
						string InnerText = nod.InnerText;
						nod.ParentNode.InsertBefore(XDoc.CreateTextNode(InnerText), nod);
						nod.ParentNode.RemoveChild(nod);
					}
				}
			}

			string Xml = XDoc.DocumentElement.InnerXml;
			return Xml;
		}
		public static string RemoveTag(string Value, string[] aTag)
		{
			bool RemoveValueToo = false;
			return RemoveTag(Value, aTag, RemoveValueToo);
		}
		public static string RemoveTagAllUsingRegex(string Value)
		{
			//string Pattern = "<" + Tag + "[^>]*>(?<Value>[^>]*)</" + Tag + ">";
			//Value = r.Replace(Value, "${Value}");

			//http://stackoverflow.com/questions/787932/using-c-sharp-regular-expressions-to-remove-html-tags
			//@"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>" -> 이런 답도 있음.

			string Pattern = @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>";
			Regex r = new Regex(Pattern, RegexOptions.Singleline);
			Match m = r.Match(Value);
			if (m.Success)
			{
				Value = r.Replace(Value, "");
			}

			return Value;
		}

		/// <summary>
		/// <![CDATA[
		/// <i><이탤릭></i>에서 <이탤릭> 양쪽의 <, >를 &lt;, &gt;로 변경
		/// ]]>
		/// </summary>
		public static string ReplaceLtGtAmpersand(string Value)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0, i2 = Value.Length; i < i2; i++)
			{
				char c = Value[i];

				if (c == '<')
				{
					if (!IsHtmlTag(Value, true, i))
					{
						sb.Append("&lt;");
					}
					else
					{
						int IndexClose = Value.IndexOf('>', i);
						sb.Append(Value.Substring(i, (IndexClose - i + 1)));
						i = IndexClose;
					}
				}
				else if (c == '>')
				{
					if (!IsHtmlTag(Value, false, i))
					{
						sb.Append("&gt;");
					}
					else
					{
						sb.Append(c);
					}
				}
				else if (c == '&')
				{
					sb.Append("&amp;");
				}
				else
				{
					sb.Append(c);
				}
			}

			return sb.ToString();
		}
		private static bool IsHtmlTag(string Value, bool IsStart, int Index)
		{
			string[] aTag = new string[] { "a", "abbr", "acronym", "address", "applet", "area", "b", "base", "basefont", "bdo", "big", "blockquote", "body", "br", "button", "caption", "center", "cite", "code", "col", "colgroup", "dd", "del", "dfn", "dir", "div", "dl", "dt", "em", "fieldset", "font", "form", "frame", "frameset", "h1 to h6", "head", "hr", "html", "i", "iframe", "img", "input", "ins", "isindex", "kbd", "label", "legend", "li", "link", "map", "menu", "meta", "noframes", "noscript", "object", "ol", "optgroup", "option", "p", "param", "pre", "q", "s", "samp", "script", "select", "small", "span", "strike", "strong", "style", "sub", "sup", "table", "tbody", "td", "textarea", "tfoot", "th", "thead", "title", "tr", "tt", "u", "ul", "var" };

			if (IsStart)
			{
				for (int i = 0, i2 = aTag.Length; i < i2; i++)
				{
					string[] aOpen = new string[] { "<" + aTag[i] + " ", "<" + aTag[i] + ">", "</" + aTag[i] + ">" };
					for (int j = 0; j < aOpen.Length; j++)
					{
						if (Value.Substring(Index).StartsWith(aOpen[j], StringComparison.CurrentCultureIgnoreCase))
							return true;
					}
				}
			}
			else
			{
				for (int i = 0, i2 = aTag.Length; i < i2; i++)
				{
					string Tag = "</" + aTag[i] + ">";
					int IndexStart = Index - Tag.Length + 1;
					if (IndexStart < 0)
						continue;

					if (Value.Substring(IndexStart).StartsWith(Tag, StringComparison.CurrentCultureIgnoreCase))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// HTML 코드에서 사용된 URL이 dicUrlReplace의 Key로 시작한다면 URL의 dicUrlReplace의 Key 부분을 dicUrlReplace의 Value로 변경함.
		/// </summary>
		/// <param name="dicStartingUrlReplace"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		/// <example>
		/// <![CDATA[
		/// string Html = @"
		/// <td><img src=""images/title.gif"" alt="""" /></td>
		/// <td style=""width:6px;""></td>
		/// <td style=""background-image:url('images/bg_version_left.gif'); width:4px;""></td>
		/// <td class=""version"" style=""background-image:url('images/bg_version_center.gif'); background-repeat:repeat-x; padding: 0px 2px 0px 2px;"">
		/// v <span id=""spnVersion"">1.10.20.1624</span>
		/// </td>
		/// <td style=""background-image:url('images/bg_version_right.gif'); width:4px;""></td>";
		/// 
		/// Dictionary<string, string> dicUrlReplace = new Dictionary<string,string>();
		/// dicUrlReplace.Add("images/", "question/");
		/// 
		/// string HtmlNew = ReplaceStartingUrl(Html, dicUrlReplace);
		/// ]]>
		/// </example>
		public static string ReplaceStartingUrl(string Value, ExtractUrlTypes UrlType, Dictionary<string, string> dicStartingUrlReplace,
			out List<Tuple<string, string>> aFromToIs)
		{
			aFromToIs = new List<Tuple<string, string>>();
			List<Tuple<string, string>> aFromTo = new List<Tuple<string, string>>();

			List<string> aPattern = GetExtractUrl(UrlType);

			for (int i = 0; i < aPattern.Count; i++)
			{
				Regex r = new Regex(aPattern[i], RegexOptions.IgnoreCase);
				Value = r.Replace(Value, new MatchEvaluator(delegate(Match m)
				{
					foreach (KeyValuePair<string, string> kv in dicStartingUrlReplace)
					{
						if (!m.Groups["Url"].Value.StartsWith(kv.Key))
							continue;

						int IndexFrom = m.Groups["Url"].Index - m.Index;
						int IndexTo = IndexFrom + kv.Key.Length - 1;

						string From = m.Groups["Url"].Value;
						string To = From.Replace(kv.Key, kv.Value);
						aFromTo.Add(new Tuple<string, string>(From, To));

						return m.Value.Substring(0, IndexFrom) + kv.Value + m.Value.Substring(IndexTo + 1);
					}

					return m.Value;
				}));
			}

			aFromToIs = aFromTo;

			return Value;
		}
		public static string ReplaceStartingUrl(string Value, ExtractUrlTypes UrlType, Dictionary<string, string> dicStartingUrlReplace)
		{
			List<Tuple<string, string>> aFromToIs = new List<Tuple<string, string>>();
			return ReplaceStartingUrl(Value, UrlType, dicStartingUrlReplace, out aFromToIs);
		}

		public static List<string> ExtractUrl(string Value, ExtractUrlTypes UrlType, IEnumerable<string> eStartingUrl)
		{
			List<string> aPattern = GetExtractUrl(UrlType);
			List<string> aUrlExtracted = new List<string>();

			for (int i = 0; i < aPattern.Count; i++)
			{
				Regex r = new Regex(aPattern[i], RegexOptions.IgnoreCase);
				for (Match m = r.Match(Value); m.Success; m = m.NextMatch())
				{
					string UrlExtracted = m.Groups["Url"].Value;

					if (eStartingUrl == null)
					{
						aUrlExtracted.Add(UrlExtracted);
					}
					else
					{
						foreach (string UrlCur in eStartingUrl)
						{
							if (!UrlExtracted.StartsWith(UrlCur))
								continue;

							aUrlExtracted.Add(UrlExtracted);
						}
					}
				}
			}

			return aUrlExtracted;
		}
		public static List<string> ExtractUrl(string Value, ExtractUrlTypes UrlType)
		{
			return ExtractUrl(Value, UrlType, null);
		}

		private static List<string> GetExtractUrl(ExtractUrlTypes UrlType)
		{
			List<string> aPattern = new List<string>();
			if ((UrlType & ExtractUrlTypes.UrlFromImgTag) == ExtractUrlTypes.UrlFromImgTag)
				aPattern.Add(CRegex.Pattern.ExtractUrlFromImgTag);
			if ((UrlType & ExtractUrlTypes.SwfUrlFromParamTag) == ExtractUrlTypes.SwfUrlFromParamTag)
				aPattern.Add(CRegex.Pattern.ExtractSwfUrlFromParamTag);
			if ((UrlType & ExtractUrlTypes.UrlFromBackground) == ExtractUrlTypes.UrlFromBackground)
				aPattern.Add(CRegex.Pattern.ExtractUrlFromBackground);
			if ((UrlType & ExtractUrlTypes.UrlFromStyleBackground) == ExtractUrlTypes.UrlFromStyleBackground)
				aPattern.Add(CRegex.Pattern.ExtractUrlFromStyleBackground);
			if ((UrlType & ExtractUrlTypes.UrlFromATag) == ExtractUrlTypes.UrlFromATag)
				aPattern.Add(CRegex.Pattern.ExtractUrlFromATag);
			if ((UrlType & ExtractUrlTypes.UrlFromNewUri) == ExtractUrlTypes.UrlFromNewUri)
				aPattern.Add(CRegex.Pattern.ExtractUrlFromNewUri);

			return aPattern;
		}
	}
}
