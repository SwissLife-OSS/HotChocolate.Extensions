using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using static SwissLife.GraphQL.Extensions.Tracking.EventSources.TrackingEventSource;

namespace HotChocolate.Extensions.Tracking.Persistence;

/// <summary>
/// Reads tracking messages from the ThreadChannel and posts them to MassTransit
/// </summary>
public sealed class TrackingHostedService : BackgroundService
{
    private readonly ChannelReader<TrackingMessage> _channelReader;
    private readonly ITrackingExporterFactory _trackingRepositoryFactory;

    public TrackingHostedService(
        Channel<TrackingMessage> trackingChannel,
        ITrackingExporterFactory trackingRepositoryFactory)
    {
        _channelReader = trackingChannel.Reader;
        _trackingRepositoryFactory = trackingRepositoryFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.TrackingPipelineStarted();

        await foreach (TrackingMessage message in ReadAllAsync(stoppingToken))
        {
            Log.SavingTrackingMessage(message.TrackingEntry);
            await _trackingRepositoryFactory.Create(message.TrackingEntry.GetType())
                .SaveTrackingEntryAsync(message.TrackingEntry, stoppingToken);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Log.TrackingPipelineStopped();
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
