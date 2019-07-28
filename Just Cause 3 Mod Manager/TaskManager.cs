using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public static class TaskManager
	{
		public class NamedTask
		{
			public Task task;
			public string description;

			public NamedTask(string desc, Task task)
			{
				this.task = task;
				this.description = desc;
			}
		}

		private static List<NamedTask> tasks = new List<NamedTask>();

		public static void AddBackgroundTask(string desc, Task task)
		{
			tasks.Add(new NamedTask(desc, task));
		}

		public static async Task WaitForTasks()
		{
			if (tasks.Count == 0)
				return;
			var busyDialogViewModel = new BusyDialogViewModel();
			Settings.mainWindow.BusyDialog.DialogContent = busyDialogViewModel;
			Settings.mainWindow.BusyDialog.IsOpen = true;
			while (tasks.Count > 0)
			{
				var namedTask = tasks[0];
				if (!namedTask.task.IsCompleted)
				{
					busyDialogViewModel.Text = namedTask.description;
					await namedTask.task;
				}
				tasks.RemoveAt(0);
			}
			Settings.mainWindow.BusyDialog.IsOpen = false;
		}

	}
}
