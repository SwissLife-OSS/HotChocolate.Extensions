using System;
using FluentAssertions;
using HotChocolate.Extensions.Tracking.Persistence;
using Xunit;

namespace HotChocolate.Extensions.Tracking.Tests.Persistence
{
    public class ExporterCandidateForAllTests
    {
        [Fact]
        public void CanHandle_ForRandomType_ShouldReturnTrue()
        {
            Type type = typeof(string);

            var candidate = new ExporterCandidateForAll(exporter: default!);

            //Act
            bool canHandle = candidate.CanHandle(type);

            //Assert
            canHandle.Should().BeTrue();
        }
    }
}
