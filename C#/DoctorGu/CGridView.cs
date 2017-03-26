using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace DoctorGu
{
	public class CGridView
	{
		public static void SetCheckedAllRows(GridView grvList, string CheckBoxId, int ColSelect, bool Checked)
		{
			for (int i = 0, i2 = grvList.Rows.Count; i < i2; i++)
			{
				CheckBox chkSelect = grvList.Rows[i].Cells[ColSelect].FindControl(CheckBoxId) as CheckBox;
				if (chkSelect == null)
					continue;

				chkSelect.Checked = Checked;
			}
		}

		public static void SetEnabledAllRows(GridView grvList, string CheckBoxId, int ColSelect, bool Enabled)
		{
			for (int i = 0, i2 = grvList.Rows.Count; i < i2; i++)
			{
				CheckBox chkSelect = grvList.Rows[i].Cells[ColSelect].FindControl(CheckBoxId) as CheckBox;
				if (chkSelect == null)
					continue;

				chkSelect.Enabled = Enabled;
			}
		}

		public static int GetSortColumnIndex(GridView grvList)
		{
			if (string.IsNullOrEmpty(grvList.SortExpression))
				return -1;

			foreach (DataControlField field in grvList.Columns)
			{
				if (field.SortExpression == grvList.SortExpression)
				{
					return grvList.Columns.IndexOf(field);
				}
			}

			return -1;
		}

		public static void AddSortSymbol(GridView grvList, GridViewRow Row, int ColumnIndex, Control ControlAsc, Control ControlDesc)
		{
			Control ControlToAdd = (grvList.SortDirection == SortDirection.Ascending) ? ControlAsc : ControlDesc;

			Row.Cells[ColumnIndex].Controls.Add(ControlToAdd);
		}
	}
}
