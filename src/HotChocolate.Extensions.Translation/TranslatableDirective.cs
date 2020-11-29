namespace HotChocolate.Extensions.Translation
{
    public class TranslatableDirective
    {
        public TranslatableDirective(
            string resourceKeyPrefix,
            bool toCodeLabelItem)
        {
            ResourceKeyPrefix = resourceKeyPrefix;
            ToCodeLabelItem = toCodeLabelItem;
        }

        public string ResourceKeyPrefix { get; }
        public bool ToCodeLabelItem { get; }
    }
}
