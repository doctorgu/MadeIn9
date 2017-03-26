using System;
using System.Data;
using System.Windows.Forms;
using System.Text;

namespace DoctorGu
{
	/// <summary>
	/// DataGrid 관련 기능 구현. DataGrid보다 훨씬 좋은 Spread 쓸 것을 권장하며, 해당 Class는 곧 사라질 예정임.
	/// </summary>
	public class CDataGrid
	{
		public CDataGrid()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// DataGrid의 선택된 행을 복사 또는 이동시킴.
		/// </summary>
		/// <param name="dgSrc"></param>
		/// <param name="dgDest"></param>
		/// <param name="IsCopy"></param>
		/// <returns></returns>
		/// <example>
		/// private void btnAdd_Click(object sender, System.EventArgs e)
		/// {
		/// 	CopyMoveSelectedRow(dgSrc, dgDest, false);
		/// }
		/// private void btnRemove_Click(object sender, System.EventArgs e)
		/// {
		/// 	CopyMoveSelectedRow(dgDest, dgSrc, false);
		/// }
		/// </example>
		public static bool CopyMoveSelectedRow(DataGrid dgSrc, DataGrid dgDest, bool IsCopy)
		{
			DataTable dtSrc = (DataTable)dgSrc.DataSource;
			int IndexSrc = dgSrc.CurrentRowIndex;
			if (IndexSrc == -1) return false;
			DataRow drSrc = dtSrc.Rows[IndexSrc];

			DataTable dtDest = (DataTable)dgDest.DataSource;
			int IndexDest = dgDest.CurrentRowIndex;
			if (IndexDest == -1) IndexDest = 0;
			
			dtDest.Rows.Add(drSrc.ItemArray);
			if (!IsCopy)
			{
				dtSrc.Rows.RemoveAt(IndexSrc);
			}

			return true;
		}

		/// <summary>
		/// 버그 때문인 지 제대로 Insert 되지 않는 문제있음.
		/// </summary>
		/// <param name="dg"></param>
		/// <param name="IsUp"></param>
		/// <returns></returns>
		public static bool UpDownSelectedRow(DataGrid dg, bool IsUp)
		{
			if (dg.DataSource == null) return false;

			DataTable dt = (DataTable)dg.DataSource;
			if (dt.Rows.Count == 0) return false;

			int IndexSrc = dg.CurrentRowIndex;
			DataRow dr = dt.Rows[IndexSrc];
			
			DataRow dr2 = dt.NewRow();
			for (int i = 0, j = dt.Columns.Count; i < j; i++)
			{
				dr2[i] = dr[i];
			}

			if (IsUp)
			{
				if (IndexSrc == 0) return false;

				dt.Rows.Remove(dr);

				//!!! Insert가 아닌 마지막에 Add가 됨.
				dt.Rows.InsertAt(dr2, IndexSrc - 1);
			}
			else
			{
				//!!! 이 부분은 테스트 안됨.
				if ((IndexSrc + 1) == dt.Rows.Count) return false;
				
				dt.Rows.Remove(dr);

				if ((IndexSrc + 1) == dt.Rows.Count)
				{
					dt.Rows.Add(dr2.ItemArray);
				}
				else
				{
					dt.Rows.InsertAt(dr2, IndexSrc + 1);
				}
			}

			return true;
		}
	}
}
