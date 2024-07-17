using Consumer.Domain.Products.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Consumer.Infrastructure.Products.ValueConverters;

public class CentreIdsValueComparer : ValueComparer<HashSet<CentreId>>
{
    public CentreIdsValueComparer() 
        : base(
            (c1, c2) => 
                (c1 == null && c2 == null) || 
                (c1 != null && c2 != null && c1.SetEquals(c2)),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), 
            c => c.ToHashSet())
    { }
}
