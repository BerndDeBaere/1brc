namespace _1brc;

public static class AggregateBlocks
{
    public static void CombineDictionary(this Dictionary<byte[], CityResult> final, Dictionary<byte[], CityResult> input)
    {
        foreach (var inputItem in input)
        {
            if (final.TryGetValue(inputItem.Key, out CityResult exsistingItem))
            {
                exsistingItem.Count += inputItem.Value.Count;
                exsistingItem.Sum += inputItem.Value.Sum;
                exsistingItem.Min = decimal.Min(exsistingItem.Min, inputItem.Value.Min);
                exsistingItem.Max = decimal.Max(exsistingItem.Max, inputItem.Value.Max);
            }
            else
            {
                final.Add(inputItem.Key, inputItem.Value);
            }
        }
    }
}