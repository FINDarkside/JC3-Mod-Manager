using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml;

namespace Just_Cause_3_Mod_Manager
{

	public static class ModManager
	{
		public static ObservableCollection<Mod> Mods { get; set; }
		public static ObservableCollection<Category> Categories { get; set; }

		public static void Init()
		{
			Categories = new ObservableCollection<Category>();
			Categories.Add(new Category("ALL") { Selected = true });

			Mods = new ObservableCollection<Mod>();

			Mods.CollectionChanged += (o, e) =>
			{
				Categories[0].ModCount += e.NewItems != null ? e.NewItems.Count : 0;
				Categories[0].ModCount -= e.OldItems != null ? e.OldItems.Count : 0;
			};


			var modsJson = Path.Combine(Settings.files, "mods.json");
			if (File.Exists(modsJson))
			{
				try
				{
					var mods = JsonConvert.DeserializeObject<ObservableCollection<Mod>>(File.ReadAllText(modsJson), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
					
					foreach (var m in mods)
					{
						if (m.Category != null)
						{
							var expanded = m.Category.Selected;
							m.Category = GetCategory(m.Category.Name);
							m.Category.Selected = expanded;
						}

						AddMod(m);
					}
				}
				catch (Exception e)
				{
					Errors.Handle("Failed to restore mods", e);
				}
			}

		}

		public static void AddMod(Mod mod)
		{

			mod.PropertyChanged += (Object s, PropertyChangedEventArgs args) =>
			{
				if (args.PropertyName == "Category")
				{
					for (var i = Categories.Count - 1; i >= 1; i--)
					{
						if (Categories[i].ModCount <= 0)
							Categories.RemoveAt(i);
					}
				}
			};

			mod.CheckForUpdates();

			Mods.Add(mod);
		}

		public static async Task AddMod(string[] files)
		{
			Mod mod = null;
			await Task.Run(() =>
			{
				var tempFolder = TempFolder.GetTempFile();
				tempFolder = Directory.CreateDirectory(tempFolder).FullName;
				foreach (var file in files)
				{
					if (File.GetAttributes(file).HasFlag(FileAttributes.Directory))
					{
						var destination = Path.Combine(tempFolder, Path.GetFileName(file));
						Util.CopyDirectory(file, destination);
					}
					else
					{
						File.Copy(file, Path.Combine(tempFolder, Path.GetFileName(file)));
					}
				}
				Util.ExtractAllFilesInDirectory(tempFolder);

				var modFiles = new Dictionary<string, string>();
				string modInfo = null;
				foreach (var file in Directory.EnumerateFiles(tempFolder, "*", SearchOption.AllDirectories))
				{
					System.Diagnostics.Debug.WriteLine(Path.GetFileName(file));
					if (GameFiles.IsGameFile(file))
					{
						System.Diagnostics.Debug.WriteLine("FOUND");
						modFiles[Path.GetFileName(file)] = file;
					}
					else if (Path.GetFileName(file).Equals("Mod.xml", StringComparison.InvariantCultureIgnoreCase))
					{
						modInfo = file;
					}
				}

				mod = new Mod();

				do
				{
					mod.Id = Guid.NewGuid().ToString("N");
				} while (Directory.Exists(mod.Folder) || File.Exists(mod.Folder));

				
				if (modInfo != null)
				{
					XmlDocument doc = new XmlDocument();
					doc.Load(modInfo);
					var docElem = doc.DocumentElement;
					var author = docElem.GetElementsByTagName("Author");
					if (author.Count > 0)
						mod.Info.Author = author[0].InnerText;
					var version = docElem.GetElementsByTagName("Version");
					if (version.Count > 0)
						mod.Info.Version = int.Parse(version[0].InnerText);
					var modPage = docElem.GetElementsByTagName("ModPage");
					if (modPage.Count > 0)
						mod.Info.ModPage = modPage[0].InnerText;
					var name = docElem.GetElementsByTagName("Name");
					if (name.Count > 0)
						mod.Info.DefaultName = name[0].InnerText;
					var settings = docElem.GetElementsByTagName("Settings");
					if (settings.Count > 0)
					{
						mod.Info.Settings = new ModSettingsViewModel((XmlElement)settings[0]);
					}
				}

				mod.Name = mod.Info.DefaultName;

				Directory.CreateDirectory(mod.Folder);
				foreach (var file in modFiles.Values)
				{
					File.Move(file, Path.Combine(mod.Folder, Path.GetFileName(file)));
				}
			});

			if (mod.Name == null && files.Length == 1 && File.Exists(files[0]) && !GameFiles.IsGameFile(files[0]))//Probably archive lol
				mod.Name = Path.GetFileNameWithoutExtension(files[0]).Replace("-", " ").Replace("_", " ");

			AddMod(mod);
		}

		public static Category GetCategory(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return null;
			var cats = Categories.Where(cat => cat.Name.Equals(name)).ToArray();
			if (cats.Length > 0)
				return cats[0];
			var category = new Category(name);
			Categories.Add(category);
			return category;
		}

		public static void Save()
		{
			var path = Path.Combine(Settings.files, "mods.json");
			File.WriteAllText(path, JsonConvert.SerializeObject(Mods, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
		}

	}


}
