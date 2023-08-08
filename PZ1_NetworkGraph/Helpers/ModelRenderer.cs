using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Models;
using PZ1_NetworkGraph.BFS;

namespace PZ1_NetworkGraph.Helpers
{
	internal class ModelRenderer
	{
		private Dictionary<long, Shape> addedShapesPowerEntities;

		private Storyboard currentStoryboard;

		private Tuple<Shape, Shape> connectedShapes;

		private List<Line> lines;

		private List<LinijaVoda> listaSvihLinija=new List<LinijaVoda>();//BFS

		public Dictionary<Polyline, LineEntity> lineDictionary = new Dictionary<Polyline, LineEntity>();//BFS

		bool iscrtaoPresek = false;

		private Dictionary<long, List<Line>> coloredLines { get; set; } = new Dictionary<long, List<Line>>();

		private double animationSize = 3;

		private double animationTime = 1;

		private Color animationColor = Colors.SpringGreen;

		internal ModelRenderer(Canvas canvas)
		{
			NetworkCanvas = canvas;
			Loader = new ModelLoader();
			addedShapesPowerEntities = new Dictionary<long, Shape>();
			lines = new List<Line>();

			Loader.LoadModelFromFile();

			AddEntitiesToAddedShapePowerEntitiesDictionary();
			//DrawLines();
			DrawLinesBFS();
			DrawIntersect();
			DrawEntities();
		}

		internal Canvas NetworkCanvas { get; set; }

		internal ModelLoader Loader { get; set; }


		public void DrawLinesBFS()
		{

			foreach (LineEntity line in Loader.Lines)
			{
				double firstX = -1;
				double firstY = -1;
				double secondX = -1;
				double secondY = -1;

				foreach (var ent in Loader.Entities)
				{
					if (line.FirstEnd == ent.Id)
					{
						firstX = ent.X;
						firstY = ent.Y;
					}
					if (line.SecondEnd == ent.Id)
					{
						secondX = ent.X;
						secondY = ent.Y;
					}
				}

				if (firstX == -1 || firstY == -1 || secondX == -1 || secondY == -1)
				{
					continue;
				}

				line.PocetakX = firstX;
				line.PocetakY = firstY;
				line.KrajX = secondX;
				line.KrajY = secondY;

				System.Windows.Point start = new System.Windows.Point(firstX, firstY);
				System.Windows.Point end = new System.Windows.Point(secondX, secondY);

				BFSProlaz(line, start, end);


			}

			foreach (LineEntity line in Loader.Lines)
			{
				double firstX = -1;
				double firstY = -1;
				double secondX = -1;
				double secondY = -1;

				foreach (var ent in Loader.Entities)
				{
					if (line.FirstEnd == ent.Id)
					{
						firstX = ent.X;
						firstY = ent.Y;
					}
					if (line.SecondEnd == ent.Id)
					{
						secondX = ent.X;
						secondY = ent.Y;
					}
				}

				if (firstX == -1 || firstY == -1 || secondX == -1 || secondY == -1)
				{
					continue;
				}

				System.Windows.Point start = new System.Windows.Point(firstX, firstY);
				System.Windows.Point end = new System.Windows.Point(secondX, secondY);

				if (line.Prolaz != "1")
				{
					BFSProlaz2(line, start, end);
				}

				line.Prolaz = "0";
			}
			iscrtaoPresek = true;
		}

		public void BFSProlaz(LineEntity line, System.Windows.Point start, System.Windows.Point end)
        {
			PozicijaPolja pocetak = new PozicijaPolja((int)line.PocetakX, (int)line.PocetakY);
			PozicijaPolja kraj = new PozicijaPolja((int)line.KrajX, (int)line.KrajY);
			PozicijaPolja[,] prev = BFSPath.BFSPronadji(line, BFSprom.BFSlinije);
			List<PozicijaPolja> putanja = BFSPath.RekonstruisanjePutanje(pocetak, kraj, prev);

			Shape firstShapePowerEntity = addedShapesPowerEntities[line.FirstEnd];
			Shape secondShapePowerEntity = addedShapesPowerEntities[line.SecondEnd];
			Tuple<Shape, Shape> connectedShapePowerEntities = new Tuple<Shape, Shape>(firstShapePowerEntity, secondShapePowerEntity);


			if (putanja == null)
			{
				line.Prolaz = "0";
			}
			else
			{
				line.Prolaz = "1";

				System.Windows.Point p1 = new System.Windows.Point(pocetak.PozX+0.5 , pocetak.PozY+0.5 ); //(pocetak.PozY * 3 + 1, -pocetak.PozX * 3 + 910 - 1)
				System.Windows.Point p3 = new System.Windows.Point(kraj.PozX+0.5 , kraj.PozY+0.5 );//(kraj.PozY * 3 + 1, -kraj.PozX * 3 + 910 - 1)
				putanja.Remove(pocetak);
				putanja.Remove(kraj);

				if (start.X != end.X)
				{
					Polyline polyline = new Polyline();
					polyline.Stroke = Brushes.Black;
					polyline.StrokeThickness = 0.25;
					polyline.ToolTip = new ToolTip
					{
						Content = "ID: " + line.Id + "\nName: " + line.Name,
						Foreground = Brushes.Black,
						Background = Brushes.White,
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(2),
						IsEnabled = true
					};
					polyline.Tag = connectedShapePowerEntities;

					if (line.IsUnderground == false)
					{
						polyline.Stroke = GetLineColor(line);


					}

					polyline.MouseRightButtonDown += Line_MouseRightButtonDown; ;

					PointCollection points = new PointCollection();
					points.Add(p1);
					foreach (PozicijaPolja zauzeto in putanja)
					{
						BFSprom.BFSlinije[zauzeto.PozX, zauzeto.PozY] = 'X';
						points.Add(new System.Windows.Point(zauzeto.PozX + 0.5, zauzeto.PozY + 0.5));//(zauzeto.PozY * 3 + 1, -zauzeto.PozX * 3 + 910 - 1)
					}
					points.Add(p3);
					polyline.Points = points;

					for (int i = 0; i < points.Count - 1; i++)
					{
						LinijaVoda novaLinija = new LinijaVoda();
						novaLinija.Id = line.Id;
						novaLinija.FirstEnd = points[i];
						novaLinija.SecondEnd = points[i + 1];
						listaSvihLinija.Add(novaLinija);
					}

					NetworkCanvas.Children.Add(polyline);
					lineDictionary.Add(polyline, line);
				}
			}
		}

		public void BFSProlaz2(LineEntity line, System.Windows.Point start, System.Windows.Point end)
        {
			PozicijaPolja pocetak = new PozicijaPolja((int)line.PocetakX, (int)line.PocetakY);
			PozicijaPolja kraj = new PozicijaPolja((int)line.KrajX, (int)line.KrajY);
			PozicijaPolja[,] prev = BFSPath.BFSPronadji(line, BFSprom.BFSlinije2);
			List<PozicijaPolja> putanja = BFSPath.RekonstruisanjePutanje(pocetak, kraj, prev);

			Shape firstShapePowerEntity = addedShapesPowerEntities[line.FirstEnd];
			Shape secondShapePowerEntity = addedShapesPowerEntities[line.SecondEnd];
			Tuple<Shape, Shape> connectedShapePowerEntities = new Tuple<Shape, Shape>(firstShapePowerEntity, secondShapePowerEntity);


			if (putanja == null)
			{
				line.Prolaz = "0";
			}
			else
			{
				line.Prolaz = "2";

				System.Windows.Point p1 = new System.Windows.Point(pocetak.PozX + 0.5, pocetak.PozY + 0.5);//(pocetak.PozY * 3 + 1, -pocetak.PozX * 3 + 910 - 1)
				System.Windows.Point p3 = new System.Windows.Point(kraj.PozX + 0.5, kraj.PozY + 0.5);//(kraj.PozY * 3 + 1, -kraj.PozX * 3 + 910 - 1)
				putanja.Remove(pocetak);
				putanja.Remove(kraj);

				if (start.X != end.X)
				{
					Polyline polyline = new Polyline();
					polyline.Stroke = Brushes.Black;
					polyline.StrokeThickness = 0.25;
					polyline.ToolTip = new ToolTip
					{
						Content = "ID: " + line.Id + "\nName: " + line.Name,
						Foreground = Brushes.Black,
						Background = Brushes.White,
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(2),
						IsEnabled = true
					};
					polyline.Tag = connectedShapePowerEntities;

					if (line.IsUnderground == false)
					{
						polyline.Stroke = GetLineColor(line);
						

					}

					polyline.MouseRightButtonDown += Line_MouseRightButtonDown;

					PointCollection points = new PointCollection();
					points.Add(p1);
					foreach (PozicijaPolja zauzeto in putanja)
					{
						points.Add(new System.Windows.Point(zauzeto.PozX + 0.5, zauzeto.PozY + 0.5));//(zauzeto.PozY * 3 + 1, -zauzeto.PozX * 3 + 910 - 1)
					}
					points.Add(p3);
					polyline.Points = points;

					List<LinijaVoda> listaNovihLinija = new List<LinijaVoda>();
					for (int i = 0; i < points.Count - 1; i++)
					{
						LinijaVoda novaLinija = new LinijaVoda();
						novaLinija.Id = line.Id;
						novaLinija.FirstEnd = points[i];
						novaLinija.SecondEnd = points[i + 1];
						listaSvihLinija.Add(novaLinija);
						listaNovihLinija.Add(novaLinija);
					}

					/*
					foreach (var novaLinijaLista in listaNovihLinija)
					{
						foreach (var linija in listaSvihLinija)
						{
							if (linija.Id == novaLinijaLista.Id)
								continue;
							else
							{
								if (PostojiPresjek(linija.FirstEnd, linija.SecondEnd, novaLinijaLista.FirstEnd, novaLinijaLista.SecondEnd))
								{
									PronalazakPresjeka(linija.FirstEnd, linija.SecondEnd, novaLinijaLista.FirstEnd, novaLinijaLista.SecondEnd);
								}
							}
						}
					}*/

					NetworkCanvas.Children.Add(polyline);
					lineDictionary.Add(polyline, line);
				}
			}
		}



		public void DrawLines()
		{
			List<Tuple<long, long>> usedEntities = new List<Tuple<long, long>>();

			foreach (LineEntity line in Loader.Lines)
			{
				bool alreadyConnected = usedEntities.Contains(new Tuple<long, long>(line.FirstEnd, line.SecondEnd)) ||
									usedEntities.Contains(new Tuple<long, long>(line.SecondEnd, line.FirstEnd));

				if (alreadyConnected)
				{
					continue;
				}

				PowerEntity firstPowerEntity = Loader.Entities.Find(x => x.Id == line.FirstEnd);
				PowerEntity secondPowerEntity = Loader.Entities.Find(x => x.Id == line.SecondEnd);

				if (firstPowerEntity == null || secondPowerEntity == null)
				{
					continue;
				}

				Shape firstShapePowerEntity = addedShapesPowerEntities[line.FirstEnd];
				Shape secondShapePowerEntity = addedShapesPowerEntities[line.SecondEnd];
				Tuple<Shape, Shape> connectedShapePowerEntities = new Tuple<Shape, Shape>(firstShapePowerEntity, secondShapePowerEntity);
				


				Line firstLine = new Line()
				{
					X1 = firstPowerEntity.X + 0.4,//1.5
					Y1 = firstPowerEntity.Y + 0.4,
					X2 = firstPowerEntity.X + 0.4,
					Y2 = secondPowerEntity.Y + 0.4,
					Stroke = Brushes.Black,
					StrokeThickness = 0.25,
					StrokeStartLineCap = PenLineCap.Round,
					ToolTip = new ToolTip
					{
						Content = "ID: " + line.Id + "\nName: " + line.Name,
						Foreground = Brushes.Black,
						Background = Brushes.White,
						BorderBrush = Brushes.Black,
						BorderThickness = new Thickness(2),
						IsEnabled = true
					},
					Tag = connectedShapePowerEntities,
					
				};

				Line secondLine = new Line()
				{
					X1 = firstPowerEntity.X + 0.4,
					Y1 = secondPowerEntity.Y + 0.4,
					X2 = secondPowerEntity.X + 0.4,
					Y2 = secondPowerEntity.Y + 0.4,
					Stroke = Brushes.Black,
					StrokeThickness = 0.25,
					StrokeStartLineCap = PenLineCap.Round,
					ToolTip = firstLine.ToolTip,
					Tag = connectedShapePowerEntities
				};

                if (line.IsUnderground == false)
                {
					firstLine.Stroke = GetLineColor(line);
					secondLine.Stroke = GetLineColor(line);

				}

				firstLine.MouseRightButtonDown += Line_MouseRightButtonDown;
				secondLine.MouseRightButtonDown += Line_MouseRightButtonDown;

				lines.Add(firstLine);
				lines.Add(secondLine);
				List<Line> l1 = new List<Line>();
				l1.Add(firstLine);
				l1.Add(secondLine);
				coloredLines.Add(line.Id, l1);

				NetworkCanvas.Children.Add(firstLine);
				NetworkCanvas.Children.Add(secondLine);

				usedEntities.Add(new Tuple<long, long>(line.FirstEnd, line.SecondEnd));
			}
		}

		public void AddEntitiesToAddedShapePowerEntitiesDictionary()
		{
			foreach (PowerEntity entity in Loader.Entities)
			{
				Rectangle shape = new Rectangle() { Width = 0.8, Height = 0.8 };
				shape.Tag = entity;
				addedShapesPowerEntities[entity.Id] = shape;
			}
		}

		public void DrawEntities()
		{
			foreach (PowerEntity entity in Loader.Entities)
			{
				Rectangle point = (Rectangle)addedShapesPowerEntities[entity.Id];

				point.ToolTip = new ToolTip
				{
					IsEnabled = true,
					Foreground = Brushes.Black,
					Background = Brushes.White,
					BorderBrush = Brushes.Black,
					BorderThickness = new Thickness(0.5)
				};

				if (entity.ConnectionsCount >= 0 && entity.ConnectionsCount < 3)
				{
					point.Fill = Brushes.PaleVioletRed;
				}
				else if (entity.ConnectionsCount >= 3 && entity.ConnectionsCount < 5)
				{
					point.Fill = Brushes.Red;
				}
				else if (entity.ConnectionsCount >5)
				{
					point.Fill = Brushes.DarkRed;
				}

				if (entity.GetType().Equals(typeof(NodeEntity)))
				{
					

					((ToolTip)point.ToolTip).Content = "ID: " + entity.Id + "\nName: " + entity.Name + "\nType: Node";
				}
				else if (entity.GetType().Equals(typeof(SwitchEntity)))
				{
					

					((ToolTip)point.ToolTip).Content = "ID: " + entity.Id + "\nName: " + entity.Name + "\nType: Switch";
				}
				else if (entity.GetType().Equals(typeof(SubstationEntity)))
				{
					

					((ToolTip)point.ToolTip).Content = "ID: " + entity.Id + "\nName: " + entity.Name + "\nType: Substation";
				}

				Canvas.SetTop(point, entity.Y);
				Canvas.SetLeft(point, entity.X);

				NetworkCanvas.Children.Add(point);
			}
		}

		private void DrawIntersect()
		{
			foreach(var line1 in lines)
			{
				var index = lines.IndexOf(line1);
				
				for (int i = 0; i < index; i++) 
				{
					var line2 = lines[i];

					var line1Start = new System.Windows.Point(line1.X1, line1.Y1);
					var line1End = new System.Windows.Point(line1.X2 , line1.Y2);
					var line2Start = new System.Windows.Point(line2.X1, line2.Y1);
					var line2End = new System.Windows.Point(line2.X2, line2.Y2);

					double a1 = line1End.Y - line1Start.Y;
					double b1 = line1Start.X - line1End.X;
					double c1 = a1 * line1Start.X + b1 * line1Start.Y;

					double a2 = line2End.Y - line2Start.Y;
					double b2 = line2Start.X - line2End.X;
					double c2 = a2 * line2Start.X + b2 * line2Start.Y;

					double det = a1 * b2 - a2 * b1;
					if (det != 0)
					{
						double x = (b2 * c1 - b1 * c2) / det;
						double y = (a1 * c2 - a2 * c1) / det;
						System.Windows.Point intersection = new System.Windows.Point(x, y);

						if (IsPointOnLine(line1Start, line1End, intersection) && IsPointOnLine(line2Start, line2End, intersection))
						{
							if (((line2Start.X == line1End.X) && (line2Start.Y == line1End.Y)) ||
								((line1Start.X == line2End.X) && (line1Start.Y == line2End.Y)))
							{
								break;
							}

							Rectangle rectangle = new Rectangle()
							{
								Fill = new SolidColorBrush(Colors.Yellow),
								Width = 0.25,
								Height = 0.25
							};

							Canvas.SetLeft(rectangle, intersection.X - 0.125);
							Canvas.SetTop(rectangle, intersection.Y - 0.125);
							
							NetworkCanvas.Children.Add(rectangle);
						}
					}
				}
			}
		}

		private bool IsPointOnLine(System.Windows.Point lineStart, System.Windows.Point lineEnd, System.Windows.Point point)
		{
			double dxc = point.X - lineStart.X;
			double dyc = point.Y - lineStart.Y;
			double dxl = lineEnd.X - lineStart.X;
			double dyl = lineEnd.Y - lineStart.Y;

			double cross = dxc * dyl - dyc * dxl;
			if (cross != 0)
			{
				return false;
			}

			if (Math.Abs(dxl) >= Math.Abs(dyl))
			{
				return dxl > 0 ? lineStart.X <= point.X && point.X <= lineEnd.X : lineEnd.X <= point.X && point.X <= lineStart.X;
			}
			else
			{
				return dyl > 0 ? lineStart.Y <= point.Y && point.Y <= lineEnd.Y : lineEnd.Y <= point.Y && point.Y <= lineStart.Y;
			}
		}

		private void Line_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (currentStoryboard != null)
			{
				currentStoryboard.AutoReverse = true;
				currentStoryboard.Begin();
				currentStoryboard.Seek(new TimeSpan(0, 0, 1));

				Canvas.SetZIndex(connectedShapes.Item1, 0);
				Canvas.SetZIndex(connectedShapes.Item2, 0);

				currentStoryboard = null;
			}

			Line line = (Line)sender;
			
			Shape firstShape = ((Tuple<Shape, Shape>)line.Tag).Item1;
			Shape secondShape = ((Tuple<Shape, Shape>)line.Tag).Item2;
			

			

			if (connectedShapes?.Item1 == firstShape && connectedShapes?.Item2 == secondShape)
			{
				connectedShapes = null;
				return;
			}

			

			ScaleTransform firstScaleTransform = new ScaleTransform(1, 1, firstShape.Width / 2, firstShape.Height / 2);
			firstShape.RenderTransform = firstScaleTransform;
			ScaleTransform secondScaleTransform = new ScaleTransform(1, 1, secondShape.Width / 2, secondShape.Height / 2);
			secondShape.RenderTransform = secondScaleTransform;

			//Animation for ScaleX of the firstShape
			DoubleAnimation sizeAnimationFirstShapeX = new DoubleAnimation();
			sizeAnimationFirstShapeX.From = 1;
			sizeAnimationFirstShapeX.To = animationSize;
			sizeAnimationFirstShapeX.Duration = TimeSpan.FromSeconds(animationTime);

			Storyboard.SetTarget(sizeAnimationFirstShapeX, firstShape);
			Storyboard.SetTargetProperty(sizeAnimationFirstShapeX, new PropertyPath("RenderTransform.ScaleX"));

			//Animation for ScaleY of the firstShape
			DoubleAnimation sizeAnimationFirstShapeY = new DoubleAnimation();
			sizeAnimationFirstShapeY.From = 1;
			sizeAnimationFirstShapeY.To = animationSize;
			sizeAnimationFirstShapeY.Duration = TimeSpan.FromSeconds(animationTime);

			Storyboard.SetTarget(sizeAnimationFirstShapeY, firstShape);
			Storyboard.SetTargetProperty(sizeAnimationFirstShapeY, new PropertyPath("RenderTransform.ScaleY"));

			//Animation for ScaleX of the secondShape
			DoubleAnimation sizeAnimationSecondShapeX = new DoubleAnimation();
			sizeAnimationSecondShapeX.From = 1;
			sizeAnimationSecondShapeX.To = animationSize;
			sizeAnimationSecondShapeX.Duration = TimeSpan.FromSeconds(animationTime);

			Storyboard.SetTarget(sizeAnimationSecondShapeX, secondShape);
			Storyboard.SetTargetProperty(sizeAnimationSecondShapeX, new PropertyPath("RenderTransform.ScaleX"));

			//Animation for ScaleY of the secondShape
			DoubleAnimation sizeAnimationSecondShapeY = new DoubleAnimation();
			sizeAnimationSecondShapeY.From = 1;
			sizeAnimationSecondShapeY.To = animationSize;
			sizeAnimationSecondShapeY.Duration = TimeSpan.FromSeconds(animationTime);

			Storyboard.SetTarget(sizeAnimationSecondShapeY, secondShape);
			Storyboard.SetTargetProperty(sizeAnimationSecondShapeY, new PropertyPath("RenderTransform.ScaleY"));

			//Color animations
			ColorAnimation colorAnimationFirstShape = new ColorAnimation();
			Brush firstBrushColor = firstShape.Fill;
			SolidColorBrush firstSolidColor = (SolidColorBrush)firstBrushColor;
			colorAnimationFirstShape.From = firstSolidColor.Color;
				
			colorAnimationFirstShape.To = animationColor;
			colorAnimationFirstShape.Duration = TimeSpan.FromSeconds(animationTime);

			Storyboard.SetTarget(colorAnimationFirstShape, firstShape);
			Storyboard.SetTargetProperty(colorAnimationFirstShape, new PropertyPath("(Rectangle.Fill).(SolidColorBrush.Color)"));

			ColorAnimation colorAnimationSecondShape = new ColorAnimation();
			Brush secondBrushColor = secondShape.Fill;
			SolidColorBrush secondSolidColor = (SolidColorBrush)secondBrushColor;
			
			colorAnimationSecondShape.From = secondSolidColor.Color;
			colorAnimationSecondShape.To = animationColor;
			colorAnimationSecondShape.Duration = TimeSpan.FromSeconds(animationTime);

			Storyboard.SetTarget(colorAnimationSecondShape, secondShape);
			Storyboard.SetTargetProperty(colorAnimationSecondShape, new PropertyPath("(Rectangle.Fill).(SolidColorBrush.Color)"));

			Storyboard storyboard = new Storyboard();
			storyboard.Children.Add(colorAnimationFirstShape);
			storyboard.Children.Add(colorAnimationSecondShape);
			storyboard.Children.Add(sizeAnimationFirstShapeX);
			storyboard.Children.Add(sizeAnimationFirstShapeY);
			storyboard.Children.Add(sizeAnimationSecondShapeX);
			storyboard.Children.Add(sizeAnimationSecondShapeY);

			Canvas.SetZIndex(firstShape, 1);
			Canvas.SetZIndex(secondShape, 1);

			connectedShapes = new Tuple<Shape, Shape>(firstShape, secondShape);

			storyboard.Begin();

			currentStoryboard = storyboard;
		}
		
		Color GetColorForType(PowerEntity type)
		{
			if (type.GetType().Equals(typeof(NodeEntity)))
			{
				return Colors.Red;
			}
			else if(type.GetType().Equals(typeof(SwitchEntity)))
			{
				return Colors.Green;
			}
			else if(type.GetType().Equals(typeof(SubstationEntity)))
			{
				return Colors.Blue;
			}

			return Colors.Black;
		}

		public void ChangeLineColorBasedOnR()
		{
			foreach (var line in Loader.Lines)
			{
				if (line.R < 1)
				{
					try
					{
						coloredLines[line.Id].ForEach(x => x.Stroke = Brushes.Red);
                    }
                    catch { }
				}
			}
			foreach (var line in Loader.Lines)
			{
				if (line.R >= 1 && line.R<=2)
				{
					try
					{
						coloredLines[line.Id].ForEach(x => x.Stroke = Brushes.Orange);
                    }
                    catch { }
				}
			}
			foreach (var line in Loader.Lines)
			{
				if (line.R > 1)
				{
					try
					{
						coloredLines[line.Id].ForEach(x => x.Stroke = Brushes.Yellow);
                    }
                    catch { }
				}
			}
			
		}

		public void HighlightSwitchStatus()
		{
			foreach (var entity in Loader.Entities)
			{
				if (entity is SwitchEntity)
				{
					if (((SwitchEntity)entity).Status == "Closed")
					{
						addedShapesPowerEntities[entity.Id].Fill = Brushes.Blue;
						foreach (var line in Loader.Lines)
						{
							if (line.FirstEnd==entity.Id || line.SecondEnd==entity.Id)
							{
								try
								{
									coloredLines[line.Id].ForEach(x => x.Stroke = Brushes.Blue);
								}
								catch { }
							}
						}
					}
					else if (((SwitchEntity)entity).Status == "Open")
					{
						addedShapesPowerEntities[entity.Id].Fill = Brushes.Gold;
						foreach (var line in Loader.Lines)
						{
							if (line.FirstEnd == entity.Id || line.SecondEnd == entity.Id)
							{
								try
								{
									coloredLines[line.Id].ForEach(x => x.Stroke = Brushes.Gold);
								}
								catch { }
							}
						}
					}
				}
                else
                {
					addedShapesPowerEntities[entity.Id].Fill = Brushes.Transparent;
				}
			}

			


		}

		public void RevertLineColor()
		{
			foreach (var line in Loader.Lines)
			{
				try
				{
					coloredLines[line.Id].ForEach(x => x.Stroke = GetLineColor(line));
                }
                catch { }
			}
		}

		public void RevertEntityColor()
        {
			foreach(var entity in Loader.Entities)
            {
				if (entity.ConnectionsCount >= 0 && entity.ConnectionsCount < 3)
				{
					addedShapesPowerEntities[entity.Id].Fill = Brushes.PaleVioletRed;
				}
				else if (entity.ConnectionsCount >= 3 && entity.ConnectionsCount < 5)
				{
					addedShapesPowerEntities[entity.Id].Fill = Brushes.Red;
				}
				else if (entity.ConnectionsCount > 5)
				{
					addedShapesPowerEntities[entity.Id].Fill = Brushes.DarkRed;
				}
			}
			
        }

		public void changeEntityGroupColors(string changedEntity,SolidColorBrush brush)
        {
			if(changedEntity == "NodeEntity")
            {
				foreach(var entity in Loader.Entities)
                {
					if(entity is NodeEntity)
                    {
						addedShapesPowerEntities[entity.Id].Fill = brush;
                    }
                }
            }
			else if(changedEntity == "SubstationEntity")
            {
				foreach (var entity in Loader.Entities)
				{
					if (entity is SubstationEntity)
					{
						addedShapesPowerEntities[entity.Id].Fill = brush;
					}
				}
			}
			else if(changedEntity == "SwitchEntity")
            {
				foreach (var entity in Loader.Entities)
				{
					if (entity is SwitchEntity)
					{
						addedShapesPowerEntities[entity.Id].Fill = brush;
					}
				}
			}


		}

		public void changeAnimation(double animationSize, double animationTime, Color animationColor)
        {
			this.animationSize = animationSize;
			this.animationTime = animationTime;
			this.animationColor = animationColor;
        }

		public void resetAnimation()
        {
			this.animationSize = 1;
			this.animationTime = 1;
			this.animationColor = Colors.SpringGreen;
		}


		private SolidColorBrush GetLineColor(LineEntity line)
		{
			if (line.ConductorMaterial == "Steel")
			{
				return new SolidColorBrush(Colors.Violet);
			}
			else if (line.ConductorMaterial == "Acsr")
			{
				return new SolidColorBrush(Colors.DimGray);
			}
			else if (line.ConductorMaterial == "Copper")
			{
				return new SolidColorBrush(Colors.Brown);
			}

			return new SolidColorBrush(Colors.LightSeaGreen);
		}


	}
}
