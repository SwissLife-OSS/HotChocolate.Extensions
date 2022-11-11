using HotChocolate.Extensions.Tracking.Pipeline;

namespace HotChocolate.Extensions.Tracking.MassTransit;

public static class PipelineBuilderExtensions
{
    public static PipelineBuilder UseMassTransitRepository(
        this PipelineBuilder builder,
        MassTransitOptions options)
    {
        builder.Services.AddIntegrationBus(options);
        builder.AddRepository<MassTransitRepository>();

        return builder;
    }
}
