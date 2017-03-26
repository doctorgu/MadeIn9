using System;

namespace DoctorGu
{
	/// <summary>
	/// "name!구|age!31"과 같은 문자열을 구성해서 
	/// name으로 구 문자열을, age로 31을 가져올 수 있게 함.
	/// </summary>
	/// <example>
	/// CKeyValue kv = new CKeyValue("", "!", "|");
	/// kv.SetValue("a", "123"); 
	/// Console.WriteLine(kv.KeyValueList); //"a!123|"
	/// kv.SetValue("b", "123");
	/// Console.WriteLine(kv.KeyValueList); //"a!123|b!123|"
	/// kv.SetValue("a", "333");
	/// Console.WriteLine(kv.KeyValueList); //"a!333|b!123|"
	/// Console.WriteLine(kv.GetKeyByIndex(0, "xxx")); //"a"
	/// Console.WriteLine(kv.GetKeyByIndex(1, "xxx")); //"b"
	/// Console.WriteLine(kv.GetKeyByIndex(2, "xxx")); //"xxx"
	/// Console.WriteLine(kv.GetValueByKey("a", "xxx")); //"333"
	/// Console.WriteLine(kv.GetValueByKey("b", "xxx")); //"123"
	/// Console.WriteLine(kv.GetValuebyIndex(0, "xxx")); //"333"
	/// Console.WriteLine(kv.GetValuebyIndex(1, "xxx")); //"123"
	/// Console.WriteLine(kv.GetValuebyIndex(2, "xxx")); //"xxx"
	/// </example>
	public class CKeyValue
	{
		private string mKeyValueList = "";
		private string mKeyValueDelim = "";
		private string mColDelim = "";
		private bool mAllowQuoteAsGroupingValue = false;

		public CKeyValue(string KeyValueList, string KeyValueDelim, string ItemDelim, bool AllowQuoteAsGroupingValue)
		{
			this.mKeyValueList = KeyValueList;
			this.mKeyValueDelim = KeyValueDelim;
			this.mColDelim = ItemDelim;
			this.mAllowQuoteAsGroupingValue = AllowQuoteAsGroupingValue;
		}

		/// <summary>
		/// Key와 Value로 구분된 문자열을 추가함. 만약 해당 Key가 이미 있다면 Value만을 변경함.
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="Value"></param>
		/// <returns></returns>
		public void SetValue(string Key, string Value)
		{
			//Key와 Value에 KeyValueDelim, ItemDelim 문자열이 있다면 에러 발생.
			if (((Key + Value).IndexOf(mKeyValueDelim) != -1) 
				|| ((Key + Value).IndexOf(mColDelim) != -1))
			{
				throw new Exception("Key와 Value에 " + "KeyValueDelim이나 ItemDelim 문자열이 포함되었습니다.");
			}

			//이미 있는 Key와 Value를 삭제함.
			int PosStart = 0, PosEnd = 0;
			PosStart = mKeyValueList.IndexOf(Key + mKeyValueDelim);
			if (PosStart != -1)
			{
				PosEnd = mKeyValueList.IndexOf(mColDelim, PosStart) - 1;
				if (PosEnd != -2)
				{
					mKeyValueList = mKeyValueList.Substring(0, PosStart) 
						+ Key + mKeyValueDelim + Value 
						+ mKeyValueList.Substring(PosEnd + 1);
					return;
				}
			}

			mKeyValueList = mKeyValueList + Key + mKeyValueDelim + Value + mColDelim;
		}
		
		/// <summary>
		/// Key와 Value로 구분된 문자열에서 해당 Key에 해당하는 Value를 리턴함.
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="Default"></param>
		/// <returns></returns>
		public string GetValueByKey(string Key, string Default)
		{
			int PosStart = 0, PosEnd = 0;
			
			if (Key == "")
			{
				return Default;
			}

			PosStart = mKeyValueList.IndexOf(Key + mKeyValueDelim);
			if (PosStart == -1)
			{
				return Default;
			}

			PosStart += (Key + mKeyValueDelim).Length;
			if (PosStart == mKeyValueList.Length)
			{
				return Default;
			}

			if ((this.mAllowQuoteAsGroupingValue) && (mKeyValueList.Substring(PosStart, 1) == "\""))
			{
				PosStart++;
				PosEnd = mKeyValueList.IndexOf("\"", PosStart) - 1;
				if (PosEnd == -2)
				{
					PosEnd = PosStart;
				}
			}
			else
			{
				PosEnd = mKeyValueList.IndexOf(mColDelim, PosStart) - 1;
				if (PosEnd == -2)
				{
					PosEnd = mKeyValueList.Length - 1;
				}
			}

			string Value = mKeyValueList.Substring(PosStart, (PosEnd - PosStart) + 1);
			
			return Value;
		}
		public string GetValueByKey(string Key)
		{
			return GetValueByKey(Key, "");
		}

		/// <summary>
		/// Key와 Value로 구분된 문자열에서 nTH번째에 해당하는 Key를 리턴함.
		/// </summary>
		/// <param name="nTH"></param>
		/// <param name="Default"></param>
		/// <returns></returns>
		public string GetKeyByIndex(int nTH, string Default)
		{
			int PosStart = 0, PosEnd = 0, PosOfnTH;

			if (nTH == 0)
			{
				PosStart = 0;
				PosEnd = mKeyValueList.IndexOf("!") - 1;
				if (PosEnd == -2)
				{
					return Default;
				}
			}
			else 
			{
				PosOfnTH = CFindRep.IndexOfnTH(mKeyValueList, mColDelim, nTH - 1);
				if (PosOfnTH == -1)
				{
					return Default;
				}

				PosStart = PosOfnTH + 1;
				PosEnd = mKeyValueList.IndexOf("!", PosStart) - 1;
				if (PosEnd == -2)
				{
					return Default;
				}
			}

			return mKeyValueList.Substring(PosStart, (PosEnd - PosStart) + 1);
		}
		public string GetKeyByIndex(int nTH)
		{
			return GetKeyByIndex(nTH, "");
		}

		/// <summary>
		/// Key와 Value로 구분된 문자열에서 nTH번째에 해당하는 Value를 리턴함.
		/// </summary>
		/// <param name="nTH"></param>
		/// <param name="Default"></param>
		/// <returns></returns>
		public string GetValueByIndex(int nTH, string Default)
		{
			string Key = GetKeyByIndex(nTH, Default);
			
			string Value = GetValueByKey(Key, Default);
			
			return Value;
		}
		public string GetValueByIndex(int nTH)
		{
			return GetValueByIndex(nTH, "");
		}

		/// <summary>
		/// 설정된 Key와 Value의 모든 목록을 리턴함.
		/// </summary>
		public string KeyValueList
		{
			get {return mKeyValueList;}
		}
	}
}
