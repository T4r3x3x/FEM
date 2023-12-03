using FemProducer.DTO;

namespace ReaserchPaper.Grid
{
	internal class GridFactory
	{
		private double[] _t;
		private double[] _x, _y, _hy, _hx, _ht;
		private int[][] _boreholes;

		private int[] _IX, _IY;
		private int[][] _areas;
		private int _n, _m;

		private double _h;
		private double[] XW, YW;
		private double[] _q;
		private double q;

		public GridFactory()
		{


		}

		public Grid GetGrid(GridParametrs gridParametrs)
		{
			_q = new double[data.Length / 2];
			List<int> xAreaLenghtes = new List<int>();
			List<int> yAreaLenghtes = new List<int>();
			//_x.Add(XW[0]);
			//_IX.Add(0);
			_n = 0;
			_m = 0;
			for (int i = 0; i < data.Length - 1; i += 2)
			{
				xAreaLenghtes.Add(int.Parse(data[i]));
				_n += int.Parse(data[i]) - 1;
				_q[i / 2] = double.Parse(data[i + 1]);
			}
			_n++;

			_x = new double[_n];
			_x[0] = XW[0];
			_hx = new double[_n - 1];
			_IX = new int[data.Length / 2 + 1];

			int startPos = 0;
			for (int i = 0; i < _q.Length; i++)
			{
				if (_q[i] == 1)
					_h = (XW[i + 1] - XW[i]) / (xAreaLenghtes[i] - 1);
				else
				{
					_h = (XW[i + 1] - XW[i]) * (_q[i] - 1) / (Math.Pow(_q[i], xAreaLenghtes[i] - 1) - 1);
				}

				MakeArea(_h, _x, _hx, startPos, xAreaLenghtes[i], _q[i]);
				_x[startPos + xAreaLenghtes[i] - 1] = XW[i + 1];
				startPos += xAreaLenghtes[i] - 1;
				_IX[i + 1] = _IX[i] + xAreaLenghtes[i] - 1;
			}

			data = sr.ReadLine().Split(' ');
			_q = new double[data.Length / 2];

			for (int i = 0; i < data.Length - 1; i += 2)
			{
				yAreaLenghtes.Add(int.Parse(data[i]));
				_m += int.Parse(data[i]) - 1;
				_q[i / 2] = double.Parse(data[i + 1]);
			}
			_m++;
			_y = new double[_m];
			_y[0] = YW[0];
			_hy = new double[_m - 1];
			_IY = new int[data.Length / 2 + 1];

			startPos = 0;
			for (int i = 0; i < _q.Length; i++)
			{
				if (_q[i] == 1)
					_h = (YW[i + 1] - YW[i]) / (yAreaLenghtes[i] - 1);
				else
				{
					_h = (YW[i + 1] - YW[i]) * (_q[i] - 1) / (Math.Pow(_q[i], yAreaLenghtes[i] - 1) - 1);
				}
				MakeArea(_h, _y, _hy, startPos, yAreaLenghtes[i], _q[i]);
				_y[startPos + yAreaLenghtes[i] - 1] = YW[i + 1];
				startPos += yAreaLenghtes[i] - 1;
				_IY[i + 1] = _IY[i] + yAreaLenghtes[i] - 1;
			}
		}

		public void ReadData()
		{

			using (StreamReader sr = new StreamReader(@"input\domain.txt"))
			{
				string[] data = sr.ReadLine().Split(' ');
				_n = int.Parse(data[0]);
				XW = new double[_n];

				data = sr.ReadLine().Split(' ');
				for (int i = 0; i < _n; i++)
					XW[i] = double.Parse(data[i]);

				data = sr.ReadLine().Split(' ');
				_m = int.Parse(data[0]);
				YW = new double[_m];

				data = sr.ReadLine().Split(' ');
				for (int i = 0; i < _m; i++)
					YW[i] = double.Parse(data[i]);

				data = sr.ReadLine().Split(' ');
				_areas = new int[int.Parse(data[0])][];
				for (int i = 0; i < _areas.Length; i++)
					_areas[i] = new int[4];

				for (int i = 0; i < _areas.Length; i++)
				{
					data = sr.ReadLine().Split(' ');
					for (int j = 0; j < 4; j++)
						_areas[i][j] = int.Parse(data[j]);
				}
			}

			using (StreamReader sr = new StreamReader(@"input\mesh.txt"))
			{
				string[] data = sr.ReadLine().Split(' ');
				_q = new double[data.Length / 2];
				List<int> xAreaLenghtes = new List<int>();
				List<int> yAreaLenghtes = new List<int>();
				//_x.Add(XW[0]);
				//_IX.Add(0);
				_n = 0;
				_m = 0;
				for (int i = 0; i < data.Length - 1; i += 2)
				{
					xAreaLenghtes.Add(int.Parse(data[i]));
					_n += int.Parse(data[i]) - 1;
					_q[i / 2] = double.Parse(data[i + 1]);
				}
				_n++;

				_x = new double[_n];
				_x[0] = XW[0];
				_hx = new double[_n - 1];
				_IX = new int[data.Length / 2 + 1];

				int startPos = 0;
				for (int i = 0; i < _q.Length; i++)
				{
					if (_q[i] == 1)
						_h = (XW[i + 1] - XW[i]) / (xAreaLenghtes[i] - 1);
					else
					{
						_h = (XW[i + 1] - XW[i]) * (_q[i] - 1) / (Math.Pow(_q[i], xAreaLenghtes[i] - 1) - 1);
					}

					MakeArea(_h, _x, _hx, startPos, xAreaLenghtes[i], _q[i]);
					_x[startPos + xAreaLenghtes[i] - 1] = XW[i + 1];
					startPos += xAreaLenghtes[i] - 1;
					_IX[i + 1] = _IX[i] + xAreaLenghtes[i] - 1;
				}

				data = sr.ReadLine().Split(' ');
				_q = new double[data.Length / 2];

				for (int i = 0; i < data.Length - 1; i += 2)
				{
					yAreaLenghtes.Add(int.Parse(data[i]));
					_m += int.Parse(data[i]) - 1;
					_q[i / 2] = double.Parse(data[i + 1]);
				}
				_m++;
				_y = new double[_m];
				_y[0] = YW[0];
				_hy = new double[_m - 1];
				_IY = new int[data.Length / 2 + 1];

				startPos = 0;
				for (int i = 0; i < _q.Length; i++)
				{
					if (_q[i] == 1)
						_h = (YW[i + 1] - YW[i]) / (yAreaLenghtes[i] - 1);
					else
					{
						_h = (YW[i + 1] - YW[i]) * (_q[i] - 1) / (Math.Pow(_q[i], yAreaLenghtes[i] - 1) - 1);
					}
					MakeArea(_h, _y, _hy, startPos, yAreaLenghtes[i], _q[i]);
					_y[startPos + yAreaLenghtes[i] - 1] = YW[i + 1];
					startPos += yAreaLenghtes[i] - 1;
					_IY[i + 1] = _IY[i] + yAreaLenghtes[i] - 1;
				}


			}
			if (new FileInfo(@"input\_boreholes.txt").Length == 0)
			{
				_boreholes = new int[0][];
			}
			else
			{
				using (StreamReader sr = new StreamReader(@"input\_boreholes.txt"))
				{
					string[] data = sr.ReadLine().Split(' ');
					_boreholes = new int[int.Parse(data[0])][];
					for (int i = 0; i < _boreholes.Length; i++)
					{
						_boreholes[i] = new int[2];
						data = sr.ReadLine().Split(' ');
						_boreholes[i][0] = int.Parse(data[0]);
						_boreholes[i][1] = int.Parse(data[1]);
					}
				}

			}

			using (StreamReader sr = new StreamReader(@"input\timeGrid.txt"))
			{
				string[] data = sr.ReadLine().Split(' ');
				int layersCount = int.Parse(data[0]);
				_t = new double[layersCount];
				_ht = new double[layersCount - 1];
				_t[0] = double.Parse(data[1]);
				_t[layersCount - 1] = double.Parse(data[2]);
				q = double.Parse(data[3]);

				if (q == 1)
					_h = (_t[layersCount - 1] - _t[0]) / (layersCount - 1);
				else
					_h = (_t[layersCount - 1] - _t[0]) * (q - 1) / (Math.Pow(q, layersCount - 1) - 1);

				for (int i = 1; i < layersCount; i++)
				{
					_t[i] = _t[i - 1] + _h;
					_ht[i - 1] = _h;
					_h *= q;
				}
			}
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
