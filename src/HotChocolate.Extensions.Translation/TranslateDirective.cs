using System;

namespace HotChocolate.Extensions.Translation
{
    public class TranslateDirective
    {
        public TranslateDirective(
            string resourceKeyPrefix,
            bool toCodeLabelItem)
        {
            if (string.IsNullOrWhiteSpace(resourceKeyPrefix))
            {
                throw new ArgumentException(
                    $"'{nameof(resourceKeyPrefix)}' cannot be null or whitespace.",
                    nameof(resourceKeyPrefix));
            }

            ResourceKeyPrefix = resourceKeyPrefix;
            ToCodeLabelItem = toCodeLabelItem;
        }

        public string ResourceKeyPrefix { get; }
        public bool ToCodeLabelItem { get; }
    }

    public class TranslateDirective<T>: TranslateDirective
    {
        public TranslateDirective(
            string resourceKeyPrefix,
            bool toCodeLabelItem)
            : base(resourceKeyPrefix, toCodeLabelItem)
        {
        }
    }
}
