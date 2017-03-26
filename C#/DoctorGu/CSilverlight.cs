using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//initParams의 value가 Empty이면 Chrome, FF에서 표시 안됨.
//height=100%이면 FF에서 안 보임.(높이가 0이 되는 것 같음)

namespace DoctorGu
{
	public struct SInfoSilverlightProperties
	{
		public int width, height;
		public string background;
		public string alt;
		public string minRuntimeVersion;
	}

	public struct SInfoSilverlightEvents
	{
		public string onError;
		public string onFullScreenChanged;
		public string onLoaded;
		public string onResized;
		public string onSourceDownloadComplete;
		public string onSourceDownloadProgressChanged;
	}

	public class CSilverlight
	{
		public static string GetTag(string Source, string Id,
			string width, string height, string background, string minRuntimeVersion,
			string initParams)
		{
			string Template =
@"
<div id=""silverlightControlHost"">
	<object id=""{{Id}}"" data=""data:application/x-silverlight-2,"" type=""application/x-silverlight-2"" width=""{{width}}"" height=""{{height}}"">
		<param name=""source"" value=""{{Source}}""/>
		<param name=""background"" value=""{{background}}"" />
		<param name=""minRuntimeVersion"" value=""{{minRuntimeVersion}}"" />
		<param name=""initParams"" value=""{{initParams}}"" />
		<param name=""autoUpgrade"" value=""true"" />
		<a href=""http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0"" style=""text-decoration:none"">
			<img src=""http://go.microsoft.com/fwlink/?LinkId=161376"" alt=""Get Microsoft Silverlight"" style=""border-style:none""/>
		</a>
	</object>
	<iframe id=""_sl_historyFrame"" style=""visibility:hidden;height:0px;width:0px;border:0px""></iframe>
</div>
";
			return Template
				.Replace("{{Source}}", Source)
				.Replace("{{Id}}", Id)
				.Replace("{{width}}", width)
				.Replace("{{height}}", height)
				.Replace("{{background}}", background)
				.Replace("{{minRuntimeVersion}}", minRuntimeVersion)
				.Replace("{{initParams}}", initParams);
		}

		public static string GetScript(string Source, string Id,
			SInfoSilverlightProperties Properties, SInfoSilverlightEvents Events, 
			Dictionary<string, string> InitParams)
		{
/*
<div id="silverlightControlHost">
	<script type="text/javascript">
	var source = "/ClientBin/RoundedBox.xap";
	var parentElement = silverlightControlHost;
	var id = "slPlugin";
	var properties = {
		width: "100", height: "100",
		background: "white", alt: alt,
		minRuntimeVersion: "2.0.30800.0"
	};
	var events = { onError: onSLError, onLoad: onSLLoad };
	var initParams = "param1=value1,param2=value2";
	
	Silverlight.createObject(source, parentElement, id, properties, events, initParams, "");
</script>
</div>
*/
			string DivId = "div" + Id;

			List<string> aStmt = new List<string>();
			aStmt.Add("var source = \"" + Source + "\";");
			aStmt.Add("var parentElement = " + DivId + ";");
			aStmt.Add("var id = \"" + Id + "\";");

			aStmt.Add("var properties = {");
			Dictionary<string, string> dicProperties = new Dictionary<string, string>();
			if (Properties.width != 0)
				dicProperties.Add("width", Properties.width.ToString());
			if (Properties.height != 0)
				dicProperties.Add("height", Properties.height.ToString());
			if (!string.IsNullOrEmpty(Properties.background))
				dicProperties.Add("background", Properties.background);
			if (!string.IsNullOrEmpty(Properties.alt))
				dicProperties.Add("alt", Properties.alt);
			if (!string.IsNullOrEmpty(Properties.minRuntimeVersion))
				dicProperties.Add("minRuntimeVersion", Properties.minRuntimeVersion);
			aStmt.Add(CScript.GetScriptKeyValueByColon(dicProperties, true));
			aStmt.Add("};");

			aStmt.Add("var events = {");
			Dictionary<string, string> dicEvents = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(Events.onError))
				dicEvents.Add("onError", Events.onError);
			if (!string.IsNullOrEmpty(Events.onFullScreenChanged))
				dicEvents.Add("onFullScreenChanged", Events.onFullScreenChanged);
			if (!string.IsNullOrEmpty(Events.onLoaded)) 
				dicEvents.Add("onLoaded", Events.onLoaded);
			if (!string.IsNullOrEmpty(Events.onResized)) 
				dicEvents.Add("onResized", Events.onResized);
			if (!string.IsNullOrEmpty(Events.onSourceDownloadComplete)) 
				dicEvents.Add("onSourceDownloadComplete", Events.onSourceDownloadComplete);
			if (!string.IsNullOrEmpty(Events.onSourceDownloadProgressChanged)) 
				dicEvents.Add("onSourceDownloadProgressChanged", Events.onSourceDownloadProgressChanged);
			aStmt.Add(CScript.GetScriptKeyValueByColon(dicEvents, true));
			aStmt.Add("};");

			aStmt.Add("var initParams = \"" + GetInitParams(InitParams) + "\";");

			aStmt.Add("Silverlight.createObject(source, parentElement, id, properties, events, initParams, \"\");");
			string Script = CScript.GetScript(aStmt, true);
			string s = "<div id=\"" + DivId + "\">" + Script + "</div>\r\n";

			return s;
		}

		private static string GetInitParams(Dictionary<string, string> KeyValue)
		{
			string s = "";
			foreach (KeyValuePair<string, string> kv in KeyValue)
			{
				s += "," + kv.Key + "=" + kv.Value;
			}
			if (s != "")
				s = s.Substring(1);

			return s;
		}

	}
}
