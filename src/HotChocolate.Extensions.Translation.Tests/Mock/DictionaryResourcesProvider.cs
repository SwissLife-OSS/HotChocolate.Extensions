using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Extensions.Translation.Resources;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class DictionaryResourcesProviderAdapter : IResourcesProviderAdapter
    {
        private IDictionary<Language, Dictionary<string, Resource>> _masterDictionary;

        internal DictionaryResourcesProviderAdapter(
            IDictionary<Language, Dictionary<string, Resource>> masterDictionary)
        {
            _masterDictionary = masterDictionary;
        }

        public Task<string> TryGetTranslationAsStringAsync(
            string key,
            CultureInfo culture,
            string fallbackValue,
            CancellationToken cancellationToken)
        {
            Language language = ToLanguage(culture);

            if (!_masterDictionary.ContainsKey(language))
            {
                return Task.FromResult(fallbackValue);
            }
            if (!_masterDictionary[language].ContainsKey(key))
            {
                return Task.FromResult(fallbackValue);
            }

            return Task.FromResult(_masterDictionary[language][key].Value);
        }

        private static Language ToLanguage(CultureInfo culture)
        {
            switch (culture.TwoLetterISOLanguageName)
            {
                case "en":
                    return Language.En;
                case "fr":
                    return Language.Fr;
                case "de":
                    return Language.De;
                case "it":
                    return Language.It;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
