using System;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.Extensions.Tracking.TagTracking
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class TrackAttribute: DescriptorAttribute
    {
        public TrackAttribute(string tagName)
        {
            TagName = tagName;
        }

        public string TagName { get; set; }

        protected override void TryConfigure(
            IDescriptorContext context,
            IDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            if (descriptor is IObjectFieldDescriptor d)
            {
                d.Track(TagName);
            }
        }
    }
}
