namespace HotChocolate.Extensions.Tracking.Persistence
{
    public interface ITrackingRepositoryFactory
    {
        ITrackingRepository Create<T>() where T : ITrackingEntry;
    }
}