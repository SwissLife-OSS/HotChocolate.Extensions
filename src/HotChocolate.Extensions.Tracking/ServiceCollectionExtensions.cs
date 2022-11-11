using System;
using System.Threading.Channels;
using HotChocolate.Extensions.Tracking.Exceptions;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Extensions.Tracking.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HotChocolate.Extensions.Tracking;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrackingPipeline(
        this IServiceCollection services,
        Action<PipelineBuilder> build)
    {
        var builder = new PipelineBuilder(services);
        build(builder);

        InitThreadChannel(services);
        InitRepository(services, builder);

        services.AddSingleton<IHostedService, TrackingHostedService>();
        return services;
    }

    private static void InitThreadChannel(IServiceCollection services)
    {
        /* In-Memory Thread Channel that will be used to exchange tracking
         * messages between the tracking directive and the HostedService*/
        var channel = Channel.CreateUnbounded<TrackingMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true
            });
        services.AddSingleton(channel);
    }

    private static void InitRepository(IServiceCollection services, PipelineBuilder builder)
    {
        if (builder.GetRepository == default)
        {
            throw new NoTrackingRepositoryConfiguredException();
        }

        services.AddSingleton(prov => builder.GetRepository(prov));
    }
}
