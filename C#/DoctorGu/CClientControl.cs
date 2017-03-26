using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;

namespace DoctorGu
{
	public enum ClientControlPropertyTypes
	{
		InputText,
		TextArea,
		InputPassword,
		Select,
		InputHidden,
		InputRadio,
		InputCheckBox,
		Span,
		StyleBackgroundImage,
		StyleDisplay,
		StyleColor,
		ImgSrc,
	}

	/// <summary>
	/// ASP.Net에서 클라이언트의 컨트롤에 값을 입력하는 JScript 코드를 작성함.
	/// </summary>
	/// <example>
	/// 다음 코드는 회원이 [ID 기억] 체크박스를 클릭한 경우엔 회원의 아이디를 txtLogInId 컨트롤에 표시하고,
	/// chkRememberId 컨트롤을 선택합니다.
	/// <code>
	/// //쿠키에 IdRemembered 값이 있다면 그 값을 가져옴.
	/// string IdRememberd = "";
	/// try { IdRememberd = HttpContext.Current.Request.Cookies["IdRemembered"].Value; }
	/// catch (Exception) { }
	///
	/// if (IdRememberd != "")
	/// {
	///	 //개체 생성
	///	 CClientControl cc = new CClientControl();
	/// 
	///	 //&lt;input type="text" name="txtLogInId"&gt; 컨트롤에 IdRemembered 값을 입력하는 JScript 코드를 생성함.
	///	 cc[ClientControlPropertyTypes.InputText, "txtLogInId"] = IdRememberd;
	///	 //&lt;input type="checkbox" name="chkRememberId"&gt; 컨트롤에 "true" 값을 입력하는 JScript 코드를 생성함.
	///	 cc[ClientControlPropertyTypes.InputCheckBox, "chkRememberId"] = "true";
	/// 
	///	 //생성된 JScript 코드를 가져옴.
	///	 string Script = cc.GetScript(false);
	/// 
	///	 //페이지가 모두 로드된 후 스크립트가 실행되도록 함.
	///	 ClientScript.RegisterStartupScript(typeof(Page), "Test", Script);
	/// }
	/// </code>
	/// </example>
	public class CClientControl
	{
		private List<string> maControlId = new List<string>();
		private List<ClientControlPropertyTypes> maPropType = new List<ClientControlPropertyTypes>();
		private List<string> maValue = new List<string>();
		private List<int> maIndex = new List<int>();
		private List<string> maFindValue = new List<string>();
		private string mFocusedCtl = "";

		/// <summary>
		/// 
		/// </summary>
		public CClientControl()
		{

		}

		public void SetFocus(string Name)
		{
			this.mFocusedCtl = Name;
		}

		/// <summary>
		/// 클라이언트 컨트롤에 값을 지정하는 JScript 코드를 생성함. 쓰기 전용.
		/// </summary>
		/// <param name="PropType">클라이언트 컨트롤의 type(text, span 등...)</param>
		/// <param name="Name">클라이언트 컨트롤의 id</param>
		public string this[ClientControlPropertyTypes PropType, string Id]
		{
			set
			{
				this.maControlId.Add(Id);
				this.maPropType.Add(PropType);
				this.maValue.Add(value);
				this.maIndex.Add(-1);
				this.maFindValue.Add("");
			}
		}

		/// <summary>
		/// 클라이언트 컨트롤에 값을 지정하는 JScript 코드를 생성함.
		/// radio 컨트롤의 경우엔 같은 name을 가진 여러 개의 컨트롤이 있는 경우가 많으므로,
		/// 특정 컨트롤의 checked를 true로 설정해야 하는 경우에 몇번째 컨트롤인 지를 뜻하는 <paramref name="Index"/> 속성을 추가로 지정할 수 있도록 함.
		/// 쓰기 전용.
		/// </summary>
		/// <param name="PropType">클라이언트 컨트롤의 type(text, span 등...)</param>
		/// <param name="Name">클라이언트 컨트롤의 id</param>
		/// <param name="Index">같은 name을 가진 두 개 이상의 클라이언트 컨트롤이 존재할 경우 해당 컨트롤의 index</param>
		/// <example>
		/// 다음은 숫자로 구성된 회원의 캐릭터 종류를 가져오고 해당 캐릭터에 해당하는 radio 컨트롤의 checked 속성을 true로
		/// 지정하는 JScript를 생성함.
		/// <code>
		/// int CharKind = Convert.ToInt32(dr["char_kind"]);
		/// CClientControl cc = new CClientControl();
		/// cc[ClientControlPropertyTypes.InputRadio, "radCharKind", (CharKind - 1)] = "true";
		/// </code>
		/// </example>
		public string this[ClientControlPropertyTypes PropType, string Id, int Index]
		{
			set
			{
				this.maControlId.Add(Id);
				this.maPropType.Add(PropType);
				this.maValue.Add(value);
				this.maIndex.Add(Index);
				this.maFindValue.Add("");
			}
		}
		/// <summary>
		/// 클라이언트 컨트롤에 값을 지정하는 JScript 코드를 생성함.
		/// radio 컨트롤의 경우엔 같은 name을 가진 여러 개의 컨트롤이 있는 경우가 많으므로,
		/// 특정 컨트롤의 checked를 true로 설정해야 하는 경우에 <paramref name="FindValue"/> 속성을 지정해서 value 속성에 해당 값을 가진
		/// 컨트롤을 찾을 수 있도록 함.
		/// 쓰기 전용.
		/// </summary>
		/// <param name="PropType">클라이언트 컨트롤의 type(text, span 등...)</param>
		/// <param name="Name">클라이언트 컨트롤의 id</param>
		/// <param name="FindValue">같은 name을 가진 두 개 이상의 클라이언트 컨트롤이 존재할 경우 해당 컨트롤의 value 속성의 값</param>
		/// <example>
		/// <code>
		/// 다음은 선호하는 온라인 가맹점을 뜻하는 radFavOnBizKind2란 이름의 radio 컨트롤이 여러개가 있고,
		/// 각 radio 컨트롤에는 가맹점 코드가 value 속성의 값으로 있는 경우에,
		/// 해당 가맹점 코드를 가진 radio 컨트롤의 checked 속성을 true로 지정하는 JScript를 생성함.
		/// string FavOnBizKind2 = dr["fav_on_biz_kind2"].ToString();
		/// CClientControl cc = new CClientControl();
		/// cc[ClientControlPropertyTypes.InputRadio, "radFavOnBizKind2", FavOnBizKind2] = "true";
		/// </code>
		/// </example>
		public string this[ClientControlPropertyTypes PropType, string Id, string FindValue]
		{
			set
			{
				this.maControlId.Add(Id);
				this.maPropType.Add(PropType);
				this.maValue.Add(value);
				this.maIndex.Add(-1);
				this.maFindValue.Add(FindValue);
			}
		}

		/// <summary>
		/// 이전에 Indexer로 지정한 값들을 해석해서 실제 JScript 코드 문자열을 리턴함.
		/// </summary>
		/// <param name="AddScriptTag">JScript 코드 양쪽에 script 태그로 둘러쌓을 지 여부.</param>
		/// <returns>JScript 코드</returns>
		/// <example>
		/// 다음은 txtName 컨트롤에 '홍길동'을 지정하는 JScript를 출력함.
		/// <code>
		/// CClientControl cc = new CClientControl();
		/// cc["span", "spnName"] = "홍길동";
		/// string Stmt = cc.GetScript(false);
		/// string StmtWithTag = cc.GetScript(true);
		///
		/// //document.getElementById("spnName").innerText = "홍길동";
		/// Console.WriteLine(Stmt);
		///
		/// //&lt;script language="javascript"&gt;
		/// //document.getElementById("spnName").innerText = "홍길동";
		/// //&lt;/script&gt;
		/// Console.WriteLine(StmtWithTag);
		/// </code>
		/// </example>
		public string GetScript(bool AddScriptTag)
		{
			string s = "";

			if (AddScriptTag)
			{
				s += "<script language=\"javascript\" type=\"text/javascript\">\r\n";
			}

			for (int i = 0, i2 = this.maValue.Count; i < i2; i++)
			{
				ClientControlPropertyTypes PropType = this.maPropType[i];
				string Property = GetPropertyByType(PropType);

				string Value = GetValueByType(PropType, i);
				string ControlId = this.maControlId[i];

				if ((this.maIndex[i] == -1) && (this.maFindValue[i] == ""))
				{
					s += "document.getElementById(\"" + ControlId + "\")." + Property + " = " + Value + ";\r\n";
				}
				else
				{
					if (this.maIndex[i] != -1)
					{
						int Index = Convert.ToInt32(this.maIndex[i]);
						s += "document.getElementById(\"" + ControlId + "\")[" + Index.ToString() + "]" + "." + Property + " = " + Value + ";\r\n";
					}
					else
					{
						s += "for (var _i = 0; _i < document.getElementById(\"" + ControlId + "\").length; _i++)\r\n";
						s += "{\r\n";
						s += "	if (document.getElementById(\"" + ControlId + "\")[_i].value == '" + this.maFindValue[i] + "')\r\n";
						s += "	{\r\n";
						s += "		document.getElementById(\"" + ControlId + "\")[_i]." + Property + " = " + Value + ";\r\n";
						s += "		break;\r\n";
						s += "	}\r\n";
						s += "}\r\n";
					}
				}
			}

			if (this.mFocusedCtl != "")
			{
				s += "document.getElementById(\"" + this.mFocusedCtl + "\").focus();\r\n";
			}

			if (AddScriptTag)
			{
				s += "</script>";
			}

			return s;
		}
		public string GetScript()
		{
			return GetScript(false);
		}

		private string GetValueByType(ClientControlPropertyTypes PropType, int Index)
		{
			string Value = "";
			if ((PropType == ClientControlPropertyTypes.InputRadio) || (PropType == ClientControlPropertyTypes.InputCheckBox))
			{
				Value = this.maValue[Index];
				if ((Value != "true") && (Value != "false"))
				{
					throw new Exception("radio, checkbox인 경우, 'true', 'false' 값만 허용됩니다.");
				}
			}
			else
			{
				Value = "\"" + CScript.ReplaceForScriptVariable(this.maValue[Index]) + "\"";
			}

			return Value;
		}

		private string GetPropertyByType(ClientControlPropertyTypes PropType)
		{
			string Property = "";
			switch (PropType)
			{
				case ClientControlPropertyTypes.InputText:
				case ClientControlPropertyTypes.TextArea:
				case ClientControlPropertyTypes.InputPassword:
				case ClientControlPropertyTypes.Select:
				case ClientControlPropertyTypes.InputHidden:
					Property = "value";
					break;
				case ClientControlPropertyTypes.StyleBackgroundImage:
					Property = "style.backgroundImage";
					break;
				case ClientControlPropertyTypes.StyleDisplay:
					Property = "style.display";
					break;
				case ClientControlPropertyTypes.StyleColor:
					Property = "style.color";
					break;
				case ClientControlPropertyTypes.ImgSrc:
					Property = "src";
					break;
				case ClientControlPropertyTypes.InputRadio:
				case ClientControlPropertyTypes.InputCheckBox:
					Property = "checked";
					break;
				case ClientControlPropertyTypes.Span:
					Property = "innerHTML";
					break;
				default:
					throw new Exception("PropType:" + PropType.ToString() + "은 허용되지 않습니다.");
			}

			return Property;
		}
	}
}