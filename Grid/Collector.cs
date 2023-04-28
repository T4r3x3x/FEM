using ResearchPaper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReaserchPaper
{
    internal class Collector
    {
        static Matrix _M, _G, _H;
        static double timeCoef;
        static double[] deltaTimes = new double[3];
        static Vector[] MQs = new Vector[2];

        static double G(double h, int i, int j)
        {
            switch (i)
            {
                case 0:
                    switch (j)
                    {
                        case 0:
                            return 1 / h;
                        case 1:
                            return -1 / h;
                    }
                    break;
                case 1:
                    switch (j)
                    {
                        case 0:
                            return -1 / h;
                        case 1:
                            return 1 / h;
                    }
                    break;
            }
            return 0;
        }
        static double M(double h, int i, int j)
        {
            switch (i)
            {
                case 0:
                    switch (j)
                    {
                        case 0:
                            return 2 * h / 6;
                        case 1:
                            return h / 6;
                    }
                    break;
                case 1:
                    switch (j)
                    {
                        case 0:
                            return h / 6;
                        case 1:
                            return 2 * h / 6;
                    }
                    break;
            }
            return 0;
        }

        public Collector(int nodesCount)
        {
            _M = new Matrix(nodesCount);
            _G = new Matrix(nodesCount);
            _H = new Matrix(nodesCount);
            GetMatrixesMG();
            // _M.Print();
            //  _G.Print(); 
            //        _H.Print();
            GetTimeConditions();
        }

        //static Point GetV(int i, int j, int k)
        //{

        //}
        /// <summary>
        ///  Сборка слау для первой задачи
        /// </summary>
        public void Collect()
        {
            ResetSlau();
            MakeSLau();
            GetBoundaryConditions();
        }


        public void Collect(int timeLayer)
        {
            ResetSlauOptimized();
            MakeSLau(timeLayer);
            GetBoundaryConditions(timeLayer);
        }

        static void SubstractM()
        {
            Master.Slau.A -= _M * timeCoef;
        }

        static void ResetSlauOptimized()
        {
            SubstractM();
            Master.Slau.b.Reset();
        }

        static void ResetSlau()
        {
            Master.Slau.A.Reset();
            Master.Slau.b.Reset();
        }

        public void SwitchTask()
        {
            ResetSlau();
            _M *= Master.Sigma;
            Master.Slau.A += _G * Master.Lamda2 + _H;

            deltaTimes[2] = Grid.t[2] - Grid.t[0];
            deltaTimes[1] = Grid.t[1] - Grid.t[0];
            deltaTimes[0] = Grid.t[2] - Grid.t[1];

            MQs[0] = _M * Master.Slau.q[0];
            MQs[1] = _M * Master.Slau.q[1];
        }

        private void GetMatrixesMG()
        {
            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++) //все кэ под скважиной
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                {
                    localMatrix = GetMassMatrix(Grid.hx[i], Grid.hy[j]);
                    AddLocalMatrix(_M, localMatrix, i, j);

                    localMatrix = GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);
                    AddLocalMatrix(_G, localMatrix, i, j);
                }


            //for (int i = 0; i < Master.borehole[0]; i++) //все кэ справа от скважины (в той же "строке")
            //    {
            //        localMatrix = GetMassMatrix(Grid.hx[i], Grid.hy[Master.borehole[2]]);
            //        AddLocalMatrix(_M, localMatrix, i, Master.borehole[2]);

            //        localMatrix = GetStiffnessMatrix(Grid.hx[i], Grid.hy[Master.borehole[2]]);
            //        AddLocalMatrix(_G, localMatrix, i, Master.borehole[2]);
            //    }

            //for (int i = Master.borehole[1]; i < Grid.N - 1; i++) //все кэ справа от скважины (в той же "строке")
            //{
            //    localMatrix = GetMassMatrix(Grid.hx[i], Grid.hy[Master.borehole[2]]);
            //    AddLocalMatrix(_M, localMatrix, i, Master.borehole[2]);

            //    localMatrix = GetStiffnessMatrix(Grid.hx[i], Grid.hy[Master.borehole[2]]);
            //    AddLocalMatrix(_G, localMatrix, i, Master.borehole[2]);
            //}

            //for (int j = Master.borehole[2] + 1; j < Grid.M - 1; j++)//все кэ над скважиной
            //        for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
            //        {
            //            localMatrix = GetMassMatrix(Grid.hx[i], Grid.hy[j]);
            //            AddLocalMatrix(_M, localMatrix, i, j);

            //            localMatrix = GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);
            //            AddLocalMatrix(_G, localMatrix, i, j);
            //        }
        }
        public void GetMatrixH()
        {
            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                {
                    localMatrix = GetGradTMatrix(LocalNumToGlobal(i, j, 0), Grid.hx[i], Grid.hy[j], Grid.x[i], Grid.y[j]);
                    AddLocalMatrix(_H, localMatrix, i, j);
                }
        }
        public static int LocalNumToGlobal(int i, int j, int k)
        {
            return i + j * Grid.N + k / 2 * Grid.N + k % 2;
        }
        static void AddLocalMatrix(Matrix matrix, double[][] localMatrix, int i, int j)
        {
            for (int p = 0; p < 4; p++)
            {
                matrix.di[LocalNumToGlobal(i, j, p)] += localMatrix[p][p];

                int ibeg = matrix.ia[LocalNumToGlobal(i, j, p)];
                int iend = matrix.ia[LocalNumToGlobal(i, j, p) + 1];
                for (int k = 0; k < p; k++)
                {
                    int index = BinarySearch(matrix.ja, LocalNumToGlobal(i, j, k), ibeg, iend - 1);

                    matrix.au[index] += localMatrix[k][p];
                    matrix.al[index] += localMatrix[p][k];
                    ibeg++;
                }
            }
        }
        static int BinarySearch(List<int> list, int value, int l, int r)
        {
            while (l != r)
            {
                int mid = (l + r) / 2 + 1;

                if (list[mid] > value)
                    r = mid - 1;
                else
                    l = mid;
            }

            return (list[l] == value) ? l : -1;
        }
        private static void GetTimeConditions()
        {
            for (int p = 0; p < 2; p++)
                for (int j = 0; j < Grid.M - 1; j++)
                    for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    {
                        Master.Slau.q[p].Elements[i + j * Grid.N] = Master.TemperatureAtBegin();
                        Master.Slau.q[p].Elements[i + j * Grid.N + 1] = Master.TemperatureAtBegin();
                        Master.Slau.q[p].Elements[i + (j + 1) * Grid.N] = Master.TemperatureAtBegin();
                        Master.Slau.q[p].Elements[i + (j + 1) * Grid.N + 1] = Master.TemperatureAtBegin();
                    }
        }
        static int mu(int i) => ((i) % 2);
        static int nu(int i) => ((i) / 2);

        static double[][] GetMassMatrix(double hx, double hy)// Grid.M - номер кэ 
        {
            // инициализация
            double[][] result = new double[4][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица масс
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] += M(hx, mu(i), mu(j)) * M(hy, nu(i), nu(j));

            return result;
        }
        static double[][] GetStiffnessMatrix(double hx, double hy)// Grid.M - номер кэ 
        {
            // инициализация
            double[][] result = new double[4][];
            for (int i = 0; i < result.Length; i++)
                result[i] = new double[result.Length];

            //матрица жесткости
            for (int i = 0; i < result.Length; i++)
                for (int j = 0; j < result.Length; j++)
                    result[i][j] = (G(hx, mu(i), mu(j)) * M(hy, nu(i), nu(j)) + M(hx, mu(i), mu(j)) * G(hy, nu(i), nu(j))); //матрица жётскости

            return result;
        }

        static void MakeSLau()
        {
            Master.Slau.A += _M * Master.Gamma + _G * Master.Lamda;
            for (int j = 0; j < Grid.M - 1; j++) //все кэ под скважиной
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                    AddLocalB(i, j);
            //for (int j = 0; j < Master.borehole[2]; j++) //все кэ под скважиной
            //    for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
            //        AddLocalB(i, j);


            //for (int i = 0; i < Master.borehole[0]; i++) //все кэ справа от скважины (в той же "строке")
            //    AddLocalB(i, Master.borehole[2]);

            //for (int i = Master.borehole[1]; i < Grid.N - 1; i++) //все кэ справа от скважины (в той же "строке")
            //    AddLocalB(i, Master.borehole[2]);

            //for (int j = Master.borehole[2] + 1; j < Grid.M - 1; j++)//все кэ над скважиной
            //    for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
            //        AddLocalB(i, j);
            //AddLocalB(1, 1);
        }

        static void MakeSLau(int timeLayer)
        {
            deltaTimes[1] = deltaTimes[0];
            deltaTimes[0] = Grid.t[timeLayer] - Grid.t[timeLayer - 1];
            deltaTimes[2] = Grid.t[timeLayer] - Grid.t[timeLayer - 2];

            MQs[0] = MQs[1];
            MQs[1] = _M * Master.Slau.q[timeLayer - 1];

            timeCoef = ((deltaTimes[2] + deltaTimes[0]) / (deltaTimes[2] * deltaTimes[0])) ;

            Master.Slau.b += -(deltaTimes[0] / (deltaTimes[2] * deltaTimes[1])) * MQs[0] + deltaTimes[2] / (deltaTimes[1] * deltaTimes[0]) * MQs[1];
            Master.Slau.A += _M * timeCoef;

        //  for (int j = 0; j < Grid.M - 1; j++)
          //      for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
           //         AddLocalB(i, j, timeLayer);
        }
        static void AddLocalB(int i, int j)
        {
            Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F1(Grid.x[i], Grid.y[j]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j]) + 2 * Master.F1(Grid.x[i], Grid.y[j + 1]) + Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
            Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.x[i], Grid.y[j]) + 4 * Master.F1(Grid.x[i + 1], Grid.y[j]) + Master.F1(Grid.x[i], Grid.y[j + 1]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F1(Grid.x[i], Grid.y[j]) + Master.F1(Grid.x[i + 1], Grid.y[j]) + 4 * Master.F1(Grid.x[i], Grid.y[j + 1]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F1(Grid.x[i], Grid.y[j]) + 2 * Master.F1(Grid.x[i + 1], Grid.y[j]) + 2 * Master.F1(Grid.x[i], Grid.y[j + 1]) + 4 * Master.F1(Grid.x[i + 1], Grid.y[j + 1]));
        }
        static void AddLocalB(int i, int j, int timeLayer)
        {
            Master.Slau.b.Elements[i + j * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (4 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
            Master.Slau.b.Elements[i + j * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + 4 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N] += Grid.hx[i] * Grid.hy[j] / 36 * (2 * Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + 4 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
            Master.Slau.b.Elements[i + (j + 1) * Grid.N + 1] += Grid.hx[i] * Grid.hy[j] / 36 * (Master.F2(Grid.x[i], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i + 1], Grid.y[j], Grid.t[timeLayer]) + 2 * Master.F2(Grid.x[i], Grid.y[j + 1], Grid.t[timeLayer]) + 4 * Master.F2(Grid.x[i + 1], Grid.y[j + 1], Grid.t[timeLayer]));
        }
        static void ZeroingRow(int row)
        {
            for (int i = Master.Slau.A.ia[row]; i < Master.Slau.A.ia[row + 1]; i++)
                Master.Slau.A.al[i] = 0;

            for (int j = row + 1; j < Master.Slau.A.Size; j++)
            {
                int jbeg = Master.Slau.A.ia[j];
                int jend = Master.Slau.A.ia[j + 1];
                int index = BinarySearch(Master.Slau.A.ja, row, jbeg, jend - 1);
                if (index != -1)
                    Master.Slau.A.au[index] = 0;
            }
        }

        static void GetBoundaryConditions()
        {
            //    Master.Slau.Print();
            double _temp = 2 * Master.BoreholePower() * (Grid.hx[Master.borehole[0]] + Grid.hx[Master.borehole[2]]) / (Grid.hx[Master.borehole[0]] * Grid.hx[Master.borehole[2]]);
            Vector temp = new Vector(4, Enumerable.Repeat(_temp, 4).ToArray());
            double[][] local = GetMassMatrix(Grid.hx[Master.borehole[0]], Grid.hx[Master.borehole[2]]);
            temp = local * temp;
            Master.Slau.b.Elements[Master.borehole[2] * Grid.N + Master.borehole[0]] = temp.Elements[0];
            Master.Slau.b.Elements[Master.borehole[2] * Grid.N + Master.borehole[1]] = temp.Elements[1];
            Master.Slau.b.Elements[Master.borehole[3] * Grid.N + Master.borehole[0]] = temp.Elements[2];
            Master.Slau.b.Elements[Master.borehole[3] * Grid.N + Master.borehole[1]] = temp.Elements[3];

            //нижняя граница

            for (int i = 0; i < Grid.N; i++)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.PressuereInReservoir(Grid.x[i], Grid.y[0]);
            }

            //верхняя граница
            for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.PressuereInReservoir(Grid.x[i % Grid.N], Grid.y[Grid.M - 1]);
            }

            //левая граница
            for (int i = Grid.N; i < (Grid.M - 1) * Grid.N; i += Grid.N)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.PressuereInReservoir(Grid.x[0], Grid.y[i / Grid.N]);
            }

            //правая граница
            for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.PressuereInReservoir(Grid.x[Grid.N - 1], Grid.y[i / Grid.N]);
            }


        }
        static void GetBoundaryConditions(int timeLayer)
        {



            //нижняя граница
            if (Master.boundaryConditions[0] == 1)//первое краевое
                for (int i = 0; i < Grid.N; i++)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
                    //  Master.Slau.A.di[i] = C;
                    //   Master.Slau.b.Elements[i] = C * Master.Func2(Grid.x[i], Grid.y[0], Grid.t[timeLayer]);
                }
            else //второе краевое            
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.TemperatureAtBoundary() + Master.TemperatureAtBoundary());
                    Master.Slau.b.Elements[i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.TemperatureAtBoundary() + 2 * Master.TemperatureAtBoundary());
                }

            if (Master.boundaryConditions[2] == 1)//верхняя граница
                for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i] += Master.Lamda * Grid.hx[i] / 6 * (2 * Master.TemperatureAtBoundary() + Master.TemperatureAtBoundary());
                    Master.Slau.b.Elements[Grid.N * (Grid.M - 1) + i + 1] += Master.Lamda * Grid.hx[i] / 6 * (Master.TemperatureAtBoundary() + 2 * Master.TemperatureAtBoundary());
                }

            if (Master.boundaryConditions[3] == 1)//левая гравнь
                for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * i] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.TemperatureAtBoundary() + Master.TemperatureAtBoundary());
                    Master.Slau.b.Elements[Grid.N * (i + 1)] += Grid.hy[i] * Master.Lamda / 6 * (Master.TemperatureAtBoundary() + 2 * Master.TemperatureAtBoundary());
                }

            if (Master.boundaryConditions[1] == 1)//правая граница
                for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
                {
                    ZeroingRow(i);
                    Master.Slau.A.di[i] = 1;
                    Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
                }
            else
                for (int i = 0; i < Grid.N - 1; i++)
                {
                    Master.Slau.b.Elements[Grid.N * (i + 1) - 1] += Grid.hy[i] * Master.Lamda / 6 * (2 * Master.TemperatureAtBoundary() + Master.TemperatureAtBoundary());
                    Master.Slau.b.Elements[Grid.N * (i + 2) - 1] += Grid.hy[i] * Master.Lamda / 6 * (Master.TemperatureAtBoundary() + 2 * Master.TemperatureAtBoundary());
                }

            double _temp = 2 * Master.TemperatureInBorehole() * (Grid.hx[Master.borehole[0]] + Grid.hx[Master.borehole[2]]) / (Grid.hx[Master.borehole[0]] * Grid.hx[Master.borehole[2]]);
            Vector temp = new Vector(4, Enumerable.Repeat(_temp, 4).ToArray());
            double[][] local = GetMassMatrix(Grid.hx[Master.borehole[0]], Grid.hx[Master.borehole[2]]);
            temp = local * temp;
            Master.Slau.b.Elements[Master.borehole[2] * Grid.N + Master.borehole[0]] = temp.Elements[0];
            Master.Slau.b.Elements[Master.borehole[2] * Grid.N + Master.borehole[1]] = temp.Elements[1];
            Master.Slau.b.Elements[Master.borehole[3] * Grid.N + Master.borehole[0]] = temp.Elements[2];
            Master.Slau.b.Elements[Master.borehole[3] * Grid.N + Master.borehole[1]] = temp.Elements[3];
        }


        static double[][] GetGradTMatrix(int elemNumber, double hx, double hy, double xLeft, double yLower)
        {
            double[][] matrix = new double[4][];
            Point xBoundaries = new Point(xLeft, xLeft + hx);
            Point yBoundaries = new Point(yLower, yLower + hy);

            for (int i = 0; i < 4; i++)
            {
                matrix[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    matrix[i][j] = NumericalMethods.GaussIntegration(elemNumber, i, j, xBoundaries, yBoundaries, hx, hy);
            }

            return matrix;
        }

    }
}