using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using SharpCompress.Archive;
using Newtonsoft.Json;

namespace Just_Cause_3_Mod_Manager
{
	public static class ModManager
	{
		public static ObservableCollection<Mod> Mods { get; set; }
		public static ListCollectionView ModsView { get; set; }
		public static ObservableCollection<Category> Categories { get; set; }

		public static void Init()
		{
			
			Categories = new ObservableCollection<Category>();

			var modsJson = Path.Combine(Settings.files, "mods.json");
			var mods = JsonConvert.DeserializeObject<ObservableCollection<Mod>>(File.ReadAllText(modsJson));
			foreach (var mod in mods)
			{
				var expanded = mod.Category.IsExpanded;
				mod.Category = GetCategory(mod.Category.Name);
				mod.Category.IsExpanded = expanded;
			}
			Mods = mods;

			ModsView = new ListCollectionView(ModManager.Mods);

			ModsView.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

			Mods.CollectionChanged += (sender, e) =>
			{
				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					foreach (Mod mod in e.NewItems)
					{
						mod.PropertyChanged += (Object s, PropertyChangedEventArgs args) =>
						{
							if (args.PropertyName == "Category")
							{
								List<Category> cats = Mods.Select(item => item.Category).Distinct().ToList();
								for (var i = Categories.Count - 1; i >= 0; i--)
								{
									if (!Mods.Any(item => item.Category == Categories[i]))
										Categories.RemoveAt(i);
								}
								ModsView.Refresh();
							}
						};
					}
				}
			};

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
				string modSettings = null;
				foreach (var file in Directory.EnumerateFiles(tempFolder, "*", SearchOption.AllDirectories))
				{
					if (GameFiles.IsGameFile(file))
					{
						modFiles[Path.GetFileName(file)] = file;
					}
					else if (Path.GetFileName(file).Equals("Mod.json", StringComparison.InvariantCultureIgnoreCase))
					{
						modSettings = file;
					}
				}

				mod = new Mod();
				do
				{
					mod.Id = Guid.NewGuid().ToString("N");
				} while (Directory.Exists(mod.Folder) || File.Exists(mod.Folder));
				if (modSettings != null)
					mod.Info = JsonConvert.DeserializeObject<ModInfo>(File.ReadAllText(modSettings));

				Directory.CreateDirectory(mod.Folder);
				foreach (var file in modFiles.Values)
				{
					File.Move(file, Path.Combine(mod.Folder, Path.GetFileName(file)));
				}
			});
			mod.Name = "";
			if (files.Length == 1 && File.Exists(files[0]) && !GameFiles.IsGameFile(files[0]))//Probably archive lol
				mod.Name = Path.GetFileNameWithoutExtension(files[0]).Replace("-", " ").Replace("_", " ");
			mod.Category = GetCategory("No category");
			mod.Info = new ModInfo();

			Settings.SetBusyContent(null);

			if (new ModEditDialog(mod).ShowDialog() == true)
				Mods.Add(mod);
		}

		public static Category GetCategory(string name)
		{
			var cats = Categories.Where(cat => cat.Name.Equals(name)).ToArray();
			if (cats.Length > 0)
				return cats[0];
			var category = new Category(name, true);
			Categories.Add(category);
			return category;
		}

		public static void Save()
		{
			var path = Path.Combine(Settings.files, "mods.json");
			File.WriteAllText(path, JsonConvert.SerializeObject(Mods));
		}



	}

	public class Category : INotifyPropertyChanged
	{
		public string Name { get; set; }
		private bool isExpanded;
		public bool IsExpanded
		{
			get { return isExpanded; }
			set { SetPropertyField(ref isExpanded, value); }
		}

		public Category(string name, bool collapsed)
		{
			Name = name;
			IsExpanded = collapsed;
		}

		public override string ToString()
		{
			return Name;
		}

		protected void SetPropertyField<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
		{
			if (!EqualityComparer<T>.Default.Equals(field, newValue))
			{
				field = newValue;
				PropertyChangedEventHandler handler = PropertyChanged;
				if (handler != null)
					handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
