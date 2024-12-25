using Tools;

namespace MathModels.Models;

public class Matrix : ICloneable
{
    public double[] Di;
    public double[] Al, Au;
    public int[] Ja, Ia;

    public Matrix(double[] di, double[] al, double[] au, int[] ja, int[] ia)
    {
        Di = di;
        Al = al;
        Au = au;
        Ja = ja;
        Ia = ia;
    }

    public int Size => Di.Length;

    public double[][] ConvertToDenseFormat()
    {
        var matrix = new double[Size][];
        for (var i = 0; i < Size; i++)
        {
            matrix[i] = new double[Size];
        }

        for (var i = 0; i < Size; i++)
        {
            matrix[i][i] = Di[i];
            for (var j = Ia[i]; j < Ia[i + 1]; j++)
            {
                matrix[i][Ja[j]] = Al[j];
                matrix[Ja[j]][i] = Au[j];
            }
        }

        return matrix;
    }

    public void ZeroingRow(int row)
    {
        for (var i = Ia[row]; i < Ia[row + 1]; i++)
            Al[i] = 0;
        for (var j = row + 1; j < Size; j++)
        {
            var jbeg = Ia[j];
            var jend = Ia[j + 1];
            //   if (jbeg >= Au.Length) continue;
            var index = SearchingAlghoritms.BinarySearch(Ja, row, jbeg, jend - 1);
            if (index != -1)
                Au[index] = 0;
        }
    }

    public static Vector operator *(Matrix matrix, Vector vector)
    {
        if (matrix.Size != vector.Length)
            throw new("Sizes of the matrix and vector aren'T equals");

        var result = new Vector(vector.Length);

        for (var i = 0; i < vector.Length; i++)
            result[i] = matrix.Di[i] * vector[i];


        for (var i = 0; i < vector.Length; i++)
        {
            for (var j = matrix.Ia[i]; j < matrix.Ia[i + 1]; j++)
            {
                result[i] += matrix.Al[j] * vector[matrix.Ja[j]];
                result[matrix.Ja[j]] += matrix.Au[j] * vector[i];
            }
        }

        return result;
    }

    public static Matrix operator *(double b, Matrix matrix)
    {
        var result = new Matrix(matrix.Di, matrix.Al, matrix.Au, matrix.Ja, matrix.Ia);

        for (var i = 0; i < matrix.Size; i++)
            result.Di[i] = matrix.Di[i] * b;

        for (var i = 0; i < matrix.Au.Count(); i++)
            result.Au[i] = matrix.Au[i] * b;

        for (var i = 0; i < matrix.Al.Count(); i++)
            result.Al[i] = matrix.Al[i] * b;

        return result;
    }

    public static Matrix operator /(Matrix matrix, double b)
    {
        var result = new Matrix(matrix.Di, matrix.Al, matrix.Au, matrix.Ja, matrix.Ia);

        for (var i = 0; i < matrix.Size; i++)
            result.Di[i] = matrix.Di[i] / b;

        for (var i = 0; i < matrix.Au.Count(); i++)
            result.Au[i] = matrix.Au[i] / b;

        for (var i = 0; i < matrix.Al.Count(); i++)
            result.Al[i] = matrix.Al[i] / b;

        return result;
    }

    public static Matrix operator +(Matrix a, Matrix b)
    {
        var result = new Matrix(a.Di, a.Al, a.Au, a.Ja, a.Ia);

        for (var i = 0; i < a.Size; i++)
            result.Di[i] = a.Di[i] + b.Di[i];

        for (var i = 0; i < a.Au.Count(); i++)
            result.Au[i] = a.Au[i] + b.Au[i];

        for (var i = 0; i < a.Al.Count(); i++)
            result.Al[i] = a.Al[i] + b.Al[i];

        return result;
    }

    public static Matrix operator -(Matrix a, Matrix b)
    {
        var result = new Matrix(a.Di, a.Al, a.Au, a.Ja, a.Ia);

        for (var i = 0; i < a.Size; i++)
            result.Di[i] = a.Di[i] - b.Di[i];

        for (var i = 0; i < a.Au.Count(); i++)
            result.Au[i] = a.Au[i] - b.Au[i];

        for (var i = 0; i < a.Al.Count(); i++)
            result.Al[i] = a.Al[i] - b.Al[i];

        return result;
    }

    public void Print()
    {
        var matrix = ConvertToDenseFormat();
        for (var i = 0; i < matrix.Length; i++)
        {
            for (var j = 0; j < matrix.Length; j++)
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
        for (var i = 0; i < Size; i++)
            Di[i] = 0;

        for (var i = 0; i < Au.Count(); i++)
            Au[i] = 0;

        for (var i = 0; i < Al.Count(); i++)
            Al[i] = 0;
    }

    public object Clone() => new Matrix(Di, Al, Au, Ja, Ia);
}