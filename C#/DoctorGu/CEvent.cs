using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace DoctorGu
{
	//인수 없는 함수용
	public delegate void DelegateVoid();
	public delegate bool DelegateBool();
	public delegate int DelegateInt();
	public delegate KeyValuePair<bool, string> DelegateBoolString();
	public delegate object DelegateObject();

	public class CProgressChangedEventArgs : EventArgs
	{
		public int ProgressPercentage;
	}

	public enum ControlShortcutKeys
	{
		ControlA,
	}

	/// <summary>
	/// 기존의 Click, SelectedIndexChanged 등의 이벤트를 생성해서 일일이 코딩했던 번거로운 작업들을
	/// 폼이 로드될 때 한번만 함수를 호출하면 자동으로 해당 이벤트들을 생성시켜서 처리할 수 있게 함.
	/// </summary>
	public class CEvent
	{
		private TabControl[] maTabControlSelectChildControlWhenIndexChanged = null;
		private TextBox[] maTxtEnterThenClick = null;
		private Button[] maBtnEnterThenClick = null;

		/// <summary>
		/// TabControl에서 탭을 선택했을 때 선택된 탭 안의 컨트롤이 TextBox와 같이 포커스를 가질 수 있는 컨트롤이면
		/// 포커스를 가질 수 있도록 함.
		/// </summary>
		/// <example>
		/// 다음은 tabText 컨트롤에서 탭을 선택할 때 해당 탭 안의 컨트롤이 포커스를 가지게 합니다.
		/// <code>
		/// private CEvent mEvent = new CEvent();
		/// private void frmMerchantManage_Load(object sender, EventArgs e)
		/// {
		///	 this.mEvent.TabControlSelectChildControlWhenIndexChanged = new TabControl[] { tabText };
		/// }
		/// </code>
		/// </example>
		public TabControl[] TabControlSelectChildControlWhenIndexChanged
		{
			set
			{
				this.maTabControlSelectChildControlWhenIndexChanged = value;

				for (int i = 0, i2 = this.maTabControlSelectChildControlWhenIndexChanged.Length; i < i2; i++)
				{
					this.maTabControlSelectChildControlWhenIndexChanged[i].SelectedIndexChanged
						+= new System.EventHandler(this.TabControlSelectChildControlWhenIndexChanged_SelectedIndexChanged);

					this.maTabControlSelectChildControlWhenIndexChanged[i].Enter
						+= new System.EventHandler(this.TabControlSelectChildControlWhenIndexChanged_Enter);
				}
			}
		}

		private void TabControlSelectChildControlWhenIndexChanged_SelectedIndexChanged(object sender, EventArgs e)
		{
			SelectChildControlOfTabPage(sender);
		}
		private void TabControlSelectChildControlWhenIndexChanged_Enter(object sender, EventArgs e)
		{
			SelectChildControlOfTabPage(sender);
		}
		private void SelectChildControlOfTabPage(object sender)
		{
			TabControl tab = (TabControl)sender;
			TabPage p = tab.TabPages[tab.SelectedIndex];
			Control ctl = p.GetChildAtPoint(new Point(p.Width / 2, p.Height / 2));
			if ((ctl != null) && (ctl.Enabled))
			{
				ctl.Focus();
			}
		}

		/// <summary>
		/// TextBox 컨트롤에서 엔터키를 눌렀을 때 특정 Button의 Click 이벤트를 발생시킴.
		/// </summary>
		/// <example>
		/// 다음은 찾는 값이 있는 txtValue 컨트롤에서 엔터키를 눌렀을 때 [찾기] 단추를 btnFind를 Click하게 합니다.
		/// <code>
		/// private CEvent mEvent = new CEvent();
		/// private void FindMember_Load(object sender, EventArgs e)
		/// {
		///	 this.mEvent.TxtBtnEnterThenClick = new Control[] { txtValue, btnFind };
		/// }
		/// </code>
		/// </example>
		public Control[] TxtBtnEnterThenClick
		{
			set
			{
				Control[] actl = value;
				if ((actl.Length % 2) != 0)
				{
					throw new Exception("TxtBtnEnterThenClick 배열의 개수가 짝수가 아닙니다");
				}

				this.maTxtEnterThenClick = new TextBox[actl.Length / 2];
				this.maBtnEnterThenClick = new Button[actl.Length / 2];
				int n = -1;
				for (int i = 0, i2 = actl.Length; i < i2; i += 2)
				{
					n++;
					this.maTxtEnterThenClick[n] = (TextBox)actl[i];
					this.maBtnEnterThenClick[n] = (Button)actl[i + 1];

					this.maTxtEnterThenClick[n].KeyDown
						+= new System.Windows.Forms.KeyEventHandler(this.TxtEnterThenClick_KeyDown);
				}
			}
		}

		private void TxtEnterThenClick_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				TextBox Txt = (TextBox)sender;

				for (int i = 0, i2 = this.maTxtEnterThenClick.Length; i < i2; i++)
				{
					if (this.maTxtEnterThenClick[i] == Txt)
					{
						this.maBtnEnterThenClick[i].PerformClick();
						break;
					}
				}
			}
		}

		public static void ApplyKeyToTextBox(ControlShortcutKeys Key, params TextBox[] aTextBox)
		{
			switch (Key)
			{
				case ControlShortcutKeys.ControlA:
					{
						foreach (TextBox txtCur in aTextBox)
						{
							txtCur.KeyDown += new KeyEventHandler(
								delegate(object sender, KeyEventArgs e)
								{
									if (e.Control && (e.KeyCode == Keys.A))
									{
										((TextBox)sender).SelectAll();
									}
								});
						}
					}
					break;
			}
		}

	}
}
