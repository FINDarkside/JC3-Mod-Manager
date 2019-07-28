using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Just_Cause_3_Mod_Manager
{

	public class ModManager : INotifyPropertyChanged
	{
		public static ModManager Instance { get; set; }

		public ObservableCollection<Mod> Mods { get; set; }
		public ObservableCollection<Category> Categories { get; set; }
		private bool allCategoriesSelected;
		public bool AllCategoriesSelected
		{
			get { return allCategoriesSelected; }
			set
			{
				if (value == true)
				{
					foreach (var cat in Categories)
						cat.Selected = false;
				}
				SetPropertyField(ref allCategoriesSelected, value);
			}
		}

		static ModManager()
		{
			Instance = new ModManager();
		}

		private ModManager()
		{
		}

		public void Init()
		{
			Categories = new ObservableCollection<Category>();
			AllCategoriesSelected = true;

			Mods = new ObservableCollection<Mod>();

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

		public void AddMod(Mod mod)
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

		public async Task<Mod> AddMod(string[] files)
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

				string modInfo = Directory.EnumerateFiles(tempFolder, "*", SearchOption.AllDirectories).Where(file => Path.GetFileName(file).Equals("mod.xml", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
				string dropzone = Directory.EnumerateDirectories(tempFolder, "*", SearchOption.AllDirectories).Where(dir => Path.GetFileName(dir).Equals("dropzone", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
				string skyfortress = Directory.EnumerateDirectories(tempFolder, "*", SearchOption.AllDirectories).Where(dir => Path.GetFileName(dir).Equals("dropzone_sky_fortress", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();



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

				Directory.CreateDirectory(mod.Folder);
				if (modInfo != null)
					File.Move(modInfo, Path.Combine(mod.Folder, "Mod.xml"));
				if(dropzone != null)
					Directory.Move(dropzone, Path.Combine(mod.Folder, "dropzone"));
				if(skyfortress != null)
					Directory.Move(skyfortress, Path.Combine(mod.Folder, "dropzone_sky_fortress"));

			});

			if(files.Length == 1){
				var file = files[0];
				if(Directory.Exists(file) && !file.Equals("dropzone",StringComparison.InvariantCultureIgnoreCase))
					mod.Name = Path.GetFileNameWithoutExtension(file).Replace("-", " ").Replace("_", " ");
				if(File.Exists(file) && (file.EndsWith("zip") || file.EndsWith("rar") ||file.EndsWith("7z")))
					mod.Name = Path.GetFileNameWithoutExtension(file).Replace("-", " ").Replace("_", " ");
			}

			AddMod(mod);
			return mod;
		}

		public Category GetCategory(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
				return null;
			var cats = Categories.Where(cat => cat.Name.Equals(name)).ToArray();
			if (cats.Length > 0)
				return cats[0];
			var category = new Category(name);
			category.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "Selected" && ((Category)sender).Selected)
				{
					AllCategoriesSelected = false;
					foreach (var cat in Categories)
					{
						if (cat != sender && cat.Selected)
							cat.Selected = false;
					}
				}
			};
			Categories.Add(category);
			return category;
		}

		public void Save()
		{
			var path = Path.Combine(Settings.files, "mods.json");
			File.WriteAllText(path, JsonConvert.SerializeObject(Mods, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
		}

		protected void SetPropertyField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			System.Diagnostics.Debug.WriteLine("Value: " + newValue.ToString());

			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				field = newValue;
				PropertyChangedEventHandler handler = PropertyChanged;
				System.Diagnostics.Debug.WriteLine("Null: " + (handler == null));
				if (handler != null)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;


	}


}
