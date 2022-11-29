using HotChocolate.Types;

namespace HotChocolate.Extensions.Tracking.TagTracking;

public static class ObjectFieldDescriptorExtensions
{
    /// <summary>
    /// Makes a Field tracked with the default
    /// TrackingEntry structure { email DateTimeOffset tag action }
    /// </summary>
    /// <param name="fieldDescriptor">the HotChocolate Field</param>
    /// <param name="trackingTag">The tag that shall be saved.</param>
    /// <returns></returns>
    /// <remarks>
    /// A tracked Field is always tracked. The caller does not need to add
    /// the @track directive to it in his query
    /// </remarks>
    public static IObjectFieldDescriptor Track(
        this IObjectFieldDescriptor fieldDescriptor,
        string trackingTag)
        => fieldDescriptor.Track(new TagTrackingEntryFactory(trackingTag));
}
