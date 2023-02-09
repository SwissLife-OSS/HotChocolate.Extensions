using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using Snapshooter.Xunit;
using Xunit;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation.Tests.Mock;
using HotChocolate.Extensions.Translation.Resources;

namespace HotChocolate.Extensions.Translation.Tests.IntegrationTests
{
    public class TranslationTests
    {
        [Fact]
        public async Task Translate_WithNonNullableEnum_ShouldTranslateProperly()
        {
            //Arrange

            System.Threading.Thread.CurrentThread.CurrentCulture
                = new System.Globalization.CultureInfo("en");

            DummyValues keys = DummyValues.Bar;

            var query = "{ foo { key label } }";

            IRequestExecutorBuilder builder = new ServiceCollection()
                .AddSingleton<IResourcesProviderAdapter>(new EvergreenResourcesProviderAdapter())
                .AddGraphQL()
                .AddDirectiveType<TranslateDirectiveType<DummyValues>>()
                .AddQueryType(d =>
                    d.Field("foo")
                        .Resolver(keys)
                        .Translate<DummyValues>("prefix")
                );

            //Act
            IExecutionResult result = await builder.ExecuteRequestAsync(
                QueryRequestBuilder.New().SetQuery(query).Create());

            //Assert
            Snapshot.Match(result);
        }

        public enum DummyValues
        {
            Foo,
            Bar,
            Baz,
            Qux
        }
    }
}
