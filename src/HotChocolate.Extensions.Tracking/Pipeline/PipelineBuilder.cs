using System;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking.Pipeline;

public sealed class PipelineBuilder
{
    internal PipelineBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public Func<IServiceProvider, ITrackingRepository>? GetRepository { get; set; }

    public IServiceCollection Services { get; }
}
