using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;

namespace DoctorGu
{
	public enum FlashAllowScriptAccess
	{
		[Description("Always permit ActionScript-to-JavaScript calls")]
		always,
		[Description("Permit ActionScript-to-JavaScript calls only when the SWF and HTML page come from the same domain")]
		sameDomain,
		[Description("Never permit ActionScript-to-JavaScript calls")]
		never
	}

	public enum FlashWmode
	{
		[Description("plays the application in its own rectangular window on a web page. Window indicates that the Flash application has no interaction with HTML layers and is always the topmost item.")]
		Window,
		[Description("makes the application hide everything behind it on the page.")]
		Opaque,
		[Description("makes the background of the HTML page show through all the transparent portions of the application and can slow animation performance.")]
		Transparent
	}

	public enum FlashSalign
	{
		[Description("Left")]
		L,
		[Description("Right")]
		R,
		[Description("Top")]
		T,
		[Description("Bottom")]
		B,
		[Description("TopLeft")]
		TL,
		[Description("TopRight")]
		TR,
		[Description("BottomLeft")]
		BL,
		[Description("BottomRight")]
		BR
	}

	public enum FlashScale
	{
		[Description("This setting renders the entire movie visible, while retaining the original aspect ratio of the movie. Because of this, the movie will not be distorted, because the proportions are constrained. Borders may be present on the two sides of the movie if the browser window's dimensions do not match the stage size's ratio.")]
		showAll, /*Default*/
		[Description("This setting scales the movie in order to fit within the specified area, to ensure that no borders will appear. The movie maintains it's original aspect ratio. No distortion will occur, but the sides of the movie may be cropped off.")]
		noBorder,
		[Description("This setting ensures that the entire movie will be visible within the specified area. Because the movie is scaled to fit at a ratio that may not reflect the original file, distortion of the movie may occur.")]
		exactFit,
		[Description("No scale")]
		noScale
	}

	public class CSwfObject
	{
		public static string GetScriptWrite(string Version, int Width, int Height,
				FlashAllowScriptAccess AllowScriptAccess, FlashWmode Wmode, FlashSalign Salign, FlashScale Scale,
				Color BgColor, string FlashVars, string Movie, string FlashId, string DivIdForContainer)
		{
			string DivId = string.IsNullOrEmpty(DivIdForContainer) ? "div" + FlashId : DivIdForContainer;

			Dictionary<string, string> nv = new Dictionary<string, string>();
			nv.Add("menu", "false");
			nv.Add("allowScriptAccess", AllowScriptAccess.ToString());
			nv.Add("wmode", Wmode.ToString());
			nv.Add("salign", Salign.ToString());
			nv.Add("scale", Scale.ToString());
			nv.Add("bgcolor", CColorConv.GetHexaByColor(BgColor));
			nv.Add("flashvars", FlashVars);
			string Params = CScript.GetScriptKeyValueByColon(nv, true);

			string s = "";
			if (string.IsNullOrEmpty(DivIdForContainer))
			{
				s += "<div id=\"" + DivId + "\">" +
					"<a href=\"http://www.adobe.com/go/getflashplayer\">" +
					"<img src=\"http://www.adobe.com/images/shared/download_buttons/get_flash_player.gif\" alt=\"Get Adobe Flash player\" />" +
					"</a>" +
					"</div>\r\n";
			}

			List<string> aStmt = new List<string>();
			aStmt.Add("var flashvars = false;");
			aStmt.Add("var params = { " + Params + " };");
			aStmt.Add("var attributes = { id: \"" + FlashId + "\", name: \"" + FlashId + "\" };");
			aStmt.Add("swfobject.embedSWF(\"" + Movie + "\", \"" + DivId + "\", \"" + Width.ToString() + "\", \"" + Height.ToString() + "\", \"" + Version + "\", \"\", flashvars, params, attributes);");

			string s2 = CScript.GetScript(aStmt, true);

			return s + s2;
		}
		public static string GetScriptWrite(string Version, int Width, int Height,
				FlashAllowScriptAccess AllowScriptAccess, FlashWmode Wmode, FlashSalign Salign, FlashScale Scale,
				Color BgColor, string FlashVars, string Movie, string FlashId)
		{
			return GetScriptWrite(Version, Width, Height, AllowScriptAccess, Wmode, Salign, Scale, BgColor, FlashVars, Movie, FlashId, "");
		}
	}
}
