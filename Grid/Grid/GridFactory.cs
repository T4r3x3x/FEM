using FemProducer.DTO;

namespace ReaserchPaper.Grid
{
	internal class GridFactory
	{
		//private double[] _t;
		//private double[] _x, _y, _hy, _hx, _ht;
		//private int[][] _boreholes;

		//private int[] _IX, _IY;
		//private int[][] _areas;
		//private int _n, _m;

		//private double _h;
		//private double[] XW, YW;
		//private double[] _q;
		//private double q;

		public GridFactory()
		{


		}

		public Grid GetGrid(GridParameters gridParametrs)
		{
			double h;
			int startPos = 0;
			var n = 0;
			foreach (var item in gridParametrs.xAreaLenghtes)
				n += item;

			var m = 0;
			foreach (var item in gridParametrs.yAreaLenghtes)
				m += item;
			var hx = new double[n - 1];
			var x = new double[n];
			var IX = new int[gridParametrs.qx.Length + 1];
			x[0] = gridParametrs.XW[0];

			for (int i = 0; i < gridParametrs.qx.Length; i++)
			{
				if (gridParametrs.qx[i] == 1)
					h = (gridParametrs.XW[i + 1] - gridParametrs.XW[i]) / (gridParametrs.xAreaLenghtes[i] - 1);
				else
				{
					h = (gridParametrs.XW[i + 1] - gridParametrs.XW[i]) * (gridParametrs.qx[i] - 1) / (Math.Pow(gridParametrs.qx[i], gridParametrs.xAreaLenghtes[i] - 1) - 1);
				}

				MakeArea(h, x, hx, startPos, gridParametrs.xAreaLenghtes[i], gridParametrs.qx[i]);
				x[startPos + gridParametrs.xAreaLenghtes[i] - 1] = gridParametrs.XW[i + 1];
				startPos += gridParametrs.xAreaLenghtes[i] - 1;
				IX[i + 1] = IX[i] + gridParametrs.xAreaLenghtes[i] - 1;
			}

			var hy = new double[m - 1];
			var y = new double[m];
			var IY = new int[gridParametrs.qy.Length + 1];
			startPos = 0;
			for (int i = 0; i < gridParametrs.qy.Length; i++)
			{
				if (gridParametrs.qy[i] == 1)
					h = (gridParametrs.YW[i + 1] - gridParametrs.YW[i]) / (gridParametrs.yAreaLenghtes[i] - 1);
				else
				{
					h = (gridParametrs.YW[i + 1] - gridParametrs.YW[i]) * (gridParametrs.qy[i] - 1) / (Math.Pow(gridParametrs.qy[i], gridParametrs.yAreaLenghtes[i] - 1) - 1);
				}
				MakeArea(h, y, hy, startPos, gridParametrs.yAreaLenghtes[i], gridParametrs.qy[i]);
				y[startPos + gridParametrs.yAreaLenghtes[i] - 1] = gridParametrs.YW[i + 1];
				startPos += gridParametrs.yAreaLenghtes[i] - 1;
				IY[i + 1] = IY[i] + gridParametrs.yAreaLenghtes[i] - 1;
			}
			//if (q == 1)
			//	_h = (_t[layersCount - 1] - _t[0]) / (layersCount - 1);
			//else
			//	_h = (_t[layersCount - 1] - _t[0]) * (q - 1) / (Math.Pow(q, layersCount - 1) - 1);

			//for (int i = 1; i < layersCount; i++)
			//{
			//	_t[i] = _t[i - 1] + _h;
			//	_ht[i - 1] = _h;
			//	_h *= q;
			//}
			return new Grid(new double[] { 0 }, x, y, hy, hx, null, null, IX, IY, gridParametrs.areas, n, m);
		}

		void MakeArea(double _h, double[] points, double[] steps, int startPos, int areaLenght, double _q)//режим область на части и записываем в массив, _h - шаг,  j - номер подобласти
		{
			areaLenght--;
			for (int j = startPos; j < areaLenght + startPos; j++)
			{
				points[j + 1] = points[j] + _h;
				steps[j] = _h;
				_h *= _q;
			}
		}

	}
}
