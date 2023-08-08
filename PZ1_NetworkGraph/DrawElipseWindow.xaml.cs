using PZ1_NetworkGraph.Helpers;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PZ1_NetworkGraph
{
	/// <summary>
	/// Interaction logic for DrawElipseWindow.xaml
	/// </summary>
	public partial class DrawElipseWindow : Window
	{
		private Point mousePosition;

		ShapeTextUnit shapeTextUnit;

		ShapeTextManager shapeTextManager;

		internal DrawElipseWindow(Point mousePosition, ShapeTextManager shapeTextManager, ShapeTextUnit shapeTextUnit)
		{
			InitializeComponent();

			this.mousePosition = mousePosition;
			this.shapeTextManager = shapeTextManager;
			this.shapeTextUnit = shapeTextUnit;

			InitCmbColors();
		}

		internal DrawElipseWindow(ShapeTextUnit shapeTextUnit)
		{
			InitializeComponent();

			this.shapeTextUnit = shapeTextUnit;
			mousePosition = new Point() { X = -1, Y = -1 };

			InitCmbColors();

			CmbColorsFill.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.Shape.Fill).Color);
			CmbColorsStroke.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.Shape.Stroke).Color);
			CmbColorsText.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.TextBlock.Foreground).Color);
			OpacitySlider.Value = 101 - (shapeTextUnit.Shape.Fill.Opacity * 100);

			TextTextBox.Text = shapeTextUnit.TextBlock.Text;
			ThicknessTextBox.Text = shapeTextUnit.Shape.StrokeThickness.ToString();

			HorizontalSemidiameterTextBox.Text = (shapeTextUnit.Shape.Width / 2).ToString();
			VerticalSemidiameterTextBox.Text = (shapeTextUnit.Shape.Height / 2).ToString();
		}

		internal void InitCmbColors()
		{
			CmbColorsStroke.ItemsSource = typeof(Colors).GetProperties();
			CmbColorsFill.ItemsSource = typeof(Colors).GetProperties();
			CmbColorsText.ItemsSource = typeof(Colors).GetProperties();

			CmbColorsStroke.SelectedItem = GetColorAsPropertyInfo(Colors.Black);
			CmbColorsFill.SelectedItem = GetColorAsPropertyInfo(Colors.White);
			CmbColorsText.SelectedItem = GetColorAsPropertyInfo(Colors.Black);
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

		internal bool CheckInputsValidity()
		{
			string verticalSemidia = VerticalSemidiameterTextBox.Text.Trim();
			bool verticalSemidiaValidity = Regex.IsMatch(verticalSemidia, "[0-9.]+");

			string horizontalSemidia = VerticalSemidiameterTextBox.Text.Trim();
			bool horizontalSemidiaValidity = Regex.IsMatch(horizontalSemidia, "[0-9.]+");

			string thickness = ThicknessTextBox.Text.Trim();
			bool thicknessValidity = Regex.IsMatch(thickness, "[0-9.]+");

			return verticalSemidiaValidity && horizontalSemidiaValidity && thicknessValidity;
		}

		internal Ellipse CreateAndAddEllipseToCanvas(double thickness, SolidColorBrush fillColor, SolidColorBrush strokeColor, double horizontalSemidia, double verticalSemidia)
		{
			Ellipse ellipse = (Ellipse)shapeTextUnit.Shape;

			ellipse.StrokeThickness = thickness;
			ellipse.Fill = fillColor;
			ellipse.Stroke = strokeColor;
			ellipse.Width = horizontalSemidia * 2;
			ellipse.Height = verticalSemidia * 2;

			if (mousePosition.X > 0 && mousePosition.Y > 0)
			{
				Canvas.SetLeft(ellipse, mousePosition.X);
				Canvas.SetTop(ellipse, mousePosition.Y);
				Canvas.SetZIndex(ellipse, 2);
			}

			return ellipse;
		}

		internal TextBlock CreateAndAddTextFillForElipse(SolidColorBrush textColor)
		{
			TextBlock textBlock = shapeTextUnit.TextBlock;
			Ellipse ellipse = (Ellipse)shapeTextUnit.Shape;

			textBlock.Text = TextTextBox.Text;
			textBlock.Foreground = textColor;
			textBlock.FontSize = 16;
			textBlock.FontFamily = new FontFamily("Arial Nova");
			textBlock.Padding = new Thickness(5);
			textBlock.Background = new SolidColorBrush(Colors.Transparent);

			Canvas.SetLeft(textBlock, Canvas.GetLeft(ellipse) + ellipse.Width / 4);
			Canvas.SetTop(textBlock, Canvas.GetTop(ellipse) + ellipse.Height / 4);
			Canvas.SetZIndex(textBlock, 2);

			return textBlock;
		}

		private void DrawElipseButton_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckInputsValidity())
			{
				MessageBox.Show("\"Semidiameters\" and \"Thickness\" must be numbers.\nCheck the input parameters and try again...",
					"Invalid input!",
					MessageBoxButton.OKCancel,
					MessageBoxImage.Warning);

				return;
			}

			double verticalSemidia = double.Parse(VerticalSemidiameterTextBox.Text.Trim());
			double horizontalSemidia = double.Parse(HorizontalSemidiameterTextBox.Text.Trim());
			double thickness = double.Parse(ThicknessTextBox.Text.Trim());

			string fillColorString = CmbColorsFill.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fillColorString));
			string strokeColorString = CmbColorsStroke.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush strokeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(strokeColorString));
			string textColorString = CmbColorsText.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush textColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textColorString));
			fillColor.Opacity = (101 - OpacitySlider.Value) / 100;

			CreateAndAddEllipseToCanvas(thickness, fillColor, strokeColor, horizontalSemidia, verticalSemidia);

			if (TextTextBox.Text.Trim().Length > 0)
			{
				CreateAndAddTextFillForElipse(textColor);
			}

			shapeTextManager?.Add(shapeTextUnit);

			Close();
		}
	}
}
