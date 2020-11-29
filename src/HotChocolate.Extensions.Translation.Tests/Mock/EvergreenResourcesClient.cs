using System;
using System.Collections.Generic;
using System.Linq;
using SwissLife.F2c.Resources.Contract;
using SwissLife.Resources.ResourcesClient;

namespace HotChocolate.Extensions.Translation.Tests.Mock
{
    public class EvergreenResourcesClient : IResourcesClient
    {
        public Resource GetResource(string key, Language language)
        {
            return new Resource
            {
                Key = key,
                Value = $"rms:{key}_{language}"
            };
        }

        public IEnumerable<Resource> GetResources(IEnumerable<string> keys, Language language)
        {
            return keys.Select(key => new Resource
            {
                Key = key,
                Value = $"rms:{key}_{language}"
            }).ToList();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public bool TryGetResource(string key, Language language, out Resource res)
        {
            res = new Resource
            {
                Key = key,
                Value = $"rms:{key}_{language}"
            };

            return true;
        }

        public bool TryGetResources(
            IEnumerable<string> keys, Language language, out IEnumerable<Resource> resources)
        {
            resources = keys.Select(key => new Resource
            {
                Key = key,
                Value = $"rms:{key}_{language}"
            }).ToList();

            return true;
        }
    }
}
