namespace FemProducer.Models
{
	public class BoundaryNode
	{
		public readonly Node node;
		public readonly int nodeIndex;

		public BoundaryNode(Node node, int nodeIndex)
		{
			this.node = node;
			this.nodeIndex = nodeIndex;
		}
	}

}