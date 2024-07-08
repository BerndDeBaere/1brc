using System.Globalization;

namespace _1brc;

public class BlockProcessor
{
    private readonly Block _block;

    public BlockProcessor(Block block)
    {
        _block = block;
    }

    public void Process(Span<byte> data, bool ensureStart = false)
    {
        if (data.Length == 0)
            return;
        int index = ensureStart ? 0 : data.IndexOf((byte)'\n') + 1;
        _block.Trim.AddRange(data.Slice(0, index).ToArray());
        start:
        int lenght = data.Slice(index).IndexOf((byte)'\n');
        if (lenght <= 0) goto end;
        Span<byte> line = data.Slice(index, lenght);
        ProcessLine(line);
        index += lenght + 1;
        goto start;
        end:
        _block.Trim.AddRange(data.Slice(index));
    }

    private void ProcessLine(Span<byte> data)
    {
        int lenght = data.IndexOf((byte)';');
        var city = data.Slice(0, lenght).ToArray();
        var value = decimal.Parse(data.Slice(lenght + 1), CultureInfo.InvariantCulture);
        if (_block.Results.TryGetValue(city, out var existingCityResult))
        {
            existingCityResult.Count++;
            existingCityResult.Sum += value;
            existingCityResult.Min = existingCityResult.Min < value ? existingCityResult.Min : value;
            existingCityResult.Max = existingCityResult.Max > value ? existingCityResult.Max : value;
            return;
        }

        _block.Results.Add(city, new CityResult(city, value));
    }
}