using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            IMiddlewareContext context)
        {
            await _next(context).ConfigureAwait(false);

            try
            {
                //TODO inject factory
                await context.SubmitTrack(
                    new DeprecatedFieldsTrackingEntryFactory(),
                    context.RequestAborted);
            }
            catch (Exception ex)
            {
                context.LogAndReportError(ex);
            }
        }
    }
}
