using System.Diagnostics;

namespace HotChocolate.Extensions.Tracking.Persistence
{
    internal static class TrackingActivity
    {
        private static readonly ActivitySource ActivitySource = new("Tracking");

        public static Activity? StartTrackingEntityHandling()
        {
            Activity? activity = ActivitySource.StartActivity(
                "Start handling message in Background Service.");

            return activity;
        }
    }
}
