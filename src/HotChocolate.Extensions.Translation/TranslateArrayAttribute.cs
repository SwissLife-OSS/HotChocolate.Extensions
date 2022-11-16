using System;
using System.Reflection;
using HotChocolate.Extensions.Translation.Exceptions;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.Extensions.Translation
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class TranslateArrayAttribute : TranslateAttribute<string>
    {
        public TranslateArrayAttribute(
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
    public class TranslateArrayAttribute<T>: DescriptorAttribute
    {
        public TranslateArrayAttribute(
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
                d.TranslateArray<T>(ResourceKeyPrefix, Nullable);
            }
            else if (descriptor is IInterfaceFieldDescriptor i)
            {
                i.TranslateArray<T>(Nullable);
            }
        }
    }
}
