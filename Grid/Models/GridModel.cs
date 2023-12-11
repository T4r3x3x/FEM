using MathModels;

namespace Grid.Models
{
	public class GridModel
	{
		public readonly IReadOnlyList<double> T;
		public readonly IReadOnlyList<double> Ht;
		public int[][] Boreholes { get; private set; }

		public List<FiniteElement> Elements { get; private set; }
		public List<Node> Nodes { get; private set; }
		public List<BoundaryNode> FirstBoundaryNodes { get; private set; }
		public List<BoundaryNode> SecondBoundaryNodes { get; private set; }
		public List<BoundaryNode> ThirdBoundaryNodes { get; private set; }

		private readonly int nodesInElementCount = 4; //количество узлов в кэ.

		public GridModel(List<FiniteElement> elements, List<Node> nodes,
			List<BoundaryNode> firstBoundaryNodes, List<BoundaryNode> secondBoundaryNodes, List<BoundaryNode> thirdBoundaryNodes,
			List<double> t = null, List<double> ht = null)
		{
			FirstBoundaryNodes = firstBoundaryNodes;
			SecondBoundaryNodes = secondBoundaryNodes;
			ThirdBoundaryNodes = thirdBoundaryNodes;
			Elements = elements;
			Nodes = nodes;
			T = t ?? [0];
			Ht = ht;
		}

		public int TimeLayersCount => T.Count;
		public int ElementsCount => Elements.Count;
		public int NodesCount => Nodes.Count;



		public IList<Node> ElementToNode(FiniteElement element)
		{
			Node[] nodes = new Node[nodesInElementCount];

			for (int i = 0; i < nodesInElementCount; i++)
				nodes[i] = Nodes[element.NodesIndexes[i]];

			return nodes;
		}

		/// <summary>
		/// Возвращает кэ, в котором находится точка.
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public FiniteElement GetElement(Point point)
		{
			for (int i = 0; i < Elements.Count; i++)
				if (IsElementContainPoint(Elements[i], point))
					return Elements[i];

			return null;
		}

		private bool IsElementContainPoint(FiniteElement element, Point point)
		{
			if (Nodes[element.NodesIndexes[0]].X < point.X && point.X < Nodes[element.NodesIndexes[1]].X)
				if (Nodes[element.NodesIndexes[0]].Y < point.Y && point.Y < Nodes[element.NodesIndexes[3]].Y)
					return true;

			return false;
		}
	}
}