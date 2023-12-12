﻿using Grid.Models;

namespace FemProducer.Basises
{
	public interface IBasis
	{
		public Dictionary<string, IList<IList<double>>> GetLocalMatrixes(IList<Node> nodes);
		public IList<double> GetLocalVector(IList<Node> nodes, Func<Node, double> func);
	}
}