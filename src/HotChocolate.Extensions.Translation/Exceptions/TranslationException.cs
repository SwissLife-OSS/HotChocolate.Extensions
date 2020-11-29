using System;

namespace HotChocolate.Extensions.Translation.Exceptions
{
    public class TranslationException : Exception
    {
        public TranslationException(string message)
            : base(message)
        {
        }
    }
}
