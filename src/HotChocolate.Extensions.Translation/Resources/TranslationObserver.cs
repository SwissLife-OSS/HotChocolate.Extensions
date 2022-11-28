using System.Threading.Tasks;

namespace HotChocolate.Extensions.Translation.Resources
{
    public abstract class TranslationObserver
    {
        public virtual Task OnMissingResource(string key)
        {
            return Task.CompletedTask;
        }
    }
}
