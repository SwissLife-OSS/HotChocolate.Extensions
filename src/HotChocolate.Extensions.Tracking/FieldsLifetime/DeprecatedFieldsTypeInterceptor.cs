using System.Collections.Generic;
using System.Linq;
using HotChocolate.Configuration;
using HotChocolate.Language;
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
                
                var directive = new TrackedDirective(
                    new DeprecatedFieldsTrackingEntryFactory());

                //var directiveDefinition = new DirectiveDefinition(
                //    new DirectiveNode(directive);

                foreach (ObjectFieldDefinition deprecatedField in deprecatedFields)
                {
                    deprecatedField.Directives.Add(new DirectiveDefinition(
                        directive,
                        completionContext.TypeInterceptor.GetTypeRef(directive.GetType())));
                }
            }
        }
    }
}
