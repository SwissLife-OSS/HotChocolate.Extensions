using System.Collections.Generic;
using System.Globalization;

namespace HotChocolate.Extensions.Translation.Resources
{
    public class ResourcesProviderAdapter : IResourcesProviderAdapter
    {
        private readonly IResourcesProvider _resourcesProvider;
        private readonly IEnumerable<TranslationObserver> _observers;

        public ResourcesProviderAdapter(
            IResourcesProvider resourcesProvider,
            IEnumerable<TranslationObserver> observers)
        {
            _resourcesProvider = resourcesProvider;
            _observers = observers;
        }

        public string TryGetResourceAsString(
            string key,
            CultureInfo culture,
            string fallbackValue)
        {
            if (_resourcesProvider.TryGetResource(key, culture, out Resource? res))
            {
                return res.Value;
            }

            foreach (TranslationObserver observer in _observers)
            {
                observer.OnMissingResource(key);
            }

            return fallbackValue;
        }
    }
}
