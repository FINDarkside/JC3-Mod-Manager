using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public class BusyDialogViewModel
	{
		private string text;
		public string Text
		{
			get { return text; }
			set { SetPropertyField(ref text, value); }
		}

		private bool isOpen;
		public bool IsOpen
		{
			get { return isOpen; }
			set { SetPropertyField(ref isOpen, value); }
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
