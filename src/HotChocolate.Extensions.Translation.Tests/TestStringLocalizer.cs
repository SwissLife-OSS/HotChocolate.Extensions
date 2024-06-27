using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using HotChocolate.Extensions.Translation.Resources;
using HotChocolate.Extensions.Translation.Tests.Mock;
using Microsoft.Extensions.Localization;

namespace HotChocolate.Extensions.Translation.Tests;

public class TestStringLocalizer : IStringLocalizer
{
    private readonly DictionaryResourcesProviderAdapter _dictionaryResourcesProviderAdapter;

    public TestStringLocalizer(
        Func<IDictionary<Mock.Language, Dictionary<string, Resource>>> func)
    {
        _dictionaryResourcesProviderAdapter =
            new DictionaryResourcesProviderAdapter(func());
    }

    public LocalizedString this[string name]
    {
        get
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;

            string notFound = "*** NOT FOUND ***";

            var translatedValue =
                _dictionaryResourcesProviderAdapter.TryGetTranslationAsStringAsync(
                $"{TestResourceType.Path}/{name}", culture, notFound, CancellationToken.None)
                .GetAwaiter().GetResult();

            return new LocalizedString(
                name,
                translatedValue == notFound ? name : translatedValue,
                resourceNotFound: translatedValue == notFound);
        }
    }

    public LocalizedString this[string name, params object[] arguments] =>
        new LocalizedString(name, string.Format(this[name].Value, arguments));


    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotImplementedException();
    }
}
