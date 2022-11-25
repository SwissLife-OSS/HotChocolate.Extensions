using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation;

namespace Microsoft.Extensions.DependencyInjection
{
    public class TranslationBuilder
    {
        private readonly IRequestExecutorBuilder _requestExecutorBuilder;
        private readonly TranslationInterfaceType _translationInterfaceType;

        public TranslationBuilder(
            IRequestExecutorBuilder requestExecutorBuilder,
            TranslationInterfaceType translationInterfaceType)
        {
            _requestExecutorBuilder = requestExecutorBuilder;
            _translationInterfaceType = translationInterfaceType;
        }

        public TranslationBuilder AddTranslatableType<T>()
            where T : notnull
        {
            _requestExecutorBuilder.AddDirectiveType<TranslateDirectiveType<T>>();

            return this;
        }

        public TranslationBuilder SetTranslationInterfaceName(string name)
        {
            _translationInterfaceType.InterfaceName = name;
            return this;
        }
    }
}
