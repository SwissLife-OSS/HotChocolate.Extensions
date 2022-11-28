using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Extensions.Tracking.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public static class PipelineBuilderExtensions
    {
        public static IRequestExecutorBuilder AddDeprecatedFieldsTracking<TExporter>(
            this PipelineBuilder builder)
            where TExporter : class, ITrackingExporter
        {
            builder.AddDeprecatedFieldsExporter<TExporter>();

            builder.Services
                .AddSingleton<IDeprecatedFieldsTrackingEntryFactory,
                    DeprecatedFieldsTrackingEntryFactory>();

            return builder.BuildPlan.RequestExecutorBuilder
                .TryAddTypeInterceptor<DeprecatedFieldsTypeInterceptor>();
        }
    }
}
