using HotChocolate.Extensions.Tracking.MassTransit;
using MassTransit;

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddIntegrationBus(
        this IServiceCollection services,
        MassTransitOptions options)
    {
        services.AddMassTransit<IMassTransitTrackingBus>(s =>
        {
            if (options.ServiceBus.ConnectionString == "InMemory")
            {
                s.UsingInMemory((_, _) => { });
            }
            else
            {
                s.UsingAzureServiceBus((_, cfg) =>
                {
                    cfg.Host(options.ServiceBus.ConnectionString);
                });
            }
        });

        return services;
    }
}
