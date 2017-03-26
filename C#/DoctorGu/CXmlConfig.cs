using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Collections.Specialized;

namespace DoctorGu
{
	/// <summary>
	/// XML 파일을 이용해 설정 사항을 저장하고 가져오기 위한 기능 구현.
	/// 단 하나의 노드에 여러 개의 Attribute의 Name와 Value로 정보가 저장됨.
	/// </summary>
	/// <example>
	/// 다음은 현재 프로그램의 경로에 config.xml 파일에 설정사항을 저장하는 예입니다.
	/// 첫번째는 Name 값이 없으므로 기본값인 "임꺽정"을 출력하지만 두번째는 Name 값이 저장되었으므로
	/// 저장된 "홍길동"을 출력합니다.
	/// <code>
	/// CXmlConfig xc = new CXmlConfig(Path.GetDirectoryName(Application.ExecutablePath) + "\\config.xml");
	/// 
	/// Console.WriteLine(xc.GetSetting("Name", "임꺽정")); // "임꺽정"
	/// xc.SaveSetting("Name", "홍길동");
	/// Console.WriteLine(xc.GetSetting("Name", "임꺽정")); // "홍길동"
	/// </code>
	/// </example>
	public class CXmlConfig
	{
		private string _XmlFullPath = "";
		private XmlDocument _XDocForReadOnly = null;

		/// <summary>
		/// </summary>
		/// <param name="XmlFullPath">설정 사항을 저장할 XML 파일의 전체 경로</param>
		public CXmlConfig(string XmlFullPath)
		{
			_XmlFullPath = XmlFullPath;
		}
		public CXmlConfig(XmlDocument XDocForReadOnly)
		{
			_XDocForReadOnly = XDocForReadOnly;
		}

		/// <summary>
		/// 설정사항을 저장함.
		/// </summary>
		/// <param name="Name">유일한 키</param>
		/// <param name="Value">키에 연결된 값</param>
		public void SaveSetting(string Name, string Value)
		{
			XmlDocument XDoc = GetDocument();
			XmlElement Doc = XDoc.DocumentElement;

			Doc.SetAttribute(Name, Value);

			SaveToFile(XDoc);
		}
		public void SaveSetting(string Name, object Value)
		{
			SaveSetting(Name, Value.ToString());
		}
		public void SaveSetting(NameValueCollection nvNameValue)
		{
			XmlDocument XDoc = GetDocument();
			XmlElement Doc = XDoc.DocumentElement;

			foreach (string Key in nvNameValue.AllKeys)
			{
				Doc.SetAttribute(Key, nvNameValue[Key]);
			}

			SaveToFile(XDoc);
		}
		private void SaveToFile(XmlDocument XDoc)
		{
			if (string.IsNullOrEmpty(_XmlFullPath))
				throw new Exception(string.Format("_XmlFullPath:{0} is null or empty.", _XmlFullPath));

			XDoc.Save(_XmlFullPath);
		}

		/// <summary>
		/// 설정사항을 가져옴.
		/// </summary>
		/// <param name="Name">유일한 키</param>
		/// <param name="DefaultValue"><paramref name="Name"/>에 해당하는 값이 없을 때 대체될 기본값</param>
		/// <returns><paramref name="Name"/>에 해당하는 값</returns>
		public string GetSetting(string Name, string DefaultValue)
		{
			XmlDocument XDoc = GetDocument();
			XmlElement Doc = XDoc.DocumentElement;

			string Value = Doc.GetAttribute(Name);
			if (string.IsNullOrEmpty(Value))
			{
				Value = DefaultValue;
			}

			return Value;
		}
		public string GetSetting(string Name, object DefaultValue)
		{
			return GetSetting(Name, DefaultValue.ToString());
		}
		public string GetSetting(string Name)
		{
			return GetSetting(Name, null);
		}

		private XmlDocument GetDocument()
		{
			if (_XDocForReadOnly == null)
			{
				XmlDocument XDoc = new XmlDocument();
				try
				{
					XDoc.Load(_XmlFullPath);
				}
				catch (Exception)
				{
					if (File.Exists(_XmlFullPath))
					{
						File.Delete(_XmlFullPath);
					}

					XDoc = CXml.CreateUtf8XmlDocument("config");
				}

				return XDoc;
			}
			else
			{
				return _XDocForReadOnly;
			}
		}

		/// <summary>
		/// 설정사항의 저장에 쓰이는 XML 파일의 전체 경로를 리턴함.
		/// </summary>
		public string XmlFullPath
		{
			get { return _XmlFullPath; }
		}
	}
}
