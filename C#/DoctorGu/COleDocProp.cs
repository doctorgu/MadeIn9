using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.IO;

namespace DoctorGu
{
#if !DotNet35
	public class COleDocProp
	{
		public static bool SetCustomProperty(string DocFullPath, string PropName, string PropValue)
		{
			dynamic DsoProp = Open(DocFullPath, false);
			if (DsoProp == null)
				return false;

			string PropValueOri = "";
			bool IsPropExists = false;
			try
			{
				PropValueOri = DsoProp.CustomProperties(PropName).Value;
				IsPropExists = true;
			}
			catch (Exception) { }

			if (IsPropExists)
			{
				if (PropValueOri != PropValue)
				{
					DsoProp.CustomProperties(PropName).Value = PropValue;
					DsoProp.Close(true);
				}
			}
			else
			{
				DsoProp.CustomProperties.Add(PropName, PropValue);
			}

			if (DsoProp.IsDirty)
				DsoProp.Close(true);
			else
				DsoProp.Close(false);


			return true;
		}

		public static bool RemoveCustomProperty(string DocFullPath, string PropName)
		{
			dynamic DsoProp = Open(DocFullPath, false);
			if (DsoProp == null)
				return false;

			string PropValueOri = "";
			bool IsPropExists = false;
			try
			{
				PropValueOri = DsoProp.CustomProperties(PropName).Value;
				IsPropExists = true;
			}
			catch (Exception) { }

			if (IsPropExists)
			{
				DsoProp.CustomProperties(PropName).Remove();
			}

			if (DsoProp.IsDirty)
				DsoProp.Close(true);
			else
				DsoProp.Close(false);


			return true;
		}

		public static string GetCustomProperty(string DocFullPath, string PropName, string DefaultValue)
		{
			dynamic DsoProp = Open(DocFullPath, true);
			if (DsoProp == null)
				return DefaultValue;


			string Value = "";
			bool IsPropExists = false;

			try
			{
				Value = DsoProp.CustomProperties(PropName).Value;
				DsoProp.Close(false);

				IsPropExists = true;
			}
			catch (Exception)
			{
				DsoProp.Close(false);
				return DefaultValue;
			}

			if (!IsPropExists)
				Value = DefaultValue;

			return Value;
		}

		private static dynamic Open(string DocFullPath, bool ReadOnly)
		{
			dynamic DsoProp = GetDsoDocProp();
			if (DsoProp == null)
				return null;

			//개체는 생성되나
			//'Open' 메서드('_OleDocumentProperties' 개체의)에서 오류가 발생하였습니다
			//에러 발생하는 경우 있음.
			try
			{
				DsoProp.Open(DocFullPath, ReadOnly, 2); //dsoOptionOpenReadOnlyIfNoWriteAccess
			}
			catch (Exception)
			{
				DsoProp.Close(false);
				return null;
			}

			if (!ReadOnly && (DsoProp.IsReadOnly))
			{
				DsoProp.Close(false);
				return null;
			}

			return DsoProp;
		}

		public static NameValueCollection GetCustomPropertyNameAndValueList(string DocFullPath)
		{
			NameValueCollection nv = new NameValueCollection();

			dynamic DsoProp = GetDsoDocProp();
			if (DsoProp == null)
				return nv;

			//개체는 생성되나
			//'Open' 메서드('_OleDocumentProperties' 개체의)에서 오류가 발생하였습니다
			//에러 발생하는 경우 있음.
			try
			{
				DsoProp.Open(DocFullPath, true, 2); //dsoOptionOpenReadOnlyIfNoWriteAccess
			}
			catch (Exception)
			{
				DsoProp.Close(false);
				return nv;
			}

			foreach (dynamic Prop in DsoProp.CustomProperties)
			{
				nv.Add(Prop.Name, Prop.Value);
			}

			DsoProp.Close(false);

			return nv;
		}

		private static dynamic GetDsoDocProp()
		{
			dynamic DsoProp = CreateDsoDocProp();

			if (DsoProp == null)
			{
				//32비트용이므로 C:\windows\SysWOW64에 있음.
				string DsoFileFullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.SystemX86), "dsofile.dll");
				if (!File.Exists(DsoFileFullPath))
					return null;

				try
				{
					CRegComponent.Register(DsoFileFullPath);
				}
				catch (Exception) { }

				DsoProp = CreateDsoDocProp();
			}

			return DsoProp;
		}
		private static dynamic CreateDsoDocProp()
		{
			dynamic DsoProp = null;
			try
			{
				Type t = Type.GetTypeFromProgID("DSOFile.OleDocumentProperties");
				DsoProp = Activator.CreateInstance(t);
			}
			catch (Exception)
			{

			}

			return DsoProp;
		}
	}
#endif
}
