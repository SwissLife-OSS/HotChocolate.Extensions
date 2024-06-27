using System;
using HotChocolate.Extensions.Translation.Resources;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;

namespace HotChocolate.Extensions.Translation
{
    public static class ObjectFieldDescriptorExtensions
    {
        /// <summary>
        /// Makes a field translated. In this case, the field value always gets
        /// translated to a language resource when invoked. 
        /// </summary>
        /// <param name="fieldDescriptor">
        /// The  HotChocoalte fieldDescriptor for the field we want to make translated.
        /// </param>
        /// <param name="keyPrefix">The prefix of the resource key.</param>
        /// <returns>The HotChocoalte fieldDescriptor for the field, made translated.</returns>
        [Obsolete("Use key-label pattern instead.")]
        public static IObjectFieldDescriptor Translated(
            this IObjectFieldDescriptor fieldDescriptor, string keyPrefix)
        {
            return fieldDescriptor
                .Directive(new TranslateDirective<string>(keyPrefix, false));
        }

        /// <summary>
        /// Transforms a Scalar-Type field into a { key label } item.
        /// </summary>
        /// <param name="fieldDescriptor">
        /// The  HotChocoalte fieldDescriptor for the field we want to make translated.
        /// </param>
        /// <param name="resourceKeyPrefix">The prefix of the resource key.</param>
        /// <param name="nullable">Indicates wheather the field's type should be nullable or not</param>
        /// <returns>The HotChocoalte fieldDescriptor for the field, made translated.</returns>
        public static IObjectFieldDescriptor Translate<T>(
            this IObjectFieldDescriptor fieldDescriptor, string resourceKeyPrefix, bool nullable = false)
        {
            if (!nullable)
            {
                fieldDescriptor
                    .Type<NonNullType<TranslatedResourceType<T>>>();
            }
            else
            {
                fieldDescriptor
                    .Type<TranslatedResourceType<T>>();
            }

            return fieldDescriptor
                .Directive(new TranslateDirective<T>(resourceKeyPrefix, toCodeLabelItem: true));
        }

        public static IObjectFieldDescriptor Translate<T>(
            this IObjectFieldDescriptor fieldDescriptor, Type resourceSource, bool nullable = false)
        {
            if (!nullable)
            {
                fieldDescriptor
                    .Type<NonNullType<TranslatedResourceType<T>>>();
            }
            else
            {
                fieldDescriptor
                    .Type<TranslatedResourceType<T>>();
            }

            return AddTranslateDirective<T>(fieldDescriptor, resourceSource);;
        }

        public static IInterfaceFieldDescriptor Translate<T>(
            this IInterfaceFieldDescriptor fieldDescriptor, bool nullable = false)
        {
            if (!nullable)
            {
                fieldDescriptor
                    .Type<NonNullType<TranslatedResourceType<T>>>();
            }
            else
            {
                fieldDescriptor
                    .Type<TranslatedResourceType<T>>();
            }

            return fieldDescriptor;
        }

        /// <summary>
        /// Transforms an array of Scalar-types into an array of { key label } items.
        /// </summary>
        /// <param name="fieldDescriptor">
        /// The  HotChocoalte fieldDescriptor for the field we want to make translated.
        /// </param>
        /// <param name="resourceKeyPrefix">The prefix of the resource key.</param>
        /// <param name="nullable">Indicates wheather the field's type should be nullable or not</param>
        /// <returns>The HotChocoalte fieldDescriptor for the field, made translated.</returns>
        public static IObjectFieldDescriptor TranslateArray<T>(
            this IObjectFieldDescriptor fieldDescriptor, string resourceKeyPrefix, bool nullable = false)
        {
            if (!nullable)
            {
                fieldDescriptor
                    .Type<NonNullType<ListType<NonNullType<TranslatedResourceType<T>>>>>();
            }
            else
            {
                fieldDescriptor
                    .Type<ListType<NonNullType<TranslatedResourceType<T>>>>();
            }

            return fieldDescriptor
                .Directive(new TranslateDirective<T>(resourceKeyPrefix, toCodeLabelItem: true));
        }

        public static IObjectFieldDescriptor TranslateArray<T>(
            this IObjectFieldDescriptor fieldDescriptor, Type resourceSource, bool nullable = false)
        {
            if (!nullable)
            {
                fieldDescriptor
                    .Type<NonNullType<ListType<NonNullType<TranslatedResourceType<T>>>>>();
            }
            else
            {
                fieldDescriptor
                    .Type<ListType<NonNullType<TranslatedResourceType<T>>>>();
            }

            return AddTranslateDirective<T>(fieldDescriptor, resourceSource);
        }

        public static IInterfaceFieldDescriptor TranslateArray<T>(
            this IInterfaceFieldDescriptor fieldDescriptor, bool nullable = false)
        {
            if (!nullable)
            {
                fieldDescriptor
                    .Type<NonNullType<ListType<NonNullType<TranslatedResourceType<T>>>>>();
            }
            else
            {
                fieldDescriptor
                    .Type<ListType<NonNullType<TranslatedResourceType<T>>>>();
            }

            return fieldDescriptor;
        }

        public static IObjectFieldDescriptor TranslateArray(
            this IObjectFieldDescriptor fieldDescriptor, string keyPrefix, bool nullable = false)
        {
            return fieldDescriptor.TranslateArray<string>(keyPrefix, nullable);
        }

        public static IObjectFieldDescriptor TranslateArray(
            this IObjectFieldDescriptor fieldDescriptor, Type resourceSource, bool nullable = false)
        {
            return fieldDescriptor.TranslateArray<string>(resourceSource, nullable);
        }

        private static IObjectFieldDescriptor AddTranslateDirective<T>(
            IObjectFieldDescriptor fieldDescriptor, Type resourceSource)
        {
            if (fieldDescriptor is IHasDescriptorContext contextHolder)
            {
                IResourceTypeResolver? resourceTypeResolver =
                    contextHolder.Context.Services.GetService<IResourceTypeResolver>();

                if (resourceTypeResolver != null)
                {
                    string resourceKeyPrefix = resourceTypeResolver.GetAlias(resourceSource);

                    return fieldDescriptor
                        .Directive(new TranslateDirective<T>(resourceKeyPrefix, toCodeLabelItem: true));
                }
            }

            return fieldDescriptor;
        }
    }
}
