using System.Globalization;
using HotChocolate.Extensions.Translation.Resources;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class EvergreenResourcesProviderAdapter : IResourcesProviderAdapter
    {
        public string TryGetResourceAsString(string key, CultureInfo culture, string fallbackValue)
        {
            return $"rms:{key}_{culture.DisplayName}";
        }
    }
}
