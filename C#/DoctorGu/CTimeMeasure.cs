using System;
using System.Net;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;

namespace DoctorGu
{
	public class CTimeMeasure : IDisposable
	{
		private long _StartTicks = 0;

		public CTimeMeasure()
		{
			this._StartTicks = DateTime.Now.Ticks;
		}

		#region IDisposable Members

		public void Dispose()
		{
			//Debug.WriteLine(GetElapsedText());
		}

		#endregion


		public string GetElapsedText()
		{
			return GetText(GetElapsed());
		}

		public long GetElapsed()
		{
			return (DateTime.Now.Ticks - this._StartTicks);
		}

		public int GetElapsedSeconds()
		{
			return Convert.ToInt32((new TimeSpan(GetElapsed())).TotalSeconds);
		}

		private string GetText(long Ticks)
		{
			TimeSpan spn = new TimeSpan(Ticks);
			return string.Concat(spn.Hours.ToString("00"),
				":", spn.Minutes.ToString("00"),
				":", spn.Seconds.ToString("00"),
				":", spn.Milliseconds.ToString("000")
				);
		}

		private long mBookmark;
		public void SetBookmark()
		{
			this.mBookmark = DateTime.Now.Ticks;
		}
		public bool HasBookmark
		{
			get { return (this.mBookmark != 0); }
		}

		public long GetElapsedToBookmark()
		{
			return (this.mBookmark - this._StartTicks);
		}

		//public long GetCenterOfBookmarkAndElapsed()
		//{
		//    return DateTime.Now.Ticks - ((this.mBookmark + this.mStartTicks) / 2);
		//}

		//private Dictionary<string, long> maLogTick = new Dictionary<string, long>();
		//public void SaveLogTicks(string Key)
		//{
		//    this.maLogTick[Key] = DateTime.Now.Ticks;
		//}
		//public long GetLogTicks(string Key)
		//{
		//    return this.maLogTick[Key];
		//}
	}
}
