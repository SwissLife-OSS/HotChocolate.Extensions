using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Extensions.Tracking;
using HotChocolate.Extensions.Tracking.TagTracking;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using StarWars.Repositories;
using StarWars.Tracking;

namespace StarWars.Reviews
{
    /// <summary>
    /// The mutations related to reviews.
    /// </summary>
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class ReviewMutations
    {
        /// <summary>
        /// Creates a review for a given Star Wars episode.
        /// </summary>
        [Track<ReviewTrackingEntryFactory>] // tracking with a custom message
        [Track("NewReview")] // simple tracking with a simple tag+date message
        public async Task<CreateReviewPayload> CreateReview(
            CreateReviewInput input,
            [Service]IReviewRepository repository,
            [Service]ITopicEventSender eventSender)
        {
            var review = new Review(input.Stars, input.Commentary);
            repository.AddReview(input.Episode, review);
            await eventSender
                .SendAsync(input.Episode, review)
                .ConfigureAwait(false);
            return new CreateReviewPayload(input.Episode, review);
        }
    }
}
