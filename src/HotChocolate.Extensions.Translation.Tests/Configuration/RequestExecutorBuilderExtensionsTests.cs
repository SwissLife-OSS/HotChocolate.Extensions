using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate.Execution;
using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;

namespace HotChocolate.Extensions.Translation.Tests.Configuration
{
    public class RequestExecutorBuilderExtensionsTests
    {
        public class Query
        {
            public string Foo { get; } = "bar";
            public DummyEnum Bar { get; } = DummyEnum.Qux;
        }

        public enum DummyEnum
        {
            Qux
        }

        [Fact]
        public async Task AddTranslation_WithTranslatedEnum_ShouldAddTheTypeForTheTranslatedEnum()
        {
            //Arrange
            var services = new ServiceCollection();
            IRequestExecutorBuilder builder = services
                .AddGraphQLServer()
                .AddQueryType<Query>(d =>
                {
                    d.Field(q => q.Bar).Translate<DummyEnum>("translation_path");
                });

            //Act
            builder = builder
                .AddTranslation(c => c
                    .AddTranslatableType<DummyEnum>()
                );

            //Assert
            ISchema schema = await builder.BuildSchemaAsync();
            schema.ToString().Should().MatchSnapshot();
        }

        [Fact]
        public async Task AddTranslation_WithCustomNameForInterface_ShouldAddTheInterfaceWithTheCorrectName()
        {
            //Arrange
            var services = new ServiceCollection();
            IRequestExecutorBuilder builder = services
                .AddGraphQLServer()
                .AddQueryType<Query>();

            //Act
            builder = builder
                .AddTranslation(c => c
                    .SetTranslationInterfaceName("MyTranslationInterfaceType")
                );

            //Assert
            ISchema schema = await builder.BuildSchemaAsync();
            schema.ToString().Should().MatchSnapshot();
        }
    }
}
