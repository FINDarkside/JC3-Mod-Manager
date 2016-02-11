using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public static class Errors
	{

		public static bool showErrorDialog = true;

		public static void Handle(Exception e)
		{
			Handle(null, e);
		}

		public static void Handle(string message)
		{
			Handle(message, null);
		}

		public static void Handle(string message, Exception e)
		{
			if (showErrorDialog)
				ErrorDialog.Show(message, e != null ? (e.Message + "\n" + e.ToString()) : null);
			if(e != null)
				Console.Error.WriteLine(e.ToString());
		}

	}
}
