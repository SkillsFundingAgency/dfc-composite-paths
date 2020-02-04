using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Functions;
using DFC.Composite.Paths.Services;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.UnitTests.Functions
{
    [TestFixture]
    public class GetListPathsHttpTriggerTests
    {
        private GetListPathsHttpTrigger _function;
        private Mock<ILogger<GetListPathsHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _httpRequestHelper;
        private Mock<IPathService> _pathService;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<GetListPathsHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _httpRequestHelper = new Mock<IHttpRequestHelper>();
            _pathService = new Mock<IPathService>();

            _function = new GetListPathsHttpTrigger(_logger.Object, _loggerHelper.Object, _httpRequestHelper.Object, _pathService.Object);
        }

        [Test]
        public async Task Produces_OkObjectResult_When_Invoked()
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
