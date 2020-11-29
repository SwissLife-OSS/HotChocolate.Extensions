using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation
{
    public class TranslatableLanguageType : EnumType<TranslatableLanguage>
    {
        protected override void Configure(IEnumTypeDescriptor<TranslatableLanguage> descriptor)
        {
            descriptor.Item(TranslatableLanguage.NotSet).Name("NOTSET");
        }
    }
}
