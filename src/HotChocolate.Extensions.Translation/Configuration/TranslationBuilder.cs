using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation;

namespace Microsoft.Extensions.DependencyInjection
{
    public class TranslationBuilder
    {
        private readonly IRequestExecutorBuilder _requestExecutorBuilder;

        internal TranslationBuilder(IRequestExecutorBuilder requestExecutorBuilder)
        {
            _requestExecutorBuilder = requestExecutorBuilder;
        }

        public TranslationBuilder AddTranslatableType<T>()
        {
            _requestExecutorBuilder.AddDirectiveType<TranslateDirectiveType<T>>();

            return this;
        }
    }
}
