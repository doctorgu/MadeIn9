using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoctorGu
{
	public partial class CInputBox : Form
	{
		public CInputBox()
		{
			InitializeComponent();
		}

		private string _Input = "";

		private void lblMsg_SizeChanged(object sender, System.EventArgs e)
		{
			const int CMargin = 20;

			int Bottom = lblMsg.Top + lblMsg.Height;
			if (Bottom > txtInput.Top)
			{
				txtInput.Top = Bottom + CMargin;
			}

			if (txtInput.Top > this.Height)
			{
				this.Height = txtInput.Top + CMargin;
			}
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this._Input = txtInput.Text;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		//=============================================================
		public static string Show(string Prompt, string Title, string Default, bool IsPassword)
		{
			string Input;
			using (CInputBox f = new CInputBox())
			{
				f.lblMsg.Text = Prompt;
				f.Text = Title;
				f.txtInput.Text = Default;

				//비밀번호를 입력받는다면 입력 문자열이 표시되지 않게 함.
				if (IsPassword) f.txtInput.PasswordChar = '*';

				f.ShowDialog();
				Input = f.Input;
			}

			return Input;
		}
		public static string Show(string Prompt, string Title, string Default)
		{
			return Show(Prompt, Title, Default, false);
		}
		public static string Show(string Prompt, string Title)
		{
			return Show(Prompt, Title, "", false);
		}
		public static string Show(string Prompt)
		{
			return Show(Prompt, "입력", "", false);
		}

		public string Input
		{
			get { return _Input; }
		}
	}
}
