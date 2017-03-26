using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoctorGu
{
	/// <summary>
	/// 양쪽에 2개의 페이지를 표시하기 위해 양쪽의 VerticalScroll을 동기화시킴.
	/// </summary>
	/// <remarks>
	/// 한번 설정하면 제대로 업데이트되지 않아 VerticalScroll.Value는 항상 2번씩 설정함.
	/// </remarks>
	/// <example>
	/// this.mSync = new CSyncTwoScroll(panel1, panel2);
	/// this.mSync.Sync();
	/// </example>
	public class CSyncTwoScroll
	{
		private int mMaximum2 = 0;
		private Panel mpnlLeft, mpnlRight;

		public CSyncTwoScroll(Panel pnlLeft, Panel pnlRight)
		{
			this.mpnlLeft = pnlLeft;
			this.mpnlRight = pnlRight;

			this.mpnlLeft.VerticalScroll.Value = 0;
			this.mpnlLeft.VerticalScroll.Value = 0;

			this.mpnlRight.VerticalScroll.Value = 0;
			this.mpnlRight.VerticalScroll.Value = 0;

			this.mMaximum2 = GetVerticalScrollMaximum();
		}

		public void Sync()
		{
			this.mpnlLeft.Scroll += new ScrollEventHandler(mpnl1_Scroll);
			this.mpnlRight.Scroll += new ScrollEventHandler(mpnl2_Scroll);
		}
		public void Unsync()
		{
			this.mpnlLeft.Scroll -= new ScrollEventHandler(mpnl1_Scroll);
			this.mpnlRight.Scroll -= new ScrollEventHandler(mpnl2_Scroll);
		}

		private void mpnl1_Scroll(object sender, ScrollEventArgs e)
		{
			int NewValue = this.mpnlLeft.VerticalScroll.Value;
			if (NewValue > mMaximum2)
			{
				NewValue = mMaximum2;
				this.mpnlLeft.VerticalScroll.Value = NewValue;
			}

			this.mpnlRight.VerticalScroll.Value = NewValue;
			this.mpnlRight.VerticalScroll.Value = NewValue;
		}
		private void mpnl2_Scroll(object sender, ScrollEventArgs e)
		{
			int NewValue = this.mpnlRight.VerticalScroll.Value;
			this.mpnlLeft.VerticalScroll.Value = NewValue;
			this.mpnlLeft.VerticalScroll.Value = NewValue;
		}

		private int GetVerticalScrollMaximum()
		{
			Control ctl = mpnlRight.Controls[0];
			int Max = ctl.Height - (mpnlRight.Height - ctl.Top);
			if (Max < 0)
				Max = 0;

			return Max;
		}
	}
}
