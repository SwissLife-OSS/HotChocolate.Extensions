using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using HotChocolate.Extensions.Translation.Resources;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class DictionaryResourcesClient : IResourcesProvider
    {
        internal DictionaryResourcesClient(
            IDictionary<Language, Dictionary<string, Resource>> masterDictionary)
        {
            _masterDictionary = masterDictionary;
        }

        private IDictionary<Language, Dictionary<string, Resource>> _masterDictionary;

        public bool TryGetResource(string key, CultureInfo culture, [NotNullWhen(returnValue: true)] out Resource? res)
        {
            res = null;
            Language language = ToLanguage(culture);

            if (!_masterDictionary.ContainsKey(language))
            {
                return false;
            }
            if (!_masterDictionary[language].ContainsKey(key))
            {
                return false;
            }

            res = _masterDictionary[language][key];
            return true;
        }

        private static Language ToLanguage(CultureInfo culture)
        {
            //TODO
            return Language.En;
        }
    }
}
