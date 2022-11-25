namespace HotChocolate.Extensions.Translation
{
    public class TranslatedResource<T>: ITranslation
    {
        public TranslatedResource(T key, string label)
        {
            Key = key;
            Label = label;
        }

        public T Key { get; }
        public string Label { get; }
    }
}
