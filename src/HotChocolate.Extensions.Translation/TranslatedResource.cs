namespace HotChocolate.Extensions.Translation
{
    public class TranslatedResource<T>
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
