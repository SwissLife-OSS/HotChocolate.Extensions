using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Extensions.Tracking.Persistence;

namespace HotChocolate.Extensions.Tracking.MassTransit;

public class MassTransitExporter : ITrackingExporter
{
    private readonly IMassTransitTrackingBus _bus;

    public MassTransitExporter(IMassTransitTrackingBus bus)
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
