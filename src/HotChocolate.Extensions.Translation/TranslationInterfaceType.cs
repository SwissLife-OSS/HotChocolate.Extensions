using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation
{
    public class TranslationInterfaceType: InterfaceType<ITranslation>
    {
        internal string? InterfaceName { get; set; }

        protected override void Configure(
            IInterfaceTypeDescriptor<ITranslation> descriptor)
        {
            if(InterfaceName != null)
            {
                descriptor.Name(InterfaceName);
            }
        }
    }
}
