using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public class ConfirmationDialogViewModel
	{
		public string Message { get; set; }

		public ConfirmationDialogViewModel(string message)
		{
			this.Message = message;
		}
	}
}
