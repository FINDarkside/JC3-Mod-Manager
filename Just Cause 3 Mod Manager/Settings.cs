using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public static class Settings
	{
		public static MainWindow mainWindow;

		public static int revision = 1;
		public static string currentPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
		public static string files = Path.Combine(currentPath, "Files");
		public static string defaultFiles = Path.Combine(files, "Default files");
		public static string gibbedsTools = Path.Combine(files, "Gibbedstools");
		public static string modFolder = Path.Combine(files, "Mods");

		public static UserSettings user;
		public static LocalSettings local;

		static Settings()
		{
			Directory.CreateDirectory(defaultFiles);

			var settingsPath = Path.Combine(files, "settings.json");

			if (File.Exists(settingsPath))
				user = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(settingsPath));
			else
				user = new UserSettings();
			user.JC3Folder = @"C:\Program Files (x86)\Steam\SteamApps\common\Just Cause 3\";

			var localPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "JC3 Mod Manager");
			Directory.CreateDirectory(localPath);
			var localDataPath = Path.Combine(localPath, "data.json");
			if (File.Exists(localDataPath))
				local = JsonConvert.DeserializeObject<LocalSettings>(File.ReadAllText(localDataPath));
			else
				local = new LocalSettings();
		}



	}

	public class UserSettings
	{
		public string JC3Folder;
		public bool checkForUpdates = true;
		public List<string> smallArchiveExtensions = new List<string>();
		public List<string> propertyExtensions = new List<string>();
		public List<string> adfExtensions = new List<string>();
		public List<string> unknownExtensions = new List<string>();

		public void Save()
		{
			var json = JsonConvert.SerializeObject(this);
			File.WriteAllText(Path.Combine(Settings.files, "settings.json"), json);
		}
	}

	public class LocalSettings
	{
		public string lastInstallPath = Settings.currentPath;
		public int lastRevision = Settings.revision;

		public void Save()
		{
			this.lastInstallPath = Settings.currentPath;
			this.lastRevision = Settings.revision;
			var json = JsonConvert.SerializeObject(this);
			var localPath = Path.Combine(Environment.GetEnvironmentVariable("LocalAppData"), "JC3 Mod Manager");
			var localDataPath = Path.Combine(localPath, "data.json");
			File.WriteAllText(localDataPath, json);
		}
	}
}
