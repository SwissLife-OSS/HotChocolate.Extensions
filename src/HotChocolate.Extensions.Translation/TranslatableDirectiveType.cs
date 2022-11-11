using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation
{
    public class TranslatableDirectiveType
        : DirectiveType<TranslatableDirective>
    {
        internal const string DirectiveName = "translatable";

        protected override void Configure(
            IDirectiveTypeDescriptor<TranslatableDirective> descriptor)
        {
            descriptor.Name(DirectiveName);
            descriptor.Location(DirectiveLocation.FieldDefinition);
        }
    }
}
