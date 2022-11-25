using System.Globalization;

namespace HotChocolate.Extensions.Translation.Resources
{
    public static class ResourceClientExtensions
    {
        public static string TryGetResourceAsString(
            this IResourcesProvider client,
            string key,
            CultureInfo culture,
            string fallbackValue)
        {
            if (client.TryGetResource(key, culture, out Resource? res))
            {
                return res.Value;
            }

            return fallbackValue;
        }
    }
}
