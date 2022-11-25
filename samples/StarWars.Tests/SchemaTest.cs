using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Snapshooter.Xunit;
using Xunit;

namespace StarWars.Tests
{
    public class SchemaTest
    {
        [Fact]
        public async Task SchemaSnapshot()
        {
            //Arrange
            var services = new ServiceCollection();
            var configurationBuilder = new ConfigurationBuilder();

            //Act
            ISchema schema = await services
                .AddGraphQLServer()
                    .AddGraphQLSchema()
                .BuildSchemaAsync();

            //Assert
            schema.ToString().MatchSnapshot("schema");
        }
    }
}
