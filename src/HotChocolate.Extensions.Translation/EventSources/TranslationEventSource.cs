using System;
using System.Diagnostics.Tracing;
using Thor.Core;
using Thor.Core.Abstractions;
using Thor.Core.Transmission.Abstractions;

namespace HotChocolate.Extensions.Translation.EventSources
{
/// <summary>
    /// Represents the eventsource for translation events
    /// </summary>

    [EventSource(Name = "Translations")]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("thor-generator", "1.0.0")]
    public sealed class TranslationEventSource
        : EventSourceBase
        , ITranslationEventSource
    {
        private TranslationEventSource() { }

        public static ITranslationEventSource Log { get; } = new TranslationEventSource();

/// <summary>
        /// This event signals that a RMS resource is missing
        /// </summary>

        [NonEvent]
        public void ResourceMissing(string key)
        {
            if (IsEnabled())
            {

				ResourceMissing(Application.Id, ActivityStack.Id, key);
                
            }
        }

        [Event(1, Level = EventLevel.Warning, Message = "Translated resource is missing: {2}", Version = 1)]
        private void ResourceMissing(int applicationId, Guid activityId, string key)
        {
            WriteCore(1, applicationId, activityId, key);
        }

    }
}
