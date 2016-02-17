using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Just_Cause_3_Mod_Manager
{
	public abstract class ModSettingsField
	{
		public string Name { get; set; }

		public ModSettingsField()
		{

		}

		public ModSettingsField(XmlNode fieldNode)
		{
			if (fieldNode.Attributes == null || fieldNode.Attributes["name"] == null)
				throw new ArgumentException(fieldNode.Name + " element must have name attribute");
			Name = fieldNode.Attributes["name"].Value;
		}
	}

	public class ModSettingsTextBox : ModSettingsField
	{
		public string Value { get; set; }
		public List<ModSettingsSetter> Setters { get; set; }

		public ModSettingsTextBox()
		{

		}

		public ModSettingsTextBox(XmlNode textBoxNode)
			: base(textBoxNode)
		{
			Setters = new List<ModSettingsSetter>();
			if (textBoxNode.Attributes["value"] == null)
				throw new Exception("TextBox must have value attribute");
			Value = textBoxNode.Attributes["value"].Value;
			foreach (XmlNode node in textBoxNode)
			{
				if (node.NodeType != XmlNodeType.Element || node.Name.ToLower() != "setter")
					throw new ArgumentException("TextBox field can only contain Setter elements");

				Setters.Add(new ModSettingsSetter(node));
			}
		}

		public ModSettingsTextBox(string value, List<ModSettingsSetter> setters)
		{
			Value = value;
			Setters = setters;
		}
	}

	public class ModSettingsToggleButton : ModSettingsField
	{
		public bool Value { get; set; }
		public List<ModSettingsSetter> CheckedSetters { get; set; }
		public List<ModSettingsSetter> UncheckedSetters { get; set; }

		public ModSettingsToggleButton()
		{

		}

		public ModSettingsToggleButton(XmlNode toggleButtonNode)
			: base(toggleButtonNode)
		{

			var checkedNode = ((XmlElement)toggleButtonNode).GetElementsByTagName("Checked");
			if (checkedNode.Count == 0)
				throw new Exception("ToggleButton element must contain Checked element");
			var uncheckedNode = ((XmlElement)toggleButtonNode).GetElementsByTagName("Unchecked");
			if (uncheckedNode.Count == 0)
				throw new Exception("ToggleButton element must contain Unchecked element");

			if (toggleButtonNode.Attributes["checked"] != null && toggleButtonNode.Attributes["checked"].Value.ToLower() == "true")
				Value = true;

			CheckedSetters = new List<ModSettingsSetter>();
			UncheckedSetters = new List<ModSettingsSetter>();

			foreach (XmlNode node in checkedNode[0])
			{
				CheckedSetters.Add(new ModSettingsSetter(node));
			}
			foreach (XmlNode node in uncheckedNode[0])
			{
				UncheckedSetters.Add(new ModSettingsSetter(node));
			}
		}

		public ModSettingsToggleButton(bool value, List<ModSettingsSetter> checkedSetters, List<ModSettingsSetter> uncheckedSetters)
		{
			Value = value;
			CheckedSetters = checkedSetters;
			UncheckedSetters = uncheckedSetters;
		}
	}

	public class ModSettingsSlider : ModSettingsField
	{
		public double Value { get; set; }
		public double MinVal { get; set; }
		public double MaxVal { get; set; }
		public double TickFrequency { get; set; }
		public List<ModSettingsSetter> Setters { get; set; }

		public ModSettingsSlider()
		{

		}

		public ModSettingsSlider(XmlNode sliderNode)
			: base(sliderNode)
		{
			Setters = new List<ModSettingsSetter>();

			if (sliderNode.Attributes["maxValue"] == null || sliderNode.Attributes["minValue"] == null)
				throw new Exception("Slider element must have minValue and maxValue attributes");
			MinVal = double.Parse(sliderNode.Attributes["minValue"].Value, CultureInfo.InvariantCulture);
			MaxVal = double.Parse(sliderNode.Attributes["maxValue"].Value, CultureInfo.InvariantCulture);

			if (sliderNode.Attributes["value"] == null)
				throw new Exception("Slider must have value attribute");
			Value = double.Parse(sliderNode.Attributes["value"].Value, CultureInfo.InvariantCulture);
			TickFrequency = 1;
			if (sliderNode.Attributes["tickFrequency"] != null)
				TickFrequency = double.Parse(sliderNode.Attributes["tickFrequency"].Value, CultureInfo.InvariantCulture);


			foreach (XmlNode node in sliderNode)
			{
				if (sliderNode.NodeType != XmlNodeType.Element || node.Name.ToLower() != "setter")
					throw new ArgumentException("Slider element can only contain Setter elements");
				Setters.Add(new ModSettingsSetter(node));
			}
		}
	}


	public class ModSettingsComboBox : ModSettingsField
	{

		public int SelectedIndex { get; set; }
		public List<ModSettingsComboBoxItem> Items { get; set; }

		public ModSettingsComboBox()
		{

		}

		public ModSettingsComboBox(XmlNode comboBoxNode)
			: base(comboBoxNode)
		{
			if (comboBoxNode.Attributes["selectedIndex"] != null)
				SelectedIndex = int.Parse(comboBoxNode.Attributes["selectedIndex"].Value);
			Items = new List<ModSettingsComboBoxItem>();
			foreach (XmlNode node in comboBoxNode)
			{
				if (node.NodeType != XmlNodeType.Element || node.Name.ToLower() != "comboboxitem")
					throw new ArgumentException("ComboBox element can only contain ComboBoxItem elements");
				var item = new ModSettingsComboBoxItem(node);
				Items.Add(item);
			}
		}
	}

	public class ModSettingsComboBoxItem
	{
		public string Name { get; set; }
		public List<ModSettingsSetter> Setters { get; set; }

		public ModSettingsComboBoxItem()
		{

		}

		public ModSettingsComboBoxItem(XmlNode comboBoxItemNode)
		{
			if (comboBoxItemNode.Attributes == null || comboBoxItemNode.Attributes["name"] == null)
				throw new Exception("ComboBoxItem element must have name attribute");
			Name = comboBoxItemNode.Attributes["name"].Value;
			Setters = new List<ModSettingsSetter>();
			foreach (XmlNode node in comboBoxItemNode)
			{
				if (node.Name.ToLower() != "setter")
					throw new Exception("ComboBoxItem can only contain Setter elements");
				Setters.Add(new ModSettingsSetter(node));
			}
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
