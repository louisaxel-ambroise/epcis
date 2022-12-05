using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FasTnT.EfCore.Store.Configuration;

public class ArrayComparer : ValueComparer<string[]>
{
    public ArrayComparer()
        : base(
            (c1, c2) => c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
            c => c.ToArray()
        )
    {
    }
}