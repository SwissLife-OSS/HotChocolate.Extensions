using System;
using System.Reflection;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;

namespace HotChocolate.Extensions.Translation
{
    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field,
        Inherited = false,
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
        AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field,
        Inherited = true,
        AllowMultiple = false)]
    public class TranslateArrayAttribute<T>: ObjectFieldDescriptorAttribute
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

        public override void OnConfigure(
            IDescriptorContext context,
            IObjectFieldDescriptor descriptor,
            MemberInfo member)
        {
            if (descriptor is IObjectFieldDescriptor d)
            {
                d.TranslateArray<T>(ResourceKeyPrefix, Nullable);
            }
        }
    }
}
