using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
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
                        { "MaritalStatus/Married", "Married" },
                        { "MaritalStatus/Complicated", "Complicated" },
                        { "MaritalStatus/Widow", "Widow" },
                        { "MaritalStatus/Single", "Single" },
                        { "Episodes/NewHope", "A new hope" },
                        { "Episodes/Empire", "The empire strikes back" },
                        { "Episodes/Jedi", "The return of the Jedi" }
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
                        { "MaritalStatus/Married", "Marié" },
                        { "MaritalStatus/Complicated", "Compliqué" },
                        { "MaritalStatus/Widow", "Veuf" },
                        { "MaritalStatus/Single", "Célibataire" },
                        { "Episodes/NewHope", "Un nouvel espoir" },
                        { "Episodes/Empire", "L'empire contre-attaque" },
                        { "Episodes/Jedi", "Le retour du Jedi" }
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
                        { "MaritalStatus/Married", "Verheiratet" },
                        { "MaritalStatus/Complicated", "Kompliziert" },
                        { "MaritalStatus/Widow", "Witwer" },
                        { "MaritalStatus/Single", "Junggeselle" },
                        { "Episodes/NewHope", "Eine neue Hoffnung" },
                        { "Episodes/Empire", "Das Imperium schlägt zurück" },
                        { "Episodes/Jedi", "Die Rückkehr der Jedi-Ritter" }
                    }
                }
            };
        }

        private IReadOnlyDictionary<Language, IReadOnlyDictionary<string, string>> _masterDictionary;

        public Task<bool> TryGetResourceAsync(
            string key,
            CultureInfo culture,
            [NotNullWhen(returnValue: true)] out Resource? res,
            CancellationToken cancellationToken)
        {
            res = null;
            Language language = ToLanguage(culture);

            if (!_masterDictionary.ContainsKey(language))
            {
                return Task.FromResult(false);
            }
            if (!_masterDictionary[language].ContainsKey(key))
            {
                return Task.FromResult(false);
            }

            string label = _masterDictionary[language][key];
            res = new Resource(key, label);

            return Task.FromResult(true);
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
