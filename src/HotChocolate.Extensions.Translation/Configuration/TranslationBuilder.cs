using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation;
using HotChocolate.Extensions.Translation.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public class TranslationBuilder
    {
        private readonly IRequestExecutorBuilder _requestExecutorBuilder;
        private readonly TranslationInterfaceType _translationInterfaceType;
        private readonly TranslationOptions _translationOptions;

        public TranslationBuilder(
            IRequestExecutorBuilder requestExecutorBuilder,
            TranslationInterfaceType translationInterfaceType)
        {
            _requestExecutorBuilder = requestExecutorBuilder;
            _translationInterfaceType = translationInterfaceType;
            _translationOptions = new TranslationOptions();
        }

        public TranslationBuilder AddTranslatableType<T>()
        {
            _requestExecutorBuilder.AddDirectiveType<TranslateDirectiveType<T>>();

            return this;
        }

        public TranslationBuilder SetTranslatedResourceNamingConvention(string format)
        {

        }

        public TranslationBuilder SetTranslationInterfaceName(string name)
        {
            _translationInterfaceType.InterfaceName = name;
            return this;
        }
    }
}
