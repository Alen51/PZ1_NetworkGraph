using System.Collections.Generic;

namespace PZ1_NetworkGraph.Helpers
{
	class ClearAction : IAction
	{
		NetworkCanvasContext networkCanvasContext;

		List<ShapeTextUnit> shapeTextUnits;

		internal ClearAction(NetworkCanvasContext networkCanvasContext, List<ShapeTextUnit> shapeTextUnits)
		{
			this.networkCanvasContext = networkCanvasContext;
			this.shapeTextUnits = shapeTextUnits;
		}

		public void DoClear()
		{
			networkCanvasContext.RemoveAllShapeTextUnitsFromCanvas();
		}

		public void Undo()
		{
			foreach(ShapeTextUnit shapeTextUnit in shapeTextUnits)
			{
				networkCanvasContext.AddShapeTextUnitToCanvas(shapeTextUnit);
			}
		}

		public void Redo()
		{
			foreach (ShapeTextUnit shapeTextUnit in shapeTextUnits)
			{
				networkCanvasContext.RemoveShapeTextUnitFromCanvas(shapeTextUnit);
			}
		}
	}
}
