using System;
using System.Collections.Generic;
using System.Globalization;
using HotChocolate.Extensions.Translation.Resources;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class DictionaryResourcesProviderAdapter : IResourcesProviderAdapter
    {
        internal DictionaryResourcesProviderAdapter(
            IDictionary<Language, Dictionary<string, Resource>> masterDictionary)
        {
            _masterDictionary = masterDictionary;
        }

        private IDictionary<Language, Dictionary<string, Resource>> _masterDictionary;

        public string TryGetResourceAsString(string key, CultureInfo culture, string fallbackValue)
        {
            Language language = ToLanguage(culture);

            if (!_masterDictionary.ContainsKey(language))
            {
                return fallbackValue;
            }
            if (!_masterDictionary[language].ContainsKey(key))
            {
                return fallbackValue;
            }

            return _masterDictionary[language][key].Value;
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
