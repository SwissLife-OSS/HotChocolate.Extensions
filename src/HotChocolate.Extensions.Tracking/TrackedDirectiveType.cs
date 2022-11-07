using System;
using System.Threading.Tasks;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace HotChocolate.Extensions.Tracking;

/// <summary>
/// Standalone directive that can be added by the Backend on a field
/// to ALWAYS track calls to this field.
/// </summary>
public sealed class TrackedDirectiveType : DirectiveType<TrackedDirective>
{
    private const string DirectiveName = "tracked";

    protected override void Configure(
        IDirectiveTypeDescriptor<TrackedDirective> descriptor)
    {
        descriptor.Name(DirectiveName);
        descriptor.Location(DirectiveLocation.FieldDefinition);
        descriptor.BindArgumentsExplicitly();

        descriptor.Use(next => context => Track(next, context));
    }

    private async ValueTask Track(
        FieldDelegate next,
        IDirectiveContext context)
    {
        // first run the field's resolver pipeline to it's end
        await next.Invoke(context);

        try
        {
            TrackedDirective trackedDirectivePayload =
                context.Directive.ToObject<TrackedDirective>();

            await context.SubmitTrack(
                trackedDirectivePayload.TrackingEntryFactory,
                context.RequestAborted);
        }
        catch (Exception ex)
        {
            context.LogAndReportError(ex);
        }
    }
}
