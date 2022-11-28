using System;
using System.Threading.Tasks;
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
            var logger = new Mock<ILogger<TranslationObserver>>(MockBehavior.Strict);
            logger
                .Setup(x => x.LogWarning("Missing translation resource: {0}", key));

            var observer = new DefaultTranslationObserver(logger.Object);

            //Act
            await observer.OnMissingResource(key);

            //Assert
            logger.VerifyAll();

        }
    }
}
