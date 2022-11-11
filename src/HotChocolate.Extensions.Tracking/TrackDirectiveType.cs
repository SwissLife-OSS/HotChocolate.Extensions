using System;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Extensions.Tracking;
using HotChocolate.Extensions.Tracking.Exceptions;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace HotChocolate.Extensions.Tracking;

/// <summary>
/// TrackDirective + TrackableDirective work together:
/// - TrackableDirective can be used to decorate a field to indicate that it's
/// trackable and to provide the necessary context
/// - TrackDirective can be sent by the client to trigger the tracking of that Field.
/// </summary>
public sealed class TrackDirectiveType : DirectiveType<TrackDirective>
{
    private const string DirectiveName = "track";

    protected override void Configure(
        IDirectiveTypeDescriptor<TrackDirective> descriptor)
    {
        descriptor.Name(DirectiveName);
        descriptor.Location(DirectiveLocation.Field);

        descriptor.Use(next => context => Track(next, context));
    }

    internal async ValueTask Track(
        FieldDelegate next,
        IDirectiveContext context)
    {
        await next.Invoke(context);

        try
        {
            TrackDirective trackDirectivePayload
                = context.Directive.ToObject<TrackDirective>();

            if (!trackDirectivePayload.If.GetValueOrDefault(true))
            {
                return;
            }

            TrackableDirective trackableDirectiveOptions
                = GetTrackableDirective(context);

            await context.SubmitTrack(
                trackableDirectiveOptions.TrackingEntryFactory,
                context.RequestAborted);
        }
        catch (Exception ex)
        {
            context.LogAndReportError(ex);
        }
    }

    private static TrackableDirective GetTrackableDirective(
        IDirectiveContext context)
    {
        IDirective? directive = null;
        foreach (IDirective? d in context.Selection.Field.Directives)
        {
            if (d.Name.Equals(TrackableDirectiveType.DirectiveName))
            {
                directive = d;
                break;
            }
        }

        if (directive is null)
        {
            throw new FieldNotTrackableException(context.Selection.Field.Name);
        }

        return directive.ToObject<TrackableDirective>();
    }
}
