using System;
using HotChocolate.Types;

namespace HotChocolate.Extensions.Translation
{
    public static class ObjectFieldDescriptorExtensions
    {
        /// <summary>
        /// Makes a field translatable. A translatable field is not-translated by default.
        /// To translate it, the user has to query this field with the
        /// field-directive @translate attached to it. Example:
        /// { foo @translate }
        /// </summary>
        /// <param name="fieldDescriptor">
        /// The  HotChocoalte fieldDescriptor for the field we want to make translatable.
        /// </param>
        /// <param name="keyPrefix">The prefix of the resource key.</param>
        /// <returns>The HotChocoalte fieldDescriptor for the field, made translatable.</returns>
        public static IObjectFieldDescriptor Translatable(
            this IObjectFieldDescriptor fieldDescriptor, string keyPrefix)
        {
            return fieldDescriptor
                .Directive(new TranslatableDirective(keyPrefix, false));
        }

        /// <summary>
        /// Makes a field translated. In this case, the field value always gets
        /// translated to a language resource when invoked. 
        /// </summary>
        /// <param name="fieldDescriptor">
        /// The  HotChocoalte fieldDescriptor for the field we want to make translated.
        /// </param>
        /// <param name="keyPrefix">The prefix of the resource key.</param>
        /// <returns>The HotChocoalte fieldDescriptor for the field, made translated.</returns>
        public static IObjectFieldDescriptor Translated(
            this IObjectFieldDescriptor fieldDescriptor, string keyPrefix)
        {
            return fieldDescriptor
                .Directive(new TranslatableDirective(keyPrefix, false))
                .Directive(new TranslateDirective<string>());
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
                .Directive(new TranslatableDirective(resourceKeyPrefix, toCodeLabelItem: true))
                .Directive(new TranslateDirective<T>());
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
                .Directive(new TranslatableDirective(resourceKeyPrefix, toCodeLabelItem: true))
                .Directive(new TranslateDirective<T>());
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
    }
}
