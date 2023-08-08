namespace PZ1_NetworkGraph.Helpers
{
	interface IAction
	{
		void Undo();

		void Redo();
	}
}
