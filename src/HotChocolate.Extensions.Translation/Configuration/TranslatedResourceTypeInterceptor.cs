using System.Collections.Generic;
using HotChocolate.Configuration;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;

namespace HotChocolate.Extensions.Translation.Configuration
{
    public class TranslatedResourceTypeInterceptor : TypeInterceptor
    {
        public override bool CanHandle(ITypeSystemObjectContext context)
        {
            return context.Type is ObjectType or InterfaceType;
        }

        public override void OnBeforeCompleteType(
            ITypeCompletionContext completionContext,
            DefinitionBase? definition,
            IDictionary<string, object?> contextData)
        {
            if (definition is ObjectTypeDefinition otd)
            {

            }
        }
    }
}
