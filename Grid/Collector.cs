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
using System.Xml;

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
            GetTimeConditions();
        }

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
            //SubstractM();
            Master.Slau.A.Reset();
            Master.Slau.b.Reset();
        }

        static void ResetSlau()
        {
            Master.Slau.A.Reset();
            Master.Slau.b.Reset();
        }

        public void SwitchTask(Solver solver)
        {
            ResetSlau();
            _G.Reset();

            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++) //y
                for (int i = 0; i < Grid.N - 1; i++) //x | проходим по КЭ 
                {
                    if (!IsBorehole(i, j))
                    {
                        localMatrix = GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);

                        for (int p = 0; p < 4; p++)
                            for (int k = 0; k < 4; k++)
                                localMatrix[p][k] *= Grid.Lamda2;

                        AddLocalMatrix(_G, localMatrix, i, j);
                    }
                }
           
            SolveSecondTimeLayer(solver);
            ResetSlau();

       //     Master.Slau.A += _G + _H;

            deltaTimes[2] = Grid.ht[1] + Grid.ht[0];
            deltaTimes[1] = Grid.ht[0];
            deltaTimes[0] = Grid.ht[1];

            MQs[1] = _M * Master.Slau.q[0];
        }

        private void SolveSecondTimeLayer(Solver solver)
        {
            double deltaT = Grid.ht[0];
            Master.Slau.A = _M * 1/deltaT + _G + _H;
            //AccountingBoreholes();
            Master.Slau.b += _M * 1/deltaT * Master.Slau.q[0];
            GetBoundaryConditions(1);
            Master.Slau.q[1]=solver.Solve(Master.Slau.A,Master.Slau.b);
        }

        private void GetMatrixesMG()
        {
            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++) //все кэ под скважиной
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                {
                    if (!IsBorehole(i, j))
                    {
                        localMatrix = GetMassMatrix(Grid.hx[i], Grid.hy[j]);
                        for (int p = 0; p < localMatrix.Length; p++)
                            for (int k = 0; k < localMatrix.Length; k++)
                                localMatrix[p][k] *= Grid.Sigma;
                        AddLocalMatrix(_M, localMatrix, i, j);

                        localMatrix = GetStiffnessMatrix(Grid.hx[i], Grid.hy[j]);
                        
                        double a = Grid.Lamda(Grid.GetAreaNumber(i, j));                        

                        for (int p = 0; p < localMatrix.Length; p++)                        
                            for (int k = 0; k < localMatrix.Length; k++)
                                localMatrix[p][k] *= a;

                        AddLocalMatrix(_G, localMatrix, i, j);
                    }
                }
        }

        bool IsBorehole(int xIndex, int yIndex)
        {
            foreach (var borehole in Grid.boreholes)
            {
                if (xIndex == borehole[0])
                    if (yIndex == borehole[1])
                        return true;
            }
           
            return false;
        }


        public void GetMatrixH()
        {
            double[][] localMatrix;
            for (int j = 0; j < Grid.M - 1; j++)
                for (int i = 0; i < Grid.N - 1; i++) // проходим по КЭ 
                {
                    if (!IsBorehole(i, j))
                    {
                        localMatrix = GetGradTMatrix(LocalNumToGlobal(i, j, 0), Grid.hx[i], Grid.hy[j], Grid.x[i], Grid.y[j], Grid.GetAreaNumber(i,j));
                        for (int p = 0; p < localMatrix.Length; p++)
                            for (int k = 0; k < localMatrix.Length; k++)
                                localMatrix[p][k] *= 0;
                        AddLocalMatrix(_H, localMatrix, i, j);
                    }
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
        static void MultiplyLocalMatrix(Matrix matrix, int i, int j, double a)
        {
            for (int p = 0; p < 4; p++)
            {
                matrix.di[LocalNumToGlobal(i, j, p)] *= a;

                int ibeg = matrix.ia[LocalNumToGlobal(i, j, p)];
                int iend = matrix.ia[LocalNumToGlobal(i, j, p) + 1];
                for (int k = 0; k < p; k++)
                {
                    int index = BinarySearch(matrix.ja, LocalNumToGlobal(i, j, k), ibeg, iend - 1);

                    matrix.au[index] *= a;
                    matrix.al[index] *= a;
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
         //   for (int j = 0; j < Grid.M - 1; j++)
           //     for (int i = 0; i < Grid.N - 1; i++)
            //        MultiplyLocalMatrix(_G, i, j, Grid.Lamda(0));

                    //  Master.Slau.A += _G * Grid.Lamda(0);
                    Master.Slau.A += _G;


        }

        static void MakeSLau(int timeLayer)
        {
            deltaTimes[1] = deltaTimes[0];
            deltaTimes[0] = Grid.ht[timeLayer - 1];// Grid.t[timeLayer] - Grid.t[timeLayer - 1];
            deltaTimes[2] = Grid.ht[timeLayer - 1] + Grid.ht[timeLayer - 2];//Grid.t[timeLayer] - Grid.t[timeLayer - 2];

            MQs[0] = MQs[1];
            MQs[1] = _M * Master.Slau.q[timeLayer - 1];

            timeCoef = ((deltaTimes[2] + deltaTimes[0]) / (deltaTimes[2] * deltaTimes[0]));

            Master.Slau.b += -(deltaTimes[0] / (deltaTimes[2] * deltaTimes[1])) * MQs[0] + deltaTimes[2] / (deltaTimes[1] * deltaTimes[0]) * MQs[1];
            Master.Slau.A += _M * timeCoef + _H + _G;
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
            for (int i = 0; i < Grid.boreholes.Length; i++)
            {
                //снизу
                Master.Slau.b.Elements[Grid.boreholes[i][0] + Grid.boreholes[i][1] * Grid.N] += Grid.hx[Grid.boreholes[i][0]] / 6 * (2 * Master.BoreholePower() + Master.BoreholePower());
                Master.Slau.b.Elements[Grid.boreholes[i][0] + Grid.boreholes[i][1] * Grid.N + 1] += Grid.hx[Grid.boreholes[i][0]] / 6 * (Master.BoreholePower() + 2 * Master.BoreholePower());

                //сверху
                Master.Slau.b.Elements[Grid.boreholes[i][0] + (Grid.boreholes[i][1] + 1) * Grid.N] += Grid.hx[Grid.boreholes[i][0]] / 6 * (2 * Master.BoreholePower() + Master.BoreholePower());
                Master.Slau.b.Elements[Grid.boreholes[i][0] + (Grid.boreholes[i][1] + 1) * Grid.N + 1] += Grid.hx[Grid.boreholes[i][0]] / 6 * (Master.BoreholePower() + 2 * Master.BoreholePower());

                //слева 
                Master.Slau.b.Elements[Grid.boreholes[i][0] + Grid.boreholes[i][1] * Grid.N] += Grid.hy[Grid.boreholes[i][0]] / 6 * (2 * Master.BoreholePower() + Master.BoreholePower());
                Master.Slau.b.Elements[Grid.boreholes[i][0] + (Grid.boreholes[i][1] + 1) * Grid.N] += Grid.hy[Grid.boreholes[i][0]] / 6 * (Master.BoreholePower() + 2 * Master.BoreholePower());

                //справа
                Master.Slau.b.Elements[Grid.boreholes[i][0] + Grid.boreholes[i][1] * Grid.N + 1] += Grid.hy[Grid.boreholes[i][0]] / 6 * (2 * Master.BoreholePower() + Master.BoreholePower());
                Master.Slau.b.Elements[Grid.boreholes[i][0] + (Grid.boreholes[i][1] + 1) * Grid.N + 1] += Grid.hy[Grid.boreholes[i][0]] / 6 * (Master.BoreholePower() + 2 * Master.BoreholePower());
            }

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
            for (int i = 0; i < Grid.N; i++)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
                //  Master.Slau.A.di[i] = C;
                //   Master.Slau.b.Elements[i] = C * Master.Func2(Grid.x[i], Grid.y[0], Grid.t[timeLayer]);
            }

            //верхняя граница
            for (int i = Master.Slau.A.Size - 1; i > Master.Slau.A.Size - Grid.N - 1; i--)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
            }


            //левая гравнь
            for (int i = Grid.N; i < Master.Slau.A.Size - Grid.N - 1; i += Grid.N)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
            }

            //правая граница
            for (int i = 2 * Grid.N - 1; i < Master.Slau.A.Size - 1; i += Grid.N)
            {
                ZeroingRow(i);
                Master.Slau.A.di[i] = 1;
                Master.Slau.b.Elements[i] = Master.TemperatureAtBoundary();
            }

            AccountingBoreholes();
        }

        static void AccountingBoreholes()
        {
            for (int i = 0; i < Grid.boreholes.Length; i++)
            {
                ZeroingRow(Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0]);
                ZeroingRow(Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0] + 1);
                ZeroingRow((Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0]);
                ZeroingRow((Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0] + 1);

                Master.Slau.A.di[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0]] = 1;
                Master.Slau.A.di[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0] + 1] = 1;
                Master.Slau.A.di[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0]] = 1;
                Master.Slau.A.di[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0] + 1] = 1;

                Master.Slau.b.Elements[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0]] = Master.TemperatureInBorehole();
                Master.Slau.b.Elements[Grid.boreholes[i][1] * Grid.N + Grid.boreholes[i][0] + 1] = Master.TemperatureInBorehole();
                Master.Slau.b.Elements[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0]] = Master.TemperatureInBorehole();
                Master.Slau.b.Elements[(Grid.boreholes[i][1] + 1) * Grid.N + Grid.boreholes[i][0] + 1] = Master.TemperatureInBorehole();

            }
        }


        static double[][] GetGradTMatrix(int elemNumber, double hx, double hy, double xLeft, double yLower, int area)
        {
            double[][] matrix = new double[4][];
            Point xBoundaries = new Point(xLeft, xLeft + hx);
            Point yBoundaries = new Point(yLower, yLower + hy);

            for (int i = 0; i < 4; i++)
            {
                matrix[i] = new double[4];
                for (int j = 0; j < 4; j++)
                    matrix[i][j] = NumericalMethods.GaussIntegration(elemNumber, i, j, xBoundaries, yBoundaries, hx, hy,area);
            }

            return matrix;
        }

    }
}