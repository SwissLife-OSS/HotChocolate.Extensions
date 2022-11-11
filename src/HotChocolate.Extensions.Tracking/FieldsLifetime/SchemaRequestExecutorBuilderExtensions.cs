using System;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public static class SchemaRequestExecutorBuilderExtensions
    {
        public static IRequestExecutorBuilder TryAddDeprecatedFieldsTracking(
            this IRequestExecutorBuilder builder)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Services
                .AddSingleton<IDeprecatedFieldsTrackingEntryFactory,
                    DeprecatedFieldsTrackingEntryFactory>();

            return builder
                .TryAddTypeInterceptor<DeprecatedFieldsTypeInterceptor>();
        }
    }
}
