using HotChocolate.Extensions.Tracking.Pipeline;
using HotChocolate.Extensions.Tracking;
using System.Threading.Tasks;
using System.Threading;

namespace StarWars.Tracking
{
    public class DummyTrackingRepository : ITrackingRepository
    {
        public Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            //TODO
            return Task.CompletedTask;
        }
    }
}
