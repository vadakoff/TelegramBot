namespace Utilites;

struct Unit
{
    public string Capacity;
    public double Value;
}

public class HumanBytes
{
    public static readonly double KB = 1024;
    public static readonly double MB = Math.Pow(KB, 2);
    public static readonly double GB = Math.Pow(KB, 3);

    public static ValueTuple<double, string> Calculate(double? value)
    {
        var units = new List<Unit>
        {
            new Unit { Capacity = "Gb", Value = GB },
            new Unit { Capacity = "Mb", Value = MB },
            new Unit { Capacity = "Kb", Value = KB },
        };

        foreach (Unit unit in units)
        {
            if (value > unit.Value)
            {
                var calculated = (double)(value / unit.Value);
                return (calculated, unit.Capacity);
            }
        }

        return ((double)value!, "B");
    }
}
