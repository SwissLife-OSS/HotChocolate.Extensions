using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Extensions.Tracking.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public static class PipelineBuilderExtensions
    {
        public static IRequestExecutorBuilder AddDeprecatedFieldsTracking<TRepository>(
            this PipelineBuilder builder)
            where TRepository : class, ITrackingExporter
        {
            builder.AddDeprecatedFieldsRepository<TRepository>();

            builder.Services
                .AddSingleton<IDeprecatedFieldsTrackingEntryFactory,
                    DeprecatedFieldsTrackingEntryFactory>();

            return builder.BuildPlan.RequestExecutorBuilder
                .TryAddTypeInterceptor<DeprecatedFieldsTypeInterceptor>();
        }
    }
}
