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
			var category = (Category)value;
			return category != null ? category.Name : null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var cat = ModManager.GetCategory((string)value);
			return cat;
		}

	}
}
