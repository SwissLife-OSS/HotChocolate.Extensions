using System;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.Extensions.Tracking
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class TrackAttribute<TFactory>: DescriptorAttribute
        where TFactory : notnull, ITrackingEntryFactory
    {
        protected override void TryConfigure(
            IDescriptorContext context,
            IDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            if (descriptor is IObjectFieldDescriptor d)
            {
                d.Track<TFactory>();
            }
        }
    }
}
