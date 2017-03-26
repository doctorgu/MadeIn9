using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace DoctorGu
{
	public class CDataGridView
	{
		public static bool UpDownSelectedRowOfDataTable(DataGridView dtg, bool IsUp)
		{
			DataTable dt = GetDataTable(dtg);

			//dtg.CurrentCell.RowIndex를 읽으면 * 표시된 추가 행을 선택했을 때에도 마지막 행 값이 읽혀지므로 SelectedCells 사용.
			int IndexCur = dtg.SelectedCells[0].RowIndex;
			int IndexNew = CDataTable.UpDownRow(dt, IndexCur, IsUp);
			if (IndexCur == IndexNew)
				return false;

			SelectNewRow(dtg, IndexNew);

			return true;
		}

		public static void CopyRow(DataGridView dtg, int IndexFrom, int IndexTo)
		{
			DataTable dt = GetDataTable(dtg);

			DataRow dr = CDataTable.CopyRow(dt, IndexFrom, true);

			int IndexNew = IndexTo;
			if ((IndexNew + 1) <= dt.Rows.Count)
			{
				dt.Rows.InsertAt(dr, IndexTo);
			}
			else
			{
				dt.Rows.Add(dr);
			}

			SelectNewRow(dtg, IndexNew);
		}
		public static bool MoveRow(DataGridView dtg, int IndexFrom, int IndexTo)
		{
			if ((IndexFrom == IndexTo) || ((IndexFrom + 1) == IndexTo))
				return false;

			DataTable dt = GetDataTable(dtg);

			DataRow dr = CDataTable.CopyRow(dt, IndexFrom, false);
			dt.Rows.RemoveAt(IndexFrom);

			int IndexNew = (IndexFrom < IndexTo) ? IndexTo - 1 : IndexTo;
			if ((IndexNew + 1) <= dt.Rows.Count)
			{
				dt.Rows.InsertAt(dr, IndexNew);
			}
			else
			{
				dt.Rows.Add(dr);
			}

			SelectNewRow(dtg, IndexNew);

			return true;
		}
		public static void DeleteRow(DataGridView dtg, int Index)
		{
			DataTable dt = GetDataTable(dtg);
			if ((Index + 1) > dt.Rows.Count)
				return;

			dt.Rows.RemoveAt(Index);

			if ((Index + 1) > dt.Rows.Count)
				SelectNewRow(dtg, dt.Rows.Count - 1);
		}

		private static void SelectNewRow(DataGridView dtg, int RowIndex)
		{
			DataGridViewCell cell = dtg.CurrentCell;
			cell.Selected = false;
			dtg.CurrentCell = dtg[cell.ColumnIndex, RowIndex];
		}

		private static DataTable GetDataTable(DataGridView dtg)
		{
			DataTable dt = null;

			DataSet ds = dtg.DataSource as DataSet;
			if (ds != null)
				dt = ds.Tables[0];
			else
				dt = dtg.DataSource as DataTable;

			return dt;
		}

		/// <summary>
		/// DataSource에 직접 DataTable을 지정하는 것으로는 바인딩되지 않음.
		/// </summary>
		public static void BindDataTable(DataGridView dtg, DataTable dt)
		{
			BindingSource bs = new BindingSource();
			bs.DataSource = dt;
			dtg.DataSource = bs;
		}

		/// <summary>
		/// DataGridView가 선택되지 않은 경우에 SelectedRows[0]을 쓰면 에러 나므로 안전하게 사용하기 위해 만듦.
		/// </summary>
		/// <param name="dtg"></param>
		/// <returns></returns>
		public static DataRowView GetSelectedDataRowView(DataGridView dtg)
		{
			if (dtg.SelectedCells.Count == 0)
				return null;
			else if (dtg.SelectedRows[0].DataBoundItem == null)
				return null;

			return (DataRowView)dtg.SelectedRows[0].DataBoundItem;
		}
	}
}
