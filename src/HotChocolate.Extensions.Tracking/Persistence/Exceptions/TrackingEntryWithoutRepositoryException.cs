using System;

namespace HotChocolate.Extensions.Tracking.Persistence.Exceptions
{
    internal class TrackingEntryWithoutRepositoryException : Exception
    {
        public TrackingEntryWithoutRepositoryException(Type type)
            : base($"No registered tracking repository can handle " +
                  $"the tracking entry of type {type.Name}")
        {
            Type = type;
        }

        public Type Type { get; }
    }
}
