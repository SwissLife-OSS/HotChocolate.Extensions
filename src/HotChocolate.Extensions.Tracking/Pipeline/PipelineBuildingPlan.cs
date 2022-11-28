using System.Collections.Generic;
using HotChocolate.Execution.Configuration;

namespace HotChocolate.Extensions.Tracking.Pipeline;

public sealed class PipelineBuildingPlan
{
    public PipelineBuildingPlan(IRequestExecutorBuilder requestExecutorBuilder)
    {
        ExporterCandidateBuilders = new List<ExporterCandidateBuilder>();
        RequestExecutorBuilder = requestExecutorBuilder;
    }

    internal List<ExporterCandidateBuilder> ExporterCandidateBuilders { get; }
    public IRequestExecutorBuilder RequestExecutorBuilder { get; }
}
