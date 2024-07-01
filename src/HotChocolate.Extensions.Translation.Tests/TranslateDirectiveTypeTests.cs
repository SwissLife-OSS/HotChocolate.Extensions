using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate.Extensions.Translation.Resources;
using HotChocolate.Extensions.Translation.Tests.Mock;
using HotChocolate.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace HotChocolate.Extensions.Translation.Tests
{
    public partial class TranslateDirectiveTypeTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateResult_StringValueAndLanguageSet_ReturnsStringValue(
            bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", false);
            string value = "myValue";

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                value,
                new Dictionary<string, string> { { "myNodePath/myValue", "foo" } });

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(
                context, value, directive, default);

            //Assert
            context.Result.Should().Be("foo/De");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateResult_StringValueToCodeLabelItem_ReturnsStringValue(
            bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", toCodeLabelItem: true);
            string value = "myValue";

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                value,
                new Dictionary<string, string> { { "myNodePath/myValue", "foo" } });

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(
                context, value, directive, default);

            //Assert
            context.Result.Should().BeEquivalentTo(new TranslatedResource<string>("myValue", "foo/De"));
        }

        [InlineData((sbyte)1, false)]
        [InlineData((byte)1, false)]
        [InlineData((short)1, false)]
        [InlineData((ushort)1, false)]
        [InlineData((int)1, false)]
        [InlineData((uint)1, false)]
        [InlineData((long)1, false)]
        [InlineData((ulong)1, false)]
        [InlineData((sbyte)1, true)]
        [InlineData((byte)1, true)]
        [InlineData((short)1, true)]
        [InlineData((ushort)1, true)]
        [InlineData((int)1, true)]
        [InlineData((uint)1, true)]
        [InlineData((long)1, true)]
        [InlineData((ulong)1, true)]
        [Theory]
        public async Task UpdateResult_NullableIntValue_ReturnsStringValue(object value, bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", false);

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                value,
                new Dictionary<string, string> { { "myNodePath/1", "foo" } });

            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(context, value, directive, default);

            //Assert
            context.Result.Should().Be("foo/Fr");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateResult_StringValueMissingKey_ReturnsKey(bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", false);
            string value = "myValue";

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                value,
                new Dictionary<string, string>());

            Thread.CurrentThread.CurrentCulture = new CultureInfo("fr");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(context, value, directive, default);

            //Assert
            context.Result.Should().Be("myValue");
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateResult_EnumerableStringValue_ReturnsStringValue(bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", false);
            IEnumerable<string> value = new List<string>
            {
                "myValue1",
                "myValue2"
            };

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                value,
                new Dictionary<string, string> {
                    { "myNodePath/myValue1", "foo" },
                    { "myNodePath/myValue2", "bar" },
                });

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(
                context, value, directive, default);

            //Assert
            context.Result.Should().BeEquivalentTo(
                new List<string>
                {
                    "foo/De",
                    "bar/De"
                }
            );
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateResult_ArrayEmpty_ShouldNotThrowException(bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", false);
            IEnumerable<string> value = new List<string>();

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                value,
                new Dictionary<string, string> {
                    { "mynodepath/myValue1", "foo" },
                    { "mynodepath/myValue2", "bar" },
                });

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(
                context, value, directive, default);

            //Assert
            context.Result.Should().BeEquivalentTo(new List<string>());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateResult_EnumerableStringValueWithSomeMissingKeys_ReturnsStringValueAndKeysWhenValueIsMissing(bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", false);
            IEnumerable<string> resolverResult = new List<string>
            {
                "myValue1",
                "myValue2",
                "myValue3",
            };

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                resolverResult: resolverResult,
                resourceDictionary: new Dictionary<string, string> {
                    { "myNodePath/myValue1", "foo" },
                    { "myNodePath/myValue3", "bar" },
                });

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(
                context, resolverResult, directive, default);

            //Assert
            context.Result.Should().BeEquivalentTo(
                new List<string>
                {
                    "foo/De",
                    "myValue2",
                    "bar/De"
                }
            );
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateResult_EnumValue_ReturnsStringValue(bool useStringLocalizer)
        {
            //Arrange
            var directive = new TranslateDirective("myNodePath", false);
            TestEnum value = TestEnum.SecondEnum;

            IMiddlewareContext context = BuildMockContext(
                useStringLocalizer,
                value,
                new Dictionary<string, string> { { "myNodePath/SecondEnum", "foo" } });

            Thread.CurrentThread.CurrentCulture = new CultureInfo("de");

            //Act
            await TranslateDirectiveType.UpdateResultAsync(
                context, value, directive, default);

            //Assert
            context.Result.Should().Be("foo/De");
        }

        public enum TestEnum
        {
            FirstEnum,
            SecondEnum
        }

        private static IMiddlewareContext BuildMockContext(
            bool useStringLocalizer,
            object resolverResult,
            Dictionary<string, string> resourceDictionary)
        {
            DictionaryResourcesProviderAdapter resourceClientAdapter =
                CreateResourceProviderAdapter(resourceDictionary);

            var contextMock = new Mock<IMiddlewareContext>();

            contextMock.SetupProperty(m => m.Result, resolverResult);

            IServiceCollection services = new ServiceCollection();

            if (useStringLocalizer)
            {
                services.AddStringLocalizer<TestStringLocalizer>(
                    ServiceLifetime.Singleton, typeof(TestResourceType));
                services.AddSingleton<Func<IDictionary<Mock.Language, Dictionary<string, Resource>>>>(
                    () => GetMasterDictionary(resourceDictionary));
            }
            else
            {
                contextMock
                    .Setup(m => m.Service<IResourcesProviderAdapter>())
                    .Returns(resourceClientAdapter);
            }

            contextMock
                .SetupGet(m => m.Services)
                .Returns(services.BuildServiceProvider());

            return contextMock.Object;
        }

        private static DictionaryResourcesProviderAdapter CreateResourceProviderAdapter(
            Dictionary<string, string> resourceDictionary)
        {
            IDictionary<Mock.Language, Dictionary<string, Resource>> masterDictionary =
                GetMasterDictionary(resourceDictionary);

            var resourceClientAdapter = new DictionaryResourcesProviderAdapter(masterDictionary);
            return resourceClientAdapter;
        }

        private static IDictionary<Mock.Language, Dictionary<string, Resource>> GetMasterDictionary(
            Dictionary<string, string> resourceDictionary)
        {
            IDictionary<Mock.Language, Dictionary<string, Resource>> masterDictionary
                = new Dictionary<Mock.Language, Dictionary<string, Resource>>();

            masterDictionary.Add(Mock.Language.De,
                ToResourceDictionary(resourceDictionary, Mock.Language.De));
            masterDictionary.Add(Mock.Language.Fr,
                ToResourceDictionary(resourceDictionary, Mock.Language.Fr));
            masterDictionary.Add(Mock.Language.It,
                ToResourceDictionary(resourceDictionary, Mock.Language.It));
            masterDictionary.Add(Mock.Language.En,
                ToResourceDictionary(resourceDictionary, Mock.Language.En));
            return masterDictionary;
        }

        private static Dictionary<string, Resource> ToResourceDictionary(
            IDictionary<string, string> dictionary, Mock.Language lang)
        {
            return dictionary.ToDictionary(
                i => i.Key,
                i => new Resource(
                    key: i.Key,
                    value: $"{i.Value}/{lang}"
                ));
        }
    }
}
