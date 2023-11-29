namespace ReaserchPaper.Grid
{
	internal class Grid
	{
		public double[] t;
		public double[] x, y, hy, hx, ht;
		public int[][] boreholes;

		private int[] _IX, _IY;
		private int[][] _areas;
		private double _q, _h;
		private int _n, _m;

		public int TimeLayersCount => t.Length;
		public int ElementsCount => (_n - 1) * (_m - 1);
		public int NodesCount => _n * _m;
		public int N => _n;
		public int M => _m;


		public void PrintTimeGrid()
		{
			Console.WriteLine();
			for (int i = 0; i < t.Length; i++)
			{
				Console.Write("[" + t[i] + "] ");
			}
			Console.WriteLine();
		}
		public void ReadData()
		{
			double[] XW, YW;
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
				double[] _q = new double[data.Length / 2];
				List<int> xAreaLenghtes = new List<int>();
				List<int> yAreaLenghtes = new List<int>();
				//x.Add(XW[0]);
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

				x = new double[_n];
				x[0] = XW[0];
				hx = new double[_n - 1];
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

					MakeArea(_h, x, hx, startPos, xAreaLenghtes[i], _q[i]);
					x[startPos + xAreaLenghtes[i] - 1] = XW[i + 1];
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
				y = new double[_m];
				y[0] = YW[0];
				hy = new double[_m - 1];
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
					MakeArea(_h, y, hy, startPos, yAreaLenghtes[i], _q[i]);
					y[startPos + yAreaLenghtes[i] - 1] = YW[i + 1];
					startPos += yAreaLenghtes[i] - 1;
					_IY[i + 1] = _IY[i] + yAreaLenghtes[i] - 1;
				}


			}
			if (new FileInfo(@"input\boreholes.txt").Length == 0)
			{
				boreholes = new int[0][];
			}
			else
			{
				using (StreamReader sr = new StreamReader(@"input\boreholes.txt"))
				{
					string[] data = sr.ReadLine().Split(' ');
					boreholes = new int[int.Parse(data[0])][];
					for (int i = 0; i < boreholes.Length; i++)
					{
						boreholes[i] = new int[2];
						data = sr.ReadLine().Split(' ');
						boreholes[i][0] = int.Parse(data[0]);
						boreholes[i][1] = int.Parse(data[1]);
					}
				}

			}

			using (StreamReader sr = new StreamReader(@"input\timeGrid.txt"))
			{
				string[] data = sr.ReadLine().Split(' ');
				int layersCount = int.Parse(data[0]);
				t = new double[layersCount];
				ht = new double[layersCount - 1];
				t[0] = double.Parse(data[1]);
				t[layersCount - 1] = double.Parse(data[2]);
				_q = double.Parse(data[3]);

				if (_q == 1)
					_h = (t[layersCount - 1] - t[0]) / (layersCount - 1);
				else
					_h = (t[layersCount - 1] - t[0]) * (_q - 1) / (Math.Pow(_q, layersCount - 1) - 1);

				for (int i = 1; i < layersCount; i++)
				{
					t[i] = t[i - 1] + _h;
					ht[i - 1] = _h;
					_h *= _q;
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

		public int GetNodeNumber(int nodeLocalNumber, int elemNumber)
		{
			int yLine = elemNumber / (N - 1);//в каком ряду по у находится кэ 
			int xLine = elemNumber - yLine * (N - 1);//в каком ряду по x находится кэ 
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

		public void WriteGrid()
		{
			using (StreamWriter sw = new StreamWriter(@"output\grid.txt"))
			{
				sw.WriteLine(boreholes.Length);
				for (int i = 0; i < boreholes.Length; i++)
				{
					sw.WriteLine("{0} {1} {2} {3}", x[boreholes[i][0]].ToString().Replace(",", "."), x[boreholes[i][0] + 1].ToString().Replace(",", "."),
					y[boreholes[i][1]].ToString().Replace(",", "."), y[boreholes[i][1] + 1].ToString().Replace(",", "."));
				}

				sw.WriteLine("{0} {1} {2} {3}", x[0].ToString().Replace(",", "."), x[x.Count() - 1].ToString().Replace(",", "."),
						 y[0].ToString().Replace(",", "."), y[y.Count() - 1].ToString().Replace(",", "."));

				sw.WriteLine(x.Count());
				sw.WriteLine(y.Count());
				//  sw.WriteLine("Hello World!!");
				for (int i = 0; i < x.Count(); i++)
				{
					sw.WriteLine(x[i].ToString().Replace(",", "."));
				}
				for (int i = 0; i < y.Count(); i++)
				{
					sw.WriteLine(y[i].ToString().Replace(",", "."));
				}
				sw.WriteLine(_areas.Length);
				foreach (var area in _areas)
					sw.WriteLine("{0} {1} {2} {3}", x[_IX[area[0]]], x[_IX[area[1]]], y[_IY[area[2]]], y[_IY[area[3]]]);
				sw.Close();
			}
		}
		public void PrintPartialGrid()
		{
			for (int j = 0; j < _m; j++)
			{
				for (int i = 0; i < _n; i++)
				{

					Console.Write(" [{0}, {1}] ", x[i].ToString("e2"), y[j].ToString("e2"));
				}
				Console.WriteLine("\n");
			}
		}
	}
}