using System.Collections.Generic;
using System.Linq;
using HotChocolate.Configuration;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors.Definitions;

namespace HotChocolate.Extensions.Tracking.FieldsLifetime
{
    public class DeprecatedFieldsTypeInterceptor : TypeInterceptor
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
                ObjectFieldDefinition[] deprecatedFields = otd.Fields
                    .Where(x => x.IsDeprecated)
                    .ToArray();

                if (!deprecatedFields.Any())
                {
                    return;
                }

                FieldMiddleware fesfvesf = FieldClassMiddlewareFactory.Create<DeprecatedFieldsTrackingMiddleware>();

                FieldMiddlewareDefinition serviceMiddleware = new FieldMiddlewareDefinition(fesfvesf, isRepeatable: false);

                foreach (ObjectFieldDefinition deprecatedField in deprecatedFields)
                {
                    deprecatedField.MiddlewareDefinitions.Insert(
                        0, serviceMiddleware);
                }
            }
        }
    }
}
