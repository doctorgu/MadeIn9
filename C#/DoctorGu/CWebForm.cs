using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

/*
- Page Event Order
		
Constructor
AddParsedSubObject
DeterminePostBackMode
OnInit : Original value assigned(ViewState and posted values not applied)

LoadPageStateFromPersistenceMedium
LoadViewState
ProcessPostData1
OnLoad

ProcessPostData2
RaiseChangedEvents
RaisePostBackEvent
OnPreRender

SaveViewState
SavePageStateToPersistenceMedium
Render
OnUnload

- Master Page는 Run time 때는 Content Page의 Child Control로 취급됨.
Init 이벤트를 제외하고는 Content Page의 이벤트가 먼저 실행됨.
즉, Content Page의 Load 이벤트가 먼저 실행된 후, Master Page의 Load 이벤트가 실행됨.
그러므로 Master Page에서 먼저 실행되게 하려면 Master Page의 Init 이벤트에 코드를 넣어야 함.
*/

namespace DoctorGu
{
	public class CWebForm
	{
		public static int GetControlIndex(Control Parent, Control ctl)
		{
			for (int i = 0, i2 = Parent.Controls.Count; i < i2; i++)
			{
				if (Parent.Controls[i] == ctl)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// ContentPlaceHolder 안에 form을 만든 경우엔 이름으로 액세스를 할 수 없으므로 만듦.
		/// </summary>
		/// <param name="p"></param>
		/// <param name="Id"></param>
		/// <returns></returns>
		public static object GetFormControl(Page p, string Id)
		{
			for (int i = 0, i2 = p.Form.Controls.Count; i < i2; i++)
			{
				if (p.Form.Controls[i].ID == Id)
				{
					return p.Form.Controls[i];
				}
			}

			return null;
		}
	}
}
