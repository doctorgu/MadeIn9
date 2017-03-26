using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace DoctorGu
{
	public class CWebTable
	{
		/// <summary>
		/// asp:TableRow의 ID는 바로 읽을 수 없으므로 모든 TableRow에 대해 루핑해서
		/// 해당 ID를 가진 TableRow를 리턴함.
		/// </summary>
		/// <param name="trows">TableRowCollection</param>
		/// <param name="ID">찾을 TableRow의 ID</param>
		/// <returns><paramref name="ID"/> 값을 가진 TableRow</returns>
		public static TableRow GetTableRowByID(TableRowCollection trows, string ID)
		{
			foreach (TableRow r in trows)
			{
				if (r.ID == ID)
				{
					return r;
				}
			}

			return null;
		}

		public static string GetLinkButtonText(TableCell cell)
		{
			LinkButton lbt = cell.Controls[0] as LinkButton;
			if (lbt == null)
				return "";

			return lbt.Text;
		}
		public static int GetLinkButtonValue(TableCell cell)
		{
			LinkButton lbt = cell.Controls[0] as LinkButton;
			if (lbt == null)
				return 0;

			return CFindRep.IfNotNumberThen0(lbt.Text);
		}
		public static bool GetCheckBoxChecked(TableCell cell)
		{
			CheckBox chk = cell.Controls[0] as CheckBox;
			if (chk == null)
				return false;

			return chk.Checked;
		}
	}
}
