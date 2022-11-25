using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotChocolate.Extensions.Translation.Configuration
{
    internal class TranslationOptions
    {
        internal TranslationOptions(string translatedResourceNamingConvention)
        {
            TranslatedResourceNamingConvention = translatedResourceNamingConvention;
        }

        internal string TranslatedResourceNamingConvention { get; }
    }
}
