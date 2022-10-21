using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation
{
    public class TranslatableLanguageType : EnumType<TranslatableLanguage>
    {
        protected override void Configure(IEnumTypeDescriptor<TranslatableLanguage> descriptor)
        {
            descriptor.Value(TranslatableLanguage.NotSet).Name("NOTSET");
        }
    }
}
