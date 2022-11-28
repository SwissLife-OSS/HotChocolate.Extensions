using System;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation;
using HotChocolate.Extensions.Translation.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RequestExecutorBuilderExtensions
    {
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
            var translatableDirective = new TranslatableDirectiveType();
            var translationInterfaceType = new TranslationInterfaceType();

            if (configure != null)
            {
                var translationBuilder = new TranslationBuilder(
                    builder, translationInterfaceType);
                configure(translationBuilder);
            }

            IRequestExecutorBuilder b = builder
                .AddDirectiveType(translateDirective)
                .AddDirectiveType(translatableDirective)
                .AddType(translationInterfaceType);

            b.Services.AddSingleton<IResourcesProviderAdapter, ResourcesProviderAdapter>();
            b.Services.AddSingleton<TranslationObserver, DefaultTranslationObserver>();

            return b;
        }
    }
}
