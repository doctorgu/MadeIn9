using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.ComponentModel;

namespace DoctorGu
{
	public class CTwoByte
	{
		/// <summary>
		/// 2바이트 문자열의 실제 길이를 2로 계산해서 왼쪽에서 Length만큼의 문자열을 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Length"></param>
		/// <param name="ModIs"></param>
		/// <returns></returns>
		/// <example>
		/// LeftH("한a글", 3) -> "한a"
		/// LeftH("한a글", 4, out ModIs) -> "한a", 이때 '글'의 왼쪽 부분이 남으므로 ModIs는 1이 됨.
		/// </example>
		public static string LeftH(string Value, int Length, out int ModIs)
		{
			ModIs = 0;

			int LenH = 0;
			for (int i = 0, j = Value.Length; i < j; i++)
			{
				char C = Value[i];
				LenH += (Is2ByteChar(C) ? 2 : 1);

				if (LenH == Length)
				{
					return Value.Substring(0, i + 1);
				}
				else if (LenH == (Length + 1))
				{
					ModIs = 1;
					return ((Length == 1) ? "" : Value.Substring(0, i));
				}
			}

			//Length가 문자열보다 긴 경우엔 문자열 자체를 리턴함.
			return Value;
		}
		public static string LeftH(string Value, int Length)
		{
			int ModIs;
			return LeftH(Value, Length, out ModIs);
		}
		/// <summary>
		/// 2바이트 문자열의 실제 길이를 2로 계산해서 오른쪽에서 Length만큼의 문자열을 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="Length"></param>
		/// <param name="ModIs"></param>
		/// <returns></returns>
		/// <example>
		/// RightH("한a글", 3, out ModIs) -> "a글"
		/// RightH("한a글", 4, out ModIs) -> "a글", 이때 '한'의 오른쪽 부분이 남으므로 ModIs는 1이 됨.
		/// </example>
		public static string RightH(string Value, int Length, out int ModIs)
		{
			ModIs = 0;

			int LenH = 0;
			for (int i = (Value.Length - 1); i >= 0; i--)
			{
				char C = Value[i];
				LenH += (Is2ByteChar(C) ? 2 : 1);

				if (LenH == Length)
				{
					return Value.Substring(i, (Value.Length - i));
				}
				else if (LenH == (Length + 1))
				{
					ModIs = 1;
					return ((Length == 1) ? "" : Value.Substring(i + 1, (Value.Length - (i + 1))));
				}
			}

			//Length가 문자열보다 긴 경우엔 문자열 자체를 리턴함.
			return Value;
		}
		public string RightH(string Value, int Length)
		{
			int ModIs;
			return RightH(Value, Length, out ModIs);
		}
		/// <summary>
		/// 2바이트 문자열의 실제 길이를 2로 계산해서 StartIndex부터 Length 길이만큼의 문자열을 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="StartIndex"></param>
		/// <param name="Length"></param>
		/// <param name="ModIs"></param>
		/// <returns></returns>
		public static string SubstringH(string Value, int StartIndex, int Length, out int ModIs)
		{
			ModIs = 0;

			//Length가 1인 경우엔 2바이트 문자열을 만났을 때 무한 루프를 돌게 됨
			if (Length <= 0)
				throw new Exception("Length가 0보다 작거나 같습니다.");

			int LenH = 0;
			int LenHCur = 0;
			string NewValue = "";
			int NewLen = 0;
			for (int i = 0, j = Value.Length; i < j; i++)
			{
				char C = Value[i];
				LenHCur = (Is2ByteChar(C) ? 2 : 1);
				LenH += LenHCur;

				if (LenH >= (StartIndex + 1))
				{
					NewValue += C;
					NewLen += LenHCur;

					if (NewLen == Length)
					{
						return NewValue;
					}
					else if (NewLen == (Length + 1))
					{
						ModIs = 1;
						return ((Length == 1) ? "" : NewValue.Substring(0, (NewValue.Length - 1)));
					}
				}
			}

			//Length가 문자열보다 긴 경우엔 문자열 자체를 리턴함.
			return NewValue;
		}
		public static string SubstringH(string Value, int StartIndex, int Length)
		{
			int ModIs;
			return SubstringH(Value, StartIndex, Length, out ModIs);
		}
		public static string SubstringH(string Value, int StartIndex)
		{
			int ModIs;
			int Length = (Value.Length * 2);
			return SubstringH(Value, StartIndex, Length, out ModIs);
		}
		
		/// <summary>
		/// 2바이트 문자열의 실제 길이를 2로 계산한 길이를 리턴함.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		/// <example>
		/// LenH("한a글") -> 5
		/// </example>
		public static int LenH(string Value)
		{
			int Length = 0;

			for (int i = 0, j = Value.Length; i < j; i++)
			{
				char C = Value[i];
				Length += (Is2ByteChar(C) ? 2 : 1);
			}

			return Length;
		}
		public static bool IsAll2ByteChar(string Value)
		{
			for (int i = 0, j = Value.Length; i < j; i++)
			{
				if (!Is2ByteChar(Value[i]))
				{
					return false;
				}
			}

			return true;
		}
		public static bool IsAll1ByteChar(string Value)
		{
			for (int i = 0, j = Value.Length; i < j; i++)
			{
				if (Is2ByteChar(Value[i]))
				{
					return false;
				}
			}

			return true;
		}

		public static bool Is2ByteChar(char c)
		{
			//Basic Latin http://msdn.microsoft.com/en-us/library/dd374090(VS.85).aspx 참조
			if (CLang.Between((int)c, 0, 127))
			{
				return false;
			}
			else
			{
				return true;
			}

			//정확하지 않아 주석.
			//UnicodeCategory cat = char.GetUnicodeCategory(c);
			//switch (cat)
			//{
			//    case UnicodeCategory.FinalQuotePunctuation: //’
			//    case UnicodeCategory.OtherLetter:
			//    case UnicodeCategory.OtherSymbol:
			//        return true;
			//}

			//return false;
		}
		public static bool Is2ByteChar(string Value, int Index)
		{
			return Is2ByteChar(Value[Index]);
		}

		/// <example>
		/// Console.WriteLine(RowSource.PadRightH("a한b", 2, 'x')); //ax
		/// Console.WriteLine(RowSource.PadRightH("한글", 2, 'x')); //한
		/// </example>
		public static string PadRightH(string Value, int TotalWidth, char PaddingChar)
		{
			int LenH = CTwoByte.LenH(Value);
			if (LenH > TotalWidth)
			{
				int ModIs;
				Value = CTwoByte.LeftH(Value, TotalWidth, out ModIs);
				LenH = CTwoByte.LenH(Value);
			}

			int nPad = TotalWidth - LenH;
			return Value + CFindRep.Repeat(PaddingChar, nPad);
		}
		/// <example>
		/// Console.WriteLine(RowSource.PadLeftH("a한b", 2, 'x')); //xa
		/// Console.WriteLine(RowSource.PadLeftH("한글", 2, 'x')); //한
		/// </example>
		public static string PadLeftH(string Value, int TotalWidth, char PaddingChar)
		{
			int LenH = CTwoByte.LenH(Value);
			if (LenH > TotalWidth)
			{
				int ModIs;
				Value = CTwoByte.LeftH(Value, TotalWidth, out ModIs);
				LenH = CTwoByte.LenH(Value);
			}

			int nPad = TotalWidth - LenH;
			return CFindRep.Repeat(PaddingChar, nPad) + Value;
		}
		/// <summary>
		/// 가운데 정렬 후 양쪽에 <paramref name="PaddingChar"/>를 채워서 <paramref name="TotalWidth"/> 길이를 맞춤.
		/// </summary>
		/// <param name="Value"></param>
		/// <param name="TotalWidth"></param>
		/// <param name="PaddingChar"></param>
		/// <returns></returns>
		/// <example>
		/// string s = CTwoByte.PadCenterH("안a녕", 8, 'x'); //"x안a녕xx"
		/// string s2 = CTwoByte.PadCenterH("12", 1, 'x'); //"12"
		/// string s3 = CTwoByte.PadCenterH("12", 3, 'x'); //"12x"
		/// </example>
		public static string PadCenterH(string Value, int TotalWidth, char PaddingChar)
		{
			int LenH = CTwoByte.LenH(Value);
			
			if (TotalWidth <= LenH)
				return Value;

			int Half = (int)((TotalWidth - LenH) / 2);
			int Mod = (TotalWidth - LenH) % 2;

			if (Half == 0)
			{
				return Value + CFindRep.Repeat(PaddingChar, Mod);
			}

			string Pad = CFindRep.Repeat(PaddingChar, Half);
			return Pad + Value + Pad + CFindRep.Repeat(PaddingChar, Mod);
		}

		/// <summary>
		/// 허용된 길이보다 길면 길이만큼만 표시하고 뒤에 ...을 붙임.
		/// </summary>
		/// <param name="AllValue"></param>
		/// <param name="Length"></param>
		/// <returns></returns>
		public static string TailDotDotDot(string AllValue, int Length)
		{
			return TailDotDotDot(AllValue, 0, Length);
		}
		/// <summary>
		/// 허용된 길이보다 길면 길이만큼만 표시하고 뒤에 ...을 붙임.
		/// </summary>
		/// <param name="AllValue"></param>
		/// <param name="Add">추가로 길이를 차지하는 이미지나 (3) 등의 너비</param>
		/// <param name="Length"></param>
		/// <returns></returns>
		public static string TailDotDotDot(string AllValue, int Add, int Length)
		{
			if (string.IsNullOrEmpty(AllValue))
				return "";

			if ((LenH(AllValue) + Add) > Length)
			{
				return LeftH(AllValue, (Length - Add)) + "...";
			}
			else
			{
				return AllValue;
			}
		}

		/// <summary>
		/// Unicode 문자열을 통신에서 전송시 깨지지 않도록 61진수 형태로 변환함.
		/// </summary>
		/// <param name="Text"></param>
		/// <param name="Delim"></param>
		/// <returns></returns>
		public static string EncryptUnicode(string Text, bool EncodeUrlParam)
		{
			const char Delim = '|';
			string All = "";

			string s = "";
			for (int i = 0, i2 = Text.Length; i < i2; i++)
			{
				char c = Text[i];
				if (CTwoByte.Is2ByteChar(c))
				{
					int AscCode = (int)c;
					string Num61 = CMath.GetNFrom10(AscCode, 61);
					s = Delim + Num61.PadRight(3);
				}
				else if (c == Delim)
				{
					s = Delim.ToString() + Delim.ToString();
				}
				else
				{
					s = EncodeUrlParam ? HttpUtility.UrlEncode(c.ToString()) : c.ToString();
				}

				All += s;
			}

			return All;
		}
		public static string EncryptUnicode(string Text)
		{
			return EncryptUnicode(Text, false);
		}
		/// <summary>
		/// 통신에서 전송시 깨지지 않도록 61진수 형태로 변환된 Unicode 문자열을 원래의 문자열로 변환함.
		/// </summary>
		/// <param name="Text"></param>
		/// <param name="Delim"></param>
		/// <returns></returns>
		public static string DecryptUnicode(string Text)
		{
			const char Delim = '|';

			string All = "";
			string s = "";
			for (int i = 0, i2 = Text.Length; i < i2; i++)
			{
				char c = Text[i];
				if (c == Delim)
				{
					if (Text.Substring(i + 1, 1) != Delim.ToString())
					{
						string Num61 = Text.Substring(i + 1, 3);
						int AscCode = (int)CMath.Get10FromN(Num61, 61);
						s = ((char)AscCode).ToString();
						i += 3;
					}
					else
					{
						s = c.ToString();
						i++;
					}
				}
				else
				{
					s = c.ToString();
				}

				All += s;
			}

			return All;
		}

		/// <summary>
		/// 같은 모양을 가지고 있으나 아스키코드가 틀린 한자 목록을 리턴함.
		/// </summary>
		/// <returns></returns>
		public static Dictionary<char, char> GetHanjaSameShapeDifferentAscii()
		{
			Dictionary<char, char> dicHanja = new Dictionary<char, char>();
			dicHanja.Add('不', '不');
			dicHanja.Add('肦', '朌');
			dicHanja.Add('車', '車');
			dicHanja.Add('喇', '喇');
			dicHanja.Add('裸', '裸');
			dicHanja.Add('臘', '臘');
			dicHanja.Add('廊', '廊');
			dicHanja.Add('浪', '浪');
			dicHanja.Add('來', '來');
			dicHanja.Add('盧', '盧');
			dicHanja.Add('老', '老');
			dicHanja.Add('菉', '菉');
			dicHanja.Add('壟', '壟');
			dicHanja.Add('籠', '籠');
			dicHanja.Add('賂', '賂');
			dicHanja.Add('樓', '樓');
			dicHanja.Add('縷', '縷');
			dicHanja.Add('陋', '陋');
			dicHanja.Add('樂', '樂');
			dicHanja.Add('寧', '寧');
			dicHanja.Add('若', '若');
			dicHanja.Add('量', '量');
			dicHanja.Add('閭', '閭');
			dicHanja.Add('麗', '麗');
			dicHanja.Add('轢', '轢');
			dicHanja.Add('年', '年');
			dicHanja.Add('撚', '撚');
			dicHanja.Add('列', '列');
			dicHanja.Add('廉', '廉');
			dicHanja.Add('捻', '捻');
			dicHanja.Add('寧', '寧');
			dicHanja.Add('玲', '玲');
			dicHanja.Add('靈', '靈');
			dicHanja.Add('領', '領');
			dicHanja.Add('了', '了');
			dicHanja.Add('硫', '硫');
			dicHanja.Add('慄', '慄');
			dicHanja.Add('栗', '栗');
			dicHanja.Add('吏', '吏');
			dicHanja.Add('李', '李');
			dicHanja.Add('痢', '痢');
			dicHanja.Add('裡', '裡');
			dicHanja.Add('里', '里');
			dicHanja.Add('璘', '璘');
			dicHanja.Add('淋', '淋');
			dicHanja.Add('立', '立');
			dicHanja.Add('笠', '笠');
			dicHanja.Add('粒', '粒');
			dicHanja.Add('煉', '煉');
			dicHanja.Add('裏', '裏');
			dicHanja.Add('龜', '龜');
			dicHanja.Add('林', '林');
			dicHanja.Add('率', '率');
			dicHanja.Add('練', '練');
			dicHanja.Add('淪', '淪');
			dicHanja.Add('屢', '屢');
			dicHanja.Add('暴', '暴');
			dicHanja.Add('亂', '亂');
			dicHanja.Add('燐', '燐');
			dicHanja.Add('梨', '梨');
			dicHanja.Add('賈', '賈');
			dicHanja.Add('倫', '倫');
			dicHanja.Add('壘', '壘');
			dicHanja.Add('麟', '麟');
			dicHanja.Add('度', '度');
			dicHanja.Add('金', '金');
			dicHanja.Add('黎', '黎');
			dicHanja.Add('匿', '匿');
			dicHanja.Add('吝', '吝');
			dicHanja.Add('論', '論');
			dicHanja.Add('牢', '牢');
			dicHanja.Add('蘆', '蘆');
			dicHanja.Add('朗', '朗');
			dicHanja.Add('律', '律');
			dicHanja.Add('劣', '劣');
			dicHanja.Add('隣', '隣');
			dicHanja.Add('塞', '塞');
			dicHanja.Add('落', '落');
			dicHanja.Add('輦', '輦');
			dicHanja.Add('陸', '陸');
			dicHanja.Add('羚', '羚');
			dicHanja.Add('念', '念');
			dicHanja.Add('聯', '聯');
			dicHanja.Add('拏', '拏');
			dicHanja.Add('嶺', '嶺');
			dicHanja.Add('鱗', '鱗');
			dicHanja.Add('料', '料');

			return dicHanja;
		}

		/// <summary>
		/// 같은 모양의 한자인 경우 다르게 처리되는 것을 막기 위해 Value로 변경함.
		/// </summary>
		/// <param name="Value"></param>
		/// <returns></returns>
		public static string ReplaceHanjaSameShape(string Value)
		{
			Dictionary<char, char> dicSame = GetHanjaSameShapeDifferentAscii();

			bool IsFound = false;
			StringBuilder sbValue = new StringBuilder();
			foreach (char c in Value)
			{
				if (dicSame.ContainsKey(c))
				{
					IsFound = true;
					sbValue.Append(dicSame[c]);
				}
				else
				{
					sbValue.Append(c);
				}
			}

			if (!IsFound)
				return Value;
			else
				return sbValue.ToString();
		}

		private static void WriteLength()
		{
			int From = 1;
			int To = 65510;

			Form f = new Form();
			Label lbl = new Label();
			lbl.AutoSize = true;
			lbl.Padding = new Padding(0);
			lbl.Font = new Font("굴림체", 10);
			f.Controls.Add(lbl);


			int OneOld = 0, OneCur = 0, OneFirst = 0;
			for (int i = From; i < To; i++)
			{
				string c = ((char)i).ToString();
				lbl.Text = c;
				int Width = lbl.Width;
				if (Width <= 12)
				{
					OneOld = OneCur;
					OneCur = i;

					if ((OneOld + 1) < OneCur)
					{
						string Line = Width.ToString() + "," + c + "," + OneFirst.ToString() + "~" + OneOld.ToString();
						CFile.AppendTextToFile("C:\\One.txt", Line);
						Debug.WriteLine(Line);

						OneFirst = OneCur;
					}
				}
			}

			MessageBox.Show("완료");
		}
	}

}
