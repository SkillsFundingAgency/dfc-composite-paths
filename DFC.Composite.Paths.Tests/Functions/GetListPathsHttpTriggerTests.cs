using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.Functions
{
    [TestFixture]
    public class GetListPathsHttpTriggerTests
    {
        private GetListPathsHttpTrigger _function;
        private Mock<ILogger<GetListPathsHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;

        [SetUp]
        public void CanDoIt()
        {
            _logger = new Mock<ILogger<GetListPathsHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();

            _function = new GetListPathsHttpTrigger(_logger.Object, _loggerHelper.Object);
        }
        
        [Test]
        public async Task Produces_NoContentResult_When_Invoked()
        {
            var result = await _function.Run(CreateHttpRequest());

            Assert.IsInstanceOf<OkResult>(result);
        }

        private HttpRequest CreateHttpRequest()
        {
            return null;
        }
    }
}
