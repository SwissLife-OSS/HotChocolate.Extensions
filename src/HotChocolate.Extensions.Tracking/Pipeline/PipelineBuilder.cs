using System;
using System.Collections.Generic;
using HotChocolate.Extensions.Tracking.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking.Pipeline;

public class PipelineBuilder
{
    internal PipelineBuildingPlan BuildPlan { get; init; }

    protected PipelineBuilder(PipelineBuildingPlan buildPlan)
    {
        BuildPlan = buildPlan;
    }

    internal PipelineBuilder(IServiceCollection services)
    {
        BuildPlan = new PipelineBuildingPlan(services);
    }

    public virtual RepositoryCandidateBuilder AddRepository<TRepository>()
        where TRepository : class, ITrackingRepository
    {
        var builder = new RepositoryCandidateBuilder(BuildPlan, typeof(TRepository));
        BuildPlan.RepositoryCandidateBuilders.Add(builder);
        return builder;
    }

    //public Func<IServiceProvider, ITrackingRepository>? GetRepository { get; set; }

    public IServiceCollection Services => BuildPlan.Services;
}

public sealed class PipelineBuildingPlan
{
    public PipelineBuildingPlan(IServiceCollection services)
    {
        Services = services;
        RepositoryCandidateBuilders = new List<RepositoryCandidateBuilder>();
    }

    internal List<RepositoryCandidateBuilder> RepositoryCandidateBuilders { get; }
    public IServiceCollection Services { get; }
}

public class RepositoryCandidateBuilder: PipelineBuilder
{
    private readonly IList<Type> _supportedTypes;

    internal RepositoryCandidateBuilder(
        PipelineBuildingPlan buildPlan,
        Type repositoryType)
        : base(buildPlan)
    {
        RepositoryType = repositoryType;
        ForAll = true;
        _supportedTypes = new List<Type>();
    }

    public Type RepositoryType { get; }
    public bool ForAll { get; private set; }
    public IReadOnlyList<Type> SupportedTypes => SupportedTypes;

    public RepositoryCandidateBuilder AddSupportedType<T>()
        where T: ITrackingEntry
    {
        _supportedTypes.Add(typeof(T));
        ForAll = false;

        return this;
    }
}
