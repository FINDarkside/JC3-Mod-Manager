using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;

namespace Just_Cause_3_Mod_Manager
{
	public class Mod : INotifyPropertyChanged
	{
		private string id;
		public string Id
		{
			get { return id; }
			set
			{
				Folder = Path.Combine(Settings.modFolder, value);
				SetPropertyField(ref id, value);
			}
		}

		private string name;
		public string Name
		{
			get { return name; }
			set { SetPropertyField(ref name, value); }
		}

		private bool active;
		public bool Active
		{
			get { return active; }
			set { SetPropertyField(ref active, value); }
		}

		private Category category;
		public Category Category
		{
			get { return category; }
			set
			{
				if (Category != null)
					Category.ModCount--;
				if (value != null)
					value.ModCount++;
				SetPropertyField(ref category, value);
			}
		}

		private ModInfo info = new ModInfo();
		public ModInfo Info
		{
			get { return info; }
			set { SetPropertyField(ref info, value); }
		}

		private string folder = null;
		[JsonIgnore]
		public string Folder
		{
			get { return folder; }
			private set { folder = value; }
		}

		private bool hasUpdate = false;
		[JsonIgnore]
		public bool HasUpdate
		{
			get { return hasUpdate; }
			private set { SetPropertyField(ref hasUpdate, value); }
		}

		[JsonIgnore]
		public ICommand EditCommand { get; set; }
		[JsonIgnore]
		public ICommand ShowSettingsCommand { get; set; }
		[JsonIgnore]
		public ICommand OpenModPageCommand { get; set; }
		[JsonIgnore]
		public ICommand DeleteCommand { get; set; }

		public Mod()
		{
			ShowSettingsCommand = new CommandHandler(() =>
			{
				if (this.Info.Settings != null && this.Info.Settings.Tabs.Count > 0)
					MaterialDesignThemes.Wpf.DialogHost.Show(this.Info.Settings);
			});
			OpenModPageCommand = new CommandHandler(() =>
			{
				if (new Uri(Info.ModPage).Host == "justcause3mods.com")
					Process.Start(info.ModPage);
			});
			EditCommand = new CommandHandler(() =>
			{
				MaterialDesignThemes.Wpf.DialogHost.Show(this);
			});
			DeleteCommand = new CommandHandler(async () =>
			{
				var result = (bool)await MaterialDesignThemes.Wpf.DialogHost.Show(new ConfirmationDialogViewModel("Are you sure?"));
				if (result)
				{
					this.category = null;
					ModManager.Mods.Remove(this);
					TaskManager.AddBackgroundTask("Deleting " + Name, Task.Run(() =>
					{
						if (folder != null && Directory.Exists(folder))
							Directory.Delete(folder, true);
					}));
				}
			});
		}

		public void CheckForUpdates()
		{
			if (info.ModPage == null)
				return;
			string host = new Uri(Info.ModPage).Host;
			Debug.WriteLine(host);
			if (host != "justcause3mods.com")
				return;
			if (Settings.user.checkForUpdates && System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
			{
				try
				{
					WebClient webClient = new WebClient();
					webClient.DownloadStringCompleted += (DownloadStringCompletedEventHandler)((sender, e) =>
					{
						if (e.Error != null)
							return;
						string result = e.Result;
						string match = Regex.Match(result, @"<b>Version</b>.+?<").Value;

						CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
						ci.NumberFormat.CurrencyDecimalSeparator = ".";
						float newestRevision = float.Parse(Regex.Match(match, @"[0-9]\.?[0-9]").Value, NumberStyles.Any, ci) + 1;
						this.HasUpdate = newestRevision > this.Info.Version;
					});
					webClient.DownloadStringTaskAsync(info.ModPage);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
					Errors.Handle("Failed to check for new version", e);
				}
			}

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

	public class ModInfo : INotifyPropertyChanged
	{
		private string author;
		public string Author
		{
			get { return author; }
			set { SetPropertyField(ref author, value); }
		}

		private float? version;
		public float? Version
		{
			get { return version; }
			set { SetPropertyField(ref version, value); }
		}

		private string modPage;
		public string ModPage
		{
			get { return modPage; }
			set { SetPropertyField(ref modPage, value); }
		}

		private string defaultName;
		public string DefaultName
		{
			get { return defaultName; }
			set { SetPropertyField(ref defaultName, value); }
		}

		private ModSettingsViewModel settings;
		public ModSettingsViewModel Settings
		{
			get { return settings; }
			set { SetPropertyField(ref settings, value); }
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
