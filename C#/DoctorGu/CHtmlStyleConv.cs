using System;
using System.Drawing;

namespace DoctorGu
{
	public class CHtmlStyleConv
	{
		public static int GetPixcel(string Value)
		{
			string Value2 = Value.ToLower().Trim();

			if (Value2.EndsWith("px"))
			{
				Value2 = Value2.Substring(0, Value2.Length - 2);
			}

			int Num = 0;
			try {Num = Convert.ToInt32(Value2);}
			catch (Exception) {}

			return Num;
		}
		public static int GetPoint(string Value)
		{
			string Value2 = Value.ToLower().Trim();

			if (Value2.EndsWith("pt"))
			{
				Value2 = Value2.Substring(0, Value2.Length - 2);
			}

			int Num = 0;
			try {Num = Convert.ToInt32(Value2);}
			catch (Exception) {}

			return Num;
		}
		public static string GetFirstFontName(string fontFamily)
		{
			int PosComma = fontFamily.IndexOf(",");
			if (PosComma == -1) return fontFamily;

			return fontFamily.Substring(0, PosComma);
		}
		public static StringAlignment GetAlignment(string align)
		{
			switch (align)
			{
				case "left":
					return StringAlignment.Near;
				case "right":
					return StringAlignment.Far;
				case "center":
					return StringAlignment.Center;
				case "justify":
					return StringAlignment.Near;
			}

			throw new Exception(align + "는(은) 허용되지 않습니다.");
		}
		public static StringAlignment GetLineAlignment(string vAlign)
		{
			//auto Aligns the contents of an object according to the value of the layout-flow attribute. 
			//baseline Default. Aligns the contents of an object supporting VALIGN to the base line. 
			//sub Vertically aligns the text to subscript. 
			//super Vertically aligns the text to superscript. 
			//top Vertically aligns the contents of an object supporting VALIGN to the top of the object. 
			//middle Vertically aligns the contents of an object supporting VALIGN to the middle of the object. 
			//bottom Vertically aligns the contents of an object supporting VALIGN to the bottom of the object. 
			//text-top Vertically aligns the text of an object supporting VALIGN to the top of the object. 
			//text-bottom Vertically aligns the text of an object supporting VALIGN to the bottom of the object. 

			switch (vAlign)
			{
				case "sub":
				case "super":
				case "baseline":
				case "top":
				case "text-top":
					return StringAlignment.Near;
				case "auto":
				case "middle":
					return StringAlignment.Center;
				case "bottom":
				case "text-bottom":
					return StringAlignment.Far;
			}

			throw new Exception(vAlign + "는(은) 허용되지 않습니다.");
		}

		public static FontStyle GetFontStyle(string fontWeight, string fontStyle)
		{
			//* fontStyle
			//normal Default. Font is normal. 
			//italic Font is italic. 
			//oblique Font is italic.  

			//* fontWeight
			//normal Default. Font is normal. 
			//bold Font is bold. 
			//bolder Font is heavier than regular bold. 
			//lighter Font is lighter than normal. 
			//100 Font is at least as light as the 200 weight. 
			//200 Font is at least as bold as the 100 weight and at least as light as the 300 weight. 
			//300 Font is at least as bold as the 200 weight and at least as light as the 400 weight. 
			//400 Font is normal. 
			//500 Font is at least as bold as the 400 weight and at least as light as the 600 weight. 
			//600 Font is at least as bold as the 500 weight and at least as light as the 700 weight. 
			//700 Font is bold. 
			//800 Font is at least as bold as the 700 weight and at least as light as the 900 weight. 
			//900 Font is at least as bold as the 800 weight. 

			FontStyle fs = FontStyle.Regular;
			switch (fontWeight)
			{
				case "normal":
				case "lighter":
				case "100":
				case "200":
				case "300":
				case "400":
					fs = FontStyle.Regular;
					break;
				default:
					fs = FontStyle.Bold;
					break;
			}
			switch (fontStyle)
			{
				case "italic":
				case "oblique":
					fs &= FontStyle.Italic;
					break;
			}

			return fs;
		}
	}
}
