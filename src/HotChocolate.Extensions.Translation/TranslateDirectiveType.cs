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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

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

            descriptor.Use((next, directive) => async context
                => await Translate(next, context, directive.AsValue<TranslateDirective<T>>()));
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
                await UpdateResultAsync(context, value, directive, context.RequestAborted)
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

        internal static async Task UpdateResultAsync(
            IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            CancellationToken cancellationToken)
        {
            if (context.Result is null)
            {
                return;
            }

            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

            IStringLocalizerFactory? localizerFactory =
                context.Services.GetService<IStringLocalizerFactory>();

            if (localizerFactory is not null)
            {
                IStringLocalizer stringLocalizer =
                    localizerFactory.Create(directiveOptions.ResourceKeyPrefix, string.Empty);

                if (stringLocalizer is IFetcher fetcher)
                {
                    await fetcher.FetchAsync();
                }

                IEnumerable<TranslationObserver>? observers =
                    context.Services.GetService<IEnumerable<TranslationObserver>>();  

                if (value is IEnumerable<T?> values)
                {
                    if (directiveOptions.ToCodeLabelItem)
                    {
                        context.Result = values
                            .Select(item => item is not null
                                ? new TranslatedResource<T>(
                                    (T)item, GetString(stringLocalizer, item, observers))
                                : null)
                            .ToArray();

                        return;
                    }

                    context.Result = values
                        .Select(item => item is not null
                            ? GetString(stringLocalizer, item, observers)
                            : null)
                        .ToArray();

                    return;
                }

                if (directiveOptions.ToCodeLabelItem && value is T t)
                {
                    context.Result =
                        new TranslatedResource<T>(
                            (T)value, GetString(stringLocalizer, value, observers));

                    return;
                }

                context.Result = GetString(stringLocalizer, value, observers);
                
                return;
            }

            IResourcesProviderAdapter client = context.Service<IResourcesProviderAdapter>();

            if (value is IEnumerable<T?> items)
            {
                var rItems = items.OfType<T>().ToList();
                await TranslateArrayAsync(
                        context, directiveOptions, client, rItems, cancellationToken)
                    .ConfigureAwait(false);
            }
            else if (directiveOptions.ToCodeLabelItem && value is T t)
            {
                await TranslateToCodeLabelItemAsync(
                        context, directiveOptions, client, t, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await TranslateFieldToStringAsync(
                        context, value, directiveOptions, client, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static string GetString(
             IStringLocalizer stringLocalizer,
             object value,
             IEnumerable<TranslationObserver>? observers)
        {
            string key = value.ToString()!;

            LocalizedString localizedString = stringLocalizer[key];

            if (localizedString.ResourceNotFound && observers is not null)
            {
                foreach (TranslationObserver observer in observers)
                {
                    observer.OnMissingResource(key);
                }
            }

            return localizedString.Value;
        }

        private static async Task TranslateFieldToStringAsync(IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
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
                            client,
                            sValue,
                            cancellationToken)
                        .ConfigureAwait(false);
                    break;
                }

                case Enum e:
                    await TranslateEnum(
                        context, value, directiveOptions, client, e, cancellationToken);
                    break;

                default:
                    throw new TranslationNotSupportedForTypeException(
                        value.GetType(),
                        context.Selection.Field);
            }
        }

        private static async Task TranslateArrayAsync(IMiddlewareContext context,
            TranslateDirective directiveOptions,
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items,
            CancellationToken cancellationToken)
        {
            if (directiveOptions.ToCodeLabelItem)
            {
                await TranslateToCodeLabelArrayAsync(
                        context, directiveOptions, client, items, cancellationToken)
                    .ConfigureAwait(false);
            }
            else
            {
                await TranslateToStringArrayAsync(
                        context, directiveOptions, client, items, cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        private static async Task TranslateStringAsync(IMiddlewareContext context,
            object value,
            TranslateDirective directiveOptions,
            IResourcesProviderAdapter client,
            string s, CancellationToken cancellationToken)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

            context.Result = await client.TryGetTranslationAsStringAsync(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                s,
                cancellationToken)
                .ConfigureAwait(false);
        }

        private static async Task TranslateToStringArrayAsync(IMiddlewareContext context,
            TranslateDirective directiveOptions,
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items, CancellationToken cancellationToken)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

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
            IResourcesProviderAdapter client,
            IReadOnlyList<T> items, CancellationToken cancellationToken)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

            TranslatedResource<T>[] result = await Task.WhenAll(items
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
            IResourcesProviderAdapter client,
            T item,
            CancellationToken cancellationToken)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

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
            IResourcesProviderAdapter client,
            Enum e,
            CancellationToken cancellationToken)
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

            context.Result = await client.TryGetTranslationAsStringAsync(
                $"{directiveOptions.ResourceKeyPrefix}/{value}",
                culture,
                e.ToString(),
                cancellationToken);
        }
    }
}
