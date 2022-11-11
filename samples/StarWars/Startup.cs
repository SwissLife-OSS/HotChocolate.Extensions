using HotChocolate.Extensions.Tracking.FieldsLifetime;
using HotChocolate.Extensions.Tracking.Pipeline;
using HotChocolate.Extensions.Translation.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StarWars.Repositories;
using StarWars.Tracking;

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

                .AddHttpContextAccessor()

                /* HotChocolate.Extensions.Tracking Pipeline */
                .AddTrackingPipeline(builder => builder
                      .AddRepository<DeprecatedFieldsTracingRepository>()
                        .AddSupportedType<DeprecatedFieldTrace>())
                .AddSingleton<DeprecatedFieldsTracingRepository>()

                .AddGraphQLServer()
                    .AddGraphQLSchema()
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
