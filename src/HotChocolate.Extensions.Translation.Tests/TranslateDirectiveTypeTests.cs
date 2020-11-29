using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using HotChocolate.Extensions.Translation.Tests.Mock;
using HotChocolate.Resolvers;
using Moq;
using SwissLife.F2c.Resources.Contract;
using SwissLife.Resources.ResourcesClient;
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
                new Dictionary<string, string> { { "mynodepath/myvalue", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().Be("foo/de");
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
                new Dictionary<string, string> { { "mynodepath/myvalue", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(context, value, directive);

            //Assert
            context.Result.Should().Be("foo/fr");
        }

        [Fact]
        public void UpdateResult_StringValueToCodeLabelItem_ReturnsStringValue()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", toCodeLabelItem: true);
            string value = "myValue";

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> { { "mynodepath/myvalue", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().BeEquivalentTo(new TranslatedResource<string>("myValue", "foo/de"));
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
                new Dictionary<string, string> { { "mynodepath/1", "foo" } });
            //Act
            TranslateDirectiveType.UpdateResult(context, value, directive);
            //Assert
            context.Result.Should().Be("foo/fr");
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
                    { "mynodepath/myvalue1", "foo" },
                    { "mynodepath/myvalue2", "bar" },
                });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().BeEquivalentTo(
                new List<string>
                {
                    "foo/de",
                    "bar/de"
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
                    { "mynodepath/myvalue1", "foo" },
                    { "mynodepath/myvalue2", "bar" },
                });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().BeEquivalentTo(new List<string>());
        }

        [Fact]
        public void UpdateResult_EnumerableStringValue__SomeMissingKeys_ReturnsStringValueAndKeysWhenValueIsMissing()
        {
            //Arrange
            var directive = new TranslatableDirective("myNodePath", false);
            IEnumerable<string> value = new List<string>
            {
                "myValue1",
                "myValue2",
                "myValue3",
            };

            IMiddlewareContext context = BuildMockContext(
                value,
                new Dictionary<string, string> {
                    { "mynodepath/myvalue1", "foo" },
                    { "mynodepath/myvalue3", "bar" },
                });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().BeEquivalentTo(
                new List<string>
                {
                    "foo/de",
                    "myValue2",
                    "bar/de"
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
                new Dictionary<string, string> { { "mynodepath/secondenum", "foo" } });

            //Act
            TranslateDirectiveType.UpdateResult(
                context, value, directive, TranslatableLanguage.De);

            //Assert
            context.Result.Should().Be("foo/de");
        }

        public enum TestEnum
        {
            FirstEnum,
            SecondEnum
        }

        private static IMiddlewareContext BuildMockContext(
            object value,
            Dictionary<string, string> dictionary)
        {
            IDictionary<Language, Dictionary<string, Resource>> masterDictionary
                = new Dictionary<Language, Dictionary<string, Resource>>();

            masterDictionary.Add(Language.de, ToResourceDictionary(dictionary, Language.de));
            masterDictionary.Add(Language.fr, ToResourceDictionary(dictionary, Language.fr));
            masterDictionary.Add(Language.it, ToResourceDictionary(dictionary, Language.it));
            masterDictionary.Add(Language.en, ToResourceDictionary(dictionary, Language.en));

            var contextMock = new Mock<IMiddlewareContext>();
            var resourceClientMock = new DictionaryResourcesClient(
                masterDictionary);

            contextMock.SetupProperty(m => m.Result, value);
            IMiddlewareContext context = contextMock.Object;

            contextMock.Setup(m => m.Service<IResourcesClient>())
                .Returns(resourceClientMock);
            return context;
        }

        private static Dictionary<string, Resource> ToResourceDictionary(
            IDictionary<string, string> dictionary, Language lang)
        {
            return dictionary.ToDictionary(
                i => i.Key,
                i => new Resource
                {
                    Key = i.Key,
                    Value = $"{i.Value}/{lang}"
                });
        }
    }
}
