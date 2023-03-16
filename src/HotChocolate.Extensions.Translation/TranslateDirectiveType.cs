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

            descriptor.Use((next, directive) => context
                => Translate(next, context, directive.AsValue<TranslateDirective<T>>()));
        }

        private static async ValueTask Translate(
            FieldDelegate next,
            IMiddlewareContext context,
            TranslateDirective directive)
        {
            await next.Invoke(context);
            var value = context.Result;

            if (value == null)
            {
                return;
            }

            try
            {
                CultureInfo culture = Thread.CurrentThread.CurrentCulture;

                await UpdateResultAsync(context, value, directive, culture, context.RequestAborted)
                    .ConfigureAwait(false);
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

        internal static async Task UpdateResultAsync(IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            CancellationToken cancellationToken)
        {
            IResourcesProviderAdapter client = context.Service<IResourcesProviderAdapter>();

            if (value is IEnumerable<T?> items)
            {
                var rItems = items.OfType<T>().ToList();
                await TranslateArrayAsync(
                        context, directiveOptions, culture, client, rItems, cancellationToken)
                    .ConfigureAwait(false);
            }
            else if (directiveOptions.ToCodeLabelItem && value is T t)
            {
                await TranslateToCodeLabelItemAsync(
                        context, directiveOptions, culture, client, t, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await TranslateFieldToStringAsync(
                        context, value, directiveOptions, culture, client, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static async Task TranslateFieldToStringAsync(IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            CancellationToken cancellationToken)
        {
            switch (value)
            {
                case string s:
                    await TranslateStringAsync(
                            context,
                            value,
                            directiveOptions,
                            culture,
                            client,
                            s,
                            cancellationToken)
                        .ConfigureAwait(false);
                    break;

                case sbyte or byte or short or ushort or int or uint or long or ulong:
                {
                    var sValue = value.ToString()
                                 ?? throw new TranslationNotSupportedForTypeException(
                                     value.GetType(),
                                     context.Selection.Field);

                    await TranslateStringAsync(
                            context,
                            value,
                            directiveOptions,
                            culture,
                            client,
                            sValue,
                            cancellationToken)
                        .ConfigureAwait(false);
                    break;
                }

                case Enum e:
                    await TranslateEnum(
                        context, value, directiveOptions, culture, client, e, cancellationToken);
                    break;

                default:
                    throw new TranslationNotSupportedForTypeException(
                        value.GetType(),
                        context.Selection.Field);
            }
        }

        private static async Task TranslateArrayAsync(IMiddlewareContext context,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items,
            CancellationToken cancellationToken)
        {
            if (directiveOptions.ToCodeLabelItem)
            {
                await TranslateToCodeLabelArrayAsync(
                        context, directiveOptions, culture, client, items, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await TranslateToStringArrayAsync(
                        context, directiveOptions, culture, client, items, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static async Task UpdateResultAsync(
            IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            Enum e,
            CancellationToken cancellationToken)
        {
            context.Result = await client
                .TryGetTranslationAsStringAsync(
                    $"{directiveOptions.ResourceKeyPrefix}/{value}",
                    culture,
                    e.ToString(),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        private static async Task TranslateStringAsync(IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            string s, CancellationToken cancellationToken)
        {
            context.Result = await client.TryGetTranslationAsStringAsync(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                s,
                cancellationToken)
                .ConfigureAwait(false);
        }

        private static async Task TranslateToStringArrayAsync(IMiddlewareContext context,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items, CancellationToken cancellationToken)
        {
            var result = await Task.WhenAll(items
                .Select(async t => await client.TryGetTranslationAsStringAsync(
                    $"{directiveOptions.ResourceKeyPrefix}/{t}",
                    culture,
                    t.ToString(),
                    cancellationToken)
                    .ConfigureAwait(false))
                .ToList());

            context.Result = result;
        }

        private static async Task TranslateToCodeLabelArrayAsync(IMiddlewareContext context,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items, CancellationToken cancellationToken)
        {
            var result = await Task.WhenAll(items
                    .Select(async item => new TranslatedResource<T>(
                        item,
                        await client.TryGetTranslationAsStringAsync(
                                $"{directiveOptions.ResourceKeyPrefix}/{item}",
                                culture,
                                item.ToString(),
                                cancellationToken)
                            .ConfigureAwait(false)
                    ))
                    .ToList())
                .ConfigureAwait(false);

            context.Result = result;
        }

        private static async Task TranslateToCodeLabelItemAsync(IMiddlewareContext context,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            T item,
            CancellationToken cancellationToken)
        {
            context.Result = item == null
                ? null
                : new TranslatedResource<T>(
                    item,
                    await client.TryGetTranslationAsStringAsync(
                            $"{directiveOptions.ResourceKeyPrefix}/{item}",
                            culture,
                            item.ToString(),
                            cancellationToken)
                        .ConfigureAwait(false)
                );
        }

        private static async Task TranslateEnum(
            IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            CultureInfo culture,
            IResourcesProviderAdapter client,
            Enum e,
            CancellationToken cancellationToken)
        {
            context.Result = await client.TryGetTranslationAsStringAsync(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                e.ToString(),
                cancellationToken);
        }
    }
}
