using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HotChocolate.Extensions.Translation.Resources;
using HotChocolate.Extensions.Translation.Tests.Mock;
using HotChocolate.Resolvers;
using Moq;
using Xunit;

namespace HotChocolate.Extensions.Translation.Tests
{
    public class TranslateDirectiveTypeTests
    {
        [Fact]
        public void UpdateResult_StringValueAndLanguageSet_ReturnsStringValue()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            string value = "myValue";

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> { { "myNodePath/myValue", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().Be("foo/De");
        }

        [Fact]
        public void UpdateResult_StringValueAndThreadlanguage_ReturnsStringValue()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);

            string value = "myValue";

            System.Threading.Thread.CurrentThread.CurrentCulture
                = new System.Globalization.CultureInfo("fr-CH");

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> { { "myNodePath/myValue", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(context, value, directive);

            //Assert
            context.Result.Should().Be("foo/Fr");
        }

        [Fact]
        public void UpdateResult_StringValueToCodeLabelItem_ReturnsStringValue()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", toCodeLabelItem: true);
            string value = "myValue";

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> { { "myNodePath/myValue", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().BeEquivalentTo(new TranslatedResource<string>("myValue", "foo/De"));
        }

        [InlineData((sbyte)1)]
        [InlineData((byte)1)]
        [InlineData((short)1)]
        [InlineData((ushort)1)]
        [InlineData((int)1)]
        [InlineData((uint)1)]
        [InlineData((long)1)]
        [InlineData((ulong)1)]
        [Theory]
        public void UpdateResult_NullableIntValueAndThreadlanguage_ReturnsStringValue(object value)
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            System.Threading.Thread.CurrentThread.CurrentCulture
                = new System.Globalization.CultureInfo("fr-CH");
            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> { { "myNodePath/1", "foo" } });
            //Act
            TranslateDirectiveType.UpdateResult(context, value, directive);
            //Assert
            context.Result.Should().Be("foo/Fr");
        }

        [Fact]
        public void UpdateResult_StringValue_MissingKey_ReturnsKey()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            string value = "myValue";

            System.Threading.Thread.CurrentThread.CurrentCulture
                = new System.Globalization.CultureInfo("fr-CH");

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string>());

            //Act
            TranslateDirectiveType.UpdateResult(context, value, directive);

            //Assert
            context.Result.Should().Be("myValue");
        }

        [Fact]
        public void UpdateResult_EnumerableStringValue_ReturnsStringValue()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            IEnumerable<string> value = new List<string>
            {
                "myValue1",
                "myValue2"
            };

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> {
                    { "myNodePath/myValue1", "foo" },
                    { "myNodePath/myValue2", "bar" },
                });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().BeEquivalentTo(
                new List<string>
                {
                    "foo/De",
                    "bar/De"
                }
            );
        }

        [Fact]
        public void UpdateResult_ArrayEmpty_ShouldNotThrowException()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            IEnumerable<string> value = new List<string>();

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> {
                    { "mynodepath/myValue1", "foo" },
                    { "mynodepath/myValue2", "bar" },
                });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().BeEquivalentTo(new List<string>());
        }

        [Fact]
        public void UpdateResult_EnumerableStringValueWithSomeMissingKeys_ReturnsStringValueAndKeysWhenValueIsMissing()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            IEnumerable<string> resolverResult = new List<string>
            {
                "myValue1",
                "myValue2",
                "myValue3",
            };

            IMiddlewareContext context = BuildMockContext(
                resolverResult: resolverResult,
                resourceDictionary: new Dictionary<string, string> {
                    { "myNodePath/myValue1", "foo" },
                    { "myNodePath/myValue3", "bar" },
                });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, resolverResult, directive, TranslatableLanguage.De);

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

        [Fact]
        public void UpdateResult_EnumValue_ReturnsStringValue()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            TestEnum value = TestEnum.SecondEnum;

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> { { "myNodePath/SecondEnum", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().Be("foo/De");
        }

        public enum TestEnum
        {
            FirstEnum,
            SecondEnum
        }

        private static IMiddlewareContext BuildMockContext(
            object resolverResult,
            Dictionary<string, string> resourceDictionary)
        {
            IDictionary<Mock.Language, Dictionary<string, Resource>> masterDictionary
                = new Dictionary<Mock.Language, Dictionary<string, Resource>>();

            masterDictionary.Add(Mock.Language.De, ToResourceDictionary(resourceDictionary, Mock.Language.De));
            masterDictionary.Add(Mock.Language.Fr, ToResourceDictionary(resourceDictionary, Mock.Language.Fr));
            masterDictionary.Add(Mock.Language.It, ToResourceDictionary(resourceDictionary, Mock.Language.It));
            masterDictionary.Add(Mock.Language.En, ToResourceDictionary(resourceDictionary, Mock.Language.En));

            var contextMock = new Mock<IMiddlewareContext>();
            var resourceClientMock = new DictionaryResourcesClient(masterDictionary);

            contextMock.SetupProperty(m => m.Result, resolverResult);
            IMiddlewareContext context = contextMock.Object;

            contextMock
                .Setup(m => m.Service<IResourcesProvider>())
                .Returns(resourceClientMock);
            return context;
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
