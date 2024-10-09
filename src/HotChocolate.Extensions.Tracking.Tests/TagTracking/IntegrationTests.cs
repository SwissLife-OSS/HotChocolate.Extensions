using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Extensions.Tracking.TagTracking;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace HotChocolate.Extensions.Tracking.Tests.TagTracking;

public class IntegrationTests
{
    [Fact]
    public async Task Track_TagOnQuery_ShouldAddATrackingEntryForTrackAndDate()
    {
        //Arrange
        string query = "{ foo }";
        Mock<IHttpContextAccessor> mockHttpContextAccessor = ArrangeHttpContextAccessor();

        var services = new ServiceCollection();
        services.AddSingleton(mockHttpContextAccessor.Object);
        services.AddLogging();

        ISchema schema = await services
            .AddGraphQLServer()
                .AddTrackingPipeline(builder => builder
                    .AddExporter<NotifyOnFirstEntryExporter>())
                .AddQueryType<Query>()
                .BuildSchemaAsync();

        IServiceProvider serviceProvider = schema.Services!;

        IHostedService trackingService = await StartTrackingBackgroundService(serviceProvider);

        IRequestExecutor executor =
               await serviceProvider.GetRequiredService<IRequestExecutorResolver>()
                   .GetRequestExecutorAsync();
        try
        {
            //Act
            IExecutionResult res = await executor.ExecuteAsync(query);

            //Assert
            res.ToMinifiedJson()
                .Should().Be("{\"data\":{\"foo\":\"bar\"}}");
            ITrackingEntry trackedEntity
                = await serviceProvider.GetRequiredService<NotifyOnFirstEntryExporter>().GetTrackedEntity;

            TagTrackingEntry trackedEntry = trackedEntity.Should().BeOfType<TagTrackingEntry>().Subject;

            trackedEntry.Tag.Should().Be("FooInvoked");
            trackedEntry.Date.Should().BeCloseTo(DateTimeOffset.UtcNow, precision: TimeSpan.FromSeconds(30));
        }
        finally
        {
            await trackingService.StopAsync(CancellationToken.None);
        }
    }

    public class Query
    {
        [Track("FooInvoked")]
        public string Foo => "bar";
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

        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        httpContextAccessor.SetupGet(h => h.HttpContext).Returns(httpContext);

        return httpContextAccessor;
    }

    /// <summary>
    /// This exporter exposes a task with a lifetime of 15 seconds.
    /// If SaveTrackingEntryAsync is invoked before the end of the 15 seconds,
    /// then the task gets completed and returns the trackingEntry parameter
    /// of the method SaveTrackingEntryAsync.
    /// If this method is not invoked within these 15 seconds, the task throws an exception.
    /// This task can be awaited in a Unit Test, to assert that a parallel Thread
    /// calls the method SaveTrackingEntryAsync.
    /// </summary>
    public class NotifyOnFirstEntryExporter : ITrackingExporter
    {
        private readonly TaskCompletionSource<ITrackingEntry> _tcs1;

        public NotifyOnFirstEntryExporter()
        {
            _tcs1 = new TaskCompletionSource<ITrackingEntry>();
            GetTrackedEntity = _tcs1.Task;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                if (!_tcs1.Task.IsCompleted)
                {
                    _tcs1.SetException(new Exception("No tracking entry after 5 seconds."));
                }
            });
        }

        public Task<ITrackingEntry> GetTrackedEntity { get; }

        public Task SaveTrackingEntryAsync(
            ITrackingEntry trackingEntry, CancellationToken cancellationToken)
        {
            _tcs1.SetResult(trackingEntry); //completes the task "GetTrackedEntity"
            return Task.CompletedTask;
        }
    }
}
