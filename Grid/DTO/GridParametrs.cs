﻿namespace FemProducer.DTO
{
	internal record GridParametrs(int n, double[] XW, int m, double[] YW, int[][] areas, double[] q, List<int> xAreaLenghtes, List<int> yAreaLenghtes);
}
