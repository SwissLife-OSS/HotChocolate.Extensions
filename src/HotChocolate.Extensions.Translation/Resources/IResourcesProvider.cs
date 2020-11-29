using System.Globalization;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesProvider
    {
        bool TryGetResource(string key, CultureInfo culture, out Resource res);
    }
}
