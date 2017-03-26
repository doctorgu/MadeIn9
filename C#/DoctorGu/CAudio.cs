using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;

namespace DoctorGu
{
	/*
*/
	public enum WaveToMp3Preset
	{
		[Description("cbr 320")]
		Cbr320, //320 kbit/s CBR cbr 320 is the exact same thing as --alt-preset insane 
		[Description("cbr 256")]
		Cbr256, //256 kbit/s CBR 
		[Description("cbr 192")]
		Cbr192, //192 kbit/s CBR 
		[Description("cbr 160")]
		Cbr160, //160 kbit/s CBR 
		[Description("cbr 128")]
		Cbr128, //128 kbit/s CBR 
		[Description("cbr 96")]
		Cbr096, //96 kbit/s CBR
		[Description("cbr 64")]
		Cbr064, //64 kbit/s CBR
		[Description("cbr 32")]
		Cbr032, //32 kbit/s CBR

		[Description("standard")]
		Standard, //(~190 kbit/s, typical 180 ... 220) 
		[Description("fast standard")]
		FastStandard, //(~190 kbit/s, faster but potentially lower quality) 
		[Description("extreme")]
		Extreme, //(~250 kbit/s, typical 220 ... 270) 
		[Description("fast extreme")]
		FastExtreme, //(~250 kbit/s, faster but potentially lower quality) 
		[Description("insane")]
		Insane, //(320 kbit/s CBR, highest possible quality) 

	}

	/// <summary>MP3 파일 플레이용(Wave는 SoundPlayer 사용)</summary>
	public class CAudio
	{
		[DllImport("winmm.dll")]  //Command Send
		private static extern int mciSendCommand(int wDeviceID, int uMessage, int dwParam1, int dwParam2);
		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]
		private static extern int mciGetDeviceID([MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer);
		[DllImport("winmm.dll")]
		private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);

		[DllImport("winmm.dll")]
		private static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);
		[DllImport("winmm.dll")]
		private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);


		public static void OpenMediaFile(string FullPath)
		{
			string Extension = Path.GetExtension(FullPath);
			string Type = GetType(Extension);

			string Command = "open \"" + FullPath + "\" type " + Type + " alias MediaFile";
			mciSendString(Command, null, 0, IntPtr.Zero);
		}
		private static string GetType(string Extension)
		{
			Extension = Extension.ToLower();
			switch (Extension)
			{
				case "avi":
					return "avivideo";
				case "cda":
					return "CDAudio";
				case "mid":
				case "rmi":
					return "Sequencer";
				case "wav":
					return "WaveAudio";
				default:
					return "MPEGVideo";
			}
		}

		public static void PlayMediaFile()
		{
			string Command = "play MediaFile";
			mciSendString(Command, null, 0, IntPtr.Zero);
		}

		public static void CloseMediaFile()
		{
			string Command = "close MediaFile";
			mciSendString(Command, null, 0, IntPtr.Zero);
		}

		public static void CloseAndOpenAndPlayMediaFile(string FullPath)
		{
			CloseMediaFile();
			OpenMediaFile(FullPath);
			PlayMediaFile();
		}

		private static int _Volume = 500;
		public static int VolumeOfMediaFile
		{
			get { return _Volume; }
			set
			{
				bool flag = false;

				for (int i = 0; i < 3; i++)
				{
					try
					{
						mciSendString(string.Concat("setaudio MediaFile volume to ", value), null, 0, IntPtr.Zero);
						flag = true;
					}
					catch (Exception)
					{
					}

					if (flag == true)
						break;

					System.Threading.Thread.Sleep(1000);
				}

				_Volume = value;
			}
		}

		public static int GetVolume(int Maximum)
		{
			// By the default set the volume to 0
			uint CurrVol = 0;
			// At this point, CurrVol gets assigned the volume
			waveOutGetVolume(IntPtr.Zero, out CurrVol);
			// Calculate the volume
			ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
			// Get the volume on a scale of 1 to 10 (to fit the trackbar)
			return CalcVol / (ushort.MaxValue / Maximum);
		}

		public static void SetVolume(int Maximum, int Value)
		{
			// Calculate the volume that's being set
			int NewVolume = ((ushort.MaxValue / Maximum) * Value);
			// Set the same volume for both the left and the right channels
			uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
			// Set the volume
			waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
		}

		public static void ConvertWaveToMp3(string FullPathWave, string FullPathMp3, string LameFullPath, WaveToMp3Preset Preset)
		{
			if (string.IsNullOrEmpty(LameFullPath))
				LameFullPath = "lame.exe";

			//lame.exe --preset cbr 256 test.wav test.mp3
			//lame.exe --preset fast standard test.wav test.mp3
			string sPreset = CEnum.GetDescriptionByValue<WaveToMp3Preset>(Preset);
			Process p = new Process();
			string Arguments = string.Format(@"--preset {0} ""{1}"" ""{2}""", sPreset, FullPathWave, FullPathMp3);
			ProcessStartInfo ps = new ProcessStartInfo(LameFullPath, Arguments);
			ps.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo = ps;
			p.Start();
			p.WaitForExit(10000);
		}
		public static void ConvertWaveToMp3(string FullPathWave, string FullPathMp3)
		{
			string LameFullPath = "";
			WaveToMp3Preset Preset = WaveToMp3Preset.Cbr064;
			ConvertWaveToMp3(FullPathWave, FullPathMp3, LameFullPath, Preset);
		}
		public static void ConvertWaveToMp3(string FullPathWave, string FullPathMp3, WaveToMp3Preset Preset)
		{
			string LameFullPath = "";
			ConvertWaveToMp3(FullPathWave, FullPathMp3, LameFullPath, Preset);
		}

	}
}
