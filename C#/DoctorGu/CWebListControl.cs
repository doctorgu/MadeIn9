using System;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;

namespace DoctorGu
{
	public class CWebListControl
	{
		private CWebListControl()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static void AddNumber(ListControl lsc, int NumberFrom, int NumberTo, int SelectedNumber,
					string Format)
		{
			bool IsReverse = (NumberFrom > NumberTo);

			if (IsReverse)
			{
				for (int i = NumberFrom; i >= NumberTo; i--)
				{
					AddNumber(lsc, i.ToString(Format), i.ToString(), (i == SelectedNumber));
				}
			}
			else
			{
				for (int i = NumberFrom; i <= NumberTo; i++)
				{
					AddNumber(lsc, i.ToString(Format), i.ToString(), (i == SelectedNumber));
				}
			}
		}
		private static void AddNumber(ListControl lsc, string Text, string Value, bool Selected)
		{
			ListItem Itm = new ListItem(Text, Value);
			Itm.Selected = Selected;
			lsc.Items.Add(Itm);
		}

		public static ListItemCollection GetListItemCollection(DataTable dt, string ColumnNameText, string ColumnNameValue)
		{
			ListItemCollection lic = new ListItemCollection();
			foreach (DataRow dr in dt.Rows)
			{
				lic.Add(new ListItem(dr[ColumnNameText].ToString(), dr[ColumnNameValue].ToString()));
			}

			return lic;
		}
	
		/// <summary>
		/// 선택된 ListItem을 다른 ListControl로 이동함.
		/// </summary>
		/// <param name="lstSrc"></param>
		/// <param name="lstDest"></param>
		/// <param name="IsNoDuplicateValue">대상 ListControl에 이미 해당 Value가 있으면 이동하지 않음.</param>
		/// <returns>ListControl에 방금 추가된 ListItem</returns>
		public static ListItem MoveSelectedItem(ListControl lstSrc, ListControl lstDest, bool IsNoDuplicateValue)
		{
			ListItem itmSrc = lstSrc.SelectedItem;
			if (itmSrc == null) return null;

			return MoveItem(lstSrc, lstDest, itmSrc, IsNoDuplicateValue);
		}
		public static ListItem MoveItem(ListControl lstSrc, ListControl lstDest, ListItem ItemToMove, bool IsNoDuplicateValue)
		{
			int IndexSrc = lstSrc.Items.IndexOf(ItemToMove);
			
			string TextSrc = ItemToMove.Text;
			string ValueSrc = ItemToMove.Value;

			if (IsNoDuplicateValue)
			{
				foreach (ListItem li in lstDest.Items)
				{
					if (li.Value == ValueSrc)
					{
						return null;
					}
				}
			}

			if (lstDest.SelectedIndex == -1)
			{
				lstDest.Items.Add(new ListItem(TextSrc, ValueSrc));
				lstDest.SelectedIndex = lstDest.Items.Count - 1;
			}
			else
			{
				int IndexToInsert = lstDest.SelectedIndex + 1;

				lstDest.Items.Insert(IndexToInsert, new ListItem(TextSrc, ValueSrc));
				lstDest.SelectedIndex = IndexToInsert;
			}
	
			lstSrc.Items.Remove(ItemToMove);

			if (lstSrc.Items.Count == 1)
			{
				lstSrc.SelectedIndex = 0;
			}
			else if ((lstSrc.Items.Count - 1) < IndexSrc)
			{
				lstSrc.SelectedIndex = lstSrc.Items.Count - 1;
			}
			else 
			{
				lstSrc.SelectedIndex = IndexSrc;
			}

			return lstDest.SelectedItem;
		}

		/// <summary>
		/// 선택된 ListItem을 다른 ListControl로 복사함.
		/// </summary>
		/// <param name="lstSrc"></param>
		/// <param name="lstDest"></param>
		/// <param name="IsNoDuplicateValue">대상 ListControl에 이미 해당 Value가 있으면 복사하지 않음.</param>
		/// <returns>ListControl에 방금 추가된 ListItem</returns>
		public static ListItem CopySelectedItem(ListControl lstSrc, ListControl lstDest, bool IsNoDuplicateValue)
		{
			ListItem itmSrc = lstSrc.SelectedItem;
			if (itmSrc == null) return null;

			return CopyItem(lstSrc, lstDest, itmSrc, IsNoDuplicateValue);
		}
		public static ListItem CopyItem(ListControl lstSrc, ListControl lstDest, ListItem ItemToCopy, bool IsNoDuplicateValue)
		{
			int IndexSrc = lstSrc.Items.IndexOf(ItemToCopy);

			string TextSrc = ItemToCopy.Text;
			string ValueSrc = ItemToCopy.Value;

			if (IsNoDuplicateValue)
			{
				foreach (ListItem li in lstDest.Items)
				{
					if (li.Value == ValueSrc)
					{
						return null;
					}
				}
			}

			if (lstDest.SelectedIndex == -1)
			{
				lstDest.Items.Add(new ListItem(TextSrc, ValueSrc));
			}
			else
			{
				int IndexToInsert = lstDest.SelectedIndex + 1;

				lstDest.Items.Insert(IndexToInsert, new ListItem(TextSrc, ValueSrc));
				lstDest.SelectedIndex = IndexToInsert;
			}

			return lstDest.SelectedItem;
		}

		/// <summary>
		/// 현재 ListControl의 모든 ListItem을 다른 ListControl로 이동함.
		/// </summary>
		/// <param name="lstSrc"></param>
		/// <param name="lstDest"></param>
		public static void MoveAllItem(ListControl lstSrc, ListControl lstDest)
		{
			int IndexDest = lstDest.SelectedIndex;

			while (lstSrc.Items.Count > 0)
			{
				lstSrc.SelectedIndex = 0;
				ListItem itmCur = lstSrc.SelectedItem;

				string TextSrc = itmCur.Text;
				string ValueSrc = itmCur.Value;

				if (IndexDest == -1)
				{
					lstDest.Items.Add(new ListItem(TextSrc, ValueSrc));
				}
				else
				{
					//추가된 리스트 밑에 넣어야 하므로 1을 더함.
					IndexDest++;
					lstDest.Items.Insert(IndexDest, new ListItem(TextSrc, ValueSrc));
				}

				lstSrc.Items.Remove(itmCur);
			}
		}

		/// <summary>
		/// 현재 ListControl의 모든 ListItem을 다른 ListControl로 복사함.
		/// </summary>
		/// <param name="lstSrc"></param>
		/// <param name="lstDest"></param>
		public static void CopyAllItem(ListControl lstSrc, ListControl lstDest)
		{
			int IndexDest = lstDest.SelectedIndex;

			foreach (ListItem li in lstSrc.Items)
			{
				string TextSrc = li.Text;
				string ValueSrc = li.Value;

				if (IndexDest == -1)
				{
					lstDest.Items.Add(new ListItem(TextSrc, ValueSrc));
				}
				else
				{
					//추가된 리스트 밑에 넣어야 하므로 1을 더함.
					IndexDest++;
					lstDest.Items.Insert(IndexDest, new ListItem(TextSrc, ValueSrc));
				}
			}
		}

		/// <summary>
		/// 현재 선택된 ListItem을 위, 아래로 이동함.
		/// </summary>
		/// <param name="lst"></param>
		/// <param name="IsUp">ListItem을 위로 이동할 지 여부</param>
		public static void UpDownSelectedItem(ListControl lst, bool IsUp)
		{
			ListItem ItemSrc = lst.SelectedItem;
			if (ItemSrc == null) return;

			int IndexSrc = lst.SelectedIndex;

			int IndexDest = 0;


			//이동하기 위해 원래 ListItem을 삭제하므로 미리 정보를 저장해놓음.
			string ValueOld = ItemSrc.Value;
			string TextOld = ItemSrc.Text;

			if (IsUp)
			{
				if (IndexSrc == 0) return;

				IndexDest = IndexSrc - 1;
				lst.Items.Remove(ItemSrc);
				lst.Items.Insert(IndexDest, new ListItem(TextOld, ValueOld));
				lst.SelectedIndex = IndexDest;
			}
			else
			{
				if ((IndexSrc + 1) >= lst.Items.Count) return;

				if ((IndexSrc + 2) == lst.Items.Count)
				{
					lst.Items.Remove(ItemSrc);
					lst.Items.Add(new ListItem(TextOld, ValueOld));
				}
				else
				{
					lst.Items.Remove(ItemSrc);
					lst.Items.Insert(IndexSrc + 1, new ListItem(TextOld, ValueOld));
				}

				lst.SelectedIndex = IndexSrc + 1;
			}
		}

		public static void ShowFilesToListControl(FileInfo[] afi, ListControl Lst)
		{
			Lst.Items.Clear();
			foreach (FileInfo fi in afi)
			{
				Lst.Items.Add(new ListItem(fi.Name, fi.FullName));
			}
		}

		/// <summary>
		/// 이미 Selected된 상태에서 다시 Selected 속성을 쓰면
		/// Cannot have multiple items selected in a DropDownList 에러 발생하므로
		/// 해결하기 위함.
		/// </summary>
		/// <param name="lst"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static ListItem SelectByValue(ListControl lst, string Value)
		{
			ListItem itm = lst.Items.FindByValue(Value);
			if (itm == null)
				return null;

			lst.SelectedIndex = lst.Items.IndexOf(itm);
			return itm;
		}
		/// <summary>
		/// 이미 Selected된 상태에서 다시 Selected 속성을 쓰면
		/// Cannot have multiple items selected in a DropDownList 에러 발생하므로
		/// 해결하기 위함.
		/// </summary>
		/// <param name="lst"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static ListItem SelectByText(ListControl lst, string Text)
		{
			ListItem itm = lst.Items.FindByText(Text);
			if (itm == null)
				return null;

			lst.SelectedIndex = lst.Items.IndexOf(itm);
			return itm;
		}

		/// <summary>
		/// 선택된 Item을 위, 아래로 이동함.
		/// </summary>
		/// <param name="lsc"></param>
		/// <param name="IsDown"></param>
		public static void UpDownItem(ListControl lsc, bool IsDown)
		{
			ListItem ItmOld = lsc.SelectedItem;
			if (ItmOld == null)
				return;

			int IndexOld = lsc.SelectedIndex;
			ListItem ItmNew = new ListItem(ItmOld.Text, ItmOld.Value);

			if (IsDown)
			{
				//맨 아래에 있다면 더 이상 내려갈 수 없으므로 빠져나감.
				if ((IndexOld + 1) == lsc.Items.Count)
					return;


				if ((IndexOld + 2) == lsc.Items.Count)
				{
					lsc.Items.Add(ItmNew);
				}
				else
				{
					lsc.Items.Insert(IndexOld + 2, ItmNew);
				}

				lsc.Items.Remove(ItmOld);
				lsc.SelectedIndex = IndexOld + 1;
			}
			else
			{
				//맨 위에 있다면 더 이상 올라갈 수 없으므로 빠져나감.
				if (IndexOld == 0)
					return;

				//먼저 삭제해야 함. 먼저 삭제하지 않으면 순서가 바뀌지 않음.
				lsc.Items.Remove(ItmOld);
				lsc.Items.Insert(IndexOld - 1, ItmNew);

				lsc.SelectedIndex = IndexOld - 1;
			}
		}

		public static void DataBind(DataTable dt, ListControl lst, string FieldNameForValue, string FieldNameForText)
		{
			lst.DataValueField = FieldNameForValue;
			lst.DataTextField = FieldNameForText;
			lst.DataSource = dt;
			lst.DataBind();
		}
	}
}
