using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace HotChocolate.Extensions.Translation.Resources;

public class DefaultLocalizerFactory : IStringLocalizerFactory
{
    private readonly IResourceTypeResolver _resourceTypeResolver;
    private readonly IServiceProvider _serviceProvider;

    public DefaultLocalizerFactory(
        IResourceTypeResolver resourceTypeResolver,
        IServiceProvider serviceProvider)
    {
        _resourceTypeResolver = resourceTypeResolver;
        _serviceProvider = serviceProvider;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        Type localizer = _resourceTypeResolver.LookupLocalizer(resourceSource);

        return (IStringLocalizer)_serviceProvider.GetRequiredService(localizer);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        Type? resourceSource = _resourceTypeResolver.Resolve(baseName);

        if (resourceSource == null)
        {
            throw new InvalidOperationException(
                $"No resource source type has been registered for {baseName} alias.");
        }

        return Create(resourceSource!);
    }
}

