using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using HotChocolate.Execution;
using HotChocolate.Extensions.Translation.Resources;
using HotChocolate.Extensions.Translation.Tests.Mock;
using HotChocolate.Fetching;
using HotChocolate.Types;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Snapshooter.Xunit;
using Xunit;

namespace HotChocolate.Extensions.Translation.Tests
{
    public class ObjectFieldDescriptionExtensionsTests
    {
        [Fact]
        public void Translate_AsNonNullable_ShouldAddBothDirectives()
        {
            //Arrange
            var keys = new string[] { "foo", "bar", "baz" };

            //Act
            ISchema schema = SchemaBuilder.New()
                .AddDirectiveType<TranslateDirectiveType<DummyValues>>()
                .AddDirectiveType<TranslatableDirectiveType>()
                .AddQueryType(d =>
                    d.Field("myField")
                        .Resolve(keys)
                        .Translate<DummyValues>("prefix")
                ).Create();

            //Assert
            Snapshot.Match(schema.ToString());
        }

        [Fact]
        public void Translate_AsNullable_ShouldAddBothDirectives()
        {
            //Arrange
            var keys = new string[] { "foo", "bar", "baz" };

            //Act
            ISchema schema = SchemaBuilder.New()
                .AddDirectiveType<TranslateDirectiveType<DummyValues>>()
                .AddDirectiveType<TranslatableDirectiveType>()
                .AddQueryType(d =>
                    d.Field("myField")
                        .Resolve(keys)
                        .Translate<DummyValues>("prefix", nullable: true)
                ).Create();

            //Assert
            Snapshot.Match(schema.ToString());
        }

        public enum DummyValues
        {
            Foo,
            Bar,
            Baz,
            Qux
        }

        [Fact]
        public void Translated_RmsNodePathOk_ShouldAddBothDirectives()
        {
            //Arrange
            string[] keys = { "foo", "bar", "baz" };

            //Act
            ISchema schema = SchemaBuilder.New()
                .AddDirectiveType<TranslateDirectiveType>()
                .AddDirectiveType<TranslatableDirectiveType>()
                .AddQueryType(d =>
                    d.Field("myField")
                        .Resolve(keys)
                        .Translated("prefix")
                ).Create();

            //Assert
            Snapshot.Match(schema.ToString(), "Translated_RmsNodePathOk_ShouldAddBothDirectives_schema");
        }

        [InlineData("en")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("it")]
        [Theory]
        public async Task Translated_StringValue_ShouldTranslate(string culture)
        {
            //Arrange
            CultureInfo oldCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);

                var key = "foo";
                IServiceProvider services = new ServiceCollection()
                    .AddSingleton<IResourcesProviderAdapter>(new EvergreenResourcesProviderAdapter())
                    .AddGraphQLCore()
                    .BuildServiceProvider();

                IReadOnlyQueryRequest request = QueryRequestBuilder.New()
                    .SetQuery(@"{ myField }")
                    .SetServices(services)
                    .Create();

                ISchema schema = SchemaBuilder.New()
                    .AddServices(services)
                    .AddDirectiveType<TranslateDirectiveType>()
                    .AddDirectiveType<TranslatableDirectiveType>()
                    .AddQueryType(d =>
                        d.Field("myField")
                            .Resolve(key)
                            .Translated("prefix")
                    ).Create();
                IRequestExecutor executor = schema.MakeExecutable();

                //Act
                IExecutionResult result = await executor.ExecuteAsync(request);

                //Assert
                result.Errors.Should().BeNull();
                Snapshot.Match(result, $"Translated_StringValue_ShouldTranslate_{culture}");
            }
            finally
            {
                //reset culture to avoid side effects in other tests
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [InlineData("en")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("it")]
        [Theory]
        public async Task Translated_ArrayOfStringValues_ShouldTranslate(string culture)
        {
            //Arrange
            CultureInfo oldCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);

                string[] keys = { "foo", "bar", "baz" };
                IServiceProvider services = new ServiceCollection()
                    .AddSingleton<IResourcesProviderAdapter>(new EvergreenResourcesProviderAdapter())
                    .AddGraphQLCore()
                    .BuildServiceProvider();

                IReadOnlyQueryRequest request = QueryRequestBuilder.New()
                    .SetQuery(@"{ myField }")
                    .SetServices(services)
                    .Create();

                ISchema schema = SchemaBuilder.New()
                    .AddServices(services)
                    .AddDirectiveType<TranslateDirectiveType>()
                    .AddDirectiveType<TranslatableDirectiveType>()
                    .AddQueryType(d =>
                        d.Field("myField")
                            .Resolve(keys)
                            .Translated("prefix")
                    ).Create();
                IRequestExecutor executor = schema.MakeExecutable();

                //Act
                IExecutionResult result = await executor.ExecuteAsync(request);

                //Assert
                result.Errors.Should().BeNull();
                Snapshot.Match(result, $"Translated_ArrayOfStringValues_ShouldTranslate_{culture}");
            }
            finally
            {
                //reset culture to avoid side effects in other tests
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [Fact]
        public void TranslateArray_RmsNodePathOk_ShouldAddBothDirectives()
        {
            //Arrange
            string[] keys = { "foo", "bar", "baz" };

            //Act
            ISchema schema = SchemaBuilder.New()
                .AddDirectiveType<TranslateDirectiveType>()
                .AddDirectiveType<TranslatableDirectiveType>()
                .AddQueryType(d =>
                    d.Field("myField")
                        .Resolve(keys)
                        .TranslateArray("prefix")
                ).Create();

            //Assert
            Snapshot.Match(schema.ToString(), "TranslateArray_RmsNodePathOk_ShouldAddBothDirectives_schema");
        }

        [Fact]
        public void TranslateArrayOfT_RmsNodePathOk_ShouldAddBothDirectives()
        {
            //Arrange
            string[] keys = { "foo", "bar", "baz" };

            //Act
            ISchema schema = SchemaBuilder.New()
                .AddDirectiveType<TranslateDirectiveType<DummyValues>>()
                .AddDirectiveType<TranslatableDirectiveType>()
                .AddQueryType(d =>
                    d.Field("myField")
                        .Resolve(keys)
                        .TranslateArray<DummyValues>("prefix")
                ).Create();

            //Assert
            Snapshot.Match(schema.ToString(), "TranslateArrayOfT_RmsNodePathOk_ShouldAddBothDirectives_schema");
        }

        [InlineData("en")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("it")]
        [Theory]
        public async Task TranslateArrayT_RmsNodePathOk_ShouldTranslate(string culture)
        {
            //Arrange
            CultureInfo oldCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);

                DummyValues[] keys = { DummyValues.Foo, DummyValues.Qux };
                IServiceProvider services = new ServiceCollection()
                    .AddSingleton<IResourcesProviderAdapter>(new EvergreenResourcesProviderAdapter())
                    .AddGraphQLCore()
                    .BuildServiceProvider();

                IReadOnlyQueryRequest request = QueryRequestBuilder.New()
                    .SetQuery(@"{ myField { key label } }")
                    .SetServices(services)
                    .Create();

                ISchema schema = SchemaBuilder.New()
                    .AddServices(services)
                    .AddDirectiveType<TranslateDirectiveType<DummyValues>>()
                    .AddDirectiveType<TranslatableDirectiveType>()
                    .AddQueryType(d =>
                        d.Field("myField")
                            .Resolve(keys)
                            .TranslateArray<DummyValues>("prefix")
                    ).Create();
                IRequestExecutor executor = schema.MakeExecutable();

                //Act
                IExecutionResult result = await executor.ExecuteAsync(request);

                //Assert
                result.Errors.Should().BeNull();
                Snapshot.Match(result, $"TranslateArrayOfT_RmsNodePathOk_ShouldTranslate_{culture}");
            }
            finally
            {
                //reset culture to avoid side effects in other tests
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [InlineData("en")]
        [InlineData("de")]
        [InlineData("fr")]
        [InlineData("it")]
        [Theory]
        public async Task TranslateArray_RmsNodePathOk_ShouldTranslate(string culture)
        {
            //Arrange
            CultureInfo oldCulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);

                string[] keys = { "foo", "bar", "baz" };
                IServiceProvider services = new ServiceCollection()
                    .AddSingleton<IResourcesProviderAdapter>(new EvergreenResourcesProviderAdapter())
                    .AddGraphQLCore()
                    .BuildServiceProvider();

                IReadOnlyQueryRequest request = QueryRequestBuilder.New()
                    .SetQuery(@"{ myField { key label } }")
                    .SetServices(services)
                    .Create();

                ISchema schema = SchemaBuilder.New()
                    .AddServices(services)
                    .AddDirectiveType<TranslateDirectiveType>()
                    .AddDirectiveType<TranslatableDirectiveType>()
                    .AddQueryType(d =>
                        d.Field("myField")
                            .Resolve(keys)
                            .TranslateArray("prefix")
                    ).Create();
                IRequestExecutor executor = schema.MakeExecutable();

                //Act
                IExecutionResult result = await executor.ExecuteAsync(request);

                //Assert
                result.Errors.Should().BeNull();
                Snapshot.Match(result, $"TranslateArray_RmsNodePathOk_ShouldTranslate_{culture}");
            }
            finally
            {
                //reset culture to avoid side effects in other tests
                Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [Fact]
        public async Task Translated_ArrayNull_ShouldReturnNull()
        {
            //Arrange
            var keys = (List<string>?)null;
            var services = new ServiceCollection();
            services.AddSingleton<IResourcesProviderAdapter>(new EvergreenResourcesProviderAdapter());

            IReadOnlyQueryRequest request = QueryRequestBuilder.New()
                .SetQuery(@"{ myField }")
                .Create();

            ISchema schema = SchemaBuilder.New()
                .AddServices(services.BuildServiceProvider())
                .AddDirectiveType<TranslateDirectiveType>()
                .AddDirectiveType<TranslatableDirectiveType>()
                .AddQueryType(d =>
                    d.Field("myField")
                        .Resolve(keys)
                        .Translated("prefix")
                ).Create();
            IRequestExecutor executor = schema.MakeExecutable();

            //Act
            IExecutionResult result = await executor.ExecuteAsync(request);

            //Assert
            result.Errors.Should().BeNull();
            Snapshot.Match(result);
        }

        [Fact]
        public async Task Translated_UnsupportedType_ShouldReturnNull()
        {
            //Arrange
            var services = new ServiceCollection();
            services.AddSingleton<IResourcesProviderAdapter>(new EvergreenResourcesProviderAdapter());

            IReadOnlyQueryRequest request = QueryRequestBuilder.New()
                .SetQuery(@"{ myField }")
                .Create();

            ISchema schema = SchemaBuilder.New()
                .AddServices(services.BuildServiceProvider())
                .AddDirectiveType<TranslateDirectiveType>()
                .AddDirectiveType<TranslatableDirectiveType>()
                .AddQueryType(d =>
                    d.Field("myField")
                        .Resolve(new Dummy())
                        .Type<ObjectType<Dummy>>()
                        .Translated("prefix")
                ).Create();
            IRequestExecutor executor = schema.MakeExecutable();

            //Act
            IExecutionResult result = await executor.ExecuteAsync(request);

            //Assert
            result.Errors.Should().NotBeNull();
        }

        [InlineData("")]
        [InlineData(" ")]
        [Theory]
        public void TranslateArray_RmsNodePathEmptyOrWhitespace_ThrowsArgumentException(
            string rmsNodePath)
        {
            //Arrange
            var services = new ServiceCollection();

            //Act
            ISchema buildSchema() =>
                SchemaBuilder
                    .New()
                    .AddServices(services.BuildServiceProvider())
                    .AddDirectiveType<TranslateDirectiveType>()
                    .AddDirectiveType<TranslatableDirectiveType>()
                    .AddQueryType(d =>
                        d.Field("myField")
                            .Resolve(new[] { "foo", "bar" })
                            .TranslateArray(rmsNodePath)
                    )
                    .Create();

            //Assert
            Assert.Throws<SchemaException>(buildSchema);
        }

        public class Dummy
        {
            public string? Foo { get; set; }
        }
    }
}
