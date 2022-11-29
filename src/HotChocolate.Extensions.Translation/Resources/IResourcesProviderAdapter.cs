using System.Globalization;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesProviderAdapter
    {
        string TryGetTranslationAsString(
            string key, CultureInfo culture, string fallbackValue);
    }
}
