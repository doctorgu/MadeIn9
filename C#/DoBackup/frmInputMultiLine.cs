using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoBackup
{
	public partial class frmInputMultiLine : Form
	{
		private string[] maText;

		public frmInputMultiLine()
		{
			InitializeComponent();
		}

		private void frmInputMultiLine_Load(object sender, EventArgs e)
		{
			txtValue.Text = this.TextReturn;
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.TextReturn = txtValue.Text;
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		public string TextSemicolon
		{
			get { return string.Join(";", this.maText); }
			set { this.maText = value.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries); }
		}

		public string TextReturn
		{
			get { return string.Join("\r\n", this.maText); }
			set { this.maText = value.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries); }
		}

	}
}
