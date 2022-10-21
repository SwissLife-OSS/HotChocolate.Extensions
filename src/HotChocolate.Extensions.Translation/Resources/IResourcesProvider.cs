using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IResourcesProvider
    {
        bool TryGetResource(string key, CultureInfo culture, [NotNullWhen(returnValue:true)] out Resource? res);
    }
}
