using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Tracking;

public static class SchemaBuilderExtensions
{
    public static IRequestExecutorBuilder RegisterTracking(
        this IRequestExecutorBuilder builder)
        => builder.ConfigureSchema(s => s.RegisterTracking());

    private static void RegisterTracking(this ISchemaBuilder builder)
    {
        builder
            .AddDirectiveType<TrackableDirectiveType>()
            .AddDirectiveType<TrackDirectiveType>()
            .AddDirectiveType<TrackedDirectiveType>();
    }
}
