using System;
using System.Reflection;
using HotChocolate.Extensions.Translation.Resources;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using Microsoft.Extensions.DependencyInjection;

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

        public TranslateAttribute(
             Type resourceSource,
             bool nullable = false)
             : base(resourceSource, nullable)
        {
        }
    }

    [AttributeUsage(
        AttributeTargets.Property | AttributeTargets.Method,
        Inherited = true,
        AllowMultiple = false)]
    public class TranslateAttribute<T>: DescriptorAttribute
    {
        private readonly Type? _resourceSource;

        public TranslateAttribute(
            string resourceKeyPrefix,
            bool nullable = false)
        {
            ResourceKeyPrefix = resourceKeyPrefix;
            Nullable = nullable;
        }

        public TranslateAttribute(
            Type resourceSource,
            bool nullable = false)
        {
            _resourceSource = resourceSource;
            ResourceKeyPrefix = string.Empty;
            Nullable = nullable;
        }

        public string ResourceKeyPrefix { get; set; }
        public bool Nullable { get; set; }

        protected override void TryConfigure(
            IDescriptorContext context,
            IDescriptor descriptor,
            ICustomAttributeProvider element)
        {
            if (_resourceSource is not null)
            {
                IResourceTypeResolver typeResolver =
                    context.Services.GetRequiredService<IResourceTypeResolver>();

                ResourceKeyPrefix = typeResolver.GetAlias(_resourceSource);
            }

            if (descriptor is IObjectFieldDescriptor d)
            {
                d.Translate<T>(ResourceKeyPrefix, Nullable);
            }
            else if (descriptor is IInterfaceFieldDescriptor i)
            {
                i.Translate<T>(Nullable);
            }
        }
    }
}
