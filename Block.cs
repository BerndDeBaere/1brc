using System.Collections;

namespace _1brc;

public class Block
{
    public Block(int order, long from, long size)
    {
        Order = order;
        From = from;
        Size = size;
    }

    public int Order { get; set; }
    public long From { get; private set; }
    public long Size { get; private set; }
    public long AlreadyRead { get; set; }

    public Dictionary<byte[], CityResult> Results { get; private set; } = new(new ByteArrayComparer());
    public List<byte> Trim { get; set; } = new();
}

public class ByteArrayComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        if (x == y) return true;
        if (x == null || y == null) return false;
        if (x.Length != y.Length) return false;

        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i]) return false;
        }

        return true;
    }

    public int GetHashCode(byte[]? obj)
    {
        if (obj == null) return 0;
        int hash = 17;
        foreach (var b in obj)
        {
            hash = hash * 31 + b;
        }

        return hash;
    }
}