using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DoctorGu
{
	[Obsolete("Use SoundPlayer")]
	public class CSound : IDisposable
	{
		[System.Runtime.InteropServices.DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true)]
		private static extern bool PlaySound(string szSound, System.IntPtr hMod, PlaySoundFlags flags);

		[System.Flags]
		public enum PlaySoundFlags : int
		{
			SND_SYNC = 0x0000,
			SND_ASYNC = 0x0001,
			SND_NODEFAULT = 0x0002,
			SND_LOOP = 0x0008,
			SND_NOSTOP = 0x0010,
			SND_NOWAIT = 0x00002000,
			SND_FILENAME = 0x00020000,
			SND_RESOURCE = 0x00040004
		}

		private Timer mTmr = null;
		private bool mIsPlaying = false;

		public static void PlayWaveSound(string WaveFullPath)
		{
			PlaySound(WaveFullPath, new System.IntPtr(), PlaySoundFlags.SND_SYNC);
		}

		/// <summary>
		/// 이 프로시저는 제대로 작동하지 않는 경우 있어 폼의 Timer 컨트롤을 이용하는 것이 좋음.
		/// </summary>
		/// <param name="WaveFullPath"></param>
		public void PlayWaveSoundInfinite(string WaveFullPath)
		{
			mTmr = new Timer(tmr_PlaySound, WaveFullPath, 0, 10);
		}
		public void StopWaveSoundInfinite()
		{
			mTmr.Dispose();
		}

		private void tmr_PlaySound(object state)
		{
			if (mIsPlaying)
				return;


			mIsPlaying = true;

			string WaveFullPath = (string)state;
			PlaySound(WaveFullPath, new System.IntPtr(), PlaySoundFlags.SND_SYNC);

			mIsPlaying = false;
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (mTmr != null)
				mTmr.Dispose();
		}

		#endregion
	}
}
