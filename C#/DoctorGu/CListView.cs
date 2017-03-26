using System;
using System.Windows.Forms;
using System.Drawing;

namespace DoctorGu
{
	/// <summary>
	/// Summary description for ListView.
	/// </summary>
	public class CListView
	{
		public CListView()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// RowSrc 행의 모든 문자열과 같은 문자열을 가진 다른 행을 리턴함.
		/// </summary>
		/// <param name="lvw"></param>
		/// <param name="RowSrc"></param>
		/// <returns></returns>
		/// <example>
		/// this.Text = CListView.GetSameRowIndex(lvwSrc, 0).ToString();
		/// </example>
		public static int GetSameRowIndex(ListView lvw, int RowSrc)
		{
			ListViewItem ItemSrc = lvw.Items[RowSrc];
			string TextSrc = ItemSrc.Text;
			for (int i = 0, j = (lvw.Columns.Count - 1); i < j; i++)
			{
				TextSrc += "\t" + ItemSrc.SubItems[i].Text;
			}
			
			for (int i = 0, j = lvw.Items.Count; i < j; i++)
			{
				if (i == RowSrc) continue;
				
				ListViewItem ItemDest = lvw.Items[i];

				string TextDest = ItemDest.Text;
				for (int ii = 0, jj = (lvw.Columns.Count - 1); ii < jj; ii++)
				{
					TextDest += "\t" + ItemDest.SubItems[ii].Text;
				}
				
				if (TextSrc == TextDest)
				{
					return i;
				}
			}

			return -1;
		}

		public static bool UpDownSelectedItem(ListView lvw, bool IsUp)
		{
			if (lvw.SelectedItems.Count == 0) return false;

			ListViewItem ItemSrc = (ListViewItem)lvw.SelectedItems[0].Clone();
			int IndexSrc = lvw.SelectedItems[0].Index;

			if (IsUp)
			{
				if (IndexSrc == 0) return false;

				lvw.Items.RemoveAt(IndexSrc);
				ListViewItem ItemDest = lvw.Items.Insert(IndexSrc - 1, ItemSrc);
				ItemDest.Selected = true;
			}
			else
			{
				if ((IndexSrc + 1) == lvw.Items.Count) return false;
					
				lvw.Items.RemoveAt(IndexSrc);

				if ((IndexSrc + 1) == lvw.Items.Count)
				{
					ListViewItem ItemDest = lvw.Items.Add(ItemSrc);
					ItemDest.Selected = true;
				}
				else
				{
					ListViewItem ItemDest = lvw.Items.Insert(IndexSrc + 1, ItemSrc);
					ItemDest.Selected = true;
				}
			}

			return true;
		}
		
		/// <summary>
		/// ListView의 ListItem을 한 행씩 복사 또는 이동시킴.
		/// </summary>
		/// <param name="lvwSrc"></param>
		/// <param name="lvwDest"></param>
		/// <param name="IsCopy"></param>
		/// <returns></returns>
		/// <example>
		/// private void btnAdd_Click(object sender, System.EventArgs e)
		/// {
		///		CListView.CopyMoveSelectedItem(lvwSrc, lvwDest, false);
		/// }
		/// private void btnRemove_Click(object sender, System.EventArgs e)
		/// {
		///		CListView.CopyMoveSelectedItem(lvwDest, lvwSrc, false);
		/// }
		/// </example>
		public static bool CopyMoveSelectedItem(ListView lvwSrc, ListView lvwDest, bool IsCopy)
		{
			if (lvwSrc.SelectedItems.Count == 0) return false;

			ListViewItem ItemSrc = (ListViewItem)lvwSrc.SelectedItems[0].Clone();
			int IndexSrc = lvwSrc.SelectedItems[0].Index;
			if (ItemSrc == null) return false;
			
			ListViewItem ItemDest = lvwDest.Items.Add(ItemSrc);
			ItemDest.Selected = true;
			
			if (!IsCopy)
			{
				lvwSrc.Items.RemoveAt(IndexSrc);

				if (lvwSrc.Items.Count > 0)
				{
					if ((IndexSrc + 1) > lvwSrc.Items.Count)
					{
						lvwSrc.Items[lvwSrc.Items.Count - 1].Selected = true;
					}
					else
					{
						lvwSrc.Items[IndexSrc].Selected = true;
					}
				}
			}
			
			return true;
		}

		public static void CopyMoveAllItem(ListView lvwSrc, ListView lvwDest, bool IsCopy)
		{
			if (IsCopy)
			{
				for (int i = 0, j = lvwSrc.Items.Count; i < j; i++)
				{
					ListViewItem ItemSrc = (ListViewItem)lvwSrc.Items[i].Clone();
					int IndexSrc = lvwSrc.SelectedItems[0].Index;

					lvwDest.Items.Add(ItemSrc);
				}
			}
			else
			{
				while (lvwSrc.Items.Count > 0)
				{
					ListViewItem ItemSrc = (ListViewItem)lvwSrc.Items[0].Clone();
					int IndexSrc = lvwSrc.SelectedItems[0].Index;

					lvwDest.Items.Add(ItemSrc);

					lvwSrc.Items.RemoveAt(IndexSrc);
				}
			}
		}

		/// <summary>
		/// 윈도95의 단축키를 흉내냄.
		/// 복사, 붙여넣기, 편집, 삭제, 전체 선택을 지원함.
		/// </summary>
		/// <param name="lvw"></param>
		/// <param name="e"></param>
		public static void ManageShortcut(ListView lvw, KeyEventArgs e)
		{
			if ((e.Modifiers == Keys.None) && (e.KeyCode == Keys.F2))
			{
				//편집
				if (lvw.SelectedItems.Count > 0)
				{
					lvw.SelectedItems[0].BeginEdit();
				}
			}
			else if (((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.C)) 
				|| ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.Insert)))
			{
				//복사
				CopySelectedItem(lvw);
			}
			else if (((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.V))
				|| ((e.Modifiers == Keys.Shift) && (e.KeyCode == Keys.Insert)))
			{
				//붙여넣기
				PasteItem(lvw);
			}
			else if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
			{
				//전체 선택
				SelectAllItem(lvw);
			}
			else if ((e.Modifiers == Keys.None) && (e.KeyCode == Keys.Delete))
			{
				//삭제
				DeleteSelectedItem(lvw);
			}
		}

		public static void CopySelectedItem(ListView lvw)
		{
			string Rows = "";

			foreach (ListViewItem itm in lvw.Items)
			{
				if (itm.Selected)
				{
					string Row = GetItemText(itm);
					Rows += "\r\n" + Row;
				}
			}

			if (!string.IsNullOrEmpty(Rows))
			{
				Rows = Rows.Substring(1);
				Clipboard.SetDataObject(Rows, true);
			}
		}
		public static void CopyAllItem(ListView lvw)
		{
			string Rows = "";

			foreach (ListViewItem itm in lvw.Items)
			{
				string Row = GetItemText(itm);
				Rows += "\r\n" + Row;
			}

			if (!string.IsNullOrEmpty(Rows))
			{
				Rows = Rows.Substring(1);
				Clipboard.SetDataObject(Rows, true);
			}
		}
		private static string GetItemText(ListViewItem Item)
		{
			string Row = "";
			for (int i = 0, i2 = Item.SubItems.Count; i < i2; i++)
			{
				Row += "\t" + Item.SubItems[i].Text;
			}
			Row = Row.Substring(1);

			return Row;
		}

		public static void PasteItem(ListView lvw)
		{
			IDataObject data = Clipboard.GetDataObject();
			if (data.GetData("System.String", true) == null) return;

			string Text = data.GetData("System.String", true).ToString();

			Text = CFindRep.ReplaceLineBreakToNewLine(Text);
			if (!CFindRep.IsPastable(Text))
				return;

			string[] Rows = Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
			for (int rw = 0, rw2 = Rows.Length; rw < rw2; rw++)
			{
				string[] Cols = Rows[rw].Split('\t');
				lvw.Items.Add(new ListViewItem(Cols));
			}
		}

		public static void DeleteSelectedItem(ListView lvw)
		{
			int i = 0;
			do
			{
				if (lvw.Items[i].Selected)
				{
					lvw.Items.RemoveAt(i);
					i--;
				}
				i++;
			} while (i < lvw.Items.Count);
		}			

		public static void SelectAllItem(ListView lvw)
		{
			foreach (ListViewItem itm in lvw.Items)
			{
				itm.Selected = true;
			}
		}

		public static void CheckAllItems(ListView lvw)
		{
			foreach (ListViewItem itm in lvw.Items)
			{
				if (!itm.Checked)
				{
					itm.Checked = true;
				}
			}
		}
		public static void UncheckAllItems(ListView lvw)
		{ 
			foreach (ListViewItem itm in lvw.Items)
			{
				if (itm.Checked)
				{
					itm.Checked = false;
				}
			}
		}

		/// <summary>
		/// X와 Y의 위치에 있는 Item의 Row와 Column 인덱스를 리턴함.
		/// </summary>
		/// <param name="lvw"></param>
		/// <param name="X"></param>
		/// <param name="Y"></param>
		/// <param name="Row"></param>
		/// <param name="Col"></param>
		/// <example>
		/// int Row, Col;
		/// GetItemRowColByXY(ibListView1, e.X, e.Y, out Row, out Col);
		/// this.Text = Row.ToString() + "x" + Col.ToString();
		/// </example>
		public static void GetItemRowColByXY(ListView lvw, int X, int Y, out int Row, out int Col)
		{
			Row = -1; Col = -1;

			ListViewItem itm = lvw.GetItemAt(10, Y);
			if (itm != null)
			{
				int Width = 0, WidthOld = 0;
				for (int i = 0, i2 = itm.SubItems.Count; i < i2; i++)
				{
					WidthOld = Width;
					Width += lvw.Columns[i].Width;
					if ((X > WidthOld) && (X < Width))
					{
						Row = itm.Index;
						Col = i;
						return;
					}
				}
			}
		}

		public static int GetIndexHasValue(ListView lvw, string Value, int ColumnIndex, int ExceptRowIndex, bool IgnoreCase)
		{
			string ValueCur = "";
			int Index = -1;

			Value = IgnoreCase ? Value.ToLower() : Value;
			for (int i = 0, j = lvw.Items.Count; i < j; i++)
			{
				if (i == ExceptRowIndex) continue;

				ValueCur = IgnoreCase ? 
					lvw.Items[i].SubItems[ColumnIndex].Text.ToLower() :
					lvw.Items[i].SubItems[ColumnIndex].Text;

				if (ValueCur == Value)
				{
					Index = i;
					break;
				}
			}

			return Index;
		}

		/// <summary>
		/// ListView의 Item 중 <paramref name="Text"/>의 값을 가진 첫번째 Item의 Index를 리턴함.
		/// </summary>
		/// <param name="lvw">ListView</param>
		/// <param name="Text">찾을 문자열</param>
		/// <returns>첫번째 Item의 Index</returns>
		public static int IndexOfText(ListView lvw, string Text, StringComparison comparisonType)
		{
			for (int i = 0; i < lvw.Items.Count; i++)
			{
				ListViewItem itmCur = lvw.Items[i];
				if (itmCur.Text.Equals(Text, comparisonType))
				{
					return i;
				}
			}

			return -1;
		}

		public static void CheckOnlyOneItem(ListView lvw, ListViewItem ItemToCheck)
		{
			foreach (ListViewItem itm in lvw.Items)
			{
				if (itm == ItemToCheck)
				{
					itm.Checked = true;
				}
				else
				{
					if (itm.Checked)
					{
						itm.Checked = false;
					}
				}
			}
		}
	}
}
