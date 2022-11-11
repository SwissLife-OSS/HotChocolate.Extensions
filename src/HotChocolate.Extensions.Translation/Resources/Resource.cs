namespace HotChocolate.Extensions.Translation.Resources
{
    public class Resource
    {
        public Resource(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }
    }
}
