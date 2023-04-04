using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesProvider
    {
        Task<Resource?> TryGetResourceAsync(
            string key,
            CultureInfo culture,
            CancellationToken cancellationToken);
    }
}
