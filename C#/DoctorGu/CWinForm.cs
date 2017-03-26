using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace DoctorGu
{
	/// <summary>
	/// 폼의 상태를 저장하고 불러올 때 어떤 값을 저장하고 불러올 지를 지정하기 위함.
	/// </summary>
	[Flags]
	public enum FormSaveRestoreKind
	{
		/// <summary>폼의 크기, 위치 정보</summary>
		SizePosition = 1,
		TextBox = 2,
		NumericUpDown = 4,
		ListControl = 8,
		CheckBox = 16,
		RadioButton = 32,
		AllControl = (TextBox | NumericUpDown | ListControl | CheckBox | RadioButton),
	}

	/// <summary>
	/// Form의 변경될 모양
	/// </summary>
	public enum FormShapeTypes
	{
		/// <summary>타원형</summary>
		Ellipse,
		/// <summary>모서리가 둥근 사각형</summary>
		RoundedRectangle
	}

	public enum FormLocationTypes
	{
		LeftTop,
		CenterMiddle,
		RightBottom
	}

	/// <summary>
	/// Form 클래스와 관련된 기능 구현
	/// </summary>
	public class CWinForm
	{
		/// <summary>
		/// Form의 위치와 크기 정보를 다음에 불러올 수 있도록 레지스트리에 저장함.
		/// </summary>
		/// <param name="AppProductName">Application.ProductName</param>
		/// <param name="FrmOrUc">Form 개체</param>
		/// <param name="Kind">저장될 값의 종류</param>
		/// <example>
		/// 다음은 폼이 닫힐 때 폼의 위치, 크기 정보를 저장하고,
		/// 폼이 열릴 때 저장되었던 폼의 위치, 크기 정보를 불러와서 열린 폼에 적용합니다.
		/// <code>
		/// private void Form1_Load(object sender, System.EventArgs e)
		/// {
		///	 CWinForm.RestoreFormStatus(Application.ProductName, this, FormSaveRestoreKind.SizePosition);
		/// }
		/// private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		/// {
		///	 CWinForm.SaveFormStatus(Application.ProductName, this, FormSaveRestoreKind.SizePosition);
		/// }
		/// </code>
		/// </example>
		public static void SaveFormStatus(string AppProductName, ContainerControl FrmOrUc, FormSaveRestoreKind Kind, CXmlConfig xc)
		{
			string FrmOrUcName = FrmOrUc.Name;
			string Section = AppProductName + "\\" + FrmOrUcName;

			if ((Kind & FormSaveRestoreKind.SizePosition) == FormSaveRestoreKind.SizePosition)
			{
				Form frm = (Form)FrmOrUc;

				if (xc != null)
				{
					xc.SaveSetting(FrmOrUcName + ".WindowState", Convert.ToInt32(frm.WindowState));
				}
				else
				{
					CRegistry.SaveSetting(Section, "WindowState", Convert.ToInt32(frm.WindowState));
				}

				if (frm.WindowState == FormWindowState.Normal)
				{
					if (xc != null)
					{
						xc.SaveSetting(FrmOrUcName + ".Left", frm.Left);
						xc.SaveSetting(FrmOrUcName + ".Top", frm.Top);
					}
					else
					{
						CRegistry.SaveSetting(Section, "Left", frm.Left);
						CRegistry.SaveSetting(Section, "Top", frm.Top);
					}

					switch (frm.FormBorderStyle)
					{
						case FormBorderStyle.Fixed3D:
						case FormBorderStyle.FixedDialog:
						case FormBorderStyle.FixedSingle:
						case FormBorderStyle.FixedToolWindow:
							break;
						default:
							if (xc != null)
							{
								xc.SaveSetting(FrmOrUcName + ".Width", frm.Width);
								xc.SaveSetting(FrmOrUcName + ".Height", frm.Height);
							}
							else
							{
								CRegistry.SaveSetting(Section, "Width", frm.Width);
								CRegistry.SaveSetting(Section, "Height", frm.Height);
							}
							break;
					}
				}
			}

			if (((Kind & FormSaveRestoreKind.TextBox) == FormSaveRestoreKind.TextBox)
				|| ((Kind & FormSaveRestoreKind.NumericUpDown) == FormSaveRestoreKind.NumericUpDown)
				|| ((Kind & FormSaveRestoreKind.ListControl) == FormSaveRestoreKind.ListControl)
				|| ((Kind & FormSaveRestoreKind.CheckBox) == FormSaveRestoreKind.CheckBox))
			{
				List<Control> aCtl = new List<Control>();
				aCtl = GetControls(FrmOrUc, ref aCtl);

				foreach (Control ctl in aCtl)
				{
					Type TypeCur = ctl.GetType();

					string ControlName = ctl.Name;

					if (((Kind & FormSaveRestoreKind.TextBox) == FormSaveRestoreKind.TextBox) && (TypeCur.BaseType == typeof(TextBoxBase)))
					{
						TextBoxBase txt = (TextBoxBase)ctl;

						if (xc != null)
						{
							xc.SaveSetting(FrmOrUcName + "." + ControlName, txt.Text);
						}
						else
						{
							CRegistry.SaveSetting(Section, ControlName, txt.Text);
						}
					}
					else if (((Kind & FormSaveRestoreKind.NumericUpDown) == FormSaveRestoreKind.NumericUpDown) && (TypeCur == typeof(NumericUpDown)))
					{
						NumericUpDown nud = (NumericUpDown)ctl;

						if (xc != null)
						{
							xc.SaveSetting(FrmOrUcName + "." + ControlName, nud.Value);
						}
						else
						{
							CRegistry.SaveSetting(Section, ControlName, nud.Value);
						}
					}
					else if (((Kind & FormSaveRestoreKind.ListControl) == FormSaveRestoreKind.ListControl) && (TypeCur.BaseType == typeof(ListControl)))
					{
						ListControl lst = (ListControl)ctl;
						SaveListControlSelectedIndex(AppProductName, FrmOrUc, lst, xc);
					}
					else if (((Kind & FormSaveRestoreKind.CheckBox) == FormSaveRestoreKind.CheckBox) && (TypeCur == typeof(CheckBox)))
					{
						CheckBox chk = (CheckBox)ctl;

						if (xc != null)
						{
							xc.SaveSetting(FrmOrUcName + "." + ControlName, CFindRep.IfTrueThen1FalseThen0(chk.Checked));
						}
						else
						{
							CRegistry.SaveSetting(Section, ControlName, CFindRep.IfTrueThen1FalseThen0(chk.Checked));
						}
					}
				}
			}
		}
		public static void SaveFormStatus(string AppProductName, ContainerControl FrmOrUc, FormSaveRestoreKind Kind)
		{
			SaveFormStatus(AppProductName, FrmOrUc, Kind, null);
		}
		public static void SaveListControlSelectedIndex(string AppProductName, ContainerControl FrmOrUc, ListControl lst, CXmlConfig xc)
		{
			string FrmOrUcName = FrmOrUc.Name;
			string Section = AppProductName + "\\" + FrmOrUcName;
			string ControlName = lst.Name;

			if (xc != null)
			{
				xc.SaveSetting(FrmOrUcName + "." + ControlName, lst.SelectedIndex);
			}
			else
			{
				CRegistry.SaveSetting(Section, ControlName, lst.SelectedIndex);
			}
		}
		public static void SaveListControlSelectedIndex(string AppProductName, ContainerControl FrmOrUc, ListControl lst)
		{
			SaveListControlSelectedIndex(AppProductName, FrmOrUc, lst, null);
		}


		/// <summary>
		/// 미리 저장된 Form의 위치와 크기 정보를 불러와서 현재 폼의 위치와 크기를 변경함.
		/// </summary>
		/// <param name="AppProductName">Application.ProductName</param>
		/// <param name="FrmOrUc">Form 개체</param>
		/// <param name="Kind">저장될 값의 종류</param>
		/// <example>
		/// 다음은 폼이 닫힐 때 폼의 위치, 크기 정보를 저장하고,
		/// 폼이 열릴 때 저장되었던 폼의 위치, 크기 정보를 불러와서 열린 폼에 적용합니다.
		/// <code>
		/// private void Form1_Load(object sender, System.EventArgs e)
		/// {
		///	 CWinForm.RestoreFormStatus(Application.ProductName, this, FormSaveRestoreKind.SizePosition);
		/// }
		/// private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		/// {
		///	 CWinForm.SaveFormStatus(Application.ProductName, this, FormSaveRestoreKind.SizePosition);
		/// }
		/// </code>
		/// </example>
		public static void RestoreFormStatus(string AppProductName, ContainerControl FrmOrUc, FormSaveRestoreKind Kind, CXmlConfig xc)
		{
			string FrmOrUcName = FrmOrUc.Name;
			string Section = AppProductName + "\\" + FrmOrUcName;

			if ((Kind & FormSaveRestoreKind.SizePosition) == FormSaveRestoreKind.SizePosition)
			{
				Form frm = (Form)FrmOrUc;

				int LeftWill, TopWill;

				int WindowStateDefault = (int)FormWindowState.Normal;
				object WindowState =
					(xc != null) ?
					xc.GetSetting(FrmOrUcName + ".WindowState", WindowStateDefault) :
					CRegistry.GetSetting(Section, "WindowState", WindowStateDefault);

				FormWindowState ws = FormWindowState.Normal;
				try { ws = (FormWindowState)Convert.ToInt32(WindowState); }
				catch (Exception) { }

				if (frm.WindowState != ws)
				{
					frm.WindowState = ws;
				}

				if (ws == FormWindowState.Normal)
				{
					int LeftDefault = frm.Left;
					object Left =
						(xc != null) ?
						xc.GetSetting(FrmOrUcName + ".Left", LeftDefault) :
						CRegistry.GetSetting(Section, "Left", LeftDefault);

					LeftWill = Convert.ToInt32(Left);
					if (LeftWill < 0) LeftWill = 0;

					int TopDefault = frm.Top;
					object Top =
						(xc != null) ?
						xc.GetSetting(FrmOrUcName + ".Top", TopDefault) :
						CRegistry.GetSetting(Section, "Top", TopDefault);

					TopWill = Convert.ToInt32(Top);
					if (TopWill < 0) TopWill = 0;

					if (frm.Left != LeftWill) frm.Left = LeftWill;
					if (frm.Top != TopWill) frm.Top = TopWill;

					switch (frm.FormBorderStyle)
					{
						case FormBorderStyle.Fixed3D:
						case FormBorderStyle.FixedDialog:
						case FormBorderStyle.FixedSingle:
						case FormBorderStyle.FixedToolWindow:
							break;
						default:
							int WidthDefault = frm.Width;
							object Width =
								(xc != null) ?
								xc.GetSetting(FrmOrUcName + ".Width", WidthDefault) :
								CRegistry.GetSetting(Section, "Width", WidthDefault);

							int WidthWill = Convert.ToInt32(Width);
							if (frm.Width != WidthWill) frm.Width = WidthWill;

							int HeightDefault = frm.Height;
							object Height =
								(xc != null) ?
								xc.GetSetting(FrmOrUcName + ".Height", HeightDefault) :
								CRegistry.GetSetting(Section, "Height", HeightDefault);

							int HeightWill = Convert.ToInt32(Height);
							if (frm.Height != HeightWill) frm.Height = HeightWill;

							break;
					}
				}
			}

			if (((Kind & FormSaveRestoreKind.TextBox) == FormSaveRestoreKind.TextBox)
				|| ((Kind & FormSaveRestoreKind.NumericUpDown) == FormSaveRestoreKind.NumericUpDown)
				|| ((Kind & FormSaveRestoreKind.ListControl) == FormSaveRestoreKind.ListControl)
				|| ((Kind & FormSaveRestoreKind.CheckBox) == FormSaveRestoreKind.CheckBox))
			{
				List<Control> aCtl = new List<Control>();
				aCtl = GetControls(FrmOrUc, ref aCtl);

				foreach (Control ctl in aCtl)
				{
					Type TypeCur = ctl.GetType();

					string ControlName = ctl.Name;

					if (((Kind & FormSaveRestoreKind.TextBox) == FormSaveRestoreKind.TextBox) && (TypeCur.BaseType == typeof(TextBoxBase)))
					{
						TextBoxBase txt = (TextBoxBase)ctl;

						string Value =
							(xc != null) ?
							xc.GetSetting(FrmOrUcName + "." + ControlName, txt.Text) :
							CRegistry.GetSetting(Section, ControlName, txt.Text).ToString();

						if (txt.Text != Value)
						{
							txt.Text = Value;
						}
					}
					else if (((Kind & FormSaveRestoreKind.NumericUpDown) == FormSaveRestoreKind.NumericUpDown) && (TypeCur == typeof(NumericUpDown)))
					{
						NumericUpDown nud = (NumericUpDown)ctl;

						string Value =
							(xc != null) ?
							xc.GetSetting(FrmOrUcName + "." + ControlName, nud.Value) :
							CRegistry.GetSetting(Section, ControlName, nud.Value).ToString();

						if (nud.Value.ToString() != Value)
						{
							nud.Value = (decimal)CFindRep.IfNotNumberThen0Decimal(Value);
						}
					}
					else if (((Kind & FormSaveRestoreKind.ListControl) == FormSaveRestoreKind.ListControl) && (TypeCur.BaseType == typeof(ListControl)))
					{
						ListControl lst = (ListControl)ctl;
						RestoreListControlSelectedIndex(Application.ProductName, FrmOrUc, lst, xc);
					}
					else if (((Kind & FormSaveRestoreKind.CheckBox) == FormSaveRestoreKind.CheckBox) && (TypeCur == typeof(CheckBox)))
					{
						CheckBox chk = (CheckBox)ctl;

						bool Checked =
							(xc != null) ?
							(xc.GetSetting(FrmOrUcName + "." + ControlName, CFindRep.IfTrueThen1FalseThen0(chk.Checked)) == "1") :
							(CRegistry.GetSetting(Section, ControlName, CFindRep.IfTrueThen1FalseThen0(chk.Checked)).ToString() == "1");

						if (chk.Checked != Checked)
						{
							chk.Checked = Checked;
						}
					}
					else if (((Kind & FormSaveRestoreKind.RadioButton) == FormSaveRestoreKind.RadioButton) && (TypeCur == typeof(RadioButton)))
					{
						RadioButton rad = (RadioButton)ctl;

						bool Checked =
							(xc != null) ?
							(xc.GetSetting(FrmOrUcName + "." + ControlName, CFindRep.IfTrueThen1FalseThen0(rad.Checked)) == "1") :
							(CRegistry.GetSetting(Section, ControlName, CFindRep.IfTrueThen1FalseThen0(rad.Checked)).ToString() == "1");

						if (rad.Checked != Checked)
						{
							rad.Checked = Checked;
						}
					}
				}
			}
		}
		public static void RestoreFormStatus(string AppProductName, ContainerControl FrmOrUc, FormSaveRestoreKind Kind)
		{
			RestoreFormStatus(AppProductName, FrmOrUc, Kind, null);
		}
		public static void RestoreListControlSelectedIndex(string AppProductName, ContainerControl FrmOrUc, ListControl lst, CXmlConfig xc)
		{
			string FrmOrUcName = FrmOrUc.Name;
			string Section = AppProductName + "\\" + FrmOrUcName;
			string ControlName = lst.Name;

			int SelectedIndex =
				(xc != null) ?
				CFindRep.IfNotNumberThen0(xc.GetSetting(FrmOrUcName + "." + ControlName, lst.SelectedIndex)) :
				CFindRep.IfNotNumberThen0(CRegistry.GetSetting(Section, ControlName, lst.SelectedIndex));

			if (lst.SelectedIndex != SelectedIndex)
			{
				//Items.Count 읽을 수 없어 try 사용.
				try { lst.SelectedIndex = SelectedIndex; }
				catch (Exception) { }
			}
		}
		public static void RestoreListControlSelectedIndex(string AppProductName, ContainerControl FrmOrUc, ListControl lst)
		{
			RestoreListControlSelectedIndex(AppProductName, FrmOrUc, lst, null);
		}

		private static List<Control> GetControls(Control Parent, ref List<Control> aCtls)
		{
			foreach (Control ctl in Parent.Controls)
			{
				if (ctl.GetType() == typeof(UserControl))
					continue;

					
				aCtls.Add(ctl);

				if (ctl.HasChildren)
				{
					aCtls = GetControls(ctl, ref aCtls);
				}
			}

			return aCtls;
		}

		/// <summary>
		/// Form의 모든 컨트롤 중 Name 속성이 <paramref name="Name"/> 값을 가진 컨트롤을 리턴함.
		/// </summary>
		/// <param name="f">Form</param>
		/// <param name="Name">컨트롤 이름</param>
		/// <returns>찾아진 컨트롤 개체</returns>
		/// <example>
		/// <code>
		/// this.Text = CWinForm.GetControlByName(this, "label1").Text;
		/// </code>
		/// </example>
		public static Control GetControlByName(ContainerControl f, string Name)
		{
			Control ctl = null;

			for (int i = 0, i2 = f.Controls.Count; i < i2; i++)
			{
				string NameCur = f.Controls[i].Name;
				if (NameCur == Name)
				{
					ctl = f.Controls[i];
					break;
				}
			}

			return ctl;
		}

		/// <summary>
		/// "Happy Manager - [홈페이지 메인/고정글]" 에서 "Happy Manager"와 "홈페이지 메인/고정글"을 각각 리턴함.
		/// </summary>
		/// <param name="Caption">MdiParent인 폼의 전체 제목 문자열</param>
		/// <param name="CaptionParent">전체 제목 문자열에서 MdiParent인 폼의 문자열</param>
		/// <param name="CaptionChild">전체 제목 문자열에서 MdiParent의 하위 폼의 문자열</param>
		/// <example>
		/// <code>
		/// string CaptionParent, CaptionChild;
		/// CWinForm.GetMdiParentChildCaption(this.Text, out CaptionParent, out CaptionChild);
		/// this.Text = CaptionParent + " - [" + CaptionChild + "]";
		/// </code>
		/// </example>
		public static void GetMdiParentChildCaption(string Caption,
			out string CaptionParent, out string CaptionChild)
		{
			CaptionParent = "";
			CaptionChild = "";

			int Pos = Caption.IndexOf(" - [");
			if (Pos == -1)
			{
				//Child가 최대화되지 않았거나 Child가 없는 상태
				CaptionParent = Caption;
				return;
			}

			if (!Caption.EndsWith("]"))
			{
				CaptionParent = Caption;
				return;
			}

			CaptionParent = Caption.Substring(0, Pos);

			Caption = Caption.Substring(Pos + 4);
			CaptionChild = Caption.Substring(0, Caption.Length - 1);
		}

		/// <summary>
		/// 폼의 모양을 직사각형이 아닌 다른 모양으로 변경함.
		/// </summary>
		/// <example>
		/// 다음은 현재 폼의 모서리를 둥글게 변경합니다.
		/// <code>
		/// private CWinForm.FormShape mFormShape = null;
		/// 
		/// private void Form1_Load(object sender, System.EventArgs e)
		/// {
		///	 this.mFormShape = new CWinForm.FormShape(this, FormShapeTypes.RoundedRectangle);
		/// }
		/// </code>
		/// </example>
		public class FormShape
		{
			private Form f;
			private FormShapeTypes fst;
			private Color BackColor, ColorToTrans;
			private int Width, Height;

			/// <summary>
			/// 폼의 모양을 지정.
			/// </summary>
			/// <param name="f">Form 개체</param>
			/// <param name="fst">Forrm의 변경될 모양</param>
			public FormShape(Form f, FormShapeTypes fst)
			{
				if (f.FormBorderStyle != FormBorderStyle.None)
				{
					f.FormBorderStyle = FormBorderStyle.None;
				}

				BackColor = f.BackColor;
				Width = f.ClientSize.Width;
				Height = f.ClientSize.Height;

				//폼의 배경색이 숨겨질 색과 같은 빨간색이라면 숨겨질 색을 노란색으로
				//설정해서 폼의 배경색을 보이게 함.
				ColorToTrans = (BackColor == Color.Red) ? Color.Yellow : Color.Red;
				f.TransparencyKey = ColorToTrans;

				f.Paint += new System.Windows.Forms.PaintEventHandler(this.FormShape_Paint);
				f.SizeChanged += new System.EventHandler(this.FormShape_SizeChanged);

				this.fst = fst;
				this.f = f;
			}

			private void FormShape_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
			{
				ChangeFormShape(this.fst, e);
			}

			private void FormShape_SizeChanged(object sender, System.EventArgs e)
			{
				this.Width = f.ClientSize.Width;
				this.Height = f.ClientSize.Height;
			}

			private void ChangeFormShape(FormShapeTypes FormShapeType, PaintEventArgs e)
			{

				//전체를 투명하게 될 색으로 먼저 설정.
				e.Graphics.FillRectangle(new SolidBrush(this.ColorToTrans), 0, 0, this.Width, this.Height);

				//다음에 원래 폼의 배경색을 채움.
				switch (FormShapeType)
				{
					case FormShapeTypes.Ellipse:
						e.Graphics.FillEllipse(new SolidBrush(this.BackColor), 0, 0, this.Width, this.Height);

						break;
					case FormShapeTypes.RoundedRectangle:
						GraphicsPath gp = DrawRoundedRectangle(20);

						Region rgn = new Region(gp);
						e.Graphics.FillRegion(new SolidBrush(this.BackColor), rgn);

						break;
				}
			}

			private GraphicsPath DrawRoundedRectangle(int RoundWidth)
			{
				GraphicsPath gp = new GraphicsPath();

				gp.AddArc(0, 0, RoundWidth, RoundWidth, 180, 90);
				gp.AddLine(RoundWidth, 0, this.Width - RoundWidth, 0);

				gp.AddArc(this.Width - RoundWidth, 0, RoundWidth, RoundWidth, -90, 90);
				gp.AddLine(this.Width, RoundWidth, this.Width, this.Height - RoundWidth);

				gp.AddArc(this.Width - RoundWidth, this.Height - RoundWidth, RoundWidth, RoundWidth, 360, 90);
				gp.AddLine(this.Width - RoundWidth, this.Height, RoundWidth, this.Height);

				gp.AddArc(0, this.Height - RoundWidth, RoundWidth, RoundWidth, 90, 90);
				gp.AddLine(0, this.Height - RoundWidth, 0, RoundWidth);

				return gp;
			}

			public static bool IsFormLoaded(FormCollection Application_OpenForms, Form Frm)
			{
				foreach (Form frmCur in Application_OpenForms)
				{
					if (frmCur.Name == Frm.Name)
						return true;
				}

				return false;
			}

			/// <summary>
			/// 바탕화면의 시작 메뉴, 고정 툴바 등을 감안한 폼의 위치를 지정함.
			/// </summary>
			/// <param name="Frm"></param>
			/// <param name="LocType"></param>
			public static void SetLocation(Form Frm, FormLocationTypes LocType)
			{
				switch (LocType)
				{
					case FormLocationTypes.LeftTop:
						Frm.Left = Screen.PrimaryScreen.WorkingArea.Left;
						Frm.Top = Screen.PrimaryScreen.WorkingArea.Top;

						break;

					case FormLocationTypes.CenterMiddle:
						Frm.Left = (Screen.PrimaryScreen.WorkingArea.Width - Frm.Width) / 2;
						Frm.Top = (Screen.PrimaryScreen.WorkingArea.Height - Frm.Height) / 2;

						break;

					case FormLocationTypes.RightBottom:
						Frm.Left = Screen.PrimaryScreen.WorkingArea.Width - Frm.Width;
						Frm.Top = Screen.PrimaryScreen.WorkingArea.Height - Frm.Height;

						break;
					default:
						break;
				}
			}
		}
	}
}