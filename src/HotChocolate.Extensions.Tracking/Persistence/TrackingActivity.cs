using System.Diagnostics;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    internal static class TrackingActivity
    {
        private static readonly ActivitySource ActivitySource
            = new("HotChocolate.Extensions.Tracking");

        public static Activity? StartTrackingEntityHandling()
        {
            Activity? activity = ActivitySource.StartActivity(
                "Tracking entity.");

            return activity;
        }
    }
}
