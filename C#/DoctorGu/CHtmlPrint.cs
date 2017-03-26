using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Collections.Generic;

// * Border 증가하면 y부터 밀리게(셀높이는 그대로)
// * 머지 기능
// * Values[0].Text = "abc"를 InnerHtml = "abc"로 변경.
// * 다음 태그만 파싱:
//   <img>, <FONT SIZE="" COLOR="" FACE="">, <B>, <I>, <U>, <STRIKE>, <BR>, <P>
// * Style 설정시 Clone 사용하지 않게
// * 태그 추가 가능하게(Element 상위 클래스 있게) -> <br>, <p>만 추가함.
// 0 HtmlParser에서 파싱시 AttrName이 style이면 Style 파싱하게
//
// 0 Border 증가하면 x부터 밀리게(전체 너비 그대로, 셀너비 감소)
// 0 좌표 x 위치에서는 CellSpacing, CellPadding, BorderLeft.Width, BorderRight.Width가 
//   모두 감안되지 않음.

namespace DoctorGu
{
	/// <summary>
	/// Summary description for HtmlPrint.
	/// </summary>
	public class CHtmlPrint
	{
		private PrintDocument mPrintDocument;
		private Margins mMargins;
		private int mPageWidth, mPageHeight;
		private int mClientPageWidth, mClientPageHeight;

		//private Tables mTables;
		private Elements mElements;
		private HtmlStyles mHtmlStyles;
		private HtmlStyle styCur;

		//구해진 너비, 높이보다 작은 너비, 높이를 가져오기 위함.
		private const float HeightRate = 0.9f, WidthRate = 0.75f;

		public CHtmlPrint(PrintDocument p)
		{
			mPrintDocument = p;
			mMargins = mPrintDocument.DefaultPageSettings.Margins;
			mPageWidth = mPrintDocument.DefaultPageSettings.PaperSize.Width;
			mPageHeight = mPrintDocument.DefaultPageSettings.PaperSize.Height;
			mClientPageWidth = mPageWidth - mMargins.Left - mMargins.Right;
			mClientPageHeight = mPageHeight - mMargins.Top - mMargins.Bottom;
			
			mElements = new Elements();
			mHtmlStyles = new HtmlStyles();
		}

		public void PrintOut(Graphics g)
		{
			int y = mMargins.Top;

			HtmlStyle styBody = mHtmlStyles["BODY"];
			if (styBody != null)
			{
				styCur = styBody;
			}

			for (int i = 0, i2 = mElements.Count; i < i2; i++)
			{
				object e = mElements[i];
				Table t = e as Table;
				if (t != null)
				{	
					PrintTable(g, t, 0, ref y, mMargins.Left, mClientPageWidth);
					continue;
				}

				Br b = e as Br;
				if (b != null)
				{
					PrintBr(g, ref y);
					continue;
				}

				P p = e as P;
				if (p != null)
				{
					PrintP(g, ref y);
					continue;
				}

				throw new Exception(e.ToString() + " 개체는 허용되지 않습니다.");
			}
		}

		public void PrintBr(Graphics g, ref int y)
		{
			Font f = (styCur != null) ? styCur.Font : CHtmlParser.GetDefaultFont();
			
			SizeF sizef = g.MeasureString(" ", f);
			y += Convert.ToInt32(sizef.Height);
		}
		public void PrintP(Graphics g, ref int y)
		{
			Font f = (styCur != null) ? styCur.Font : CHtmlParser.GetDefaultFont();
			
			SizeF sizef = g.MeasureString(" \n ", f);
			y += Convert.ToInt32(sizef.Height);
		}


		public void PrintTable(Graphics g, Table table, float x, ref int y, int ContainerMarginLeft, int ContainerWidth)
		{
			float XStart = 0;
			int YStart = 0;
			Color BorderColor = table.BorderColor;
			
			//100%에 해당
			if (table.Width == -1)
			{
				table.Width = ContainerWidth - table.Border;
			}
			
			SetColWidthOverwriteStyle(table);

			Pen PenBorder = new Pen(new SolidBrush(BorderColor), table.Border);
			Pen PenTopLine, PenBottomLine;

			switch (table.Align)
			{
				case HAlign.Left:
					x += ContainerMarginLeft;
					break;
				case HAlign.Center:
					x += (ContainerWidth - table.Width) / 2;
					break;
				case HAlign.Right:
					x += ContainerWidth - table.Width;
					break;
			}

			XStart = x; YStart = y;

			CRows rows = table.Rows;

			for (int rw = 0, rw2 = rows.Count; rw < rw2; rw++)
			{
				CRow row = rows[rw];
				//if (row.Style.BackgroundColor == Color.Black) {int xx = 0;}

				row.Height = GetRowHeight(g, table, row);
				
				x = XStart;

				
				int TopLineWidth = 0;
				if (rw == 0)
				{
					TopLineWidth = table.Border;
					PenTopLine = new Pen(new SolidBrush(BorderColor), TopLineWidth);
				}
				else
				{
					TopLineWidth = table.Border > 0 ? 1 : 0;
					PenTopLine = new Pen(new SolidBrush(BorderColor), TopLineWidth);
				}

				int BottomLineWidth = 0;
				if ((rw + 1) == rw2)
				{
					BottomLineWidth = table.Border;
					PenBottomLine = new Pen(new SolidBrush(BorderColor), BottomLineWidth);
				}
				else
				{
					BottomLineWidth = table.Border > 0 ? 1 : 0;
					PenBottomLine = new Pen(new SolidBrush(BorderColor), BottomLineWidth);
				}

				y += TopLineWidth + table.CellSpacing;
				
				Columns cols = row.Columns;
				
				for (int cl = 0, cl2 = cols.Count; cl < cl2; cl++)
				{
					Column col = cols[cl];
					
					col.Align = OverwriteHAlign(row.Align, col.Align);
					col.VAlign = OverwriteVAlign(row.VAlign, col.VAlign);

					//Values.Count가 2개 이상이면 DrawStringStep에서 정렬함.
					if (col.Values.Count == 1)
					{
						switch (col.Align)
						{
							case HAlign.Left:
								col.Values[0].Style.Format.Alignment = StringAlignment.Near;
								break;
							case HAlign.Center:
								col.Values[0].Style.Format.Alignment = StringAlignment.Center;
								break;
							case HAlign.Right:
								col.Values[0].Style.Format.Alignment = StringAlignment.Far;
								break;
						}
					}

					//수직 가운데는 StringFormat 속성에 없으므로 시작위치를 설정함.
					float ytext = 0;
					switch (col.VAlign)
					{
						case VAlign.Top:
							ytext = y + table.CellPadding + col.Style.BorderTop.WidthReal;
							break;
						case VAlign.Middle:
							float textHeightMargin = row.Height - (table.CellPadding * 2) 
								- col.Style.BorderTop.WidthReal - col.Style.BorderBottom.WidthReal;
							ytext = y + table.CellPadding + col.Style.BorderTop.WidthReal 
								+ ((textHeightMargin - col.TextHeight) / 2f);
							break;
						case VAlign.Bottom:
							ytext = y + (row.Height - table.CellPadding - col.Style.BorderBottom.WidthReal 
								- col.TextHeight);
							break;
					}

					if (col.Values.Count > 0)
					{
						//안에 Table이 있으면 RowHeight가 변경되므로 문자열 출력이 항상 먼저 있어야 함.
						//그러나 이번이 두번째 이상의 열이면 첫번째 열과 높이가 틀려진다는 버그 있음.
						if (col.Values[0].Table != null)
						{
							//테이블이 안에 있으면 무조건 왼쪽 정렬, 위쪽 정렬로 설정함.
							int Y2 = y + table.CellPadding + col.Style.BorderTop.WidthReal;
							int Y2Old = Y2;
							//수직선인 경우 가운데를 중심으로 양쪽의 공간을 차지함.
							float X2 = x + (table.Border / 2f) + (col.Values[0].Table.Border / 2f);
							PrintTable(g, col.Values[0].Table, X2, ref Y2, 0, col.Width);

							int RowHeight2 = Y2 - Y2Old;
							if (RowHeight2 > row.Height)
							{
								row.Height = RowHeight2;
							}
						}
						else
						{
							if (col.Values.Count == 1)
							{
								//if (col.Values[0].Text == "구좌코드"){int xx=0;}
								g.DrawString(col.Values[0].Text, col.Values[0].Style.Font, new SolidBrush(col.Values[0].Style.Color),
									new RectangleF(x, ytext, col.Width - table.Border, col.TextHeight),
									col.Values[0].Style.Format);
							}
							else if (col.Values.Count > 1)
							{
								DrawStringStep(g, col, Convert.ToSingle(x), ytext, col.Width - table.Border, col.TextHeight);
							}
						}
					}

					if (col.Style.BackgroundColor != Color.Transparent)
					{
						RectangleF rectBack = new RectangleF(
							x + (table.Border / 2f), y,
							col.Width - table.Border, row.Height);

						g.FillRectangle(new SolidBrush(col.Style.BackgroundColor), rectBack);
					}

					if (table.Border > 0)
					{
						if (rw == 0)
							//위쪽
							g.DrawLine(PenTopLine, 
								x - (table.Border / 2f), y - table.CellSpacing - (TopLineWidth / 2f),
								x + col.Width - (table.Border / 2f), y - table.CellSpacing - (TopLineWidth / 2f));
						if (cl == 0)
							//왼쪽
							g.DrawLine(PenBorder,
								x, y - table.CellSpacing,
								x, y + row.Height + BottomLineWidth);

						//오른쪽
						g.DrawLine(PenBorder, 
							x + col.Width, y - table.CellSpacing - TopLineWidth, 
							x + col.Width, y + row.Height);
						//아래쪽
						g.DrawLine(PenBottomLine, 
							x + (table.Border / 2f), y + row.Height + (BottomLineWidth / 2f),
							x + col.Width + (table.Border / 2f), y + row.Height + (BottomLineWidth / 2f));
					}

					//BORDER-TOP 등의 스타일 적용
					if (col.Style.BorderTop.WidthReal > 0)
					{
						//위쪽
						DrawBorder(g, col.Style.BorderTop.Color, 
							x + (table.Border / 2f), y,
							col.Width - table.Border, col.Style.BorderTop.Width, BorderDirection.Top, col.Style.BorderTop.Style);
					}
					if (col.Style.BorderRight.WidthReal > 0)
					{
						//오른쪽
						DrawBorder(g, col.Style.BorderRight.Color,
							x + col.Width - (table.Border / 2f), y,
							row.Height, col.Style.BorderRight.Width, BorderDirection.Right, col.Style.BorderRight.Style);
					}
					if (col.Style.BorderBottom.WidthReal > 0)
					{
						//아래쪽
						DrawBorder(g, col.Style.BorderBottom.Color,
							x + (table.Border / 2f), y + row.Height,
							col.Width - table.Border, col.Style.BorderBottom.Width, BorderDirection.Bottom, col.Style.BorderBottom.Style);
					}
					if (col.Style.BorderLeft.WidthReal > 0)
					{
						//왼쪽
						DrawBorder(g, col.Style.BorderLeft.Color,
							x + (table.Border / 2f), y,
							row.Height, col.Style.BorderLeft.Width, BorderDirection.Left, col.Style.BorderLeft.Style);
					}

					//다음 번 루프에선 span된 열은 건너뛰게 함.
					if (col.ColSpan > 1)
					{
						cl += (col.ColSpan - 1);
					}

					x += col.Width;
				} //cl

				y += row.Height;
			} //rw

			y += table.Border;
		}

		private void DrawStringStep(Graphics g, Column col, float x, float y, float drawableWidth, float textHeight)
		{
			float textWidth = 0;
			float startx = x;

			Value value;

			List<string> stackText = new List<string>();
			List<HtmlStyle> stackStyle = new List<HtmlStyle>();
			List<float> stackWidth = new List<float>();

			float textHeightMax = 0;
			for (int v = 0, v2 = col.Values.Count; v < v2; v++)
			{
				value = col.Values[v];
				if (value.Text == "") continue;
			
				for (int c = 0, c2 = value.Text.Length; c < c2; c++)
				{
					string s = value.Text.Substring(c, 1);

					SizeF textSize = g.MeasureString(s, value.Style.Font, 999999, value.Style.Format);
					if (textSize.Height > textHeightMax)
						textHeightMax = textSize.Height;

					float WidthRMargin = textSize.Width * WidthRate;
					textWidth += WidthRMargin;

					if ((textWidth + (textSize.Width * 0.277f)  > drawableWidth) || s == "\n")
					{
						if (textWidth + (textSize.Width * 0.277f)  > drawableWidth)
						{
							DrawOneLine(g, col.Align, startx, y, drawableWidth, textWidth, stackText, stackStyle, stackWidth);
							c--; //넘친 문자열은 다음 번 루핑에서 인쇄되게 함.
						}
						else if (s == "\n")
						{
							DrawOneLine(g, col.Align, startx, y, drawableWidth, textWidth, stackText, stackStyle, stackWidth);
						}

						stackText = new List<string>();
						stackStyle = new List<HtmlStyle>();
						stackWidth = new List<float>();

						x = startx;
						y += textHeightMax * HeightRate;
						textHeightMax = textSize.Height;
						textWidth = 0;
					}
					else
					{
						stackText.Add(s);
						stackStyle.Add(value.Style);
						stackWidth.Add(WidthRMargin);
					}

					x += WidthRMargin;
				} //c
			} //v

			if (stackText.Count > 0)
			{
				DrawOneLine(g, col.Align, startx, y, drawableWidth, textWidth, stackText, stackStyle, stackWidth);
			}
		}
		private void DrawOneLine(Graphics g, HAlign hAlign, float startx, float y, float drawableWidth, float textWidth,
			List<string> stackText, List<HtmlStyle> stackStyle, List<float> stackWidth)
		{
			float x = 0;

			switch (hAlign)
			{
				case HAlign.Left:
					x = startx; break;
				case HAlign.Center:
					x = startx + (drawableWidth - textWidth) / 2; break;
				case HAlign.Right:
					x = startx + (drawableWidth - textWidth); break;
			}

			for (int i = 0, i2 = stackText.Count; i < i2; i++)
			{
				string s = stackText[i];
				HtmlStyle sty = stackStyle[i];
				float width = stackWidth[i];

				g.DrawString(s, sty.Font, new SolidBrush(sty.Color),
					new PointF(x, y), sty.Format);

				x += width;
			}
		}

		private int GetRowHeight(Graphics g, Table table, CRow row)
		{
			int Max = 0;
			
			Columns cols = row.Columns;

			for (int cl = 0, cl2 = cols.Count; cl < cl2; cl++)
			{
				Column col = cols[cl];
				Value value;
				int BorderWidth = col.Style.BorderTop.WidthReal + col.Style.BorderBottom.WidthReal;
				float drawableWidth = col.Width - table.Border;
 
				if (col.Values.Count > 0)
				{
					if (col.Values.Count == 1)
					{
						value = col.Values[0];
						if (value.Text != "")
						{				
							SizeF textSize = g.MeasureString(value.Text, value.Style.Font, 
								Convert.ToInt32(drawableWidth), value.Style.Format);
							col.TextHeight = textSize.Height;
						}
					}
					else if (col.Values.Count > 1)
					{
						float textWidth = 0;
						float y = 0;
						float textHeightMax = 0;
						for (int v = 0, v2 = col.Values.Count; v < v2; v++)
						{
							value = col.Values[v];
							if (value.Text == "") continue;
						
							for (int c = 0, c2 = value.Text.Length; c < c2; c++)
							{
								string s = value.Text.Substring(c, 1);

								SizeF textSize = g.MeasureString(s, value.Style.Font, 999999, value.Style.Format);
								if (textSize.Height > textHeightMax)
									textHeightMax = textSize.Height;

								float WidthRMargin = textSize.Width * WidthRate;
								textWidth += WidthRMargin;

								if ((textWidth + (textSize.Width * 0.277f)  > drawableWidth) || s == "\n")
								{
									if (textWidth + (textSize.Width * 0.277f)  > drawableWidth)
									{
										c--; //넘친 문자열은 다음 번 루핑에서 인쇄되게 함.
									}
									else if (s == "\n")
									{
									}
						
									y += textHeightMax * HeightRate;
									textHeightMax = textSize.Height;
									textWidth = 0;
								}
							} //c
						} //v

						col.TextHeight = y + textHeightMax;
					}
				}

				int Cur = (table.CellPadding * 2) + BorderWidth + Convert.ToInt32(col.TextHeight);
				
				if (row.Height > Cur)
					Cur = row.Height;
				
				if (Cur > Max)
					Max = Cur;
			}

			return Max;
		}

		private void OverwriteStyle(HtmlStyle SOld, HtmlStyle SNew)
		{
			HtmlStyle DefaultStyle = new HtmlStyle();

			if (!SNew.BackgroundColorSetted)
				SNew.BackgroundColor = SOld.BackgroundColor;

			if (!SNew.ColorSetted)
				SNew.Color = SOld.Color;

			string NameNew = SNew.FontNameSetted ? SNew.Font.Name : SOld.Font.Name;
			float SizeNew = SNew.FontSizeSetted ? SNew.Font.Size : SOld.Font.Size;
			FontStyle FontStyleNew = SNew.FontStyleSetted ? SNew.Font.Style : SOld.Font.Style;
			SNew.Font = new Font(NameNew, SizeNew, FontStyleNew);
			
			if (!SNew.FormatAlignmentSetted)
				SNew.Format.Alignment = SOld.Format.Alignment;

			if (!SNew.FormatTrimmingSetted)
				SNew.Format.Trimming = SOld.Format.Trimming;
			
			//Border 관련 스타일은 상속되지 않으므로 현재 값을 사용함.
		}
		private HAlign OverwriteHAlign(HAlign AOld, HAlign ANew)
		{
			if (ANew == CHtmlParser.GetDefaultHAlign())
				return AOld;
			else
				return ANew;
		}
		private VAlign OverwriteVAlign(VAlign AOld, VAlign ANew)
		{
			if (ANew == CHtmlParser.GetDefaultVAlign())
				return AOld;
			else
				return ANew;
		}

		private void SetColWidthOverwriteStyle(Table table)
		{
			CRows rows = table.Rows;
			int ColSpanRest = 0;
			bool ColSpanStarted = false;
			int ColSpanWidthAvg = 0;
			int ColWidth = 0;

			for (int rw = 0, rw2 = rows.Count; rw < rw2; rw++)
			{
				CRow row = table.Rows[rw];

				//.TABLE 스타일 있다면 적용.
				HtmlStyle styT = mHtmlStyles["TABLE"];
				if (styT != null) OverwriteStyle(styT, table.Style);

				OverwriteStyle(table.Style, row.Style);
				//.TR 스타일 있다면 적용.
				HtmlStyle styTR = mHtmlStyles["TR"];
				if (styTR != null) OverwriteStyle(styTR, row.Style);

				Columns cols = row.Columns;

				for (int cl = 0, cl2 = cols.Count; cl < cl2; cl++)
				{
					Column col = cols[cl];

					OverwriteStyle(row.Style, col.Style);
					//.TD 스타일 있다면 적용.
					HtmlStyle styTD = mHtmlStyles["TD"];
					if (styTD != null) OverwriteStyle(styTD, col.Style);
					
					for (int v = 0, v2 = col.Values.Count; v < v2; v++)
					{
						OverwriteStyle(col.Style, col.Values[v].Style);
						//if (col.Values[v].Text == "333333-3333333")
						//	System.Diagnostics.Debug.Assert(false);
					}

					if (rw == 0)
					{
						if (col.ColSpan > 1)
						{
							ColSpanStarted = true;
							ColSpanRest = col.ColSpan - 1;
							ColWidth = col.Width;
							ColSpanWidthAvg = Convert.ToInt32(ColWidth / col.ColSpan);
							col.WidthBeforeSpan = ColSpanWidthAvg;
						}
						else if (ColSpanStarted)
						{
							ColSpanRest--;
							if (ColSpanRest == 0) ColSpanStarted = false;
							col.WidthBeforeSpan = ColSpanWidthAvg;
							col.Width = 0;
						}
						else
						{
							if (col.Width == 0)
							{
								//첫번째 행이면 테이블 너비 / 열 개수
								col.Width = Convert.ToInt32(table.Width / row.Columns.Count);
							}
						}
					}
					else
					{
						if (col.ColSpan > 1)
						{
							ColSpanStarted = true;
							ColSpanRest = col.ColSpan - 1;

							col.WidthBeforeSpan = GetPrevColWidth(rows, rw, cl);
							if (col.Width == 0)
							{
								col.Width = GetPrevColWidth(rows, rw, cl, col.ColSpan);
							}
						}
						else if (ColSpanStarted)
						{
							ColSpanRest--;
							if (ColSpanRest == 0) ColSpanStarted = false;
							col.WidthBeforeSpan = GetPrevColWidth(rows, rw, cl);
							col.Width = 0;
						}
						else
						{
							if (col.Width == 0)
							{
								//두번째 행부터는 이전 행의 너비를 따름.
								col.Width = GetPrevColWidth(rows, rw, cl);
							}
						}
					}
				}
			}
		}

		private int GetPrevColWidth(CRows rows, int rw, int cl)
		{
			int WidthLast = 0;

			int WidthBeforeSpan = rows[rw - 1].Columns[cl].WidthBeforeSpan;
			int Width = rows[rw - 1].Columns[cl].Width;

			if ((Width == 0) && (WidthBeforeSpan > 0))
			{
				WidthLast = WidthBeforeSpan;
			}
			else if ((Width > 0) && (WidthBeforeSpan == 0))
			{
				WidthLast = Width;
			}
			else if ((Width > 0) && (WidthBeforeSpan > 0))
			{
				WidthLast = (Width > WidthBeforeSpan) ? WidthBeforeSpan : Width;
			}

			return WidthLast;
		}
		private int GetPrevColWidth(CRows rows, int rw, int cl, int ColSpan)
		{
			int ColWidth = 0;
			for (int i = cl; i < (cl + ColSpan); i++)
			{
				ColWidth += GetPrevColWidth(rows, rw, i);
			}
			return ColWidth;
		}

		/// <summary>
		/// HTML 스타일의 BORDER-TOP을 그림.
		/// </summary>
		/// <param name="g"></param>
		/// <param name="c"></param>
		/// <param name="Width"></param>
		/// <param name="BorderWidth"></param>
		/// <param name="startX">
		/// direct의 위치에 따라 다음과 같이 시작 위치가 정해짐.(시계 방향)
		/// Top: 왼쪽에서 시작, Right: 위쪽에서 시작, Bottom : 오른쪽에서 시작, Left: 아래쪽에서 시작
		/// </param>
		/// <param name="startY">
		/// direct의 위치에 따라 다음과 같이 시작 위치가 정해짐.(시계 방향)
		/// Top: 왼쪽에서 시작, Right: 위쪽에서 시작, Bottom : 오른쪽에서 시작, Left: 아래쪽에서 시작
		/// </param>
		/// <param name="direct"></param>
		/// <example>
		/// private void Form1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		/// {
		/// 	Graphics g = e.Graphics;
		/// 
		/// 	Color c = Color.Transparent;
		/// 	BorderDirection direct = BorderDirection.Top;
		/// 	int BorderWidth = 10;
		/// 	int Width = 100;
		/// 	float startX = 0, startY = 0;
		/// 
		/// 	for (int i = 0; i &lt; 4; i++)
		/// 	{
		/// 		switch (i)
		/// 		{
		/// 			case 0:
		/// 				c = Color.Red;
		/// 				startX = 0; startY = 0;
		/// 				direct = BorderDirection.Top;
		/// 				break;
		/// 			case 1:
		/// 				c = Color.Green;
		/// 				startX = 100; startY = 0;
		/// 				direct = BorderDirection.Right;
		/// 				break;
		/// 			case 2:
		/// 				c = Color.Blue;
		/// 				startX = 100; startY = 100;
		/// 				direct = BorderDirection.Bottom;
		/// 				break;
		/// 			case 3:
		/// 				c = Color.Black;
		/// 				startX = 0; startY = 100;
		/// 				direct = BorderDirection.Left;
		/// 				break;
		/// 		}
		/// 
		/// 		DrawBorder(g, c, Width, BorderWidth,
		/// 			startX, startY, direct);
		/// 	}
		/// }
		/// </example>
		private void DrawBorder(Graphics g, Color c, 
			float startX, float startY, int Width, int BorderWidth, BorderDirection direct, HtmlBorderStyle BStyle)
		{
			RectangleF RectF = new RectangleF(0f, 0f, 0f, 0f);

			switch (BStyle)
			{
				case HtmlBorderStyle.solid:
				switch(direct)
				{
					case BorderDirection.Top:
						RectF.X = startX;
						RectF.Y = startY;
						RectF.Width = Width;
						RectF.Height = BorderWidth;
						break;
					case BorderDirection.Right:
						RectF.X = startX - BorderWidth;
						RectF.Y = startY;
						RectF.Width = BorderWidth;
						RectF.Height = Width;
						break;
					case BorderDirection.Bottom:
						RectF.X = startX;
						RectF.Y = startY - BorderWidth;
						RectF.Width = Width;
						RectF.Height = BorderWidth;
						break;
					case BorderDirection.Left:
						RectF.X = startX;
						RectF.Y = startY;
						RectF.Width = BorderWidth;
						RectF.Height = Width;
						break;
				}

					g.FillRectangle(new SolidBrush(c), RectF);
					break;
				case HtmlBorderStyle.double_:
					RectangleF RectF2 = new RectangleF(0f, 0f, 0f, 0f);
					//두개의 채워진 도형과 하나의 빈 도형을 그림. 
					//가장 많은 너비를 갖는 우선순위는 첫번째 채워진 도형, 두번째 채워진 도형, 빈 도형임.
					int b2 = (int)CMath.RoundDown(BorderWidth / 3M);
					int b3 = (int)CMath.RoundDown((BorderWidth - b2) / 2M);
					int b1 = BorderWidth - b3 - b2;
					
				switch(direct)
				{
					case BorderDirection.Top:
						RectF.X = startX;
						RectF.Y = startY;
						RectF.Width = Width;
						RectF.Height = b1;

						RectF2.X = startX;
						RectF2.Y = startY + b1 + b2;
						RectF2.Width = Width;
						RectF2.Height = b3;

						break;
					case BorderDirection.Right:
						RectF.X = startX - BorderWidth;
						RectF.Y = startY;
						RectF.Width = b1;
						RectF.Height = Width;

						RectF2.X = startX - b3;
						RectF2.Y = startY;
						RectF2.Width = b3;
						RectF2.Height = Width;

						break;
					case BorderDirection.Bottom:
						RectF.X = startX;
						RectF.Y = startY - BorderWidth;
						RectF.Width = Width;
						RectF.Height = b1;

						RectF2.X = startX;
						RectF2.Y = startY - b3;
						RectF2.Width = Width;
						RectF2.Height = b3;
							
						break;
					case BorderDirection.Left:
						RectF.X = startX;
						RectF.Y = startY;
						RectF.Width = BorderWidth;
						RectF.Height = Width;

						RectF2.X = startX + b1 + b2;
						RectF2.Y = startY;
						RectF2.Width = b3;
						RectF2.Height = Width;

						break;
				}

					g.FillRectangles(new SolidBrush(c), new RectangleF[2]{RectF, RectF2});
					break;
			}
		}

		public HtmlStyles HtmlStyles
		{
			get {return mHtmlStyles;}
		}


		public Elements Elements
		{
			get {return mElements;}
		}
	}

	public class Elements
	{
		private Hashtable mht;
		private List<string> KeyIndex;

		public Elements()
		{
			mht = new Hashtable();
			KeyIndex = new List<string>();
		}

		public int Count
		{
			get {return mht.Count;}
		}

		public object this [string Key]
		{
			get {return mht[Key];}
			set {mht[Key] = value;}
		}
		public object this [int Index]
		{
			get 
			{
				string Key = KeyIndex[Index];
				return mht[Key];
			}
			set 
			{
				string Key = KeyIndex[Index];
				mht[Key] = value;
			}
		}

		/*
		public Table this [string Key]
		{
			get {return (Table)mht[Key];}
			set {mht[Key] = value;}
		}
		public Table this [int Index]
		{
			get 
			{
				string Key = KeyIndex[Index];
				return (Table)mht[Key];
			}
			set 
			{
				string Key = KeyIndex[Index];
				mht[Key] = value;
			}
		}
		*/

		public object Add(object o)
		{
			string Id = System.Guid.NewGuid().ToString();
			mht.Add(Id, o);
			KeyIndex.Add(Id);

			return o;
		}
		public Table Add(Table t, int Rows, int Cols)
		{
			mht.Add(t.Id, t);
			KeyIndex.Add(t.Id);

			return Table.AppendRowCol(t, Rows, Cols);
		}
		public void Remove(string Key)
		{
			mht.Remove(Key);
			KeyIndex.Remove(Key);
		}
	}

	public class Table
	{
		public HtmlStyle Style = new HtmlStyle();

		public string Id;
		public HAlign Align;
		public int Border;
		public Color BorderColor = Color.Gray;
		public int Width;
		public int CellPadding = 1;
		public int CellSpacing = 2;

		//height

		private CRows mRows;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Id">테이블의 고유 ID, 중복시 에러남.</param>
		/// <param name="Width">테이블의 너비, -1이면 100%를 의미함.</param>
		public Table(string Id, int Width)
		{
			this.Id = Id;
			this.Width = Width;

			mRows = new CRows();
		}
		public Table(int Width)
		{
			this.Id = Guid.NewGuid().ToString();
			this.Width = Width;
		}

		public CRows Rows
		{
			get {return mRows;}
		}

		public static Table AppendRowCol(Table t, int Rows, int Cols)
		{
			for (int rw = 0; rw < Rows; rw++)
			{
				CRow row = t.Rows.Add(new CRow("r" + rw.ToString()));

				for (int cl = 0; cl < Cols; cl++)
				{
					Column col = row.Columns.Add(new Column("c" + cl.ToString()));
				}
			}

			return t;
		}
	}

	
	public class CRows
	{
		private Hashtable mht;
		private List<string> KeyIndex;

		public CRows()
		{
			mht = new Hashtable();
			KeyIndex = new List<string>();
		}

		public int Count
		{
			get {return mht.Count;}
		}

		public CRow this [string Key]
		{
			get
			{
				return (CRow)mht[Key];
			}
			set 
			{
				mht[Key] = value;
			}
		}
		public CRow this [int Index]
		{
			get 
			{
				string Key = KeyIndex[Index];
				return (CRow)mht[Key];
			}
			set 
			{
				string Key = KeyIndex[Index];
				mht[Key] = value;
			}
		}

		public CRow Add(CRow c)
		{
			mht.Add(c.Id, c);
			KeyIndex.Add(c.Id);

			return c;
		}
		public void Remove(string Key)
		{
			mht.Remove(Key);
			KeyIndex.Remove(Key);
		}
	}

	public class CRow
	{
		public HtmlStyle Style = new HtmlStyle();

		public HAlign Align;
		public VAlign VAlign;
		public int Height;
		public string Id;

		private Columns mColumns;
		//rowspan

		public CRow(string Id)
		{
			this.Id = Id;

			this.Align = CHtmlParser.GetDefaultHAlign();
			this.VAlign = CHtmlParser.GetDefaultVAlign();
			
			this.mColumns = new Columns();
		}

		public Columns Columns
		{
			get {return mColumns;}
		}
	}

	
	public class Columns
	{
		private Hashtable mht;
		private List<string> KeyIndex;

		public Columns()
		{
			mht = new Hashtable();
			KeyIndex = new List<string>();
		}

		public int Count
		{
			get {return mht.Count;}
		}

		public Column this [string Key]
		{
			get 
			{
				return (Column)mht[Key];
			}
			set 
			{
				mht[Key] = value;
			}
		}
		public Column this [int Index]
		{
			get 
			{
				string Key = KeyIndex[Index];
				return (Column)mht[Key];
			}
			set 
			{
				string Key = KeyIndex[Index];
				mht[Key] = value;
			}
		}

		public Column Add(Column c)
		{
			mht.Add(c.Id, c);
			KeyIndex.Add(c.Id);

			return c;
		}
		public void Remove(string Key)
		{
			mht.Remove(Key);
			KeyIndex.Remove(Key);
		}
		public void Remove(int Index)
		{
			string Key = this[Index].Id;
			Remove(Key);
		}
	}

	public class Column
	{
		public HtmlStyle Style = new HtmlStyle();

		public HAlign Align;
		public VAlign VAlign;
		public int Width;
		public bool NoWrap;
		internal float TextHeight; //GetRowHeight에서 설정됨.
		public string Id;

		public int ColSpan;
		public int WidthBeforeSpan;

		private Values mValues;
		private string mInnerHtml;

		public Column(string Id, int Width)
		{
			this.Id = Id;

			this.Align = CHtmlParser.GetDefaultHAlign();
			this.VAlign = CHtmlParser.GetDefaultVAlign();
			this.Width = Width;
			this.ColSpan = 1;
			this.WidthBeforeSpan = 0;

			this.mValues = new Values();
		}
		public Column(string Id)
		{
			this.Id = Id;

			this.Align = CHtmlParser.GetDefaultHAlign();
			this.VAlign = CHtmlParser.GetDefaultVAlign();
			this.Width = 0;

			this.mValues = new Values();
		}

		public string InnerHtml
		{
			get {return mInnerHtml;}
			set
			{
				CHtmlParser hp = new CHtmlParser();
				HtmlNodes nodes = hp.ParseHtml(value);

				for (int i = 0, i2 = nodes.Count; i < i2; i++)
				{
					HtmlNode node = nodes[i];
					NameValueCollection nvAttrs = node.AttributeList;
					Value v = new Value("");
					//<img>, <FONT SIZE="" COLOR="" FACE="">, <B>, <I>, <U>, <STRIKE>, <BR>, <P>
					switch (node.NodeName.ToLower())
					{
						case "img":
							string Src = nvAttrs["src"];
							if (Src.StartsWith("http://") || Src.StartsWith("www."))
							{
								WebClient client = new WebClient();

								try {v.Image = Image.FromStream(client.OpenRead(Src));}
								catch (Exception) {}
							}
							else
							{
								try {v.Image = Image.FromFile(Src);}
								catch (Exception) {}
							}
						
							break;
						case "font":
							float Size = 0;
							try {Size = Convert.ToInt32(nvAttrs["size"]);}
							catch (Exception) {}
							if (Size != 0)
							{
								Size = CHtmlParser.GetEmSizeByFontSize(Convert.ToInt32(Size));
							}
							else
							{
								Size = CHtmlParser.GetDefaultFontSize();
							}
						
							Color Color = Color.Transparent;
							try {Color = CColorConv.GetColorByHexaOrNamedColor(nvAttrs["color"]);}
							catch (Exception) {}
							if (Color == Color.Transparent)
							{
								Color = CHtmlParser.GetDefaultColor();
							}

							string Name = "";
							try {Name = CHtmlParser.GetFontNameByFontFamily(nvAttrs["name"]);}
							catch (Exception) {}
							if (Name == "")
							{
								Name = CHtmlParser.GetDefaultFontName();
							}

							v.Style.Font = new Font(Name, Size);
							v.Style.Color = Color;
							v.Text = node.NodeValue;

							break;
						case "b":
							v.Style.FontStyle = FontStyle.Bold;
							v.Text = node.NodeValue;
							break;
						case "i":
							v.Style.FontStyle = FontStyle.Italic;
							v.Text = node.NodeValue;
							break;
						case "u":
							v.Style.FontStyle = FontStyle.Underline;
							v.Text = node.NodeValue;
							break;
						case "strike":
							v.Style.FontStyle = FontStyle.Strikeout;
							v.Text = node.NodeValue;
							break;
						case "br":
							v.Text = "\n";
							break;
						case "p":
							v.Text = "\n\n";
							break;
						case "":
							//태그가 없는 일반 문자열인 경우.
							v.Text = node.NodeValue;
							break;
						default:
							throw new Exception(node.NodeName + "은 지원되지 않는 태그입니다.");
					} //switch

					this.Values.Add(v);
				} //nodes.Count

				this.mInnerHtml = value;
			} //set
		} //InnerHtml

		public Values Values
		{
			get {return mValues;}
		}
	}

	
	public class Values
	{
		private List<Value> mAry;

		public Values()
		{
			mAry = new List<Value>();
		}

		public int Count
		{
			get {return mAry.Count;}
		}

		public Value this [int Index]
		{
			get 
			{
				return mAry[Index];
			}
			set 
			{
				mAry[Index] = value;
			}
		}

		public Value Add(Value c)
		{
			mAry.Add(c);

			return c;
		}
		public void Remove(int Index)
		{
			mAry.Remove(mAry[Index]);
		}
	}

	public class Value
	{
		public HtmlStyle Style = new HtmlStyle();
		public string Text = "";
		public Image Image = null;
		public Table Table = null;

		public Value(string Text)
		{
			this.Text = Text;
		}
		public Value(int Num)
		{
			this.Text = Num.ToString();
		}
		public Value(Image Image)
		{
			this.Image = Image;
		}
		public Value(Table t, int Rows, int Cols)
		{
			this.Table = Table.AppendRowCol(t, Rows, Cols);
		}
	}

	/// <summary>
	/// br 태그를 의미함.
	/// </summary>
	public class Br { }
	/// <summary>
	/// p 태그를 의미함.
	/// </summary>
	public class P { }

	public enum HAlign
	{
		Left, Center, Right
	}
	public enum VAlign
	{
		Top, Middle, Bottom
	}
	public enum BorderDirection
	{
		Top, Right, Bottom, Left
	}
}
