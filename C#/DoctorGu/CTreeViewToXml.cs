using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace DoctorGu
{
	public class CTreeViewToXml
	{
		private Encoding mEncoding;
		private string mDocumentElementName;
		private string mElementName;
		private string mAttrNameForText;
		private string mAttrNameForTag;

		public CTreeViewToXml(Encoding Encoding, string DocumentElementName, string ElementName,
			string AttrNameForText, string AttrNameForTag)
		{
			this.mEncoding = Encoding;
			this.mDocumentElementName = DocumentElementName;
			this.mElementName = ElementName;
			this.mAttrNameForText = AttrNameForText;
			this.mAttrNameForTag = AttrNameForTag;
		}

		public string ToXml(TreeNode Parent)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				using (XmlTextWriter xw = new XmlTextWriter(ms, this.mEncoding))
				{
					xw.Formatting = Formatting.Indented;
					xw.IndentChar = '\t';

					xw.WriteStartDocument();
					xw.WriteStartElement(mDocumentElementName);

					WriteAttr(xw, Parent);

					ToXml2(Parent, xw);

					xw.WriteEndElement();
					xw.WriteEndDocument();

					xw.Flush();

					ms.Position = 0;
					StreamReader sr = new StreamReader(ms, this.mEncoding);
					string Xml = sr.ReadToEnd();
					return Xml;
				}
			}
		}
		private void ToXml2(TreeNode Parent, XmlWriter xw)
		{
			foreach (TreeNode Child in Parent.Nodes)
			{
				xw.WriteStartElement(mElementName);

				WriteAttr(xw, Child);

				if (Child.Nodes.Count > 0)
				{
					ToXml2(Child, xw);
				}

				xw.WriteEndElement();
			}
		}
		private void WriteAttr(XmlWriter xw, TreeNode Node)
		{
			if (!string.IsNullOrEmpty(this.mAttrNameForText))
			{
				xw.WriteStartAttribute(this.mAttrNameForText);
				xw.WriteValue(Node.Text);
			}

			if (!string.IsNullOrEmpty(this.mAttrNameForTag))
			{
				xw.WriteStartAttribute(this.mAttrNameForTag);
				xw.WriteValue(Node.Tag);
			}
		}

		public void ToTreeView(TreeView tvw, string XmlFullPath)
		{
			XmlDocument XDoc = new XmlDocument();
			XDoc.Load(XmlFullPath);

			XmlElement Elem = XDoc.DocumentElement;

			TreeNode tnParent = CreateTreeNodeChild(tvw, null, Elem);
			ToTreeView(tnParent, Elem);
		}
		private void ToTreeView(TreeNode tnParent, XmlElement xeParent)
		{
			for (int i = 0, i2 = xeParent.ChildNodes.Count; i < i2; i++)
			{
				if (xeParent.ChildNodes[i].NodeType != XmlNodeType.Element)
					continue;

				XmlElement xeChildCur = (XmlElement)xeParent.ChildNodes[i];


				TreeNode tnChildCur = CreateTreeNodeChild(null, tnParent, xeChildCur);

				ToTreeView(tnChildCur, xeChildCur);
			}
		}
		private TreeNode CreateTreeNodeChild(TreeView tvw, TreeNode tnParent, XmlElement xeChild)
		{
			TreeNode tnNew = new TreeNode();
			if (tvw != null)
			{
				tnNew = tvw.Nodes.Add("");
			}
			else
			{
				tnNew = tnParent.Nodes.Add("");
			}

			if (!string.IsNullOrEmpty(this.mAttrNameForText))
			{
				tnNew.Text = xeChild.GetAttribute(this.mAttrNameForText);
			}

			if (!string.IsNullOrEmpty(this.mAttrNameForTag))
			{
				tnNew.Tag = xeChild.GetAttribute(this.mAttrNameForTag);
			}

			return tnNew;
		}
	}
}
