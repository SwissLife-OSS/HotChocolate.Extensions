using System;
using System.Collections.Generic;
using System.Linq;
using HotChocolate.Configuration;
using HotChocolate.Language;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
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

                //var directiveDefinition = new DirectiveDefinition(
                //    new DirectiveNode(DeprecatedFieldsTrackingDirectiveType.DirectiveName));

                FieldMiddlewareDefinition serviceMiddleware =
                    new(next => async context =>
                    {
                        await next(context).ConfigureAwait(false);

                        try
                        {
                            //TODO inject factory
                            await context.SubmitTrack(
                                new DeprecatedFieldsTrackingEntryFactory(),
                                context.RequestAborted);
                        }
                        catch (Exception ex)
                        {
                            context.LogAndReportError(ex);
                        }
                    },
                    isRepeatable: false);

                foreach (ObjectFieldDefinition deprecatedField in deprecatedFields)
                {
                    deprecatedField.MiddlewareDefinitions.Insert(
                        0, serviceMiddleware);

                    //deprecatedField.Directives.Add(directiveDefinition);
                }
            }
        }
    }
}
