using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Extensions.Tracking.Default;
using HotChocolate.Extensions.Tracking.Persistence;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Snapshooter.Xunit;
using Xunit;

namespace HotChocolate.Extensions.Tracking.Tests;

public class IntegrationTests
{
    [InlineData("{ foo @track }")]
    [InlineData("{ foo @track(if:true) }")]
    [Theory]
    public async Task Trackable_IntegrationTest_ShouldAddATrackingEntry(string query)
    {
        //Arrange

        /* repository where the KPIs will be stored. In real life this could
            be for instance a Mongo repo or a masstransit repo */
        Mock<IHttpContextAccessor> mockHttpContextAccessor = ArrangeHttpContextAccessor();

        IServiceProvider services = new ServiceCollection()
            .AddGraphQL()
                .AddQueryType(c =>
                    c.Name("Query")
                        .Field("foo")
                        .Type<StringType>()
                        .Resolve("bar")
                        .Trackable("tracked"))
                .AddTrackingPipeline(b => b.AddRepository<NotifyOnFirstEntryRepository>())
            .Services
            .AddSingleton(mockHttpContextAccessor.Object)
            .BuildServiceProvider();

        IHostedService trackingService = await StartTrackingBackgroundService(services);

        try
        {
            //Act
            IRequestExecutor executor =
                await services.GetRequiredService<IRequestExecutorResolver>()
                    .GetRequestExecutorAsync();
            IExecutionResult res = await executor.ExecuteAsync(query);

            //Assert
            res.ToMinifiedJson()
                .Should().Be("{\"data\":{\"foo\":\"bar\"}}");
            ITrackingEntry trackedEntity
                = await services.GetRequiredService<NotifyOnFirstEntryRepository>().GetTrackedEntity;
            TrackingEntry trackedEntry = trackedEntity.Should()
                .BeOfType<TrackingEntry>().Subject;
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

    [InlineData("{ foo }")]
    [InlineData("{ foo @track(if:false) }")]
    [Theory]
    public async Task Trackable_IntegrationTestWithoutTag_ShouldNotAddATrackingEntry(string query)
    {
        //Arrange

        /* This repository will throw an exception if a tracking entry is added */
        var kpiRepo = new NeverCallThisRepository();

        Mock<IHttpContextAccessor> mockHttpContextAccessor = ArrangeHttpContextAccessor();

        IServiceProvider services = new ServiceCollection()
            .AddGraphQL()
                .AddQueryType(c =>
                    c.Name("Query")
                        .Field("foo")
                        .Type<StringType>()
                        .Resolve("bar")
                        .Track("tracked"))
                .AddTrackingPipeline(b => b.AddExporter<NeverCallThisRepository>())
            .Services
            .AddSingleton(mockHttpContextAccessor.Object)
            .BuildServiceProvider();

        IHostedService trackingService = await StartTrackingBackgroundService(services);

        try
        {
            //Act
            IRequestExecutor executor =
                await services.GetRequiredService<IRequestExecutorResolver>()
                    .GetRequestExecutorAsync();
            IExecutionResult res = await executor.ExecuteAsync(query);

            //Assert
            res.ToMinifiedJson()
                .Should().Be("{\"data\":{\"foo\":\"bar\"}}");

            /* not very elegant, but its the easiest way to make sure
               nothing was sent to the threadchannel */
            Thread.Sleep(TimeSpan.FromSeconds(5));
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
    /// This repository exposes a task with a lifetime of 15 seconds.
    /// If SaveTrackingEntryAsync is invoked before the end of the 15 seconds,
    /// then the task gets completed and returns the trackingEntry parameter
    /// of the method SaveTrackingEntryAsync.
    /// If this method is not invoked within these 15 seconds, the task throws an exception.
    /// This task can be awaited in a Unit Test, to assert that a parallel Thread
    /// calls the method SaveTrackingEntryAsync.
    /// </summary>
    public class NotifyOnFirstEntryRepository : ITrackingExporter
    {
        private readonly TaskCompletionSource<ITrackingEntry> _tcs1;

        public NotifyOnFirstEntryRepository()
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

    /// <summary>
    /// This repository will throw an exception if a tracking entry is added
    /// </summary>
    public class NeverCallThisRepository : ITrackingExporter
    {
        public Task SaveTrackingEntryAsync(ITrackingEntry trackingEntry, CancellationToken cancellationToken)
        {
            throw new Exception("You came to the wrong place.");
        }
    }
}
