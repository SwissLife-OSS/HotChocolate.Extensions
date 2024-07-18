using System.Threading.Tasks;

namespace HotChocolate.Extensions.Translation.Resources
{
    public interface IFetcher
    {
        Task FetchAsync();
    }
}
