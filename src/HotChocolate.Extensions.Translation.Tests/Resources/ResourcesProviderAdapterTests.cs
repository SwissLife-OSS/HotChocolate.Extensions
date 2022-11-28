using System;
using System.Collections.Generic;
using System.Globalization;
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
        public void TryGetTranslationAsString_WhenResourceFound_ShouldReturnResource()
        {
            //Arrange testData
            var key = GetRandomString();
            var culture = new CultureInfo("de");
            Resource? resource = new Resource(key, value: GetRandomString());

            //Arrange dependencies
            var provider = new Mock<IResourcesProvider>(MockBehavior.Strict);
            provider
                .Setup(p => p.TryGetResource(key, culture, out resource))
                .Returns(true);

            var adapter = new ResourcesProviderAdapter(provider.Object, observers: default!);

            //Act
            var value = adapter.TryGetTranslationAsString(key, culture, default!);

            //Assert
            value.Should().Be(resource.Value);
        }

        [Fact]
        public void TryGetTranslationAsString_WhenResourceNotFound_ShouldReturnFallBakValue()
        {
            //Arrange testData
            var key = GetRandomString();
            var culture = new CultureInfo("de");
            var fallBackValue = GetRandomString();
            Resource? resource = null;

            //Arrange dependencies
            var provider = new Mock<IResourcesProvider>(MockBehavior.Strict);
            provider
                .Setup(p => p.TryGetResource(key, culture, out resource))
                .Returns(false);
            var observer = new DummyObserver();

            var adapter = new ResourcesProviderAdapter(provider.Object, observers: new[] { observer });

            //Act
            var value = adapter.TryGetTranslationAsString(key, culture, fallBackValue);

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
