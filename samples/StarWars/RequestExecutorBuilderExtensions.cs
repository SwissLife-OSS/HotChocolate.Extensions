using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Tracking.FieldsLifetime;
using Microsoft.Extensions.DependencyInjection;
using StarWars.Characters;
using StarWars.Reviews;
using StarWars.Tracking;

namespace StarWars
{
    public static class RequestExecutorBuilderExtensions
    {
        public static IRequestExecutorBuilder AddGraphQLSchema(this IRequestExecutorBuilder builder)
        {
            return builder
                .AddQueryType()
                .AddStarWarsTypes()
                .AddMutationType()
                .AddSubscriptionType()

                .AddType<Human>()
                .AddType<Droid>()
                .AddType<Starship>()
                .AddUnionType<ISearchResult>()
                .AddInterfaceType<ICharacter>()

                .AddFiltering()
                .AddSorting()

                // Adds Translation Support to the server
                .AddTranslation(c => c
                    .AddTranslatableType<HairColor>()
                    .AddTranslatableType<MaritalStatus>()
                    .AddTranslatableType<Episode>()
                )

                .AddInMemorySubscriptions()

                .AddApolloTracing()

                /* HotChocolate.Extensions.Tracking Pipeline */
                .AddTrackingPipeline(builder => builder
                    .AddExporter<KpiTrackingExporter>()
                    .AddDeprecatedFieldsTracking<DeprecatedFieldsTracingExporter>())

                .ModifyOptions(o => o.SortFieldsByName = true);
        }
    }
}
