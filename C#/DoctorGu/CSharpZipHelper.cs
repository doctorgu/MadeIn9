using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;

namespace DoctorGu
{
	public class CBeforeCompressEventArgs : EventArgs
	{
		public string FullPathSrc;
		public string FolderNameInZip;

		public string NewFolderNameInZipIs;
		public bool Cancel;
	}
	public class CBeforeExtractEventArgs : EventArgs
	{
		public string FolderNameInZip;
		public string FolderName;
		public string FileName;

		public long TotalCountToExtract;
		public long CountExtracted;
		public int ProgressPercentage;

		public string NewFolderNameIs;
	}
	public class CAfterExtractEventArgs : EventArgs
	{
		public string FullPath;
		public string FileName;
	}

	public class CSharpZipHelper
	{
		public event EventHandler<CBeforeCompressEventArgs> BeforeCompress;
		public event EventHandler<CBeforeExtractEventArgs> BeforeExtract;
		public event EventHandler<CAfterExtractEventArgs> AfterExtract;

		public CSharpZipHelper()
		{
		}

		/// <summary>
		/// SharpZipLib를 이용해 압축함. 간단 버전은 SharpZipLib의 FastZip 클래스 이용하면 되나 BeforeCompress 이벤트를 사용하기 위함.
		/// </summary>
		/// <param name="SearchPattern">
		/// The search string. For example, "System*" can be used to search for all directories that begin with the word "System".
		/// </param>
		public void CreateZip(string ZipFullPath, string SearchPattern, SearchOption SearchOption, string[] aSourceFolder, string Password)
		{
			using (ZipOutputStream OutputStream = new ZipOutputStream(File.Create(ZipFullPath)))
			{
				OutputStream.SetLevel(6); // 0 - store only to 9 - means best compression

				if (!string.IsNullOrEmpty(Password))
					OutputStream.Password = Password;

				foreach (string SourceFolder in aSourceFolder)
				{
					DirectoryInfo di = new DirectoryInfo(SourceFolder);
					FileInfo[] aFiles = di.GetFiles(SearchPattern, SearchOption);
					ZipNameTransform NameTransform = new ZipNameTransform(SourceFolder);

					foreach (FileInfo file in aFiles)
					{
						string FullPathSrc = file.FullName;

						string FolderNameInZip = NameTransform.TransformFile(file.FullName);

						if (this.BeforeCompress != null)
						{
							CBeforeCompressEventArgs e = new CBeforeCompressEventArgs()
							{
								FullPathSrc = FullPathSrc,
								FolderNameInZip = FolderNameInZip
							};
							this.BeforeCompress(this, e);

							if (e.Cancel)
								continue;

							if (!string.IsNullOrEmpty(e.NewFolderNameInZipIs))
								FolderNameInZip = e.NewFolderNameInZipIs;
						}

						ZipEntry entry = new ZipEntry(FolderNameInZip);

						entry.DateTime = file.LastWriteTime;

						// set Size and the crc, because the information
						// about the size and crc should be stored in the header
						// if it is not set it is automatically written in the footer.
						// (in this case size == crc == -1 in the header)
						// Some ZIP programs have problems with zip files that don't store
						// the size and crc in the header.
						entry.Size = file.Length;

						OutputStream.PutNextEntry(entry);

						byte[] buffer = new byte[4096];
						using (FileStream streamReader = File.OpenRead(FullPathSrc))
						{
							StreamUtils.Copy(streamReader, OutputStream, buffer);
						}
						OutputStream.CloseEntry();
					}
				}

				OutputStream.Finish();
				OutputStream.Close();
			}
		}
		//public void CreateZip(string ZipFullPath, string SearchPattern, SearchOption SearchOption, string[] aSourceFolder, string Password)
		//{
		//    Crc32 crc = new Crc32();
		//    using (ZipOutputStream OutputStream = new ZipOutputStream(File.Create(ZipFullPath)))
		//    {
		//        OutputStream.SetLevel(6); // 0 - store only to 9 - means best compression

		//        if (!string.IsNullOrEmpty(Password))
		//            OutputStream.Password = Password;

		//        foreach (string SourceFolder in aSourceFolder)
		//        {
		//            DirectoryInfo di = new DirectoryInfo(SourceFolder);
		//            FileInfo[] aFiles = di.GetFiles(SearchPattern, SearchOption);
		//            ZipNameTransform NameTransform = new ZipNameTransform(SourceFolder);

		//            foreach (FileInfo file in aFiles)
		//            {
		//                string FullPathSrc = file.FullName;

		//                string FolderNameInZip = NameTransform.TransformFile(file.FullName);

		//                if (this.BeforeCompress != null)
		//                {
		//                    CBeforeCompressEventArgs e = new CBeforeCompressEventArgs()
		//                    {
		//                        FullPathSrc = FullPathSrc,
		//                        FolderNameInZip = FolderNameInZip
		//                    };
		//                    this.BeforeCompress(this, e);

		//                    if (e.Cancel)
		//                        continue;

		//                    if (!string.IsNullOrEmpty(e.NewFolderNameInZipIs))
		//                        FolderNameInZip = e.NewFolderNameInZipIs;
		//                }

		//                FileStream fs = File.OpenRead(file.FullName);

		//                byte[] buffer = new byte[fs.Length];
		//                fs.Read(buffer, 0, buffer.Length);

		//                ZipEntry entry = new ZipEntry(FolderNameInZip);

		//                entry.DateTime = file.LastWriteTime;

		//                // set Size and the crc, because the information
		//                // about the size and crc should be stored in the header
		//                // if it is not set it is automatically written in the footer.
		//                // (in this case size == crc == -1 in the header)
		//                // Some ZIP programs have problems with zip files that don't store
		//                // the size and crc in the header.
		//                entry.Size = fs.Length;
		//                fs.Close();

		//                crc.Reset();
		//                crc.Update(buffer);

		//                entry.Crc = crc.Value;

		//                OutputStream.PutNextEntry(entry);

		//                OutputStream.Write(buffer, 0, buffer.Length);
		//            }
		//        }

		//        OutputStream.Finish();
		//        OutputStream.Close();
		//    }
		//}
		/// <param name="SearchPattern">
		/// The search string. For example, "System*" can be used to search for all directories that begin with the word "System".
		/// </param>
		public void CreateZip(string ZipFullPath, string SearchPattern, SearchOption SearchOption, string[] aSourceFolder)
		{
			CreateZip(ZipFullPath, SearchPattern, SearchOption, aSourceFolder, string.Empty);
		}
		/// <param name="SearchPattern">
		/// The search string. For example, "System*" can be used to search for all directories that begin with the word "System".
		/// </param>
		public void CreateZip(string ZipFullPath, string SearchPattern, SearchOption SearchOption, string SourceFolder)
		{
			CreateZip(ZipFullPath, SearchPattern, SearchOption, new string[] { SourceFolder }, string.Empty);
		}

		//public void ExtractZipOld(string ZipFullPath, string TargetDirectory)
		//{
		//    using (ZipInputStream ZipStream = new ZipInputStream(File.OpenRead(ZipFullPath)))
		//    {
		//        ZipEntry Entry;
		//        while ((Entry = ZipStream.GetNextEntry()) != null)
		//        {
		//            string DirectoryName = Path.GetDirectoryName(Entry.Name);
		//            string FileName = Path.GetFileName(Entry.Name);

		//            if (FileName == String.Empty)
		//                continue;

		//            string FolderName = Path.Combine(TargetDirectory, DirectoryName);
		//            if (this.BeforeExtract != null)
		//            {
		//                CBeforeExtractEventArgs e = new CBeforeExtractEventArgs()
		//                {
		//                    FolderNameInZip = DirectoryName,
		//                    FolderName = FolderName,
		//                    FileName = FileName
		//                };
		//                this.BeforeExtract(this, e);
		//                if (!string.IsNullOrEmpty(e.NewFolderNameIs))
		//                    FolderName = e.NewFolderNameIs;
		//            }

		//            if (!Directory.Exists(FolderName))
		//                Directory.CreateDirectory(FolderName);

		//            string FullPathDest = Path.Combine(FolderName, FileName);
		//            using (FileStream fs = File.Create(FullPathDest))
		//            {
		//                int Size = 2048;
		//                byte[] aByte = new byte[2048];
		//                while (true)
		//                {
		//                    Size = ZipStream.Read(aByte, 0, aByte.Length);
		//                    if (Size == 0)
		//                        break;

		//                    fs.Write(aByte, 0, Size);
		//                }
		//            }

		//            FileInfo fi = new FileInfo(FullPathDest);
		//            fi.LastWriteTime = Entry.DateTime;
		//        }
		//    }
		//}

		public static bool ContainsAllFile(string ZipFullPath, string TargetDirectory, string Password, bool IsCheckSize, bool IsCheckDate)
		{
			ZipFile zf = null;
			try
			{
				FileStream fs = File.OpenRead(ZipFullPath);
				zf = new ZipFile(fs);
				if (!string.IsNullOrEmpty(Password))
				{
					zf.Password = Password;		// AES encrypted entries are handled automatically
				}
				
				foreach (ZipEntry Entry in zf)
				{
					if (!Entry.IsFile)
						continue;			// Ignore directories

					string DirectoryName = Path.GetDirectoryName(Entry.Name);
					string FileName = Path.GetFileName(Entry.Name);

					string FolderName = Path.Combine(TargetDirectory, DirectoryName);
					if (!Directory.Exists(FolderName))
						return false;

					string FullPathDest = Path.Combine(FolderName, FileName);
					FileInfo fi = new FileInfo(FullPathDest);
					if (!fi.Exists)
						return false;

					if (IsCheckSize)
					{
						if (Entry.Size != fi.Length)
							return false;
					}

					if (IsCheckDate)
					{
						if (Entry.DateTime != fi.LastWriteTime)
							return false;
					}
				}
			}
			finally
			{
				if (zf != null)
				{
					zf.IsStreamOwner = true; // Makes close also shut the underlying stream
					zf.Close(); // Ensure we release resources
				}
			}

			return true;
		}

		public void ExtractZip(string ZipFullPath, string TargetDirectory, string Password)
		{
			ZipFile zf = null;
			try
			{
				FileStream fs = File.OpenRead(ZipFullPath);
				zf = new ZipFile(fs);
				if (!string.IsNullOrEmpty(Password))
				{
					zf.Password = Password;		// AES encrypted entries are handled automatically
				}

				long TotalCount = zf.Count;
				
				foreach (ZipEntry Entry in zf)
				{
					if (!Entry.IsFile)
						continue;			// Ignore directories

					string DirectoryName = Path.GetDirectoryName(Entry.Name);
					string FileName = Path.GetFileName(Entry.Name);

					string FolderName = Path.Combine(TargetDirectory, DirectoryName);
					if (this.BeforeExtract != null)
					{
						CBeforeExtractEventArgs e = new CBeforeExtractEventArgs()
						{
							FolderNameInZip = DirectoryName,
							FolderName = FolderName,
							FileName = FileName,

							TotalCountToExtract = TotalCount,
							CountExtracted = Entry.ZipFileIndex + 1,
							ProgressPercentage = Math.Min(100, Convert.ToInt32(((Entry.ZipFileIndex + 1) / (double)TotalCount) * 100))
						};
						this.BeforeExtract(this, e);
						if (!string.IsNullOrEmpty(e.NewFolderNameIs))
							FolderName = e.NewFolderNameIs;
					}

					byte[] buffer = new byte[4096];		// 4K is optimum
					Stream zipStream = zf.GetInputStream(Entry);

					if (!Directory.Exists(FolderName))
						Directory.CreateDirectory(FolderName);

					string FullPathDest = Path.Combine(FolderName, FileName);

					// Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
					// of the file, but does not waste memory.
					// The "using" will close the stream even if an exception occurs.
					using (FileStream streamWriter = File.Create(FullPathDest))
					{
						StreamUtils.Copy(zipStream, streamWriter, buffer);
					}

					FileInfo fi = new FileInfo(FullPathDest);
					fi.LastWriteTime = Entry.DateTime;

					if (this.AfterExtract != null)
					{
						CAfterExtractEventArgs e = new CAfterExtractEventArgs()
						{
							FullPath = FullPathDest,
							FileName = FileName
						};
						this.AfterExtract(this, e);
					}
				}
			}
			finally
			{
				if (zf != null)
				{
					zf.IsStreamOwner = true; // Makes close also shut the underlying stream
					zf.Close(); // Ensure we release resources
				}
			}
		}
		public void ExtractZip(string ZipFullPath, string TargetDirectory)
		{
			string Password = "";
			ExtractZip(ZipFullPath, TargetDirectory, Password);
		}

		public List<MemoryStream> ExtractZipToStream(string ZipFullPath, string SearchPattern, string Password)
		{
			List<MemoryStream> aZipStream = new List<MemoryStream>();

			ZipFile zf = null;
			try
			{
				FileStream fs = File.OpenRead(ZipFullPath);
				zf = new ZipFile(fs);
				if (!string.IsNullOrEmpty(Password))
				{
					zf.Password = Password;		// AES encrypted entries are handled automatically
				}

				long TotalCount = zf.Count;

				foreach (ZipEntry Entry in zf)
				{
					if (!Entry.IsFile)
						continue;			// Ignore directories

					if (!CLang.Like(Entry.Name, SearchPattern, true))
						continue;

					string DirectoryName = Path.GetDirectoryName(Entry.Name);
					string FileName = Path.GetFileName(Entry.Name);

					byte[] buffer = new byte[4096];		// 4K is optimum
					Stream zipStream = zf.GetInputStream(Entry);
					MemoryStream ms = CFile.GetMemoryStreamFromStream(zipStream);
					aZipStream.Add(ms);
				}
			}
			finally
			{
				if (zf != null)
				{
					zf.IsStreamOwner = true; // Makes close also shut the underlying stream
					zf.Close(); // Ensure we release resources
				}
			}

			return aZipStream;
		}
	}
}
