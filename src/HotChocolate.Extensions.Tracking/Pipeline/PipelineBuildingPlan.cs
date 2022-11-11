using System.Collections.Generic;
using HotChocolate.Execution.Configuration;

namespace HotChocolate.Extensions.Tracking.Pipeline;

public sealed class PipelineBuildingPlan
{
    public PipelineBuildingPlan(IRequestExecutorBuilder requestExecutorBuilder)
    {
        RepositoryCandidateBuilders = new List<RepositoryCandidateBuilder>();
        RequestExecutorBuilder = requestExecutorBuilder;
    }

    internal List<RepositoryCandidateBuilder> RepositoryCandidateBuilders { get; }
    public IRequestExecutorBuilder RequestExecutorBuilder { get; }
}
