using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Drawing;

namespace DoctorGu
{
	public class CAssembly
	{
		public static string GetFolder(Assembly Assem)
		{
			return Path.GetDirectoryName(Assem.Location);
		}
		public static string GetFolder()
		{
			return GetFolder(GetEntryOrExecuting());
		}

		public static string GetFileName(Assembly Assem)
		{
			return Assem.ManifestModule.Name;
		}
		public static string GetFileName()
		{
			return GetFileName(GetEntryOrExecuting());
		}

		public static string GetFileNameWithoutExtension(Assembly Assem)
		{
			return Path.GetFileNameWithoutExtension(Assem.ManifestModule.Name);
		}
		public static string GetFileNameWithoutExtension()
		{
			return GetFileNameWithoutExtension(Assembly.GetEntryAssembly());
		}

		public static string GetCopyright(Assembly Assem)
		{
			return ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(Assem, typeof(AssemblyCopyrightAttribute), false)).Copyright;
		}
		public static string GetCopyright()
		{
			return GetCopyright(GetEntryOrExecuting());
		}

		public static Assembly GetEntryOrExecuting()
		{
			Assembly Assem = Assembly.GetEntryAssembly();
			if (Assem == null)
				Assem = Assembly.GetExecutingAssembly();

			return Assem;
		}

		public static Icon GetAssociatedIcon(Assembly Assem)
		{
			Icon ico = null;

			try
			{
				ico = Icon.ExtractAssociatedIcon(Assem.Location);
			}
			catch (Exception) { }

			return ico;
		}
		public static Icon GetAssociatedIcon()
		{
			return GetAssociatedIcon(CAssembly.GetEntryOrExecuting());
		}
	}
}
