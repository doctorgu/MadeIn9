using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DoctorGu
{
	public class CMatchInfo
	{
		public Match Match;
		public string ValueBeforeMatch;
		public int ValueStart;
		public int ValueEnd;
	}

	public class CRegex
	{
		public struct Options
		{
			public const RegexOptions Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace = RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace;
		}

		/// <summary>
		/// Ungreedy하게 찾으려면 *, + 대신 *?, +?를 쓸 것		/// 
		/// </summary>
		public struct Pattern
		{
			//<img src="/images/star.gif" />에서 /images/star.gif을 추출함.
			//<img src="/images/star.gif" /><img src="/images/star.gif" />와 같이 줄바꿈이 없는 경우 있어 "\s*.*"를 "[^/>]*"로 변경함.
			public const string ExtractUrlFromImgTag = @"<\s*img[^.]+?src\s*=\s*['""]?(?<Url>[^>""']*)['""]?[^/>]*/?>";

			public const string ExtractUrlFromSms = @"((http://|https://)(?<Url>[^\s\n\)\]\}\>]+))|((?<Url>[a-z|0-9|\-_\.]+(\.aero|\.asia|\.biz|\.cat|\.com|\.jobs|\.coop|\.name|\.info|\.int|\.tel|\.mobi|\.museum|\.net|\.org|\.post|\.pro|\.travel|\.edu|\.gov|\.mil|\.ac|\.ad|\.ae|\.af|\.ag|\.ai|\.al|\.am|\.an|\.ao|\.aq|\.ar|\.as|\.at|\.au|\.aw|\.ax|\.az|\.ba|\.bb|\.bd|\.be|\.bf|\.bg|\.bh|\.bi|\.bj|\.bm|\.bn|\.bo|\.br|\.bs|\.bt|\.bv|\.bw|\.by|\.bz|\.ca|\.cc|\.cd|\.cf|\.cg|\.ch|\.ci|\.ck|\.cl|\.cm|\.cn|\.co|\.cr|\.cs|\.cu|\.cv|\.cx|\.cy|\.cz|\.dd|\.de|\.dj|\.dk|\.dm|\.do|\.dz|\.ec|\.ee|\.eg|\.eh|\.er|\.es|\.et|\.eu|\.fi|\.fj|\.fk|\.fm|\.fo|\.fr|\.ga|\.gb|\.gd|\.ge|\.gf|\.gg|\.gh|\.gi|\.gl|\.gm|\.gn|\.gp|\.gq|\.gr|\.gs|\.gt|\.gu|\.gw|\.gy|\.hk|\.hm|\.hn|\.hr|\.ht|\.hu|\.id|\.ie|\.il|\.im|\.in|\.io|\.iq|\.ir|\.is|\.it|\.je|\.jm|\.jo|\.jp|\.ke|\.kg|\.kh|\.ki|\.km|\.kn|\.kp|\.kr|\.kw|\.ky|\.kz|\.la|\.lb|\.lc|\.li|\.lk|\.lr|\.ls|\.lt|\.lu|\.lv|\.ly|\.ma|\.mc|\.md|\.me|\.mg|\.mh|\.mk|\.ml|\.mm|\.mn|\.mo|\.mp|\.mq|\.mr|\.ms|\.mt|\.mu|\.mv|\.mw|\.mx|\.my|\.mz|\.na|\.nc|\.ne|\.nf|\.ng|\.ni|\.nl|\.no|\.np|\.nr|\.nu|\.nz|\.om|\.pa|\.pe|\.pf|\.pg|\.ph|\.pk|\.pl|\.pm|\.pn|\.pr|\.ps|\.pt|\.pw|\.py|\.qa|\.re|\.ro|\.rs|\.ru|\.rw|\.sa|\.sb|\.sc|\.sd|\.se|\.sg|\.sh|\.si|\.sj|\.sk|\.sl|\.sm|\.sn|\.so|\.sr|\.ss|\.st|\.su|\.sv|\.sx|\.sy|\.sz|\.tc|\.td|\.tf|\.tg|\.th|\.tj|\.tk|\.tl|\.tm|\.tn|\.to|\.tp|\.tr|\.tt|\.tv|\.tw|\.tz|\.ua|\.ug|\.uk|\.us|\.uy|\.uz|\.va|\.vc|\.ve|\.vg|\.vi|\.vn|\.vu|\.wf|\.ws|\.ye|\.yt|\.yu|\.za|\.zm|\.zw))($|[\s\n\)\]\>}]))|((?<Url>[a-z|0-9|\-_\.]+(\.aero|\.asia|\.biz|\.cat|\.com|\.jobs|\.coop|\.name|\.info|\.int|\.tel|\.mobi|\.museum|\.net|\.org|\.post|\.pro|\.travel|\.edu|\.gov|\.mil|\.ac|\.ad|\.ae|\.af|\.ag|\.ai|\.al|\.am|\.an|\.ao|\.aq|\.ar|\.as|\.at|\.au|\.aw|\.ax|\.az|\.ba|\.bb|\.bd|\.be|\.bf|\.bg|\.bh|\.bi|\.bj|\.bm|\.bn|\.bo|\.br|\.bs|\.bt|\.bv|\.bw|\.by|\.bz|\.ca|\.cc|\.cd|\.cf|\.cg|\.ch|\.ci|\.ck|\.cl|\.cm|\.cn|\.co|\.cr|\.cs|\.cu|\.cv|\.cx|\.cy|\.cz|\.dd|\.de|\.dj|\.dk|\.dm|\.do|\.dz|\.ec|\.ee|\.eg|\.eh|\.er|\.es|\.et|\.eu|\.fi|\.fj|\.fk|\.fm|\.fo|\.fr|\.ga|\.gb|\.gd|\.ge|\.gf|\.gg|\.gh|\.gi|\.gl|\.gm|\.gn|\.gp|\.gq|\.gr|\.gs|\.gt|\.gu|\.gw|\.gy|\.hk|\.hm|\.hn|\.hr|\.ht|\.hu|\.id|\.ie|\.il|\.im|\.in|\.io|\.iq|\.ir|\.is|\.it|\.je|\.jm|\.jo|\.jp|\.ke|\.kg|\.kh|\.ki|\.km|\.kn|\.kp|\.kr|\.kw|\.ky|\.kz|\.la|\.lb|\.lc|\.li|\.lk|\.lr|\.ls|\.lt|\.lu|\.lv|\.ly|\.ma|\.mc|\.md|\.me|\.mg|\.mh|\.mk|\.ml|\.mm|\.mn|\.mo|\.mp|\.mq|\.mr|\.ms|\.mt|\.mu|\.mv|\.mw|\.mx|\.my|\.mz|\.na|\.nc|\.ne|\.nf|\.ng|\.ni|\.nl|\.no|\.np|\.nr|\.nu|\.nz|\.om|\.pa|\.pe|\.pf|\.pg|\.ph|\.pk|\.pl|\.pm|\.pn|\.pr|\.ps|\.pt|\.pw|\.py|\.qa|\.re|\.ro|\.rs|\.ru|\.rw|\.sa|\.sb|\.sc|\.sd|\.se|\.sg|\.sh|\.si|\.sj|\.sk|\.sl|\.sm|\.sn|\.so|\.sr|\.ss|\.st|\.su|\.sv|\.sx|\.sy|\.sz|\.tc|\.td|\.tf|\.tg|\.th|\.tj|\.tk|\.tl|\.tm|\.tn|\.to|\.tp|\.tr|\.tt|\.tv|\.tw|\.tz|\.ua|\.ug|\.uk|\.us|\.uy|\.uz|\.va|\.vc|\.ve|\.vg|\.vi|\.vn|\.vu|\.wf|\.ws|\.ye|\.yt|\.yu|\.za|\.zm|\.zw)([/\?][a-z|0-9|/\?#%&\-_+=,]+)))";

			//Mozilla/5.0 (Windows; U; MSIE 9.0; Windows NT 9.0; en-US)에서 9.0을 추출함
			public const string ExtractIeVersionFromUserAgent = @"MSIE\s(?<Version>[0-9]+\.[0-9]+)";

			//<param name="movie" value="/ckfinder/userfiles/flash/Speaking_part-3(Set-A)(1).swf" /> 에서 /ckfinder/userfiles/flash/Speaking_part-3(Set-A)(1).swf를 추출함.
			//IE 전용이 아닐 때는 다음 값도 추출해야 함.
			//data="/ckfinder/userfiles/flash/Speaking_part-3(Set-A)(1).swf"
			//<embed src="/ckfinder/userfiles/flash/Speaking_part-3(Set-A)(1).swf"
			public const string ExtractSwfUrlFromParamTag = @"<\s*param[^.]+value\s*=\s*['""]?(?<Url>[^>""']*)['""]?\s*.*/?>";

			public const string ExtractUrlFromBackground = @"\s*background\s*=\s*['""]?(?<Url>[^>""']*)['""]?\s*.*/?>";
			public const string ExtractUrlFromStyleBackground = @"\s*background(-image)?\s*:\s*url\s*\(['""]?(?<Url>[^)""']*)['""]?\).*/?>";

			//<a href="/member/member.aspx" />에서 /member/member.aspx을 추출함.
			public const string ExtractUrlFromATag = @"<\s*a[^.]+?href\s*=\s*['""]?(?<Url>[^>""']*)['""]?\s*[/]?>";

			//var UriClose = new Uri("/img/test.gif");에서 /img/test.gif를 추출함.
			public const string ExtractUrlFromNewUri = @"new\s+Uri\(['""](?<Url>.+)['""]\)";

			//<a[^>]*>(?<Text>.*)</a>
			public const string ExtractTextFromATag = @"<a[^>]*>(?<Text>[^<]*)</a>";

			//<!--Title.Begin-->알립니다.<!--Title.End--> 에서 Title을 추출함.
			public const string ExtractNameFromTemplate = @"<!--(?<Name>\w+).Begin-->";

			public const string BrTag = @"<br\s*/?>";

			//02)567-3284 -> 011,016,017,018,019가 아니므로 제외
			//016.032.2234 -> 2번째 자리가 0으로 시작하므로 제외
			//012-32431-2343 -> 두번째 자리가 3~4자리가 아니므로 제외
			//(010) 7335 3748과 같이 )와 스페이스가 연속되면 빠지는 단점 있음
			public const string MobileNumber = @"\(?01[0|1|6|7|8|9][\.\-)\s]?[1-9]\d{2,3}[\.\-\s]?\d{4}";
			//MobileNumber는 첫 3자리에 대한 규제가 심하나 PhoneNumber는 0으로 시작하고 뒤의 2자리는 숫자이기만 하면 됨.
			public const string PhoneNumber = @"\(?0\d{1,2}[\.\-)\s]?[1-9]\d{2,3}[\.\-\s]?\d{4}";

			public const string CurrencyWon = @"^\d{1,3}(?:(?<Comma>[,])\d{3})*(?<Won>원)?$";
			public const string Number = @"^[+-]?\d+(\.\d+)?$"; // .5와 같이 .으로 시작하는 형식 제외. 1.2e-3과 같은 형식 제외.

			public const string IpAddress = @"(?<First>[01]?\d\d?|2[0-4]\d|25[0-5])\.(?<Second>[01]?\d\d?|2[0-4]\d|25[0-5])\.(?<Third>[01]?\d\d?|2[0-4]\d|25[0-5])\.(?<Fourth>[01]?\d\d?|2[0-4]\d|25[0-5])(?x)";

			//IgnoreCase 옵션 반드시 사용해야 함.
			public const string RgbFunction = @"rgb\((?<R>[01]?\d\d?|2[0-4]\d|25[0-5]),\s*(?<G>[01]?\d\d?|2[0-4]\d|25[0-5]),\s*(?<B>[01]?\d\d?|2[0-4]\d|25[0-5])\)";

			//http://blogs.msdn.com/b/bclteam/archive/2005/03/15/396452.aspx
			public const string OpenAndCloseParentheses = @"
\(
	[^\(\)]*
	(
		(
			(?<Open>\()
			[^\(\)]*
		)+
		(
			(?<Close-Open>\))
			[^\(\)]*
		)+
	)*
	(?(Open)(?!))
\)";
			public const string OpenAndCloseBraces = @"
{
	[^{}]*
	(
		(
			(?<Open>{)
			[^{}]*
		)+
		(
			(?<Close-Open>})
			[^{}]*
		)+
	)*
	(?(Open)(?!))
}";

			//한자 영역
			//㐀(13312) ~ 䶵(19893): 문자표에는 없음
			//一(19968) ~ 龥(40869): CJK Unified Ideograph
			//豈(63744) ~ 鶴(64045): CJK Compatibility Ideograph
			public const string HanjaRange = @"[㐀-䶵|一-龥|豈-鶴]";

			//LoadXml 호출시 에러나는 문자열 영역
			public const string HexadecimalRangeInvalidForXml = @"[\x00-\x08|\x0b-\x0c|\x0e-\x1f]";

			//http://en.wikipedia.org/wiki/Mapping_of_Unicode_characters#Surrogates
			//The surrogate pair (0xDB82, 0x3C) is invalid. A high surrogate character (0xD800 - 0xDBFF) must always be paired with a low surrogate character (0xDC00 - 0xDFFF).
			public const string SurrogateHighAndLow = @"[\uD800-\uDBFF][\uDC00-\uDFFF]";
			public const string SurrogateHighOrLow = @"[\uD800-\uDBFF|\uDC00-\uDFFF]";
		}

		public static IEnumerable<CMatchInfo> GetMatchResult(Regex r, string Input)
		{
			StringBuilder sb = new StringBuilder();

			int PosStart = 0, PosEnd = 0;
			for (Match m = r.Match(Input); m.Success; m = m.NextMatch())
			{
				PosEnd = (m.Index - 1);

				string Value = Input.Substring(PosStart, (PosEnd - PosStart + 1));
				yield return new CMatchInfo() { Match = m, ValueBeforeMatch = Value, ValueStart = PosStart, ValueEnd = PosEnd };

				PosStart = (m.Index + m.Length);
			}

			if ((PosStart + 1) < Input.Length)
			{
				PosEnd = Input.Length - 1;

				string Value = Input.Substring(PosStart, (PosEnd - PosStart + 1));
				yield return new CMatchInfo() { Match = null, ValueBeforeMatch = Value, ValueStart = PosStart, ValueEnd = PosEnd };
			}
		}
	}
}
