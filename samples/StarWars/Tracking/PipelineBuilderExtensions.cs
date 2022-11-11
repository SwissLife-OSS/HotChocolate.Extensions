using HotChocolate.Extensions.Tracking.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace StarWars.Tracking
{
    public static class PipelineBuilderExtensions
    {
        public static PipelineBuilder UseDummyRepository(
            this PipelineBuilder builder)
        {
            builder.GetRepository = (IServiceProvider prov) =>
            {
                return new DummyTrackingRepository(
                    prov
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger<DummyTrackingRepository>());
            };

            return builder;
        }
    }

}
