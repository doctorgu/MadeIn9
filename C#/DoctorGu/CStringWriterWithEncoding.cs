using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DoctorGu
{
	/// <summary>
	/// <![CDATA[
	/// StringBuilder sb = new StringBuilder();
	/// using (CStringWriterWithEncoding sw = new CStringWriterWithEncoding(sb, Encoding.UTF8))
	/// {
	/// 	using (XmlTextWriter xw = new XmlTextWriter(sw))
	/// 	{
	/// 		WriteXmlErrMsg(xw, IsSelect, ErrMsg);
	/// 		Xml = sb.ToString();
	/// 	}
	/// }
	/// ]]>
	/// </summary>
	public class CStringWriterWithEncoding : StringWriter
	{
		private Encoding _encoding;

		public CStringWriterWithEncoding()
			: base() { }

		public CStringWriterWithEncoding(IFormatProvider formatProvider)
			: base(formatProvider) { }

		public CStringWriterWithEncoding(StringBuilder sb)
			: base(sb) { }

		public CStringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider)
			: base(sb, formatProvider) { }

		public CStringWriterWithEncoding(Encoding encoding)
			: base()
		{
			_encoding = encoding;
		}

		public CStringWriterWithEncoding(IFormatProvider formatProvider, Encoding encoding)
			: base(formatProvider)
		{
			_encoding = encoding;
		}

		public CStringWriterWithEncoding(StringBuilder sb, Encoding encoding)
			: base(sb)
		{
			_encoding = encoding;
		}

		public CStringWriterWithEncoding(StringBuilder sb, IFormatProvider formatProvider, Encoding encoding)
			: base(sb, formatProvider)
		{
			_encoding = encoding;
		}

		public override Encoding Encoding
		{
			get
			{
				return (null == _encoding) ? base.Encoding : _encoding;
			}
		}
	}
}
