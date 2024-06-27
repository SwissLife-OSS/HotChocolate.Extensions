using System;

namespace HotChocolate.Extensions.Translation.Resources;

[AttributeUsage(
    AttributeTargets.Class,
    Inherited = true,
    AllowMultiple = false)]
public class ResourceTypeAliasAttribute : Attribute
{
    public ResourceTypeAliasAttribute(string Value)
    {
        this.Value = Value;
    }

    public string Value { get; }
}
