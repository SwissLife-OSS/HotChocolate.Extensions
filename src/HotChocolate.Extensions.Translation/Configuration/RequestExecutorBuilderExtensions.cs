using System;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation;
using HotChocolate.Extensions.Translation.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RequestExecutorBuilderExtensions
    {
        private const string DefaultInterfaceName = "Translation";
        
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
                { InterfaceName = DefaultInterfaceName };

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
    }
}
