using System;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.Extensions.Translation
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class TranslateAttribute : TranslateAttribute<string>
    {
        public TranslateAttribute(
            string resourceKeyPrefix,
            bool nullable = false)
            : base(resourceKeyPrefix, nullable)
        {
        }
    }

    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class TranslateAttribute<T>: DescriptorAttribute
    {
        public TranslateAttribute(
            string resourceKeyPrefix,
            bool nullable = false)
        {
            ResourceKeyPrefix = resourceKeyPrefix;
            Nullable = nullable;
        }

        public string ResourceKeyPrefix { get; set; }
        public bool Nullable { get; set; }

        protected override void TryConfigure(
            IDescriptorContext context,
            IDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            if (descriptor is IObjectFieldDescriptor d)
            {
                d.Translate<T>(ResourceKeyPrefix, Nullable);
            }
            else if (descriptor is IInterfaceFieldDescriptor i)
            {
                i.TranslateArray<T>(Nullable);
            }
        }
    }
}
