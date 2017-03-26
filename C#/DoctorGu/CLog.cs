using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DoctorGu;
using System.Reflection;
using System.IO;
using System.Timers;
using System.Diagnostics;
#if !DotNet35
using System.Collections.Concurrent;
using System.Threading;
#endif

namespace DoctorGu
{
    public class CWriteLogEvent : EventArgs
    {
        public string Log;
    }

    public enum LogFileNameFormat
    {
        yyyyMMdd,
        yyyyMMddHH,
    }

    public enum LogFileNamePrefix
    {
        Success,
        Fail,
        Error,
        Task,
        Test,
    }

    /// <summary>
    /// <![CDATA[
    /// for (int i = 0; i < 100; i++)
    /// {
    ///     Thread th = new Thread(WriteLogThread);
    ///     th.Name = "th" + i.ToString();
    ///     th.Start();
    /// }			
    /// private static void WriteLogThread()
    /// {
    ///		CLog Log = new CLog();
    ///     for (int i = 0; i < 100; i++)
    ///     {
    ///         string s = Thread.CurrentThread.Name + " " + i.ToString() + " " + DateTime.Now.ToString();
    ///         Log.WriteLog(LogTypes.Test, s);
    ///     }
    /// }
    /// ]]>
    /// </summary>
    public class CLog
    {
        private static string _LogFolder;

        private ReaderWriterLock _locker = new ReaderWriterLock();

        public event EventHandler<CWriteLogEvent> BeforeWriteLog;

        static CLog()
        {
            Assembly Assem = CAssembly.GetEntryOrExecuting();
            if (Assem == null)
                throw new Exception("Assem is null");

            string AppFolder = CAssembly.GetFolder(Assem);
            string AppName = Assem.GetName().Name;
            _LogFolder = Path.Combine(AppFolder, "Log\\" + AppName);
        }

        public CLog(string LogFolder)
        {
            if (!string.IsNullOrEmpty(LogFolder))
                _LogFolder = LogFolder;

            if (!Directory.Exists(_LogFolder))
                Directory.CreateDirectory(_LogFolder);
        }
        public CLog() : this(null) { }

        private string GetLogFullPath(Enum FileNamePrefix, LogFileNameFormat FileNameFormat)
        {
            string FileName = string.Format("{0}{1}.log", FileNamePrefix, DateTime.Now.ToString(FileNameFormat.ToString()));

            return Path.Combine(_LogFolder, FileName);
        }

        public void WriteLog(string LogFullPath, string Log)
        {
            try
            {
                _locker.AcquireWriterLock(int.MaxValue);
                File.AppendAllText(LogFullPath, Log);
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }
        }
        public void WriteLog(Enum FileNamePrefix, string Log, LogFileNameFormat FileNameFormat)
        {
            if (BeforeWriteLog != null)
                BeforeWriteLog(null, new CWriteLogEvent() { Log = Log });

            string LogFullPath = GetLogFullPath(FileNamePrefix, FileNameFormat);

            Log = "==================================================\r\n"
                + DateTime.Now.ToString(CConst.Format_yyyy_MM_dd_HH_mm_ss_fff) + "\r\n"
                + Log + "\r\n";

            WriteLog(LogFullPath, Log);
        }
        public void WriteLog(Enum FileNamePrefix, string Log)
        {
            LogFileNameFormat FileNameFormat = LogFileNameFormat.yyyyMMdd;
            WriteLog(FileNamePrefix, Log, FileNameFormat);
        }
        public void WriteLog(Exception ex, LogFileNameFormat FileNameFormat)
        {
            string Log = CInfo.GetExceptionText(ex);
            WriteLog(LogFileNamePrefix.Error, Log, FileNameFormat);
        }
        public void WriteLog(Exception ex)
        {
            string Log = CInfo.GetExceptionText(ex);
            LogFileNameFormat FileNameFormat = LogFileNameFormat.yyyyMMdd;
            WriteLog(LogFileNamePrefix.Error, Log, FileNameFormat);
        }
        public void WriteLog(Enum FileNamePrefix, Exception ex)
        {
            string Log = CInfo.GetExceptionText(ex);
            LogFileNameFormat FileNameFormat = LogFileNameFormat.yyyyMMdd;
            WriteLog(FileNamePrefix, Log, FileNameFormat);
        }
        public void WriteLog(string Log, Exception ex, LogFileNameFormat FileNameFormat)
        {
            Log = "Log:" + Log + "\r\n" + CInfo.GetExceptionText(ex);
            WriteLog(LogFileNamePrefix.Error, Log, FileNameFormat);
        }
        public void WriteLog(string Log, Exception ex)
        {
            Log = "Log:" + Log + "\r\n" + CInfo.GetExceptionText(ex);
            LogFileNameFormat FileNameFormat = LogFileNameFormat.yyyyMMdd;
            WriteLog(LogFileNamePrefix.Error, Log, FileNameFormat);
        }

        //public void MessageAndLogAndThrow(string Msg, Exception ex, LogFileNameFormat FileNameFormat)
        //{
        //    System.Windows.Forms.MessageBox.Show(Msg, "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        //    WriteLog(ex, FileNameFormat);
        //    throw new Exception(Msg, ex);
        //}
        //public void MessageAndLogAndThrow(string Msg, Exception ex)
        //{
        //    MessageAndLogAndThrow(Msg, ex, LogFileNameFormat.yyyyMMdd);
        //}
        //public void MessageAndLog(string Msg, Exception ex, LogFileNameFormat FileNameFormat)
        //{
        //    System.Windows.Forms.MessageBox.Show(Msg, "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        //    WriteLog(ex, FileNameFormat);
        //}
        //public void MessageAndLog(string Msg, Exception ex)
        //{
        //    MessageAndLog(Msg, ex, LogFileNameFormat.yyyyMMdd);
        //}
        //public void MessageAndLog(string Msg, Enum FileNamePrefix, string Log, LogFileNameFormat FileNameFormat)
        //{
        //    System.Windows.Forms.MessageBoxIcon Icon = (FileNamePrefix.ToString().IndexOf("Error") != -1) ? System.Windows.Forms.MessageBoxIcon.Error : System.Windows.Forms.MessageBoxIcon.Information;
        //    System.Windows.Forms.MessageBox.Show(Msg, "Information", System.Windows.Forms.MessageBoxButtons.OK, Icon);
        //    WriteLog(FileNamePrefix, Log, FileNameFormat);
        //}
        //public void MessageAndLog(string Msg, Enum FileNamePrefix, string Log)
        //{
        //    MessageAndLog(Msg, FileNamePrefix, Log, LogFileNameFormat.yyyyMMdd);
        //}
    }
}
