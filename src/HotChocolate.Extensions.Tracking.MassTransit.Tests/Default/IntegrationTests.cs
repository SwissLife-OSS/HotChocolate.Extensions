using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Extensions.Tracking;
using HotChocolate.Extensions.Tracking.Default;
using HotChocolate.Types;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace HotChocolate.Extensions.Tracking.MassTransit.Tests.Default;

public class IntegrationTests
{
    [InlineData("{ foo }")]
    [Theory]
    public async Task RequestWithTrackedField_WithTrackingFactory_ShouldPublishTrackingEntryToBus(string query)
    {
        //Arrange

        Mock<IHttpContextAccessor> mockHttpContextAccessor = ArrangeHttpContextAccessor();

        IServiceProvider services = new ServiceCollection()
            .AddSingleton(mockHttpContextAccessor.Object)
            .AddGraphQL()
                .AddQueryType(c =>
                    c.Name("Query")
                        .Field("foo")
                        .Type<StringType>()
                        .Resolve("bar")
                        .Track("tracked"))
                .AddTrackingPipeline(
                    builder => builder.AddMassTransitRepository(
                        new MassTransitOptions(
                            new ServiceBusOptions("InMemory"))))
            .Services
            .AddSingleton(mockHttpContextAccessor.Object)
            .BuildServiceProvider();

        /* needed for the asserts */
        var publishObserver = new NotifyOnFirstPublishedEntryObserver();
        services.GetRequiredService<IMassTransitTrackingBus>()
            .ConnectPublishObserver(publishObserver);

        IHostedService trackingService = await StartTrackingBackgroundService(services);
        IRequestExecutor executor = await services
            .GetRequiredService<IRequestExecutorResolver>()
            .GetRequestExecutorAsync();

        try
        {
            //Act
            IExecutionResult res = await executor.ExecuteAsync(
                QueryRequestBuilder.New()
                    .SetQuery(query)
                    .SetServices(services)
                    .Create());

            //Assert
            res.ToMinifiedJson().Should().Be("{\"data\":{\"foo\":\"bar\"}}");

            var publishedEntity = await publishObserver.GetPublishedEntity;
            TrackingEntry trackedEntry =
                publishedEntity.Should().BeOfType<TrackingEntry>().Subject;
            trackedEntry.Tag.Should().Be("tracked");
            trackedEntry.UserEmail.Should().Be("test@email.com");
            trackedEntry.Date.Should().BeAfter(DateTimeOffset.UtcNow.AddMinutes(-1));
            trackedEntry.Date.Should().BeBefore(DateTimeOffset.UtcNow);
        }
        finally
        {
            await trackingService.StopAsync(CancellationToken.None);
        }
    }

    private static async Task<IHostedService> StartTrackingBackgroundService(
        IServiceProvider serviceProvider)
    {
        IHostedService trackingService = serviceProvider.GetRequiredService<IHostedService>();
        await trackingService.StartAsync(CancellationToken.None);
        return trackingService;
    }

    private static Mock<IHttpContextAccessor> ArrangeHttpContextAccessor()
    {
        var httpContext = new DefaultHttpContext();
        var testClaims = new List<Claim>
        {
            new Claim("email", "test@email.com")
        };
        httpContext.User = new DummyClaimsPrincipal(testClaims);

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(h => h.HttpContext).Returns(httpContext);
        return httpContextAccessor;
    }

    public class DummyClaimsPrincipal : ClaimsPrincipal
    {
        public DummyClaimsPrincipal(IEnumerable<Claim> claims)
        {
            Claims = claims;
        }

        public override IEnumerable<Claim> Claims { get; }
    }

    /// <summary>
    /// This PublishObserver exposes a task with a lifetime of 15 seconds.
    /// If PostPublish is invoked before the end of the 15 seconds,
    /// then the task gets completed and returns the published item.
    /// If this method is not invoked within these 15 seconds, the task throws an exception.
    /// This task can be awaited in a Unit Test, to assert that a parallel Thread
    /// does the publishing.
    /// </summary>
    public class NotifyOnFirstPublishedEntryObserver : IPublishObserver
    {
        private readonly TaskCompletionSource<object> _tcs1;

        public NotifyOnFirstPublishedEntryObserver()
        {
            _tcs1 = new TaskCompletionSource<object>();
            GetPublishedEntity = _tcs1.Task;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(15));
                _tcs1.SetException(new Exception("No publish after 15 seconds."));
            });
        }

        public Task<object> GetPublishedEntity { get; }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
        {
            _tcs1.SetResult(context.Message); //completes the task "GetTrackedEntity"
            return Task.CompletedTask;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            return Task.CompletedTask;
        }

        public Task PublishFault<T>(PublishContext<T> context, Exception exception)
            where T : class
        {
            throw new NotImplementedException();
        }
    }
}
