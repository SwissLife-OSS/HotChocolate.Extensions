using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Extensions.Tracking.Pipeline;
using HotChocolate.Extensions.Tracking.Pipeline.Exceptions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrackingPipeline(
        this IServiceCollection services,
        Action<PipelineBuilder> build)
    {
        var builder = new PipelineBuilder(services);
        build(builder);

        InitThreadChannel(services);
        InitRepositories(services, builder);

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

    private static void InitRepositories(IServiceCollection services, PipelineBuilder builder)
    {
        if (!builder.BuildPlan.RepositoryCandidateBuilders.Any())
        {
            throw new NoTrackingRepositoryConfiguredException();
        }

        if (builder.BuildPlan.RepositoryCandidateBuilders.Count(b => b.ForAll) > 1)
        {
            throw new MoreThanOneGlobalTrackingRepositoryException();
        }

        services.AddSingleton<ITrackingRepositoryFactory>(sp => {

            List<IRepositoryCandidate> candidates = builder.BuildPlan.RepositoryCandidateBuilders
            .Where(b => !b.ForAll)
            .Select(b => (IRepositoryCandidate) new RepositoryCandidate(
                (ITrackingRepository)sp.GetRequiredService(b.RepositoryType),
                b.SupportedTypes))
            .ToList();

            RepositoryCandidateBuilder? fallbackCandidateBuilder
                = builder.BuildPlan.RepositoryCandidateBuilders.SingleOrDefault(b => b.ForAll);
            if(fallbackCandidateBuilder != null)
            {
                var fallbackCandidate
                    = new RepositoryCandidateForAll(
                        (ITrackingRepository)sp.GetRequiredService(
                            fallbackCandidateBuilder.RepositoryType));
                candidates.Add(fallbackCandidate);

            }

            return new TrackingRepositoryFactory(candidates);
        });
    }
}
