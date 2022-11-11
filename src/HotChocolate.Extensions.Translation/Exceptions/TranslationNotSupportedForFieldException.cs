using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation.Exceptions
{
    public class TranslationNotSupportedForFieldException : TranslationException
    {
        public TranslationNotSupportedForFieldException(IObjectField field)
        : base($"Translation not supported for field [{field.Name}]")
        {
        }
    }
}
