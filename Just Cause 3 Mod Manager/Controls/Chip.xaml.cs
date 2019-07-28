using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Just_Cause_3_Mod_Manager
{
	/// <summary>
	/// Interaction logic for Chip.xaml
	/// </summary>
	public partial class Chip : UserControl
	{

		public static readonly DependencyProperty FirstValueProperty = DependencyProperty.Register("FirstValue", typeof(string), typeof(Chip), new PropertyMetadata(""));

		public string FirstValue
		{
			get { return (string)GetValue(FirstValueProperty); }
			set { SetValue(FirstValueProperty, value); }
		}

		public static readonly DependencyProperty SecondValueProperty = DependencyProperty.Register("SecondValue", typeof(string), typeof(Chip), new PropertyMetadata(""));

		public string SecondValue
		{
			get { return (string)GetValue(SecondValueProperty); }
			set { SetValue(SecondValueProperty, value); }
		}

		public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(Chip),
			new PropertyMetadata(false, new PropertyChangedCallback((o, e) =>
			{
				var sender = o as Chip;
				if (sender == null)
					return;
				if (e.NewValue != e.OldValue)
					sender.UpdateAnimations((bool)e.NewValue);
			})));

		public bool Selected
		{
			get { return (bool)GetValue(SelectedProperty); }
			set
			{
				SetValue(SelectedProperty, value);
			}
		}

		public ICommand ChipClickedCommand { get; set; }

		public Chip()
		{
			ChipClickedCommand  = new CommandHandler(() =>
			{
				Selected = true;
			});
			InitializeComponent();
		}

		private static readonly Color firstValueForegroundActive = (Color)ColorConverter.ConvertFromString("#2196F3");
		private static readonly Color firstValueForegroundNonActive = (Color)ColorConverter.ConvertFromString("#CC000000");

		private static readonly Color secondValueBackgroundActive = (Color)ColorConverter.ConvertFromString("#2196F3");
		private static readonly Color secondValueBackgroundNonActive = (Color)ColorConverter.ConvertFromString("#e9e9e9");

		private static readonly Color secondValueForegroundActive = (Color)ColorConverter.ConvertFromString("#fff");
		private static readonly Color secondValueForegroundNonActive = (Color)ColorConverter.ConvertFromString("#999");

		private void UpdateAnimations(bool isMouseOver)
		{
			var duration = new Duration(TimeSpan.FromMilliseconds(300));
			SolidColorBrush brush;
			ColorAnimation animation;
			var active = isMouseOver || Selected;

			brush = tbFirstValue.Foreground as SolidColorBrush;
			animation = new ColorAnimation();
			animation.To = active ? firstValueForegroundActive : firstValueForegroundNonActive;
			animation.Duration = duration;
			brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

			brush = secondValueBorder.Background as SolidColorBrush;
			animation = new ColorAnimation();
			animation.To = active ? secondValueBackgroundActive : secondValueBackgroundNonActive;
			animation.Duration = duration;
			brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

			brush = tbSecondValue.Foreground as SolidColorBrush;
			animation = new ColorAnimation();
			animation.To = active ? secondValueForegroundActive : secondValueForegroundNonActive;
			animation.Duration = duration;
			brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);

			var dropShadow = dropShadowEffect;
			DoubleAnimation opacityAnimation = new DoubleAnimation();
			opacityAnimation.From = dropShadow.Opacity;
			opacityAnimation.To = active ? .42 : 0;
			opacityAnimation.Duration = duration;
			dropShadowEffect.BeginAnimation(DropShadowEffect.OpacityProperty, opacityAnimation);

		}

		private void Border_MouseEnter(object sender, MouseEventArgs e)
		{
			UpdateAnimations(true);
		}

		private void Border_MouseLeave(object sender, MouseEventArgs e)
		{
			UpdateAnimations(false);
		}
	}
}
