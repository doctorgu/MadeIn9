using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace DoctorGu
{
	/// <summary>
	/// HTML 코드를 파싱함. 다음의 예외가 있음.
	/// 0 font 태그로 시작해서 font 태그로 끝나도 끝난 것으로 인정됨.
	/// 0 태그 안에 태그가 있는 것은 감안하지 않음.
	/// </summary>
	/// <example>
	/// HtmlParser c = new HtmlParser();
	/// string s = "&lt;font style='font-family:굴림;font-size:12px;' color=red&gt;안녕&lt;/font&gt;"
	/// 	+ "\n&lt;b&gt;굵&lt;/b&gt;";
	/// 
	/// HtmlNodes nodes = c.ParseHtml(s);
	/// for (int i = 0, i2 = nodes.Count; i &lt; i2; i++)
	/// {
	/// 	HtmlNode node = (HtmlNode)nodes[i];
	/// 	
	/// 	string tmp = "&lt;" + node.NodeName;
	/// 	NameValueCollection nvAttr = node.AttributeList;
	/// 	for (int i = 0, i2 = nvAttr.Count; i &lt; i2; i++)
	/// 	{
	/// 		tmp += " " + nvAttr.GetKey(i) + "='" + nvAttr[i] + "'";
	/// 	}
	/// 	tmp += "&gt;" + node.NodeValue + "&lt;/" + node.NodeName + "&gt;";
	/// 	Console.WriteLine(tmp);
	/// }
	/// </example>
	public class CHtmlParser
	{
		private bool NormalTextStarted = false, NormalTextEnded = false;
		private bool NodeLeftStarted = false, NodeNameLeftEnded = false, NodeLeftEnded = false;
		private bool NodeRightStarted = false, NodeRightEnded = false;
		private bool NodeValueStarted = false, NodeValueEnded = false;
		private bool AttrStarted = false, AttrEnded = false;
		private bool AttrValueStarted = false, AttrValueEnded = false;
		private bool AttrEqualStarted = false;

		private string NormalText = "", NodeName = "", NodeValue = "", AttrName = "", AttrValue = "";
		private char AttrValueSep = 'x';

		private HtmlNodes nodes;
		private HtmlNode node;
		private NameValueCollection nvAttrs;

		public HtmlNodes ParseHtml(string Value)
		{
			bool Skip;

			nodes = new HtmlNodes();
			nvAttrs = new NameValueCollection();
			

			char[] c = Value.ToCharArray();

			for (int i = 0, i2 = c.Length; i < i2; i++)
			{
				if (c[i] == '<')
				{
					DoLt(c[i + 1], out Skip);
					if (Skip) continue;
				}
				else if (c[i] == ' ')
				{
					DoSpace();
				}
				else if (c[i] == '=')
				{
					if (AttrStarted && !AttrEnded)
					{
						//!!! style이면 style 파싱하기
						AttrEnded = true;
						AttrEqualStarted = true;
					}
				}
				else if ((c[i] == '\'') || (c[i] == '\"'))
				{
					DoQuot(c[i], out Skip);
					if (Skip) continue;
				}
				else if (c[i] == '>')
				{
					DoGt(out Skip);
					if (Skip) continue;
				}
				else if (c[i] == '/')
				{
					if (NodeValueEnded && !NodeRightStarted)
					{
						NodeRightStarted = true;
						continue;
					}
				}
				else if (Char.IsLetterOrDigit(c[i]))
				{
					DoLetterDigit();
				}

				
				if (NormalTextStarted && !NormalTextEnded)
				{
					NormalText += c[i];
				}
				else if (NodeLeftStarted && !NodeNameLeftEnded)
				{
					NodeName += c[i];
				}
				else if (AttrStarted && !AttrEnded)
				{
					AttrName += c[i];
				}
				else if (AttrValueStarted && !AttrValueEnded)
				{
					AttrValue += c[i];
				}
				else if (NodeValueStarted && !NodeValueEnded)
				{
					NodeValue += c[i];
				}
				else if (NodeRightEnded)
				{
					AddNode_InitNodeVar();
				}
			}

			if (NormalText != "")
			{
				AddNode_InitNodeVar();
			}

			return nodes;
		}
		private void AddNode_InitNodeVar()
		{
			if (NormalText != "")
			{
				node = new HtmlNode("", NormalText, nvAttrs);
				nodes.Add(node);
			}
			else
			{
				node = new HtmlNode(NodeName, NodeValue, nvAttrs);
				nodes.Add(node);
			}

			/*
			string tmp = "<" + NodeName;
			foreach (DictionaryEntry d in htAttrs)
			{
				tmp += " " + d.Key.ToString() + "='" + d.Value + "'";
			}
			tmp += ">" + NodeValue + "</" + NodeName + ">";
			Console.WriteLine(tmp);
			*/

			nvAttrs = new NameValueCollection();

			NormalTextStarted = false;
			NormalTextEnded = false;
			NodeLeftStarted = false;
			NodeNameLeftEnded = false;
			NodeLeftEnded = false;
			NodeValueStarted = false;
			NodeValueEnded = false;
			NodeRightStarted = false;
			NodeRightEnded = false;

			NormalText = "";		
			NodeName = "";
			NodeValue = "";
		}
		private void InitAttrVar()
		{
			AttrStarted = false;
			AttrEqualStarted = false;
			AttrValueStarted = false;
			AttrValueEnded = false;
			AttrEnded = false;
	
			AttrName = "";
			AttrValue = "";
			AttrValueSep = 'x';
		}
		/// <summary>
		/// 원인을 알수 없으나 이 코드를 안에 넣으면 if 문의 코드가 실행된 후
		/// 바로 다음의 else if 문의 코드가 실행되어 방지하기 위해 함수로 떼어냄.
		/// </summary>
		private void DoSpace()
		{
			if (AttrStarted && !AttrEnded)
			{
				AttrEnded = true;
			}
			else if (AttrValueStarted && !AttrValueEnded)
			{
				if (AttrValueSep == ' ')
				{
					AttrValueEnded = true;
					nvAttrs.Add(AttrName, AttrValue);
					//한 노드에 여러 개의 Attr이 가능하므로 이전 속성을 초기화시킴.
					InitAttrVar();
				}
			}
			else if (NodeLeftStarted && !NodeNameLeftEnded)
			{
				NodeNameLeftEnded = true;
			}
		}
		private void DoQuot(char c, out bool Skip)
		{
			Skip = false;

			if (AttrEqualStarted && !AttrValueStarted)
			{
				AttrValue = "";
				AttrValueStarted = true;
				AttrValueSep = c;
				Skip = true;
			}
			else if (AttrValueStarted && !AttrValueEnded)
			{
				if (AttrValueSep == c)
				{
					AttrValueEnded = true;
					nvAttrs.Add(AttrName, AttrValue);
					//한 노드에 여러 개의 Attr이 가능하므로 이전 속성을 초기화시킴.
					InitAttrVar();
				}
			}
		}
		private void DoLt(char charNext, out bool Skip)
		{
			Skip = false;

			if (charNext == '/')
			{
				NodeValueEnded = true;
			}
			else
			{
				//닫는 태그 없이 여는 태그가 시작되었다면 닫는 태그가 있는 것으로 가정함.
				//이때 닫는 태그가 없더라도 그때까지의 Value를 저장하게 허용함.
				if (NodeValueStarted && !NodeValueEnded)
				{
					AddNode_InitNodeVar();
				}
					//태그가 없이 문자열만 있는 경우
				else if (NormalTextStarted && !NormalTextEnded)
				{
					AddNode_InitNodeVar();
				}

				NodeLeftStarted = true;
				Skip = true;
			}
		}
		private void DoGt(out bool Skip)
		{
			Skip = false;

			if (NodeLeftStarted && !NodeValueStarted)
			{
				NodeNameLeftEnded = true;
				NodeLeftEnded = true;
				NodeValueStarted = true;

				if (AttrValueSep == ' ')
				{
					AttrValueEnded = true;
					nvAttrs.Add(AttrName, AttrValue);
					//한 노드에 여러 개의 Attr이 가능하므로 이전 속성을 초기화시킴.
					InitAttrVar();
				}

				Skip = true;
			}
			else if (NodeRightStarted && !NodeRightEnded)
			{
				NodeRightEnded = true;
			}
		}
		private void DoLetterDigit()
		{
			if (NodeNameLeftEnded && !AttrStarted && !NodeLeftEnded)
			{
				AttrStarted = true;
			}
			else if (AttrEqualStarted && !AttrValueStarted)
			{
				AttrValueStarted = true;
				AttrValueSep = ' ';
			}
			else if (!NodeLeftStarted)
			{
				NormalTextStarted = true;
			}
		}

		//		public HtmlStyles ParseStyle(string Value)
		//		{
		//			HtmlStyle st = new HtmlStyle();
		//
		//			char[] c = Value.ToCharArray();
		//
		//			for (int i = 0, i2 = c.Length; i < i2; i++)
		//			{
		//
		//			}
		//		}

		public static string GetFontNameByFontFamily(string fontFamilyName)
		{
			//실제로는 값에 대해 루핑을 하면서 시스템에 있는 글꼴임이
			//확인되면 그것을 리턴해야 하나 현재는 값의 첫번째 글꼴만
			//무조건 리턴함.
			string[] aFont = fontFamilyName.Split(',');
			return aFont[0];
		}

		/// <summary>
		/// FONT 태그의 SIZE 속성을 프로그램에서 쓸 수 있는 크기로 리턴함.
		/// </summary>
		/// <param name="Size"></param>
		/// <returns></returns>
		public static float GetEmSizeByFontSize(int fontSize)
		{
			float size = 0;

			if (fontSize > 7)
				fontSize = 7;
			else if (fontSize < 1)
				fontSize = 1;

			switch (fontSize)
			{
				case 1:
					size = 9; break;
				case 2:
					size = 13; break;
				case 3:
					size = 17; break;
				case 4:
					size = 19; break;
				case 5:
					size = 26; break;
				case 6:
					size = 32; break;
				case 7:
					size = 48; break;
			}

			return size;
		}

		#region GetDefault
		public static StringFormat GetDefaultFormat()
		{
			StringFormat sformat = new StringFormat();
			sformat.Alignment = StringAlignment.Near;
			//sformat.Trimming = StringTrimming.None;

			return sformat;
		}
		
		public static Font GetDefaultFont()
		{
			return new Font(GetDefaultFontName(), GetDefaultFontSize());
		}
		public static string GetDefaultFontName()
		{
			return "굴림";
		}
		public static float GetDefaultFontSize()
		{
			return 12;
		}

		public static Color GetDefaultBackgroundColor()
		{
			return Color.Transparent;
		}
		public static Color GetDefaultColor()
		{
			return Color.Black;
		}
		public static HAlign GetDefaultHAlign()
		{
			return HAlign.Left;
		}
		public static VAlign GetDefaultVAlign()
		{
			return VAlign.Middle;
		}
		public static HtmlBorder GetDefaultBorder()
		{
			HtmlBorder b = new HtmlBorder();
			b.Color = Color.Transparent;
			b.Style = HtmlBorderStyle.none;
			b.Width = 4;

			return b;
		}
		#endregion GetDefault
	}

	public class HtmlNodes
	{
		private Hashtable htNodes;
		private int Index = -1;

		public HtmlNodes()
		{
			htNodes = new Hashtable();
		}

		public int Count
		{
			get {return htNodes.Count;}
		}

		public HtmlNode this[int Index]
		{
			get {return (HtmlNode)htNodes[Index];}
		}

		public void Add(HtmlNode n)
		{
			Index++;
			htNodes.Add(Index, n);
		}
	}
	public class HtmlNode
	{
		private string mNodeName = "";
		private string mNodeValue = "";
		private NameValueCollection mnvAttrList = null;

		public HtmlNode(string NodeName, string NodeValue, NameValueCollection nvAttrList)
		{
			this.mNodeName = NodeName;
			this.mNodeValue = NodeValue;
			this.mnvAttrList = nvAttrList;
		}

		public string NodeName
		{
			get {return mNodeName;}
		}
		public string NodeValue
		{
			get {return mNodeValue;}
		}
		public NameValueCollection AttributeList
		{
			get {return mnvAttrList;}
		}
	}

	public class HtmlStyles
	{
		private Hashtable mht;
		private List<string> KeyIndex;

		public HtmlStyles()
		{
			mht = new Hashtable();
			KeyIndex = new List<string>();
		}

		public int Count
		{
			get {return mht.Count;}
		}

		public HtmlStyle this [string Key]
		{
			get
			{
				//return (HtmlStyle)mht[Key];
				//대소문자 구분을 하지 않도록 함.
				foreach (DictionaryEntry d in mht)
				{
					if (String.Compare(d.Key.ToString(), Key, true) == 0)
					{
						return (HtmlStyle)d.Value;
					}
				}

				return null;
			}
			set 
			{
				mht[Key] = value;
			}
		}
		public HtmlStyle this [int Index]
		{
			get 
			{
				string Key = KeyIndex[Index].ToString();
				return (HtmlStyle)mht[Key];
			}
			set 
			{
				string Key = KeyIndex[Index].ToString();
				mht[Key] = value;
			}
		}

		public HtmlStyle Add(HtmlStyle c, string Id)
		{
			mht.Add(Id, c);
			KeyIndex.Add(Id);

			return c;
		}
		public void Remove(string Key)
		{
			mht.Remove(Key);
			KeyIndex.Remove(Key);
		}

		internal HtmlStyle Clone()
		{
			return (HtmlStyle)this.MemberwiseClone();
		}
	}

	public class HtmlStyle
	{
		private System.Drawing.Font mFont;
		private StringFormat mFormat;
		private Color mBackgroundColor;
		private Color mColor;
		private HtmlBorder mBorderTop;
		private HtmlBorder mBorderRight;
		private HtmlBorder mBorderBottom;
		private HtmlBorder mBorderLeft;


		//Style은 테이블 내에서 상속되므로 사용자가 설정했는 지 여부를 알아야 함.
		//단 Border 관련 Style은 상속되지 않으므로 상관하지 않음.
		internal bool FontNameSetted;
		internal bool FontSizeSetted;
		internal bool FontStyleSetted;
		internal bool FormatAlignmentSetted;
		internal bool FormatTrimmingSetted;
		internal bool BackgroundColorSetted;
		internal bool ColorSetted;
		

		public HtmlStyle()
		{
			this.mFont = CHtmlParser.GetDefaultFont();
			this.mFormat = CHtmlParser.GetDefaultFormat();
			this.mBackgroundColor = CHtmlParser.GetDefaultBackgroundColor();
			this.mColor = CHtmlParser.GetDefaultColor();

			this.mBorderTop = CHtmlParser.GetDefaultBorder();
			this.mBorderRight = CHtmlParser.GetDefaultBorder();
			this.mBorderBottom = CHtmlParser.GetDefaultBorder();
			this.mBorderLeft = CHtmlParser.GetDefaultBorder();
		}

		public HtmlStyle Clone()
		{
			return (HtmlStyle)this.MemberwiseClone();
		}

		public Font Font
		{
			get {return mFont;}
			set
			{
				System.Drawing.Font newFont = value;

				FontNameSetted = (mFont.Name != newFont.Name);
				FontSizeSetted = (mFont.Size != newFont.Size);
				FontStyleSetted = (mFont.Style != newFont.Style);

				mFont = value;
			}
		}
		public string FontName
		{
			get {return mFont.Name;}
			set
			{
				if (mFont != null)
					mFont = new Font(value, mFont.Size, mFont.Style);
				else
					mFont = new Font(value, CHtmlParser.GetDefaultFontSize());

				FontNameSetted = true;
			}
		}
		public float FontSize
		{
			get {return mFont.Size;}
			set
			{
				if (mFont != null)
					mFont = new Font(mFont.Name, value, mFont.Style);
				else
					mFont = new Font(CHtmlParser.GetDefaultFontName(), value);

				FontSizeSetted = true;
			}
		}
		public FontStyle FontStyle
		{
			get {return mFont.Style;}
			set
			{
				if (mFont != null)
					mFont = new Font(mFont.Name, mFont.Size, value);
				else
					mFont = new Font(CHtmlParser.GetDefaultFontName(), CHtmlParser.GetDefaultFontSize(), value);

				FontStyleSetted = true;
			}
		}

		public StringFormat Format
		{
			get {return mFormat;}
			set
			{
				StringFormat newFormat = value;

				FormatAlignmentSetted = (mFormat.Alignment != newFormat.Alignment);
				FormatTrimmingSetted = (mFormat.Trimming != newFormat.Trimming);
				
				mFormat = value;
			}
		}
		public Color BackgroundColor
		{
			get {return mBackgroundColor;}
			set
			{
				mBackgroundColor = value;
				BackgroundColorSetted = true;
			}
		}
		public Color Color
		{
			get {return mColor;}
			set
			{
				mColor = value;
				ColorSetted = true;
			}
		}
		
		public HtmlBorder BorderTop
		{
			get {return mBorderTop;}
			set
			{
				mBorderTop = value;
			}
		}
		public HtmlBorder BorderRight
		{
			get {return mBorderRight;}
			set
			{
				mBorderRight = value;
			}
		}
		public HtmlBorder BorderBottom
		{
			get {return mBorderBottom;}
			set
			{
				mBorderBottom = value;
			}
		}
		public HtmlBorder BorderLeft
		{
			get {return mBorderLeft;}
			set
			{
				mBorderLeft = value;
			}
		}
		public void AddBorders(HtmlBorder BorderTop, HtmlBorder BorderRight,
			HtmlBorder BorderBottom, HtmlBorder BorderLeft)
		{
			if (BorderTop != null) mBorderTop = BorderTop;
			if (BorderRight != null) mBorderRight = BorderRight;
			if (BorderBottom != null) mBorderBottom = BorderBottom;
			if (BorderLeft != null) mBorderLeft = BorderLeft;
		}
	}

	public class HtmlBorder
	{
		private Color mColor;
		private int mWidth;
		private HtmlBorderStyle mStyle;
		private int mWidthReal;

		public Color Color
		{
			get {return mColor;}
			set
			{
				mColor = value;
			}
		}

		public int Width
		{
			get {return mWidth;}
			set
			{
				mWidth = value;
				if (mStyle != HtmlBorderStyle.none)
					mWidthReal = mWidth;
				else
					mWidthReal = 0;
			}
		}

		public HtmlBorderStyle Style
		{
			get {return mStyle;}
			set 
			{
				mStyle = value;
				if (mStyle != HtmlBorderStyle.none)
					mWidthReal = mWidth;
				else
					mWidthReal = 0;
			}
		}

		internal int WidthReal
		{
			get {return mWidthReal;}
		}

		public HtmlBorder Clone()
		{
			return (HtmlBorder)this.MemberwiseClone();
		}
	}

	public enum HtmlBorderStyle
	{
		none, dotted, dashed, solid, double_, groove, ridge, inset, outset
	}
}
