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
        public override void OnBeforeCompleteType(
            ITypeCompletionContext completionContext,
            DefinitionBase? definition)
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

                FieldMiddleware trackingMiddleware
                    = FieldClassMiddlewareFactory.Create<DeprecatedFieldsTrackingMiddleware>();

                FieldMiddlewareDefinition trackingMiddlewareDefinition
                    = new FieldMiddlewareDefinition(
                        trackingMiddleware,
                        isRepeatable: false);

                foreach (ObjectFieldDefinition deprecatedField in deprecatedFields)
                {
                    deprecatedField.MiddlewareDefinitions.Insert(
                        0,
                        trackingMiddlewareDefinition);
                }
            }
        }
    }
}
