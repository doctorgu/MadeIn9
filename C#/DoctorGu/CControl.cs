using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DoctorGu
{
	/// <summary>
	/// Windows Form에서 쓰이는 각종 컨트롤에 대한 기능 구현.
	/// </summary>
	public class CControl
	{
		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hwnd, int msg, IntPtr hdc, int flags);

		/// <summary>
		/// <paramref name="Parent"/>의 하위 컨트롤들을 배열 형식으로 리턴함.
		/// </summary>
		/// <param name="Parent">기준이 되는 부모 컨트롤</param>
		/// <returns><paramref name="Parent"/> 컨트롤의 모든 하위 컨트롤</returns>
		/// <example>
		/// 다음은 groupBox1 컨트롤 안에 label1, button1 컨트롤이 있는 경우에 해당 컨트롤의 이름을 출력합니다.
		/// <code>
		/// <![CDATA[
		/// private void Form1_Load(object sender, EventArgs e)
		/// {
		///	 Control[] aSub = CControl.GetControls(groupBox1);
		///	 string s = "";
		///	 for (int i = 0, i2 = aSub.Length; i < i2; i++)
		///	 {
		///		 s += "," + aSub[i].Name;
		///	 }
		///	 Console.WriteLine(s); //,label1,button1
		/// }
		/// ]]>
		/// </code>
		/// </example>
		public static Control[] GetControls(Control Parent)
		{
			List<Control> aCtls = new List<Control>();
			aCtls = GetControls(Parent, ref aCtls);

			return aCtls.ToArray();
		}

		/// <summary>
		/// <paramref name="Parent"/>의 하위 컨트롤들을 배열 형식으로 리턴함.
		/// 이때 <paramref name="ThisTypeOnly"/> 형식의 컨트롤만 리턴값에 포함시킴.
		/// </summary>
		/// <param name="Parent">기준이 되는 부모 컨트롤</param>
		/// <param name="ThisTypeOnly">리턴값에 포함될 컨트롤의 형식</param>
		/// <returns><paramref name="Parent"/> 컨트롤의 모든 하위 컨트롤</returns>
		/// <example>
		/// 다음은 groupBox1 컨트롤 안에 label1, button1 컨트롤이 있는 경우에, Label 형식인 label1 컨트롤의 이름을 출력합니다.
		/// <code>
		/// <![CDATA[
		/// private void Form1_Load(object sender, EventArgs e)
		/// {
		///	 Control[] aSub = CControl.GetControls(groupBox1, typeof(Label));
		///	 string s = "";
		///	 for (int i = 0, i2 = aSub.Length; i < i2; i++)
		///	 {
		///		 s += "," + aSub[i].Name;
		///	 }
		///	 Console.WriteLine(s); //,label1
		/// }
		/// ]]>
		/// </code>
		/// </example>
		public static Control[] GetControls(Control Parent, System.Type ThisTypeOnly)
		{
			List<Control> aCtls = new List<Control>();
			aCtls = GetControls(Parent, ref aCtls, ThisTypeOnly);

			return aCtls.ToArray();
		}

		/// <summary>
		/// <paramref name="Parent"/>의 하위 컨트롤들을 배열 형식으로 리턴함.
		/// 이때 <paramref name="ThisTypeOnly"/> 형식의 컨트롤만 리턴값에 포함시키고,
		/// <paramref name="Except"/> 컨트롤은 <paramref name="ThisTypeOnly"/> 형식의 컨트롤이라도 제외시킴.
		/// </summary>
		/// <param name="Parent">기준이 되는 부모 컨트롤</param>
		/// <param name="ThisTypeOnly">리턴값에 포함될 컨트롤의 형식</param>
		/// <param name="Except">제외될 컨트롤</param>
		/// <returns><paramref name="Parent"/> 컨트롤의 모든 하위 컨트롤</returns>
		/// <example>
		/// 다음은 groupBox1 컨트롤 안에 label1, label2, button1 컨트롤이 있는 경우에, Label 형식이면서 label2가 아닌 label1 컨트롤의 이름을 출력합니다.
		/// <code>
		/// <![CDATA[
		/// private void Form1_Load(object sender, EventArgs e)
		/// {
		///	 Control[] aSub = CControl.GetControls(groupBox1, typeof(Label), label2);
		///	 string s = "";
		///	 for (int i = 0, i2 = aSub.Length; i < i2; i++)
		///	 {
		///		 s += "," + aSub[i].Name;
		///	 }
		///	 Console.WriteLine(s); //,label1
		/// }
		/// ]]>
		/// </code>
		/// </example>
		public static Control[] GetControls(Control Parent, System.Type ThisTypeOnly, Control Except)
		{
			List<Control> aCtls = new List<Control>();
			aCtls = GetControls(Parent, ref aCtls, ThisTypeOnly, Except);

			return aCtls.ToArray();
		}
		
		private static List<Control> GetControls(Control Parent, ref List<Control> aCtls)
		{
			foreach (Control ctl in Parent.Controls)
			{
				aCtls.Add(ctl);

				if (ctl.HasChildren)
				{
					aCtls = GetControls(ctl, ref aCtls);
				}
			}

			return aCtls;
		}
		private static List<Control> GetControls(Control Parent, ref List<Control> aCtls, System.Type ThisTypeOnly)
		{
			foreach (Control ctl in Parent.Controls)
			{
				if (ctl.GetType() == ThisTypeOnly)
				{
					aCtls.Add(ctl);
				}

				if (ctl.HasChildren)
				{
					aCtls = GetControls(ctl, ref aCtls, ThisTypeOnly);
				}
			}

			return aCtls;
		}
		private static List<Control> GetControls(Control Parent, ref List<Control> aCtls, System.Type ThisTypeOnly, Control Except)
		{
			foreach (Control ctl in Parent.Controls)
			{
				if ((ctl.GetType() == ThisTypeOnly) && (ctl != Except))
				{
					aCtls.Add(ctl);
				}

				if (ctl.HasChildren)
				{
					aCtls = GetControls(ctl, ref aCtls, ThisTypeOnly, Except);
				}
			}

			return aCtls;
		}

		public static Control FindControlDeep(Control Parent, string Name)
		{
			foreach (Control ctl in Parent.Controls)
			{
				if (ctl.Name == Name)
				{
					return ctl;
				}

				if (ctl.HasChildren)
				{
					Control ctlFound = FindControlDeep(ctl, Name);
					//찾지 못했는데 리턴하면 다음 ctl에 대한 검사를 못하므로 찾았을 때만 리턴함.
					if (ctlFound != null)
						return ctlFound;
				}
			}

			return null;
		}

		/// <summary>
		/// <paramref name="Parent"/> 컨트롤의 하위 컨트롤 중 <paramref name="Name"/> 이름을 가진 컨트롤을 리턴함.
		/// </summary>
		/// <param name="Parent">기준이 되는 상위 컨트롤</param>
		/// <param name="Name">하위 컨트롤 중에서 찾을 컨트롤의 이름</param>
		/// <returns>찾아진 컨트롤, 못 찾았을 경우 null을 리턴함.</returns>
		/// <example>
		/// <code>
		/// Control ctl = CControl.GetControlByName(this, "label2");
		/// Console.WriteLine(ctl.Name); //label2
		/// </code>
		/// </example>
		public static Control GetControlByName(Control Parent, string Name)
		{
			Control[] aCtl = GetControls(Parent);
			for (int i = 0, i2 = aCtl.Length; i < i2; i++)
			{
				if (aCtl[i].Name == Name)
				{
					return aCtl[i];
				}
			}

			return null;
		}

		/// <summary>
		/// 윈도우의 기본 이동키인 탭키가 아닌, 엔터키를 눌러 다음 컨트롤로 이동하게 함.
		/// </summary>
		/// <param name="Frm">엔터키 이동이 적용될 폼</param>
		/// <param name="Key">눌러진 키</param>
		/// <param name="Modifiers">Alt, Control, Shift 등의 키 정보</param>
		/// <param name="CtlNotAllowedToMoveFrom">이 컨트롤이 포커스를 가지면 엔터키를 눌러도 다른 컨트롤로 이동하지 않음.</param>
		/// <example>
		/// 다음은 폼이 로드될 때 KeyPreview 속성을 true로 지정하고,
		/// KeyDown 이벤트에서 엔터키로 이동할 수 있도록 함.
		/// <code>
		///	private void Form1_Load(object sender, System.EventArgs e)
		///	{
		///		if (!this.KeyPreview) this.KeyPreview = true;
		///	}
		///	private void Form1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		///	{
		///		CControl.MoveByEnterKey(this, e.KeyCode, e.Modifiers, new Control[]{listView1});
		///	}
		/// </code>
		/// </example>
		public static void MoveByEnterKey(Form Frm, Keys Key, Keys Modifiers, Control[] CtlNotAllowedToMoveFrom)
		{
			//Shift, Ctrl, Alt 등을 눌렀다면 빠져나감.
			if (Modifiers != Keys.None) return;

			if (Key != Keys.Return) return;
			
			Control ActiveControl = Frm.ActiveControl;
			if (!ActiveControl.TabStop) return;
			
			//다음으로 이동하지 않아야 할 컨트롤이면 빠져나감
			//(주로 Grid 종류)
			if (CtlNotAllowedToMoveFrom != null)
			{
				for (int i = 0, i2 = CtlNotAllowedToMoveFrom.Length; i < i2; i++)
				{
					if (ActiveControl == CtlNotAllowedToMoveFrom[i])
					{
						return;
					}
				}
			}

			ComboBox cbo = ActiveControl as ComboBox;
			if (cbo != null)
			{
				//펼쳐진 상태면 빠져나감.
				if (cbo.DroppedDown) return;
			}
			
			//특정 형식의 컨트롤로만 이동하게 함.
			Type[] AllowedTypes = new Type[]{typeof(TextBox), typeof(CheckBox), typeof(RadioButton),
												typeof(ListBox), typeof(CheckedListBox), typeof(ComboBox),
												typeof(DateTimePicker)};
			bool IsAllowed = false;
			for (int i = 0, i2 = AllowedTypes.Length; i < i2; i++)
			{
				if (ActiveControl.GetType() == AllowedTypes[i])
				{
					IsAllowed = true;
					break;
				}
			}
			if (!IsAllowed) return;

			SendKeys.SendWait("{TAB}");
		}

		/// <summary>
		/// 폼이나 컨트롤에 그려진 내용을 캡쳐하고 Bitmap 형식으로 리턴함.
		/// </summary>
		/// <param name="Width">캡쳐할 폼(컨트롤)의 너비</param>
		/// <param name="Height">캡쳐할 폼(컨트롤)의 높이</param>
		/// <param name="Handle">캡쳐할 폼(컨트롤)의 핸들</param>
		/// <returns>캡쳐된 Bitmap 형식의 이미지</returns>
		public static Bitmap GetCapturedBitmap(int Width, int Height, IntPtr Handle)
		{
			Bitmap bmp = new Bitmap(Width, Height);
			Graphics g = Graphics.FromImage(bmp);
			IntPtr hdc = g.GetHdc();

			try
			{
				SendMessage(Handle, 0x0317, hdc, 0x36);
			}
			finally
			{
				g.ReleaseHdc(hdc);
			}

			return bmp;
		}
	}
}
