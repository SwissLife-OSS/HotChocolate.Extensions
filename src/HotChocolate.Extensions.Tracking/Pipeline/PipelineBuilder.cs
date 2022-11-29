using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Tracking.FieldsLifetime;
using HotChocolate.Extensions.Tracking.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking.Pipeline;

public class PipelineBuilder
{
    internal PipelineBuildingPlan BuildPlan { get; init; }

    protected PipelineBuilder(
        PipelineBuildingPlan buildPlan)
    {
        BuildPlan = buildPlan;
    }

    internal PipelineBuilder(
        IRequestExecutorBuilder requestExecutorBuilder)
    {
        BuildPlan = new PipelineBuildingPlan(requestExecutorBuilder);
    }

    public virtual ExporterCandidateBuilder AddExporter<TExporter>()
        where TExporter : class, ITrackingExporter
    {
        var builder = new ExporterCandidateBuilder(
            BuildPlan, typeof(TExporter));
        BuildPlan.ExporterCandidateBuilders.Add(builder);

        Services.AddSingleton<TExporter>();

        return builder;
    }

    public virtual ExporterCandidateBuilder AddDeprecatedFieldsExporter<TExporter>()
        where TExporter : class, ITrackingExporter
    {
        ExporterCandidateBuilder builder = AddExporter<TExporter>();
        builder.AddSupportedType<DeprecatedFieldTrace>();

        return builder;
    }

    public IServiceCollection Services => BuildPlan.RequestExecutorBuilder.Services;
}
