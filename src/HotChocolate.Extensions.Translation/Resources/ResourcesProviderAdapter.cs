using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task<string> TryGetTranslationAsStringAsync(
            string key,
            CultureInfo culture,
            string fallbackValue,
            CancellationToken cancellationToken)
        {
            Resource? res = await _resourcesProvider
                .TryGetResourceAsync(key, culture, cancellationToken)
                .ConfigureAwait(false);

            if (res is { })
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
