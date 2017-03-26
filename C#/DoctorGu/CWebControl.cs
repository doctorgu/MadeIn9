using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Web;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;

namespace DoctorGu
{
	public class CWebControl
	{
		public static Control[] GetControls(Control Parent, bool NoMoreChildIfUserControl)
		{
			List<Control> aCtl = new List<Control>();
			_GetControls(Parent, NoMoreChildIfUserControl, aCtl);
			return aCtl.ToArray();
		}
		public static Control[] GetControls(Control Parent)
		{
			bool NoMoreChildIfUserControl = false;
			return GetControls(Parent, NoMoreChildIfUserControl);
		}
		private static void _GetControls(Control Parent, bool NoMoreChildIfUserControl, List<Control> aCtl)
		{
			foreach (Control ctl in Parent.Controls)
			{
				aCtl.Add(ctl);

				if ((ctl is UserControl) && NoMoreChildIfUserControl)
					continue;

				if (ctl.HasControls())
				{
					_GetControls(ctl, NoMoreChildIfUserControl, aCtl);
				}
			}
		}

		public static Control FindControlDeep(Control Parent, string Id)
		{
			foreach (Control ctl in Parent.Controls)
			{
				if (!string.IsNullOrEmpty(ctl.ID))
				{
					if (ctl.ID == Id)
					{
						return ctl;
					}
				}

				if (ctl.HasControls())
				{
					Control ctlFound = FindControlDeep(ctl, Id);
					//찾지 못했는데 리턴하면 다음 ctl에 대한 검사를 못하므로 찾았을 때만 리턴함.
					if (ctlFound != null)
						return ctlFound;
				}
			}

			return null;
		}

		public static void SetControlValueByType(Control Ctl, string Value)
		{
			TextBox txt = Ctl as TextBox;
			if (txt != null)
			{
				txt.Text = Value;
				return;
			}

			ListControl lsc = Ctl as ListControl;
			if (lsc != null)
			{
				CWebListControl.SelectByValue(lsc, Value);
				return;
			}
		}
		public static void SetControlToolTipByType(Control Ctl, string ToolTip)
		{
			TextBox txt = Ctl as TextBox;
			if (txt != null)
			{
				txt.ToolTip = ToolTip;
				return;
			}

			ListControl lsc = Ctl as ListControl;
			if (lsc != null)
			{
				lsc.ToolTip = ToolTip;
				return;
			}
		}
		public static void SetControlEnabledByType(Control ctl, bool Enabled)
		{
			TextBox txt = ctl as TextBox;
			Label lbl = ctl as Label;
			Button btn = ctl as Button;
			Calendar cal = ctl as Calendar;

			if (txt != null)
				txt.Enabled = Enabled;
			if (lbl != null)
				lbl.Enabled = Enabled;
			if (btn != null)
				btn.Enabled = Enabled;
			if (cal != null)
				cal.Enabled = Enabled;
		}

		public static string GetControlValueByType(Control Ctl)
		{
			string Value = "";

			TextBox txt = Ctl as TextBox;
			if (txt != null)
			{
				Value = ((TextBox)Ctl).Text;
			}

			ListControl lsc = Ctl as ListControl;
			if (lsc != null)
			{
				Value = ((ListControl)Ctl).SelectedValue;
			}

			return Value;
		}
		public static string GetControlToolTipByType(Control Ctl)
		{
			string Value = "";

			TextBox txt = Ctl as TextBox;
			if (txt != null)
			{
				Value = ((TextBox)Ctl).ToolTip;
			}

			ListControl lsc = Ctl as ListControl;
			if (lsc != null)
			{
				Value = ((ListControl)Ctl).ToolTip;
			}

			return Value;
		}


		public static Size GetPostedImageSize(HttpPostedFile pfile)
		{
			Size sz = new Size();
			if (IsPostedFileIsImage(pfile))
			{
				using (Bitmap bm = new Bitmap(pfile.InputStream))
				{
					sz.Width = bm.Width;
					sz.Height = bm.Height;
				}
			}

			return sz;
		}
		public static bool IsPostedFileIsImage(HttpPostedFile pfile)
		{
			return (pfile != null) && (pfile.ContentLength > 0)
				&& (Regex.IsMatch(pfile.ContentType, "image/\\S+") || CFile.IsImageFileAvailableInBrowser(pfile.FileName));
		}

		public static string GetFormControlValue(Page p, int MaxValue, string DelimCol, string DelimRow)
		{
			StringBuilder sb = new StringBuilder();

			Control[] aCtl = CWebControl.GetControls(p);
			foreach (Control ctl in aCtl)
			{
				string Path = GetPath(ctl, ".");
				string ID = ctl.ID;
				string TypeName = ctl.GetType().Name;

				string PropertyName = "";

				//Literal은 내용이 너무 많으므로 제외함.

				if ((ctl is TextBox) || (ctl is Label))
				{
					PropertyName = "Text";
				}
				else if (ctl is ListControl)
				{
					PropertyName = "SelectedValue";
				}
				else if (ctl is CheckBox)
				{
					PropertyName = "Checked";
				}
				else if (ctl is UserControl)
				{
					//uc_inputip_ascx와 같이 구성됨.
					//보류
					string Name = ctl.GetType().Name;
					if (Name.StartsWith("uc_") && Name.EndsWith("_ascx"))
					{
						string UscName = Name.Split('_')[1];
					}
				}

				if ((ID == null) || (PropertyName == ""))
					continue;

				string Value = CReflection.GetFieldOrPropertyValue(ctl, PropertyName).ToString();
				if (Value.Length > MaxValue)
					Value = Value.Substring(0, MaxValue);

				sb.Append(Path + "." + ID + "." + PropertyName + DelimCol + Value.ToString() + DelimRow);
			}

			return sb.ToString();
		}
		private static string GetPath(Control ctl, string Delim)
		{
			List<string> aPath = new List<string>();
			Control ctlParent = ctl.Parent;
			while (ctlParent != null)
			{
				string ParentID = ctlParent.ID;
				if (ParentID == null)
					break;

				aPath.Add(ParentID);
				ctlParent = ctlParent.Parent;
			}

			aPath.Reverse();
			return string.Join(Delim, aPath.ToArray());
		}

		public static void SetMetaContent(HtmlHead header, string HttpEquiv, string Content)
		{
			//<meta http-equiv="X-UA-Compatible" content="edge" />
			//<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />

			HtmlMeta meta = GetMeta(header, HttpEquiv);
			if (meta != null)
			{
				meta.Content = Content;
			}
			else
			{
				meta = new HtmlMeta() { HttpEquiv = HttpEquiv, Content = Content };

				//X-UA-Compatible의 경우 마지막에 추가하면 적용되지 않아 가장 처음에 추가함.
				header.Controls.AddAt(0, meta);
				//header.Controls.Add(meta);
			}
		}
		public static HtmlMeta GetMeta(HtmlHead header, string HttpEquiv)
		{
			for (int i = 0; i < header.Controls.Count; i++)
			{
				HtmlMeta meta = header.Controls[i] as HtmlMeta;
				if (meta == null)
					continue;

				if (meta.HttpEquiv != HttpEquiv)
					continue;

				return meta;
			}

			return null;
		}
		public static string GetMetaContent(HtmlHead header, string HttpEquiv)
		{
			HtmlMeta meta = GetMeta(header, HttpEquiv);
			if (meta == null)
				return null;
			else
				return meta.Content;
		}

		/// <summary>
		/// ASP.Net에선 ReadOnly가 true이면 PostBack시에 값을 읽지 않으므로 강제로 값을 설정함.
		/// Master Page의 Page_Load 이벤트에서 호출할 것
		/// </summary>
		/// <param name="p"></param>
		public static void SetReadOnlyTextBoxValueWhenPostBack(Page p)
		{
			if (!p.IsPostBack)
				return;

			Control[] aCtl = CWebControl.GetControls(p);
			foreach (Control Ctl in aCtl)
			{
				TextBox txt = Ctl as TextBox;
				if (txt == null)
					continue;

				if (txt.ReadOnly)
					txt.Text = p.Request.Form[txt.UniqueID];
			}
		}
	}
}
