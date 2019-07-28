using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public static class ModInstaller
	{

		public static void InstallMods(IList<Mod> mods)
		{
			var dropzoneFiles = new List<string>();
			var skyfortressFiles = new List<string>();

			foreach (var mod in mods)
			{
				if (!mod.Active)
					continue;

				if (Directory.Exists(Path.Combine(mod.Folder, "dropzone")))
					dropzoneFiles.AddRange(Directory.GetFiles(Path.Combine(mod.Folder, "dropzone"), "*", SearchOption.AllDirectories));
				if (Directory.Exists(Path.Combine(mod.Folder, "dropzone_sky_fortress")))
					skyfortressFiles.AddRange(Directory.GetFiles(Path.Combine(mod.Folder, "dropzone_sky_fortress"), "*", SearchOption.AllDirectories));
			}

			if (dropzoneFiles.Count > 0 || skyfortressFiles.Count > 0)
				InstallMods(dropzoneFiles, skyfortressFiles);
		}

		private static Dictionary<string, List<string>> GroupFiles(List<string> files)
		{
			var grouped = new Dictionary<string, List<string>>();
			foreach (var file in files)
			{
				var name = Path.GetFileName(file);
				if (grouped.ContainsKey(name))
					grouped[name].Add(file);
				else
					grouped.Add(name, new List<string> { file });
			}
			return grouped;
		}

		public static async void InstallMods(List<string> dropzoneFiles, List<string> skyfortressFiles)
		{
			var groupedDropzoneFiles = GroupFiles(dropzoneFiles);
			var groupedSkyfortressFiles = GroupFiles(skyfortressFiles);

			var progressViewModel = new ProgressDialogViewModel();
			Settings.mainWindow.BusyDialog.DialogContent = progressViewModel;
			Settings.mainWindow.BusyDialog.IsOpen = true;
			int progress = 0;

			foreach (var value in groupedDropzoneFiles.Values)
			{
				var files = value;
				await Task.Run(() =>
				{
					if (files.Count == 1)
					{
						var file = GameFiles.GetDefaultFiles(Path.GetFileName(files[0]));
						var outputPath = Path.Combine(Settings.user.JC3Folder, "dropzone", file[0].Substring(Settings.defaultFiles.Length + 1));
						var name = Path.GetFileNameWithoutExtension(outputPath);
						outputPath = Path.GetDirectoryName(outputPath) + "\\" + name.Substring(0, name.LastIndexOf('_')) + Path.GetExtension(outputPath);
						File.Copy(files[files.Count - 1], outputPath, true);
					}
					else
					{
						try
						{
							Combiner.Combine(files, false, "dropzone");
						}
						catch (Exception ex)
						{
							Errors.Handle("Failed to combine " + files.Count + " mods.", ex);
							var file = GameFiles.GetDefaultFiles(Path.GetFileName(files[0]));
							var outputPath = Path.Combine(Settings.user.JC3Folder, "dropzone", file[0].Substring(Settings.defaultFiles.Length + 1));
							var name = Path.GetFileNameWithoutExtension(outputPath);
							outputPath = Path.GetDirectoryName(outputPath) + "\\" + name.Substring(0, name.LastIndexOf('_')) + Path.GetExtension(outputPath);
							File.Copy(files[files.Count - 1], outputPath, true);
						}
					}
				});
				progress++;
				progressViewModel.Progress = progress / (groupedDropzoneFiles.Count + groupedSkyfortressFiles.Count);
			}

			foreach (var value in groupedSkyfortressFiles.Values)
			{
				var files = value;
				await Task.Run(() =>
				{
					if (files.Count == 1)
					{
						var file = GameFiles.GetDefaultFiles(Path.GetFileName(files[0]));
						var outputPath = Path.Combine(Settings.user.JC3Folder, "dropzone_sky_fortress", file[0].Substring(Settings.defaultFiles.Length + 1));
						var name = Path.GetFileNameWithoutExtension(outputPath);
						outputPath = Path.GetDirectoryName(outputPath) + "\\" + name.Substring(0, name.LastIndexOf('_')) + Path.GetExtension(outputPath);
						File.Copy(files[files.Count - 1], outputPath, true);
					}
					else
					{
						try
						{
							Combiner.Combine(files, false, "dropzone_sky_fortress");
						}
						catch (Exception ex)
						{
							Errors.Handle("Failed to combine " + files.Count + " mods.", ex);
							var file = GameFiles.GetDefaultFiles(Path.GetFileName(files[0]));
							var outputPath = Path.Combine(Settings.user.JC3Folder, "dropzone_sky_fortress", file[0].Substring(Settings.defaultFiles.Length + 1));
							var name = Path.GetFileNameWithoutExtension(outputPath);
							outputPath = Path.GetDirectoryName(outputPath) + "\\" + name.Substring(0, name.LastIndexOf('_')) + Path.GetExtension(outputPath);
							File.Copy(files[files.Count - 1], outputPath, true);
						}
					}

				});
				progress++;
				progressViewModel.Progress = progress / (groupedDropzoneFiles.Count + groupedSkyfortressFiles.Count);
			}

			if (groupedSkyfortressFiles.Count > 0)
				GibbedsTools.SkyFortressPack();

			Settings.mainWindow.BusyDialog.IsOpen = false;

		}

	}
}
