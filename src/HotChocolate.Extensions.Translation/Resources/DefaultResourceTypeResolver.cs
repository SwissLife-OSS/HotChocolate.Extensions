using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace HotChocolate.Extensions.Translation.Resources;

public class DefaultResourceTypeResolver : IResourceTypeResolver
{
    private readonly ConcurrentDictionary<string, Type> _aliasToTypeMap = new();
    private readonly ConcurrentDictionary<Type, string> _typeToAliasMap = new();
    private readonly ConcurrentDictionary<Type, Type> _typeToLocalizerTypeMap = new();
    
    public void RegisterType(Type resourceSource, Type localizer)
    {
        if (!TryGetAliasFromAttribute(resourceSource, out string? alias))
        {
            alias = $"{resourceSource.Namespace?.Replace(".", "::")}::{resourceSource.Name}";
        }

        _aliasToTypeMap[alias!] = resourceSource;
        _typeToAliasMap[resourceSource] = alias!;
        _typeToLocalizerTypeMap[resourceSource] = localizer;
    }

    public Type? Resolve(string alias)
    {
        return _aliasToTypeMap.TryGetValue(alias, out Type? type) ? type : null;
    }

    public string GetAlias(Type type)
    {
        if (_typeToAliasMap.TryGetValue(type, out var alias))
        {
            return alias;
        }

        throw new KeyNotFoundException($"No alias is registered for {type.Name} type.");
    }

    public Type LookupLocalizer(Type resourceSource)
    {
        return _typeToLocalizerTypeMap[resourceSource];
    }

    private static bool TryGetAliasFromAttribute(Type type, out string? aliasValue)
    {
        aliasValue = type.GetCustomAttribute<ResourceTypeAliasAttribute>()?.Value;

        return aliasValue is not null;
    }
}
