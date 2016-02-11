using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Just_Cause_3_Mod_Manager
{
	public static class ModManager
	{
		public static ObservableCollection<Mod> Mods { get; set; }
		public static ListCollectionView ModsView{get;set;}
		public static ObservableCollection<Category> Categories { get; set; }

		public static void Init()
		{
			Mods = new ObservableCollection<Mod>();
			Categories = new ObservableCollection<Category>();
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
									if (!cats.Contains(Categories[i]))
										Categories.RemoveAt(i);
								}
								ModsView.Refresh();
							}
						};
					}
				}
			};
			var cat = new Category("No category", true);
			Categories.Add(cat);
			Mods.Add(new Mod() { Name = "test", Category = cat, Info = new ModInfo() { Author = "FINDarkside", Version = 2 } });
			Mods.Add(new Mod() { Name = "test2", Category = cat, Info = new ModInfo() { Author = "FINDarkside", Version = 2 } });
			Mods.Add(new Mod() { Name = "test2", Category = cat, Info = new ModInfo() { Author = "FINDarkside", Version = 2 } });
			Mods.Add(new Mod() { Name = "test2", Category = cat, Info = new ModInfo() { Author = "FINDarkside", Version = 2 } });

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
