using PZ1_NetworkGraph.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PZ1_NetworkGraph
{
	/// <summary>
	/// Interaction logic for DrawPolygonWindow.xaml
	/// </summary>
	public partial class DrawPolygonWindow : Window
	{
		List<Point> points;

		ShapeTextManager shapeTextManager;

		ShapeTextUnit shapeTextUnit;

		internal DrawPolygonWindow(List<Point> points, ShapeTextManager shapeTextManager, ShapeTextUnit shapeTextUnit)
		{
			InitializeComponent();

			this.points = points;
			this.shapeTextManager = shapeTextManager;
			this.shapeTextUnit = shapeTextUnit;

			InitCmbColors();
		}

		internal DrawPolygonWindow(ShapeTextUnit shapeTextUnit)
		{
			InitializeComponent();

			this.shapeTextUnit = shapeTextUnit;

			BrushConverter converter = new BrushConverter();

			InitCmbColors();
			
			CmbColorsFill.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.Shape.Fill).Color);
			CmbColorsStroke.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.Shape.Stroke).Color);
			CmbColorsText.SelectedItem = GetColorAsPropertyInfo(((SolidColorBrush)shapeTextUnit.TextBlock.Foreground).Color);
			OpacitySlider.Value = 101 - (shapeTextUnit.Shape.Fill.Opacity * 100);
			
			TextTextBox.Text = shapeTextUnit.TextBlock.Text;
			ThicknessTextBox.Text = shapeTextUnit.Shape.StrokeThickness.ToString();
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

		internal void CreateAndAddPolygonToCanvas(double thickness, SolidColorBrush fillColor, SolidColorBrush strokeColor)
		{
			Polygon polygon = (Polygon)shapeTextUnit.Shape;

			if (points != null)
			{
				polygon.Points = new PointCollection(points);
			}

			polygon.StrokeThickness = thickness;
			polygon.Fill = fillColor;
			polygon.Stroke = strokeColor;

			Canvas.SetZIndex(polygon, 2);
		}

		void CreateAndAddTextForPolygon(SolidColorBrush textColor)
		{
			TextBlock textBlock = shapeTextUnit.TextBlock;
			Polygon polygon = (Polygon)shapeTextUnit.Shape;

			textBlock.Text = TextTextBox.Text;
			textBlock.Foreground = textColor;
			textBlock.FontSize = 16;
			textBlock.FontFamily = new FontFamily("Arial Nova");
			textBlock.Padding = new Thickness(5);
			textBlock.Background = new SolidColorBrush(Colors.Transparent);

			if (points == null)
			{
				return;
			}

			double minX = points.Select(x => x.X).Min();
			double minY = points.Select(x => x.Y).Min();

			double maxX = points.Select(x => x.X).Max();
			double maxY = points.Select(x => x.Y).Max();

			Canvas.SetLeft(textBlock, minX + (maxX - minX) / 6);
			Canvas.SetTop(textBlock, minY + (maxY - minY) / 6);

			Canvas.SetZIndex(textBlock, 2);
		}

		private void DrawPolygonButton_Click(object sender, RoutedEventArgs e)
		{
			if (!CheckInputsValidity())
			{
				MessageBox.Show("\"Thickness\" must be numbers.\nCheck the input parameters and try again...",
								"Invalid input!",
								MessageBoxButton.OKCancel,
								MessageBoxImage.Warning);
				return;
			}

			double thickness = double.Parse(ThicknessTextBox.Text);
			string fillColorString = CmbColorsFill.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush fillColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(fillColorString));
			string strokeColorString = CmbColorsStroke.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush strokeColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(strokeColorString));
			string textColorString = CmbColorsText.SelectedItem.ToString().Split(' ')[1];
			SolidColorBrush textColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(textColorString));
			fillColor.Opacity = (101 - OpacitySlider.Value) / 100;

			CreateAndAddPolygonToCanvas(thickness, fillColor, strokeColor);

			if (TextTextBox.Text.Length > 0)
			{
				CreateAndAddTextForPolygon(textColor);
			}

			shapeTextManager?.Add(shapeTextUnit);

			Close();
		}

		bool CheckInputsValidity()
		{
			string thickness = ThicknessTextBox.Text.Trim();
			bool thicknessValidity = Regex.IsMatch(thickness, "[0-9.]+");

			return thicknessValidity;
		}
	}
}
