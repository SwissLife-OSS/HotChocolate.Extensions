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
            descriptor.Location(DirectiveLocation.FieldDefinition);

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
                CultureInfo culture = DetermineOutputCulture(context);

                TranslateDirective directiveOptions
                    = context.Directive.ToObject<TranslateDirective>();

                UpdateResult(context, value, directiveOptions, culture);
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
            TranslateDirective directiveOptions,
            CultureInfo culture)
        {
            IResourcesProviderAdapter client = context.Service<IResourcesProviderAdapter>();

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
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client)
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
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
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
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            Enum e)
        {
            context.Result = client.TryGetTranslationAsString(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                e.ToString());
        }

        private static void TranslateString(
            IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            string s)
        {
            context.Result = client.TryGetTranslationAsString(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                s);
        }

        private static void TranslateToStringArray(
            IMiddlewareContext context,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items)
        {
            context.Result = items
                .Select(t => client.TryGetTranslationAsString(
                        $"{directiveOptions.ResourceKeyPrefix}/{t}",
                        culture,
                        t.ToString()))
                .ToList();
        }

        private static void TranslateToCodeLabelArray(
            IMiddlewareContext context,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items)
        {
            context.Result = items
                .Select(item => new TranslatedResource<T>(
                    item,
                    client.TryGetTranslationAsString(
                        $"{directiveOptions.ResourceKeyPrefix}/{item}",
                        culture,
                        item.ToString())
                )).ToList();
        }

        private static void TranslateToCodeLabelItem(
            IMiddlewareContext context,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            T item)
        {
            context.Result = item == null
                ? null
                : new TranslatedResource<T>(
                    item,
                    client.TryGetTranslationAsString(
                        $"{directiveOptions.ResourceKeyPrefix}/{item}",
                        culture,
                        item.ToString())
                );
        }

        internal static CultureInfo DetermineOutputCulture(
            IDirectiveContext context)
        {
            return Thread.CurrentThread.CurrentCulture;
        }
    }
}
