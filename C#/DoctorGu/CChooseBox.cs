using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace DoctorGu
{
	/// <summary>
	/// 여러 개 중 하나의 값을 선택할 수 있는 대화상자를 표시함.
	/// </summary>
	/// <example>
	/// 다음은 감자, 고구마, 당근 중 하나를 선택하는 대화상자를 표시하고,
	/// 기본적으로 고구마가 선택되게 합니다.
	/// <code>
	/// string Prompt = "다음 중 하나를 선택하세요.";
	/// string[] aChoice = new string[] { "감자", "고구마", "당근" };
	///
	/// int IndexChosen = CChooseBox.Show(Prompt, "선택", aChoice, 1);
	/// if (IndexChosen == -1)
	/// {
	///	 Console.WriteLine("취소되었습니다.");
	/// }
	/// else
	/// {
	///	 Console.WriteLine(aChoice[IndexChosen] + "를 선택했습니다.");
	/// }
	/// </code>
	/// </example>
	public partial class CChooseBox : Form
	{
		/// <summary>
		/// </summary>
		public CChooseBox()
		{
			InitializeComponent();
		}

		private string[] mChoice = null;
		private int mIndex = -1;
		private int mDefaultIndex = -1;
		private string mPrompt = "";
		private string mTitle = "";
		private bool mUseButton = false;

		private void CChooseBox_Load(object sender, EventArgs e)
		{
			lblPrompt.Text = mPrompt;
			this.Text = mTitle;

			ShowChoice();
		}
		private void CChooseBox_Activated(object sender, EventArgs e)
		{
			if (mUseButton)
			{
				if (mDefaultIndex != -1)
				{
					Button btn = (Button)flw.Controls["btnChoice_" + mDefaultIndex.ToString()];
					btn.Focus();
				}
			}
		}

		private void btnChoice_Click(object sender, EventArgs e)
		{
			if (!mUseButton)
				return;

			Button btn = (Button)sender;
			string[] aName = btn.Name.Split('_');
			int Index = Convert.ToInt32(aName[1]);
			this.mIndex = Index;

			this.Close();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			int IndexChecked = -1;
			for (int i = 0, i2 = this.mChoice.Length; i <i2; i++)
			{
				RadioButton rad = (RadioButton)flw.Controls["radChoice_" + i.ToString()];
				if (rad.Checked)
				{
					IndexChecked = i;
					break;
				}
			}
			this.mIndex = IndexChecked;

			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.mIndex = -1;
			this.Close();
		}

		/// <summary>
		/// 사용자가 선택할 수 있는 대화상자를 Modal 형식으로 표시함.
		/// </summary>
		/// <param name="Prompt">표시 문자열</param>
		/// <param name="Title">대화상자 제목</param>
		/// <param name="aChoice">표시되는 각 Radio 컨트롤의 Text 값</param>
		/// <param name="DefaultIndex">대화상자가 표시됐을 때 기본적으로 선택될 Index(0 base)</param>
		/// <param name="UseButton">옵션 단추를 버튼 형식으로 표시하고 [확인], [취소] 단추를 숨기는 지 여부</param>
		/// <returns>선택된 Radio 컨트롤의 Index(0 base)</returns>
		public static int Show(IWin32Window Owner, string Prompt, string Title, string[] aChoice, int DefaultIndex, bool UseButton)
		{
			int Index = -1;
			using (CChooseBox f = new CChooseBox())
			{
				f.Prompt = Prompt;
				f.Title = Title;
				f.Choice = aChoice;
				f.DefaultIndex = DefaultIndex;
				f.UseButton = UseButton;

				f.ShowDialog(Owner);
				Index = f.Index;
			}

			return Index;
		}
		public static int Show(IWin32Window Owner, string Prompt, string Title, string[] aChoice, int DefaultIndex)
		{
			return Show(Owner, Prompt, Title, aChoice, DefaultIndex, false);
		}
		
		private void ShowChoice()
		{
			int LeftFirst = lblPrompt.Left;
			int TopFirst = lblPrompt.Top + lblPrompt.Height + 30;

			btnOK.Visible = !mUseButton;
			btnCancel.Visible = !mUseButton;

			if (mUseButton)
			{
				for (int i = 0, i2 = this.mChoice.Length; i < i2; i++)
				{
					Button btnChoice = new Button();
					btnChoice.AutoSize = true;
					btnChoice.Name = "btnChoice_" + i.ToString();
					btnChoice.TabIndex = i;
					btnChoice.TabStop = true;
					btnChoice.Text = this.mChoice[i];
					btnChoice.Click += new EventHandler(btnChoice_Click);

					flw.Controls.Add(btnChoice);
				}

				this.Height = (this.Height - this.ClientSize.Height) + flw.Top + flw.Height + 15;
			}
			else
			{
				for (int i = 0, i2 = this.mChoice.Length; i < i2; i++)
				{
					RadioButton radChoice = new RadioButton();
					radChoice.AutoSize = true;
					radChoice.Name = "radChoice_" + i.ToString();
					radChoice.TabIndex = i;
					radChoice.TabStop = true;
					radChoice.Text = this.mChoice[i];
					radChoice.UseVisualStyleBackColor = true;
					radChoice.Checked = false;

					flw.Controls.Add(radChoice);
				}

				if (mDefaultIndex != -1)
				{
					RadioButton rad = (RadioButton)flw.Controls["radChoice_" + this.mDefaultIndex.ToString()];
					rad.Checked = true;
				}

				this.Height =  (this.Height - this.ClientSize.Height) + btnOK.Top + btnOK.Height + 15;
			}
		}

		/// <summary>
		/// 표시 문자열
		/// </summary>
		public string Prompt
		{
			set { this.mPrompt = value; }
		}

		/// <summary>
		/// 대화상자 제목
		/// </summary>
		public string Title
		{
			set { this.mTitle = value; }
		}

		/// <summary>
		/// 대화상자가 표시됐을 때 기본적으로 선택될 Index(0 base)
		/// </summary>
		public int DefaultIndex
		{
			set { this.mDefaultIndex = value; }
		}

		/// <summary>
		/// 옵션 단추를 버튼 형식으로 표시하고 [확인], [취소] 단추를 숨기는 지 여부
		/// </summary>
		public bool UseButton
		{
			set { this.mUseButton = value; }
		}

		/// <summary>
		/// 선택된 Radio 컨트롤의 Index(0 base)
		/// </summary>
		public int Index
		{
			get { return mIndex; }
		}

		/// <summary>
		/// 표시되는 각 Radio 컨트롤의 Text 값
		/// </summary>
		public string[] Choice
		{
			set { this.mChoice = value; }
		}

	}
}