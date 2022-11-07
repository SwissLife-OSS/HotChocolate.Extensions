using MassTransit;

namespace HotChocolate.Extensions.Tracking.MassTransit;

/// <summary>
/// Specific Bus instance for the Tracking via MassTransit
/// </summary>
/// <see cref="https://masstransit-project.com/usage/containers/multibus.html"/>
public interface IMassTransitTrackingBus : IBus
{
}
