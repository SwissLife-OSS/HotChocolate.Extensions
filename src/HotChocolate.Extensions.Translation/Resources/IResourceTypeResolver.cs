using System;

namespace HotChocolate.Extensions.Translation.Resources;

public interface IResourceTypeResolver
{
    Type? Resolve(string alias);
    string GetAlias(Type resourceSource);
    Type LookupLocalizer(Type resourceSource);
}
