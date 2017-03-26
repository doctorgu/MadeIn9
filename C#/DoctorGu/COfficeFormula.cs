using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class COfficeFormula
	{
		//public static string GetA1ByRowColumn(int RowIndex, int ColumnIndex)
		//{
		//    return ColumnIndexToAlpha(ColumnIndex) + (RowIndex + 1).ToString();
		//}
		public static string GetA1ByRowColumnIndex(int RowIndex, int ColumnIndex)
		{
			return ColumnNumberToAlpha(ColumnIndex + 1) + (RowIndex + 1).ToString();
		}

		/// <summary>
		/// 1, 2, 3을 엑셀의 열 주소 표현방식인 A, B, C로 바꿈.
		/// </summary>
		public static string ColumnNumberToAlpha(int ColumnNumber)
		{
			int Dividend = ColumnNumber;
			string ColumnName = String.Empty;
			int Modulo;

			while (Dividend > 0)
			{
				Modulo = (Dividend - 1) % 26;
				ColumnName = Convert.ToChar(65 + Modulo).ToString() + ColumnName;
				Dividend = (int)((Dividend - Modulo) / 26);
			}

			return ColumnName;
		}
	}
}
