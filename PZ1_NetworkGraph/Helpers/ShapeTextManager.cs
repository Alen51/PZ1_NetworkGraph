using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace PZ1_NetworkGraph.Helpers
{
	public class ShapeTextManager
	{
		private NetworkCanvasContext networkCanvasContext;

		private ShapeTextContext shapeTextContext;

		internal ShapeTextManager(Canvas networkCanvas)
		{
			networkCanvasContext = new NetworkCanvasContext(networkCanvas);
			shapeTextContext = new ShapeTextContext();
		}

		internal void Add(ShapeTextUnit unit)
		{
			AddAction addAction = new AddAction(unit, networkCanvasContext);
			addAction.DoAdd();

			shapeTextContext.PushUndoStack(addAction);
			shapeTextContext.ClearRedoStack();
		}

		internal void Clear()
		{
			if (networkCanvasContext.AllCanvasShapeTextChildrenEmpty())
			{
				return;
			}

			List<ShapeTextUnit> shapeTextUnits = networkCanvasContext.GetAllShapeTextUnitsFromCanvas();
			ClearAction clearAction = new ClearAction(networkCanvasContext, shapeTextUnits);
			clearAction.DoClear();

			shapeTextContext.PushUndoStack(clearAction);
			shapeTextContext.ClearRedoStack();
		}

		internal void Undo()
		{
			IAction action = shapeTextContext.PopUndoStack();
			if (action == null)
			{
				return;
			}

			shapeTextContext.PushRedoStack(action);
			action.Undo();
		}

		internal void Redo()
		{
			IAction action = shapeTextContext.PopRedoStack();
			if (action == null)
			{
				return;
			}

			shapeTextContext.PushUndoStack(action);
			action.Redo();
		}

		internal ShapeTextUnit FindShapeTextUnitByShape(Shape shape)
		{
			return networkCanvasContext.FindShapeTextUnitByShape(shape);
		}

		internal ShapeTextUnit FindShapeTextUnitByText(TextBlock textBlock)
		{
			return networkCanvasContext.FindShapeTextUnitByText(textBlock);
		}
	}
}
