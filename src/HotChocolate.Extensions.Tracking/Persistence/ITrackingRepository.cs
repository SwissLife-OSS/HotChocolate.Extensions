using System.Threading;
using System.Threading.Tasks;

namespace HotChocolate.Extensions.Tracking.Persistence;

public interface ITrackingRepository
{
    Task SaveTrackingEntryAsync(
        ITrackingEntry trackingEntry,
        CancellationToken cancellationToken);
}
