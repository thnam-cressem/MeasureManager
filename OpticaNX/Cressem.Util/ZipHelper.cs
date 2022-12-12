using System;
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

namespace Cressem.Util
{
	/// <summary>
	/// https://github.com/icsharpcode/SharpZipLib
	/// https://github.com/icsharpcode/SharpZipLib/wiki/Zip-Samples#anchorUnpackFull
	/// </summary>
	public class ZipHelper
	{
		/// <summary>
		/// Method for decompressing a compressed archive
		/// </summary>
		/// <param name="zipFilePath">the archive file we're decompressing</param>
		/// <param name="destPath">directory we want it unzipped to</param>
		/// <param name="deleteOriginal">delete or keep the original zip file</param>
		/// <returns></returns>
		public static bool Unzip(string zipFilePath, string destPath, bool deleteOriginal = false)
		{
			if (File.Exists(zipFilePath) == false)
			{
				return false;
			}

			using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath)))
			{
				ZipEntry entry;
				while ((entry = zipStream.GetNextEntry()) != null)
				{
					string directoryName = Path.GetDirectoryName(entry.Name);
					string fileName = Path.GetFileName(entry.Name);

					// Create directory
					//if (String.IsNullOrEmpty(directoryName) == false)
					Directory.CreateDirectory(Path.Combine(destPath, directoryName));

					if (String.IsNullOrEmpty(fileName) == false)
					{
						using (FileStream writer = File.Create(Path.Combine(destPath, entry.Name)))
						{
							int size = 2048;
							byte[] data = new byte[size];
							while (true)
							{
								size = zipStream.Read(data, 0, data.Length);
								if (size > 0)
									writer.Write(data, 0, size);
								else
									break;
							}
						}
					}
				}
			}

			if (deleteOriginal == true)
				File.Delete(zipFilePath);

			return true;
		}

		/// <summary>
		/// Method for decompressing a compressed archive
		/// </summary>
		/// <param name="zipFilePath">the archive file we're decompressing</param>
		/// <param name="destPath">directory we want it unzipped to</param>
		/// <param name="password">archive password (if one exists)</param>
		/// <param name="deleteOriginal">delete or keep the original zip file</param>
		/// <returns></returns>
		public bool Unzip(string zipFilePath, string destPath, string password, bool deleteOriginal = false)
		{
			// Open a new ZipInputStream
			using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath)))
			{
				// Check for a password value, if none provided then set it to null (no password)
				if (String.IsNullOrEmpty(password) == false)
					zipStream.Password = password;

				// Create a ZipEntry
				ZipEntry entry = null;
				string tempEntry = string.Empty;

				// Loop through the zip file grabbing each ZipEntry one at a time
				while ((entry = zipStream.GetNextEntry()) != null)
				{
					string fileName = Path.GetFileName(entry.Name);

					// Create the directory
					if (String.IsNullOrEmpty(destPath) == false)
						Directory.CreateDirectory(destPath);

					// Make sure we have a file name and go from there
					if (String.IsNullOrEmpty(fileName))
						continue;

					if (entry.Name.IndexOf(".ini") >= 0)
						continue;

					string path = destPath + @"\" + entry.Name;
					path = path.Replace("\\ ", "\\");
					string dirPath = Path.GetDirectoryName(path);

					if (Directory.Exists(dirPath) == false)
						Directory.CreateDirectory(dirPath);

					using (FileStream stream = File.Create(path))
					{
						int size = 2048;
						byte[] data = new byte[2048];
						byte[] buffer = new byte[size];

						while (true)
						{
							size = zipStream.Read(buffer, 0, buffer.Length);
							if (size > 0)
								// Write data to new file and go back and grab the next entry.
								// Loop until all files have been decompressed and are in the proper directory
								stream.Write(buffer, 0, size);
							else
								break;
						}
					}

				}
			}

			// One last thing to do, if the user provided true then we need to delete the original zip file, otherwise we're now done.
			if (deleteOriginal == true)
				File.Delete(zipFilePath);

			return true;
		}

		/// <summary>
		/// Method for compressing a folder as a compressed archive
		/// </summary>
		/// <param name="outputZipFilePath">the archive file to compress</param>
		/// <param name="folderName">destination folder to decompress the archive</param>
		/// <returns></returns>
		public static bool ZipDirectory(string outputZipFilePath, string folderName)
		{
			return ZipDirectory(outputZipFilePath, null, folderName);
		}

		/// <summary>
		/// Method for compressing a folder as a compressed archive
		/// </summary>
		/// <param name="outputZipFilePath">the archive file to compress</param>
		/// <param name="folderName">destination folder to decompress the archive</param>
		/// <param name="compressionLevel">level of compression (default = 6)</param>
		/// <returns></returns>
		public static bool ZipDirectory(string outputZipFilePath, string folderName, int compressionLevel = 6)
		{
			return ZipDirectory(outputZipFilePath, null, folderName, compressionLevel);
		}

		/// <summary>
		/// Method for compressing a folder as a compressed archive
		/// </summary>
		/// <param name="outputZipFilePath">the archive file to compress</param>
		/// <param name="password">archive password (if neccessory)</param>
		/// <param name="folderName">destination folder to decompress the archive</param>
		/// <param name="compressionLevel">level of compression (default = 6)</param>
		/// <returns></returns>
		public static bool ZipDirectory(string outputZipFilePath, string password, string folderName, int compressionLevel = 6)
		{
			if (Directory.Exists(folderName) == false)
				return false;

			using (ZipOutputStream zipStream = new ZipOutputStream(File.Create(outputZipFilePath)))
			{
				zipStream.SetLevel(compressionLevel); // 0-9, 9 being the highest level of compression

				if (String.IsNullOrEmpty(password) == false)
					zipStream.Password = password;  // optional. Null is the same as not setting.

				// This setting will strip the leading part of the folder path in the entries, to
				// make the entries relative to the starting folder.
				// To include the full path for each entry up to the drive root, assign folderOffset = 0.
				int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

				CompressFolder(folderName, zipStream, folderOffset);

				zipStream.IsStreamOwner = true;

				// Finish/Close arent needed strictly as the using statement does this automatically

				// Finish is important to ensure trailing information for a Zip file is appended.  Without this
				// the created file would be invalid.
				zipStream.Finish();
				// Makes the Close also Close the underlying stream
				zipStream.Close();
			}

			return true;
		}

		private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
		{
			string[] files = Directory.GetFiles(path);

			foreach (string filename in files)
			{
				FileInfo fi = new FileInfo(filename);

				string entryName = filename.Substring(folderOffset);
				// Makes the name in zip based on the folder
				entryName = ZipEntry.CleanName(entryName);
				// Removes drive from name and fixes slash direction
				ZipEntry newEntry = new ZipEntry(entryName);
				newEntry.DateTime = fi.LastWriteTime;
				// Note the zip format stores 2 second granularity

				// Specifying the AESKeySize triggers AES encryption. Allowable values are 0 (off), 128 or 256.
				//   newEntry.AESKeySize = 256;

				// To permit the zip to be unpacked by built-in extractor in WinXP and Server2003, WinZip 8, Java, and other older code,
				// you need to do one of the following: Specify UseZip64.Off, or set the Size.
				// If the file may be bigger than 4GB, or you do not need WinXP built-in compatibility, you do not need either,
				// but the zip will be in Zip64 format which not all utilities can understand.
				//   zipStream.UseZip64 = UseZip64.Off;
				newEntry.Size = fi.Length;

				zipStream.PutNextEntry(newEntry);

				// Zip the file in buffered chunks
				// the "using" will close the stream even if an exception occurs
				byte[] buffer = new byte[4096];
				using (FileStream streamReader = File.OpenRead(filename))
				{
					StreamUtils.Copy(streamReader, zipStream, buffer);
				}
				zipStream.CloseEntry();
			}

			string[] folders = Directory.GetDirectories(path);
			foreach (string folder in folders)
			{
				CompressFolder(folder, zipStream, folderOffset);
			}
		}
	}
}
