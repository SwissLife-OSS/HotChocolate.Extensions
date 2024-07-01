using System;
using System.Collections.Generic;
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

        public static IRequestExecutorBuilder AddStringLocalizer<T>(
            this IRequestExecutorBuilder builder,
            ServiceLifetime serviceLifetime,
            params Type[] resourceTypes)
            where T : class, IStringLocalizer
        {
            builder.Services.AddStringLocalizer<T>(serviceLifetime, resourceTypes);

            return builder;
        }

        public static IRequestExecutorBuilder AddStringLocalizer(
           this IRequestExecutorBuilder builder,
           ServiceLifetime serviceLifetime,
           Type stringLocalizerType,
           params Type[] resourceTypes)
        {
            builder.Services.AddStringLocalizer(serviceLifetime, stringLocalizerType, resourceTypes);

            return builder;
        }

        internal static void AddStringLocalizer<T>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime,
            params Type[] resourceTypes)
            where T : class, IStringLocalizer
        {
            services.TryAddSingleton<IResourceTypeResolver>(new DefaultResourceTypeResolver());

            services.TryAddSingleton<IStringLocalizerFactory, DefaultLocalizerFactory>();

            foreach (Type resourceType in resourceTypes)
            {
                services.UpdateResourceTypeResolver(typeof(T), resourceType);
            }

            services.TryAdd(
                new ServiceDescriptor(
                    typeof(T),
                    typeof(T),
                    serviceLifetime));
        }

        internal static void AddStringLocalizer(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime,
            Type stringLocalizerType,
            params Type[] resourceTypes)
        {
            if (!stringLocalizerType.IsClass || stringLocalizerType.IsAbstract)
            {
                throw new ArgumentException("It supports non abstract classes only.",
                    nameof(stringLocalizerType));
            }

            if (!typeof(IStringLocalizer).IsAssignableFrom(stringLocalizerType))
            {
                throw new ArgumentException(
                    $"The {stringLocalizerType.Name} should implement IStringLocalizer interface.",
                    nameof(stringLocalizerType));
            }

            services.TryAddSingleton<IResourceTypeResolver>(new DefaultResourceTypeResolver());

            services.TryAddSingleton<IStringLocalizerFactory, DefaultLocalizerFactory>();

            bool isOpenGeneric =
                stringLocalizerType.IsGenericType &&
                stringLocalizerType.ContainsGenericParameters;

            foreach (Type resourceType in resourceTypes)
            {
                services.UpdateResourceTypeResolver(
                    isOpenGeneric
                    ? stringLocalizerType.MakeGenericType(resourceType)
                    : stringLocalizerType,
                    resourceType);
            }

            services.TryAdd(
                new ServiceDescriptor(stringLocalizerType, stringLocalizerType, serviceLifetime));
        }

        private static void UpdateResourceTypeResolver(
            this IServiceCollection services, Type localizer, Type resourceType)
        {
            ServiceDescriptor? descriptor = services.FirstOrDefault(x =>
                x.Lifetime == ServiceLifetime.Singleton
                && x.ServiceType == typeof(IResourceTypeResolver)
                && x.ImplementationInstance?.GetType() == typeof(DefaultResourceTypeResolver));

            DefaultResourceTypeResolver typeResolver =
                (DefaultResourceTypeResolver)descriptor!.ImplementationInstance!;

            typeResolver.RegisterType(resourceType, localizer);
        }
    }
}
