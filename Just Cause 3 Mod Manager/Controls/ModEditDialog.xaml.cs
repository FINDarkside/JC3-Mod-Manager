using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Just_Cause_3_Mod_Manager
{
	/// <summary>
	/// Interaction logic for AddModDialog.xaml
	/// </summary>
	public partial class ModEditDialog : Window
	{

		public Mod Mod { get; set; }

		public ModEditDialog()
		{
			InitializeComponent();
		}

		public ModEditDialog(Mod mod)
		{
			if (Settings.mainWindow != null)
			{
				this.Owner = Settings.mainWindow;
				this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			}
			InitializeComponent();
			this.Icon = BitmapImage.Create(2, 2, 96, 96, PixelFormats.Indexed1, new BitmapPalette(new List<System.Windows.Media.Color> { Colors.Transparent }), new byte[] { 0, 0, 0, 0 }, 1);
			Mod = mod;
			this.DataContext = this;
		}

		private void OkClicked(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}
	}
}
