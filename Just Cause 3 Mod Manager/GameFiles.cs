using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public static class GameFiles
	{
		public class DefaultFileInformation
		{
			public string relativePath;
			public string hash;
			public int size;
			public string tabFile;

			public DefaultFileInformation(string line, string tabFile)
			{
				string[] splitLine = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				size = Convert.ToInt32(splitLine[2], 16);
				hash = splitLine[0].Substring(2);
				relativePath = splitLine[4];
				this.tabFile = tabFile;
			}
		}

		private static HashSet<string> gameFiles = new HashSet<string>();

		public static bool IsGameFile(string file)
		{
			return gameFiles.Contains(Path.GetFileName(file));
		}

		public static Task LoadFileNamesAsync(){
			return Task.Run(()=>{
				var fileLists = new List<string>();
				fileLists.AddRange(Directory.EnumerateFiles(Path.Combine(Settings.user.JC3Folder, "archives_win64"), "*", SearchOption.AllDirectories).Where(name => Regex.IsMatch(name, "game_hash_names[0-9]+\\.txt")));
				if (Directory.Exists(Path.Combine(Settings.user.JC3Folder, "dlc")))
					fileLists.AddRange(Directory.EnumerateFiles(Path.Combine(Settings.user.JC3Folder, "dlc"), "*", SearchOption.AllDirectories).Where(name => Regex.IsMatch(name, "game_hash_names[0-9]+\\.txt")).ToList<string>());
				if (Directory.Exists(Path.Combine(Settings.user.JC3Folder, "patch_win64")))
					fileLists.AddRange(Directory.EnumerateFiles(Path.Combine(Settings.user.JC3Folder, "patch_win64")).Where(name => Regex.IsMatch(name, "game_hash_names[0-9]+\\.txt")).ToList<string>());

				foreach (var fileList in fileLists)
				{
					var lines = File.ReadAllLines(fileList);
					for (var i = 2; i < lines.Length; i++)
					{
						var line = lines[i];
						if (line != null && line.Contains("/"))
						{
							gameFiles.Add(line.Substring(line.LastIndexOf("/") + 1));
						}
					}
				}
			});
		}



		public static List<string> GetDefaultFiles(string fileName)
		{
			var result = new List<string>();
			var cachedFiles = Directory.EnumerateFiles(Settings.defaultFiles, Path.GetFileNameWithoutExtension(fileName) + "_*" + Path.GetExtension(fileName), SearchOption.AllDirectories);
			foreach (var file in cachedFiles)
			{
				result.Add(file);
			}
			//Find file from jc3 folders

			var fileLists = new List<string>();
			fileLists.AddRange(Directory.EnumerateFiles(Path.Combine(Settings.user.JC3Folder, "archives_win64"), "*", SearchOption.AllDirectories).Where(name => Regex.IsMatch(name, "game_hash_names[0-9]+\\.txt")));
			fileLists = fileLists.OrderBy(s => int.Parse(Path.GetFileNameWithoutExtension(s).Substring(15))).ToList<string>();
			if (Directory.Exists(Path.Combine(Settings.user.JC3Folder, "dlc")))
			{
				var dlcFileLists = Directory.EnumerateFiles(Path.Combine(Settings.user.JC3Folder, "dlc"), "*", SearchOption.AllDirectories).Where(name => Regex.IsMatch(name, "game_hash_names[0-9]+\\.txt")).ToList<string>();
				dlcFileLists = dlcFileLists.OrderBy(s => int.Parse(Path.GetFileNameWithoutExtension(s).Substring(15))).ToList<string>();
				fileLists.AddRange(dlcFileLists);
			}
			if (Directory.Exists(Path.Combine(Settings.user.JC3Folder, "patch_win64")))
			{
				var patchFileLists = Directory.EnumerateFiles(Path.Combine(Settings.user.JC3Folder, "patch_win64")).Where(name => Regex.IsMatch(name, "game_hash_names[0-9]+\\.txt")).ToList<string>();
				patchFileLists = patchFileLists.OrderBy(s => int.Parse(Path.GetFileNameWithoutExtension(s).Substring(15))).ToList<string>();
				fileLists.AddRange(patchFileLists);
			}


			var fileInfos = new List<DefaultFileInformation>();
			foreach (string fileList in fileLists)
			{
				string[] lines = File.ReadAllLines(fileList);
				foreach (string line in lines)
				{
					if (line.Contains(fileName))
					{

						string num = Path.GetFileName(fileList).Substring(15, Path.GetFileName(fileList).Length - 15 - 4);
						string tabFile = Path.Combine(Path.GetDirectoryName(fileList), "game" + num + ".tab");
						fileInfos.Add(new DefaultFileInformation(line, tabFile));
					}
				}
			}

			if (result.Count == fileInfos.Count)
			{
				return result;
			}

			foreach (string file in result)
			{
				File.Delete(file);
			}

			foreach (var fileInfo in fileInfos)
			{
				var outputPath = TempFolder.GetTempFile();
				string extractedFolder = GibbedsTools.Unpack(fileInfo.tabFile, outputPath, fileInfo.hash + "\\.*");

				if (extractedFolder == null)
					continue;

				var files = Directory.EnumerateFiles(extractedFolder, fileInfo.hash + ".*", SearchOption.AllDirectories);
				foreach (string file in files)
				{
					if (new FileInfo(file).Length != fileInfo.size)
						continue;
					string newPath = Path.Combine(Settings.defaultFiles, fileInfo.relativePath);
					newPath = Util.GetUniqueFile(newPath);

					if (!Directory.Exists(Path.GetDirectoryName(newPath)))
						Directory.CreateDirectory(Path.GetDirectoryName(newPath));
					File.Move(file, newPath);
					if (File.Exists(newPath))
						result.Add(newPath);
					break;
				}
			}
			return result;
		}
	}
}
