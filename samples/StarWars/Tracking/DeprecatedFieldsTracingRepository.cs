using HotChocolate.Extensions.Tracking;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using HotChocolate.Extensions.Tracking.Persistence;
using Newtonsoft.Json;

namespace StarWars.Tracking
{
    public class DeprecatedFieldsTracingRepository : ITrackingRepository
    {
        private readonly ILoggerFactory _loggerFactory;

        public DeprecatedFieldsTracingRepository(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry,
            CancellationToken cancellationToken)
        {
            _loggerFactory.CreateLogger<DeprecatedFieldsTracingRepository>()
                .LogWarning($"Use of deprecated field: {JsonConvert.SerializeObject(trackingEntry)}");
            return Task.CompletedTask;
        }
    }
}
