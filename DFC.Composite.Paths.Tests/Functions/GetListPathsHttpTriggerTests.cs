using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Functions;
using DFC.Composite.Paths.Services;
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
        private Mock<IPathService> _pathService;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<GetListPathsHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _pathService = new Mock<IPathService>();

            _function = new GetListPathsHttpTrigger(_logger.Object, _loggerHelper.Object, _pathService.Object);
        }

        [Test]
        public async Task Produces_NoContentResult_When_Invoked()
        {
            var result = await _function.Run(CreateHttpRequest());

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        private HttpRequest CreateHttpRequest()
        {
            return null;
        }
    }
}
