using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesProviderAdapter
    {
        Task<string> TryGetTranslationAsStringAsync(
            string key,
            CultureInfo culture,
            string fallbackValue,
            CancellationToken cancellationToken);
    }
}
