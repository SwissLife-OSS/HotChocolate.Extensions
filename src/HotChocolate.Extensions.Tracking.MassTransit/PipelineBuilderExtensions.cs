using System;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate.Extensions.Tracking.Pipeline;

namespace HotChocolate.Extensions.Tracking.MassTransit;

public static class PipelineBuilderExtensions
{
    public static PipelineBuilder UseMassTransitRepository(
        this PipelineBuilder builder,
        MassTransitOptions options)
    {
        builder.Services.AddIntegrationBus(options);
        builder.GetRepository = (IServiceProvider prov) =>
        {
            return new MassTransitRepository(
                prov.GetRequiredService<IMassTransitTrackingBus>());
        };

        return builder;
    }
}
