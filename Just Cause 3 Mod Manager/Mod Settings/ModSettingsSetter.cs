using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Just_Cause_3_Mod_Manager
{
	public class ModSettingsSetter
	{
		public string DataType { get; set; }
		private string File { get; set; }
		private string Path { get; set; }
		private object Value { get; set; }

		public ModSettingsSetter()
		{

		}

		public ModSettingsSetter(XmlNode setterNode)
		{
			if(setterNode.Attributes == null || setterNode.Attributes["file"] == null || setterNode.Attributes["path"] == null)
				throw new Exception("Setter element must have file and path attributes");

			if (setterNode.Attributes["dataType"] != null)
				DataType = setterNode.Attributes["dataType"].Value;

			File = setterNode.Attributes["file"].Value;
			Path = setterNode.Attributes["path"].Value;
			Value = setterNode.InnerText ?? null;
		}

	}
}
