using HotChocolate.Types;

namespace HotChocolate.Extensions.Tracking;

/// <summary>
/// TrackDirective + TrackableDirective work together:
/// - TrackableDirective can be used to decorate a field to indicate that it's
/// trackable and to provide the necessary context
/// - TrackDirective can be sent by the client to trigger the tracking of that Field.
/// </summary>
public sealed class TrackableDirectiveType : DirectiveType<TrackableDirective>
{
    internal const string DirectiveName = "trackable";

    protected override void Configure(IDirectiveTypeDescriptor<TrackableDirective> descriptor)
    {
        descriptor.Name(DirectiveName);
        descriptor.Location(DirectiveLocation.FieldDefinition);
        descriptor.BindArgumentsExplicitly();
    }
}
