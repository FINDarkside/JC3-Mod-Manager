using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
			set { SetPropertyField(ref category, value); }
		}

		private ModInfo info;
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

		[JsonIgnore]
		public ICommand ShowSettingsCommand { get; set; }

		public Mod()
		{
			ShowSettingsCommand = new CommandHandler(() => ShowSettings(), true);
		}

		public void ShowSettings()
		{
			ErrorDialog.Show("Not implemented yet");
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

		private int? version;
		public int? Version
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

		private XmlElement settings;
		public XmlElement Settings
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
