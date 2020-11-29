using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using SwissLife.F2c.Resources.Contract;
using SwissLife.Resources.ResourcesClient;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class DictionaryResourcesClient : IResourcesClient
    {
        internal DictionaryResourcesClient(
            IDictionary<Language, Dictionary<string, Resource>> masterDictionary)
        {
            _masterDictionary = masterDictionary;
        }

        private IDictionary<Language, Dictionary<string, Resource>> _masterDictionary;

        /// <summary>
        /// Gets a single resource by it's key and language
        /// </summary>
        /// <param name="key">The key of the resource</param>
        /// <param name="language">The language of the resource</param>
        /// <returns></returns>
        public Resource GetResource(string key, Language language)
        {
            // if key not present in dictionary
            if (!TryGetResource(key, language, out Resource res))
            {
                throw new Exception($"Given resource key {key}" +
                    " can not be found.");
            }

            return res;
        }

        /// <summary>
        /// Gets a single resource by it's key and language
        /// </summary>
        /// <param name="key">The key of the resource</param>
        /// <param name="language">The language of the resource</param>
        /// <param name="res">the resource, if found</param>
        /// <returns>true if the resource was found.</returns>
        public bool TryGetResource(
            string key,
            Language language,
            [NotNullWhen(returnValue: true)] out Resource res)
        {
            res = null;

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key),
                    "Given argument key may not be null or empty.");
            }

            if (_masterDictionary == null)
            {
                throw new Exception("Method Initialize() of " +
                    $"{nameof(ResourcesClient)} needs to be executed on " +
                    "application startup.");
            }

            // if key present in dictionary
            var k = key.ToLower(CultureInfo.InvariantCulture);
            if (_masterDictionary[language].ContainsKey(k))
            {
                res = _masterDictionary[language][k];
            }

            return res != null;
        }

        /// <summary>
        /// Gets resources by keys and language
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public IEnumerable<Resource> GetResources(IEnumerable<string> keys,
            Language language)
        {
            // if at least one key not present in dictionary
            if (!TryGetResources(keys, language, out IEnumerable<Resource> resources))
            {
                throw new Exception("At least one resource key in the given " +
                    "key list can not be found.");
            }

            return resources;
        }

        /// <summary>
        /// Gets resources by keys and language
        /// </summary>
        /// <param name="keys">the resource keys</param>
        /// <param name="language">the target language</param>
        /// <param name="resources">the resources found</param>
        /// <returns>true if all the resources were found.</returns>
        public bool TryGetResources(
            IEnumerable<string> keys,
            Language language,
            [NotNullWhen(returnValue: true)] out IEnumerable<Resource> resources)
        {
            resources = null;

            if (keys == null || !keys.Any())
            {
                throw new ArgumentNullException(nameof(keys),
                    "Given argument key may not be null or empty.");
            }

            if (_masterDictionary == null)
            {
                throw new Exception("Method Initialize() of " +
                    $"{nameof(ResourcesClient)} needs to be executed on " +
                    "application startup.");
            }

            // if all keys are present in dictionary
            ICollection<string> validKeys = keys
                .Where(key => _masterDictionary[language].ContainsKey(
                    key.ToLower(CultureInfo.InvariantCulture)))
                .ToList();

            if (validKeys.Any())
            {
                resources = validKeys
                    .Select(key =>
                    _masterDictionary[language][key.ToLower(CultureInfo.InvariantCulture)]);

                return resources.Count() == keys.Count();
            }

            return false;
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
