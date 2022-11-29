using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HotChocolate.Extensions.Translation.Resources;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HotChocolate.Extensions.Translation.Tests.Resources
{
    public class DefaultTranslationObserverTests
    {
        [Fact]
        public async Task OnMissingResource_WithKeyProvided_ShouldLogWarning()
        {
            //Arrange testData
            var key = Guid.NewGuid().ToString();

            //Arrange dependencies
            var logger = new Mock<ILogger<TranslationObserver>>();

            var observer = new DefaultTranslationObserver(logger.Object);

            //Act
            await observer.OnMissingResource(key);

            //Assert
            var log = logger.Invocations.Single();
            log.Arguments.OfType<LogLevel>().Should().ContainSingle().Which.Should().Be(LogLevel.Warning);

        }
    }
}
