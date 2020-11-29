using FluentAssertions;
using HotChocolate;
using HotChocolate.Types;
using Xunit;

namespace HotChocolate.Extensions.Translation.Tests
{
    public class SchemaBuilderExtensionsTests
    {
        private class QueryType : ObjectType
        {
            protected override void Configure(IObjectTypeDescriptor descriptor)
            {
                descriptor.Name("Query");
                descriptor.Field("foo").Resolver("bar")
                    .Type<NonNullType<StringType>>();
            }
        }

        [Fact]
        public void RegisterTranslation_WithBuilder_ShouldReturnSameBuilder()
        {
            //Arrange
            ISchemaBuilder builder = SchemaBuilder.New();

            //Act
            ISchemaBuilder builder2 = builder.RegisterTranslation();

            //Assert
            builder2.Should().Be(builder);
        }

        [Fact]
        public void RegisterTranslation_WithBuilder_ShouldAddBothTranslationDirectives()
        {
            //Arrange
            ISchemaBuilder builder = SchemaBuilder.New()
                .AddQueryType<QueryType>();

            //Act
            builder.RegisterTranslation();

            //Assert
            ISchema schema = builder.Create();
            schema.DirectiveTypes.Should().Contain(d => d.RuntimeType
                == typeof(TranslateDirective<string>));
            schema.DirectiveTypes.Should().Contain(d => d.RuntimeType
                == typeof(TranslatableDirective));
        }
    }
}