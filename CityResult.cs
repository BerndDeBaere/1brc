using System.Text;

namespace _1brc;

public class CityResult
{
    public CityResult(byte[] city, decimal value)
    {
        Count = 1;
        Sum = value;
        City = city;
        Min = value;
        Max = value;
    }

    public byte[] City { get; private set; }
    public string CityString => Encoding.UTF8.GetString(City);
    public int Count { get; set; }
    public decimal Sum { get; set; }
    public decimal Min { get; set; }
    public string MinString => Min.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
    public decimal Max { get; set; }
    public string MaxString => Max.ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
    
    public string AvgString => Math.Round(Sum / Count, 1).ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
    public override string ToString()
    {
        return $"{CityString}={MinString}/{AvgString}/{MaxString}";
    }
}