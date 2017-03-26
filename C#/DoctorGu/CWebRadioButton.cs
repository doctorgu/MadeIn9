using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace DoctorGu
{
	public class CWebRadioButton
	{
		/// <summary>
		/// 한쪽의 RadioButton의 Checked를 true로 설정해도 다른쪽의 RadioButton의 Checked가
		/// 자동으로 false로 되지 않으므로 함수로 만듦.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="radTrue"></param>
		/// <param name="radFalse"></param>
		public static void CheckRadioButton(bool Value, RadioButton radTrue, RadioButton radFalse)
		{
			if (Value)
			{
				radTrue.Checked = true;
				radFalse.Checked = false;
			}
			else
			{
				radTrue.Checked = false;
				radFalse.Checked = true;
			}
		}
	}
}
