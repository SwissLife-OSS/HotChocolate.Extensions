using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private readonly ITrackingExporterFactory _trackingExporterFactory;
    private readonly ILogger<TrackingHostedService> _logger;

    public TrackingHostedService(
        Channel<TrackingMessage> trackingChannel,
        ITrackingExporterFactory trackingExporterFactory,
        ILogger<TrackingHostedService> logger)
    {
        _channelReader = trackingChannel.Reader;
        _trackingExporterFactory = trackingExporterFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Tracking Pipeline started.");

        await foreach (TrackingMessage message in _channelReader.ReadAllAsync(stoppingToken))
        {
            using Activity? activity = TrackingActivity.StartTrackingEntityHandling();

            try
            {
                await _trackingExporterFactory.Create(message.TrackingEntry.GetType())
                    .SaveTrackingEntryAsync(message.TrackingEntry, stoppingToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    $"Error on attempt to export Message: {message?.TrackingEntry?.Date}", ex);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Tracking Pipeline stopped.");
        return Task.CompletedTask;
    }
}
