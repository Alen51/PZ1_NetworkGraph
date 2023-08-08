namespace PZ1_NetworkGraph.Helpers
{
	class AddAction : IAction
	{
		private NetworkCanvasContext networkCanvasContext;

		private ShapeTextUnit shapeTextUnit;

		internal AddAction(ShapeTextUnit shapeTextUnit, NetworkCanvasContext networkCanvasContext)
		{
			this.networkCanvasContext = networkCanvasContext;
			this.shapeTextUnit = shapeTextUnit;
		}

		public void DoAdd()
		{
			networkCanvasContext.AddShapeTextUnitToCanvas(shapeTextUnit);
		}

		public void Undo()
		{
			networkCanvasContext.RemoveShapeTextUnitFromCanvas(shapeTextUnit);
		}

		public void Redo()
		{
			DoAdd();
		}
	}
}
