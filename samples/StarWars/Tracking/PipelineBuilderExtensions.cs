using HotChocolate.Extensions.Tracking.Pipeline;
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
                return new DummyTrackingRepository();
            };

            return builder;
        }
    }

}
