using ResearchPaper;

namespace ReaserchPaper
{
    internal class Grid
    {
        public static double[] t;
        static int[][] areas;
        public static double[] x, y, hy, hx, ht;
        public static int[][] boreholes;
        static int[] IX, IY;
        static double q, h;
        static int n, m;

        static double Ro => 0.950;        
        static double С => 0.4200;
        
        static double K(int area)
        {
            switch (area)
            {
                case 1:
                    return 0.005;
                default: return 0.4;
            }
        }      
        static double Eta => 0.001;        

        public static double Lamda(int area) => K(area) / Eta;
        public static double Sigma => Ro * С;   
        public static double Lamda2 => 0.6;


        public static int TimeLayersCount => t.Length;
        public static int ElementsCount => (n - 1) * (m - 1);
        public static int NodesCount => n * m;
        public static int N => n;
        public static int M => m;


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
            double[] XW, YW;
            using (StreamReader sr = new StreamReader(@"input\domain.txt"))
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
                    areas[i] = new int[5];

                for (int i = 0; i < areas.Length; i++)
                {
                    data = sr.ReadLine().Split(' ');
                    for (int j = 0; j < areas[i].Length; j++)
                        areas[i][j] = int.Parse(data[j]);
                }
            }

            using (StreamReader sr = new StreamReader(@"input\mesh.txt"))
            {
                string[] data = sr.ReadLine().Split(' ');
                double[] q = new double[data.Length / 2];
                List<int> xAreaLenghtes = new List<int>();
                List<int> yAreaLenghtes = new List<int>();
                //x.Add(XW[0]);
                //IX.Add(0);
                n = 0;
                m = 0;
                for (int i = 0; i < data.Length - 1; i += 2)
                {
                    xAreaLenghtes.Add(int.Parse(data[i]));
                    n += int.Parse(data[i]);
                    q[i / 2] = double.Parse(data[i + 1]);
                }
                n-=xAreaLenghtes.Count()-1;

                x = new double[n];
                x[0] = XW[0];
                hx = new double[n - 1];
                IX = new int[data.Length / 2 + 1];

                int startPos = 0;
                for (int i = 0; i < q.Length; i++)
                {
                    if (q[i] == 1)
                        h = (XW[i + 1] - XW[i]) / (xAreaLenghtes[i] - 1);
                    else
                    {
                        h = (XW[i + 1] - XW[i]) * (q[i] - 1) / (Math.Pow(q[i], xAreaLenghtes[i] - 1) - 1);
                    }

                    MakeArea(h, x, hx, startPos, xAreaLenghtes[i], q[i]);
                    x[startPos + xAreaLenghtes[i] - 1] = XW[i + 1];
                    startPos += xAreaLenghtes[i] - 1;
                    IX[i + 1] = IX[i] + xAreaLenghtes[i] - 1;
                }

                data = sr.ReadLine().Split(' ');
                q = new double[data.Length / 2];

                for (int i = 0; i < data.Length - 1; i += 2)
                {
                    yAreaLenghtes.Add(int.Parse(data[i]));
                    m += int.Parse(data[i]);
                    q[i / 2] = double.Parse(data[i + 1]);
                }
                m -= yAreaLenghtes.Count() - 1;
                y = new double[m];
                y[0] = YW[0];
                hy = new double[m - 1];
                IY = new int[data.Length / 2 + 1];

                startPos = 0;
                for (int i = 0; i < q.Length; i++)
                {
                    if (q[i] == 1)
                        h = (YW[i + 1] - YW[i]) / (yAreaLenghtes[i] - 1);
                    else
                    {
                        h = (YW[i + 1] - YW[i]) * (q[i] - 1) / (Math.Pow(q[i], yAreaLenghtes[i] - 1) - 1);
                    }
                    MakeArea(h, y, hy, startPos, yAreaLenghtes[i], q[i]);
                    y[startPos + yAreaLenghtes[i] - 1] = YW[i + 1];
                    startPos += yAreaLenghtes[i] - 1;
                    IY[i + 1] = IY[i] + yAreaLenghtes[i] - 1;
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
                q = double.Parse(data[3]);

                if (q == 1)
                    h = (t[layersCount - 1] - t[0]) / (layersCount - 1);
                else
                    h = (t[layersCount - 1] - t[0]) * (q - 1) / (Math.Pow(q, layersCount - 1) - 1);

                for (int i = 1; i < layersCount; i++)
                {
                    t[i] = t[i - 1] + h;
                    ht[i - 1] = h;
                    h *= q;
                }
            }
        }
        static void MakeArea(double h, double[] points, double[] steps, int startPos, int areaLenght, double q)//режим область на части и записываем в массив, h - шаг,  j - номер подобласти
        {
            areaLenght--;
            for (int j = startPos; j < areaLenght + startPos; j++)
            {
                points[j + 1] = (points[j] + h);
                steps[j] = h;
                h *= q;
            }
        }

        public static int GetAreaNumber(int IndexOfX, int IndexOfY)
        {
            for (int i = areas.Length - 1; i >= 0; i--)//идём в обратном порядке чтобы не было бага, когда в качестве подобласти возвращается 0 (0 - вся расчётная область, которая уже вкл. подобласти)
            {
                if (IY[areas[i][2]] <= IndexOfY && IndexOfY <= IY[areas[i][3]])
                {
                    if (IX[areas[i][0]] <= IndexOfX && IndexOfX <= IX[areas[i][1]])
                    {
                        return areas[i][4];
                    }
                }
            }
            return -1;
        }

        public static void WriteGrid()
        {
            using (StreamWriter sw = new StreamWriter(@"output\grid.txt"))
            {
                sw.WriteLine(boreholes.Length);
                for (int i = 0; i < boreholes.Length; i++)
                {
                    sw.WriteLine("{0} {1} {2} {3}", x[Grid.boreholes[i][0]].ToString().Replace(",", "."), x[Grid.boreholes[i][0] + 1].ToString().Replace(",", "."),
                    y[Grid.boreholes[i][1]].ToString().Replace(",", "."), y[Grid.boreholes[i][1] + 1].ToString().Replace(",", "."));
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
                sw.WriteLine(areas.Length);
                foreach (var area in areas)
                    sw.WriteLine("{0} {1} {2} {3}", x[IX[area[0]]], x[IX[area[1]]], y[IY[area[2]]], y[IY[area[3]]]);
                sw.Close();
            }
        }
        public static void PrintPartialGrid()
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