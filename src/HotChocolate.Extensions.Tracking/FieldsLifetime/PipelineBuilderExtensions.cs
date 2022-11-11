using System;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Tracking.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public static class PipelineBuilderExtensions
    {
        public static IRequestExecutorBuilder AddDeprecatedFieldsTracking(
            this PipelineBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddSingleton<IDeprecatedFieldsTrackingEntryFactory,
                    DeprecatedFieldsTrackingEntryFactory>();

            return builder.BuildPlan.RequestExecutorBuilder
                .TryAddTypeInterceptor<DeprecatedFieldsTypeInterceptor>();
        }
    }
}
