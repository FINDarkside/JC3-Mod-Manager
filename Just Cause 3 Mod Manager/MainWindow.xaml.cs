using Microsoft.Win32;
using Ookii.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Just_Cause_3_Mod_Manager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public ListCollectionView ModsView { get; set; }
		public List<Task> tasks = new List<Task>();

		public MainWindow()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((object sender, UnhandledExceptionEventArgs args) =>
			{
				File.WriteAllText(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "crash.txt"), args.ExceptionObject.ToString());
				Application.Current.Shutdown();
			});
#if !DEBUG
			CheckForUpdates();
#endif
			tasks.Add(GameFiles.LoadFileNames());
			ModManager.Init();
			ModsView = ModManager.ModsView;
			InitializeComponent();
			Title += " r" + Settings.revision;

			this.Drop += (sender, e) =>
			{
				FileDrop(sender, e);
			};



			this.DataContext = this;
			Settings.mainWindow = this;
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (Settings.local.lastRevision >= 5 && Settings.local.lastRevision < Settings.revision && Settings.local.lastInstallPath != Settings.currentPath)
			{
				var defaultFilesPath = Path.Combine(Settings.local.lastInstallPath, @"Files\Default files");

				if (Directory.Exists(defaultFilesPath))
				{
					busyIndicator.IsBusy = true;
					busyIndicator.BusyContent = "Moving default file cache from " + defaultFilesPath;
					await Task.Run(() =>
					{
						foreach (var file in Directory.EnumerateFiles(defaultFilesPath, "*", SearchOption.AllDirectories))
						{
							var relativePath = file.Substring(defaultFilesPath.Length + 1);
							var newPath = Path.Combine(Settings.defaultFiles, relativePath);
							if (!File.Exists(newPath))
							{
								File.Move(file, newPath);
							}
						}
					});
					busyIndicator.IsBusy = false;
				}
			}

			Settings.local.Save();
		}

		public async Task CompleteTasks()
		{
			Settings.SetBusyContent("Waiting");
			for (var i = tasks.Count - 1; i >= 0; i--)
			{
				if (!tasks[i].IsCompleted)
					await tasks[i];
				tasks.RemoveAt(i);
			}
			Settings.SetBusyContent(null);
		}

		public async void FileDrop(object sender, DragEventArgs e)
		{
			await CompleteTasks();

			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{

				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				Settings.SetBusyContent("Extracting and determining mod files");

				await ModManager.AddMod(files);

				Settings.SetBusyContent(null);

			}
		}

		private static void CheckForUpdates()
		{
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
						string match = Regex.Match(result, @"<b>Version</b>r[0-9]+<").Value;
						int newestRevision = int.Parse(Regex.Match(match, "r[0-9]+").Value.Substring(1));
						if (newestRevision > Settings.revision && System.Windows.MessageBox.Show("Current version: r" + Settings.revision + "\nNewest version: r" + newestRevision + "\nOpen justcause3mods.com mod page?", "New version available", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
							Process.Start("http://justcause3mods.com/mods/mod-combiner/");
					});
					webClient.DownloadStringTaskAsync("http://justcause3mods.com/mods/mod-manager/");
				}
				catch (Exception e)
				{
					Errors.Handle("Failed to check for new version", e);
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			ModManager.Save();
		}
	}
}
