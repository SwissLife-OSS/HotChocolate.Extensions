using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace HotChocolate.Extensions.Translation.Resources
{
    public class DefaultTranslationObserver: TranslationObserver
    {
        private readonly ILogger<TranslationObserver> _logger;

        public DefaultTranslationObserver(ILogger<TranslationObserver> logger)
        {
            _logger = logger;
        }

        public override Task OnMissingResource(string key)
        {
            _logger.LogWarning("Missing translation resource: {key}", key);

            return base.OnMissingResource(key);
        }
    }
}
