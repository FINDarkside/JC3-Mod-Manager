using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Just_Cause_3_Mod_Manager
{

	public class ModSettingsViewModel
	{

		public List<ModSettingsTab> Tabs { get; set; }

		public ModSettingsViewModel(){

		}

		public ModSettingsViewModel(XmlElement element)
		{
			Tabs = new List<ModSettingsTab>();
			foreach (XmlNode node in element.GetElementsByTagName("Tab"))
			{
				try
				{
					var tab = new ModSettingsTab(node);
					Tabs.Add(tab);
				}
				catch (Exception ex)
				{
					Errors.Handle("Failed to create settings viewmodel" ,ex);
				}
			}
		}
	}
}
