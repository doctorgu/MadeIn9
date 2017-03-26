using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace DoctorGu
{
	/// <summary>
	/// Build Action이 Embedded Resource인 내용 읽기
	/// </summary>
	public class CResource
	{
		/// <param name="Name">MyNamespace.MyFolder.MyImage.gif 형식</param>
		public static Stream GetStream(Assembly ExecutingAssembly, string Name)
		{
			return ExecutingAssembly.GetManifestResourceStream(Name);
		}

		/// <param name="Name">MyNamespace.MyFolder.MyText.txt 형식</param>
		public static string GetText(Assembly ExecutingAssembly, string Name)
		{
			Stream Strm = GetStream(ExecutingAssembly, Name);
			StreamReader sr = new StreamReader(Strm);
			return sr.ReadToEnd();
		}
	}
}
