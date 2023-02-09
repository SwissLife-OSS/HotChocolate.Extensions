using FluentAssertions;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
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
                descriptor.Field("foo").Resolve("bar")
                    .Type<NonNullType<StringType>>();
            }
        }

        [Fact]
        public void RegisterTranslation_WithBuilder_ShouldReturnSameBuilder()
        {
            //Arrange
            ISchemaBuilder builder = SchemaBuilder.New();

            //Act
            ISchemaBuilder builder2 = builder.AddTranslation();

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
            builder.AddTranslation();

            //Assert
            ISchema schema = builder.Create();
            schema.DirectiveTypes.Should().Contain(d => d.RuntimeType
                == typeof(TranslateDirective<string>));
        }
    }
}
