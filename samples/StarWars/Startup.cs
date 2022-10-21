using HotChocolate.Extensions.Translation;
using HotChocolate.Extensions.Translation.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarWars.Characters;
using StarWars.Repositories;
using StarWars.Reviews;

namespace StarWars
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddRouting()

                .AddSingleton<ICharacterRepository, CharacterRepository>()
                .AddSingleton<IReviewRepository, ReviewRepository>()
                .AddSingleton<IResourcesProvider, DictionaryResourcesProvider>()

                .AddGraphQLServer()

                    .AddQueryType()
                        .AddTypeExtension<CharacterQueries>()
                        .AddTypeExtension<ReviewQueries>()
                    .AddMutationType()
                        .AddTypeExtension<ReviewMutations>()
                    .AddSubscriptionType()
                        .AddTypeExtension<ReviewSubscriptions>()

                    .AddType<Human>()
                        .AddType<HumanType>()
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

                    .InitializeOnStartup();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseHeaderLocalization()
                .UseWebSockets()
                .UseRouting()
                .UseEndpoints(endpoint => endpoint.MapGraphQL());
        }
    }
}
