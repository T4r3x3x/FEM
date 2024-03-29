﻿using MathModels;

namespace Grid.Models
{
	public class GridModel
	{
		public readonly IReadOnlyList<double> T;
		public readonly IReadOnlyList<double> Ht;

		private readonly int nodesInElementCount = 4; //количество узлов в кэ.

		public GridModel(IList<FiniteElement> elements, IList<Node> nodes,
			IEnumerable<int> firstBoundaryNodes, IEnumerable<int> secondBoundaryNodes, IEnumerable<int> thirdBoundaryNodes, int xCount, int yCount, double[][] subdomains,
			double[] t = null, List<double> ht = null)
		{
			FirstBoundaryNodes = firstBoundaryNodes;
			SecondBoundaryNodes = secondBoundaryNodes;
			ThirdBoundaryNodes = thirdBoundaryNodes;
			Elements = elements;
			Nodes = nodes;
			T = t ?? [0];
			Ht = ht;
			XCount = xCount;
			YCount = yCount;
			Subdomains = subdomains;
		}

		public double[][] Subdomains { get; }
		public int[][] Boreholes { get; private set; }
		public IList<FiniteElement> Elements { get; private set; }
		public IList<Node> Nodes { get; private set; }
		public IEnumerable<int> FirstBoundaryNodes { get; private set; }
		public IEnumerable<int> SecondBoundaryNodes { get; private set; }
		public IEnumerable<int> ThirdBoundaryNodes { get; private set; }
		public int TimeLayersCount => T.Count;
		public int ElementsCount => Elements.Count;
		public int NodesCount => Nodes.Count;
		public int XCount { get; }
		public int YCount { get; }
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