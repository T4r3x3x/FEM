namespace Grid.Models.InputModels
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Q">коэффициент разрядки</param>
    /// <param name="SplitsCount"></param>
    public record AxisInputParameters(double[] Q, int[] SplitsCount);
}
