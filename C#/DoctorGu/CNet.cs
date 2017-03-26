using System;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Specialized;

namespace DoctorGu
{
	public enum DownloadState
	{
		Started, Downloading, Ended
	}

	public class CNet
	{
		//public delegate void StateChanged(string PathFile, DownloadState State, long FileSize);
		//public StateChanged OnStateChanged;
		public event EventHandler<CDownloadProgressChangedEventArgs> DownloadProgress;


		/// <summary>
		/// 특정 Url의 파일을 다운 받음.
		/// </summary>
		/// <param name="Url"> </param>
		public void DownloadFile(string Url, string FullPath, bool Overwrite)
		{
			HttpWebRequest webreq = null;
			HttpWebResponse webres = null;

			try
			{
				webreq = (HttpWebRequest)WebRequest.Create(Url);
				webreq.ReadWriteTimeout = 10 * 1000;
				webres = (HttpWebResponse)webreq.GetResponse();
			}
			catch (Exception ex)
			{
				throw new Exception("Url: " + Url + Environment.NewLine + ex.Message, ex);
			}

			if (Overwrite)
			{
				CFile.DeleteUntilSuccess(FullPath, 1000 * 5);
			}

			using (BinaryWriter bw = new BinaryWriter(new FileStream(FullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None), Encoding.UTF8))
			{
				BinaryReader br = new BinaryReader(webres.GetResponseStream(), Encoding.UTF8);
				long FileSize = webres.ContentLength;

				CDownloadProgressChangedEventArgs arg = new CDownloadProgressChangedEventArgs();
				arg.FullPath = FullPath;
				arg.TotalBytesToReceive = FileSize;

				//if (this.OnStateChanged != null)
				//    this.OnStateChanged(PathFile, DownloadState.Started, FileSize);

				int Max = 4096;
				byte[] Buffer = new byte[Max];

				int BytesRead = 0;
				while ((BytesRead = br.Read(Buffer, 0, Max)) > 0)
				{
					arg.BytesReceived += BytesRead;
					arg.ProgressPercentage = Math.Min(100, Convert.ToInt32((arg.BytesReceived / (double)arg.TotalBytesToReceive) * 100));
					if (this.DownloadProgress != null)
						this.DownloadProgress(this, arg);

					bw.Write(Buffer, 0, BytesRead);
				}
			}
		}

		/// <summary>
		/// 특정 Url의 파일의 내용을 Stream 형식으로 리턴함.
		/// </summary>
		/// <param name="Url"> </param>
		public static Stream GetStreamFromUrl(string Url, string UserAgent)
		{
			HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(Url);
			webreq.ReadWriteTimeout = 10 * 1000;
			if (!string.IsNullOrEmpty(UserAgent))
				webreq.UserAgent = UserAgent;

			HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();

			return webres.GetResponseStream();
		}
		public static Stream GetStreamFromUrl(string Url)
		{
			string UserAgent = "";
			return GetStreamFromUrl(Url, UserAgent);
		}
		/// <summary>
		/// 특정 Url의 HTML 소스를 리턴함.
		/// </summary>
		/// <param name="Url"> </param>
		public static string GetTextFromUrl(string Url, string UserAgent, Encoding Enc)
		{
			Stream strm = GetStreamFromUrl(Url, UserAgent);
			StreamReader sr = new StreamReader(strm, Enc);

			return sr.ReadToEnd();
		}
		public static string GetTextFromUrl(string Url, Encoding Enc)
		{
			string UserAgent = "";
			return GetTextFromUrl(Url, UserAgent, Enc);
		}
		public static string GetTextFromUrl(string Url, string UserAgent)
		{
			Encoding Enc = Encoding.UTF8;
			return GetTextFromUrl(Url, UserAgent, Enc);
		}
		public static string GetTextFromUrl(string Url)
		{
			string UserAgent = "";
			Encoding Enc = Encoding.UTF8;
			return GetTextFromUrl(Url, UserAgent, Enc);
		}
		public static string GetTextFromUrlRetry(string Url, string UserAgent)
		{
			string Html = "";
			for (int i = 0, i2 = 10; i < i2; i++)
			{
				try
				{
					Html = CNet.GetTextFromUrl(Url, UserAgent);
					break;
				}
				catch (Exception)
				{
					if ((i + 1) == i2)
						throw new Exception("can't get Html");
					else
						Thread.Sleep(10000);
				}
			}

			return Html;
		}

		public static Stream GetStreamFromUrlPost(string Url, NameValueCollection nvParam)
		{
			HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(Url);
			WebReq.ReadWriteTimeout = 10 * 1000;
			WebReq.Method = "POST";

			string Value = "";
			if (nvParam != null)
			{
				foreach (string Name in nvParam)
				{
					Value += Name + "=" + nvParam[Name];
				}
			}
			byte[] bytValue = Encoding.UTF8.GetBytes(Value);

			WebReq.ContentType = "application/x-www-form-urlencoded";
			WebReq.ContentLength = bytValue.Length;

			Stream PostData = WebReq.GetRequestStream();
			PostData.Write(bytValue, 0, bytValue.Length);
			PostData.Close();

			HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();
			//Console.WriteLine(WebResp.StatusCode);
			//Console.WriteLine(WebResp.Server);

			return WebResp.GetResponseStream();


			//HttpWebResponse webres = (HttpWebResponse)webreq.GetResponse();
			//return webres.GetResponseStream();
		}
		public static string GetTextFromUrlPost(string Url, NameValueCollection nvParam, Encoding Enc)
		{
			Stream strm = GetStreamFromUrlPost(Url, nvParam);
			StreamReader sr = new StreamReader(strm, Enc);

			return sr.ReadToEnd();
		}
		public static string GetTextFromUrlPost(string Url, NameValueCollection nvParam)
		{
			return GetTextFromUrlPost(Url, nvParam, Encoding.UTF8);
		}

		//public static List<string> GetIpAddresses()
		//{
		//    List<string> aIp = new List<string>();

		//    IPHostEntry iphost = Dns.GetHostEntry(Dns.GetHostName());
		//    if (iphost.AddressList.Length == 0)
		//        return aIp;

		//    foreach (IPAddress ip in iphost.AddressList)
		//    {
		//        //v4만 가져옴.
		//        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
		//        {
		//            aIp.Add(ip.ToString());
		//        }
		//    }

		//    return aIp;
		//}
		//public static string GetIpAddress()
		//{
		//    List<string> aIp = GetIpAddresses();
		//    if (aIp.Count == 0)
		//        return "";

		//    return aIp[0];
		//}

		public static List<IPAddress> GetIpAddresses()
		{
			// This works on both Mono and .NET , but there is a difference: it also 
			// includes the LocalLoopBack so we need to filter that one out 
			List<IPAddress> Addresses = new List<IPAddress>();
			// Obtain a reference to all network interfaces in the machine 
			NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in adapters)
			{
				IPInterfaceProperties properties = adapter.GetIPProperties();
				foreach (IPAddressInformation uniCast in properties.UnicastAddresses)
				{
					// Ignore loop-back addresses & IPv6 
					if (!IPAddress.IsLoopback(uniCast.Address) && uniCast.Address.AddressFamily != AddressFamily.InterNetworkV6)
						Addresses.Add(uniCast.Address);
				}

			}
			return Addresses;
		}

		public static bool GetInternetConnected()
		{
            string[] aTestSite = new string[] { "www.google.com", "www.microsoft.com" };
            return GetHostConnected(aTestSite);
		}

        public static bool GetHostConnected(string[] UrlListToTest)
        {

            for (int i = 0; i < UrlListToTest.Length; i++)
            {
                try
                {
                    Uri u = new Uri(UrlListToTest[i]);
                    string Host = u.DnsSafeHost;
                    System.Net.IPHostEntry iph = System.Net.Dns.GetHostEntry(Host);
                    return true;
                }
                catch (Exception) { }
            }

            return false;
            //return (nSuccess == aTestSite.Length);
        }
	}
}
