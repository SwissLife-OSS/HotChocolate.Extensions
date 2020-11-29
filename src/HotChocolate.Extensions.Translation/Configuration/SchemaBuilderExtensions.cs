using HotChocolate;
using HotChocolate.Extensions.Translation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaBuilderExtensions
    {
        public static ISchemaBuilder RegisterTranslation(this ISchemaBuilder builder)
        {
            builder.AddDirectiveType<TranslateDirectiveType>();
            builder.AddDirectiveType<TranslatableDirectiveType>();

            return builder;
        }

        public static ISchemaBuilder RegisterAdditionalTranslation<T>(
            this ISchemaBuilder builder)
        {
            builder.AddDirectiveType<TranslateDirectiveType<T>>();

            return builder;
        }
    }
}
