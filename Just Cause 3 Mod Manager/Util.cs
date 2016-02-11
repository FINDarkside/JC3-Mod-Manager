using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Just_Cause_3_Mod_Manager
{
	public static class Util
	{
		public static string GetUniqueFile(string preferredFile)
		{
			int num = 1;
			string result = null;
			do
			{
				var ext = Path.GetExtension(preferredFile);
				result = Path.Combine(Path.GetDirectoryName(preferredFile), Path.GetFileNameWithoutExtension(preferredFile) + "_" + num + ext);
			} while (File.Exists(result));
			return result;
		}
	}
}
