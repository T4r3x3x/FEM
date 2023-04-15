namespace ReaserchPaper
{
    internal class Grid
    {
        public static double[]  XW, YW, t;
        static int[][] areas;
        public static List<double> x, y, hy, hx;
        static List<int> IX, IY;
        static double q,h;
        static  int n, m;

        public static int TimeLayersCount => t.Length;
        public static int ElementsCount => (n - 1) * (m - 1);
        public static int NodesCount => n*m;
        public static int N => n;
        public static int M => m;


        static Grid()
        {
            x = new List<double>();
            y = new List<double>();
            hy = new List<double>();
            hx = new List<double>();
            IX = new List<int>();
            IY = new List<int>();

        }

        public static void PrintTimeGrid()
        {
            Console.WriteLine();
            for (int i = 0; i < t.Length; i++)
            {
                Console.Write("[" + t[i] + "] ");
            }
            Console.WriteLine();
        }
        public static void ReadData()
        {
            using (StreamReader sr = new StreamReader("domain.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                n = int.Parse(data[0]);
                XW = new double[n];

                data = sr.ReadLine().Split(' ');
                for (int i = 0; i < n; i++)
                    XW[i] = double.Parse(data[i]);

                data = sr.ReadLine().Split(' ');
                m = int.Parse(data[0]);
                YW = new double[m];

                data = sr.ReadLine().Split(' ');
                for (int i = 0; i < m; i++)
                    YW[i] = double.Parse(data[i]);

                data = sr.ReadLine().Split(' ');
                areas = new int[int.Parse(data[0])][];
                for (int i = 0; i < areas.Length; i++)
                    areas[i] = new int[4];

                for (int i = 0; i < areas.Length; i++)
                {
                    data = sr.ReadLine().Split(' ');
                    for (int j = 0; j < 4; j++)
                        areas[i][j] = int.Parse(data[j]);
                }
            }

            using (StreamReader sr = new StreamReader("mesh.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                x.Add(XW[0]);
                IX.Add(0);
                for (int i = 0; i < data.Length - 1; i += 2)
                {
                    n = int.Parse(data[i]);
                    q = double.Parse(data[i + 1]);
                    if (q == 1)
                        h = (XW[i / 2 + 1] - XW[i / 2]) / (n - 1);
                    else
                    {
                        h = (XW[i / 2 + 1] - XW[i / 2]) * (q - 1) / (Math.Pow(q, n - 1) - 1);
                    }
                    MakeArea(h, x, hx, n, q);
                    IX.Add(x.Count() - 1);
                }

                data = sr.ReadLine().Split(' ');
                y.Add(YW[0]);
                IY.Add(0);
                for (int i = 0; i < data.Length - 1; i += 2)
                {
                    m = int.Parse(data[i]);
                    q = double.Parse(data[i + 1]);
                    if (q == 1)
                        h = (YW[i / 2 + 1] - YW[i / 2]) / (n - 1);
                    else
                    {
                        h = (YW[i / 2 + 1] - YW[i / 2]) * (q - 1) / (Math.Pow(q, n - 1) - 1);
                    }
                    MakeArea(h, y, hy, n, q);
                    IY.Add(y.Count() - 1);
                }
            }

            using (StreamReader sr = new StreamReader("timeGrid.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                int layersCount = int.Parse(data[0]);
                t = new double[layersCount];
                t[0] = double.Parse(data[1]);
                t[layersCount - 1] = double.Parse(data[2]);
                q = double.Parse(data[3]);

                if (q == 1)
                    h = (t[layersCount - 1] - t[0]) / (layersCount - 1);
                else
                    h = (t[layersCount - 1] - t[0]) * (q - 1) / (Math.Pow(q, layersCount - 1) - 1);

                for (int i = 1; i < layersCount; i++)
                {
                    t[i] = t[i - 1] + h;
                    h *= q;
                }
            }
        }
        static void MakeArea(double h, List<double> points, List<double> steps, int n, double q)//режим область на части и записываем в массив, h - шаг,  j - номер подобласти
        {
            n--;
            int size = points.Count();
            for (int j = size; j < n + size; j++)
            {
                points.Add(points[j - 1] + h);
                steps.Add(h);
                h *= q;
            }
        }

        //                  y      x
        public int GetAreaNumber(int _i, int _j)
        {
            for (int i = 0; i < areas.Length; i++)
            {
                if (IY[areas[i][2]] <= _i && _i <= IY[areas[i][3]])
                {
                    if (IY[areas[i][0]] <= _j && _j <= IY[areas[i][1]])
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
       public  static void PrintPartialGrid()
        {
            for (int j = 0; j < m; j++)
            {
                for (int i = 0; i < n; i++)
                {

                    Console.Write(" [{0}, {1}] ", x[i].ToString("e2"), y[j].ToString("e2"));
                }
                Console.WriteLine("\n");
            }
        }
    }
}