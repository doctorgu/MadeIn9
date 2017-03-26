using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace DoctorGu
{
	public enum WordsType
	{
		OnlyWordExcludeDelim,
		WordIsOneDelimIsOne
	}

	/// <summary>
	/// 문장을 이루는 문자열 안에서 현재 위치를 기준으로
	/// 현재 단어, 이전 단어, 다음 단어를 가져옴.
	/// </summary>
	public class CParagraph
	{
		public struct DelimWord
		{
			public const string Default = "`~!@#$%^&*()-_=+[{]}|;:'\",<.>/? \t\n\r";
			public const string NoDash = "`~!@#$%^&*()_=+[{]}|;:'\",<.>/? \t\n\r";
			public const string NoUnderbar = "`~!@#$%^&*()-=+[{]}|;:'\",<.>/? \t\n\r";
		}

		private string mWordDelimeters = CParagraph.DelimWord.Default;

		public CParagraph(string WordDelimeters)
		{
			this.mWordDelimeters = WordDelimeters;
		}
		public CParagraph()
		{
		}

		public bool IsDelimeter(char c)
		{
			return (this.mWordDelimeters.IndexOf(c) != -1);
		}

		public Dictionary<int, string> GetIndexAndWords(string Value, bool IncludeDelimeter)
		{
			Dictionary<int, string> dicWords = new Dictionary<int, string>();
			string Word = "", Delim = "";
			int IndexWord = -1, IndexDelim = -1;
			for (int i = 0; i < Value.Length; i++)
			{
				char c = Value[i];
				if ((this.mWordSolo != null) && (CArray.IndexOf(this.mWordSolo, c) != -1))
				{
					if ((Delim != "") && IncludeDelimeter)
						dicWords.Add(IndexDelim, Delim);

					if (Word != "")
						dicWords.Add(IndexWord, Word);

					Delim = "";
					IndexDelim = -1;

					Word = "";
					IndexWord = -1;


					dicWords.Add(i, c.ToString());

					continue;
				}

				if (this.IsDelimeter(c))
				{
					Delim += c.ToString();

					if (IndexDelim == -1)
						IndexDelim = i;

					if (Word != "")
						dicWords.Add(IndexWord, Word);

					Word = "";
					IndexWord = -1;
				}
				else
				{
					Word += c.ToString();

					if (IndexWord == -1)
						IndexWord = i;

					if ((Delim != "") && IncludeDelimeter)
						dicWords.Add(IndexDelim, Delim);

					Delim = "";
					IndexDelim = -1;
				}
			}

			if (Word != "")
				dicWords.Add(IndexWord, Word);
			else if ((Delim != "") && IncludeDelimeter)
				dicWords.Add(IndexDelim, Delim);

			return dicWords;
		}

		public List<string> GetWords(string Value, bool IncludeDelimeter)
		{
			return GetIndexAndWords(Value, IncludeDelimeter).Values.ToList();

			//List<string> aWordOrDelim = new List<string>();
			//List<bool> aIsWord = new List<bool>();
			//string Word = "", Delim = "";
			//for (int i = 0; i < Value.Length; i++)
			//{
			//    char c = Value[i];

			//    if (IsDelimeter(c))
			//    {
			//        Delim += c.ToString();

			//        if (Word != "")
			//        {
			//            AddWord(Word, aWordOrDelim, aIsWord);
			//        }

			//        Word = "";
			//    }
			//    else
			//    {
			//        Word += c.ToString();

			//        if ((Delim != "") && (Type != WordsType.OnlyWordExcludeDelim))
			//        {
			//            AddDelim(Delim, aWordOrDelim, aIsWord);
			//        }

			//        Delim = "";
			//    }
			//}

			//if (Word != "")
			//{
			//    AddWord(Word, aWordOrDelim, aIsWord);
			//}
			//else if ((Delim != "") && (Type != WordsType.OnlyWordExcludeDelim))
			//{
			//    AddDelim(Delim, aWordOrDelim, aIsWord);
			//}

			//return aWordOrDelim;
		}
		private static void AddWord(string Word, List<string> aWordOrDelim, List<bool> aIsWord)
		{
			aWordOrDelim.Add(Word);
			aIsWord.Add(true);
		}
		private static void AddDelim(string Delim, List<string> aWordOrDelim, List<bool> aIsWord)
		{
			aWordOrDelim.Add(Delim);
			aIsWord.Add(false);
		}

		private char[] mWordSolo;
		/// <summary>독립된 단어로 취급할 문자열. "a, b"에서 new char[] { ',' }를 지정하면 "a", ",", " ", "b"로 나눠짐.</summary>
		public char[] WordSolo
		{
			get { return this.mWordSolo; }
			set { this.mWordSolo = value; }
		}

		///// <summary>
		///// 현재 위치에 있는 단어를 리턴함.
		///// </summary>
		///// <param name="Value">검사 대상 문자열</param>
		///// <param name="Index">현재 위치</param>
		///// <example>
		///// CParagraph p = new CParagraph();
		///// Console.WriteLine(p.GetWordFromPoint("나는 너를 본다.", -1)); //error
		///// Console.WriteLine(p.GetWordFromPoint("나는 너를 본다.", 0)); //"나는"
		///// Console.WriteLine(p.GetWordFromPoint("나는 너를 본다.", 1)); //"나는"
		///// Console.WriteLine(p.GetWordFromPoint("나는 너를 본다.", 2)); //"나는"
		///// Console.WriteLine(p.GetWordFromPoint("나는 너를 본다.", 3)); //"너를"
		///// Console.WriteLine(p.GetWordFromPoint("나는 너를 본다.", 99)); //""
		///// </example>
		//public string GetWordFromPoint(string Value, int Index)
		//{
		//    int PosChar = 0;

		//    //현재 위치가 글자수보다 많으면 빠져나감.
		//    if ((Index + 1) > Value.Length)
		//    {
		//        return "";
		//    }

		//    //현재 위치 이전부터 조사해서 단어의 시작 위치를 찾음.
		//    //시작 위치를 못 찾는다면 기본값인 0이 설정됨.
		//    for (int i = (Index - 1); i >= 0; i--)
		//    {
		//        if (mWordDelimeters.IndexOf(Value[i]) != -1)
		//        {
		//            PosChar = i + 1;
		//            break;
		//        }
		//    }
			
		//    for (int i = Index; i < Value.Length; i++)
		//    {
		//        //단어의 종료 위치를 찾았다면 시작 위치부터 현재까지 리턴함.
		//        if (mWordDelimeters.IndexOf(Value[i]) != -1)
		//        {
		//            return Value.Substring(PosChar, i - PosChar);
		//        }
		//    }

		//    //단어의 종료 위치를 못 찾았다면 마지막 문자열까지 리턴함.
		//    return Value.Substring(PosChar, Value.Length);
		//}

		///// <summary>
		///// 현재 위치 이전의 단어를 리턴함.
		///// </summary>
		///// <param name="Value">검사 대상 문자열</param>
		///// <param name="Index">현재 위치</param>
		///// <example>
		///// CParagraph p = new CParagraph();
		///// Console.WriteLine(p.GetPrevWordFromPoint("나는 너를 본다.", -1)); //error
		///// Console.WriteLine(p.GetPrevWordFromPoint("나는 너를 본다.", 1)); //""
		///// Console.WriteLine(p.GetPrevWordFromPoint("나는 너를 본다.", 2)); //"나는"
		///// Console.WriteLine(p.GetPrevWordFromPoint("나는 너를 본다.", 3)); //"나는"
		///// Console.WriteLine(p.GetPrevWordFromPoint("나는 너를 본다.", 6)); //"너를"
		///// Console.WriteLine(p.GetPrevWordFromPoint("나는 너를 본다.", 99)); //""
		///// </example>
		//public string GetPrevWordFromPoint(string Value, int Index)
		//{
		//    int PosLast = -1;
		//    int PosFirst = -1;

		//    //현재 위치가 글자수보다 많으면 빠져나감.
		//    if ((Index + 1) > Value.Length)
		//    {
		//        return "";
		//    }

		//    //현재 위치부터 조사해서 단어의 마지막 위치를 찾음.
		//    //마지막 위치를 못 찾았다면 이전 단어가 없는 것이므로 ""을 리턴함.
		//    for (int i = Index; i >= 0; i--)
		//    {
		//        if (mWordDelimeters.IndexOf(Value[i]) != -1)
		//        {
		//            PosLast = i - 1;
		//            break;
		//        }
		//    }
		//    if (PosLast == -1)
		//    {
		//        return "";
		//    }
			
		//    //마지막 위치부터 조사해서 단어의 처음 위치를 찾음.
		//    //처음 위치를 못 찾았다면 0을 시작위치로 함.
		//    for (int i = PosLast; i >= 0; i--)
		//    {
		//        if (mWordDelimeters.IndexOf(Value[i]) != -1)
		//        {
		//            PosFirst = i + 1;
		//            break;
		//        }
		//    }
		//    if (PosFirst == -1)
		//    {
		//        PosFirst = 0;
		//    }

		//    return Value.Substring(PosFirst, PosLast - PosFirst + 1);
		//}

		///// <summary>
		///// 현재 위치 다음의 단어를 리턴함.
		///// </summary>
		///// <param name="Value">검사 대상 문자열</param>
		///// <param name="Index">현재 위치</param>
		///// <example>
		///// CParagraph p = new CParagraph();
		///// Console.WriteLine(p.GetNextWordFromPoint("나는 너를 본다.", -1)); //error
		///// Console.WriteLine(p.GetNextWordFromPoint("나는 너를 본다.", 1)); //너를
		///// Console.WriteLine(p.GetNextWordFromPoint("나는 너를 본다.", 2)); //"너를"
		///// Console.WriteLine(p.GetNextWordFromPoint("나는 너를 본다.", 3)); //"본다"
		///// Console.WriteLine(p.GetNextWordFromPoint("나는 너를 본다.", 6)); //""
		///// Console.WriteLine(p.GetNextWordFromPoint("나는 너를 본다.", 99)); //""
		///// </example>
		//public string GetNextWordFromPoint(string Value, int Index)
		//{
		//    int PosLast = -1;
		//    int PosFirst = -1;

		//    //현재 위치가 글자수보다 많으면 빠져나감.
		//    if ((Index + 1) > Value.Length)
		//    {
		//        return "";
		//    }

		//    //현재 위치부터 조사해서 단어의 처음 위치를 찾음.
		//    //처음 위치를 못 찾았다면 다음 단어가 없는 것이므로 ""을 리턴함.
		//    for (int i = Index; i < Value.Length; i++)
		//    {
		//        if (mWordDelimeters.IndexOf(Value[i]) != -1)
		//        {
		//            PosFirst = i + 1;
		//            break;
		//        }
		//    }
		//    if (PosFirst == -1)
		//    {
		//        return "";
		//    }
			
		//    //처음 위치부터 조사해서 단어의 마지막 위치를 찾음.
		//    //마지막 위치를 못 찾았다면 문자열의 마지막 부분을 마지막 위치로 함.
		//    for (int i = PosFirst; i < Value.Length; i++)
		//    {
		//        if (mWordDelimeters.IndexOf(Value[i]) != -1)
		//        {
		//            PosLast = i - 1;
		//            break;
		//        }
		//    }
		//    if (PosLast == -1)
		//    {
		//        PosLast = Value.Length - 1;
		//    }

		//    return Value.Substring(PosFirst, PosLast - PosFirst + 1);
		//}

		////단어를 구분짓는 구분자를 설정하거나 리턴함.
		//public string WordDelimeters
		//{
		//    get 
		//    {
		//        return WordDelimeters;
		//    }

		//    set
		//    {
		//        mWordDelimeters = value;
		//    }
		//}
	}
}
