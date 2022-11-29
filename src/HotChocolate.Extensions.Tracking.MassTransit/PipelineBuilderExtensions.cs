using HotChocolate.Extensions.Tracking.MassTransit;
using HotChocolate.Extensions.Tracking.Pipeline;

namespace Microsoft.Extensions.DependencyInjection;

public static class PipelineBuilderExtensions
{
    public static PipelineBuilder AddMassTransitExporter(
        this PipelineBuilder builder,
        MassTransitOptions options)
    {
        builder.Services.AddIntegrationBus(options);
        builder.AddExporter<MassTransitExporter>();

        return builder;
    }
}
