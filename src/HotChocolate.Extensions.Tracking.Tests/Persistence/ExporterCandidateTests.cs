using System;
using FluentAssertions;
using HotChocolate.Extensions.Tracking.Persistence;
using Xunit;

namespace HotChocolate.Extensions.Tracking.Tests.Persistence
{
    public class ExporterCandidateTests
    {
        [Fact]
        public void CanHandle_ForSupportedType_ShouldReturnTrue()
        {
            Type type = typeof(string);

            var candidate = new ExporterCandidate(exporter: default!, new[] { type });

            //Act
            bool canHandle = candidate.CanHandle(type);

            //Assert
            canHandle.Should().BeTrue();
        }

        [Fact]
        public void CanHandle_ForUnsupportedType_ShouldReturnTrue()
        {
            Type supportedType = typeof(string);
            Type requestedType = typeof(int);

            var candidate = new ExporterCandidate(exporter: default!, new[] { supportedType });

            //Act
            bool canHandle = candidate.CanHandle(requestedType);

            //Assert
            canHandle.Should().BeFalse();
        }
    }
}
