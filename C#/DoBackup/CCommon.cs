using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DoctorGu;
using System.Reflection;
using System.IO;

namespace DoBackup
{
	public class CCommon
	{
		public struct Const
		{
			public static string ConfigXmlFullPath
			{
				//get { return Path.Combine(CInfo.GetAppFolder(Assembly.GetExecutingAssembly()), "Config.xml"); }
				get { return Path.Combine(CAssembly.GetFolder(), "Config.xml"); }
			}
		}
	}
}

