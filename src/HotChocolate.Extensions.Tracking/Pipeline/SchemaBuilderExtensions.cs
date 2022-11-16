using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Tracking;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Extensions.Tracking.Pipeline;
using HotChocolate.Extensions.Tracking.Pipeline.Exceptions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class SchemaBuilderExtensions
{
    public static IRequestExecutorBuilder AddTrackingPipeline(
        this IRequestExecutorBuilder requestExecutorBuilder,
        Action<PipelineBuilder> build)
    {
        var trackingBuilder = new PipelineBuilder(requestExecutorBuilder);
        build(trackingBuilder);

        InitThreadChannel(requestExecutorBuilder.Services);
        InitRepositories(requestExecutorBuilder.Services, trackingBuilder);

        requestExecutorBuilder.AddTrackingDirectives();
        requestExecutorBuilder.Services.AddSingleton<IHostedService, TrackingHostedService>();

        return requestExecutorBuilder;
    }

    private static IRequestExecutorBuilder AddTrackingDirectives(
        this IRequestExecutorBuilder builder)
    {
        return builder
            .AddDirectiveType<TrackedDirectiveType>();
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