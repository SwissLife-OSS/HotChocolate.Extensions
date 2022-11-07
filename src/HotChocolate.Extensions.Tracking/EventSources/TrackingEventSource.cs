using HotChocolate.Extensions.Tracking;
using System;
using System.Diagnostics.Tracing;
using Thor.Core;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace SwissLife.GraphQL.Extensions.Tracking.EventSources
{
/// <summary>
    /// Represents the eventsource for swisslife tracking events
    /// </summary>

    [EventSource(Name = "SwissLife-Tracking")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("thor-generator", "1.0.0")]
    public sealed class TrackingEventSource
        : EventSourceBase
        , ITrackingEventSource
    {
        private TrackingEventSource() { }

        public static ITrackingEventSource Log { get; } = new TrackingEventSource();

/// <summary>
        /// This event signals that an exception happened during
        /// an attempt to create a Tracking entry
        /// </summary>

        [NonEvent]
        public void ExceptionDuringTracking(string fieldName, Exception ex)
        {
            if (IsEnabled())
            {
				var attachmentId = AttachmentId.NewId();

				ExceptionDuringTracking(Application.Id, ActivityStack.Id, attachmentId, fieldName);
                
                AttachmentDispatcher.Instance.Dispatch(
					 AttachmentFactory.Create(attachmentId, "ex", ex)
				);
            }
        }

        [Event(1, Level = EventLevel.Error, Message = "Exception during attempt at tracking on field {2}.", Version = 1)]
        private void ExceptionDuringTracking(int applicationId, Guid activityId, string attachmentId, string fieldName)
        {
            WriteCore(1, applicationId, activityId, attachmentId, fieldName);
        }

/// <summary>
        /// This event signals that the tracking pipeline started.
        /// </summary>

        [NonEvent]
        public void TrackingPipelineStarted()
        {
            if (IsEnabled())
            {

				TrackingPipelineStarted(Application.Id, ActivityStack.Id);
                
            }
        }

        [Event(2, Level = EventLevel.Informational, Message = "Tracking pipeline started.", Version = 1)]
        private void TrackingPipelineStarted(int applicationId, Guid activityId)
        {
            WriteCore(2, applicationId, activityId);
        }

/// <summary>
        /// This event signals that the tracking pipeline was stopped.
        /// </summary>

        [NonEvent]
        public void TrackingPipelineStopped()
        {
            if (IsEnabled())
            {

				TrackingPipelineStopped(Application.Id, ActivityStack.Id);
                
            }
        }

        [Event(3, Level = EventLevel.Informational, Message = "Tracking pipeline was stopped.", Version = 1)]
        private void TrackingPipelineStopped(int applicationId, Guid activityId)
        {
            WriteCore(3, applicationId, activityId);
        }

/// <summary>
        /// This event signals that a new Tracking Entry was saved.
        /// </summary>

        [NonEvent]
        public void SavingTrackingMessage(ITrackingEntry tag)
        {
            if (IsEnabled())
            {
				var attachmentId = AttachmentId.NewId();

				SavingTrackingMessage(Application.Id, ActivityStack.Id, attachmentId);
                
                AttachmentDispatcher.Instance.Dispatch(
					 AttachmentFactory.Create(attachmentId, "tag", tag)
				);
            }
        }

        [Event(4, Level = EventLevel.Verbose, Message = "Saving a new TrackingEntry.", Version = 1)]
        private void SavingTrackingMessage(int applicationId, Guid activityId, string attachmentId)
        {
            WriteCore(4, applicationId, activityId, attachmentId);
        }

        [NonEvent]
        private unsafe void WriteCore(int eventId, int applicationId, Guid activityId, string a, string b)
        {
            if (IsEnabled())
            {
                StringExtensions.SetToEmptyIfNull(ref a);
                StringExtensions.SetToEmptyIfNull(ref b);

                fixed (char* aBytes = a)
                {
                fixed (char* bBytes = b)
                {
                    const short dataCount = 4;
                    EventData* data = stackalloc EventData[dataCount];
                    data[0].DataPointer = (IntPtr)(&applicationId);
                    data[0].Size = 4;
                    data[1].DataPointer = (IntPtr)(&activityId);
                    data[1].Size = 16;
                    data[2].DataPointer = (IntPtr)(aBytes);
                    data[2].Size = ((a.Length + 1) * 2);
                    data[3].DataPointer = (IntPtr)(bBytes);
                    data[3].Size = ((b.Length + 1) * 2);

                    WriteEventCore(eventId, dataCount, data);
                }
                }
            }
        }

        [NonEvent]
        private unsafe void WriteCore(int eventId, int applicationId, Guid activityId)
        {
            if (IsEnabled())
            {

                    const short dataCount = 2;
                    EventData* data = stackalloc EventData[dataCount];
                    data[0].DataPointer = (IntPtr)(&applicationId);
                    data[0].Size = 4;
                    data[1].DataPointer = (IntPtr)(&activityId);
                    data[1].Size = 16;

                    WriteEventCore(eventId, dataCount, data);
            }
        }

    }
}
