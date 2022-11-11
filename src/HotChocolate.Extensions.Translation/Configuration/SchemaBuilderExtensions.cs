using HotChocolate;
using HotChocolate.Extensions.Translation;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SchemaBuilderExtensions
    {
        public static ISchemaBuilder AddTranslation(this ISchemaBuilder builder)
        {
            builder.AddDirectiveType<TranslateDirectiveType>();
            builder.AddDirectiveType<TranslatableDirectiveType>();

            return builder;
        }

        public static ISchemaBuilder AddAdditionalTranslation<T>(
            this ISchemaBuilder builder)
            where T : notnull
        {
            builder.AddDirectiveType<TranslateDirectiveType<T>>();

            return builder;
        }
    }
}
