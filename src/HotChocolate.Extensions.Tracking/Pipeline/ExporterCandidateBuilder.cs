using System;
using System.Collections.Generic;

namespace HotChocolate.Extensions.Tracking.Pipeline;

public class ExporterCandidateBuilder: PipelineBuilder
{
    private readonly List<Type> _supportedTypes;

    internal ExporterCandidateBuilder(
        PipelineBuildingPlan buildPlan,
        Type exporterType)
        : base(buildPlan)
    {
        ExporterType = exporterType;
        ForAll = true;
        _supportedTypes = new List<Type>();
    }

    public Type ExporterType { get; }
    public bool ForAll { get; private set; }
    public IReadOnlyList<Type> SupportedTypes => _supportedTypes;

    public ExporterCandidateBuilder AddSupportedType<T>()
        where T: ITrackingEntry
    {
        _supportedTypes.Add(typeof(T));
        ForAll = false;

        return this;
    }
}
