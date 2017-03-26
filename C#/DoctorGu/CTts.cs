using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Synthesis;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;

namespace DoctorGu
{
	public class CTtsVariable
	{
		public string Man;
		public string Woman;
		public string Break;
		public string Ignore;
	}

	public class CTts
	{
		private string _VoiceNameMale;
		private string _VoiceNameFemale;
		private string _VoiceNameDefault;

		public CTts(string VoiceNameMale, string VoiceNameFemale, string VoiceNameDefault)
		{
			_VoiceNameMale = VoiceNameMale;
			_VoiceNameFemale = VoiceNameFemale;
			_VoiceNameDefault = VoiceNameDefault;
		}

		public string ConvertToSsml(string TextToSpeak)
		{
			//숫자 읽는 방식은 say-as로 변경 가능함. (telephone으로 설정해도 전화번호 형식이 아니면 안됨)
			//<say-as interpret-as="telephone">1-800-282-0114</say-as>
			//http://msdn.microsoft.com/en-us/library/dd450828(v=office.13).aspx

			//속도는 <prosody rate="+100%">two time fast.</prosody>

			//원래는 xmlns="http://www.w3.org/2001/10/synthesis" 이나 xmlns 속성이 자동으로 생겨 :w를 추가함.
			string XmlTemplate =
@"<?xml version=""1.0""?>
<speak version=""1.0""
	xmlns:w=""http://www.w3.org/2001/10/synthesis""
	xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
	xsi:schemaLocation=""http://www.w3.org/2001/10/synthesis
	http://www.w3.org/TR/speech-synthesis/synthesis.xsd""
	xml:lang=""en-US"">
</speak>";
			XmlDocument XDoc = new XmlDocument();
			XDoc.LoadXml(XmlTemplate);
			XmlElement Doc = XDoc.DocumentElement;


			//string Pattern = "(^" + VarMan + ")|(^" + VarWoman + ")|(^" + VarBreak + "(?<Seconds>\\d+[\\.\\d]*))";
			string Pattern = string.Format("(?<Man>{0})|(?<Woman>{1})|(?<Break>{2})|(?<Ignore>{3})", _Variable.Man, _Variable.Woman, _Variable.Break, _Variable.Ignore);
			Regex r = new Regex(Pattern, CRegex.Options.Compiled_Multiline_IgnoreCase_IgnorePatternWhitespace);

			List<XmlAttribute> aAttr = new List<XmlAttribute>();
			List<XmlElement> aElemBreak = new List<XmlElement>();

			XmlElement VoiceCur = XDoc.CreateElement("voice");
			foreach (CMatchInfo mi in CRegex.GetMatchResult(r, TextToSpeak))
			{
				Match m = mi.Match;

				string Value = CFindRep.TrimWhiteSpace(mi.ValueBeforeMatch);
				if (!string.IsNullOrEmpty(Value))
				{
					//int PosColon = Value.IndexOf(":");
					//if (PosColon != -1)
					//{
					//    CParagraph p = new CParagraph();
					//    List<string> aWord = p.GetWords(Value.Substring(0, PosColon + 1), false);
					//    throw new Exception(aWord[aWord.Count - 1] + ":은 잘못된 변수명입니다. ':' 문자열을 지우거나 허용된 변수명을 입력하세요.");
					//}

					AddElementVoice(XDoc, Value, VoiceCur);
					VoiceCur = XDoc.CreateElement("voice");
				}

				if (m == null)
					break;

				if (!string.IsNullOrEmpty(m.Groups["Man"].Value))
				{
					foreach (XmlAttribute AttrCur in GetAttrNameGender(XDoc, false))
					{
						VoiceCur.Attributes.Append(AttrCur);
					}
				}
				else if (!string.IsNullOrEmpty(m.Groups["Woman"].Value))
				{
					foreach (XmlAttribute AttrCur in GetAttrNameGender(XDoc, true))
					{
						VoiceCur.Attributes.Append(AttrCur);
					}
				}
				else if (!string.IsNullOrEmpty(m.Groups["Break"].Value))
				{
					VoiceCur.AppendChild(GetElementBreak(XDoc, m.Groups["Seconds"].Value));
				}
				else if (!string.IsNullOrEmpty(m.Groups["Ignore"].Value))
				{
					//
				}
			}

			string Indented = CXml.GetIndentedContent(XDoc);
			return Indented;
			//return XDoc.OuterXml;
		}

		private void AddElementVoice(XmlDocument XDoc, string Voice, XmlElement Elem)
		{
			if (Elem.Attributes.Count == 0)
			{
				foreach (XmlAttribute AttrCur in GetAttrNameGenderDefault(XDoc))
				{
					Elem.Attributes.Append(AttrCur);
				}
			}

			Elem.AppendChild(XDoc.CreateTextNode(Voice));

			XDoc.DocumentElement.AppendChild(Elem);
		}
		private XmlElement GetElementBreak(XmlDocument XDoc, string Seconds)
		{
			double dSeconds = CFindRep.IfNotNumberThen0Double(Seconds);

			XmlElement el = XDoc.CreateElement("break");

			if (!string.IsNullOrEmpty(Seconds))
			{
				el.Attributes.Append(GetAttrTime(XDoc, dSeconds));
			}

			return el;
		}

		private List<XmlAttribute> GetAttrNameGenderDefault(XmlDocument XDoc)
		{
			List<XmlAttribute> aAttr = new List<XmlAttribute>();

			XmlAttribute AttrGender = XDoc.CreateAttribute("gender");
			XmlAttribute AttrName = XDoc.CreateAttribute("name");

			AttrGender.Value = "neutral";
			AttrName.Value = _VoiceNameDefault;

			aAttr.Add(AttrGender);
			aAttr.Add(AttrName);


			return aAttr;
		}
		private List<XmlAttribute> GetAttrNameGender(XmlDocument XDoc, bool IsFemale)
		{
			List<XmlAttribute> aAttr = new List<XmlAttribute>();

			XmlAttribute AttrGender = XDoc.CreateAttribute("gender");
			XmlAttribute AttrName = XDoc.CreateAttribute("name");
			if (IsFemale)
			{
				AttrGender.Value = "female";
				AttrName.Value = _VoiceNameFemale;
			}
			else
			{
				AttrGender.Value = "male";
				AttrName.Value = _VoiceNameMale;
			}

			aAttr.Add(AttrGender);
			aAttr.Add(AttrName);

			return aAttr;
		}
		private XmlAttribute GetAttrTime(XmlDocument XDoc, double Seconds)
		{
			XmlAttribute Attr = XDoc.CreateAttribute("time");
			Attr.Value = string.Concat((Seconds * 1000), "ms");

			return Attr;
		}


		public void SaveAsMp3(string Ssml, bool UseThread, string FullPathMp3, string LameFullPath, WaveToMp3Preset Preset)
		{
			string FullPathWave = CPath.GetNumberedFullPath(Path.Combine(Path.GetDirectoryName(FullPathMp3), Path.GetFileNameWithoutExtension(FullPathMp3) + ".wav"));

			if (UseThread)
			{
				Thread t = new Thread(() =>
				{
					using (SpeechSynthesizer ss = new SpeechSynthesizer())
					{
						ss.SetOutputToWaveFile(FullPathWave);
						ss.SpeakSsml(Ssml);
					}
				});
				t.Start();
				t.Join();
			}
			else
			{
				using (SpeechSynthesizer ss = new SpeechSynthesizer())
				{
					ss.SetOutputToWaveFile(FullPathWave);
					ss.SpeakSsml(Ssml);
				}
			}

			CAudio.ConvertWaveToMp3(FullPathWave, FullPathMp3, LameFullPath, Preset);
			File.Delete(FullPathWave);
		}
		public MemoryStream GetMemoryStream(string Ssml, bool UseThread)
		{
			//Response.OutputStream에 바로 쓰면 다음 에러 나서 주석.
			//Specified method is not supported.
			//using (SpeechSynthesizer ss = new SpeechSynthesizer())
			//{
			//    ss.SetOutputToWaveStream(Strm);
			//    ss.SpeakSsml(Ssml);
			//}

			MemoryStream ms = new MemoryStream();

			//ASP.Net에서 Asyn 방식 안쓰고 호출하면 다음 에러 발생하므로 Thread 사용.
			//이 컨텍스트에서는 비동기 작업을 수행할 수 없습니다. 비동기 작업을 시작하는 페이지에는 Async 특성이 true로 설정되어 있어야 하며 PreRenderComplete 이벤트가 발생하기 전에만 페이지에서 비동기 작업을 시작할 수 있습니다.
			if (UseThread)
			{
				Thread t = new Thread(() =>
				{
					using (SpeechSynthesizer ss = new SpeechSynthesizer())
					{
						ss.SetOutputToWaveStream(ms);
						ss.SpeakSsml(Ssml);
					}
				});
				t.Start();
				t.Join();
			}
			else
			{
				using (SpeechSynthesizer ss = new SpeechSynthesizer())
				{
					ss.SetOutputToWaveStream(ms);
					ss.SpeakSsml(Ssml);
				}
			}

			ms.Position = 0;
			return ms;
		}

		public void Speak(string Ssml)
		{
			using (SpeechSynthesizer ss = new SpeechSynthesizer())
			{
				ss.SpeakSsml(Ssml);
			}
		}


		private CTtsVariable _Variable = new CTtsVariable() { Man = "(^Man:)|(^M:)", Woman = "(^Woman:)|(^W:)", Break = "^Break:(?<Seconds>\\s?\\d+[\\.\\d]*)", Ignore = "_+" };
		public CTtsVariable Variable
		{
			get { return _Variable; }
			set { _Variable = value; }
		}
	}
}
