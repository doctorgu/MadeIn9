using System;
using System.Collections.Generic;
using System.Text;
//using FarPoint.Win.Spread;
//using FarPoint.Win.Spread.Model;

namespace DoctorGu
{
	public class CSprWin
	{
		//public delegate void AfterRowAdded(SheetView svDest, int RowIndexDest);
		//public AfterRowAdded OnAfterRowAdded;

		//public delegate void AfterAllRowsAdded(SheetView svDest);
		//public AfterAllRowsAdded OnAfterAllRowsAdded;

		//public delegate void BeforeRowDelete(SheetView svSrc, int RowIndexSrc, int RowIndexDest);
		//public BeforeRowDelete OnBeforeRowDelete;

		////private SheetView msvSrc = null, msvDest = null;
		//private FpSpread msprSrc = null, msprDest = null;

		////public CSpr(SheetView svSrc)
		////{
		////	this.msvSrc = svSrc;
		////}
		////public CSpr(SheetView svSrc, SheetView svDest)
		////{
		////	this.msvSrc = svSrc;
		////	this.msvDest = svDest;
		////}

		//public CSprWin(FpSpread sprSrc)
		//{
		//	this.msprSrc = sprSrc;
		//}
		//public CSprWin(FpSpread sprSrc, FpSpread sprDest)
		//{
		//	this.msprSrc = sprSrc;
		//	this.msprDest = sprDest;
		//}

		//public bool MoveRowBetweenSpr(string[] aKeyColHeaderSrc, int RowSrc)
		//{
		//	//SheetView svSrc = (this.msvSrc == null) ? this.msprDest.ActiveSheet : this.msvSrc;
		//	//SheetView svDest = (this.msvDest == null) ? this.msprDest.ActiveSheet : this.msvDest;
		//	SheetView svSrc = this.msprSrc.ActiveSheet;
		//	SheetView svDest = this.msprDest.ActiveSheet;

		//	int RowDest = svDest.Rows.Count;
		//	return CopyMoveRowBetweenSpr(svSrc, svDest, false, aKeyColHeaderSrc, RowSrc, RowSrc, RowDest);
		//}
		//private bool CopyMoveRowBetweenSpr(SheetView svSrc, SheetView svDest, bool IsCopy, string[] aKeyColHeaderSrc, int RowSrc, int Row2Src, int RowDest)
		//{
		//	//If RowSrc = 0 Then RowSrc = sprSrc.ActiveRow
		//	//If RowSrc < 1 Then Exit Function
		//	//If Row2Src = 0 Then Row2Src = RowSrc
		//	//If RowDest = -1 Then
		//	//	RowDest = sprDest.MaxRows + 1
		//	//ElseIf RowDest = 0 Then
		//	//	RowDest = 1
		//	//End If

		//	if (svSrc.NonEmptyRowCount == 0)
		//	{
		//		return false;
		//	}

		//	int Rows = Math.Abs(Row2Src - RowSrc) + 1;

		//	bool IsCopyOrMoved = false;
		//	int RowSrcCur = RowSrc;
		//	int RowDestCur = RowDest;
		//	while (RowSrcCur <= Row2Src)
		//	{
		//		bool IsCopyOrMoveAllowed = false;
		//		if ((aKeyColHeaderSrc == null) && (aKeyColHeaderSrc.Length == 0))
		//		{
		//			IsCopyOrMoveAllowed = true;
		//		}
		//		else if (GetSameRowByColHeaderInDest(svSrc, svDest, RowSrcCur, aKeyColHeaderSrc) == -1)
		//		{
		//			IsCopyOrMoveAllowed = true;
		//		}

		//		//같은 행이 대상 스프레드에 존재하지 않으면 원본 행을 복사시킴.
		//		if (IsCopyOrMoveAllowed)
		//		{
		//			svDest.Rows.Add(RowDestCur, 1);

		//			CopyRowByColHeader(svSrc, svDest, RowSrcCur, RowDestCur);

		//			if (this.OnAfterRowAdded != null)
		//			{
		//				this.OnAfterRowAdded(svDest, RowDest);
		//			}

		//			RowDestCur++;

		//			if (IsCopy)
		//			{
		//				RowSrcCur++;
		//			}
		//			else
		//			{
		//				//Move이므로 RowSrcCur는 변함없음.
		//				if (this.OnBeforeRowDelete != null)
		//				{
		//					this.OnBeforeRowDelete(svSrc, RowSrcCur, (RowDestCur - 1));
		//				}

		//				svSrc.Rows.Remove(RowSrcCur, 1);

		//				Row2Src--;
		//			}

		//			IsCopyOrMoved = true;
		//		}
		//		else
		//		{
		//			RowSrcCur++;
		//		}
		//	}

		//	if (this.OnAfterAllRowsAdded != null)
		//	{
		//		this.OnAfterAllRowsAdded(svDest);
		//	}

		//	return IsCopyOrMoved;
		//}

		///// <summary>
		///// sprSrc.RowSrc 행의 KeyColHeaderSrc 열의 문자열이 sprDest의 KeyColHeaderSrc 열에
		///// 존재한다면 그 행번호를 리턴함.
		///// </summary>
		///// <param name="RowSrc"></param>
		///// <param name="aKeyColHeaderSrc"></param>
		///// <returns></returns>
		//private int GetSameRowByColHeaderInDest(SheetView svSrc, SheetView svDest, int RowSrc, string[] aKeyColHeaderSrc)
		//{
		//	if (svSrc.NonEmptyRowCount == 0)
		//	{
		//		return -1;
		//	}

		//	if ((RowSrc + 1) > svDest.Rows.Count)
		//	{
		//		return -1;
		//	}

		//	string[] aTextSrc = new string[aKeyColHeaderSrc.Length];

		//	for (int cl = 0, cl2 = aKeyColHeaderSrc.Length; cl < cl2; cl++)
		//	{
		//		string ColHeaderCur = aKeyColHeaderSrc[cl];
		//		int ColCur = GetColByColHeader(svSrc, ColHeaderCur);
		//		aTextSrc[cl] = svSrc.Cells[RowSrc, ColCur].Text;
		//	}

		//	//모든 행에 대해 검사를 해서 같은 문자열이 있다면 그 행 번호를 리턴함.
		//	for (int rw = 0, rw2 = svDest.Rows.Count; rw < rw2; rw++)
		//	{
		//		int nSame = 0;
		//		for (int cl = 0, cl2 = aKeyColHeaderSrc.Length; cl < cl2; cl++)
		//		{
		//			string ColHeaderCur = aKeyColHeaderSrc[cl];
		//			int ColCur = GetColByColHeader(svDest, ColHeaderCur);
		//			string TextDest = svDest.Cells[rw, ColCur].Text;

		//			if (TextDest == aTextSrc[cl])
		//			{
		//				nSame++;
		//			}
		//		}

		//		if (nSame == aKeyColHeaderSrc.Length)
		//		{
		//			return rw;
		//		}
		//	}

		//	return -1;
		//}

		///// <summary>
		///// 특정 열머리글에 해당하는 열번호를 리턴함.
		///// </summary>
		///// <param name="sv"></param>
		///// <param name="ColHeader"></param>
		///// <returns></returns>
		//public static int GetColByColHeader(SheetView sv, string ColHeader)
		//{
		//	for (int cl = 0, cl2 = sv.Columns.Count; cl < cl2; cl++)
		//	{
		//		if (sv.Columns[cl].Label == ColHeader)
		//		{
		//			return cl;
		//		}
		//	}

		//	return -1;
		//}

		//private void CopyRowByColHeader(SheetView svSrc, SheetView svDest,
		//						int RowSrc, int RowDest)
		//{
		//	for (int cl = 0, cl2 = svSrc.Columns.Count; cl < cl2; cl++)
		//	{
		//		string HeaderSrc = svSrc.Columns[cl].Label;
		//		string TextSrc = svSrc.Cells[RowSrc, cl].Text;

		//		int ColDest = GetColByColHeader(svDest, HeaderSrc);
		//		if (ColDest != -1)
		//		{
		//			svDest.Cells[RowDest, ColDest].Text = TextSrc;
		//		}
		//	}

		//	svDest.Rows[RowDest].Tag = svSrc.Rows[RowSrc].Tag;
		//}

		///// <summary>
		///// 한 행씩 위, 아래로 옮김.
		///// </summary>
		///// <param name="sv"></param>
		///// <param name="RowIndexFrom"></param>
		///// <param name="IsDown"></param>
		///// <param name="ColToGroup"></param>
		///// <param name="IsCantReplaceIs"></param>
		///// <param name="ErrMsgIs"></param>
		///// <returns></returns>
		//public static bool ReplaceRowOrder(SheetView sv, int RowIndexFrom,
		//	bool IsDown, int ColToGroup, out bool IsCantMoveInNextIs,
		//	out string ErrMsgIs)
		//{
		//	IsCantMoveInNextIs = false;
		//	ErrMsgIs = "";

		//	//참고로 셀에 \n 문자열이 있다면 제대로 작동하지 않으므로 ClipValue는 쓰지 않음.

		//	if (RowIndexFrom <= -1)
		//	{
		//		ErrMsgIs = "행을 선택하지 않았습니다.";
		//		return false;
		//	}
		//	if (sv.Rows.Count <= 1)
		//	{
		//		ErrMsgIs = "행수가 하나 뿐이므로 옮길 수 없습니다.";
		//		return false;
		//	}

		//	int RowIndexFromStartIs = -1, RowIndexFromEndIs = -1;
		//	if (ColToGroup >= 0)
		//	{
		//		//그룹 열이 있으면 현재 그룹으로 묶여진 행을 구함.
		//		//VB의 GetRowsFromRowByGrouppedCol 함수 참고요
		//		throw new Exception("지원되지 않습니다.");
		//	}
		//	else
		//	{
		//		RowIndexFromStartIs = RowIndexFrom;
		//		RowIndexFromEndIs = RowIndexFrom;
		//	}

		//	int RowCountSrc = RowIndexFromEndIs - RowIndexFromStartIs + 1;
		//	int NextRow = -1;

		//	//위, 또는 아래 행의 첫번째 행의 위치를 알아냄.
		//	if (IsDown)
		//	{
		//		NextRow = RowIndexFromEndIs + 1;
		//		if ((NextRow + 1) > sv.NonEmptyRowCount)
		//		{
		//			ErrMsgIs = "마지막 행에 있으므로 더 이상 옮길 수 없습니다.";
		//			return false;
		//		}
		//	}
		//	else
		//	{
		//		NextRow = RowIndexFromStartIs - 1;
		//		if (NextRow <= -1)
		//		{
		//			ErrMsgIs = "첫 행에 있으므로 더 이상 옮길 수 없습니다.";
		//			return false;
		//		}
		//	}

		//	int RowIndexToStartIs = -1, RowIndexToEndIs = -1;
		//	if (ColToGroup >= 0)
		//	{
		//		//그룹 열이 있으면 아래, 또는 위의 그룹으로 묶인 행을 구함.
		//	}
		//	else
		//	{
		//		RowIndexToStartIs = NextRow;
		//		RowIndexToEndIs = NextRow;
		//	}
		//	int RowCountDest = RowIndexToEndIs - RowIndexToStartIs + 1;

		//	//옮겨진 후에 더 이상 옮길 수 없는 지 확인함.
		//	if (IsDown)
		//	{
		//		if ((RowIndexToEndIs + 1) == sv.NonEmptyRowCount)
		//		{
		//			IsCantMoveInNextIs = true;
		//		}
		//	}
		//	else
		//	{
		//		if (RowIndexToStartIs == 1)
		//		{
		//			IsCantMoveInNextIs = true;
		//		}
		//	}

		//	//옮길 행의 값이 지워질 것이므로 먼저 마지막 행 다음에 행을 추가함.
		//	sv.Rows.Count++;

		//	//대상 행이 원본 행으로 바뀌기 전에 새로 생긴 마지막 행에 데이터를 옮겨 놓음.
		//	int RowIndexTemp = sv.Rows.Count - RowCountSrc;
		//	CSpr.MoveRangeWithRowTag(sv, RowIndexFromStartIs, 0, RowIndexTemp, 0, RowCountSrc, sv.ColumnCount, false);

		//	//대상 행을 원본 행으로 옮김.
		//	int RowIndexSrc = IsDown ? RowIndexFromStartIs : (RowIndexToStartIs + RowCountSrc);
		//	CSpr.MoveRangeWithRowTag(sv, RowIndexToStartIs, 0, RowIndexSrc, 0, RowCountDest, sv.ColumnCount, false);

		//	//임시 행을 대상 행으로 옮김.
		//	int RowIndexDest = IsDown ? (RowIndexFromStartIs + RowCountDest) : RowIndexToStartIs;
		//	CSpr.MoveRangeWithRowTag(sv, RowIndexTemp, 0, RowIndexDest, 0, RowCountSrc, sv.ColumnCount, false);

		//	sv.RowCount -= RowCountSrc;

		//	//대상 행을 선택함.
		//	if (IsDown)
		//	{
		//		sv.ActiveRowIndex = RowIndexFrom + RowCountDest;
		//	}
		//	else
		//	{
		//		sv.ActiveRowIndex = RowIndexFrom - RowCountDest;
		//	}

		//	return true;
		//}

		//private static void MoveRangeWithRowTag(SheetView sv, int fromRow, int fromColumn,
		//	int toRow, int toColumn, int rowCount, int columnCount, bool dataOnly)
		//{
		//	int rw2 = toRow - 1;
		//	for (int rw = fromRow; rw <= (fromRow + rowCount - 1); rw++)
		//	{
		//		object TagSrc = sv.Rows[rw].Tag;

		//		rw2++;
		//		sv.Rows[rw2].Tag = TagSrc;
		//	}

		//	sv.MoveRange(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount, dataOnly);
		//}

		//public static bool ReplaceRowOrder(SheetView sv, int RowIndexFrom,
		//	bool IsDown, out string ErrMsgIs)
		//{
		//	bool IsCantMoveInNextIs = false;
		//	return ReplaceRowOrder(sv, RowIndexFrom, IsDown, -1, out IsCantMoveInNextIs, out ErrMsgIs);
		//}

		///// <summary>
		///// 체크박스 형식의 열의 모든 행의 체크를 선택함.
		///// </summary>
		///// <param name="sv"></param>
		///// <param name="ColHasCheck"></param>
		//public static void CheckAllRowsOfSpecificCol(SheetView sv, int ColHasCheck)
		//{
		//	for (int rw = 0, rw2 = sv.RowCount; rw < rw2; rw++)
		//	{
		//		sv.Cells[rw, ColHasCheck].Value = 1;
		//	}
		//}
		//public static void UncheckAllRowsOfSpecificCol(SheetView sv, int ColHasCheck)
		//{
		//	for (int rw = 0, rw2 = sv.RowCount; rw < rw2; rw++)
		//	{
		//		sv.Cells[rw, ColHasCheck].Value = 0;
		//	}
		//}

		//public static void ToggleAllRowsIfColHeaderClicked(FpSpread spr, CellClickEventArgs e, int ColIndex)
		//{
		//	//e.Row는 열제목을 클릭해도 0을 리턴하므로 열제목을 클릭했는 지 여부를 알기 위함.
		//	int RowIndex = spr.GetCellFromPixel(0, 0, e.X, e.Y).Row;

		//	SheetView sv = spr.ActiveSheet;

		//	if ((RowIndex == -1) && (e.Column == ColIndex))
		//	{
		//		bool IsUncheckExists = false;
		//		for (int rw = 0, rw2 = sv.RowCount; rw < rw2; rw++)
		//		{
		//			if (CFindRep.IfNotNumberThen0(sv.Cells[rw, ColIndex].Value) != 1)
		//			{
		//				IsUncheckExists = true;
		//				break;
		//			}
		//		}

		//		if (IsUncheckExists)
		//		{
		//			CheckAllRowsOfSpecificCol(spr.ActiveSheet, ColIndex);
		//		}
		//		else
		//		{
		//			UncheckAllRowsOfSpecificCol(spr.ActiveSheet, ColIndex);
		//		}
		//	}
		//}

		///// <summary>
		///// 페이지를 시트의 탭으로 표현하기 위함.
		///// </summary>
		///// <param name="spr"></param>
		///// <param name="PageSize"></param>
		///// <param name="CountAll"></param>
		//public static void SetSheetCountByPageSize(FpSpread spr, int PageSize, int CountAll)
		//{
		//	spr.Sheets.Count = 1;

		//	int PageCount = CMath.GetPageCount(CountAll, PageSize);
		//	if (PageCount == 0) PageCount = 1;

		//	SheetView svOld = spr.Sheets[0];

		//	if (spr.Sheets[0].SheetName != "1")
		//	{
		//		spr.Sheets[0].SheetName = "1";
		//	}
		//	while (spr.Sheets.Count < PageCount)
		//	{
		//		string SheetName = (spr.Sheets.Count + 1).ToString();
		//		SheetView svNew = new SheetView(SheetName);

		//		svNew.ColumnCount = svOld.ColumnCount;
		//		for (int cl = 0, cl2 = svNew.ColumnCount; cl < cl2; cl++)
		//		{
		//			svNew.Columns[cl].Label = svOld.Columns[cl].Label;
		//			svNew.Columns[cl].Locked = svOld.Columns[cl].Locked;
		//			svNew.Columns[cl].Width = svOld.Columns[cl].Width;
		//			svNew.Columns[cl].CellType = svOld.Columns[cl].CellType;
		//			svNew.Columns[cl].HorizontalAlignment = svOld.Columns[cl].HorizontalAlignment;
		//			svNew.Columns[cl].VerticalAlignment = svOld.Columns[cl].VerticalAlignment;
		//		}

		//		spr.Sheets.Add(svNew);
		//	}
		//}
	}
}
