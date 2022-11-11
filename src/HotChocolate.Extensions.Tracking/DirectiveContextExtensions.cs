using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;
using static SwissLife.GraphQL.Extensions.Tracking.EventSources.TrackingEventSource;

namespace HotChocolate.Extensions.Tracking;

internal static class DirectiveContextExtensions
{
    internal static async Task SubmitTrack(
        this IMiddlewareContext context,
        ITrackingEntryFactory trackingEntryFactory,
        CancellationToken cancellationToken)
    {
        IHttpContextAccessor httpContextAccessor = context.Service<IHttpContextAccessor>();
        Channel<TrackingMessage> channel = context.Service<Channel<TrackingMessage>>();

        ITrackingEntry? trackingEntry =
            trackingEntryFactory.CreateTrackingEntry(
                httpContextAccessor,
                context);

        if (trackingEntry != null)
        {
            await channel.Writer.WriteAsync(
               new TrackingMessage(trackingEntry),
               cancellationToken);
        }
    }

    internal static void LogAndReportError(
        this IMiddlewareContext context, Exception ex)
    {
        context.ReportError(
            ErrorBuilder.New()
                .SetException(ex)
                .SetMessage(ex.Message)
                .Build());

        Log.ExceptionDuringTracking(context.Selection.Field.Name, ex);
    }
}
