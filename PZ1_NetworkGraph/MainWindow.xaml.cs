using PZ1_NetworkGraph.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PZ1_NetworkGraph
{
	public partial class MainWindow : Window
	{
		ModelRenderer renderer;

		ShapeTextManager shapeTextManager;

		List<Point> polygonPoints;

		Canvas networkCanvas;

		public MainWindow()
		{
			networkCanvas = NetworkCanvas;
			InitializeComponent();
			polygonPoints = new List<Point>();
			renderer = new ModelRenderer(NetworkCanvas);
			shapeTextManager = new ShapeTextManager(NetworkCanvas);
		}

		private void OpenDrawElipseDialog(Point mousePosition)
		{
			ShapeTextUnit shapeTextUnit = new ShapeTextUnit(new Ellipse(), new TextBlock());
			DrawElipseWindow drawElipseWindow = new DrawElipseWindow(mousePosition, shapeTextManager, shapeTextUnit)
			{
				Owner = Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};

			shapeTextUnit.Shape.MouseDown += UpdateShapeTextUnit;
			shapeTextUnit.TextBlock.MouseDown += UpdateShapeTextUnit;

			drawElipseWindow.ShowDialog();
		}

		public void OpenDrawPolygonDialog()
		{
			ShapeTextUnit shapeTextUnit = new ShapeTextUnit(new Polygon(), new TextBlock());
			DrawPolygonWindow drawPolygonWindow = new DrawPolygonWindow(polygonPoints, shapeTextManager, shapeTextUnit)
			{
				Owner = Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};

			shapeTextUnit.Shape.MouseLeftButtonDown += UpdateShapeTextUnit;
			shapeTextUnit.TextBlock.MouseLeftButtonDown += UpdateShapeTextUnit;

			drawPolygonWindow.ShowDialog();
		}

		public void OpenAddTextDialog(Point mousePosition)
		{
			ShapeTextUnit shapeTextUnit = new ShapeTextUnit(new TextBlock());
			AddTextWindow addTextWindow = new AddTextWindow(mousePosition, shapeTextManager, shapeTextUnit)
			{
				Owner = Window.GetWindow(this),
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};

			shapeTextUnit.TextBlock.MouseLeftButtonDown += UpdateShapeTextUnit;

			addTextWindow.ShowDialog();
		}

		private void EnableAllMenuButtons()
		{
			DrawElipseButton.IsEnabled = true;
			DrawPolygonButton.IsEnabled = true;
			AddTextButton.IsEnabled = true;
		}

		private bool CheckIfSomeButtonIsPressed()
		{
			return !DrawElipseButton.IsEnabled || !DrawPolygonButton.IsEnabled || !AddTextButton.IsEnabled;
		}

		private void UpdateShapeTextUnit(object sender, MouseEventArgs args)
		{
			bool isEllipse = sender.GetType().Equals(typeof(Ellipse));
			bool isPolygon = sender.GetType().Equals(typeof(Polygon));
			bool isText = sender.GetType().Equals(typeof(TextBlock));

			if (isEllipse)
			{
				ShapeTextUnit shapeTextUnit = shapeTextManager.FindShapeTextUnitByShape((Ellipse)sender);

				DrawElipseWindow drawElipseWindow = new DrawElipseWindow(shapeTextUnit)
				{
					Owner = Window.GetWindow(this),
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				};
				drawElipseWindow.ShowDialog();
			}
			else if(isPolygon)
			{
				ShapeTextUnit shapeTextUnit = shapeTextManager.FindShapeTextUnitByShape((Polygon)sender);

				DrawPolygonWindow drawElipseWindow = new DrawPolygonWindow(shapeTextUnit)
				{
					Owner = Window.GetWindow(this),
					WindowStartupLocation = WindowStartupLocation.CenterOwner
				};
				drawElipseWindow.ShowDialog();
			}
			else if(isText)
			{
				ShapeTextUnit shapeTextUnit = shapeTextManager.FindShapeTextUnitByText((TextBlock)sender);

				if (shapeTextUnit.Shape != null && shapeTextUnit.Shape.GetType().Equals(typeof(Ellipse)))
				{
					DrawElipseWindow drawElipseWindow = new DrawElipseWindow(shapeTextUnit)
					{
						Owner = Window.GetWindow(this),
						WindowStartupLocation = WindowStartupLocation.CenterOwner
					};
					drawElipseWindow.ShowDialog();
				}
				else if(shapeTextUnit.Shape != null && shapeTextUnit.Shape.GetType().Equals(typeof(Polygon)))
				{
					DrawPolygonWindow drawElipseWindow = new DrawPolygonWindow(shapeTextUnit)
					{
						Owner = Window.GetWindow(this),
						WindowStartupLocation = WindowStartupLocation.CenterOwner
					};
					drawElipseWindow.ShowDialog();
				}
				else
				{
					AddTextWindow addTextWindow = new AddTextWindow(shapeTextUnit)
					{
						Owner = Window.GetWindow(this),
						WindowStartupLocation = WindowStartupLocation.CenterOwner
					};
					addTextWindow.ShowDialog();
				}
			}
		}

		private void DrawElipseButton_Click(object sender, RoutedEventArgs e)
		{
			EnableAllMenuButtons();
			DrawElipseButton.IsEnabled = false;
		}

		private void DrawPolygonButton_Click(object sender, RoutedEventArgs e)
		{
			EnableAllMenuButtons();
			DrawPolygonButton.IsEnabled = false;
		}

		private void AddTextButton_Click(object sender, RoutedEventArgs e)
		{
			EnableAllMenuButtons();
			AddTextButton.IsEnabled = false;
		}

		private void ColorsButton_Click(object sender, RoutedEventArgs e)
        {
			ColorsWindow colorsWindow = new ColorsWindow(renderer);
			colorsWindow.ShowDialog();
        }

		private void NetworkCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if(!DrawPolygonButton.IsEnabled && polygonPoints.Count >= 3)
			{
				OpenDrawPolygonDialog();
			}

			polygonPoints.Clear();
			EnableAllMenuButtons();
		}

		private void NetworkCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			Point mousePosition = Mouse.GetPosition(NetworkCanvas);

			if (!DrawElipseButton.IsEnabled)
			{
				DrawElipseButton.IsEnabled = true;
				OpenDrawElipseDialog(mousePosition);
			}
			else if (!DrawPolygonButton.IsEnabled)
			{
				polygonPoints.Add(mousePosition);
			}
			else if (!AddTextButton.IsEnabled)
			{
				AddTextButton.IsEnabled = true;
				OpenAddTextDialog(mousePosition);
			}
		}

		private void NetworkCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if(Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				if (e.Delta > 0)
				{
					CanvasZoomSlider.Value++;
				}
				else
				{
					CanvasZoomSlider.Value--;
				}

				e.Handled = true;
			}
			else if (Keyboard.IsKeyDown(Key.LeftShift))
			{
				if (e.Delta > 0)
				{
					CanvasScrollViewer.ScrollToHorizontalOffset(CanvasScrollViewer.HorizontalOffset - 16);
				}
				else
				{
					CanvasScrollViewer.ScrollToHorizontalOffset(CanvasScrollViewer.HorizontalOffset + 16);
				}

				e.Handled = true;
			}
			else
			{
				if (e.Delta > 0)
				{
					CanvasScrollViewer.ScrollToVerticalOffset(CanvasScrollViewer.VerticalOffset - 16);
				}
				else
				{
					CanvasScrollViewer.ScrollToVerticalOffset(CanvasScrollViewer.VerticalOffset + 16);
				}

				e.Handled = true;
			}
		}

		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			double zoom = e.NewValue;

			if (zoom == 0)
			{
				zoom = 1;
			}
			else if (zoom < 0)
			{
				zoom = (25 + zoom) / 25;
			}
			else
			{
				zoom = 1 + zoom / 15;
			}

			zoom = Math.Max(0.1, Math.Min(10.0, zoom));

			NetworkCanvasBorder.LayoutTransform = new ScaleTransform(zoom, zoom);
		}

		private void UndoButton_Click(object sender, RoutedEventArgs e)
		{
			shapeTextManager.Undo();
		}

		private void RedoButton_Click(object sender, RoutedEventArgs e)
		{
			shapeTextManager.Redo();
		}

		private void ClearButton_Click(object sender, RoutedEventArgs e)
		{
			shapeTextManager.Clear();
		}

		private void ChangeLineColor_CheckBox_Click(object sender, RoutedEventArgs e)
		{
			if (((CheckBox)sender).IsChecked == true)
			{
				renderer.ChangeLineColorBasedOnR();

				ChangeBasedOnChecked();

				return;
			}

			renderer.RevertLineColor();

			ChangeBasedOnChecked();
		}

		private void ChangeSwitchColor_Checkbox_Click(object sender, RoutedEventArgs e)
		{
			if (((CheckBox)sender).IsChecked == true)
			{
				renderer.HighlightSwitchStatus();

				return;
			}

			renderer.RevertEntityColor();
			renderer.RevertLineColor();
			ChangeBasedOnChecked();
		}

		public void ChangeBasedOnChecked()
		{
			if (ChangeLineColor_Checkbox.IsChecked == true)
			{
				renderer.ChangeLineColorBasedOnR();
			}
			
			if (ChangeSwitchColor_Checkbox.IsChecked == true)
			{
				renderer.HighlightSwitchStatus();
			}
			/*
			if (HideInactive_Checkbox.IsChecked == true)
			{
				renderer.HideInactiveNetwork();
			}*/
		}

        private void AnimationButton_Click(object sender, RoutedEventArgs e)
        {
			AnimationWindow animationWindow = new AnimationWindow(renderer);
			animationWindow.Show();
        }

        private void ScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
			string path = "C:\\Users\\Alen\\Desktop";
		    double scaleFactor = 2;

			double width = 2000 * scaleFactor;
			double height = 1240 * scaleFactor;
			
			RenderTargetBitmap bmp = new RenderTargetBitmap((int)width, (int)height, 96, 96, PixelFormats.Default);
			bmp.Render(NetworkCanvas);

			PngBitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(bmp));

			string filePath = path +"\\Slike\\"+ DateTime.Now.ToString("(dd-MM-yyyy hh-mm-ss)") + ".png";

			using (FileStream stream = new FileStream(filePath, FileMode.Create))
			{
				encoder.Save(stream);
			}
		}
    }
}
