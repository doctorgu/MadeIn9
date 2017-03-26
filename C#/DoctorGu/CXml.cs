using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Specialized;

namespace DoctorGu
{
	public class CXml
	{
		public struct Const
		{
			public const string DefaultPrefix = "def_";
		}

		public static string ReplaceNotAllowedForAttributeName(string AttrName, char ReplaceChar)
		{
			string AttrNameNew = "";
			for (int i = 0, i2 = AttrName.Length; i < i2; i++)
			{
				char c = AttrName[i];
				switch (c)
				{
					case '\\':
					case '/':
					case ':':
					case '*':
					case '?':
					case '"':
					case '<':
					case '>':
					case '|':
					case '=':
					case '&':
						AttrNameNew += ReplaceChar;
						break;
					default:
						AttrNameNew += c;
						break;
				}
			}

			return AttrNameNew;
		}
		public static string ReplaceNotAllowedForAttributeValue(string Value)
		{
			return ReplaceNotAllowedForValue(Value)
				.Replace("\"", "&quot;");
		}
		public static string ReplaceNotAllowedForInnerText(string Value)
		{
			return ReplaceNotAllowedForValue(Value);
		}
		private static string ReplaceNotAllowedForValue(string Value)
		{
			return Value
				.Replace("&", "&amp;")
				.Replace("<", "&lt;")
				.Replace(">", "&gt;");
		}

		/// <summary>
		/// <![CDATA[&, <, >]]> 등은 Xml 기능을 이용해 값을 설정하면 자동 변환되지만
		/// Surrogate 문자열의 경우는 자동 변환되지 않고
		/// 1. SetAttribute를 쓰면 설정은 되나 읽을 때 에러,
		/// 2. WriteAttributeString을 쓰면 설정할 때 에러남.
		/// </summary>
		public static string ReplaceNotAllowedForXml(string Value, string ValueToReplace)
		{
			//먼저 Surrogate pair를 먼저 변환, pair가 아닌 분리된 각각을 변환.
			Value = Regex.Replace(Value, CRegex.Pattern.SurrogateHighAndLow, ValueToReplace);
			Value = Regex.Replace(Value, CRegex.Pattern.SurrogateHighOrLow, ValueToReplace);
			return Value;
		}

		public static XmlNode GetParentHasNodeName(XmlNode nod, string Name)
		{
			XmlNode nodParent = nod.ParentNode;
			while (nodParent != null)
			{
				if (nodParent.Name == Name)
					return nodParent;

				nodParent = nodParent.ParentNode;
			}

			return null;
		}

		public static void CopyAttribute(XmlDocument XDoc, XmlNode nodSrc, XmlNode nodDest)
		{
			foreach (XmlAttribute AttrSrc in nodSrc.Attributes)
			{
				if (nodDest.Attributes[AttrSrc.Name] != null)
				{
					nodDest.Attributes[AttrSrc.Name].Value = AttrSrc.Value;
				}
				else
				{
					nodDest.Attributes.Append(XDoc.CreateAttribute(AttrSrc.Name)).Value = AttrSrc.Value;
				}
			}
		}

		/// <summary>
		/// <![CDATA[
		/// <TEXT CharShape="0"><BOOKMARK Name="참조" /><CHAR>특</CHAR><CHAR>히</CHAR></TEXT>
		/// -> <TEXT CharShape="0"><BOOKMARK Name="참조" /><CHAR>특히</CHAR></TEXT>
		/// ]]>
		/// </summary>
		public static void MergeChildNodes(XmlDocument XDoc, XmlNode nodText, string TagNameToMerge)
		{
			for (int i = 0; i < nodText.ChildNodes.Count - 1; i++)
			{
				XmlNode nodChild = nodText.ChildNodes[i];
				XmlNode nodChildNext = nodText.ChildNodes[i + 1];

				if ((nodChild.Name == TagNameToMerge) && (nodChildNext.Name == TagNameToMerge))
				{
					nodChild.InnerText += nodChildNext.InnerText;
					nodText.RemoveChild(nodChildNext);
					i--;
				}
			}
		}

		/// <summary>
		/// PreserveWhitespace = true이면 NextSibling이 Whitespace를 리턴할 수 있으므로 다음 Element만을 리턴하게 함.
		/// </summary>
		/// <param name="nodCurrent"></param>
		/// <returns></returns>
		public static XmlNode GetNextElementNode(XmlNode nodCurrent)
		{
			XmlNode nodNext = null;
			for (nodNext = nodCurrent.NextSibling; nodNext != null; nodNext = nodNext.NextSibling)
			{
				if (nodNext.NodeType == XmlNodeType.Element)
					return nodNext;
			}

			return null;
		}

		public static XmlNode SetAttribute(XmlDocument XDoc, XmlNode nod, string Name, string Value)
		{
			if (nod.Attributes[Name] != null)
			{
				nod.Attributes[Name].Value = Value;
			}
			else
			{
				XmlAttribute Attr = XDoc.CreateAttribute(Name);
				Attr.Value = Value;
				nod.Attributes.Append(Attr);
			}

			return nod;
		}

		public static string GetAttributeString(XmlAttribute Attr)
		{
			if (Attr != null)
				return Attr.Value;
			else
				return "";
		}
		public static string GetAttributeString(XmlNode Node, string Name)
		{
			return GetAttributeString(Node.Attributes[Name]);
		}

		public static int GetAttributeInt32(XmlAttribute Attr)
		{
			if (Attr != null)
				return CFindRep.IfNotNumberThen0(Attr.Value);
			else
				return 0;
		}
		public static int GetAttributeInt32(XmlNode Node, string Name)
		{
			return GetAttributeInt32(Node.Attributes[Name]);
		}

		public static double GetAttributeDouble(XmlAttribute Attr)
		{
			if (Attr != null)
				return CFindRep.IfNotNumberThen0Double(Attr.Value);
			else
				return 0;
		}
		public static double GetAttributeDouble(XmlNode Node, string Name)
		{
			return GetAttributeDouble(Node.Attributes[Name]);
		}

		public static Decimal GetAttributeDecimal(XmlAttribute Attr)
		{
			if (Attr != null)
				return CFindRep.IfNotNumberThen0Decimal(Attr.Value);
			else
				return 0;
		}
		public static Decimal GetAttributeDecimal(XmlNode Node, string Name)
		{
			return GetAttributeDecimal(Node.Attributes[Name]);
		}

		public static bool GetAttributeBoolean(XmlAttribute Attr)
		{
			if (Attr != null)
				return CLang.In(Attr.Value, "-1", "1", "true", "True");
			else
				return false;
		}
		public static bool GetAttributeBoolean(XmlNode Node, string Name)
		{
			return GetAttributeBoolean(Node.Attributes[Name]);
		}

		/// <summary>
		/// Attribute의 Name이 대문자이든 소문자이든 상관없이 찾을 수 있게 함.
		/// </summary>
		public static XmlAttribute GetAttributeIgnoreCase(XmlNode Node, string Name)
		{
			XmlAttribute Attr = Node.Attributes.Cast<XmlAttribute>().Where(a => string.Compare(a.Name, Name, true) == 0).SingleOrDefault();
			return Attr;
		}

		public static int GetNodeLevel(XmlNode CurNode)
		{
			if (CurNode == null) return -1;

			XmlNode ParentNode = CurNode;
			int i = -1;
			while ((ParentNode != null) && (ParentNode.NodeType != XmlNodeType.Document))
			{
				i++;
				ParentNode = ParentNode.ParentNode;
			}

			return i;
		}

		public static XmlAttribute FirstAttributeFromCurrentToChild(XmlNode Node, string AttrName)
		{
			if (Node.Attributes[AttrName] != null)
				return Node.Attributes[AttrName];

			foreach (XmlNode nodChild in Node.ChildNodes)
			{
				return FirstAttributeFromCurrentToChild(nodChild, AttrName);
			}

			return null;
		}

		public static XmlDocument CreateUtf8XmlDocument()
		{
			return CreateUtf8XmlDocument("");
		}
		public static XmlDocument CreateUtf8XmlDocument(string DocumentElementName)
		{
			XmlDocument XDoc = new XmlDocument();
			XmlDeclaration XmlDeclare = XDoc.CreateXmlDeclaration("1.0", "utf-8", null);
			XDoc.InsertBefore(XmlDeclare, XDoc.DocumentElement);
			
			if (!string.IsNullOrEmpty(DocumentElementName))
				XDoc.AppendChild(XDoc.CreateElement(DocumentElementName));
			
			return XDoc;
		}

		//http://www.knowdotnet.com/articles/indentxml.html
		public static string GetIndentedContent(XmlDocument XDoc)
		{
			string outXml = string.Empty;
			MemoryStream ms = new MemoryStream();
			XmlTextWriter xtw = new XmlTextWriter(ms, Encoding.Unicode);

			xtw.Formatting = Formatting.Indented;
			XDoc.WriteContentTo(xtw);

			xtw.Flush();
			ms.Seek(0, SeekOrigin.Begin);

			StreamReader sr = new StreamReader(ms);
			return sr.ReadToEnd();
		}

		public static XmlNamespaceManager AddNamespace(XmlDocument XDoc)
		{
			XmlNamespaceManager ns = new XmlNamespaceManager(XDoc.NameTable);
			
/*
<?xml version="1.0" encoding="UTF-8" standalone="no"?>
<ERwin xmlns="http://www.ca.com/erwin" xmlns:UDP="http://www.ca.com/erwin/metadata" xmlns:EMX="http://www.ca.com/erwin/data" xmlns:EM2="http://www.ca.com/erwin/EM2data" FileVersion="7.1.1075" Format="ERwin"><EMX:Model id="{5A326242-5CEE-47CE-998C-0C6899C9ED3D}+00000000" name="Model_2" xmlns="http://www.ca.com/erwin/data"><ModelEnvProps><Locator>erwin://C:\Users\Administrator\Desktop\kcci.xml</Locator><Disposition>Read Only = Yes;</Disposition><Persistence_Unit_Id>{4C75CD37-8871-49B0-B825-9582842A1FB7}+00000000</Persistence_Unit_Id><Model_Type>3</Model_Type><Target_Server>1075859016</Target_Server><Target_Server_Version>8</Target_Server_Version><Target_Server_Minor_Version>0</Target_Server_Minor_Version><Active_Model>-1</Active_Model><Hidden_Model>0</Hidden_Model><Storage_Format>4012</Storage_Format></ModelEnvProps></EMX:Model></ERwin>
*/
			foreach (XmlAttribute Attr in XDoc.DocumentElement.Attributes)
			{
				if (Attr.Prefix == "xmlns")
					ns.AddNamespace(Attr.LocalName, Attr.Value);
				else if (Attr.Name == "xmlns")
					ns.AddNamespace(CXml.Const.DefaultPrefix, Attr.Value);
			}
			
			return ns;
		}

		public static string ReplaceXPathWithPrefix(string XPath, string Prefix, XmlNamespaceManager nsm)
		{
			if (!nsm.HasNamespace(Prefix))
				return XPath;


			XPath = Regex.Replace(XPath, "^(?!::)([^/:]+)(?=/)", Prefix + ":$1");                             // beginning
			XPath = Regex.Replace(XPath, "/([^/:\\*\\(]+)(?=[/\\[])", "/" + Prefix + ":$1");                  // segment
			XPath = Regex.Replace(XPath, "::([A-Za-z][^/:*]*)(?=/)", "::" + Prefix + ":$1");                  // axis specifier
			XPath = Regex.Replace(XPath, "\\[([A-Za-z][^/:*\\(]*)(?=[\\[\\]])", "[" + Prefix + ":$1");        // within predicate
			XPath = Regex.Replace(XPath, "/([A-Za-z][^/:\\*\\(]*)(?!<::)$", "/" + Prefix + ":$1");               // end
			XPath = Regex.Replace(XPath, "^([A-Za-z][^/:]*)$", Prefix + ":$1");                               // edge case
			XPath = Regex.Replace(XPath, "([A-Za-z][-A-Za-z]+)\\(([^/:\\.,\\(\\)]+)(?=[,\\)])", "$1(" + Prefix + ":$2"); // xpath functions

			return XPath;
		}
		public static string ReplaceXPathWithPrefix(string XPath, XmlNamespaceManager nsm)
		{
			string Prefix = CXml.Const.DefaultPrefix;
			return ReplaceXPathWithPrefix(XPath, Prefix, nsm);
		}

		public static string CreateElementWithAttribute(string ElementName, NameValueCollection nvAttr)
		{
			string s = "";
			s += "<" + ElementName;

			foreach (string Name in nvAttr)
			{
				s += " " + Name + " = \"" + CXml.ReplaceNotAllowedForAttributeValue(nvAttr[Name]) + "\"";
			}

			s += " />";

			return s;
		}

		public static string GetAttributeValue(string ElementXml, string Name)
		{
			XmlDocument XDoc = new XmlDocument();
			XDoc.LoadXml(ElementXml);

			if (XDoc.DocumentElement.Attributes[Name] == null)
				return "";
			else
				return XDoc.DocumentElement.Attributes[Name].Value;
		}

		public static float GetAttributeFloat(XmlAttribute Attr)
		{
			if (Attr != null)
				return CFindRep.IfNotNumberThen0Float(Attr.Value);
			else
				return 0;
		}
		public static float GetAttributeFloat(XmlNode Node, string Name)
		{
			return GetAttributeFloat(Node.Attributes[Name]);
		}

		public static string[] SplitXml(string Xml)
		{
			string[] aXml = Xml.Split(new string[] { "/>" }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < aXml.Length; i++)
			{
				aXml[i] += "/>";
			}

			return aXml;
		}
	}
}
