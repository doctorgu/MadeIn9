using System;
using System.Drawing;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace DoctorGu
{
	/// <summary>
	/// ff0000과 같은 헥사코드, Red와 같은 색이름, Color.Red와 같은 Color structure, 
	/// RGB(255, 0, 0)과 같은 VB에서 사용되는 색들 사이에 일치되는 값으로 변환하기 위함.
	/// </summary>
	/// <example>
	/// 다음은 Hexa 값을 NamedColor로 변환하고, NamedColor를 Color로 변환하고, Color를 VbColor로 변환하고, VbColor를 다시 Hexa 값으로 변환해서
	/// 처음 Hexa 값과 최종 Hexa 값이 틀린 경우에만 해당 Hexa값을 출력함.(절대 출력되지 않음)
	/// <code>
	/// object[,] ct = ColorConv.GetColorTable();
	/// for (int i = 0, i2 = ct.GetLength(1); i &lt; i2; i++)
	/// {
	/// 	//Hexa -> NamedColor -> Color -> VbColor
	/// 	string Hexa = (string)ct[0, i];
	/// 	string NamedColor = ColorConv.GetNamedColorByHexa(Hexa);
	/// 	Color c = ColorConv.GetColorByNamedColor(NamedColor);
	/// 	int vc = ColorConv.GetVbColorByColor(c);
	///	 string Hexa2 = ColorConv.GetHexaByVbColor(vc);
	/// 
	/// 	if (String.Compare(Hexa, Hexa2, true) != 0)
	/// 	{
	/// 		Console.WriteLine(Hexa);
	/// 	}
	/// }
	/// </code>
	/// </example>
	public class CColorConv
	{
		private const int ColHexa = 0, ColNamedColor = 1, ColColor = 2, ColVbColor = 3;
		private const int nNamedColor = 139;
		private static object[,] colNamedColor = new object[4, nNamedColor];

		private static string[] aHexaColor = new string[] { "000000", "000020", "000040", "000060", "000080", "0000A0", "0000C0", "0000E0", "0000FF", "002000", "002020", "002040", "002060", "002080", "0020A0", "0020C0", "0020E0", "0020FF", "004000", "004020", "004040", "004060", "004080", "0040A0", "0040C0", "0040E0", "0040FF", "006000", "006020", "006040", "006060", "006080", "0060A0", "0060C0", "0060E0", "0060FF", "008000", "008020", "008040", "008060", "008080", "0080A0", "0080C0", "0080E0", "0080FF", "00A000", "00A020", "00A040", "00A060", "00A080", "00A0A0", "00A0C0", "00A0E0", "00A0FF", "00C000", "00C020", "00C040", "00C060", "00C080", "00C0A0", "00C0C0", "00C0E0", "00C0FF", "00E000", "00E020", "00E040", "00E060", "00E080", "00E0A0", "00E0C0", "00E0E0", "00E0FF", "00FF00", "00FF20", "00FF40", "00FF60", "00FF80", "00FFA0", "00FFC0", "00FFE0", "00FFFF", "200000", "200020", "200040", "200060", "200080", "2000A0", "2000C0", "2000E0", "2000FF", "202000", "202020", "202040", "202060", "202080", "2020A0", "2020C0", "2020E0", "2020FF", "204000", "204020", "204040", "204060", "204080", "2040A0", "2040C0", "2040E0", "2040FF", "206000", "206020", "206040", "206060", "206080", "2060A0", "2060C0", "2060E0", "2060FF", "208000", "208020", "208040", "208060", "208080", "2080A0", "2080C0", "2080E0", "2080FF", "20A000", "20A020", "20A040", "20A060", "20A080", "20A0A0", "20A0C0", "20A0E0", "20A0FF", "20C000", "20C020", "20C040", "20C060", "20C080", "20C0A0", "20C0C0", "20C0E0", "20C0FF", "20E000", "20E020", "20E040", "20E060", "20E080", "20E0A0", "20E0C0", "20E0E0", "20E0FF", "20FF00", "20FF20", "20FF40", "20FF60", "20FF80", "20FFA0", "20FFC0", "20FFE0", "20FFFF", "400000", "400020", "400040", "400060", "400080", "4000A0", "4000C0", "4000E0", "4000FF", "402000", "402020", "402040", "402060", "402080", "4020A0", "4020C0", "4020E0", "4020FF", "404000", "404020", "404040", "404060", "404080", "4040A0", "4040C0", "4040E0", "4040FF", "406000", "406020", "406040", "406060", "406080", "4060A0", "4060C0", "4060E0", "4060FF", "408000", "408020", "408040", "408060", "408080", "4080A0", "4080C0", "4080E0", "4080FF", "40A000", "40A020", "40A040", "40A060", "40A080", "40A0A0", "40A0C0", "40A0E0", "40A0FF", "40C000", "40C020", "40C040", "40C060", "40C080", "40C0A0", "40C0C0", "40C0E0", "40C0FF", "40E000", "40E020", "40E040", "40E060", "40E080", "40E0A0", "40E0C0", "40E0E0", "40E0FF", "40FF00", "40FF20", "40FF40", "40FF60", "40FF80", "40FFA0", "40FFC0", "40FFE0", "40FFFF", "600000", "600020", "600040", "600060", "600080", "6000A0", "6000C0", "6000E0", "6000FF", "602000", "602020", "602040", "602060", "602080", "6020A0", "6020C0", "6020E0", "6020FF", "604000", "604020", "604040", "604060", "604080", "6040A0", "6040C0", "6040E0", "6040FF", "606000", "606020", "606040", "606060", "606080", "6060A0", "6060C0", "6060E0", "6060FF", "608000", "608020", "608040", "608060", "608080", "6080A0", "6080C0", "6080E0", "6080FF", "60A000", "60A020", "60A040", "60A060", "60A080", "60A0A0", "60A0C0", "60A0E0", "60A0FF", "60C000", "60C020", "60C040", "60C060", "60C080", "60C0A0", "60C0C0", "60C0E0", "60C0FF", "60E000", "60E020", "60E040", "60E060", "60E080", "60E0A0", "60E0C0", "60E0E0", "60E0FF", "60FF00", "60FF20", "60FF40", "60FF60", "60FF80", "60FFA0", "60FFC0", "60FFE0", "60FFFF", "800000", "800020", "800040", "800060", "800080", "8000A0", "8000C0", "8000E0", "8000FF", "802000", "802020", "802040", "802060", "802080", "8020A0", "8020C0", "8020E0", "8020FF", "804000", "804020", "804040", "804060", "804080", "8040A0", "8040C0", "8040E0", "8040FF", "806000", "806020", "806040", "806060", "806080", "8060A0", "8060C0", "8060E0", "8060FF", "808000", "808020", "808040", "808060", "808080", "8080A0", "8080C0", "8080E0", "8080FF", "80A000", "80A020", "80A040", "80A060", "80A080", "80A0A0", "80A0C0", "80A0E0", "80A0FF", "80C000", "80C020", "80C040", "80C060", "80C080", "80C0A0", "80C0C0", "80C0E0", "80C0FF", "80E000", "80E020", "80E040", "80E060", "80E080", "80E0A0", "80E0C0", "80E0E0", "80E0FF", "80FF00", "80FF20", "80FF40", "80FF60", "80FF80", "80FFA0", "80FFC0", "80FFE0", "80FFFF", "A00000", "A00020", "A00040", "A00060", "A00080", "A000A0", "A000C0", "A000E0", "A000FF", "A02000", "A02020", "A02040", "A02060", "A02080", "A020A0", "A020C0", "A020E0", "A020FF", "A04000", "A04020", "A04040", "A04060", "A04080", "A040A0", "A040C0", "A040E0", "A040FF", "A06000", "A06020", "A06040", "A06060", "A06080", "A060A0", "A060C0", "A060E0", "A060FF", "A08000", "A08020", "A08040", "A08060", "A08080", "A080A0", "A080C0", "A080E0", "A080FF", "A0A000", "A0A020", "A0A040", "A0A060", "A0A080", "A0A0A0", "A0A0C0", "A0A0E0", "A0A0FF", "A0C000", "A0C020", "A0C040", "A0C060", "A0C080", "A0C0A0", "A0C0C0", "A0C0E0", "A0C0FF", "A0E000", "A0E020", "A0E040", "A0E060", "A0E080", "A0E0A0", "A0E0C0", "A0E0E0", "A0E0FF", "A0FF00", "A0FF20", "A0FF40", "A0FF60", "A0FF80", "A0FFA0", "A0FFC0", "A0FFE0", "A0FFFF", "C00000", "C00020", "C00040", "C00060", "C00080", "C000A0", "C000C0", "C000E0", "C000FF", "C02000", "C02020", "C02040", "C02060", "C02080", "C020A0", "C020C0", "C020E0", "C020FF", "C04000", "C04020", "C04040", "C04060", "C04080", "C040A0", "C040C0", "C040E0", "C040FF", "C06000", "C06020", "C06040", "C06060", "C06080", "C060A0", "C060C0", "C060E0", "C060FF", "C08000", "C08020", "C08040", "C08060", "C08080", "C080A0", "C080C0", "C080E0", "C080FF", "C0A000", "C0A020", "C0A040", "C0A060", "C0A080", "C0A0A0", "C0A0C0", "C0A0E0", "C0A0FF", "C0C000", "C0C020", "C0C040", "C0C060", "C0C080", "C0C0A0", "C0C0C0", "C0C0E0", "C0C0FF", "C0E000", "C0E020", "C0E040", "C0E060", "C0E080", "C0E0A0", "C0E0C0", "C0E0E0", "C0E0FF", "C0FF00", "C0FF20", "C0FF40", "C0FF60", "C0FF80", "C0FFA0", "C0FFC0", "C0FFE0", "C0FFFF", "E00000", "E00020", "E00040", "E00060", "E00080", "E000A0", "E000C0", "E000E0", "E000FF", "E02000", "E02020", "E02040", "E02060", "E02080", "E020A0", "E020C0", "E020E0", "E020FF", "E04000", "E04020", "E04040", "E04060", "E04080", "E040A0", "E040C0", "E040E0", "E040FF", "E06000", "E06020", "E06040", "E06060", "E06080", "E060A0", "E060C0", "E060E0", "E060FF", "E08000", "E08020", "E08040", "E08060", "E08080", "E080A0", "E080C0", "E080E0", "E080FF", "E0A000", "E0A020", "E0A040", "E0A060", "E0A080", "E0A0A0", "E0A0C0", "E0A0E0", "E0A0FF", "E0C000", "E0C020", "E0C040", "E0C060", "E0C080", "E0C0A0", "E0C0C0", "E0C0E0", "E0C0FF", "E0E000", "E0E020", "E0E040", "E0E060", "E0E080", "E0E0A0", "E0E0C0", "E0E0E0", "E0E0FF", "E0FF00", "E0FF20", "E0FF40", "E0FF60", "E0FF80", "E0FFA0", "E0FFC0", "E0FFE0", "E0FFFF", "FF0000", "FF0020", "FF0040", "FF0060", "FF0080", "FF00A0", "FF00C0", "FF00E0", "FF00FF", "FF2000", "FF2020", "FF2040", "FF2060", "FF2080", "FF20A0", "FF20C0", "FF20E0", "FF20FF", "FF4000", "FF4020", "FF4040", "FF4060", "FF4080", "FF40A0", "FF40C0", "FF40E0", "FF40FF", "FF6000", "FF6020", "FF6040", "FF6060", "FF6080", "FF60A0", "FF60C0", "FF60E0", "FF60FF", "FF8000", "FF8020", "FF8040", "FF8060", "FF8080", "FF80A0", "FF80C0", "FF80E0", "FF80FF", "FFA000", "FFA020", "FFA040", "FFA060", "FFA080", "FFA0A0", "FFA0C0", "FFA0E0", "FFA0FF", "FFC000", "FFC020", "FFC040", "FFC060", "FFC080", "FFC0A0", "FFC0C0", "FFC0E0", "FFC0FF", "FFE000", "FFE020", "FFE040", "FFE060", "FFE080", "FFE0A0", "FFE0C0", "FFE0E0", "FFE0FF", "FFFF00", "FFFF20", "FFFF40", "FFFF60", "FFFF80", "FFFFA0", "FFFFC0", "FFFFE0", "FFFFFF" };
		
		//static 메쏘드만을 호출할 것이므로 static 생성자를 사용해서 cols에 값을 입력함.
		static CColorConv()
		{
			int i = -1;

			i++; colNamedColor[ColHexa, i] = "000000"; colNamedColor[ColNamedColor, i] = "Black"; colNamedColor[ColColor, i] = Color.Black; colNamedColor[ColVbColor, i] = 0;
			i++; colNamedColor[ColHexa, i] = "ffffff"; colNamedColor[ColNamedColor, i] = "White"; colNamedColor[ColColor, i] = Color.White; colNamedColor[ColVbColor, i] = 16777215;
			i++; colNamedColor[ColHexa, i] = "ffffff"; colNamedColor[ColNamedColor, i] = "Transparent"; colNamedColor[ColColor, i] = Color.Transparent; colNamedColor[ColVbColor, i] = 16777215;
			i++; colNamedColor[ColHexa, i] = "808080"; colNamedColor[ColNamedColor, i] = "Gray"; colNamedColor[ColColor, i] = Color.Gray; colNamedColor[ColVbColor, i] = 8421504;
			i++; colNamedColor[ColHexa, i] = "c0c0c0"; colNamedColor[ColNamedColor, i] = "Silver"; colNamedColor[ColColor, i] = Color.Silver; colNamedColor[ColVbColor, i] = 12632256;
			i++; colNamedColor[ColHexa, i] = "ff0000"; colNamedColor[ColNamedColor, i] = "Red"; colNamedColor[ColColor, i] = Color.Red; colNamedColor[ColVbColor, i] = 255;
			i++; colNamedColor[ColHexa, i] = "008000"; colNamedColor[ColNamedColor, i] = "Green"; colNamedColor[ColColor, i] = Color.Green; colNamedColor[ColVbColor, i] = 32768;
			i++; colNamedColor[ColHexa, i] = "0000ff"; colNamedColor[ColNamedColor, i] = "Blue"; colNamedColor[ColColor, i] = Color.Blue; colNamedColor[ColVbColor, i] = 16711680;
			i++; colNamedColor[ColHexa, i] = "ffff00"; colNamedColor[ColNamedColor, i] = "Yellow"; colNamedColor[ColColor, i] = Color.Yellow; colNamedColor[ColVbColor, i] = 65535;
			i++; colNamedColor[ColHexa, i] = "800080"; colNamedColor[ColNamedColor, i] = "Purple"; colNamedColor[ColColor, i] = Color.Purple; colNamedColor[ColVbColor, i] = 8388736;
			i++; colNamedColor[ColHexa, i] = "808000"; colNamedColor[ColNamedColor, i] = "Olive"; colNamedColor[ColColor, i] = Color.Olive; colNamedColor[ColVbColor, i] = 32896;
			i++; colNamedColor[ColHexa, i] = "000080"; colNamedColor[ColNamedColor, i] = "Navy"; colNamedColor[ColColor, i] = Color.Navy; colNamedColor[ColVbColor, i] = 8388608;
			i++; colNamedColor[ColHexa, i] = "00ffff"; colNamedColor[ColNamedColor, i] = "Aqua"; colNamedColor[ColColor, i] = Color.Aqua; colNamedColor[ColVbColor, i] = 16776960;
			i++; colNamedColor[ColHexa, i] = "00ff00"; colNamedColor[ColNamedColor, i] = "Lime"; colNamedColor[ColColor, i] = Color.Lime; colNamedColor[ColVbColor, i] = 65280;
			i++; colNamedColor[ColHexa, i] = "800000"; colNamedColor[ColNamedColor, i] = "Maroon"; colNamedColor[ColColor, i] = Color.Maroon; colNamedColor[ColVbColor, i] = 128;
			i++; colNamedColor[ColHexa, i] = "008080"; colNamedColor[ColNamedColor, i] = "Teal"; colNamedColor[ColColor, i] = Color.Teal; colNamedColor[ColVbColor, i] = 8421376;
			i++; colNamedColor[ColHexa, i] = "ff00ff"; colNamedColor[ColNamedColor, i] = "Fuchsia"; colNamedColor[ColColor, i] = Color.Fuchsia; colNamedColor[ColVbColor, i] = 16711935;

			i++; colNamedColor[ColHexa, i] = "006400"; colNamedColor[ColNamedColor, i] = "DarkGreen"; colNamedColor[ColColor, i] = Color.DarkGreen; colNamedColor[ColVbColor, i] = 25600;
			i++; colNamedColor[ColHexa, i] = "191970"; colNamedColor[ColNamedColor, i] = "MidnightBlue"; colNamedColor[ColColor, i] = Color.MidnightBlue; colNamedColor[ColVbColor, i] = 7346457;
			i++; colNamedColor[ColHexa, i] = "696969"; colNamedColor[ColNamedColor, i] = "DimGray"; colNamedColor[ColColor, i] = Color.DimGray; colNamedColor[ColVbColor, i] = 6908265;
			i++; colNamedColor[ColHexa, i] = "708090"; colNamedColor[ColNamedColor, i] = "SlateGray"; colNamedColor[ColColor, i] = Color.SlateGray; colNamedColor[ColVbColor, i] = 9470064;
			i++; colNamedColor[ColHexa, i] = "778899"; colNamedColor[ColNamedColor, i] = "LightSlateGray"; colNamedColor[ColColor, i] = Color.LightSlateGray; colNamedColor[ColVbColor, i] = 10061943;
			i++; colNamedColor[ColHexa, i] = "00008b"; colNamedColor[ColNamedColor, i] = "DarkBlue"; colNamedColor[ColColor, i] = Color.DarkBlue; colNamedColor[ColVbColor, i] = 9109504;
			i++; colNamedColor[ColHexa, i] = "0000cd"; colNamedColor[ColNamedColor, i] = "MediumBlue"; colNamedColor[ColColor, i] = Color.MediumBlue; colNamedColor[ColVbColor, i] = 13434880;
			i++; colNamedColor[ColHexa, i] = "008b8b"; colNamedColor[ColNamedColor, i] = "DarkCyan"; colNamedColor[ColColor, i] = Color.DarkCyan; colNamedColor[ColVbColor, i] = 9145088;
			i++; colNamedColor[ColHexa, i] = "00bfff"; colNamedColor[ColNamedColor, i] = "DeepSkyBlue"; colNamedColor[ColColor, i] = Color.DeepSkyBlue; colNamedColor[ColVbColor, i] = 16760576;
			i++; colNamedColor[ColHexa, i] = "00ced1"; colNamedColor[ColNamedColor, i] = "DarkTurquoise"; colNamedColor[ColColor, i] = Color.DarkTurquoise; colNamedColor[ColVbColor, i] = 13749760;
			i++; colNamedColor[ColHexa, i] = "00fa9a"; colNamedColor[ColNamedColor, i] = "MediumSpringGreen"; colNamedColor[ColColor, i] = Color.MediumSpringGreen; colNamedColor[ColVbColor, i] = 10156544;
			i++; colNamedColor[ColHexa, i] = "00ff7f"; colNamedColor[ColNamedColor, i] = "SpringGreen"; colNamedColor[ColColor, i] = Color.SpringGreen; colNamedColor[ColVbColor, i] = 8388352;
			i++; colNamedColor[ColHexa, i] = "1e90ff"; colNamedColor[ColNamedColor, i] = "DodgerBlue"; colNamedColor[ColColor, i] = Color.DodgerBlue; colNamedColor[ColVbColor, i] = 16748574;
			i++; colNamedColor[ColHexa, i] = "20b2aa"; colNamedColor[ColNamedColor, i] = "LightSeaGreen"; colNamedColor[ColColor, i] = Color.LightSeaGreen; colNamedColor[ColVbColor, i] = 11186720;
			i++; colNamedColor[ColHexa, i] = "228b22"; colNamedColor[ColNamedColor, i] = "ForestGreen"; colNamedColor[ColColor, i] = Color.ForestGreen; colNamedColor[ColVbColor, i] = 2263842;
			i++; colNamedColor[ColHexa, i] = "2e8b57"; colNamedColor[ColNamedColor, i] = "SeaGreen"; colNamedColor[ColColor, i] = Color.SeaGreen; colNamedColor[ColVbColor, i] = 5737262;
			i++; colNamedColor[ColHexa, i] = "2f4f4f"; colNamedColor[ColNamedColor, i] = "DarkSlateGray"; colNamedColor[ColColor, i] = Color.DarkSlateGray; colNamedColor[ColVbColor, i] = 5197615;
			i++; colNamedColor[ColHexa, i] = "32cd32"; colNamedColor[ColNamedColor, i] = "LimeGreen"; colNamedColor[ColColor, i] = Color.LimeGreen; colNamedColor[ColVbColor, i] = 3329330;
			i++; colNamedColor[ColHexa, i] = "3cb371"; colNamedColor[ColNamedColor, i] = "MediumSeaGreen"; colNamedColor[ColColor, i] = Color.MediumSeaGreen; colNamedColor[ColVbColor, i] = 7451452;
			i++; colNamedColor[ColHexa, i] = "40e0d0"; colNamedColor[ColNamedColor, i] = "Turquoise"; colNamedColor[ColColor, i] = Color.Turquoise; colNamedColor[ColVbColor, i] = 13688896;
			i++; colNamedColor[ColHexa, i] = "4169e1"; colNamedColor[ColNamedColor, i] = "RoyalBlue"; colNamedColor[ColColor, i] = Color.RoyalBlue; colNamedColor[ColVbColor, i] = 14772545;
			i++; colNamedColor[ColHexa, i] = "4682b4"; colNamedColor[ColNamedColor, i] = "SteelBlue"; colNamedColor[ColColor, i] = Color.SteelBlue; colNamedColor[ColVbColor, i] = 11829830;
			i++; colNamedColor[ColHexa, i] = "483d8b"; colNamedColor[ColNamedColor, i] = "DarkSlateBlue"; colNamedColor[ColColor, i] = Color.DarkSlateBlue; colNamedColor[ColVbColor, i] = 9125192;
			i++; colNamedColor[ColHexa, i] = "48d1cc"; colNamedColor[ColNamedColor, i] = "MediumTurquoise"; colNamedColor[ColColor, i] = Color.MediumTurquoise; colNamedColor[ColVbColor, i] = 13422920;
			i++; colNamedColor[ColHexa, i] = "4b0082"; colNamedColor[ColNamedColor, i] = "Indigo"; colNamedColor[ColColor, i] = Color.Indigo; colNamedColor[ColVbColor, i] = 8519755;
			i++; colNamedColor[ColHexa, i] = "556b2f"; colNamedColor[ColNamedColor, i] = "DarkOliveGreen"; colNamedColor[ColColor, i] = Color.DarkOliveGreen; colNamedColor[ColVbColor, i] = 3107669;
			i++; colNamedColor[ColHexa, i] = "5f9ea0"; colNamedColor[ColNamedColor, i] = "CadetBlue"; colNamedColor[ColColor, i] = Color.CadetBlue; colNamedColor[ColVbColor, i] = 10526303;
			i++; colNamedColor[ColHexa, i] = "6495ed"; colNamedColor[ColNamedColor, i] = "CornflowerBlue"; colNamedColor[ColColor, i] = Color.CornflowerBlue; colNamedColor[ColVbColor, i] = 15570276;
			i++; colNamedColor[ColHexa, i] = "66cdaa"; colNamedColor[ColNamedColor, i] = "MediumAquamarine"; colNamedColor[ColColor, i] = Color.MediumAquamarine; colNamedColor[ColVbColor, i] = 11193702;
			i++; colNamedColor[ColHexa, i] = "6a5acd"; colNamedColor[ColNamedColor, i] = "SlateBlue"; colNamedColor[ColColor, i] = Color.SlateBlue; colNamedColor[ColVbColor, i] = 13458026;
			i++; colNamedColor[ColHexa, i] = "6b8e23"; colNamedColor[ColNamedColor, i] = "OliveDrab"; colNamedColor[ColColor, i] = Color.OliveDrab; colNamedColor[ColVbColor, i] = 2330219;
			i++; colNamedColor[ColHexa, i] = "7b68ee"; colNamedColor[ColNamedColor, i] = "MediumSlateBlue"; colNamedColor[ColColor, i] = Color.MediumSlateBlue; colNamedColor[ColVbColor, i] = 15624315;
			i++; colNamedColor[ColHexa, i] = "7cfc00"; colNamedColor[ColNamedColor, i] = "LawnGreen"; colNamedColor[ColColor, i] = Color.LawnGreen; colNamedColor[ColVbColor, i] = 64636;
			i++; colNamedColor[ColHexa, i] = "7fff00"; colNamedColor[ColNamedColor, i] = "Chartreuse"; colNamedColor[ColColor, i] = Color.Chartreuse; colNamedColor[ColVbColor, i] = 65407;
			i++; colNamedColor[ColHexa, i] = "7fffd4"; colNamedColor[ColNamedColor, i] = "Aquamarine"; colNamedColor[ColColor, i] = Color.Aquamarine; colNamedColor[ColVbColor, i] = 13959039;
			i++; colNamedColor[ColHexa, i] = "87ceeb"; colNamedColor[ColNamedColor, i] = "SkyBlue"; colNamedColor[ColColor, i] = Color.SkyBlue; colNamedColor[ColVbColor, i] = 15453831;
			i++; colNamedColor[ColHexa, i] = "87cefa"; colNamedColor[ColNamedColor, i] = "LightSkyBlue"; colNamedColor[ColColor, i] = Color.LightSkyBlue; colNamedColor[ColVbColor, i] = 16436871;
			i++; colNamedColor[ColHexa, i] = "8a2be2"; colNamedColor[ColNamedColor, i] = "BlueViolet"; colNamedColor[ColColor, i] = Color.BlueViolet; colNamedColor[ColVbColor, i] = 14822282;
			i++; colNamedColor[ColHexa, i] = "8b0000"; colNamedColor[ColNamedColor, i] = "DarkRed"; colNamedColor[ColColor, i] = Color.DarkRed; colNamedColor[ColVbColor, i] = 139;
			i++; colNamedColor[ColHexa, i] = "8b008b"; colNamedColor[ColNamedColor, i] = "DarkMagenta"; colNamedColor[ColColor, i] = Color.DarkMagenta; colNamedColor[ColVbColor, i] = 9109643;
			i++; colNamedColor[ColHexa, i] = "8b4513"; colNamedColor[ColNamedColor, i] = "SaddleBrown"; colNamedColor[ColColor, i] = Color.SaddleBrown; colNamedColor[ColVbColor, i] = 1262987;
			i++; colNamedColor[ColHexa, i] = "8fbc8b"; colNamedColor[ColNamedColor, i] = "DarkSeaGreen"; colNamedColor[ColColor, i] = Color.DarkSeaGreen; colNamedColor[ColVbColor, i] = 9157775;
			i++; colNamedColor[ColHexa, i] = "90ee90"; colNamedColor[ColNamedColor, i] = "LightGreen"; colNamedColor[ColColor, i] = Color.LightGreen; colNamedColor[ColVbColor, i] = 9498256;
			i++; colNamedColor[ColHexa, i] = "9370db"; colNamedColor[ColNamedColor, i] = "MediumPurple"; colNamedColor[ColColor, i] = Color.MediumPurple; colNamedColor[ColVbColor, i] = 14381203;
			i++; colNamedColor[ColHexa, i] = "9400d3"; colNamedColor[ColNamedColor, i] = "DarkViolet"; colNamedColor[ColColor, i] = Color.DarkViolet; colNamedColor[ColVbColor, i] = 13828244;
			i++; colNamedColor[ColHexa, i] = "98fb98"; colNamedColor[ColNamedColor, i] = "PaleGreen"; colNamedColor[ColColor, i] = Color.PaleGreen; colNamedColor[ColVbColor, i] = 10025880;
			i++; colNamedColor[ColHexa, i] = "9932cc"; colNamedColor[ColNamedColor, i] = "DarkOrchid"; colNamedColor[ColColor, i] = Color.DarkOrchid; colNamedColor[ColVbColor, i] = 13382297;
			i++; colNamedColor[ColHexa, i] = "9acd32"; colNamedColor[ColNamedColor, i] = "YellowGreen"; colNamedColor[ColColor, i] = Color.YellowGreen; colNamedColor[ColVbColor, i] = 3329434;
			i++; colNamedColor[ColHexa, i] = "a0522d"; colNamedColor[ColNamedColor, i] = "Sienna"; colNamedColor[ColColor, i] = Color.Sienna; colNamedColor[ColVbColor, i] = 2970272;
			i++; colNamedColor[ColHexa, i] = "a52a2a"; colNamedColor[ColNamedColor, i] = "Brown"; colNamedColor[ColColor, i] = Color.Brown; colNamedColor[ColVbColor, i] = 2763429;
			i++; colNamedColor[ColHexa, i] = "a9a9a9"; colNamedColor[ColNamedColor, i] = "DarkGray"; colNamedColor[ColColor, i] = Color.DarkGray; colNamedColor[ColVbColor, i] = 11119017;
			i++; colNamedColor[ColHexa, i] = "add8e6"; colNamedColor[ColNamedColor, i] = "LightBlue"; colNamedColor[ColColor, i] = Color.LightBlue; colNamedColor[ColVbColor, i] = 15128749;
			i++; colNamedColor[ColHexa, i] = "adff2f"; colNamedColor[ColNamedColor, i] = "GreenYellow"; colNamedColor[ColColor, i] = Color.GreenYellow; colNamedColor[ColVbColor, i] = 3145645;
			i++; colNamedColor[ColHexa, i] = "afeeee"; colNamedColor[ColNamedColor, i] = "PaleTurquoise"; colNamedColor[ColColor, i] = Color.PaleTurquoise; colNamedColor[ColVbColor, i] = 15658671;
			i++; colNamedColor[ColHexa, i] = "b0c4de"; colNamedColor[ColNamedColor, i] = "LightSteelBlue"; colNamedColor[ColColor, i] = Color.LightSteelBlue; colNamedColor[ColVbColor, i] = 14599344;
			i++; colNamedColor[ColHexa, i] = "b0e0e6"; colNamedColor[ColNamedColor, i] = "PowderBlue"; colNamedColor[ColColor, i] = Color.PowderBlue; colNamedColor[ColVbColor, i] = 15130800;
			i++; colNamedColor[ColHexa, i] = "b22222"; colNamedColor[ColNamedColor, i] = "Firebrick"; colNamedColor[ColColor, i] = Color.Firebrick; colNamedColor[ColVbColor, i] = 2237106;
			i++; colNamedColor[ColHexa, i] = "b8860b"; colNamedColor[ColNamedColor, i] = "DarkGoldenrod"; colNamedColor[ColColor, i] = Color.DarkGoldenrod; colNamedColor[ColVbColor, i] = 755384;
			i++; colNamedColor[ColHexa, i] = "ba55d3"; colNamedColor[ColNamedColor, i] = "MediumOrchid"; colNamedColor[ColColor, i] = Color.MediumOrchid; colNamedColor[ColVbColor, i] = 13850042;
			i++; colNamedColor[ColHexa, i] = "bc8f8f"; colNamedColor[ColNamedColor, i] = "RosyBrown"; colNamedColor[ColColor, i] = Color.RosyBrown; colNamedColor[ColVbColor, i] = 9408444;
			i++; colNamedColor[ColHexa, i] = "bdb76b"; colNamedColor[ColNamedColor, i] = "DarkKhaki"; colNamedColor[ColColor, i] = Color.DarkKhaki; colNamedColor[ColVbColor, i] = 7059389;
			i++; colNamedColor[ColHexa, i] = "c71585"; colNamedColor[ColNamedColor, i] = "MediumVioletRed"; colNamedColor[ColColor, i] = Color.MediumVioletRed; colNamedColor[ColVbColor, i] = 8721863;
			i++; colNamedColor[ColHexa, i] = "cd5c5c"; colNamedColor[ColNamedColor, i] = "IndianRed"; colNamedColor[ColColor, i] = Color.IndianRed; colNamedColor[ColVbColor, i] = 6053069;
			i++; colNamedColor[ColHexa, i] = "cd853f"; colNamedColor[ColNamedColor, i] = "Peru"; colNamedColor[ColColor, i] = Color.Peru; colNamedColor[ColVbColor, i] = 4163021;
			i++; colNamedColor[ColHexa, i] = "d2691e"; colNamedColor[ColNamedColor, i] = "Chocolate"; colNamedColor[ColColor, i] = Color.Chocolate; colNamedColor[ColVbColor, i] = 1993170;
			i++; colNamedColor[ColHexa, i] = "d2b48c"; colNamedColor[ColNamedColor, i] = "Tan"; colNamedColor[ColColor, i] = Color.Tan; colNamedColor[ColVbColor, i] = 9221330;
			i++; colNamedColor[ColHexa, i] = "d3d3d3"; colNamedColor[ColNamedColor, i] = "LightGray"; colNamedColor[ColColor, i] = Color.LightGray; colNamedColor[ColVbColor, i] = 13882323;
			i++; colNamedColor[ColHexa, i] = "d8bfd8"; colNamedColor[ColNamedColor, i] = "Thistle"; colNamedColor[ColColor, i] = Color.Thistle; colNamedColor[ColVbColor, i] = 14204888;
			i++; colNamedColor[ColHexa, i] = "da70d6"; colNamedColor[ColNamedColor, i] = "Orchid"; colNamedColor[ColColor, i] = Color.Orchid; colNamedColor[ColVbColor, i] = 14053594;
			i++; colNamedColor[ColHexa, i] = "daa520"; colNamedColor[ColNamedColor, i] = "Goldenrod"; colNamedColor[ColColor, i] = Color.Goldenrod; colNamedColor[ColVbColor, i] = 2139610;
			i++; colNamedColor[ColHexa, i] = "db7093"; colNamedColor[ColNamedColor, i] = "PaleVioletRed"; colNamedColor[ColColor, i] = Color.PaleVioletRed; colNamedColor[ColVbColor, i] = 9662683;
			i++; colNamedColor[ColHexa, i] = "dc143c"; colNamedColor[ColNamedColor, i] = "Crimson"; colNamedColor[ColColor, i] = Color.Crimson; colNamedColor[ColVbColor, i] = 3937500;
			i++; colNamedColor[ColHexa, i] = "dcdcdc"; colNamedColor[ColNamedColor, i] = "Gainsboro"; colNamedColor[ColColor, i] = Color.Gainsboro; colNamedColor[ColVbColor, i] = 14474460;
			i++; colNamedColor[ColHexa, i] = "dda0dd"; colNamedColor[ColNamedColor, i] = "Plum"; colNamedColor[ColColor, i] = Color.Plum; colNamedColor[ColVbColor, i] = 14524637;
			i++; colNamedColor[ColHexa, i] = "deb887"; colNamedColor[ColNamedColor, i] = "BurlyWood"; colNamedColor[ColColor, i] = Color.BurlyWood; colNamedColor[ColVbColor, i] = 8894686;
			i++; colNamedColor[ColHexa, i] = "e0ffff"; colNamedColor[ColNamedColor, i] = "LightCyan"; colNamedColor[ColColor, i] = Color.LightCyan; colNamedColor[ColVbColor, i] = 16777184;
			i++; colNamedColor[ColHexa, i] = "e6e6fa"; colNamedColor[ColNamedColor, i] = "Lavender"; colNamedColor[ColColor, i] = Color.Lavender; colNamedColor[ColVbColor, i] = 16443110;
			i++; colNamedColor[ColHexa, i] = "e9967a"; colNamedColor[ColNamedColor, i] = "DarkSalmon"; colNamedColor[ColColor, i] = Color.DarkSalmon; colNamedColor[ColVbColor, i] = 8034025;
			i++; colNamedColor[ColHexa, i] = "ee82ee"; colNamedColor[ColNamedColor, i] = "Violet"; colNamedColor[ColColor, i] = Color.Violet; colNamedColor[ColVbColor, i] = 15631086;
			i++; colNamedColor[ColHexa, i] = "eee8aa"; colNamedColor[ColNamedColor, i] = "PaleGoldenrod"; colNamedColor[ColColor, i] = Color.PaleGoldenrod; colNamedColor[ColVbColor, i] = 11200750;
			i++; colNamedColor[ColHexa, i] = "f08080"; colNamedColor[ColNamedColor, i] = "LightCoral"; colNamedColor[ColColor, i] = Color.LightCoral; colNamedColor[ColVbColor, i] = 8421616;
			i++; colNamedColor[ColHexa, i] = "f0e68c"; colNamedColor[ColNamedColor, i] = "Khaki"; colNamedColor[ColColor, i] = Color.Khaki; colNamedColor[ColVbColor, i] = 9234160;
			i++; colNamedColor[ColHexa, i] = "f0f8ff"; colNamedColor[ColNamedColor, i] = "AliceBlue"; colNamedColor[ColColor, i] = Color.AliceBlue; colNamedColor[ColVbColor, i] = 16775408;
			i++; colNamedColor[ColHexa, i] = "f0fff0"; colNamedColor[ColNamedColor, i] = "Honeydew"; colNamedColor[ColColor, i] = Color.Honeydew; colNamedColor[ColVbColor, i] = 15794160;
			i++; colNamedColor[ColHexa, i] = "f0ffff"; colNamedColor[ColNamedColor, i] = "Azure"; colNamedColor[ColColor, i] = Color.Azure; colNamedColor[ColVbColor, i] = 16777200;
			i++; colNamedColor[ColHexa, i] = "f4a460"; colNamedColor[ColNamedColor, i] = "SandyBrown"; colNamedColor[ColColor, i] = Color.SandyBrown; colNamedColor[ColVbColor, i] = 6333684;
			i++; colNamedColor[ColHexa, i] = "f5deb3"; colNamedColor[ColNamedColor, i] = "Wheat"; colNamedColor[ColColor, i] = Color.Wheat; colNamedColor[ColVbColor, i] = 11788021;
			i++; colNamedColor[ColHexa, i] = "f5f5dc"; colNamedColor[ColNamedColor, i] = "Beige"; colNamedColor[ColColor, i] = Color.Beige; colNamedColor[ColVbColor, i] = 14480885;
			i++; colNamedColor[ColHexa, i] = "f5f5f5"; colNamedColor[ColNamedColor, i] = "WhiteSmoke"; colNamedColor[ColColor, i] = Color.WhiteSmoke; colNamedColor[ColVbColor, i] = 16119285;
			i++; colNamedColor[ColHexa, i] = "f5fffa"; colNamedColor[ColNamedColor, i] = "MintCream"; colNamedColor[ColColor, i] = Color.MintCream; colNamedColor[ColVbColor, i] = 16449525;
			i++; colNamedColor[ColHexa, i] = "f8f8ff"; colNamedColor[ColNamedColor, i] = "GhostWhite"; colNamedColor[ColColor, i] = Color.GhostWhite; colNamedColor[ColVbColor, i] = 16775416;
			i++; colNamedColor[ColHexa, i] = "fa8072"; colNamedColor[ColNamedColor, i] = "Salmon"; colNamedColor[ColColor, i] = Color.Salmon; colNamedColor[ColVbColor, i] = 7504122;
			i++; colNamedColor[ColHexa, i] = "faebd7"; colNamedColor[ColNamedColor, i] = "AntiqueWhite"; colNamedColor[ColColor, i] = Color.AntiqueWhite; colNamedColor[ColVbColor, i] = 14150650;
			i++; colNamedColor[ColHexa, i] = "faf0e6"; colNamedColor[ColNamedColor, i] = "Linen"; colNamedColor[ColColor, i] = Color.Linen; colNamedColor[ColVbColor, i] = 15134970;
			i++; colNamedColor[ColHexa, i] = "fafad2"; colNamedColor[ColNamedColor, i] = "LightGoldenrodYellow"; colNamedColor[ColColor, i] = Color.LightGoldenrodYellow; colNamedColor[ColVbColor, i] = 13826810;
			i++; colNamedColor[ColHexa, i] = "fdf5e6"; colNamedColor[ColNamedColor, i] = "OldLace"; colNamedColor[ColColor, i] = Color.OldLace; colNamedColor[ColVbColor, i] = 15136253;
			i++; colNamedColor[ColHexa, i] = "ff1493"; colNamedColor[ColNamedColor, i] = "DeepPink"; colNamedColor[ColColor, i] = Color.DeepPink; colNamedColor[ColVbColor, i] = 9639167;
			i++; colNamedColor[ColHexa, i] = "ff4500"; colNamedColor[ColNamedColor, i] = "OrangeRed"; colNamedColor[ColColor, i] = Color.OrangeRed; colNamedColor[ColVbColor, i] = 17919;
			i++; colNamedColor[ColHexa, i] = "ff6347"; colNamedColor[ColNamedColor, i] = "Tomato"; colNamedColor[ColColor, i] = Color.Tomato; colNamedColor[ColVbColor, i] = 4678655;
			i++; colNamedColor[ColHexa, i] = "ff69b4"; colNamedColor[ColNamedColor, i] = "HotPink"; colNamedColor[ColColor, i] = Color.HotPink; colNamedColor[ColVbColor, i] = 11823615;
			i++; colNamedColor[ColHexa, i] = "ff7f50"; colNamedColor[ColNamedColor, i] = "Coral"; colNamedColor[ColColor, i] = Color.Coral; colNamedColor[ColVbColor, i] = 5275647;
			i++; colNamedColor[ColHexa, i] = "ff8c00"; colNamedColor[ColNamedColor, i] = "DarkOrange"; colNamedColor[ColColor, i] = Color.DarkOrange; colNamedColor[ColVbColor, i] = 36095;
			i++; colNamedColor[ColHexa, i] = "ffa07a"; colNamedColor[ColNamedColor, i] = "LightSalmon"; colNamedColor[ColColor, i] = Color.LightSalmon; colNamedColor[ColVbColor, i] = 8036607;
			i++; colNamedColor[ColHexa, i] = "ffa500"; colNamedColor[ColNamedColor, i] = "Orange"; colNamedColor[ColColor, i] = Color.Orange; colNamedColor[ColVbColor, i] = 42495;
			i++; colNamedColor[ColHexa, i] = "ffb6c1"; colNamedColor[ColNamedColor, i] = "LightPink"; colNamedColor[ColColor, i] = Color.LightPink; colNamedColor[ColVbColor, i] = 12695295;
			i++; colNamedColor[ColHexa, i] = "ffc0cb"; colNamedColor[ColNamedColor, i] = "Pink"; colNamedColor[ColColor, i] = Color.Pink; colNamedColor[ColVbColor, i] = 13353215;
			i++; colNamedColor[ColHexa, i] = "ffd700"; colNamedColor[ColNamedColor, i] = "Gold"; colNamedColor[ColColor, i] = Color.Gold; colNamedColor[ColVbColor, i] = 55295;
			i++; colNamedColor[ColHexa, i] = "ffdab9"; colNamedColor[ColNamedColor, i] = "PeachPuff"; colNamedColor[ColColor, i] = Color.PeachPuff; colNamedColor[ColVbColor, i] = 12180223;
			i++; colNamedColor[ColHexa, i] = "ffdead"; colNamedColor[ColNamedColor, i] = "NavajoWhite"; colNamedColor[ColColor, i] = Color.NavajoWhite; colNamedColor[ColVbColor, i] = 11394815;
			i++; colNamedColor[ColHexa, i] = "ffe4b5"; colNamedColor[ColNamedColor, i] = "Moccasin"; colNamedColor[ColColor, i] = Color.Moccasin; colNamedColor[ColVbColor, i] = 11920639;
			i++; colNamedColor[ColHexa, i] = "ffe4c4"; colNamedColor[ColNamedColor, i] = "Bisque"; colNamedColor[ColColor, i] = Color.Bisque; colNamedColor[ColVbColor, i] = 12903679;
			i++; colNamedColor[ColHexa, i] = "ffe4e1"; colNamedColor[ColNamedColor, i] = "MistyRose"; colNamedColor[ColColor, i] = Color.MistyRose; colNamedColor[ColVbColor, i] = 14804223;
			i++; colNamedColor[ColHexa, i] = "ffebcd"; colNamedColor[ColNamedColor, i] = "BlanchedAlmond"; colNamedColor[ColColor, i] = Color.BlanchedAlmond; colNamedColor[ColVbColor, i] = 13495295;
			i++; colNamedColor[ColHexa, i] = "ffefd5"; colNamedColor[ColNamedColor, i] = "PapayaWhip"; colNamedColor[ColColor, i] = Color.PapayaWhip; colNamedColor[ColVbColor, i] = 14020607;
			i++; colNamedColor[ColHexa, i] = "fff0f5"; colNamedColor[ColNamedColor, i] = "LavenderBlush"; colNamedColor[ColColor, i] = Color.LavenderBlush; colNamedColor[ColVbColor, i] = 16118015;
			i++; colNamedColor[ColHexa, i] = "fff5ee"; colNamedColor[ColNamedColor, i] = "SeaShell"; colNamedColor[ColColor, i] = Color.SeaShell; colNamedColor[ColVbColor, i] = 15660543;
			i++; colNamedColor[ColHexa, i] = "fff8dc"; colNamedColor[ColNamedColor, i] = "Cornsilk"; colNamedColor[ColColor, i] = Color.Cornsilk; colNamedColor[ColVbColor, i] = 14481663;
			i++; colNamedColor[ColHexa, i] = "fffacd"; colNamedColor[ColNamedColor, i] = "LemonChiffon"; colNamedColor[ColColor, i] = Color.LemonChiffon; colNamedColor[ColVbColor, i] = 13499135;
			i++; colNamedColor[ColHexa, i] = "fffaf0"; colNamedColor[ColNamedColor, i] = "FloralWhite"; colNamedColor[ColColor, i] = Color.FloralWhite; colNamedColor[ColVbColor, i] = 15792895;
			i++; colNamedColor[ColHexa, i] = "fffafa"; colNamedColor[ColNamedColor, i] = "Snow"; colNamedColor[ColColor, i] = Color.Snow; colNamedColor[ColVbColor, i] = 16448255;
			i++; colNamedColor[ColHexa, i] = "ffffe0"; colNamedColor[ColNamedColor, i] = "LightYellow"; colNamedColor[ColColor, i] = Color.LightYellow; colNamedColor[ColVbColor, i] = 14745599;
			i++; colNamedColor[ColHexa, i] = "fffff0"; colNamedColor[ColNamedColor, i] = "Ivory"; colNamedColor[ColColor, i] = Color.Ivory; colNamedColor[ColVbColor, i] = 15794175;

		}

		/// <summary>
		/// 모든 Color 목록을 2차원 배열 형태로 리턴함.
		/// </summary>
		/// <returns>2차원 배열</returns>
		public static object[,] GetColorTable()
		{
			return colNamedColor;
		}
		public static string[] GetColorTableHexa()
		{
			return aHexaColor;
		}

		/// <summary>
		/// Hexa 값에 해당하는 Blue, Red 등의 색 이름을 리턴함.
		/// </summary>
		public static string GetNamedColorByHexa(string Hexa)
		{
			Hexa = NormalizeHexa(Hexa);
			if (Hexa == "")
				return "";

			for (int i = 0; i < nNamedColor; i++)
			{
				if (String.Compare(colNamedColor[ColHexa, i].ToString(), Hexa, true) == 0)
				{
					return (string)colNamedColor[ColNamedColor, i];
				}
			}

			return "";
		}

		/// <summary>
		/// Hexa 값에 해당하는 Color.Blue, Color.Red 등의 Color structure를 리턴함.
		/// </summary>
		public static Color GetColorByHexa(string Hexa)
		{
			Hexa = NormalizeHexa(Hexa);
			if (string.IsNullOrEmpty(Hexa))
				throw new Exception(Hexa + "는(은) 없는 색입니다.");

			int R = CMath.Dec(Hexa.Substring(0, 2));
			if ((R < 0) || (R > 255))
				throw new Exception(Hexa + "는(은) 없는 색입니다.");

			int G = CMath.Dec(Hexa.Substring(2, 2));
			if ((G < 0) || (G > 255))
				throw new Exception(Hexa + "는(은) 없는 색입니다.");

			int B = CMath.Dec(Hexa.Substring(4, 2));
			if ((G < 0) || (G > 255))
				throw new Exception(Hexa + "는(은) 없는 색입니다.");

			return Color.FromArgb(R, G, B);
		}

		public static Color GetColorByRgbFunction(string Value)
		{
			Regex r = new Regex(CRegex.Pattern.RgbFunction, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);
			Match m = r.Match(Value);
			if (!m.Success)
				throw new Exception(Value + "는(은) 없는 색입니다.");

			int R = Convert.ToInt32(m.Groups["R"].Value);
			int G = Convert.ToInt32(m.Groups["G"].Value);
			int B = Convert.ToInt32(m.Groups["B"].Value);
			return Color.FromArgb(R, G, B);
		}

		/// <summary>
		/// Hexa 값에 해당하는 VB Color 값을 리턴함.
		/// </summary>
		public static int GetVbColorByHexa(string Hexa)
		{
			Hexa = NormalizeHexa(Hexa);
			if (Hexa == "")
				return 0;

			int R = CMath.Dec(Hexa.Substring(0, 2));
			if ((R < 0) || (R > 255))
				return 0;

			int G = CMath.Dec(Hexa.Substring(2, 2));
			if ((G < 0) || (G > 255))
				return 0;
			
			int B = CMath.Dec(Hexa.Substring(4, 2));
			if ((G < 0) || (G > 255))
				return 0;

			return GetVbRGB(R, G, B);
		}

		/// <summary>
		/// Named Color 값에 해당하는 Hexa 값을 리턴함.
		/// </summary>
		public static string GetHexaByNamedColor(string NamedColor)
		{
			for (int i = 0; i < nNamedColor; i++)
			{
				if (String.Compare(colNamedColor[ColNamedColor, i].ToString(), NamedColor, true) == 0)
				{
					return (string)colNamedColor[ColHexa, i];
				}
			}

			throw new Exception(NamedColor + "는(은) 없는 색입니다.");
		}

		/// <summary>
		/// Named Color 값에 해당하는 Color structure 값을 리턴함.
		/// </summary>
		public static Color GetColorByNamedColor(string NamedColor)
		{
			for (int i = 0; i < nNamedColor; i++)
			{
				if (String.Compare(colNamedColor[ColNamedColor, i].ToString(), NamedColor, true) == 0)
				{
					return (Color)colNamedColor[ColColor, i];
				}
			}

			throw new Exception(NamedColor + "는(은) 없는 색입니다.");
		}

		/// <summary>
		/// Named Color 값에 해당하는 VB Color 값을 리턴함.
		/// </summary>
		public static int GetVbColorByNamedColor(string NamedColor)
		{
			for (int i = 0; i < nNamedColor; i++)
			{
				if (String.Compare(colNamedColor[ColNamedColor, i].ToString(), NamedColor, true) == 0)
				{
					Color c = (Color)colNamedColor[ColColor, i];
					return GetVbRGB(c.R, c.G, c.B);
				}
			}

			throw new Exception(NamedColor + "는(은) 없는 색입니다.");
		}

		/// <summary>
		/// Color structure 값에 해당하는 Named Color 값을 리턴함.
		/// </summary>
		public static string GetNamedColorByColor(Color c)
		{
			for (int i = 0; i < nNamedColor; i++)
			{
				if (((Color)colNamedColor[ColColor, i]) == c)
				{
					return (string)colNamedColor[ColNamedColor, i];
				}
			}

			throw new Exception(c.ToString() + "는(은) 없는 색입니다.");
		}

		/// <summary>
		/// Color structure 값에 해당하는 VB Color 값을 리턴함.
		/// </summary>
		public static int GetVbColorByColor(Color c)
		{
			return GetVbRGB(c.R, c.G, c.B);
		}

		/// <summary>
		/// VB Color 값에 해당하는 Hexa 값을 리턴함.
		/// </summary>
		public static string GetHexaByVbColor(int VbColor)
		{
			int R, G, B;

			R = GetVbRedValue(VbColor);
			G = GetVbGreenValue(VbColor);
			B = GetVbBlueValue(VbColor);
			return GetHexaByRGB(R, G, B);
		}

		/// <summary>
		/// VB Color 값에 해당하는 Named Color 값을 리턴함.
		/// </summary>
		public static string GetNamedColorByVbColor(int VbColor)
		{
			for (int i = 0; i < nNamedColor; i++)
			{
				if (((int)colNamedColor[ColVbColor, i]) == VbColor)
				{
					return (string)colNamedColor[ColNamedColor, i];
				}
			}

			throw new Exception(VbColor.ToString() + "는(은) 없는 색입니다.");
		}

		/// <summary>
		/// VB Color 값에 해당하는 Color 값을 리턴함.
		/// </summary>
		public static Color GetColorByVbColor(int VbColor)
		{
			int r = GetVbRedValue(VbColor);
			int g = GetVbGreenValue(VbColor);
			int b = GetVbBlueValue(VbColor);

			return Color.FromArgb(r, g, b);
		}

		/// <summary>
		/// Hexa 또는 Named Color 값에 해당하는 Color structure 값을 리턴함.
		/// </summary>
		public static Color GetColorByHexaOrNamedColor(string Value)
		{
			Color c = Color.Transparent;
			bool IsErr = false;

			IsErr = false;
			try { c = GetColorByHexa(Value); }
			catch (Exception) { IsErr = true; }
			if (!IsErr)
				return c;

			IsErr = false;
			try { c = GetColorByNamedColor(Value); }
			catch (Exception) { IsErr = true; }
			if (!IsErr)
				return c;

			throw new Exception(Value + "는(은) 없는 색입니다.");
		}

		public static Color GetColorByHexaOrNamedColorOrRgbFunction(string Value)
		{
			Color c = Color.Transparent;
			bool IsErr = false;

			IsErr = false;
			try { c = GetColorByHexa(Value); }
			catch (Exception) { IsErr = true; }
			if (!IsErr)
				return c;

			IsErr = false;
			try { c = GetColorByNamedColor(Value); }
			catch (Exception) { IsErr = true; }
			if (!IsErr)
				return c;

			IsErr = false;
			try { c = GetColorByRgbFunction(Value); }
			catch (Exception) { IsErr = true; }
			if (!IsErr)
				return c;

			throw new Exception(Value + "는(은) 없는 색입니다.");
		}

		/// <summary>
		/// R, G, B 값에 해당하는 Hexa 값을 리턴함.
		/// </summary>
		/// <param name="Red">Red</param>
		/// <param name="Green">Green</param>
		/// <param name="Blue">Blue</param>
		/// <returns>Hexa 값</returns>
		public static string GetHexaByRGB(int Red, int Green, int Blue)
		{
			string r = "", g = "", b = "";

			r = CMath.Hex(Red, false);
			g = CMath.Hex(Green, false);
			b = CMath.Hex(Blue, false);
			return r.PadLeft(2, '0') + g.PadLeft(2, '0') + b.PadLeft(2, '0');
		}
		/// <summary>
		/// Color structure 값에 해당하는 Hexa 값을 리턴함.
		/// </summary>
		/// <param name="c">Color</param>
		public static string GetHexaByColor(Color c)
		{
			return GetHexaByRGB(c.R, c.G, c.B);
		}

		/// <summary>
		/// VB의 RGB 함수와 같은 결과값을 리턴함.
		/// </summary>
		/// <param name="R">Red 값</param>
		/// <param name="G">Green 값</param>
		/// <param name="B">Blue 값</param>
		public static int GetVbRGB(int R, int G, int B)
		{
			return (R) + (G * 256) + (B * (256 * 256));
		}

		/// <summary>
		/// VB에서 쓰이는 Color 중에서 Red 값을 리턴함.
		/// </summary>
		public static int GetVbRedValue(int VbColor)
		{
			return VbColor & 0xFF;
		}
		/// <summary>
		/// VB에서 쓰이는 Color 중에서 Green 값을 리턴함.
		/// </summary>
		public static int GetVbGreenValue(int VbColor)
		{
			return Convert.ToInt32(Math.Floor(Convert.ToDouble(VbColor / 0x100))) & 0xFF;
		}
		/// <summary>
		/// VB에서 쓰이는 VbColor 중에서 Blue 값을 리턴함.
		/// </summary>
		public static int GetVbBlueValue(int VbColor)
		{
			return Convert.ToInt32(Math.Floor(Convert.ToDouble(VbColor / 0x10000))) & 0xFF;
		}

		public static double GetGrayColor(Color c)
		{
			return ((c.R * .30) + (c.G * .59) + (c.B * .11));
		}
		public static double GetColorDifferenceUsingGray(Color c1, Color c2)
		{
			double Gray1 = GetGrayColor(c1);
			double Gray2 = GetGrayColor(c2);

			return (Gray1 - Gray2) * 100.0 / 256.0;
		}

		private static string NormalizeHexa(string Hexa)
		{
			//Get10FromN 함수에서 대문자 A와 소문자 a의 값이 서로 다르게 취급됨.
			Hexa = Hexa.ToUpper();
			if (string.IsNullOrEmpty(Hexa))
				return "";

			if (Hexa.Substring(0, 1) == "#")
				Hexa = Hexa.Substring(1);

			if (Hexa.Length != 6)
				return "";

			return Hexa;
		}

		
	}
}
