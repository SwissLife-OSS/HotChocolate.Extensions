using System;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation;

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
            Action<TranslationBuilder>? configure
            )
        {
            IRequestExecutorBuilder b = builder
                .AddDirectiveType<TranslateDirectiveType>()
                .AddDirectiveType<TranslatableDirectiveType>();

            if(configure != null)
            {
                var translationBuilder = new TranslationBuilder(b);
                configure(translationBuilder);
            }

            return b;
        }
    }
}
