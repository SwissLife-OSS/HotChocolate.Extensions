//using System;
//using System.Threading.Tasks;
//using HotChocolate.Extensions.Tracking.FieldsLifetime;
//using HotChocolate.Resolvers;
//using HotChocolate.Types;

//namespace HotChocolate.Extensions.Tracking;

///// <summary>
///// Standalone directive that can be added by the Backend on a field
///// to ALWAYS track calls to this field.
///// </summary>
//public sealed class DeprecatedFieldsTrackingDirectiveType : DirectiveType
//{
//    internal const string DirectiveName = "deprecatedtracked";

//    protected override void Configure(
//        IDirectiveTypeDescriptor descriptor)
//    {
//        descriptor.Name(DirectiveName);
//        descriptor.Location(DirectiveLocation.FieldDefinition);

//        descriptor.Use(next => context => Track(next, context));
//    }

//    private async ValueTask Track(
//        FieldDelegate next,
//        IDirectiveContext context)
//    {
//        // first run the field's resolver pipeline to it's end
//        await next.Invoke(context);

//        try
//        {
//            //TODO inject factory
//            await context.SubmitTrack(
//                new DeprecatedFieldsTrackingEntryFactory(),
//                context.RequestAborted);
//        }
//        catch (Exception ex)
//        {
//            context.LogAndReportError(ex);
//        }
//    }
//}
