namespace Grid.Models
{
	public class FiniteElement
	{
		public int[] NodesIndexes;

		public FiniteElement(int[] NodesIndexes)
		{
			this.NodesIndexes = NodesIndexes;
		}
	}
}