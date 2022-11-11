using System.Diagnostics.Tracing;
using Thor.Core.Abstractions;

namespace HotChocolate.Extensions.Translation.EventSources
{
    /// <summary>
    /// Represents the eventsource for translation events
    /// </summary>
    [EventSourceDefinition(Name = "Translations")]
    public interface ITranslationEventSource
    {
        /// <summary>
        /// This event signals that a RMS resource is missing
        /// </summary>
        [Event(1, Level = EventLevel.Warning,
            Message = "Translated resource is missing: {key}", Version = 1)]
        void ResourceMissing(string key);
    }
}
