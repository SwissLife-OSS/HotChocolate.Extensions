using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace HotChocolate.Extensions.Translation.Resources;

public class DefaultLocalizerFactory : IStringLocalizerFactory
{
    private readonly IResourceTypeResolver _resourceTypeResolver;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DefaultLocalizerFactory(
        IResourceTypeResolver resourceTypeResolver,
        IHttpContextAccessor httpContextAccessor)
    {
        _resourceTypeResolver = resourceTypeResolver;
        _httpContextAccessor = httpContextAccessor;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        Type localizer = _resourceTypeResolver.LookupLocalizer(resourceSource);

        return (IStringLocalizer)_httpContextAccessor.HttpContext.RequestServices
            .GetRequiredService(localizer);
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

