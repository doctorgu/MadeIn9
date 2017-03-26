using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace DoctorGu
{
	/// <summary>
	/// 각 변수에 대한 자세한 정보
	/// </summary>
	public class CVarInfo
	{
		/// <summary>변수 이름</summary>
		public string Key = "";
		/// <summary>변수 태그를 포함한 문자열</summary>
		public string OuterText = "";
		/// <summary>변수 태그를 포함하지 않는 문자열</summary>
		public string InnerText = "";
		/// <summary>변수 태그의 시작 위치</summary>
		public int PosOuterStart = 0;
		/// <summary>변수 태그의 종료 위치</summary>
		public int PosOuterEnd = 0;
		/// <summary>변수 태그 안의 값의 시작 위치</summary>
		public int PosInnerStart = 0;
		/// <summary>변수 태그 안의 값의 종료 위치</summary>
		public int PosInnerEnd = 0;
	}

	/// <summary>
	/// &lt;--Name.Begin--&gt;홍길동&lt;--Name.End--&gt;와 같이 지정된 변수에 실제 값을 입력하기 위함.
	/// </summary>
	/// <example>
	/// 다음은 CTemplate 개체를 생성하고 ReplaceInner를 이용해서 태그 안의 값만 변경하고,
	/// ReplaceOuter를 이용해서 태그와 태그 안의 값을 변경하는 방법입니다.
	/// <code>
	/// string sTmp = "<!--Title.Begin-->알립니다.<!--Title.End--><!--Name.Begin-->관리자<!--Name.End-->";
	/// CTemplate Tmp = new CTemplate(sTmp, "Title", "Name");
	///
	/// string sInner = Tmp.ReplaceInner("회식 있음", "이순신");
	/// Console.WriteLine(sInner); //"<!--Title.Begin-->회식 있음<!--Title.End--><!--Name.Begin-->이순신<!--Name.End-->"
	///
	/// string sOuter = Tmp.ReplaceOuter("퇴근했음", "홍길동");
	/// Console.WriteLine(sOuter); //"퇴근했음홍길동"
	/// </code>
	/// </example>
	public class CTemplate
	{
		private string mTemplate;
		private DataTable mdt = null;
		private string[] maVarName = null;

		/// <summary>
		/// 생성자
		/// </summary>
		/// <param name="Template">&lt;--Name.Begin--&gt;홍길동&lt;--Name.End--&gt; 형식의 변수가 포함된 문자열</param>
		/// <param name="aVarName">변수 이름(&lt;--Name.Begin--&gt;홍길동&lt;--Name.End--&gt; 형식에서 변수 이름은 "Name")</param>
		public CTemplate(string Template, string[] aVarName)
		{
			this.mTemplate = Template;
			this.maVarName = aVarName;
			this.mdt = GetVarInfo(this.mTemplate, this.maVarName);
		}
		public CTemplate(string Template)
		{
			this.mTemplate = Template;
			this.maVarName = GetVarName(this.mTemplate);
			this.mdt = GetVarInfo(this.mTemplate, this.maVarName);
		}

		private string[] GetVarName(string Template)
		{
			List<string> aVarName = new List<string>();

			Regex r = new Regex(CRegex.Pattern.ExtractNameFromTemplate, RegexOptions.Compiled);
			for (Match m = r.Match(Template); m.Success; m = m.NextMatch())
			{
				if (m.Groups.Count == 0)
					continue;

				string Name = m.Groups["Name"].Value;
				if (aVarName.IndexOf(Name) == -1)
				{
					aVarName.Add(Name);
				}
			}

			return aVarName.ToArray();
		}

		private DataTable GetVarInfo(string Template, string[] aVarName)
		{
			DataTable dt = new DataTable();

			for (int i = 0, i2 = aVarName.Length; i < i2; i++)
			{
				dt.Columns.Add(aVarName[i], typeof(CVarInfo));
			}

			//Template의 규칙은 <!--VarName.Begin-->내용<!--VarName.End-->임.
			int PosBegin = -1, PosEnd = -1;
			bool IsFirstLine = true;
			while (true)
			{
				CVarInfo[] avi = new CVarInfo[dt.Columns.Count];
				bool IsFound = false;
				for (int cl = 0, i2 = dt.Columns.Count; cl < i2; cl++)
				{
					string VarName = dt.Columns[cl].ColumnName;

					string Begin = "<!--" + VarName + ".Begin-->";
					string End = "<!--" + VarName + ".End-->";

					//태그 안에 태그가 있을 수 있으므로 이전 Begin 위치 다음부터 현재 Begin을 찾음.
					int Pos = IsFirstLine ? (PosBegin + 1) : (PosBegin + Begin.Length);
					PosBegin = Template.IndexOf(Begin, Pos, StringComparison.CurrentCultureIgnoreCase);
					if (PosBegin == -1)
					{
						if (IsFirstLine)
						{
							throw new Exception(Begin + " 태그가 없거나 첫번째 위치에 " + Begin + " 태그가 존재하지 않습니다.");
						}

						break;
					}

					PosEnd = Template.IndexOf(End, PosBegin + 1, StringComparison.CurrentCultureIgnoreCase);
					if (PosEnd == -1)
					{
						throw new Exception(Begin + " 태그가 있으나 " + End + " 태그가 없습니다.");
					}
					PosEnd = PosEnd + End.Length - 1;

					avi[cl] = new CVarInfo();
					avi[cl].Key = VarName;
					avi[cl].OuterText = Template.Substring(PosBegin, PosEnd - PosBegin + 1);
					avi[cl].InnerText = avi[cl].OuterText.Substring(Begin.Length, avi[cl].OuterText.Length - Begin.Length - End.Length);
					avi[cl].PosOuterStart = PosBegin;
					avi[cl].PosOuterEnd = PosEnd;
					avi[cl].PosInnerStart = PosBegin + Begin.Length;
					avi[cl].PosInnerEnd = PosEnd - End.Length;

					IsFound = true;
				}
				IsFirstLine = false;

				if (IsFound)
				{
					dt.Rows.Add(avi);
				}
				else
				{
					break;
				}
			}

			return dt;
		}

		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열을 리턴함.
		/// 만약 DataTable을 가져와서 변수 값을 변경했거나, ReplaceInner나 ReplaceOuter를 이용해서 변수 값을 변경했다면
		/// 변경된 값이 적용된 Template 문자열을 리턴함.
		/// </summary>
		/// <param name="UseInner">true: 둘러싼 태그를 보존하고 그 안의 변수만 값으로 대체함. false: 둘러싼 태그와 변수를 값으로 대체함.</param>
		/// <returns>변경된 값이 적용된 최종 문자열</returns>
		/// <example>
		/// 다음은 두 개의 행을 가진 Title, Name 필드의 값을 각각 변경하는 과정입니다.
		/// 첫번째 행의 Title 필드는 초기값이 "알립니다1."에서 "회식 있음"으로 변경되었고,
		/// 두번째 행의 Title 필드는 최기값이 "알립니다2."에서 "퇴근했음"으로 변경되었습니다.
		/// <code>
		/// string sTmpMult = "<!--Title.Begin-->알립니다1.<!--Title.End--><!--Name.Begin-->관리자1<!--Name.End-->\r\n"
		///				+ "<!--Title.Begin-->알립니다2.<!--Title.End--><!--Name.Begin-->관리자2<!--Name.End-->";
		/// CTemplate TmpMult = new CTemplate(sTmpMult, "Title", "Name");
		/// 
		/// DataTable dtTmpMult = TmpMult.DataTable;
		/// 
		/// CVarInfo vi = null;
		/// 
		/// vi = (CVarInfo)dtTmpMult.Rows[0]["Title"];
		/// vi.OuterText = "회식 있음";
		/// vi = (CVarInfo)dtTmpMult.Rows[0]["Name"];
		/// vi.OuterText = "이순신";
		/// 
		/// vi = (CVarInfo)dtTmpMult.Rows[1]["Title"];
		/// vi.OuterText = "퇴근했음";
		/// vi = (CVarInfo)dtTmpMult.Rows[1]["Name"];
		/// vi.OuterText = "홍길동";
		/// 
		/// string sMultOuter = TmpMult.GetTemplate();
		/// Console.WriteLine(sMultOuter);
		/// //--결과
		/// //회식 있음이순신
		/// //퇴근했음홍길동
		/// </code>
		/// </example>
		public string GetTemplate(bool UseInner)
		{
			string Template = this.mTemplate;
			StringBuilder sbNew = new StringBuilder();

			int PosPrev = 0;
			foreach (DataRow dr in this.mdt.Rows)
			{
				foreach (DataColumn dc in this.mdt.Columns)
				{
					CVarInfo vi = (CVarInfo)dr[dc];

					if (!UseInner)
					{
						sbNew.Append(Template.Substring(PosPrev, (vi.PosOuterStart - PosPrev)));
						sbNew.Append(vi.OuterText);
						PosPrev = vi.PosOuterEnd + 1;
					}
					else
					{
						sbNew.Append(Template.Substring(PosPrev, (vi.PosInnerStart - PosPrev)));
						sbNew.Append(vi.InnerText);
						PosPrev = vi.PosInnerEnd + 1;
					}
				}
			}

			sbNew.Append(Template.Substring(PosPrev));

			return sbNew.ToString();
		}
		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열을 리턴함.
		/// 만약 DataTable을 가져와서 변수 값을 변경했거나, ReplaceInner나 ReplaceOuter를 이용해서 변수 값을 변경했다면
		/// 변경된 값이 적용된 Template 문자열을 리턴함.
		/// </summary>
		/// <returns>변경된 값이 적용된 최종 문자열</returns>
		/// <example>
		/// 다음은 두 개의 행을 가진 Title, Name 필드의 값을 각각 변경하는 과정입니다.
		/// 첫번째 행의 Title 필드는 초기값이 "알립니다1."에서 "회식 있음"으로 변경되었고,
		/// 두번째 행의 Title 필드는 최기값이 "알립니다2."에서 "퇴근했음"으로 변경되었습니다.
		/// <code>
		/// string sTmpMult = "<!--Title.Begin-->알립니다1.<!--Title.End--><!--Name.Begin-->관리자1<!--Name.End-->\r\n"
		///				+ "<!--Title.Begin-->알립니다2.<!--Title.End--><!--Name.Begin-->관리자2<!--Name.End-->";
		/// CTemplate TmpMult = new CTemplate(sTmpMult, "Title", "Name");
		/// 
		/// DataTable dtTmpMult = TmpMult.DataTable;
		/// 
		/// CVarInfo vi = null;
		/// 
		/// vi = (CVarInfo)dtTmpMult.Rows[0]["Title"];
		/// vi.OuterText = "회식 있음";
		/// vi = (CVarInfo)dtTmpMult.Rows[0]["Name"];
		/// vi.OuterText = "이순신";
		/// 
		/// vi = (CVarInfo)dtTmpMult.Rows[1]["Title"];
		/// vi.OuterText = "퇴근했음";
		/// vi = (CVarInfo)dtTmpMult.Rows[1]["Name"];
		/// vi.OuterText = "홍길동";
		/// 
		/// string sMultOuter = TmpMult.GetTemplate();
		/// Console.WriteLine(sMultOuter);
		/// //--결과
		/// //회식 있음이순신
		/// //퇴근했음홍길동
		/// </code>
		/// </example>
		public string GetTemplate()
		{
			bool UseInner = false;
			return GetTemplate(UseInner);
		}

		/// <summary>
		/// <paramref name="VarName"/>을 이름으로 하는 태그와 태그 안의 값을 삭제함.
		/// </summary>
		/// <param name="Value"><paramref name="VarName"/> 태그를 포함하는 문자열</param>
		/// <param name="VarName">태그의 이름(&lt;--Name.Begin--&gt;홍길동&lt;--Name.End--&gt;에서 Name을 뜻함)</param>
		/// <returns>특정 태그와 태그 안의 값이 삭제된 문자열</returns>
		/// <example>
		/// 다음은 a 태그와 a 태그 안의 값을 삭제한 문자열을 리턴합니다.
		/// <code>
		/// string s = "1<!--a.Begin-->2<!--a.End-->3";
		/// s = CTemplate.RemoveOuter(s, "a"); //"13"
		/// </code>
		/// </example>
		public static string RemoveOuter(string Value, string VarName)
		{
			int PosStart = Value.IndexOf("<!--" + VarName + ".Begin-->", 0, StringComparison.CurrentCultureIgnoreCase);
			if (PosStart == -1) return Value;

			int PosEnd = Value.IndexOf("<!--" + VarName + ".End-->", PosStart + 1, StringComparison.CurrentCultureIgnoreCase);
			if (PosEnd == -1) return Value;
			PosEnd += ("<!--" + VarName + ".End-->").Length;

			if (PosStart != 0)
			{
				Value = Value.Substring(0, PosStart) + Value.Substring(PosEnd);
			}
			else
			{
				Value = Value.Substring(PosEnd);
			}

			return Value;
		}

		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열에
		/// aVarName을 이름으로 하는 태그와 태그 안의 값을 aValue의 값으로 변경해서 리턴함.
		/// 순서대로 변경됨. 즉, aVarName 배열의 첫번째 값은 aValue 배열의 첫번째 값과 대응됨.
		/// </summary>
		/// <param name="Index">같은 이름의 변수가 여러 개일 때 0부터 매겨지는 변수의 순서</param>
		/// <param name="aValue">변수 안에 입력될 실제 값</param>
		/// <example>
		/// 다음은 CTemplate 개체를 생성하고 ReplaceOuter를 이용해서 태그와 태그 안의 값을 변경하는 방법입니다.
		/// <code>
		/// string sTmp = "<!--Title.Begin-->알립니다.<!--Title.End--><!--Name.Begin-->관리자<!--Name.End-->\r\n"
		///				+ "<!--Title.Begin-->알립니다2.<!--Title.End--><!--Name.Begin-->관리자2<!--Name.End-->";
		/// CTemplate Tmp = new CTemplate(sTmp, "Title", "Name");
		///
		/// Tmp.ReplaceOuter(0, "퇴근했음", "홍길동");
		/// Tmp.ReplaceOuter(1, "출근했음", "이순신");
		/// Console.WriteLine(Tmp.GetTemplate());
		/// --결과
		/// 퇴근했음홍길동
		/// 출근했음이순신
		/// </code>
		/// </example>
		public void ReplaceOuter(int Index, params string[] aValue)
		{
			if ((Index + 1) > mdt.Rows.Count)
			{
				string Msg = "Template 안의 변수 개수가 " + mdt.Rows.Count.ToString() + "이므로"
							+ " Index 값: " + Index.ToString() + "은 허용되지 않습니다.";
				throw new Exception(Msg);
			}

			for (int cl = 0, cl2 = this.maVarName.Length; cl < cl2; cl++)
			{
				CVarInfo vi = (CVarInfo)this.mdt.Rows[Index][this.maVarName[cl]];
				vi.OuterText = aValue[cl];
			}
		}
		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열에
		/// aVarName을 이름으로 하는 태그와 태그 안의 값을 aValue의 값으로 변경해서 리턴함.
		/// 순서대로 변경됨. 즉, aVarName 배열의 첫번째 값은 aValue 배열의 첫번째 값과 대응됨.
		/// </summary>
		/// <param name="aValue">변수 안에 입력될 실제 값</param>
		/// <returns>변경이 적용된 개체를 생성할 때 인수로 넘긴 Template</returns>
		/// <example>
		/// 다음은 CTemplate 개체를 생성하고 ReplaceOuter를 이용해서 태그와 태그 안의 값을 변경하는 방법입니다.
		/// <code>
		/// string sTmp = "<!--Title.Begin-->알립니다.<!--Title.End--><!--Name.Begin-->관리자<!--Name.End-->";
		/// CTemplate Tmp = new CTemplate(sTmp, "Title", "Name");
		///
		/// string sOuter = Tmp.ReplaceOuter("퇴근했음", "홍길동");
		/// Console.WriteLine(sOuter); //"퇴근했음홍길동"
		/// </code>
		/// </example>
		public string ReplaceOuter(params string[] aValue)
		{
			ReplaceOuter(0, aValue);

			return GetTemplate();
		}

		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열에
		/// aVarName을 이름으로 하는 태그 안의 값을 aValue의 값으로 변경해서 리턴함.
		/// 순서대로 변경됨. 즉, aVarName 배열의 첫번째 값은 aValue 배열의 첫번째 값과 대응됨.
		/// </summary>
		/// <param name="RowIndex">같은 이름의 변수가 여러 개일 때 0부터 매겨지는 변수의 순서</param>
		/// <param name="aValue">변수 안에 입력될 실제 값</param>
		/// <example>
		/// 다음은 CTemplate 개체를 생성하고 ReplaceInner를 이용해서 태그 안의 값만 변경하는 방법입니다.
		/// <code>
		/// string sTmp = "<!--Title.Begin-->알립니다.<!--Title.End--><!--Name.Begin-->관리자<!--Name.End-->\r\n"
		///				   + "<!--Title.Begin-->알립니다2.<!--Title.End--><!--Name.Begin-->관리자2<!--Name.End-->";
		/// CTemplate Tmp = new CTemplate(sTmp, "Title", "Name");
		///
		/// Tmp.ReplaceInner(0, "퇴근했음", "홍길동");
		/// Tmp.ReplaceInner(1, "출근했음", "이순신");
		/// Console.WriteLine(Tmp.GetTemplate(true));
		/// --결과
		/// <!--Title.Begin-->퇴근했음<!--Title.End--><!--Name.Begin-->홍길동<!--Name.End-->
		/// <!--Title.Begin-->출근했음<!--Title.End--><!--Name.Begin-->이순신<!--Name.End-->
		/// </code>
		/// </example>
		public void ReplaceInner(int RowIndex, params string[] aValue)
		{
			if ((RowIndex + 1) > mdt.Rows.Count)
			{
				string Msg = "Template 안의 변수 개수가 " + mdt.Rows.Count.ToString() + "이므로"
							+ " RowIndex 값: " + RowIndex.ToString() + "은 허용되지 않습니다.";
				throw new Exception(Msg);
			}

			for (int cl = 0, cl2 = this.maVarName.Length; cl < cl2; cl++)
			{
				CVarInfo vi = (CVarInfo)this.mdt.Rows[RowIndex][this.maVarName[cl]];
				vi.InnerText = aValue[cl];
			}
		}
		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열에
		/// aVarName을 이름으로 하는 태그 안의 값을 aValue의 값으로 변경해서 리턴함.
		/// 순서대로 변경됨. 즉, aVarName 배열의 첫번째 값은 aValue 배열의 첫번째 값과 대응됨.
		/// </summary>
		/// <param name="aValue">변수 안에 입력될 실제 값</param>
		/// <returns>변경이 적용된 개체를 생성할 때 인수로 넘긴 Template</returns>
		/// <example>
		/// 다음은 CTemplate 개체를 생성하고 ReplaceInner를 이용해서 태그 안의 값만 변경하는 방법입니다.
		/// <code>
		/// string sTmp = "<!--Title.Begin-->알립니다.<!--Title.End--><!--Name.Begin-->관리자<!--Name.End-->";
		/// CTemplate Tmp = new CTemplate(sTmp, "Title", "Name");
		///
		/// string sInner = Tmp.ReplaceInner("회식 있음", "이순신");
		/// Console.WriteLine(sInner); //"<!--Title.Begin-->회식 있음<!--Title.End--><!--Name.Begin-->이순신<!--Name.End-->"
		/// </code>
		/// </example>
		public string ReplaceInner(params string[] aValue)
		{
			ReplaceInner(0, aValue);

			return GetTemplate(true);
		}

		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열 안의 <paramref name="VarName"/>을 이름으로 하는
		/// 태그 안의 값을 리턴함.
		/// </summary>
		/// <param name="VarName">태그의 이름(&lt;--Name.Begin--&gt;홍길동&lt;--Name.End--&gt;에서 Name을 뜻함)</param>
		/// <returns><paramref name="VarName"/> 태그 안의 문자열 값</returns>
		/// <example>
		/// 다음은 GetInner를 이용해서 Exam1 태그의 값들을 모두 가져온 후,
		/// Q1, Q2 태그의 값을 변경하고,
		/// 변경된 Q1, Q2 태그의 값을 이용해 Exam1 태그의 값을 변경하는 과정입니다.
		/// <code>
		/// string sTmpMain = "<!--Exam1.Begin-->\r\n"
		///				 + "	<!--Q1.Begin-->한라산<!--Q1.End-->\r\n"
		///				 + "	<!--Q2.Begin-->백두산<!--Q2.End-->\r\n"
		///				 +"<!--Exam1.End-->";
		/// CTemplate TmpMain = new CTemplate(sTmpMain, "Exam1");
		/// 
		/// string sTmpSub = TmpMain.GetInner("Exam1");
		/// Console.WriteLine(sTmpSub);
		/// //--결과
		/// //	<!--Q1.Begin-->한라산<!--Q1.End-->
		/// //	<!--Q2.Begin-->백두산<!--Q2.End-->
		/// 
		/// CTemplate TmpSub = new CTemplate(sTmpSub, new string[] { "Q1", "Q2" });
		/// sTmpSub = TmpSub.ReplaceOuter(new string[] { "두만강", "낙동강" });
		/// Console.WriteLine(sTmpSub);
		/// //--결과
		/// //	두만강
		/// //	낙동강
		/// 
		/// sTmpMain = TmpMain.ReplaceInner(sTmpSub);
		/// Console.WriteLine(sTmpMain);
		/// //--결과
		/// //<!--Exam1.Begin-->
		/// //	두만강
		/// //	낙동강
		/// //<!--Exam1.End-->
		/// </code>
		/// </example>
		public string GetInner(string VarName)
		{
			CVarInfo vi = (CVarInfo)this.mdt.Rows[0][VarName];
			return vi.InnerText;
		}

		/// <summary>
		/// 개체를 생성할 때 인수로 넘긴 Template 문자열 안의 aVarName의 각 항목을 필드 이름으로,
		/// 그 안의 값들을 행 값으로 하는 DataTable을 리턴함.
		/// 만약 Template 문자열 안에 같은 이름의 태그가 2개 이상 있다면 행 수는 그에 맞춰 늘어남.
		/// 예를 들어 &lt;--Name.Begin--&gt;나일등&lt;--Name.End--&gt;&lt;--Name.Begin--&gt;나이등&lt;--Name.End--&gt;
		/// 과 같은 값이 있으면 첫번째 행의 Name 필드의 값은 "나일등", 두번째 행의 Name 필드의 값은 "나이등"이 됨.
		/// </summary>
		/// <example>
		/// 다음은 GetInner를 이용해서 Exam1 태그의 값들을 모두 가져온 후,
		/// Q1, Q2 태그의 값을 변경하고,
		/// 변경된 Q1, Q2 태그의 값을 이용해 Exam1 태그의 값을 변경하는 과정입니다.
		/// <code>
		/// string sTmpMain = "<!--Exam1.Begin-->\r\n"
		///				 + "	<!--Q1.Begin-->한라산<!--Q1.End-->\r\n"
		///				 + "	<!--Q2.Begin-->백두산<!--Q2.End-->\r\n"
		///				 +"<!--Exam1.End-->";
		/// CTemplate TmpMain = new CTemplate(sTmpMain, "Exam1");
		/// 
		/// string sTmpSub = TmpMain.GetInner("Exam1");
		/// Console.WriteLine(sTmpSub);
		/// //--결과
		/// //	<!--Q1.Begin-->한라산<!--Q1.End-->
		/// //	<!--Q2.Begin-->백두산<!--Q2.End-->
		/// 
		/// CTemplate TmpSub = new CTemplate(sTmpSub, new string[] { "Q1", "Q2" });
		/// sTmpSub = TmpSub.ReplaceOuter(new string[] { "두만강", "낙동강" });
		/// Console.WriteLine(sTmpSub);
		/// //--결과
		/// //	두만강
		/// //	낙동강
		/// 
		/// sTmpMain = TmpMain.ReplaceInner(sTmpSub);
		/// Console.WriteLine(sTmpMain);
		/// //--결과
		/// //<!--Exam1.Begin-->
		/// //	두만강
		/// //	낙동강
		/// //<!--Exam1.End-->
		/// </code>
		/// </example>
		public DataTable DataTable
		{
			get { return this.mdt; }
		}
	}
}