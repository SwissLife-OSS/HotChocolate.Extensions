using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using HotChocolate.Extensions.Translation.Resources;

namespace StarWars
{
    public class DictionaryResourcesProvider : IResourcesProvider
    {
        public DictionaryResourcesProvider()
        {
            _masterDictionary = new Dictionary<Language, IReadOnlyDictionary<string, string>>()
            {
                {
                    Language.En,
                    new Dictionary<string, string>
                    {
                        { "HairColors/Blond", "Blond" },
                        { "HairColors/Brown", "Brown" },
                        { "HairColors/Red", "Red" },
                        { "HairColors/Black", "Black" },
                    }
                },
                {
                    Language.Fr,
                    new Dictionary<string, string>
                    {
                        { "HairColors/Blond", "Blond" },
                        { "HairColors/Brown", "Brun" },
                        { "HairColors/Red", "Roux" },
                        { "HairColors/Black", "Noir" },
                    }
                },
                {
                    Language.De,
                    new Dictionary<string, string>
                    {
                        { "HairColors/Blond", "Blond" },
                        { "HairColors/Brown", "Braun" },
                        { "HairColors/Red", "Rot" },
                        { "HairColors/Black", "Schwarz" },
                    }
                }
            };
        }

        private IReadOnlyDictionary<Language, IReadOnlyDictionary<string, string>> _masterDictionary;

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

            string label = _masterDictionary[language][key];
            res = new Resource(key, label);

            return true;
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
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
