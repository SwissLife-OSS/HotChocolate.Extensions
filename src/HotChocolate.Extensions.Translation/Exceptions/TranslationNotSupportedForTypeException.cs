using System;
using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation.Exceptions
{
    public class TranslationNotSupportedForTypeException : TranslationException
    {
        public TranslationNotSupportedForTypeException(
            Type type,
            IObjectField field)
            : base($"Translation not supported for type [{type.Name}], field [{field.Name}]")
        {
        }
    }
}
