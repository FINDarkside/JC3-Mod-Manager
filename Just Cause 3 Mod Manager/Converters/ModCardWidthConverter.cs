using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Just_Cause_3_Mod_Manager
{
	public class ModCardWidthConverter : IValueConverter
	{
		private const double preferredWidth = 350;

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var w = (double)value;
			var columns = Math.Floor(w / (preferredWidth + 10));
			if (columns < 1)
				columns = 1;
			return (w / columns) - 10;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
