using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Extensions.Translation.Resources;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class EvergreenResourcesProviderAdapter : IResourcesProviderAdapter
    {
        public Task<string> TryGetTranslationAsStringAsync(
            string key,
            CultureInfo culture,
            string fallbackValue,
            CancellationToken cancellationToken)
        {
            return Task.FromResult($"rms:{key}_{culture.DisplayName}");
        }
    }
}
