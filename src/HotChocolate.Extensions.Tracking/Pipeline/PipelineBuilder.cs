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

    public virtual RepositoryCandidateBuilder AddRepository<TRepository>()
        where TRepository : class, ITrackingRepository
    {
        var builder = new RepositoryCandidateBuilder(
            BuildPlan, typeof(TRepository));
        BuildPlan.RepositoryCandidateBuilders.Add(builder);

        Services.AddSingleton<TRepository>();

        return builder;
    }

    public virtual RepositoryCandidateBuilder AddDeprecatedFieldsRepository<TRepository>()
        where TRepository : class, ITrackingRepository
    {
        RepositoryCandidateBuilder builder = AddRepository<TRepository>();
        builder.AddSupportedType<DeprecatedFieldTrace>();

        return builder;
    }

    public IServiceCollection Services => BuildPlan.RequestExecutorBuilder.Services;
}
