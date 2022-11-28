using System.Globalization;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesProviderAdapter
    {
        string TryGetResourceAsString(
            string key, CultureInfo culture, string fallbackValue);
    }
}
