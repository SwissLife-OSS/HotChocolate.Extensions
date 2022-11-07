using System;
using System.Diagnostics.Tracing;
using HotChocolate.Extensions.Tracking;
using Thor.Core.Abstractions;

namespace SwissLife.GraphQL.Extensions.Tracking.EventSources
{
    /// <summary>
    /// Represents the eventsource for swisslife tracking events
    /// </summary>
    [EventSourceDefinition(Name = "SwissLife-Tracking")]
    public interface ITrackingEventSource
    {
        /// <summary>
        /// This event signals that an exception happened during
        /// an attempt to create a Tracking entry
        /// </summary>
        [Event(1, Level = EventLevel.Error,
            Message = "Exception during attempt at tracking on field {fieldName}.", Version = 1)]
        void ExceptionDuringTracking(string fieldName, Exception ex);

        /// <summary>
        /// This event signals that the tracking pipeline started.
        /// </summary>
        [Event(2, Level = EventLevel.Informational,
            Message = "Tracking pipeline started.", Version = 1)]
        void TrackingPipelineStarted();

        /// <summary>
        /// This event signals that the tracking pipeline was stopped.
        /// </summary>
        [Event(3, Level = EventLevel.Informational,
            Message = "Tracking pipeline was stopped.", Version = 1)]
        void TrackingPipelineStopped();

        /// <summary>
        /// This event signals that a new Tracking Entry was saved.
        /// </summary>
        [Event(4, Level = EventLevel.Verbose,
            Message = "Saving a new TrackingEntry.", Version = 1)]
        void SavingTrackingMessage(ITrackingEntry tag);
    }
}
