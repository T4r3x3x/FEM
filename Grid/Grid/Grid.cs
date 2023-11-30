namespace ReaserchPaper.Grid
{
	public class Grid
	{
		public double[] T;
		public double[] X, Y, Hy, Hx, Ht;
		public int[][] Boreholes;

		private int[] _IX, _IY;
		private int[][] _areas;
		private int _n, _m;

		public Grid(double[] t, double[] x, double[] y, double[] hy, double[] hx, double[] ht, int[][] boreholes, int[] iX, int[] iY, int[][] areas, int n, int m)
		{
			T = t;
			X = x;
			Y = y;
			Hy = hy;
			Hx = hx;
			Ht = ht;
			Boreholes = boreholes;
			_IX = iX;
			_IY = iY;
			_areas = areas;
			_n = n;
			_m = m;
		}

		public int TimeLayersCount => T.Length;
		public int ElementsCount => (_n - 1) * (_m - 1);
		public int NodesCount => _n * _m;
		public int N => _n;
		public int M => _m;

		public int GetAreaNumber(int IndexOfX, int IndexOfY)
		{
			for (int i = _areas.Length - 1; i >= 0; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)
			{
				if (_IY[_areas[i][2]] <= IndexOfY && IndexOfY <= _IY[_areas[i][3]])
				{
					if (_IX[_areas[i][0]] <= IndexOfX && IndexOfX <= _IX[_areas[i][1]])
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int GetNodeGlobalNumber(int nodeLocalNumber, int elemNumber)
		{
			int yLine = elemNumber / (N - 1);//в каком ряду по у находится кэ 
			int xLine = elemNumber - yLine * (N - 1);//в каком ряду по X находится кэ 
			int xOffset = 0, yOffset = 0;
			switch (nodeLocalNumber)
			{
				case 1:
					xOffset = 1;
					break;
				case 2:
					yOffset = 1;
					break;
				case 3:
					yOffset = 1;
					xOffset = 1;
					break;
			}

			return xLine + xOffset + (yLine + yOffset) * N;
		}

	}
}