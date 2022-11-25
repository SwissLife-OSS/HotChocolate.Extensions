using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HotChocolate.Extensions.Tracking.Persistence;

/// <summary>
/// Reads tracking messages from the ThreadChannel and posts them to MassTransit
/// </summary>
public sealed class TrackingHostedService : BackgroundService
{
    private readonly ChannelReader<TrackingMessage> _channelReader;
    private readonly ITrackingExporterFactory _trackingRepositoryFactory;
    private readonly ILogger<TrackingHostedService> _logger;

    public TrackingHostedService(
        Channel<TrackingMessage> trackingChannel,
        ITrackingExporterFactory trackingRepositoryFactory,
        ILogger<TrackingHostedService> logger)
    {
        _channelReader = trackingChannel.Reader;
        _trackingRepositoryFactory = trackingRepositoryFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Tracking Pipeline Started");

        await foreach (TrackingMessage message in ReadAllAsync(stoppingToken))
        {
            _logger.LogInformation("Saving tracking message", message.TrackingEntry);
            await _trackingRepositoryFactory.Create(message.TrackingEntry.GetType())
                .SaveTrackingEntryAsync(message.TrackingEntry, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Tracking Pipeline Started");
        return Task.CompletedTask;
    }

    private async IAsyncEnumerable<TrackingMessage> ReadAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (await _channelReader.WaitToReadAsync(cancellationToken)
                .ConfigureAwait(false))
        {
            while (_channelReader.TryRead(out TrackingMessage? item))
            {
                yield return item;
            }
        }
    }
}
