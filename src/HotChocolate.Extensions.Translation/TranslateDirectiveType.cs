using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Extensions.Translation.Exceptions;
using HotChocolate.Extensions.Translation.Resources;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation
{
    public class TranslateDirectiveType : TranslateDirectiveType<string>
    {
    }

    public class TranslateDirectiveType<T> : DirectiveType<TranslateDirective<T>>
        where T : notnull
    {
        private string DirectiveName { get; }

        public TranslateDirectiveType()
        {
            DirectiveName = typeof(T) == typeof(string)
                ? "translate"
                : $"translate{typeof(T).Name}";
        }

        protected override void Configure(
            IDirectiveTypeDescriptor<TranslateDirective<T>> descriptor)
        {
            descriptor.Name(DirectiveName);
            descriptor.Location(DirectiveLocation.Field);
            descriptor.Location(DirectiveLocation.FieldDefinition);
            descriptor.Argument(t => t.Language).Type<TranslatableLanguageType>();

            descriptor.Use(next => context => Translate(next, context));
        }

        internal static async ValueTask Translate(
            FieldDelegate next,
            IDirectiveContext context)
        {
            await next.Invoke(context);
            var value = context.Result;

            if (value == null)
            {
                return;
            }

            try
            {
                TranslatableLanguage language
                    = context.Directive.ToObject<TranslateDirective<T>>().Language;
                TranslatableDirective directiveOptions = GetDirectiveOptions(context);

                UpdateResult(context, value, directiveOptions, language);
            }
            catch (TranslationException ex)
            {
                context.ReportError(
                    ErrorBuilder.New()
                        .SetException(ex)
                        .SetMessage(ex.Message)
                        .Build());
                throw;
            }
        }

        internal static void UpdateResult(
            IMiddlewareContext context,
            object value,
            TranslatableDirective directiveOptions,
            TranslatableLanguage language = TranslatableLanguage.NotSet)
        {
            CultureInfo culture = DetermineOutputCulture(language);

            IResourcesProvider client = context.Service<IResourcesProvider>();

            if (value is IEnumerable<T> items)
            {
                var rItems = items.Where(t => t != null).ToList();
                TranslateArray(context, directiveOptions, culture, client, rItems);
            }
            else if (directiveOptions.ToCodeLabelItem && value is T t)
            {
                TranslateToCodeLabelItem(context, directiveOptions, culture, client, t);
            }
            else
            {
                TranslateFieldToString(context, value, directiveOptions, culture, client);
            }
        }

        private static void TranslateFieldToString(
            IMiddlewareContext context,
            object value,
            TranslatableDirective directiveOptions,
            CultureInfo culture,
            IResourcesProvider client)
        {
            if (value is string s)
            {
                TranslateString(
                    context, value, directiveOptions, culture, client, s);
            }
            else if (value is sbyte
                || value is byte
                || value is short
                || value is ushort
                || value is int
                || value is uint
                || value is long
                || value is ulong)
            {
                var sValue = value.ToString()
                    ?? throw new TranslationNotSupportedForTypeException(
                            value.GetType(), context.Field);

                TranslateString(
                    context, value, directiveOptions, culture, client, sValue);
            }
            else if (value is Enum e)
            {
                TranslateEnum(
                    context, value, directiveOptions, culture, client, e);
            }
            else
            {
                throw new TranslationNotSupportedForTypeException(
                    value.GetType(),
                    context.Field);
            }
        }

        private static void TranslateArray(
            IMiddlewareContext context,
            TranslatableDirective directiveOptions,
            CultureInfo culture,
            IResourcesProvider client,
            IReadOnlyList<T> items)
        {
            if (directiveOptions.ToCodeLabelItem)
            {
                TranslateToCodeLabelArray(
                    context, directiveOptions, culture, client, items);
            }
            else
            {
                TranslateToStringArray(
                    context, directiveOptions, culture, client, items);
            }
        }

        private static void TranslateEnum(
            IMiddlewareContext context,
            object value,
            TranslatableDirective directiveOptions,
            CultureInfo culture,
            IResourcesProvider client,
            Enum e)
        {
            context.Result = client.TryGetResourceAsString(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                e.ToString());
        }

        private static void TranslateString(
            IMiddlewareContext context,
            object value,
            TranslatableDirective directiveOptions,
            CultureInfo culture,
            IResourcesProvider client,
            string s)
        {
            context.Result = client.TryGetResourceAsString(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                s);
        }

        private static void TranslateToStringArray(
            IMiddlewareContext context,
            TranslatableDirective directiveOptions,
            CultureInfo culture,
            IResourcesProvider client,
            IReadOnlyList<T> items)
        {
            context.Result = items
                .Select(t => client.TryGetResourceAsString(
                        $"{directiveOptions.ResourceKeyPrefix}/{t}",
                        culture,
                        t.ToString()))
                .ToList();
        }

        private static void TranslateToCodeLabelArray(
            IMiddlewareContext context,
            TranslatableDirective directiveOptions,
            CultureInfo culture,
            IResourcesProvider client,
            IReadOnlyList<T> items)
        {
            context.Result = items
                .Select(item => new TranslatedResource<T>(
                    item,
                    client.TryGetResourceAsString(
                        $"{directiveOptions.ResourceKeyPrefix}/{item}",
                        culture,
                        item.ToString())
                )).ToList();
        }

        private static void TranslateToCodeLabelItem(
            IMiddlewareContext context,
            TranslatableDirective directiveOptions,
            CultureInfo culture,
            IResourcesProvider client,
            T item)
        {
            context.Result = item == null
                ? null
                : new TranslatedResource<T>(
                    item,
                    client.TryGetResourceAsString(
                        $"{directiveOptions.ResourceKeyPrefix}/{item}",
                        culture,
                        item.ToString())
                );
        }

        internal static TranslatableDirective GetDirectiveOptions(
            IDirectiveContext context)
        {
            IDirective? directive = context.Selection.Field.Directives.FirstOrDefault(
                d => d.Name.Equals(TranslatableDirectiveType.DirectiveName));

            if (directive == null)
            {
                throw new TranslationNotSupportedForFieldException(context.Selection.Field);
            }

            TranslatableDirective translatableOptions
                = directive.ToObject<TranslatableDirective>();

            return translatableOptions;
        }

        internal static CultureInfo DetermineOutputCulture(
            TranslatableLanguage language)
        {
            if (language != TranslatableLanguage.NotSet)
            {
                return new CultureInfo(language.ToString());
            }

            return Thread.CurrentThread.CurrentCulture;
        }
    }
}
