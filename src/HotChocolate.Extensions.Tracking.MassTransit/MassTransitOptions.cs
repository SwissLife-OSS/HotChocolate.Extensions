namespace HotChocolate.Extensions.Tracking.MassTransit;

public class MassTransitOptions
{
    public ServiceBusOptions ServiceBus { get; }

    public MassTransitOptions(ServiceBusOptions serviceBus)
    {
        ServiceBus = serviceBus;
    }
}

public class ServiceBusOptions
{
    public ServiceBusOptions(string connectionString)
    {
        ConnectionString = connectionString;
    }

    public string ConnectionString { get; }
}
