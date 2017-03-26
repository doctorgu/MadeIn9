//http://www.codeproject.com/KB/cs/csdynamicscrres.aspx

using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace DoctorGu
{
    /// <summary>
    /// 모니터 해상도를 변경함.
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// MessageBox.Show("Resolution is going to change to "+FixWidth.ToString()+" X "+FixHeight.ToString());
    /// int nRet = CResolution.ChangeResolution(FixWidth, FixHeight);
    ///
    /// switch (nRet)
    /// {
    ///     case CResolution.User_32.DISP_CHANGE_SUCCESSFUL:
    ///         {
    ///             //successfull change
    ///             break;
    ///         }
    ///     case CResolution.User_32.DISP_CHANGE_RESTART:
    ///         {
    ///             //windows 9x series you have to restart
    ///             MessageBox.Show("Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    ///             break;
    ///         }
    ///     default:
    ///         {
    ///             //failed to change
    ///             MessageBox.Show("Description: Failed To Change The Resolution.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    ///             break;
    ///         }
    /// }
    /// ]]>
    /// </example>
    public class CResolution
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE1
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;

            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;

            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;

            public int dmDisplayFlags;
            public int dmDisplayFrequency;

            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;

            public int dmPanningWidth;
            public int dmPanningHeight;
        };

        public class User_32
        {
            [DllImport("user32.dll")]
            public static extern int EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE1 devMode);
            [DllImport("user32.dll")]
            public static extern int ChangeDisplaySettings(ref DEVMODE1 devMode, int flags);

            public const int ENUM_CURRENT_SETTINGS = -1;
            public const int CDS_UPDATEREGISTRY = 0x01;
            public const int CDS_FULLSCREEN = 0x04;
            public const int CDS_TEST = 0x02;
            public const int DISP_CHANGE_SUCCESSFUL = 0;
            public const int DISP_CHANGE_RESTART = 1;
            public const int DISP_CHANGE_FAILED = -1;
        }

        public static int ChangeResolution(int Width, int Height)
        {
            Screen screen = Screen.PrimaryScreen;

            DEVMODE1 dm = new DEVMODE1();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);


            if (User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref dm) == 0)
                return User_32.DISP_CHANGE_FAILED;


            dm.dmPelsWidth = Width;
            dm.dmPelsHeight = Height;

            int iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_TEST);

            if (iRet == User_32.DISP_CHANGE_FAILED)
            {
                return iRet;
            }
            else
            {
                /*
                Thats good article, it gave me some initial knowledge and the direction of further searching.
                There is some useful tip that I have encountered while writing my own application:
                You can use another constant CDS_FULLSCREEN = 0x04 instead of CDS_UPDATEREGISTRY = 0x01 when changing resolution:
                [code] User_32.ChangeDisplaySettings(ref dm, User_32.CDS_FULLSCREEN); [/code]
                This gives you some benefits: resolution is changed temporally, you even do not need to restore previous settings - it is done automatically when the application exits. And, this doesn't disorder the icons - the desktop is not even affected.
                I used this in writing a small game that run in full screen - form was covering the whole screen (automatically covering taskbar).
                Maybe this information can help someone else too.
                */
                return User_32.ChangeDisplaySettings(ref dm, User_32.CDS_FULLSCREEN);
                //return User_32.ChangeDisplaySettings(ref dm, User_32.CDS_UPDATEREGISTRY);
            }
        }
    }
}
