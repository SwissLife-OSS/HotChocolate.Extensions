using System.Globalization;
using HotChocolate.Extensions.Translation.Resources;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class EvergreenResourcesClient : IResourcesProvider
    {
        public bool TryGetResource(string key, CultureInfo culture, out Resource res)
        {
            res = new Resource(
                key: key,
                value: $"rms:{key}_{culture.DisplayName}");

            return true;
        }
    }
}
