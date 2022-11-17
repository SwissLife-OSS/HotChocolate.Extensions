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
                    .AddTypeExtension<CharacterQueries>()
                    .AddTypeExtension<ReviewQueries>()
                .AddMutationType()
                    .AddTypeExtension<ReviewMutations>()
                .AddSubscriptionType()
                    .AddTypeExtension<ReviewSubscriptions>()

                .AddType<Human>()
                    .AddType<HumanType>()
                    .AddTypeExtension<HumanTypeExtension>()
                .AddType<Droid>()
                .AddType<Starship>()
                .AddTypeExtension<CharacterResolvers>()

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
                       // .AddSupportedType<ReviewTrackingEntry>()
                    .AddDeprecatedFieldsTracking<DeprecatedFieldsTracingExporter>())

                .ModifyOptions(o => o.SortFieldsByName = true);
        }
    }
}
