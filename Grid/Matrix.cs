using ReaserchPaper;
using System.Drawing;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace ResearchPaper
{
    class Matrix
    {
        public int Size => di.Length;
        public double[] di;
        public List<double> al, au;
        public List<int> ja, ia;
        public Matrix(int size)
        {
            di = new double[size];
            ia = new List<int>(size+1);
            al = new List<double>();
            au = new List<double>();
            ja = new List<int>();
            GenerateSLAU(size);
        }

      

        int GetNodeNumber(int nodelocalNumber,int elemNumber)
        {
            int yLine = elemNumber / (Grid.N - 1);//в каком ряду по у находится кэ 
            int xLine = elemNumber - yLine * (Grid.N-1);//в каком ряду по x находится кэ 
            int xOffset = 0, yOffset = 0;
            switch (nodelocalNumber)
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

            return xLine + xOffset + (yLine + yOffset ) *  Grid.N;
        }

        public double[][] Convert()
        {
            double[][] matrix = new double[Size][];
            for (int i = 0; i < Size; i++)
            {
                matrix[i] = new double[Size];
            }

            for (int i = 0; i < Size; i++)
            {
                matrix[i][i] = di[i];
                for (int j = ia[i]; j < ia[i+1]; j++)
                {
                    matrix[i][ja[j]] = al[j];
                    matrix[ja[j]][i] = au[j];
                }
            }

            return matrix;
          
        }


        void GenerateSLAU(int size)
        {
            int memory = size * 8;
            List<List<int>> list = new List<List<int>>(2);
            list.Add(new List<int>(memory));
            list.Add(new List<int>(memory));
            list[0].AddRange(Enumerable.Repeat(0, list[0].Capacity));
            list[1].AddRange(Enumerable.Repeat(0, list[1].Capacity));
            ja = new List<int>(memory);
            List<int> listbeg = new List<int>(size);
            listbeg.AddRange(Enumerable.Repeat(0,listbeg.Capacity));
            int listSize = 0;

            for (int t = 0; t < Grid.ElementsCount; t++)
            {
                for (int i = 0; i < 4; i++)
                {
                    int k = GetNodeNumber(i,t);

                    for (int j = i + 1; j < 4; j++)
                    {
                        int ind1 = k;

                        int ind2 = GetNodeNumber(j, t);
                        if (ind2 < ind1)
                        {
                            ind1 = ind2;
                            ind2 = k;
                        }
                        int iaddr = listbeg[ind2];
                        if (iaddr == 0)
                        {
                            listSize++;
                            listbeg[ind2] = listSize;
                            list[0][listSize] = ind1;
                            list[1][listSize] = 0;
                        }
                        else
                        {
                            while (list[0][iaddr] < ind1 && list[1][iaddr] > 0)
                            {
                                iaddr = list[1][iaddr];
                            }
                            if (list[0][iaddr] > ind1)
                            {
                                listSize++;
                                list[0][listSize] = list[0][iaddr];
                                list[1][listSize] = list[1][iaddr];
                                list[0][iaddr] = ind1;
                                list[1][iaddr] = listSize;
                            }
                            else
                            {
                                if (list[0][iaddr] < ind1)
                                {
                                    listSize++;
                                    list[1][iaddr] = listSize;
                                    list[0][listSize] = ind1;
                                    list[1][listSize] = 0;
                                }
                            }
                        }
                    }
                }
            }

            this.ia.Add(0);
            for (int i = 0; i < size; i++)
            {
                ia.Add(ia[i]);
                int iaddr = listbeg[i];
                while (iaddr != 0)
                {
                    ja.Add(list[0][iaddr]);
                    ia[i + 1]++;
                    iaddr = list[1][iaddr];
                }
            }
            al = new List<double>(ja.Count());
            al.AddRange(Enumerable.Repeat(0.0, al.Capacity));
            au = new List<double>(ja.Count());
            au.AddRange(Enumerable.Repeat(0.0, au.Capacity));
        }

        public static Vector operator *(Matrix matrix, Vector vector)
        {
            if (matrix.Size == vector.Length)
            {
                Vector result = new Vector(vector.Length);

                for (int i = 0; i < vector.Length; i++)
                    result.Elements[i] = matrix.di[i] * vector.Elements[i];


                for (int i = 0; i < vector.Length; i++)
                {
                    for (int j = matrix.ia[i]; j < matrix.ia[i + 1]; j++)
                    {
                        result.Elements[i] += matrix.al[j] * vector.Elements[matrix.ja[j]];
                        result.Elements[matrix.ja[j]] += matrix.au[j] * vector.Elements[i];
                    }
                }

                return result;
            }
            else
                throw new Exception("Sizes of the matrix and vector aren't equals");
        }
        public static Matrix operator *(Matrix matrix, double b)
        {
            Matrix result = new Matrix(matrix.Size);

            for (int i = 0; i < matrix.Size; i++)
                result.di[i] = matrix.di[i]* b;

            for (int i = 0; i < matrix.au.Count(); i++)
                result.au[i] = matrix.au[i] * b;

            for (int i = 0; i < matrix.al.Count(); i++)
                result.al[i] = matrix.al[i] * b;

            return result;
        }

        public static Matrix operator /(Matrix matrix, double b)
        {
            Matrix result = new Matrix(matrix.Size);

            for (int i = 0; i < matrix.Size; i++)
                result.di[i] = matrix.di[i] / b;

            for (int i = 0; i < matrix.au.Count(); i++)
                result.au[i] = matrix.au[i] / b;

            for (int i = 0; i < matrix.al.Count(); i++)
                result.al[i] = matrix.al[i] / b;

            return result;
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(a.Size);

            for (int i = 0; i< a.Size; i++)
                result.di[i] = a.di[i] + b.di[i];

            for (int i = 0; i < a.au.Count(); i++)
                result.au[i] = a.au[i] + b.au[i];

            for (int i = 0; i < a.al.Count(); i++)
                result.al[i] = a.al[i] + b.al[i];

            return result;
        }

        public static Matrix operator -(Matrix a, Matrix b)
        {
            Matrix result = new Matrix(a.Size);

            for (int i = 0; i < a.Size; i++)
                result.di[i] = a.di[i] - b.di[i];

            for (int i = 0; i < a.au.Count(); i++)
                result.au[i] = a.au[i] - b.au[i];

            for (int i = 0; i < a.al.Count(); i++)
                result.al[i] = a.al[i] - b.al[i];

            return result;
        }

        public void Print()
        {
            double[][] matrix = this.Convert();
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                {
                    if (matrix[i][j] >= 0)
                        Console.Write(" ");
                    Console.Write(matrix[i][j].ToString("E2") + " ");
                }               
                Console.WriteLine();
            }
            Console.WriteLine("\n\n");
        }

        public void Reset()
        {
            for (int i = 0; i < Size; i++)
                di[i] = 0;

            for (int i = 0; i < au.Count(); i++)
                au[i] = 0;

            for (int i = 0; i < al.Count(); i++)
                al[i] = 0;
        }
    }
}