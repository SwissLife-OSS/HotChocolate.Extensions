using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation
{
    public class TranslatedResourceType<T> : ObjectType<TranslatedResource<T>>
    {
        protected override void Configure(
            IObjectTypeDescriptor<TranslatedResource<T>> descriptor)
        {
            descriptor.Name(
                typeof(T) == typeof(string)
                ? "Translation"
                : $"{typeof(T).Name}Translation");

            descriptor.BindFieldsImplicitly();
        }
    }
}
