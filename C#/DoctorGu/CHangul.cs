using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoctorGu
{
	public class CHangul
	{
		/**
		https://gist.github.com/sooop/4958873

		초성 중성 종성 분리 하기
	
		유니코드 한글은 0xAC00 으로부터 
		초성 19개, 중상21개, 종성28개로 이루어지고
		이들을 조합한 11,172개의 문자를 갖는다.
 
		한글코드의 값 = ((초성 * 21) + 중성) * 28 + 종성 + 0xAC00 
		(0xAC00은 'ㄱ'의 코드값)
 
		따라서 다음과 같은 계산 식이 구해진다. 
		유니코드 한글 문자 코드 값이 X일 때,
 
		초성 = ((X - 0xAC00) / 28) / 21
		중성 = ((X - 0xAC00) / 28) % 21
		종성 = (X - 0xAC00) % 28
 
		이 때 초성, 중성, 종성의 값은 각 소리 글자의 코드값이 아니라
		이들이 각각 몇 번째 문자인가를 나타내기 때문에 다음과 같이 다시 처리한다. 
 
		초성문자코드 = 초성 + 0x1100 //('ㄱ')
		중성문자코드 = 중성 + 0x1161 // ('ㅏ')
		종성문자코드 = 종성 + 0x11A8 - 1 // (종성이 없는 경우가 있으므로 1을 뺌)
 
		**/
		public static char GetInitialConsonant(char Value)
		{
			int r = ((Value - 0xac00) / 28) / 21;
			return (char)(r + 0x1100);
		}

		public static char GetMedialVowel(char Value)
		{
			int r = ((Value - 0xac00) / 28) % 21;
			return (char)(r + 0x1161);
		}

		public static char GetFinalConsonant(char Value)
		{
			int r = (Value - 0xac00) % 28;
			return (char)(r + 0x11A8 - 1);
		}

		public static char Join(char Initial, char Medial, char Final)
		{
			const string InitialList = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";
			const string MedialList = "ㅏㅐㅑㅒㅓㅔㅕㅖㅗㅘㅙㅚㅛㅜㅝㅞㅟㅠㅡㅢㅣ";
			const string FinalList = " ㄱㄲㄳㄴㄵㄶㄷㄹㄺㄻㄼㄽㄾㄿㅀㅁㅂㅄㅅㅆㅇㅈㅊㅋㅌㅍㅎ";
			const int UnicodeHangulBase = 0xAC00;

			int PosInitial = InitialList.IndexOf(Initial);
			int PosMedial = MedialList.IndexOf(Medial);
			int PosFinal = FinalList.IndexOf(Final);

			int Value = UnicodeHangulBase + (PosInitial * 21 + PosMedial) * 28 + PosFinal;
			return (char)Value;
		}
	}
}
