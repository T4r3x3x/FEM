namespace Grid.Models;

public record Area<T>(
    T XLeft,
    T XRight,
    T YBottom,
    T YTop,
    T ZBack,
    T ZFront,
    int FormulaNumber,
    EAreaType AreaType)
{
    public Area(T[] nums, int formulaNumber, EAreaType areaType) : this(
        nums[0],
        nums[1],
        nums[2],
        nums[3],
        nums[4],
        nums[5],
        formulaNumber,
        areaType)
    {
    }

    public T[] ToArray() => [XLeft, XRight, YBottom, YTop, ZBack, ZFront];


}

public enum EAreaType
{
    Common,
    Well
}