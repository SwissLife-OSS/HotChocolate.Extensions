using System.Globalization;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesClient
    {
        bool TryGetResource(string key, CultureInfo culture, out Resource res);
    }
}
