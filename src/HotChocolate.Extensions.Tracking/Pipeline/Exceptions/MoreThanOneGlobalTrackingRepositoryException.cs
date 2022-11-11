using System;

namespace HotChocolate.Extensions.Tracking.Pipeline.Exceptions
{
    public class MoreThanOneGlobalTrackingRepositoryException : Exception
    {
        public MoreThanOneGlobalTrackingRepositoryException()
            : base("More than one Repository was declared without declaring its supported types.")
        {
        }
    }
}
