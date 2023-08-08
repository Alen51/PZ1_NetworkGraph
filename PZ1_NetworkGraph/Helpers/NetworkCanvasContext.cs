using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PZ1_NetworkGraph.Helpers
{
	internal class NetworkCanvasContext
	{
		private Canvas networkCanvas;

		private List<ShapeTextUnit> allCanvasShapeTextChildren;

		internal NetworkCanvasContext(Canvas networkCanvas)
		{
			this.networkCanvas = networkCanvas;
			allCanvasShapeTextChildren = new List<ShapeTextUnit>();
		}

		internal bool AllCanvasShapeTextChildrenEmpty()
		{
			return !allCanvasShapeTextChildren.Any();
		}

		internal void AddShapeTextUnitToCanvas(ShapeTextUnit shapeTextUnit)
		{
			allCanvasShapeTextChildren.Add(shapeTextUnit);

			if (shapeTextUnit.Shape != null)
			{
				networkCanvas.Children.Add(shapeTextUnit.Shape);
			}

			if (shapeTextUnit.TextBlock != null)
			{
				networkCanvas.Children.Add(shapeTextUnit.TextBlock);
			}
		}

		internal void RemoveShapeTextUnitFromCanvas(ShapeTextUnit shapeTextUnit)
		{
			allCanvasShapeTextChildren.Remove(shapeTextUnit);

			if (shapeTextUnit.Shape != null)
			{
				networkCanvas.Children.Remove(shapeTextUnit.Shape);
			}

			if (shapeTextUnit.TextBlock != null)
			{
				networkCanvas.Children.Remove(shapeTextUnit.TextBlock);
			}
		}

		internal List<ShapeTextUnit> GetAllShapeTextUnitsFromCanvas()
		{
			return new List<ShapeTextUnit>(allCanvasShapeTextChildren);
		}

		internal void RemoveAllShapeTextUnitsFromCanvas()
		{
			foreach (ShapeTextUnit shapeTextUnit in allCanvasShapeTextChildren)
			{
				if (shapeTextUnit.Shape != null)
				{
					networkCanvas.Children.Remove(shapeTextUnit.Shape);
				}

				if (shapeTextUnit.TextBlock != null)
				{
					networkCanvas.Children.Remove(shapeTextUnit.TextBlock);
				}
			}

			allCanvasShapeTextChildren.Clear();
		}

		internal void RemoveShapeTextUnitsFromCanvas(List<ShapeTextUnit> shapeTextUnits)
		{
			foreach (ShapeTextUnit shapeTextUnit in shapeTextUnits)
			{
				if (shapeTextUnit.Shape != null)
				{
					networkCanvas.Children.Remove(shapeTextUnit.Shape);
				}

				if (shapeTextUnit.TextBlock != null)
				{
					networkCanvas.Children.Remove(shapeTextUnit.TextBlock);
				}
			}

			allCanvasShapeTextChildren.RemoveAll(x => shapeTextUnits.Contains(x));
		}

		internal ShapeTextUnit FindShapeTextUnitByShape(Shape shape)
		{
			return allCanvasShapeTextChildren.FirstOrDefault(x => x.Shape == shape);
		}

		internal ShapeTextUnit FindShapeTextUnitByText(TextBlock textBlock)
		{
			return allCanvasShapeTextChildren.FirstOrDefault(x => x.TextBlock == textBlock);
		}
	}
}
