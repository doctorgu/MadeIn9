using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace DoctorGu
{
	public class CQueryString
	{
		//Dictionary<string, string>으로 하면 Key가 없을 때 에러나므로
		//Key가 없으면 null을 리턴하는 NameValueCollection을 사용함.
		private NameValueCollection _dicQs = new NameValueCollection();
		public CQueryString(string Url)
		{
			MakeQs(Url);
		}

		public CQueryString(Uri uri)
		{
			MakeQs(uri.AbsoluteUri);
		}

		private void MakeQs(string Url)
		{
			if (string.IsNullOrEmpty(Url))
				return;


			_dicQs.Clear();

			// store the part before, too.
			int qPos = Url.IndexOf("?");
			string Query = "";
			if (qPos >= 0)
			{
				_PathOnly = Url.Substring(0, qPos - 0);
				Query = Url.Substring(qPos + 1);
			}
			else
			{
				if (Url.IndexOf("=") == -1)
					_PathOnly = Url;
				else
					Query = Url;
			}

			if (Query.Length > 0 && Query.Substring(0, 1) == "?")
			{
				Query = Query.Substring(1);
			}

			//파라미터가 없는 경우엔 빠져나감.
			if (Query.IndexOf('=') == -1)
			{
				return;
			}

			string[] aQuery = Query.Split('&');
			foreach (string NameValue in aQuery)
			{
				KeyValuePair<string, string> kv = GetNameValue(NameValue);
				this[kv.Key] = kv.Value;
			}
		}
		private void MakeQs(Uri uri)
		{
			if (uri != null)
			{
				MakeQs(uri.AbsoluteUri);
			}
		}

		private KeyValuePair<string, string> GetNameValue(string NameValue)
		{
			string Name = string.Empty;
			string Value = string.Empty;

			string[] aNameValue = NameValue.Split('=');
			if (aNameValue.Length >= 1)
				Name = aNameValue[0];
			if (aNameValue.Length >= 2)
				Value = aNameValue[1];

			//CWeb.DecodeUrlParam은 한글이 깨져서 사용 안함
			return new KeyValuePair<string, string>(Name, Value);
		}

		public bool HasParameter(string ParameterName)
		{
			if (string.IsNullOrEmpty(ParameterName))
			{
				return false;
			}

			ParameterName = ParameterName.Trim();
			return (this._dicQs[ParameterName] == null);
		}

		public void RemoveParameter(string Name)
		{
			this._dicQs.Remove(Name);
		}
		public void RemoveParameterExcept(string Name)
		{
			for (int i = 0; i < this._dicQs.Count; i++)
			{
				string NameCur = this._dicQs.GetKey(i);
				if (NameCur != Name)
				{
					this._dicQs.Remove(NameCur);
					i--;
				}
			}
		}
		public void RemoveAllParameters()
		{
			this._dicQs.Clear();
		}

		public string this[int Index]
		{
			get { return this._dicQs[Index]; }
			set { this._dicQs[this._dicQs.GetKey(Index)] = value; }
		}
		public string this[string Name]
		{
			get { return this._dicQs[Name]; }
			set { this._dicQs[Name] = value; }
		}

		private string _PathOnly = "";
		public string PathOnly
		{
			get { return _PathOnly; }
			set { _PathOnly = value; }
		}

		public string FileName
		{
			get { return CPath.GetFileName(this._PathOnly, '/'); }
			set
			{
				string Folder = CPath.GetFolderName(this._PathOnly, '/');
				this._PathOnly = CPath.CombineUrl(Folder, value);
			}
		}
		public string Extension
		{
			get { return Path.GetExtension(this.FileName); }
		}
		public string FileNameWithoutExtension
		{
			get { return Path.GetFileNameWithoutExtension(this.FileName); }
			set
			{
				string Folder = CPath.GetFolderName(this._PathOnly, '/');
				string Extension = this.Extension;
				this._PathOnly = CPath.CombineUrl(Folder, (value + Extension));
			}
		}

		public string PathAndQuery
		{
			get
			{
				string Param = this.QueryOnly;
				return this._PathOnly + (string.IsNullOrEmpty(Param) ? "" : "?" + Param);
			}
		}

		public string QueryOnly
		{
			get
			{
				if (this._dicQs.Count == 0)
					return "";

				string Query = "";
				foreach (string Name in this._dicQs)
				{
					string Value = this._dicQs[Name];
					if (!string.IsNullOrEmpty(Value))
					{
						Query += "&" + Name + "=" + Value;
						//한글이 깨져서 주석
						//Param += CWeb.EncodeUrlParam(val);
					}
				}
				Query = Query.Substring(1);

				return Query;
			}
		}

		public string FirstQuery
		{
			get
			{
				if (this._dicQs.Count == 0)
					return "";

				string FirstKey = this._dicQs.GetKey(0);
				//한글이 깨져서 주석
				//string FirstValue = CWeb.EncodeUrlParam(this.mdicQs[FirstKey]);
				string FirstValue = this._dicQs[FirstKey];
				return FirstKey + "=" + FirstValue;
			}
		}
		public string PathAndFirstQuery
		{
			get
			{
				if (this._dicQs.Count == 0)
					return this._PathOnly;

				return this._PathOnly + "?" + this.FirstQuery;
			}
		}

		public void Append(string Query)
		{
			KeyValuePair<string, string> kv = GetNameValue(Query);
			this[kv.Key] = kv.Value;
		}

		public static string AppendQueryString(string Url, string QueryString)
		{
			Url = Url.TrimEnd('?', '&');

			if (!string.IsNullOrEmpty(QueryString))
			{
				if (Url.IndexOf("?") >= 0)
				{
					return Url + "&" + QueryString;
				}
				else
				{
					return Url + "?" + QueryString;
				}
			}
			else
			{
				return Url;
			}
		}
		public static string AppendQueryString(Uri uri, string QueryString)
		{
			return CQueryString.AppendQueryString(uri.AbsoluteUri, QueryString);
		}
	}
}