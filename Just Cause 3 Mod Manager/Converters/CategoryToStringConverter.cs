using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Just_Cause_3_Mod_Manager
{
	public class CategoryToStringConverter : IValueConverter
	{

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.WriteLine("Converting cat to string");
			var category = (Category)value;
			return category.Name;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Debug.WriteLine("Converting back");

			string s = (string)value;
			var cats = ModManager.Categories.Where(cat => cat.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase)).ToArray();
			if (cats.Length > 0)
			{
				return cats[0];
			}
			var category = new Category(s, true);
			ModManager.Categories.Add(category);
			return category;
		}

	}
}
