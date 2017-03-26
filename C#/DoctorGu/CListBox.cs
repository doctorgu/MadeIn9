using System;
using System.Windows.Forms;

namespace DoctorGu
{
	/// <summary>
	/// Summary description for ListBox.
	/// </summary>
	public class CListBox
	{
		private CListBox()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static int GetIndexHasValue(ComboBox cbo, string Value, bool IgnoreCase)
		{
			string ValueCur = "";
			int Index = -1;

			Value = IgnoreCase ? Value.ToLower() : Value;
			for (int i = 0, j = cbo.Items.Count; i < j; i++)
			{
				ValueCur = IgnoreCase ? cbo.Items[i].ToString().ToLower() : cbo.Items[i].ToString();
				if (ValueCur == Value)
				{
					Index = i;
					break;
				}
			}

			return Index;
		}

		/// <summary>
		/// 끌어서놓기로 가져온 파일명을 ListBox에 추가함.
		/// </summary>
		/// <param name="lst"></param>
		/// <param name="Data"></param>
		/// <param name="ClearBefore"></param>
		public static void AddFileNamesToListBox(ListBox lst, IDataObject Data, bool ClearBefore)
		{
			if (Array.IndexOf(Data.GetFormats(false), "FileDrop") == -1) return;

			string[] aPathFile = (string[])Data.GetData("FileDrop");

			if (ClearBefore) lst.Items.Clear();

			lst.Items.AddRange(aPathFile);
		}

		public static string[] GetAllItems(ListBox lst)
		{
			string[] s = new String[lst.Items.Count];
			lst.Items.CopyTo(s, 0);
			return s;
		}

		public class KeyValue
		{
			public const int drKeyWidth = 4;

			/// <summary>
			/// ComboBox에서 Item을 설정하는 방법을 못 찾았으므로 "1   고기"와 같은 형식으로 Text를 추가하기 위함.
			/// </summary>
			/// <param name="Key"></param>
			/// <param name="Value"></param>
			/// <returns></returns>
			public static string GetKeySpaceValueForCbo(int Key, string Value)
			{
				return Key.ToString().PadRight(drKeyWidth, ' ') + Value;
			}
			public static int GetKeyFromCboText(string CboText)
			{
				return Int32.Parse(CboText.Substring(0, drKeyWidth));
			}
			public static string GetValueFromCboText(string CboText, int KeyWidth)
			{
				return CboText.Substring(KeyWidth);
			}
			public static string GetValueFromCboText(string CboText)
			{
				return GetValueFromCboText(CboText, drKeyWidth).Trim();
			}
			public static int GetIndexHasKey(ComboBox Cbo, int Key)
			{
				for (int i = 0, j = Cbo.Items.Count; i < j; i++)
				{
					int KeyCur = GetKeyFromCboText(Cbo.Items[i].ToString());
					if (KeyCur == Key)
					{
						return i;
					}
				}

				return -1;
			}
		}
	}
}
