using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !DotNet35
using System.Dynamic;
#endif
using System.Reflection;

namespace DoctorGu
{
#if !DotNet35
	//http://msdn.microsoft.com/en-us/library/system.dynamic.dynamicobject.tryinvokemember.aspx

	public class CDynamicDictionary : DynamicObject
	{
		Dictionary<string, object> _Dic = new Dictionary<string, object>();

		//d["Name"] = "홍길동"과 같이 쓸 수 있게 함.
		public object this[string Name]
		{
			get { return _Dic[Name]; }
			set { _Dic[Name] = value; }
		}

		// Getting a property.
		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			return _Dic.TryGetValue(binder.Name, out result);
		}

		// Setting a property.
		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			_Dic[binder.Name] = value;
			return true;
		}

		// Calling a method.
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			Type DicType = typeof(Dictionary<string, object>);
			try
			{
				result = DicType.InvokeMember(binder.Name, BindingFlags.InvokeMethod, null, _Dic, args);
				return true;
			}
			catch
			{
				result = null;
				return false;
			}
		}

		// This methods prints out dictionary elements.
		public override string ToString()
		{
			string s = "";
			foreach (var pair in _Dic)
				s += ", " + pair.Key + ": " + pair.Value;
			if (s != "")
				s = s.Substring(2);

			return s;
		}
	}
#endif
}
