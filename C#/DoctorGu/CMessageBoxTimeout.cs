using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DoctorGu
{
	//http://stackoverflow.com/a/14522952/2958717
	public class CMessageBoxTimeout
	{
		private const int WM_CLOSE = 0x0010;

		[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		private System.Threading.Timer _tmrTimeout;
		private string _Caption;
		private MessageBoxButtons _Button;
		private MessageBoxDefaultButton? _DefaultButton;
		private DialogResult? _Ret;

		private CMessageBoxTimeout(int TimeoutInSeconds, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon, MessageBoxDefaultButton? DefaultButton)
		{
			_Caption = Caption;
			_Button = Buttons;
			_DefaultButton = DefaultButton;
			_tmrTimeout = new System.Threading.Timer(OnTimerElapsed, null, TimeoutInSeconds * 1000, System.Threading.Timeout.Infinite);

			DialogResult dret;
			if (_DefaultButton != null)
				dret = MessageBox.Show(Text, Caption, Buttons, Icon, DefaultButton.Value);
			else
				dret = MessageBox.Show(Text, Caption, Buttons, Icon);

			//Timer에서 SendMessage로 닫혔다면 _Ret에 값이 설정되므로 그 값이 없을 때만 _Ret에 값을 설정함.
			if (_Ret == null)
				_Ret = dret;
		}
		private CMessageBoxTimeout(int TimeoutInSeconds, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon)
			: this(TimeoutInSeconds, Text, Caption, Buttons, Icon, null)
		{
		}

		public static DialogResult Show(int TimeoutInSeconds, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon, MessageBoxDefaultButton DefaultButton)
		{
			CMessageBoxTimeout msg = new CMessageBoxTimeout(TimeoutInSeconds, Text, Caption, Buttons, Icon, DefaultButton);
			return msg._Ret.Value;
		}
		public static DialogResult Show(int TimeoutInSeconds, string Text, string Caption, MessageBoxButtons Buttons, MessageBoxIcon Icon)
		{
			CMessageBoxTimeout msg = new CMessageBoxTimeout(TimeoutInSeconds, Text, Caption, Buttons, Icon);
			return msg._Ret.Value;
		}

		private void OnTimerElapsed(object state)
		{
			IntPtr hWnd = FindWindow(null, _Caption);
			if (hWnd != IntPtr.Zero)
			{
				//사용자가 선택하지 않았다면 DefaultButton을 선택한 것으로 간주함.
				//SendMessage로 창을 닫으면 호출한 곳에서 다음의 코드가 실행되므로 SendMessage 전에 _Ret에 값을 저장해야 하므로 순서 바꾸면 안됨.
				if (_DefaultButton != null)
				{
					switch (_Button)
					{
						case MessageBoxButtons.AbortRetryIgnore:
							switch (_DefaultButton)
							{
								case MessageBoxDefaultButton.Button1: _Ret = DialogResult.Abort; break;
								case MessageBoxDefaultButton.Button2: _Ret = DialogResult.Retry; break;
								case MessageBoxDefaultButton.Button3: _Ret = DialogResult.Ignore; break;
							}
							break;
						case MessageBoxButtons.OK:
							switch (_DefaultButton)
							{
								case MessageBoxDefaultButton.Button1: _Ret = DialogResult.OK; break;
								case MessageBoxDefaultButton.Button2: _Ret = DialogResult.Cancel; break;
								case MessageBoxDefaultButton.Button3: _Ret = DialogResult.Cancel; break;
							}
							break;
						case MessageBoxButtons.OKCancel:
							switch (_DefaultButton)
							{
								case MessageBoxDefaultButton.Button1: _Ret = DialogResult.OK; break;
								case MessageBoxDefaultButton.Button2: _Ret = DialogResult.Cancel; break;
								case MessageBoxDefaultButton.Button3: _Ret = DialogResult.Cancel; break;
							}
							break;
						case MessageBoxButtons.RetryCancel:
							switch (_DefaultButton)
							{
								case MessageBoxDefaultButton.Button1: _Ret = DialogResult.Retry; break;
								case MessageBoxDefaultButton.Button2: _Ret = DialogResult.Cancel; break;
								case MessageBoxDefaultButton.Button3: _Ret = DialogResult.Cancel; break;
							}
							break;
						case MessageBoxButtons.YesNo:
							switch (_DefaultButton)
							{
								case MessageBoxDefaultButton.Button1: _Ret = DialogResult.Yes; break;
								case MessageBoxDefaultButton.Button2: _Ret = DialogResult.No; break;
								case MessageBoxDefaultButton.Button3: _Ret = DialogResult.Cancel; break;
							}
							break;
						case MessageBoxButtons.YesNoCancel:
							switch (_DefaultButton)
							{
								case MessageBoxDefaultButton.Button1: _Ret = DialogResult.Yes; break;
								case MessageBoxDefaultButton.Button2: _Ret = DialogResult.No; break;
								case MessageBoxDefaultButton.Button3: _Ret = DialogResult.Cancel; break;
							}
							break;
						default:
							break;
					}
				}

				SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
			}

			_tmrTimeout.Dispose();
		}
	}
}
