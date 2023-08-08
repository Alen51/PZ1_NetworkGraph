using System.Collections.Generic;

namespace PZ1_NetworkGraph.Helpers
{
	class ShapeTextContext
	{
		Stack<IAction> undoStack;

		Stack<IAction> redoStack;

		internal ShapeTextContext()
		{
			undoStack = new Stack<IAction>();
			redoStack = new Stack<IAction>();
		}

		internal void PushUndoStack(IAction action)
		{
			undoStack.Push(action);
		}

		internal void PushRedoStack(IAction action)
		{
			redoStack.Push(action);
		}

		internal IAction PopUndoStack()
		{
			if(undoStack.Count > 0)
			{
				return undoStack.Pop();
			}

			return null;
		}

		internal IAction PopRedoStack()
		{
			if (redoStack.Count > 0)
			{
				return redoStack.Pop();
			}

			return null;
		}

		internal void ClearRedoStack()
		{
			redoStack.Clear();
		}
	}
}
