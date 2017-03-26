using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Web;
using System.Data;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;

namespace DoctorGu
{
	public struct FeaturesForWindowOpen
	{
		/// <summary>
		/// Internet Explorer 7. Sets the width of the window in pixels. The minimum value is 250, and specifies the minimum width of the browsers content area. 
		/// </summary>
		public int? width;

		/// <summary>
		/// Internet Explorer 7. Sets the height of the window in pixels. The minimum value is 150, and specifies the minimum height of the browser content area. 
		/// Prior to Internet Explorer 7 the minimum height value is 100. 
		/// </summary>
		public int? height;

		/// <summary>
		/// Specifies the left position, in pixels. This value is relative to the upper-left corner of the screen. The value must be greater than or equal to 0. 
		/// </summary>
		public int? left;

		/// <summary>
		/// Specifies the top position, in pixels. This value is relative to the upper-left corner of the screen. The value must be greater than or equal to 0. 
		/// </summary>
		public int? top;

		/// <summary>
		/// { yes | no | 1 | 0 }
		/// Internet Explorer 7. Specifies whether to display the navigation bar. The default is yes. 
		/// Prior to Internet Explorer 7 this feature specifies whether to display the address bar. 
		/// The Back, Forward, and Stop commands are now located in the Navigation bar. Prior to Internet Explorer 7 navigation commands were located in the toolbar.
		/// </summary>
		public bool? location;

		///// <summary>
		///// { yes | no | 1 | 0 }
		///// Specifies whether to add directory buttons. The default is yes. 
		///// Internet Explorer 7. This feature is no longer supported. 
		///// </summary>
		//public bool? directories;

		/// <summary>
		/// { yes | no | 1 | 0 }
		/// Specifies whether to display the menu bar. The default is yes. 
		/// Internet Explorer 7. By default the menu bar is hidden unless revealed by the ALT key. menubar = { no | 0 } prohibits the menubar from appearing even when the Alt key is pressed. 
		/// The combination of menubar = { no | 0 } and toolbar = { no | 0 } hides the toolbar and disables any additional third-party user interfaces. 
		/// </summary>
		public bool? menubar;

		/// <summary>
		/// { yes | no | 1 | 0 }
		/// Specifies whether to display resize handles at the corners of the window. The default is yes. 
		/// Internet Explorer 7. resizable = { no | 0 } disables tabs in a new window.
		/// </summary>
		public bool? resizable;

		/// <summary>
		/// { yes | no | 1 | 0 }
		/// Specifies whether to display horizontal and vertical scroll bars. The default is yes.
		/// </summary>
		public bool? scrollbars;

		/// <summary>
		/// { yes | no | 1 | 0 }
		/// Specifies whether to add a Status Bar at the bottom of the window. The default is yes.
		/// </summary>
		public bool? status;

		/// <summary>
		/// { yes | no | 1 | 0 }
		/// Internet Explorer 7. Specifies whether to display the browser command bar, making buttons such as Favorites Center, Add to Favorites, and Tools available. The default is yes. 
		/// The combination of menubar = { no | 0 } and toolbar = { no | 0 } turn off the Toolbar and any additional third-party user interfaces. 
		/// Prior to Internet Explorer 7 the toolbar sFeature specifies whether to display the browser toolbar, making buttons such as Back, Forward, and Stop available. 
		/// </summary>
		public bool? toolbar;
	}

	public class CScript
	{
		public static string GetScriptAlert(string Msg, bool AddScriptTag)
		{
			string s = "alert(\"" + CScript.ReplaceForScriptVariable(Msg) + "\");";

			s = CScript.GetScript(new string[] { s }, AddScriptTag);

			return s;
		}
		public static string GetScriptAlert(string Msg)
		{
			return GetScriptAlert(Msg, false);
		}

		public static string GetScriptAlertJQuery(string Msg, bool AddScriptTag)
		{
			string Alert = GetScriptAlert(Msg, AddScriptTag);

			List<string> aStmt = new List<string>();
			aStmt.Add("if (typeof jQuery != 'undefined')");
			aStmt.Add("{");
			aStmt.Add("	$(document).ready(function () { " + Alert + " });");
			aStmt.Add("}");
			aStmt.Add("else");
			aStmt.Add("{");
			aStmt.Add("	" + Alert);
			aStmt.Add("}");
			string Script = GetScript(aStmt);

			return Script;
		}
		public static string GetScriptAlertJQuery(string Msg)
		{
			return GetScriptAlertJQuery(Msg, false);
		}

		public static string GetScriptDocumentWrite(string Value, bool AddScriptTag)
		{
			string s = "document.write(\"" + CScript.ReplaceForScriptVariable(Value) + "\");";

			s = CScript.GetScript(new string[] { s }, AddScriptTag);

			return s;
		}
		public static string GetScriptDocumentWrite(string Msg)
		{
			return GetScriptDocumentWrite(Msg, false);
		}

		public static string GetScriptWindowOpen(string Url, string Target, FeaturesForWindowOpen Features, bool PrefixJavascriptVoid)
		{
			string sFeatures = "";

			if (Features.width != null)
				sFeatures += ",width=" + Features.width.ToString();
			if (Features.height != null)
				sFeatures += ",height=" + Features.height.ToString();
			if (Features.left != null)
				sFeatures += ",left=" + Features.left.ToString();
			if (Features.top != null)
				sFeatures += ",top=" + Features.top.ToString();

			if (Features.location != null)
				sFeatures += ",location=" + CFindRep.IfTrueThen1FalseThen0(Features.location.Value);
			//if (Features.directories != null)
			//    sFeatures += ",directories=" + CFindRep.IfTrueThen1FalseThen0(Features.directories.Value);
			if (Features.menubar != null)
				sFeatures += ",menubar=" + CFindRep.IfTrueThen1FalseThen0(Features.menubar.Value);
			if (Features.resizable != null)
				sFeatures += ",resizable=" + CFindRep.IfTrueThen1FalseThen0(Features.resizable.Value);
			if (Features.scrollbars != null)
				sFeatures += ",scrollbars=" + CFindRep.IfTrueThen1FalseThen0(Features.scrollbars.Value);
			if (Features.status != null)
				sFeatures += ",status=" + CFindRep.IfTrueThen1FalseThen0(Features.status.Value);
			if (Features.toolbar != null)
				sFeatures += ",toolbar=" + CFindRep.IfTrueThen1FalseThen0(Features.toolbar.Value);

			if (!string.IsNullOrEmpty(sFeatures))
				sFeatures = sFeatures.Substring(1);

			string s =
				(PrefixJavascriptVoid ? "javascript:void(" : "")
				+ "window.open(\"" + Url + "\", \"" + Target + "\", \"" + sFeatures + "\")"
				+ (PrefixJavascriptVoid ? ");" : ";");

			return s;
		}

		public static string GetScript(IEnumerable<string> aStmt, bool AddScriptTag)
		{
			string s = "";

			if (AddScriptTag)
			{
				s += "<script language=\"javascript\" type=\"text/javascript\">\r\n";
			}
			foreach (string StmtCur in aStmt)
			{
				s += StmtCur + "\r\n";
			}
			if (AddScriptTag)
			{
				s += "</script>\r\n";
			}

			return s;
		}
		public static string GetScript(string Stmt, bool AddScriptTag)
		{
			return CScript.GetScript(new string[] { Stmt }, AddScriptTag);
		}
		public static string GetScript(IEnumerable<string> aStmt)
		{
			return CScript.GetScript(aStmt, false);
		}

		public static void Alert(string Msg, Page p)
		{
			string Alert = CScript.GetScriptAlertJQuery(Msg);
			p.ClientScript.RegisterStartupScript(typeof(Page), "AlertOnStartup", Alert, true);
		}
		/// <summary>
		/// You must use a ScriptManager control on a page to enable the following features of ASP.NET AJAX:
		///•Client-script functionality of the Microsoft AJAX Library, and any custom script that you want to send to the browser. For more information, see ASP.NET AJAX and JavaScript.
		///•Partial-page rendering, which enables regions on the page to be independently refreshed without a postback. The ASP.NET AJAX UpdatePanel, UpdateProgress, and Timer controls require a ScriptManager control to support partial-page rendering.
		///•JavaScript proxy classes for Web services, which enable you to use client script to access Web services by exposing Web services as strongly typed objects.
		///•JavaScript classes to access ASP.NET authentication and profile application services.
		/// http://forums.asp.net/t/1169763.aspx/1
		/// </summary>
		public static void Alert(string Msg, Control ctl)
		{
			string Alert = CScript.GetScriptAlertJQuery(Msg);
			ScriptManager.RegisterStartupScript(ctl, ctl.GetType(), "AlertOnStartup", Alert, true);
		}
		public static void Alert(string Msg, bool EndResponse, HttpContext ctx)
		{
			string Alert = CScript.GetScriptAlert(Msg);
			string s = CScript.GetScript(new string[] { Alert }, true);
			HttpContext.Current.Response.Write(s);

			if (EndResponse)
				ctx.Response.End();
		}
		public static void Alert(string Msg)
		{
			Alert(Msg, false, HttpContext.Current);
		}
		public static void AlertAndBack(string Msg, bool EndResponse, HttpContext ctx)
		{
			string Alert = CScript.GetScriptAlert(Msg);
			string Back = "history.back();\r\n";

			string s = CScript.GetScript(new string[] { Alert, Back }, true);

			ctx.Response.Write(s);

			if (EndResponse)
				ctx.Response.End();
		}
		public static void AlertAndBack(string Msg, bool EndResponse)
		{
			AlertAndBack(Msg, EndResponse, HttpContext.Current);
		}
		public static void AlertAndBack(string Msg)
		{
			AlertAndBack(Msg, false, HttpContext.Current);
		}
		public static void AlertAndRedirectTo(string Msg, string Url, bool EndResponse, HttpContext ctx)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add(CScript.GetScriptAlert(Msg));
			aStmt.Add("window.location.href = \"" + Url + "\";");
			string s = CScript.GetScript(aStmt, true);

			ctx.Response.Write(s);

			if (EndResponse)
				ctx.Response.End();
		}
		public static void AlertAndRedirectTo(string Msg, string Url)
		{
			AlertAndRedirectTo(Msg, Url, false, HttpContext.Current);
		}

		public static void AlertAndRedirectOpenerAndCloseWindow(string Msg, string Url, bool EndResponse, HttpContext ctx)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add(CScript.GetScriptAlert(Msg));
			aStmt.Add("if (window.opener)");
			aStmt.Add("{");
			aStmt.Add("	window.opener.location.href = \"" + Url + "\";");
			aStmt.Add("}");
			//팝업 띄운 창에서 window.close()를 쓰면 Chrome, Safari의 경우 fail 메세지 표시되고 안 닫히므로 타이머를 사용해 닫음.
			aStmt.Add("setTimeout(function () { window.close(); }, 100);");
			string s = CScript.GetScript(aStmt, true);

			ctx.Response.Write(s);

			if (EndResponse)
				ctx.Response.End();
		}
		public static void AlertAndRedirectOpenerAndCloseWindow(string Msg, string Url)
		{
			AlertAndRedirectOpenerAndCloseWindow(Msg, Url, false, HttpContext.Current);
		}

		public static void RedirectOpenerAndCloseWindow(string Url)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add("if (window.opener)");
			aStmt.Add("{");
			aStmt.Add("	window.opener.location.href = \"" + Url + "\";");
			aStmt.Add("}");
			//팝업 띄운 창에서 window.close()를 쓰면 Chrome, Safari의 경우 fail 메세지 표시되고 안 닫히므로 타이머를 사용해 닫음.
			aStmt.Add("setTimeout(function () { window.close(); }, 100);");
			string s = CScript.GetScript(aStmt, true);

			HttpContext.Current.Response.Write(s);
			HttpContext.Current.Response.End();
		}
		public static void SubmitOpenerAndCloseWindow(string OpenerFrmName)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add("if (window.opener) {");
			aStmt.Add("	window.opener.document.forms[\"" + OpenerFrmName + "\"].submit();");
			aStmt.Add("}");
			//팝업 띄운 창에서 window.close()를 쓰면 Chrome, Safari의 경우 fail 메세지 표시되고 안 닫히므로 타이머를 사용해 닫음.
			aStmt.Add("setTimeout(function () { window.close(); }, 100);");
			string s = CScript.GetScript(aStmt, true);

			HttpContext.Current.Response.Write(s);
			HttpContext.Current.Response.End();
		}

		public static void AlertAndRedirectToRoot(string Msg, bool EndResponse)
		{
			AlertAndRedirectTo(Msg, "/", EndResponse, HttpContext.Current);
		}
		public static void AlertAndRedirectToRoot(string Msg)
		{
			bool EndResponse = false;
			AlertAndRedirectToRoot(Msg, EndResponse);
		}

		public static void AlertAndClose(string Msg, bool EndResponse, HttpContext ctx)
		{
			string Alert = CScript.GetScriptAlert(Msg);
			string Close = "window.close();";
			string s = CScript.GetScript(new string[] { Alert, Close }, true);
			ctx.Response.Write(s);

			if (EndResponse)
				ctx.Response.End();
		}
		public static void AlertAndClose(string Msg, bool EndResponse)
		{
			AlertAndClose(Msg, EndResponse, HttpContext.Current);
		}
		private static void AlertAndFocus(string Msg, string CtlClientID, Page p, Control ctl)
		{
			List<string> aStmt = new List<string>();

			string Alert = CScript.GetScriptAlert(Msg);
			aStmt.Add(Alert);

			if (!string.IsNullOrEmpty(CtlClientID))
			{
				string Focus = GetScriptFocus(CtlClientID);
				aStmt.Add(Focus);
			}

			string Stmt = CScript.GetScript(aStmt);
			if (p != null)
				p.ClientScript.RegisterStartupScript(typeof(Page), "AlertAndFocus", Stmt, true);
			else
				ScriptManager.RegisterStartupScript(ctl, ctl.GetType(), "AlertAndFocus", Stmt, true);
		}
		public static void AlertAndFocus(string Msg, string CtlClientID, Page p)
		{
			AlertAndFocus(Msg, CtlClientID, p, null);
		}
		public static void AlertAndFocus(string Msg, string CtlClientID, Control ctl)
		{
			AlertAndFocus(Msg, CtlClientID, null, ctl);
		}

		public static void SetFocus(string CtlClientID, Page p)
		{
			string Focus = GetScriptFocus(CtlClientID);
			p.ClientScript.RegisterStartupScript(typeof(Page), "Focus", Focus, true);
		}
		public static void SetFocus(string CtlClientID, Control ctl)
		{
			string Focus = GetScriptFocus(CtlClientID);
			ScriptManager.RegisterStartupScript(ctl, ctl.GetType(), "Focus", Focus, true);
		}
		private static string GetScriptFocus(string CtlClientID)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add("document.getElementById(\"" + CtlClientID + "\").focus();");
			return GetScript(aStmt);
		}

		public static void AddStyleSheetAsUrl(string FullPath)
		{
			string s =
	@"<script language=""javascript"" type=""text/javascript"">
if (document.createStyleSheet) {
	document.createStyleSheet('" + FullPath + @"');
}
else {
	var styles = ""@import url('" + FullPath + @"');""
	var newSS = document.createElement('link');
	newSS.rel = 'stylesheet';
	newSS.href = 'data:text/css,' + escape(styles);
	document.getElementsByTagName(""head"")[0].appendChild(newSS);
}
</script>";

			HttpContext.Current.Response.Write(s);
		}
		public static void AddStyleSheetAsContent(string Content)
		{
			string s = "<style type=text/css>\r\n" + Content + "\r\n</style>";
			HttpContext.Current.Response.Write(s);
		}

		public static string GetClientArray<T>(IEnumerable<T> eValue, string VarName, bool AddScriptTag)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add("var " + VarName + " = [];");

			int i = -1;
			foreach (T Value in eValue)
			{
				string Quot = "";
				string ValueNew = "";

				if (CLang.In(typeof(T), typeof(string), typeof(char)))
				{
					Quot = "\"";
					ValueNew = CScript.ReplaceForScriptVariable(Value.ToString());
				}
				else if (typeof(T) == typeof(DateTime))
				{
					ValueNew = CScript.GetJavaScriptDateTime(Convert.ToDateTime(Value));
				}
				else
				{
					Quot = "";
					ValueNew = Value.ToString();
				}

				aStmt.Add(VarName + "[" + (++i).ToString() + "] = " + Quot + ValueNew + Quot + ";");
			}

			return CScript.GetScript(aStmt, AddScriptTag);
		}

//        /// <summary>
//        /// 서버의 DataTable을 클라이언트의 자바스크립트에서 쓸 수 있도록 변경함.
//        /// </summary>
//        /// <param name="dt"></param>
//        /// <param name="AddScriptTag">&lt;script&gt; 태그 추가</param>
//        /// <remarks>오라클의 경우는 필드 이름이 전부 대문자로 리턴되므로 소문자로 변경하려면 ColumnNameToLower 인수를 true로 할 것.</remarks>
//        /// <returns></returns>
//        private static string GetClientDataSet(object DataSetOrDataTable, string VarName, bool IsArray, bool IsDataTable, bool ColumnNameToLower, bool AddScriptTag)
//        {
//            List<DataSet> ads = new List<DataSet>();
//            List<DataTable> adt = new List<DataTable>();

//            if (IsDataTable)
//            {
//                if (IsArray)
//                {
//                    adt.AddRange((IEnumerable<DataTable>)DataSetOrDataTable);
//                }
//                else
//                {
//                    adt.Add((DataTable)DataSetOrDataTable);
//                }
//            }
//            else
//            {
//                if (IsArray)
//                {
//                    ads.AddRange((IEnumerable<DataSet>)DataSetOrDataTable);
//                }
//                else
//                {
//                    ads.Add((DataSet)DataSetOrDataTable);
//                }
//            }

//            List<string> aStmt = new List<string>();

//            if (IsDataTable)
//            {
//                if (IsArray)
//                {
//                    aStmt.Add("var " + VarName + " = [];");
//                }

//                for (int nTbl = 0; nTbl < adt.Count; nTbl++)
//                {
//                    DataTable dt = adt[nTbl];

//                    string VarNameCur = IsArray ? VarName + "[" + nTbl + "]" : VarName;
//                    aStmt = GetClientDataTableCommon(aStmt, dt, VarNameCur, ColumnNameToLower);
//                }
//            }
//            else
//            {
//                /*
//var ads = [];

//ads[0] = { };
//ads[0].Tables = [];
//ads[0].Tables[0] = new CDataTable();
//ads[0].Tables[1] = new CDataTable();


//ads[1].Tables = [];
//ads[1].Tables[0] = new CDataTable();
//ads[1].Tables[1] = new CDataTable();


//var ds = { };
//ds.Tables = [];
//ds.Tables[0] = new CDataTable();
//ds.Tables[1] = new CDataTable();

//*/
//                if (IsArray)
//                    aStmt.Add("var " + VarName + " = [];");
//                else
//                    aStmt.Add("var " + VarName + " = { Tables: [] };");

//                for (int nDs = 0; nDs < ads.Count; nDs++)
//                {
//                    DataSet ds = ads[nDs];

//                    if (IsArray)
//                        aStmt.Add(VarName + "[" + nDs + "] = { Tables: [] };");

//                    string VarNameCur = IsArray ? VarName + "[" + nDs + "].Tables" : VarName + ".Tables";

//                    for (int nTbl = 0; nTbl < ds.Tables.Count; nTbl++)
//                    {
//                        DataTable dt = ds.Tables[nTbl];

//                        string VarNameCur2 = VarNameCur + "[" + nTbl + "]";
//                        aStmt = GetClientDataTableCommon(aStmt, dt, VarNameCur2, ColumnNameToLower);
//                    }
//                }
//            }


//            string s = CScript.GetScript(aStmt, AddScriptTag);
//            return s;
//        }
		//private static List<string> GetClientDataTableCommon(List<string> aStmt, DataTable dt, string VarName, bool ColumnNameToLower)
		//{
		//    aStmt.Add(VarName + " = new CDataTable();");

		//    string[] aColName = GetColumnNameInArray(dt, ColumnNameToLower);

		//    string ColumnNameList = "";
		//    for (int cl = 0; cl < aColName.Length; cl++)
		//    {
		//        ColumnNameList += ", \"" + aColName[cl] + "\"";
		//    }
		//    ColumnNameList = ColumnNameList.Substring(2);
		//    aStmt.Add(VarName + ".Columns = [ " + ColumnNameList + " ];");

		//    string RowList = GetClientDataRow(dt, aColName);
		//    aStmt.Add(VarName + ".Rows = " + RowList + ";");

		//    return aStmt;
		//}

		//public static string GetClientDataRow(DataTable dt, string[] aColName)
		//{
		//    List<string> aRow = new List<string>();
		//    for (int rw = 0, rw2 = dt.Rows.Count; rw < rw2; rw++)
		//    {
		//        DataRow dr = dt.Rows[rw];

		//        string[] aNameValue = new string[dt.Columns.Count];
		//        for (int cl = 0, cl2 = dt.Columns.Count; cl < cl2; cl++)
		//        {
		//            object oValue = dr[cl];

		//            string Text = GetValueForJson(oValue);

		//            //JSON 형식을 리턴할 때 Name에 큰 따옴표를 붙여야만 에러를 내지 않음.
		//            aNameValue[cl] = "\"" + aColName[cl] + "\":" + Text;
		//        }
		//        string NameValueList = string.Join(", ", aNameValue);
		//        aRow.Add("{ " + NameValueList + " }");
		//    }

		//    string RowList = "[ " + string.Join(",\r\n", aRow.ToArray()) + " ]";
		//    return RowList;
		//}
		//public static string GetClientDataRow(DataTable dt)
		//{
		//    string[] aColName = GetColumnNameInArray(dt);
		//    return GetClientDataRow(dt, aColName);
		//}

		/// <summary>
		/// JSON 형식으로 리턴하기 위함.
		/// null은 null로, 날짜 형식과 문자열은 따옴표를 양쪽에 묶고, 나머지는 따옴표를 묶지 않은 값을 리턴함.
		/// </summary>
		/// <param name="oValue"></param>
		/// <returns></returns>
		public static string GetValueForJson(object oValue)
		{
			string NullString = "null";
			string Delim = "";

			string Text = CType.ConvertColumnValueToString(oValue, NullString);
			if (Text != NullString)
			{
				SqlColumnTypeSimple TypeSimple = CSql.GetColumnTypeSimple(oValue);
				if (TypeSimple == SqlColumnTypeSimple.DateTime)
				{
					Delim = "\"";

					var dtValue = (DateTime)oValue;
					Text = Delim + dtValue.ToString(CConst.Format_yyyy_MM_dd_HH_mm_ss_fff) + Delim;

					//new Date는 JSON 형식에 맞지 않으므로 주석.
					//Text = string.Format("new Date({0}, {1}, {2}, {3}, {4}, {5}, {6})",
					//    dtValue.Year, dtValue.Month - 1, dtValue.Day,
					//    dtValue.Hour, dtValue.Minute, dtValue.Second, dtValue.Millisecond);
				}
				else if (TypeSimple == SqlColumnTypeSimple.Boolean)
				{
					Text = (bool)oValue ? "true" : "false";
				}
				else
				{
					Delim = (TypeSimple == SqlColumnTypeSimple.String) ? "\"" : "";
					Text = Delim + CScript.ReplaceForScriptVariable(Text) + Delim;
				}
			}

			return Text;
		}

		//private static string[] GetColumnNameInArray(DataTable dt, bool ColumnNameToLower)
		//{
		//    string[] aColName = new string[dt.Columns.Count];
		//    for (int cl = 0, cl2 = dt.Columns.Count; cl < cl2; cl++)
		//    {
		//        string ColumnName = dt.Columns[cl].ColumnName;
		//        if (ColumnNameToLower)
		//        {
		//            //오라클의 경우 필드 이름을 전부 대문자로 리턴하므로
		//            ColumnName = ColumnName.ToLower();
		//        }

		//        aColName[cl] = ColumnName;
		//    }

		//    return aColName;
		//}
		//private static string[] GetColumnNameInArray(DataTable dt)
		//{
		//    bool ColumnNameToLower = false;
		//    return GetColumnNameInArray(dt, ColumnNameToLower);
		//}

		//public static string GetClientDataSet(IEnumerable<DataSet> ads, string VarName, bool ColumnNameToLower, bool AddScriptTag)
		//{
		//    bool IsArray = true;
		//    bool IsDataTable = false;
		//    return GetClientDataSet(ads, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}
		//public static string GetClientDataSet(IEnumerable<DataSet> ads, string VarName, bool AddScriptTag)
		//{
		//    bool IsArray = true;
		//    bool IsDataTable = false;
		//    bool ColumnNameToLower = false;
		//    return GetClientDataSet(ads, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}
		//public static string GetClientDataSet(DataSet ds, string VarName, bool ColumnNameToLower, bool AddScriptTag)
		//{
		//    bool IsArray = false;
		//    bool IsDataTable = false;
		//    return GetClientDataSet(ds, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}
		//public static string GetClientDataSet(DataSet ds, string VarName, bool AddScriptTag)
		//{
		//    bool IsArray = false;
		//    bool IsDataTable = false;
		//    bool ColumnNameToLower = false;
		//    return GetClientDataSet(ds, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}

		//public static string GetClientDataTable(IEnumerable<DataTable> adt, string VarName, bool ColumnNameToLower, bool AddScriptTag)
		//{
		//    bool IsArray = true;
		//    bool IsDataTable = true;
		//    return GetClientDataSet(adt, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}
		//public static string GetClientDataTable(IEnumerable<DataTable> adt, string VarName, bool AddScriptTag)
		//{
		//    bool IsArray = true;
		//    bool IsDataTable = true;
		//    bool ColumnNameToLower = false;
		//    return GetClientDataSet(adt, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}
		//public static string GetClientDataTable(DataTable dt, string VarName, bool ColumnNameToLower, bool AddScriptTag)
		//{
		//    bool IsArray = false;
		//    bool IsDataTable = true;
		//    return GetClientDataSet(dt, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}
		//public static string GetClientDataTable(DataTable dt, string VarName, bool AddScriptTag)
		//{
		//    bool IsArray = false;
		//    bool IsDataTable = true;
		//    bool ColumnNameToLower = false;
		//    return GetClientDataSet(dt, VarName, IsArray, IsDataTable, ColumnNameToLower, AddScriptTag);
		//}

		//스타일 적용을 망쳐서 주석
		///// <summary>
		///// 자바스크립트 코드를 .cs 파일에서 쓰면 문서의 가장 앞에 쓰여지므로
		///// 문서가 로드된 후에 참조할 수 있는 컨트롤을 참조할 경우엔 에러가 발생하므로
		///// Window의 onload 이벤트에서 실행될 수 있도록 함.
		///// </summary>
		///// <param name="Stmt"></param>
		//public static void WriteScriptAsWindowOnload(string Stmt)
		//{
		//    if (Stmt.IndexOf("<script") != -1)
		//    {
		//        throw new Exception("Stmt의 내용에 <script 태그를 사용할 수 없습니다.");
		//    }

		//    string s = "<script for=\"window\" event=\"onload\" language=\"javascript\">\r\n"
		//                + Stmt + "\r\n"
		//                + "</script>\r\n";

		//    HttpContext.Current.Response.Write(s);
		//}

		public static void WriteScript(string Stmt)
		{
			if (Stmt.IndexOf("<script") != -1)
			{
				throw new Exception("Stmt의 내용에 <script 태그를 사용할 수 없습니다.");
			}

			string s = "<script language=\"jscript\">\r\n"
						+ Stmt + "\r\n"
						+ "</script>\r\n";

			HttpContext.Current.Response.Write(s);
		}

		/// <summary>
		/// ASP.Net에서 Cookie를 설정할 때 Domain 속성을 설정하면 실제로는 작동되지 않아 JScript로 Cookie를 설정함.
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="Value"></param>
		/// <param name="ExpireDays"></param>
		/// <param name="Domain"></param>
		/// <returns></returns>
		public static string SetCookie(string Key, string Value, int ExpireDays, string Domain)
		{
			List<string> aStmt = new List<string>();

			aStmt.Add("var Now = new Date();");
			aStmt.Add("Now.setDate(Now.getDate() + " + ExpireDays.ToString() + ");");

			aStmt.Add("var Cookie = \"" + Key + "=" + Value + ";\";");
			aStmt.Add("Cookie += \"path=/;\";");

			if (ExpireDays > 0)
			{
				aStmt.Add("Cookie += \"expires=\" + Now.toGMTString() + \";\";");
			}

			if (Domain != "")
			{
				aStmt.Add("Cookie += \"domain=" + Domain + ";\";");
			}

			aStmt.Add("document.cookie = Cookie;");

			return CScript.GetScript(aStmt, false);
		}
		public static string SetCookie(string Key, string Value, string Domain)
		{
			return SetCookie(Key, Value, 0, Domain);
		}

		/// <summary>
		/// 줄바꿈, 탭 등을 감안해서 스크립트의 변수에 값을 입력하는 코드를 생성함.
		/// </summary>
		/// <param name="Value">문자열 값</param>
		/// <returns>줄바꿈, 탭 등이 \n, \t 등의 문자열로 변환된 문자열</returns>
		/// <example>
		/// <code>
		/// string s = 
		/// @"1번줄
		/// 2번줄
		/// 3번줄";
		/// 
		/// Console.WriteLine(CScript.ReplaceForScriptVariable(s)); // "1번줄\r\n2번줄\r\n3번줄"
		/// </code>
		/// </example>
		public static string ReplaceForScriptVariable(string Value)
		{
			if (Value == null)
				return Value;

			return Value
					.Replace("\\", "\\\\")
					.Replace("\"", "\\\"")
					.Replace("\r", "\\r")
					.Replace("\n", "\\n")
					.Replace("\t", "\\t");
		}

		public static string GetJavaScriptDate(DateTime dt)
		{
			return string.Concat("new Date(", dt.Year, ", ", (dt.Month - 1), ", ", dt.Day, ")");
		}
		public static string GetJavaScriptDateTime(DateTime dt)
		{
			return string.Concat("new Date(", dt.Year, ", ", (dt.Month - 1), ", ", dt.Day, ", ", dt.Hour, ", ", dt.Minute, ", ", dt.Second, ")");
		}

		public static string GetArraySetting(string ArrayName, int ArrayIndex, string Value, bool AddQuote)
		{
			string Quot = (AddQuote ? "\"" : "");
			return string.Concat(ArrayName, "[", ArrayIndex, "] = ", Quot, CScript.ReplaceForScriptVariable(Value.ToString()), Quot, ";");
		}
		public static string GetArraySetting(string ArrayName, int ArrayIndex, string Value)
		{
			return GetArraySetting(ArrayName, ArrayIndex, Value, false);
		}

		/// <summary>
		/// width: "100", height: "100", 와 같은 문자열을 만들기 위함.
		/// </summary>
		/// <param name="KeyValue"></param>
		/// <param name="EncloseDoubleQuote"></param>
		/// <returns></returns>
		public static string GetScriptKeyValueByColon(Dictionary<string, string> KeyValue, bool EncloseDoubleQuote)
		{
			string s = "";
			string q = EncloseDoubleQuote ? "\"" : "";
			foreach (KeyValuePair<string, string> kv in KeyValue)
			{
				string Value = EncloseDoubleQuote ? CScript.ReplaceForScriptVariable(kv.Value) : kv.Value;
				s += ", " + kv.Key + ": " + q + Value + q;
			}
			if (s != "")
				s = s.Substring(1);

			return s;
		}

		/// <summary>
		/// PostBackUrl 속성이 있는 Button, ImageButton, LinkButton을 자바스크립트에서 호출 가능한 함수를 리턴함.
		/// </summary>
		public static string GetScriptDoPostBackWithOptions(string ClientIDOfButton, string PostBackUrl, bool IsLinkButton)
		{
			//LinkButton의 경우, clientSubmit가 true값을 가짐.
			//function WebForm_PostBackOptions(eventTarget, eventArgument, validation, validationGroup, actionUrl, trackFocus, clientSubmit)

			string clientSubmit = IsLinkButton ? "true" : "false";
			string Script = "WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions('" + ClientIDOfButton + "', '', false, '', '" + PostBackUrl + "', false, " + clientSubmit + "));";
			return Script;
		}
		/// <summary>
		/// PostBackUrl 속성이 없는 Button, ImageButton, LinkButton을 자바스크립트에서 호출 가능한 함수를 리턴함.(ImageButton, LinkButton은 테스트 못했음)
		/// </summary>
		/// <param name="ClientIDOfButton"></param>
		/// <returns></returns>
		public static string GetScriptDoPostBack(string ClientIDOfButton)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add("var __btn = document.getElementById('" + ClientIDOfButton + "');");
			aStmt.Add("__doPostBack(__btn.name, 'OnClick');");
			return GetScript(aStmt);
		}

		/// <summary>
		/// TextBox의 TextMode가 MultiLine이면 MaxLength가 먹히지 않아 사용함.
		/// </summary>
		/// <param name="txt"></param>
		public static void LimitToMaxLength(Page p, TextBox txt)
		{
			List<string> aStmt = new List<string>();
			string TxtCliendID = txt.ClientID;
			int MaxLength = txt.MaxLength;
			aStmt.Add("var " + TxtCliendID + " = document.getElementById(\"" + TxtCliendID + "\");");
			aStmt.Add("if (" + TxtCliendID + ") {");
			aStmt.Add("	" + TxtCliendID + ".onkeyup = function (e) {");
			aStmt.Add("		var e = window.event || e;");
			aStmt.Add("		var src = e.srcElement || e.target;");
			aStmt.Add("		var maxlength = " + MaxLength.ToString() + ";");
			aStmt.Add("		if (src.value.length > maxlength) {");
			aStmt.Add("			alert(\"허용길이인 \" + maxlength + \"자를 초과했습니다.\");");
			aStmt.Add("			src.value = src.value.substring(0, maxlength);");
			aStmt.Add("		}");
			aStmt.Add("	}");
			aStmt.Add("}");

			string Stmt = CScript.GetScript(aStmt);
			p.ClientScript.RegisterStartupScript(typeof(Page), "LimitToLength" + TxtCliendID, Stmt, true);
		}

	}
}
