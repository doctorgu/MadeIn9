﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

//http://www.codeproject.com/KB/cs/sync_volumecontrol.aspx

namespace DoctorGu
{
	/// <summary>
	/// XP에서 시스템 Volume 조절하기 위함.
	/// </summary>
	public class CSpeakerVolumeEventArgs : EventArgs
	{
		public bool VolumeChanged;
	}

	public class CSpeakerVolume : IMessageFilter, IDisposable
	{
		public event EventHandler<CSpeakerVolumeEventArgs> OnVolumeNotification;

		// This is the only error code we will use
		public const int MMSYSERR_NOERROR = 0;

		//-------------------------------------------------------------------

		public const int MAXPNAMELEN = 32;

		public const int MIXER_SHORT_NAME_CHARS = 16;
		public const int MIXER_LONG_NAME_CHARS = 64;

		public const int MIXER_GETLINECONTROLSF_ONEBYTYPE = 2;
		public const int MIXER_GETLINEINFOF_COMPONENTTYPE = 3;

		public const int MIXER_GETCONTROLDETAILSF_VALUE = 0;
		public const int MIXER_SETCONTROLDETAILSF_VALUE = 0;

		//-------------------------------------------------------------------

		public const int MIXERCONTROL_CT_CLASS_SWITCH = 0x20000000;
		public const int MIXERCONTROL_CT_CLASS_FADER = 0x50000000;

		public const int MIXERCONTROL_CT_UNITS_BOOLEAN = 0x10000;
		public const int MIXERCONTROL_CT_UNITS_UNSIGNED = 0x30000;

		//-------------------------------------------------------------------

		public const int MIXERCONTROL_CONTROLTYPE_FADER =
			(MIXERCONTROL_CT_CLASS_FADER | MIXERCONTROL_CT_UNITS_UNSIGNED);

		public const int MIXERCONTROL_CONTROLTYPE_VOLUME =
			(MIXERCONTROL_CONTROLTYPE_FADER + 1);

		public const int MIXERCONTROL_CONTROLTYPE_BOOLEAN =
			(MIXERCONTROL_CT_CLASS_SWITCH | MIXERCONTROL_CT_UNITS_BOOLEAN);

		public const int MIXERCONTROL_CONTROLTYPE_MUTE =
			(MIXERCONTROL_CONTROLTYPE_BOOLEAN + 2);

		//-------------------------------------------------------------------

		// DST COMPONENT TYPE
		public const int MIXERLINE_COMPONENTTYPE_DST_SPEAKERS = 4;

		//-------------------------------------------------------------------

		// This is the callback notification which we use for synchronization
		public const int MM_MIXM_CONTROL_CHANGE = 0x3D1;

		//-------------------------------------------------------------------

		// This is the parameter used to enable .._CONTROL_CHANGE notifications
		public const int CALLBACK_WINDOW = 0x10000;

		//-------------------------------------------------------------------

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]                                                    // mixerOpen
		private static extern int mixerOpen(
										out int phmx,
										int uMxId,
										int dwCallback,
										int dwInstance,
										int fdwOpen);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]                                                    // mixerClose
		private static extern int mixerClose(int hmx);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]                                                    // mixerGetControlDetailsA
		private static extern int mixerGetControlDetailsA(
											int hmxobj,
											ref MIXERCONTROLDETAILS pmxcd,
											int fdwDetails);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]                                                    // mixerGetLineControlsA
		private static extern int mixerGetLineControlsA(
											int hmxobj,
											ref MIXERLINECONTROLS pmxlc,
											int fdwControls);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]                                                    // mixerGetLineInfoA
		private static extern int mixerGetLineInfoA(
											int hmxobj,
											ref MIXERLINE pmxl,
											int fdwInfo);

		[DllImport("winmm.dll", CharSet = CharSet.Ansi)]                                                    // mixerSetControlDetails
		private static extern int mixerSetControlDetails(
											int hmxobj,
											ref MIXERCONTROLDETAILS pmxcd,
											int fdwDetails);

		// -----------------------------------------------------------------------

		public struct MIXERCONTROL                                                                          // MIXERCONTROL
		{
			public int cbStruct;                    // Size in bytes of MIXERCONTROL
			public int dwControlID;                 // Unique control ID for mixer device
			public int dwControlType;               // MIXERCONTROL_CONTROLTYPE_xxx
			public int fdwControl;                  // MIXERCONTROL_CONTROLF_xxx
			public int cMultipleItems;              // If MIXERCONTROL_CONTROLF_MULTIPLE set

			[MarshalAs(UnmanagedType.ByValTStr,
			SizeConst = MIXER_SHORT_NAME_CHARS)]
			public string szShortName;              // Short name of control

			[MarshalAs(UnmanagedType.ByValTStr,
			SizeConst = MIXER_LONG_NAME_CHARS)]
			public string szName;                   // Long name of control

			public int lMinimum;                    // Minimum value
			public int lMaximum;                    // Maximum value

			[MarshalAs(UnmanagedType.U4,
			SizeConst = 10)]
			public int reserved;
		}

		public struct MIXERCONTROLDETAILS                                                                   // MIXERCONTROLDETAILS
		{
			public int cbStruct;                    // Size in bytes of MIXERCONTROLDETAILS
			public int dwControlID;                 // Control ID to get/set details on
			public int cChannels;                   // Number of channels in paDetails array
			public int item;                        // hwndOwner or cMultipleItems
			public int cbDetails;                   // Size of _one_details_XX struct
			public IntPtr paDetails;                // Pointer to array of details_XX_struct
		}

		public struct MIXERCONTROLDETAILS_UNSIGNED                                                          // MIXERCONTROLDETAILS_UNSIGNED
		{
			public int dwValue;
		}

		public struct MIXERLINE                                                                             // MIXERLINE
		{
			public int cbStruct;                    // Size of MIXERLINE struct
			public int dwDestination;               // Zero based destination index
			public int dwSource;                    // Zero based source index (if source)
			public int dwLineID;                    // Unique line ID for mixer device
			public int fdwLine;                     // State/information about line
			public int dwUser;                      // Driver specific information
			public int dwComponentType;             // Component type line connects to
			public int cChannels;                   // Number of channels line supports
			public int cConnections;                // Number of possible connections
			public int cControls;                   // Number of controls belonging to this line

			[MarshalAs(UnmanagedType.ByValTStr,
			SizeConst = MIXER_SHORT_NAME_CHARS)]
			public string szShortName;              // Short name of control

			[MarshalAs(UnmanagedType.ByValTStr,
			SizeConst = MIXER_LONG_NAME_CHARS)]
			public string szName;                   // Long name of control

			public int dwType;
			public int dwDeviceID;
			public int wMid;                        // Manufacturer ID                        
			public int wPid;                        // Product ID
			public int vDriverVersion;              // Driver Vers. No.

			[MarshalAs(UnmanagedType.ByValTStr,
			SizeConst = MAXPNAMELEN)]
			public string szPname;                  // Product name
		}

		public struct MIXERLINECONTROLS                                                                     // MIXERLINECONTROLS
		{
			public int cbStruct;                    // Size in bytes of MIXERLINECONTROLS
			public int dwLineID;                    // Line ID (from MIXERLINE.dwLineID)
			// MIXER_GETLINECONTROLSF_ONEBYID or
			public int dwControl;                   // MIXER_GETLINECONTROLSF_ONEBYTYPE
			public int cControls;                   // Number of controls pamxctrl points to
			public int cbmxctrl;                    // Size in bytes of _one_ MIXERCONTROL
			public IntPtr pamxctrl;                 // Pointer to first MIXERCONTROL array
		}

		private const int SpeakerVolumeControl = MIXERCONTROL_CONTROLTYPE_VOLUME;
		private const int SpeakerComponent = MIXERLINE_COMPONENTTYPE_DST_SPEAKERS;
		private const int SpeakerMuteControl = MIXERCONTROL_CONTROLTYPE_MUTE;
		int SpeakerVol = 0;      // 0 .. 65535

		public int SpeakerVolumeControlID;
		public int SpeakerMuteControlID;


		public bool PreFilterMessage(ref Message m)
		{
			if (m.Msg == CSpeakerVolume.MM_MIXM_CONTROL_CHANGE)     // Code 0x3D1 indicates a control change
			{
				int i = (int)(m.LParam);

				// We can't use switch so we must do it this way:
				bool IsVolume = (i == this.SpeakerVolumeControlID) ? true : false;
				bool IsMute = (i == this.SpeakerMuteControlID) ? true : false;

				if (IsVolume || IsMute)
				{
					bool VolumeChanged = IsVolume;

					if (this.OnVolumeNotification != null)
						this.OnVolumeNotification(this, new CSpeakerVolumeEventArgs() { VolumeChanged = VolumeChanged });
				}
			}

			return false;
		}

		public void Dispose()
		{
			Application.RemoveMessageFilter(this);
		}

		public CSpeakerVolume(IntPtr FormHandle)
		{
			if (CheckMixer() != MMSYSERR_NOERROR)
				throw new Exception("Could not load control details - mixer is inaccessible");

			this.SpeakerVolumeControlID = GetControlID(SpeakerComponent, SpeakerVolumeControl);
			this.SpeakerMuteControlID = GetControlID(SpeakerComponent, SpeakerMuteControl);

			if ((this.SpeakerVolumeControlID < 0) || (this.SpeakerMuteControlID < 0))
				throw new Exception("The GetControlID(..) function failed");


			//// Set up a window to receive MM_MIXM_CONTROL_CHANGE messages ...
			//SubclassHWND w = new SubclassHWND();
			//w.AssignHandle(FormHandle);

			//// Note that the window's handle needs to be cast as an integer before it can be used
			//int iw = (int)FormHandle;

			//... and we can now activate the message monitor 
			bool b = MonitorControl((int)FormHandle);
			Application.AddMessageFilter(this);
		}

		public int CheckMixer()                                                                      // CheckMixer
		{
			int retValue = -1;
			int rc1, rc2 = -1;
			int hmixer;

			rc1 = mixerOpen(
						out hmixer,
						0,
						0,
						0,
						0);

			rc2 = mixerClose(hmixer);

			return retValue = (MMSYSERR_NOERROR == rc1) && (MMSYSERR_NOERROR == rc2) ? MMSYSERR_NOERROR : retValue;
		}

		public int GetControlID(int component, int control)
		{
			MIXERCONTROL mxc = new MIXERCONTROL();
			int _i;         // Though we won't need _i, it still must be declared                                    

			bool b = false;
			int retValue = 0;

			b = GetMixerControl(
							0,
							component,
							control,
							out mxc,
							out _i);

			return retValue = b ? mxc.dwControlID : -1;
		}

		public bool MonitorControl(int iw)     // iw is the window handle                          // MonitorControl
		{
			int rc = -1;
			bool retValue = false;

			int hmixer;
			rc = mixerOpen(
						out hmixer,
						0,
						iw,
						0,
						CALLBACK_WINDOW);

			return retValue = (MMSYSERR_NOERROR == rc) ? true : false;
		}

		public bool GetMute()
		{
			int MuteStatus = GetVolume(SpeakerMuteControl, SpeakerComponent);
			return (MuteStatus > 0 ? true : false);
		}
		public int GetVolume(int Maximum)
		{
			int CurrVol = GetVolume(CSpeakerVolume.SpeakerVolumeControl, CSpeakerVolume.SpeakerComponent);

			// Calculate the volume
			ushort CalcVol = (ushort)(CurrVol & 0x0000ffff);
			// Get the volume on a scale of 1 to 10 (to fit the trackbar)
			return CalcVol / (ushort.MaxValue / Maximum);
		}
		private int GetVolume(int control, int component)
		{
			int hmixer = 0;
			int currVol = -1;

			MIXERCONTROL volCtrl = new MIXERCONTROL();

			int rc = mixerOpen(
							out hmixer,
							0,
							0,
							0,
							0);

			bool b = GetMixerControl(
								hmixer,
								component,
								control,
								out volCtrl,        // Not used
								out currVol);

			mixerClose(hmixer);

			return currVol;
		}

		public void SetMute(bool Mute)
		{
			int MuteStatus = Mute ? 1 : 0;
			SetVolume(SpeakerMuteControl, SpeakerComponent, MuteStatus);
		}
		public void SetVolume(int Value, int Maximum)
		{
			// Calculate the volume that's being set
			int NewVolume = ((ushort.MaxValue / Maximum) * Value);
			// Set the volume
			SetVolume(SpeakerVolumeControl, SpeakerComponent, NewVolume);
		}
		private void SetVolume(int control, int component, int newVol)                              // SetVolume
		{
			int hmixer = 0;
			int currentVol;

			MIXERCONTROL volCtrl = new MIXERCONTROL();

			mixerOpen(
					out hmixer,
					0,
					0,
					0,
					0);

			GetMixerControl(
						hmixer,
						component,
						control,
						out volCtrl,
						out currentVol);            // Not used

			SetVolumeControl(hmixer, volCtrl, newVol);

			mixerClose(hmixer);
		}

		private bool SetVolumeControl(                                                               // SetVolumeControl
							int hmixer,
							MIXERCONTROL mxc,
							int volume)
		{
			bool retValue = false;
			int rc;

			MIXERCONTROLDETAILS mxcd = new MIXERCONTROLDETAILS();
			MIXERCONTROLDETAILS_UNSIGNED vol = new MIXERCONTROLDETAILS_UNSIGNED();

			mxcd.item = 0;
			mxcd.dwControlID = mxc.dwControlID;
			mxcd.cbStruct = Marshal.SizeOf(mxcd);
			mxcd.cbDetails = Marshal.SizeOf(vol);

			// Allocate a buffer for the control value buffer 
			mxcd.cChannels = 1;
			vol.dwValue = volume;

			// Copy the data into the control value buffer 
			mxcd.paDetails = Marshal.AllocCoTaskMem(
								Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_UNSIGNED)));

			Marshal.StructureToPtr(
								vol,
								mxcd.paDetails,
								false);

			// Set the control value 
			rc = mixerSetControlDetails(
									hmixer,
									ref mxcd,
									MIXER_SETCONTROLDETAILSF_VALUE);

			return retValue = MMSYSERR_NOERROR == rc ? true : false;
		}

		// This function attempts to obtain a specified mixer control/component pair - 
		// returns true if successful.
		private static bool GetMixerControl(                                                                // GetMixerControl
										int hmixer,
										int component,
										int control,
										out MIXERCONTROL mxc,
										out int vCurrentVol)
		{
			bool retValue = false;
			mxc = new MIXERCONTROL();

			MIXERLINECONTROLS mxlc = new MIXERLINECONTROLS();
			MIXERLINE mxl = new MIXERLINE();
			MIXERCONTROLDETAILS pmxcd = new MIXERCONTROLDETAILS();
			MIXERCONTROLDETAILS_UNSIGNED du = new MIXERCONTROLDETAILS_UNSIGNED();

			vCurrentVol = -1;       // A dummy value

			mxl.cbStruct = Marshal.SizeOf(mxl);
			mxl.dwComponentType = component;

			int rc = mixerGetLineInfoA(
								hmixer,
								ref mxl,
								MIXER_GETLINEINFOF_COMPONENTTYPE);

			if (MMSYSERR_NOERROR == rc)
			{
				int sizeofMIXERCONTROL = 152;
				int ctrl = Marshal.SizeOf(typeof(MIXERCONTROL));
				mxlc.pamxctrl = Marshal.AllocCoTaskMem(sizeofMIXERCONTROL);
				mxlc.cbStruct = Marshal.SizeOf(mxlc);
				mxlc.dwLineID = mxl.dwLineID;
				mxlc.dwControl = control;
				mxlc.cControls = 1;
				mxlc.cbmxctrl = sizeofMIXERCONTROL;

				// Allocate a buffer for the control 
				mxc.cbStruct = sizeofMIXERCONTROL;

				// Get the control 
				rc = mixerGetLineControlsA(
										hmixer,
										ref mxlc,
										MIXER_GETLINECONTROLSF_ONEBYTYPE);

				if (MMSYSERR_NOERROR == rc)
				{
					retValue = true;
					// Copy the control into the destination structure 
					mxc = (MIXERCONTROL)Marshal.PtrToStructure(
															mxlc.pamxctrl,
															typeof(MIXERCONTROL));
				}

				int sizeofMIXERCONTROLDETAILS =
					Marshal.SizeOf(typeof(MIXERCONTROLDETAILS));

				int sizeofMIXERCONTROLDETAILS_UNSIGNED =
					Marshal.SizeOf(typeof(MIXERCONTROLDETAILS_UNSIGNED));

				pmxcd.cbStruct = sizeofMIXERCONTROLDETAILS;
				pmxcd.dwControlID = mxc.dwControlID;
				pmxcd.paDetails = Marshal.AllocCoTaskMem(sizeofMIXERCONTROLDETAILS_UNSIGNED);
				pmxcd.cChannels = 1;
				pmxcd.item = 0;
				pmxcd.cbDetails = sizeofMIXERCONTROLDETAILS_UNSIGNED;

				rc = mixerGetControlDetailsA(
									hmixer,
									ref pmxcd,
									MIXER_GETCONTROLDETAILSF_VALUE);

				du = (MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(
															pmxcd.paDetails,
															typeof(MIXERCONTROLDETAILS_UNSIGNED));

				vCurrentVol = du.dwValue;

				return retValue;    // true
			}

			return retValue;        // false
		}
	}
}
