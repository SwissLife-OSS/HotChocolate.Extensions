using System;
using System.Linq;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation;
using HotChocolate.Extensions.Translation.Resources;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RequestExecutorBuilderExtensions
    {
        private const string DefaultTranslationInterfaceName = "Translation";
        
        public static IRequestExecutorBuilder AddTranslation(
            this IRequestExecutorBuilder builder)
        {
            return builder.AddTranslation(null);
        }

        public static IRequestExecutorBuilder AddTranslation(
            this IRequestExecutorBuilder builder,
            Action<TranslationBuilder>? configure)
        {
            var translateDirective = new TranslateDirectiveType();
            var translationInterfaceType = new TranslationInterfaceType
                { InterfaceName = DefaultTranslationInterfaceName };

            if (configure != null)
            {
                var translationBuilder = new TranslationBuilder(
                    builder, translationInterfaceType);
                configure(translationBuilder);
            }

            IRequestExecutorBuilder b = builder
                .AddDirectiveType(translateDirective)
                .AddType(translationInterfaceType);

            b.Services.AddSingleton<IResourcesProviderAdapter, ResourcesProviderAdapter>();
            b.Services.AddSingleton<TranslationObserver, DefaultTranslationObserver>();

            return b;
        }

        public static IRequestExecutorBuilder AddStringLocalizerFactory(
            this IRequestExecutorBuilder builder)
        {
            builder.Services.AddDefaultStringLocalizerFactory();

            return builder;
        }

        internal static void AddDefaultStringLocalizerFactory(
            this IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizerFactory, DefaultLocalizerFactory>();
        }

        public static IRequestExecutorBuilder AddStringLocalizer<T>(
            this IRequestExecutorBuilder builder,
            ServiceLifetime serviceLifetime,
            params Type[] resourceTypes)
            where T : class, IStringLocalizer
        {
            builder.Services.AddStringLocalizer<T>(serviceLifetime, resourceTypes);

            return builder;
        }

        internal static void AddStringLocalizer<T>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime,
            params Type[] resourceTypes)
            where T : class, IStringLocalizer
        {
            if (serviceLifetime == ServiceLifetime.Transient)
            {
                services.TryAddTransient<T>();
                services.TryAddTransient(typeof(IStringLocalizer<T>), typeof(T));
            }
            else if (serviceLifetime == ServiceLifetime.Scoped)
            {
                services.TryAddScoped<T>();
                services.TryAddScoped(typeof(IStringLocalizer<T>), typeof(T));
            }
            else
            {
                services.TryAddSingleton<T>();
                services.TryAddSingleton(typeof(IStringLocalizer<T>), typeof(T));
            }

            services.AddOrUpdateResourceTypeResolver(typeof(T), resourceTypes);
        }

        private static void AddOrUpdateResourceTypeResolver(
            this IServiceCollection services, Type localizer, params Type[] resourceTypes)
        {
            ServiceDescriptor? descriptor = services.FirstOrDefault(x =>
                x.Lifetime == ServiceLifetime.Singleton
                && x.ServiceType == typeof(IResourceTypeResolver)
                && x.ImplementationType == typeof(DefaultResourceTypeResolver));

            DefaultResourceTypeResolver typeResolver;

            if (descriptor is null)
            {
                typeResolver = new();

                services.AddSingleton<IResourceTypeResolver>(typeResolver);
            }
            else
            {
                typeResolver = (DefaultResourceTypeResolver)descriptor.ImplementationInstance!;
            }

            foreach (Type resourceType in resourceTypes)
            {
                typeResolver.RegisterType(resourceType, localizer);
            }
        }
    }
}
