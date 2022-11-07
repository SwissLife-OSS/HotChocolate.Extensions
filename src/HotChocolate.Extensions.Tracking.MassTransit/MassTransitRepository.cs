using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Extensions.Tracking.Pipeline;

namespace HotChocolate.Extensions.Tracking.MassTransit;

public class MassTransitRepository : ITrackingRepository
{
    private readonly IMassTransitTrackingBus _bus;

    public MassTransitRepository(IMassTransitTrackingBus bus)
    {
        _bus = bus;
    }

    public async Task SaveTrackingEntryAsync(
        ITrackingEntry trackingEntry,
        CancellationToken cancellationToken)
    {
        await _bus.Publish(trackingEntry, trackingEntry.GetType(), cancellationToken);
    }
}