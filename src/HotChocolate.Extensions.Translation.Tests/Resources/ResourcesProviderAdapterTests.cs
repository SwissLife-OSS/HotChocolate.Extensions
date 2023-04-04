using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate.Extensions.Translation.Resources;
using Moq;
using Xunit;

namespace HotChocolate.Extensions.Translation.Tests.Resources
{
    public class ResourcesProviderAdapterTests
    {
        [Fact]
        public async Task TryGetTranslationAsString_WhenResourceFound_ShouldReturnResource()
        {
            //Arrange testData
            var key = GetRandomString();
            var culture = new CultureInfo("de");
            Resource? resource = new Resource(key, value: GetRandomString());

            //Arrange dependencies
            var provider = new Mock<IResourcesProvider>(MockBehavior.Strict);
            provider
                .Setup(p => p.TryGetResourceAsync(key, culture, It.IsAny<CancellationToken>()))
                .ReturnsAsync(resource);

            var adapter = new ResourcesProviderAdapter(provider.Object, observers: default!);

            //Act
            var value = await adapter.TryGetTranslationAsStringAsync(key, culture, default!, default);

            //Assert
            value.Should().Be(resource.Value);
        }

        [Fact]
        public async Task TryGetTranslationAsString_WhenResourceNotFound_ShouldReturnFallBakValue()
        {
            //Arrange testData
            var key = GetRandomString();
            var culture = new CultureInfo("de");
            var fallBackValue = GetRandomString();
            Resource? resource = null;

            //Arrange dependencies
            var provider = new Mock<IResourcesProvider>(MockBehavior.Strict);
            provider
                .Setup(p => p.TryGetResourceAsync(key, culture, It.IsAny<CancellationToken>()))
                .ReturnsAsync(resource);
            
            var observer = new DummyObserver();

            var adapter = new ResourcesProviderAdapter(provider.Object, observers: new[] { observer });

            //Act
            var value = await adapter.TryGetTranslationAsStringAsync(key, culture, fallBackValue, default);

            //Assert
            value.Should().Be(fallBackValue);
            observer.MissingResources.Should().ContainSingle().Which.Should().Be(key);
        }

        private static string GetRandomString()
        {
            return Guid.NewGuid().ToString();
        }

        public class DummyObserver : TranslationObserver
        {
            internal List<string> MissingResources { get; }

            public DummyObserver()
            {
                MissingResources = new List<string>();
            }

            public override Task OnMissingResource(string key)
            {
                MissingResources.Add(key);

                return base.OnMissingResource(key);
            }
        }
    }
}
