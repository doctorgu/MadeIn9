using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace DoctorGu
{
	public class CGridViewCheckBox
	{
		public static void CheckCurrentRowOnly(GridView grvList, string CheckBoxId, int RowIndex, int ColSelect)
		{
			for (int i = 0, i2 = grvList.Rows.Count; i < i2; i++)
			{
				CheckBox chkSelect = grvList.Rows[i].Cells[ColSelect].FindControl(CheckBoxId) as CheckBox;
				if (chkSelect == null)
					continue;

				chkSelect.Checked = (RowIndex == i);
			}
		}
		public static void CheckAllRows(GridView grvList, string CheckBoxId, int ColSelect)
		{
			for (int i = 0, i2 = grvList.Rows.Count; i < i2; i++)
			{
				CheckBox chkSelect = grvList.Rows[i].Cells[ColSelect].FindControl(CheckBoxId) as CheckBox;
				if (chkSelect == null)
					continue;

				chkSelect.Checked = true;
			}
		}

		public static string GetSeqListSelected(GridView grvList, string CheckBoxId, int ColSelect, int ColSeq)
		{
			string SeqList = "";
			for (int i = 0, i2 = grvList.Rows.Count; i < i2; i++)
			{
				CheckBox chkSelect = grvList.Rows[i].Cells[ColSelect].FindControl(CheckBoxId) as CheckBox;
				if (chkSelect == null)
					continue;
				if (!chkSelect.Checked)
					continue;

				int Seq = CFindRep.IfNotNumberThen0(grvList.Rows[i].Cells[ColSeq].Text);
				if (Seq == 0)
					continue;

				SeqList += "," + Seq.ToString();
			}
			if (SeqList != "")
			{
				SeqList = SeqList.Substring(1);
			}

			return SeqList;
		}

		public static string GetHeaderTextForToggleChecked(GridView grv, int ColumnIndex, string chkSelectId, string chkSelectArrayName)
		{
			List<string> aStmt = new List<string>();
			aStmt.Add("var " + chkSelectArrayName + " = CControl.GetDotNetGridViewCheckBoxItems('" + grv.ClientID + "', '" + chkSelectId + "');");

			//2009-4-4
			//GetDotNetGridViewCheckBoxItems 함수에서 배열을 다 가져오므로 주석.
			//참고로 MultiSelect 속성에서 이 함수를 호출했을 때 grv.Rows.Count를 읽은 경우 다음과 같이 name과 id 속성이 바뀌면서
			//btnFind_Click 이벤트가 일어나지 않는 버그 있었음.
			//원인은 Init 이벤트 전에 Rows.Count를 사용하면 Naming rule에 영향을 주는 것 같음.
			//<input type="submit" name="ctl00$cphContentBody$dtfExamPaper$btnFind" value="찾기" id="ctl00_cphContentBody_dtfExamPaper_btnFind" />
			//<input type="submit" name="dtfExamPaper$btnFind" value="찾기" id="dtfExamPaper_btnFind" />
			//for (int i = 0, i2 = grv.Rows.Count; i < i2; i++)
			//{
			//    CheckBox chkSelect = grv.Rows[i].Cells[ColumnIndex].FindControl(chkSelectId) as CheckBox;
			//    if (chkSelect == null)
			//    {
			//        continue;
			//    }

			//    aStmt.Add(chkSelectArrayName + ".push(document.getElementById('" + chkSelect.ClientID + "'));");
			//}
			aStmt.Add("CControl.ToggleChecked(" + chkSelectArrayName + ");");

			string Script = string.Join(" ", aStmt.ToArray());
			string HeaderNew = "<a href=\"javascript:" + Script + "\">선택</a>";

			return HeaderNew;
		}
	}
}