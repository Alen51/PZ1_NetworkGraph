using System.Windows.Controls;
using System.Windows.Shapes;

namespace PZ1_NetworkGraph.Helpers
{
	internal class ShapeTextUnit
	{
		internal ShapeTextUnit()
		{
		}

		internal ShapeTextUnit(TextBlock textBlock)
		{
			TextBlock = textBlock;
		}

		internal ShapeTextUnit(Shape shape)
		{
			Shape = shape;
		}

		internal ShapeTextUnit(Shape shape, TextBlock textBlock)
		{
			Shape = shape;
			TextBlock = textBlock;
		}

		internal TextBlock TextBlock { get; set; }

		internal Shape Shape { get; set; }
	}
}
