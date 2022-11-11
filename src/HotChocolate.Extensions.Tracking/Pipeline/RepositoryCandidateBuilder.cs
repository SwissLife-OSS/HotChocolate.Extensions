using System;
using System.Collections.Generic;

namespace HotChocolate.Extensions.Tracking.Pipeline;

public class RepositoryCandidateBuilder: PipelineBuilder
{
    private readonly List<Type> _supportedTypes;

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
    public IReadOnlyList<Type> SupportedTypes => _supportedTypes;

    public RepositoryCandidateBuilder AddSupportedType<T>()
        where T: ITrackingEntry
    {
        _supportedTypes.Add(typeof(T));
        ForAll = false;

        return this;
    }
}
