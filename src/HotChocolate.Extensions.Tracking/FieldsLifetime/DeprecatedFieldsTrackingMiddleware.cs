using System;
using System.Threading.Tasks;
using HotChocolate.Resolvers;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    internal class DeprecatedFieldsTrackingMiddleware
    {
        private readonly FieldDelegate _next;

        public DeprecatedFieldsTrackingMiddleware(
            FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            IMiddlewareContext context,
            IDeprecatedFieldsTrackingEntryFactory trackingFactory)
        {
            await _next(context).ConfigureAwait(false);

            try
            {
                await context.SubmitTrack(
                    trackingFactory,
                    context.RequestAborted);
            }
            catch (Exception ex)
            {
                context.LogAndReportError(ex);
            }
        }
    }
}
