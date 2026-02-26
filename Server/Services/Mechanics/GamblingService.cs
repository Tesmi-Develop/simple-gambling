using Hypercube.Utilities.Dependencies;
using Server.ServiceRealisation;
using Shared;

namespace Server.Services;

[Service]
public sealed class GamblingService
{
    [Dependency] private readonly DataTrackerService _dataTrackerService = null!;
        
    public SpinItem Spin()
    {
        var items = _dataTrackerService.Read().SpinItems;
        
        var random = new Random();
        var totalWeight = items.Sum(i => i.Weight);
        var roll =  random.Next(0, totalWeight);

        var cursor = 0;

        foreach (var item in items)
        {
            cursor += item.Weight;
            if (roll < cursor)
                return item;
        }

        return items.Last();
    }
}