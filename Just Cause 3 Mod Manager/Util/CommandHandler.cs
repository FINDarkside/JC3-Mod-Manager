﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Just_Cause_3_Mod_Manager
{
	public class CommandHandler : ICommand
	{
		private Action _action;
		private bool _canExecute;
		public CommandHandler(Action action)
		{
			_action = action;
			_canExecute = true;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			_action();
		}
	}

}
