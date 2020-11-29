using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;
using Snapshooter.Xunit;
using SwissLife.Resources.ResourcesClient;
using Xunit;
using HotChocolate.Execution.Configuration;
using HotChocolate.Extensions.Translation.Tests.Mock;

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
                .AddSingleton<IResourcesClient>(new EvergreenResourcesClient())
                .AddGraphQL()
                .AddDirectiveType<TranslateDirectiveType<DummyValues>>()
                .AddDirectiveType<TranslatableDirectiveType>()
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
