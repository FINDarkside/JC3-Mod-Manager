using SharpCompress.Archive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public static class Util
	{
		public static string ComputeSHA256(string file)
		{
			using (var stream = new BufferedStream(File.OpenRead(file), 1200000))
			{
				SHA256Managed sha = new SHA256Managed();
				byte[] checksum = sha.ComputeHash(stream);
				return BitConverter.ToString(checksum).Replace("-", String.Empty);
			}
		}

		public static string GetUniqueFile(string preferredFile)
		{
			int num = 1;
			string result = null;
			do
			{
				var ext = Path.GetExtension(preferredFile);
				result = Path.Combine(Path.GetDirectoryName(preferredFile), Path.GetFileNameWithoutExtension(preferredFile) + "_" + num + ext);
			} while (File.Exists(result));
			return result;
		}

		public static void CopyDirectory(string path, string destination)
		{
			Directory.CreateDirectory(destination);
			foreach (string dirPath in Directory.EnumerateDirectories(path, "*", SearchOption.AllDirectories))
				Directory.CreateDirectory(dirPath.Replace(path, destination));
			foreach (string filePath in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
				File.Copy(filePath, filePath.Replace(path, destination), true);
		}

		public static void ExtractAllFilesInDirectory(string folder)
		{
				//TODO OPTIMIZE
				var filesExtracted = false;
				do
				{
					filesExtracted = false;
					foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
					{
						if (file.EndsWith(".zip") || file.EndsWith(".rar") || file.EndsWith(".7z") || file.EndsWith(".gz") || file.EndsWith(".tar") || file.EndsWith(".bz2"))
						{
							filesExtracted = true;
							var outputPath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
							Directory.CreateDirectory(outputPath);
							ArchiveFactory.WriteToDirectory(file, outputPath, SharpCompress.Common.ExtractOptions.ExtractFullPath);
							File.Delete(file);
						}
					}
				} while (filesExtracted);
		}
	}
}
