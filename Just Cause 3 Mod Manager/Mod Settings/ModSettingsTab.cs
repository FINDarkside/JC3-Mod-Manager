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
	public class ModSettingsTab
	{

		public string Name { get; set; }
		public List<ModSettingsField> Fields { get; set; }

		public ModSettingsTab()
		{

		}

		public ModSettingsTab(XmlNode tabNode)
		{
			if (tabNode.Attributes == null || tabNode.Attributes["name"] == null)
				throw new ArgumentException("Settings tab must have name attribute");
			Name = tabNode.Attributes["name"].Value;
			Fields = new List<ModSettingsField>();
			foreach (XmlNode node in tabNode)
			{
				if (node.NodeType != XmlNodeType.Element)
					throw new Exception("Tab element can only contain XmlElements");

				try
				{
					if (node.Name.ToLower() == "textbox")
					{
						Fields.Add(new ModSettingsTextBox(node));
					}
					else if (node.Name.ToLower() == "slider")
					{
						Fields.Add(new ModSettingsSlider(node));
					}
					else if (node.Name.ToLower() == "combobox")
					{
						Fields.Add(new ModSettingsComboBox(node));
					}
					else if (node.Name.ToLower() == "togglebutton")
					{
						Fields.Add(new ModSettingsToggleButton(node));
					}
					else
					{
						throw new ArgumentException("Unknown field type \"" + node.Name + "\"");
					}
				}
				catch (Exception ex)
				{
					Errors.Handle("Failed to create settings field", ex);
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
