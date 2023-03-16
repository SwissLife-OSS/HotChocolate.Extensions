using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesProvider
    {
        Task<bool> TryGetResourceAsync(
            string key,
            CultureInfo culture,
            [NotNullWhen(returnValue:true)] out Resource? res,
            CancellationToken cancellationToken);
    }
}
