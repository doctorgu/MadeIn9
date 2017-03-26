using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace DoctorGu
{
	//http://www.codeproject.com/KB/HTML/HTML_to_Plain_Text.aspx?msg=3692345

	public class CHtmlToText
	{
		private Dictionary<string, int> _dicHtmlToText;
		private Dictionary<int, string> _dicTextToHtml;

		public CHtmlToText()
		{
			_dicHtmlToText = GetHtmlToText();
			_dicTextToHtml = GetTextToHtml();
		}

		private Dictionary<string, int> GetHtmlToText()
		{
			// Add all possible special character into a dictionary
			Dictionary<string, int> dic = new Dictionary<string, int>();

			dic.Add("&Aacute;", 193); dic.Add("&aacute;", 225); dic.Add("&Acirc;", 194);
			dic.Add("&acirc;", 226); dic.Add("&acute;", 180); dic.Add("&AElig;", 198);
			dic.Add("&aelig;", 230); dic.Add("&Agrave;", 192); dic.Add("&agrave;", 224);
			dic.Add("&alefsym;", 8501); dic.Add("&Alpha;", 913); dic.Add("&alpha;", 945);
			dic.Add("&amp;", 38); dic.Add("&and;", 8743); dic.Add("&ang;", 8736);
			dic.Add("&Aring;", 197); dic.Add("&aring;", 229); dic.Add("&asymp;", 8776);
			dic.Add("&Atilde;", 195); dic.Add("&atilde;", 227); dic.Add("&Auml;", 196);
			dic.Add("&auml;", 228); dic.Add("&bdquo;", 8222); dic.Add("&Beta;", 914);
			dic.Add("&beta;", 946); dic.Add("&brvbar;", 166); dic.Add("&bull;", 8226);
			dic.Add("&cap;", 8745); dic.Add("&Ccedil;", 199); dic.Add("&ccedil;", 231);
			dic.Add("&cedil;", 184); dic.Add("&cent;", 162); dic.Add("&Chi;", 935);
			dic.Add("&chi;", 967); dic.Add("&circ;", 710); dic.Add("&clubs;", 9827);
			dic.Add("&cong;", 8773); dic.Add("&copy;", 169); dic.Add("&crarr;", 8629);
			dic.Add("&cup;", 8746); dic.Add("&curren;", 164); dic.Add("&dagger;", 8224);
			dic.Add("&Dagger;", 8225); dic.Add("&darr;", 8595); dic.Add("&dArr;", 8659);
			dic.Add("&deg;", 176); dic.Add("&Delta;", 916); dic.Add("&delta;", 948);
			dic.Add("&diams;", 9830); dic.Add("&divide;", 247); dic.Add("&Eacute;", 201);
			dic.Add("&eacute;", 233); dic.Add("&Ecirc;", 202); dic.Add("&ecirc;", 234);
			dic.Add("&Egrave;", 200); dic.Add("&egrave;", 232); dic.Add("&emdash;", 8212);
			dic.Add("&empty;", 8709); dic.Add("&emsp;", 8195); dic.Add("&endash;", 8211);
			dic.Add("&ensp;", 8194); dic.Add("&Epsilon;", 917); dic.Add("&epsilon;", 949);
			dic.Add("&equiv;", 8801); dic.Add("&Eta;", 919); dic.Add("&eta;", 951);
			dic.Add("&ETH;", 208); dic.Add("&eth;", 240); dic.Add("&Euml;", 203);
			dic.Add("&euml;", 235); dic.Add("&euro;", 8364); dic.Add("&exist;", 8707);
			dic.Add("&fnof;", 402); dic.Add("&forall;", 8704); dic.Add("&frac12;", 189);
			dic.Add("&frac14;", 188); dic.Add("&frac34;", 190); dic.Add("&frasl;", 8260);
			dic.Add("&Gamma;", 915); dic.Add("&gamma;", 947); dic.Add("&ge;", 8805);
			dic.Add("&gt;", 62); dic.Add("&harr;", 8596); dic.Add("&hArr;", 8660);
			dic.Add("&hearts;", 9829); dic.Add("&hellip;", 8230); dic.Add("&Iacute;", 205);
			dic.Add("&iacute;", 237); dic.Add("&Icirc;", 206); dic.Add("&icirc;", 238);
			dic.Add("&iexcl;", 161); dic.Add("&Igrave;", 204); dic.Add("&igrave;", 236);
			dic.Add("&image;", 8465); dic.Add("&infin;", 8734); dic.Add("&int;", 8747);
			dic.Add("&Iota;", 921); dic.Add("&iota;", 953); dic.Add("&iquest;", 191);
			dic.Add("&isin;", 8712); dic.Add("&Iuml;", 207); dic.Add("&iuml;", 239);
			dic.Add("&Kappa;", 922); dic.Add("&kappa;", 954); dic.Add("&Lambda;", 923);
			dic.Add("&lambda;", 955); dic.Add("&lang;", 9001); dic.Add("&laquo;", 171);
			dic.Add("&larr;", 8592); dic.Add("&lArr;", 8656); dic.Add("&lceil;", 8968);
			dic.Add("&ldquo;", 8220); dic.Add("&le;", 8804); dic.Add("&lfloor;", 8970);
			dic.Add("&lowast;", 8727); dic.Add("&loz;", 9674); dic.Add("&lrm;", 8206);
			dic.Add("&lsaquo;", 8249); dic.Add("&lsquo;", 8216); dic.Add("&lt;", 60);
			dic.Add("&macr;", 175); dic.Add("&mdash;", 8212); dic.Add("&micro;", 181);
			dic.Add("&middot;", 183); dic.Add("&minus;", 8722); dic.Add("&Mu;", 924);
			dic.Add("&mu;", 956); dic.Add("&nabla;", 8711); dic.Add("&nbsp;", 32); // Using space instead of Ascii 160
			dic.Add("&ndash;", 8211); dic.Add("&ne;", 8800); dic.Add("&ni;", 8715);
			dic.Add("&not;", 172); dic.Add("&notin;", 8713); dic.Add("&nsub;", 8836);
			dic.Add("&Ntilde;", 209); dic.Add("&ntilde;", 241); dic.Add("&Nu;", 925);
			dic.Add("&nu;", 957); dic.Add("&Oacute;", 211); dic.Add("&oacute;", 243);
			dic.Add("&Ocirc;", 212); dic.Add("&ocirc;", 244); dic.Add("&OElig;", 338);
			dic.Add("&oelig;", 339); dic.Add("&Ograve;", 210); dic.Add("&ograve;", 242);
			dic.Add("&oline;", 8254); dic.Add("&Omega;", 937); dic.Add("&omega;", 969);
			dic.Add("&Omicron;", 927); dic.Add("&omicron;", 959); dic.Add("&oplus;", 8853);
			dic.Add("&or;", 8744); dic.Add("&ordf;", 170); dic.Add("&ordm;", 186);
			dic.Add("&Oslash;", 216); dic.Add("&oslash;", 248); dic.Add("&Otilde;", 213);
			dic.Add("&otilde;", 245); dic.Add("&otimes;", 8855); dic.Add("&Ouml;", 214);
			dic.Add("&ouml;", 246); dic.Add("&para;", 182); dic.Add("&part;", 8706);
			dic.Add("&permil;", 8240); dic.Add("&perp;", 8869); dic.Add("&Phi;", 934);
			dic.Add("&phi;", 966); dic.Add("&Pi;", 928); dic.Add("&pi;", 960);
			dic.Add("&piv;", 982); dic.Add("&plusmn;", 177); dic.Add("&pound;", 163);
			dic.Add("&prime;", 8242); dic.Add("&Prime;", 8243); dic.Add("&prod;", 8719);
			dic.Add("&prop;", 8733); dic.Add("&Psi;", 936); dic.Add("&psi;", 968);
			dic.Add("&quot;", 34); dic.Add("&radic;", 8730); dic.Add("&rang;", 9002);
			dic.Add("&raquo;", 187); dic.Add("&rarr;", 8594); dic.Add("&rArr;", 8658);
			dic.Add("&rceil;", 8969); dic.Add("&rdquo;", 8221); dic.Add("&real;", 8476);
			dic.Add("&reg;", 174); dic.Add("&rfloor;", 8971); dic.Add("&Rho;", 929);
			dic.Add("&rho;", 961); dic.Add("&rlm;", 8207); dic.Add("&rsaquo;", 8250);
			dic.Add("&rsquo;", 8217); dic.Add("&sbquo;", 8218); dic.Add("&Scaron;", 352);
			dic.Add("&scaron;", 353); dic.Add("&sdot;", 8901); dic.Add("&sect;", 167);
			dic.Add("&shy;", 173); dic.Add("&Sigma;", 931); dic.Add("&sigma;", 963);
			dic.Add("&sigmaf;", 962); dic.Add("&sim;", 8764); dic.Add("&spades;", 9824);
			dic.Add("&sub;", 8834); dic.Add("&sube;", 8838); dic.Add("&sum;", 8721);
			dic.Add("&sup;", 8835); dic.Add("&sup1;", 185); dic.Add("&sup2;", 178);
			dic.Add("&sup3;", 179); dic.Add("&supe;", 8839); dic.Add("&szlig;", 223);
			dic.Add("&Tau;", 932); dic.Add("&tau;", 964); dic.Add("&there4;", 8756);
			dic.Add("&Theta;", 920); dic.Add("&theta;", 952); dic.Add("&thetasym;", 977);
			dic.Add("&thinsp;", 8201); dic.Add("&THORN;", 222); dic.Add("&thorn;", 254);
			dic.Add("&tilde;", 732); dic.Add("&times;", 215); dic.Add("&trade;", 8482);
			dic.Add("&Uacute;", 218); dic.Add("&uacute;", 250); dic.Add("&uarr;", 8593);
			dic.Add("&uArr;", 8657); dic.Add("&Ucirc;", 219); dic.Add("&ucirc;", 251);
			dic.Add("&Ugrave;", 217); dic.Add("&ugrave;", 249); dic.Add("&uml;", 168);
			dic.Add("&upsih;", 978); dic.Add("&Upsilon;", 933); dic.Add("&upsilon;", 965);
			dic.Add("&Uuml;", 220); dic.Add("&uuml;", 252); dic.Add("&weierp;", 8472);
			dic.Add("&Xi;", 926); dic.Add("&xi;", 958); dic.Add("&Yacute;", 221);
			dic.Add("&yacute;", 253); dic.Add("&yen;", 165); dic.Add("&yuml;", 255);
			dic.Add("&Yuml;", 376); dic.Add("&Zeta;", 918); dic.Add("&zeta;", 950);
			dic.Add("&zwj;", 8205); dic.Add("&zwnj;", 8204);

			return dic;
		}

		private Dictionary<int, string> GetTextToHtml()
		{
			Dictionary<int, string> dic = new Dictionary<int, string>();

			dic.Add(34, "&quot;");
			dic.Add(38, "&amp;");
			//&apos;는 제외(CKEditor에서 자동변환되지 않으므로)
			//dic.Add(39, "&apos;");
			dic.Add(60, "&lt;");
			dic.Add(62, "&gt;");
			dic.Add(160, "&nbsp;");
			dic.Add(161, "&iexcl;");
			dic.Add(162, "&cent;");
			dic.Add(163, "&pound;");
			dic.Add(164, "&curren;");
			dic.Add(165, "&yen;");
			dic.Add(166, "&brvbar;");
			dic.Add(167, "&sect;");
			dic.Add(168, "&uml;");
			dic.Add(169, "&copy;");
			dic.Add(170, "&ordf;");
			dic.Add(171, "&laquo;");
			dic.Add(172, "&not;");
			dic.Add(173, "&shy;");
			dic.Add(174, "&reg;");
			dic.Add(175, "&macr;");
			dic.Add(176, "&deg;");
			dic.Add(177, "&plusmn;");
			dic.Add(178, "&sup2;");
			dic.Add(179, "&sup3;");
			dic.Add(180, "&acute;");
			dic.Add(181, "&micro;");
			dic.Add(182, "&para;");
			dic.Add(183, "&middot;");
			dic.Add(184, "&cedil;");
			dic.Add(185, "&sup1;");
			dic.Add(186, "&ordm;");
			dic.Add(187, "&raquo;");
			dic.Add(188, "&frac14;");
			dic.Add(189, "&frac12;");
			dic.Add(190, "&frac34;");
			dic.Add(191, "&iquest;");
			dic.Add(192, "&Agrave;");
			dic.Add(193, "&Aacute;");
			dic.Add(194, "&Acirc;");
			dic.Add(195, "&Atilde;");
			dic.Add(196, "&Auml;");
			dic.Add(197, "&Aring;");
			dic.Add(198, "&AElig;");
			dic.Add(199, "&Ccedil;");
			dic.Add(200, "&Egrave;");
			dic.Add(201, "&Eacute;");
			dic.Add(202, "&Ecirc;");
			dic.Add(203, "&Euml;");
			dic.Add(204, "&Igrave;");
			dic.Add(205, "&Iacute;");
			dic.Add(206, "&Icirc;");
			dic.Add(207, "&Iuml;");
			dic.Add(208, "&ETH;");
			dic.Add(209, "&Ntilde;");
			dic.Add(210, "&Ograve;");
			dic.Add(211, "&Oacute;");
			dic.Add(212, "&Ocirc;");
			dic.Add(213, "&Otilde;");
			dic.Add(214, "&Ouml;");
			dic.Add(215, "&times;");
			dic.Add(216, "&Oslash;");
			dic.Add(217, "&Ugrave;");
			dic.Add(218, "&Uacute;");
			dic.Add(219, "&Ucirc;");
			dic.Add(220, "&Uuml;");
			dic.Add(221, "&Yacute;");
			dic.Add(222, "&THORN;");
			dic.Add(223, "&szlig;");
			dic.Add(224, "&agrave;");
			dic.Add(225, "&aacute;");
			dic.Add(226, "&acirc;");
			dic.Add(227, "&atilde;");
			dic.Add(228, "&auml;");
			dic.Add(229, "&aring;");
			dic.Add(230, "&aelig;");
			dic.Add(231, "&ccedil;");
			dic.Add(232, "&egrave;");
			dic.Add(233, "&eacute;");
			dic.Add(234, "&ecirc;");
			dic.Add(235, "&euml;");
			dic.Add(236, "&igrave;");
			dic.Add(237, "&iacute;");
			dic.Add(238, "&icirc;");
			dic.Add(239, "&iuml;");
			dic.Add(240, "&eth;");
			dic.Add(241, "&ntilde;");
			dic.Add(242, "&ograve;");
			dic.Add(243, "&oacute;");
			dic.Add(244, "&ocirc;");
			dic.Add(245, "&otilde;");
			dic.Add(246, "&ouml;");
			dic.Add(247, "&divide;");
			dic.Add(248, "&oslash;");
			dic.Add(249, "&ugrave;");
			dic.Add(250, "&uacute;");
			dic.Add(251, "&ucirc;");
			dic.Add(252, "&uuml;");
			dic.Add(253, "&yacute;");
			dic.Add(254, "&thorn;");
			dic.Add(255, "&yuml;");
			dic.Add(402, "&fnof;");
			dic.Add(913, "&Alpha;");
			dic.Add(914, "&Beta;");
			dic.Add(915, "&Gamma;");
			dic.Add(916, "&Delta;");
			dic.Add(917, "&Epsilon;");
			dic.Add(918, "&Zeta;");
			dic.Add(919, "&Eta;");
			dic.Add(920, "&Theta;");
			dic.Add(921, "&Iota;");
			dic.Add(922, "&Kappa;");
			dic.Add(923, "&Lambda;");
			dic.Add(924, "&Mu;");
			dic.Add(925, "&Nu;");
			dic.Add(926, "&Xi;");
			dic.Add(927, "&Omicron;");
			dic.Add(928, "&Pi;");
			dic.Add(929, "&Rho;");
			dic.Add(931, "&Sigma;");
			dic.Add(932, "&Tau;");
			dic.Add(933, "&Upsilon;");
			dic.Add(934, "&Phi;");
			dic.Add(935, "&Chi;");
			dic.Add(936, "&Psi;");
			dic.Add(937, "&Omega;");
			dic.Add(945, "&alpha;");
			dic.Add(946, "&beta;");
			dic.Add(947, "&gamma;");
			dic.Add(948, "&delta;");
			dic.Add(949, "&epsilon;");
			dic.Add(950, "&zeta;");
			dic.Add(951, "&eta;");
			dic.Add(952, "&theta;");
			dic.Add(953, "&iota;");
			dic.Add(954, "&kappa;");
			dic.Add(955, "&lambda;");
			dic.Add(956, "&mu;");
			dic.Add(957, "&nu;");
			dic.Add(958, "&xi;");
			dic.Add(959, "&omicron;");
			dic.Add(960, "&pi;");
			dic.Add(961, "&rho;");
			dic.Add(962, "&sigmaf;");
			dic.Add(963, "&sigma;");
			dic.Add(964, "&tau;");
			dic.Add(965, "&upsilon;");
			dic.Add(966, "&phi;");
			dic.Add(967, "&chi;");
			dic.Add(968, "&psi;");
			dic.Add(969, "&omega;");
			dic.Add(977, "&thetasym;");
			dic.Add(978, "&upsih;");
			dic.Add(982, "&piv;");
			dic.Add(8226, "&bull;");
			dic.Add(8230, "&hellip;");
			dic.Add(8242, "&prime;");
			dic.Add(8243, "&Prime;");
			dic.Add(8254, "&oline;");
			dic.Add(8260, "&frasl;");
			dic.Add(8472, "&weierp;");
			dic.Add(8465, "&image;");
			dic.Add(8476, "&real;");
			dic.Add(8482, "&trade;");
			dic.Add(8501, "&alefsym;");
			dic.Add(8592, "&larr;");
			dic.Add(8593, "&uarr;");
			dic.Add(8594, "&rarr;");
			dic.Add(8595, "&darr;");
			dic.Add(8596, "&harr;");
			dic.Add(8629, "&crarr;");
			dic.Add(8656, "&lArr;");
			dic.Add(8657, "&uArr;");
			dic.Add(8658, "&rArr;");
			dic.Add(8659, "&dArr;");
			dic.Add(8660, "&hArr;");
			dic.Add(8704, "&forall;");
			dic.Add(8706, "&part;");
			dic.Add(8707, "&exist;");
			dic.Add(8709, "&empty;");
			dic.Add(8711, "&nabla;");
			dic.Add(8712, "&isin;");
			dic.Add(8713, "&notin;");
			dic.Add(8715, "&ni;");
			dic.Add(8719, "&prod;");
			dic.Add(8721, "&sum;");
			dic.Add(8722, "&minus;");
			dic.Add(8727, "&lowast;");
			dic.Add(8730, "&radic;");
			dic.Add(8733, "&prop;");
			dic.Add(8734, "&infin;");
			dic.Add(8736, "&ang;");
			dic.Add(8743, "&and;");
			dic.Add(8744, "&or;");
			dic.Add(8745, "&cap;");
			dic.Add(8746, "&cup;");
			dic.Add(8747, "&int;");
			dic.Add(8756, "&there4;");
			dic.Add(8764, "&sim;");
			dic.Add(8773, "&cong;");
			dic.Add(8776, "&asymp;");
			dic.Add(8800, "&ne;");
			dic.Add(8801, "&equiv;");
			dic.Add(8804, "&le;");
			dic.Add(8805, "&ge;");
			dic.Add(8834, "&sub;");
			dic.Add(8835, "&sup;");
			dic.Add(8836, "&nsub;");
			dic.Add(8838, "&sube;");
			dic.Add(8839, "&supe;");
			dic.Add(8853, "&oplus;");
			dic.Add(8855, "&otimes;");
			dic.Add(8869, "&perp;");
			dic.Add(8901, "&sdot;");
			dic.Add(8968, "&lceil;");
			dic.Add(8969, "&rceil;");
			dic.Add(8970, "&lfloor;");
			dic.Add(8971, "&rfloor;");
			dic.Add(9001, "&lang;");
			dic.Add(9002, "&rang;");
			dic.Add(9674, "&loz;");
			dic.Add(9824, "&spades;");
			dic.Add(9827, "&clubs;");
			dic.Add(9829, "&hearts;");
			dic.Add(9830, "&diams;");
			dic.Add(338, "&OElig;");
			dic.Add(339, "&oelig;");
			dic.Add(352, "&Scaron;");
			dic.Add(353, "&scaron;");
			dic.Add(376, "&Yuml;");
			dic.Add(710, "&circ;");
			dic.Add(732, "&tilde;");
			dic.Add(8194, "&ensp;");
			dic.Add(8195, "&emsp;");
			dic.Add(8201, "&thinsp;");
			dic.Add(8204, "&zwnj;");
			dic.Add(8205, "&zwj;");
			dic.Add(8206, "&lrm;");
			dic.Add(8207, "&rlm;");
			dic.Add(8211, "&ndash;");
			dic.Add(8212, "&mdash;");
			dic.Add(8216, "&lsquo;");
			dic.Add(8217, "&rsquo;");
			dic.Add(8218, "&sbquo;");
			dic.Add(8220, "&ldquo;");
			dic.Add(8221, "&rdquo;");
			dic.Add(8222, "&bdquo;");
			dic.Add(8224, "&dagger;");
			dic.Add(8225, "&Dagger;");
			dic.Add(8240, "&permil;");
			dic.Add(8249, "&lsaquo;");
			dic.Add(8250, "&rsaquo;");
			dic.Add(8364, "&euro;");

			return dic;
		}

		public string ConvertHtmlToText(string source)
		{
			// !!! DocX에서 문자열 삽입할 때 줄바꿈 적용될 수 있도록 \r -> \n으로 일괄 수정

			string AmpStr;
			MatchCollection AmpCodes;

			HtmlToTextReplaceElement[] ReplaceStrings = new HtmlToTextReplaceElement[]
            {
                // Remove HTML Development formatting
                //Replace any white space characters (line breaks, tabs, spaces) with space because browsers inserts space 
                new HtmlToTextReplaceElement(@"\s", " ", HtmlToTextReplaceType.RegEx),
                // Remove repeating speces becuase browsers ignore them 
                new HtmlToTextReplaceElement(@" {2,}", " ", HtmlToTextReplaceType.RegEx),
                /*
                  I'm using .* in my regex from here to match "all" characters. It works here ONLY because I've removed
                  all linebreaks beforehand. If you doesn't done that you MUST replace .* with [\s\đ]* to match all characters
                  in multiple lines
                */
                // Remove HTML comment
                new HtmlToTextReplaceElement(@"<! *--.*?-- *>", " ", HtmlToTextReplaceType.RegEx),
                // Remove the header
                new HtmlToTextReplaceElement(@"< *head( *>| [^>]*>).*< */ *head *>", string.Empty, HtmlToTextReplaceType.RegEx),
                // remove all scripts
                new HtmlToTextReplaceElement(@"< *script( *>| [^>]*>).*?< */ *script *>", string.Empty, HtmlToTextReplaceType.RegEx),
                // remove all styles (prepare first by clearing attributes)
                new HtmlToTextReplaceElement(@"< *style( *>| [^>]*>).*?< */ *style *>", string.Empty, HtmlToTextReplaceType.RegEx),
                // insert tabs in spaces of <td> tags
                new HtmlToTextReplaceElement(@"< *td[^>]*>","\t", HtmlToTextReplaceType.RegEx),
                // insert line breaks in places of <BR> and <LI> tags 
                new HtmlToTextReplaceElement(@"< *(br|li) */{0,1} *>", "\n", HtmlToTextReplaceType.RegEx),
                new HtmlToTextReplaceElement(@"< *(div|tr|p)( *>| [^>]*>)", "\n\n", HtmlToTextReplaceType.RegEx),
                // Remove remaining tags like <a>, links, images, etc - anything thats enclosed inside < > 
                new HtmlToTextReplaceElement(@"<[^>]*>", string.Empty, HtmlToTextReplaceType.RegEx),
                // Replace   with whitespace. It is done here because the generated space will be used in
                // whitespace optimizations
                new HtmlToTextReplaceElement(@"&nbsp;", " ", HtmlToTextReplaceType.String),
                // Remove extra line breaks and tabs:
                // Romove any whitespace and tab at and of any line
                new HtmlToTextReplaceElement(@"[ \t]+\n", "\n", HtmlToTextReplaceType.RegEx),
                // Remove whitespace beetween tabs
                new HtmlToTextReplaceElement(@"\t +\t", "\t\t", HtmlToTextReplaceType.RegEx),
                // Remove whitespace begining of a line if followed by a tab
                new HtmlToTextReplaceElement(@"\n +\t", "\n\t", HtmlToTextReplaceType.RegEx),
                // Remove multible tabs following a linebreak with just one tab 
                new HtmlToTextReplaceElement(@"\n\t{2,}", "\n\t", HtmlToTextReplaceType.RegEx),
                // replace over 2 breaks with 2 and over 4 tabs with 4.  
                new HtmlToTextReplaceElement(@"\n{3,}", "\n\n", HtmlToTextReplaceType.RegEx),
                new HtmlToTextReplaceElement(@"\t{4,}", "\t\t\t\t", HtmlToTextReplaceType.RegEx)
            };
			string result = source;
			// run pattern matching
			for (int i = 0; i < ReplaceStrings.Length; i++)
			{
				switch (ReplaceStrings[i].Type)
				{
					case HtmlToTextReplaceType.String:
						result = result.Replace(ReplaceStrings[i].Pattern, ReplaceStrings[i].Substitute);
						break;
					case HtmlToTextReplaceType.RegEx:
						result = Regex.Replace(result, ReplaceStrings[i].Pattern, ReplaceStrings[i].Substitute, RegexOptions.IgnoreCase);
						break;
				}
			}
			// Replace decimal character codes
			AmpCodes = Regex.Matches(result, @"&#\d{1,5};", RegexOptions.IgnoreCase);
			for (int i = AmpCodes.Count - 1; i >= 0; i--)
			{
				AmpStr = AmpCodes[i].Value;
				result = result.Substring(0, AmpCodes[i].Index) +
					Convert.ToChar(Int32.Parse(AmpStr.Substring(2, AmpStr.Length - 3))) +
					result.Substring(AmpCodes[i].Index + AmpCodes[i].Length);
			}
			// Replace hexadecimal character codes
			AmpCodes = Regex.Matches(result, @"&#x[0-9a-f]{1,4};", RegexOptions.IgnoreCase);
			for (int i = AmpCodes.Count - 1; i >= 0; i--)
			{
				AmpStr = AmpCodes[i].Value;
				result = result.Substring(0, AmpCodes[i].Index) +
					Convert.ToChar(Int32.Parse(AmpStr.Substring(3, AmpStr.Length - 4), NumberStyles.AllowHexSpecifier)) +
					result.Substring(AmpCodes[i].Index + AmpCodes[i].Length);
			}
			// Replace named character codes
			AmpCodes = Regex.Matches(result, @"&\w+;", RegexOptions.IgnoreCase);
			for (int i = AmpCodes.Count - 1; i >= 0; i--)
			{
				if (_dicHtmlToText.ContainsKey(AmpCodes[i].Value))
				{
					result = result.Substring(0, AmpCodes[i].Index) +
						Convert.ToChar(_dicHtmlToText[AmpCodes[i].Value]) +
						result.Substring(AmpCodes[i].Index + AmpCodes[i].Length);
				}
			}
			// Remove all others
			result = Regex.Replace(result, @"&[^;]*;", string.Empty, RegexOptions.IgnoreCase);
			return result;
		}

		public string ConvertTextToHtml(string Value, bool ExcludeNamedCharacter)
		{
			/*	entityTable = { 34: 'quot', 38: 'amp', 39: 'apos', 60: 'lt', 62: 'gt', 160: 'nbsp', 161: 'iexcl', 162: 'cent', 163: 'pound', 164: 'curren', 165: 'yen', 166: 'brvbar', 167: 'sect', 168: 'uml', 169: 'copy', 170: 'ordf', 171: 'laquo', 172: 'not', 173: 'shy', 174: 'reg', 175: 'macr', 176: 'deg', 177: 'plusmn', 178: 'sup2', 179: 'sup3', 180: 'acute', 181: 'micro', 182: 'para', 183: 'middot', 184: 'cedil', 185: 'sup1', 186: 'ordm', 187: 'raquo', 188: 'frac14', 189: 'frac12', 190: 'frac34', 191: 'iquest', 192: 'Agrave', 193: 'Aacute', 194: 'Acirc', 195: 'Atilde', 196: 'Auml', 197: 'Aring', 198: 'AElig', 199: 'Ccedil', 200: 'Egrave', 201: 'Eacute', 202: 'Ecirc', 203: 'Euml', 204: 'Igrave', 205: 'Iacute', 206: 'Icirc', 207: 'Iuml', 208: 'ETH', 209: 'Ntilde', 210: 'Ograve', 211: 'Oacute', 212: 'Ocirc', 213: 'Otilde', 214: 'Ouml', 215: 'times', 216: 'Oslash', 217: 'Ugrave', 218: 'Uacute', 219: 'Ucirc', 220: 'Uuml', 221: 'Yacute', 222: 'THORN', 223: 'szlig', 224: 'agrave', 225: 'aacute', 226: 'acirc', 227: 'atilde', 228: 'auml', 229: 'aring', 230: 'aelig', 231: 'ccedil', 232: 'egrave', 233: 'eacute', 234: 'ecirc', 235: 'euml', 236: 'igrave', 237: 'iacute', 238: 'icirc', 239: 'iuml', 240: 'eth', 241: 'ntilde', 242: 'ograve', 243: 'oacute', 244: 'ocirc', 245: 'otilde', 246: 'ouml', 247: 'divide', 248: 'oslash', 249: 'ugrave', 250: 'uacute', 251: 'ucirc', 252: 'uuml', 253: 'yacute', 254: 'thorn', 255: 'yuml', 402: 'fnof', 913: 'Alpha', 914: 'Beta', 915: 'Gamma', 916: 'Delta', 917: 'Epsilon', 918: 'Zeta', 919: 'Eta', 920: 'Theta', 921: 'Iota', 922: 'Kappa', 923: 'Lambda', 924: 'Mu', 925: 'Nu', 926: 'Xi', 927: 'Omicron', 928: 'Pi', 929: 'Rho', 931: 'Sigma', 932: 'Tau', 933: 'Upsilon', 934: 'Phi', 935: 'Chi', 936: 'Psi', 937: 'Omega', 945: 'alpha', 946: 'beta', 947: 'gamma', 948: 'delta', 949: 'epsilon', 950: 'zeta', 951: 'eta', 952: 'theta', 953: 'iota', 954: 'kappa', 955: 'lambda', 956: 'mu', 957: 'nu', 958: 'xi', 959: 'omicron', 960: 'pi', 961: 'rho', 962: 'sigmaf', 963: 'sigma', 964: 'tau', 965: 'upsilon', 966: 'phi', 967: 'chi', 968: 'psi', 969: 'omega', 977: 'thetasym', 978: 'upsih', 982: 'piv', 8226: 'bull', 8230: 'hellip', 8242: 'prime', 8243: 'Prime', 8254: 'oline', 8260: 'frasl', 8472: 'weierp', 8465: 'image', 8476: 'real', 8482: 'trade', 8501: 'alefsym', 8592: 'larr', 8593: 'uarr', 8594: 'rarr', 8595: 'darr', 8596: 'harr', 8629: 'crarr', 8656: 'lArr', 8657: 'uArr', 8658: 'rArr', 8659: 'dArr', 8660: 'hArr', 8704: 'forall', 8706: 'part', 8707: 'exist', 8709: 'empty', 8711: 'nabla', 8712: 'isin', 8713: 'notin', 8715: 'ni', 8719: 'prod', 8721: 'sum', 8722: 'minus', 8727: 'lowast', 8730: 'radic', 8733: 'prop', 8734: 'infin', 8736: 'ang', 8743: 'and', 8744: 'or', 8745: 'cap', 8746: 'cup', 8747: 'int', 8756: 'there4', 8764: 'sim', 8773: 'cong', 8776: 'asymp', 8800: 'ne', 8801: 'equiv', 8804: 'le', 8805: 'ge', 8834: 'sub', 8835: 'sup', 8836: 'nsub', 8838: 'sube', 8839: 'supe', 8853: 'oplus', 8855: 'otimes', 8869: 'perp', 8901: 'sdot', 8968: 'lceil', 8969: 'rceil', 8970: 'lfloor', 8971: 'rfloor', 9001: 'lang', 9002: 'rang', 9674: 'loz', 9824: 'spades', 9827: 'clubs', 9829: 'hearts', 9830: 'diams', 34: 'quot', 38: 'amp', 60: 'lt', 62: 'gt', 338: 'OElig', 339: 'oelig', 352: 'Scaron', 353: 'scaron', 376: 'Yuml', 710: 'circ', 732: 'tilde', 8194: 'ensp', 8195: 'emsp', 8201: 'thinsp', 8204: 'zwnj', 8205: 'zwj', 8206: 'lrm', 8207: 'rlm', 8211: 'ndash', 8212: 'mdash', 8216: 'lsquo', 8217: 'rsquo', 8218: 'sbquo', 8220: 'ldquo', 8221: 'rdquo', 8222: 'bdquo', 8224: 'dagger', 8225: 'Dagger', 8240: 'permil', 8249: 'lsaquo', 8250: 'rsaquo', 8364: 'euro' };

				return Text.replace(/[\u00A0-\u2666<>\&]/g,
								function (c)
								{
									return '&' + (entityTable[c.charCodeAt(0)] || '#' + c.charCodeAt(0)) + ';';
								});
			*/

			return Regex.Replace(
				Value,
				@"(?<Value>[\u00A0-\u2666<>\&\n])",
				delegate(Match m)
				{
					char c = m.Groups["Value"].Value[0];

					if (_dicTextToHtml.ContainsKey(c))
					{
						if (!ExcludeNamedCharacter)
							return _dicTextToHtml[c];
						else
							return c.ToString();
					}
					else if (c == '\n')
					{
						return "<br />";
					}
					else
					{
						return "&#" + ((int)c).ToString() + ";";
					}
				});

			//return Regex.Replace(
			//	Value,
			//	@"(?<Value>[\u00A0-\u2666<>\&])",
			//	m =>
			//	{
			//		char c = m.Groups["Value"].Value[0];

			//		if (_dicTextToHtml.ContainsKey(c))
			//		{
			//			if (!ExcludeNamedCharacter)
			//				return _dicTextToHtml[c];
			//			else
			//				return c.ToString();
			//		}
			//		else if (c == '\n')
			//			return "<br />";
			//		else
			//		{
			//			return "&#" + ((int)c).ToString() + ";";
			//		}
			//	});

			//StringBuilder sb = new StringBuilder();

			////연속되는 스페이스는 두번째부터 &nbsp;로 변경
			//char CharOld = (char)0;
			//char CharCur = (char)0;
			//for (int i = 0; i < source.Length; i++)
			//{
			//    CharOld = CharCur;
			//    CharCur = source[i];

			//    if (_dicTextToHtml.ContainsKey(CharCur))
			//    {
			//        if (CharCur != ' ')
			//        {
			//            sb.Append(_dicTextToHtml[CharCur]);
			//        }
			//        else
			//        {
			//            if (CharOld == ' ')
			//                sb.Append(_dicTextToHtml[CharCur]);
			//            else
			//                sb.Append(CharCur);
			//        }
			//    }
			//    else
			//    {
			//        sb.Append(CharCur);
			//    }
			//}

			//return sb.ToString();
		}
		public string ConvertTextToHtml(string Value)
		{
			bool ExcludeNamedCharacter = false;
			return ConvertTextToHtml(Value, ExcludeNamedCharacter);
		}
		
		private enum HtmlToTextReplaceType { String, RegEx }
		private class HtmlToTextReplaceElement
		{
			public HtmlToTextReplaceElement() { }
			public HtmlToTextReplaceElement(string Pattern, string Substitute, HtmlToTextReplaceType Type)
			{
				this.Pattern = Pattern;
				this.Substitute = Substitute;
				this.Type = Type;
			}
			public string Pattern;
			public string Substitute;
			public HtmlToTextReplaceType Type;
		}
	}
}
