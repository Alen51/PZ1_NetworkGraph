using PZ1_NetworkGraph.Helpers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PZ1_NetworkGraph
{
	/// <summary>
	/// Interaction logic for AddTextWindow.xaml
	/// </summary>
	public partial class AddTextWindow : Window
	{
		Point point;

		ShapeTextManager shapeTextManager;

		ShapeTextUnit shapeTextUnit;

		internal AddTextWindow(Point point, ShapeTextManager shapeTextManager, ShapeTextUnit shapeTextUnit)
		{
			InitializeComponent();

			this.point = point;
			this.shapeTextManager = shapeTextManager;
			this.shapeTextUnit = shapeTextUnit;

			InitCmbColors();
		}

		internal AddTextWindow(ShapeTextUnit shapeTextUnit)
		{
			InitializeComponent();

			this.shapeTextUnit = shapeTextUnit;

			InitCmbColors();

			TextTextBox.Text = shapeTextUnit.TextBlock.Text;
			FontSizeTextBox.Text = shapeTextUnit.TextBlock.FontSize.ToString();
			CmbColorsBackground.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.TextBlock.Background).Color);
			CmbColorsText.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.TextBlock.Foreground).Color);
		}

		internal void InitCmbColors()
		{
			CmbColorsBackground.ItemsSource = typeof(Colors).GetProperties();
			CmbColorsText.ItemsSource = typeof(Colors).GetProperties();

			CmbColorsText.SelectedItem = GetColorAsPropertyInfo(Colors.Black);
			CmbColorsBackground.SelectedItem = GetColorAsPropertyInfo(Colors.White);
		}

		internal PropertyInfo GetColorAsPropertyInfo(Color color)
		{
			foreach (var variable in typeof(Colors).GetProperties())
			{
				if ((Color)(variable as PropertyInfo).GetValue(1, null) == color)
				{
					return variable;
				}
			}

			return null;
		}

		void AddTextButton_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckInputsValidity())
			{
				MessageBox.Show("\"Text\" cant be left blank and \"Font\" must be between 10 and 120.\nCheck the input parameters and try again...",
					"Invalid input!",
					MessageBoxButton.OKCancel,
					MessageBoxImage.Warning);

				return;
			}

			string textColorString = CmbColorsText.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush textColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textColorString));
			string backgroundColorString = CmbColorsBackground.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush backgroundColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(backgroundColorString));
			string text = TextTextBox.Text.Trim();
			double fontSize = double.Parse(FontSizeTextBox.Text);

			TextBlock textBlock = shapeTextUnit.TextBlock;

			textBlock.Text = text;
			textBlock.Foreground = textColor;
			textBlock.FontSize = fontSize;
			textBlock.FontFamily = new FontFamily("Arial Nova");
			textBlock.Padding = new Thickness(5);
			textBlock.Background = backgroundColor;

			if(point.X > 0 && point.Y > 0)
			{
				Canvas.SetLeft(textBlock, point.X);
				Canvas.SetTop(textBlock, point.Y);
				Canvas.SetZIndex(textBlock, 2);
			}

			shapeTextManager?.Add(shapeTextUnit);

			Close();
		}

		bool CheckInputsValidity()
		{
			string fontSize = FontSizeTextBox.Text.Trim();
			bool fontSizeValidity = Regex.IsMatch(fontSize, "^([1-9][0-9])|100$");

			bool insideTextExists = TextTextBox.Text.Trim().Length > 0;

			return fontSizeValidity && insideTextExists;
		}
	}
}
