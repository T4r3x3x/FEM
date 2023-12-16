namespace Grid.Models
{
	public class FiniteElement
	{
		public int[] NodesIndexes;
		public int formulaNumber;

		public FiniteElement(int[] NodesIndexes, int formulaNumber)
		{
			this.NodesIndexes = NodesIndexes;
			this.formulaNumber = formulaNumber;
		}
	}
}