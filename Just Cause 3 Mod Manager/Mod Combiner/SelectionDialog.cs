﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Just_Cause_3_Mod_Manager
{
	public class SelectionItem
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public bool Selected { get; set; }
		public object Value { get; set; }
	}

	public partial class SelectionDialog : Window
	{

		public bool DontShowAgain { get; set; }
		public IList<SelectionItem> Items { get; set; }
		public object SelectedItem;

		public SelectionDialog()
		{
			//InitializeComponent();
		}


		public SelectionDialog(IList<SelectionItem> items)
		{
			//InitializeComponent();

			foreach (var item in items)
				item.Selected = false;
			this.MaxHeight = System.Windows.SystemParameters.PrimaryScreenWidth;
			this.Items = items;

			this.DataContext = this;
		}

		public static bool Show(string header, IList<SelectionItem> items, out object selectedValue, out bool notifyCollissions)
		{
			object result = null;
			bool notifyColl = false;
			bool dialogResult = false;
			Application.Current.Dispatcher.Invoke((Action)delegate
			{
				var dialog = new SelectionDialog(items);
				//dialog.tbHeader.Text = header;
				dialog.Owner = Settings.mainWindow;
				dialog.WindowStartupLocation = Settings.mainWindow != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen;
				dialog.ShowDialog();
				result = dialog.SelectedItem;
				notifyColl = dialog.DontShowAgain;
				dialogResult = dialog.DialogResult == true;
			});
			selectedValue = result;
			notifyCollissions = notifyColl;
			return dialogResult;
		}

		private void SelectClicked(object sender, RoutedEventArgs e)
		{
			bool selectedValue = false;

			foreach (SelectionItem item in Items)
			{
				if (item.Selected)
				{
					this.SelectedItem = item.Value;
					selectedValue = true;
					break;
				}
			}

			if (selectedValue)
			{
				this.DialogResult = true;
			}
			else
			{
				MessageBox.Show("Select value");
			}
		}
	}
}
