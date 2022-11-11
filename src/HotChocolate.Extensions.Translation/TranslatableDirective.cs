using System;

namespace HotChocolate.Extensions.Translation
{
    public class TranslatableDirective
    {
        public TranslatableDirective(
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
}
