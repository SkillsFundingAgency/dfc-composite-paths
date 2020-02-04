using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Functions;
using DFC.Composite.Paths.Models;
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
    public class GetPathHttpTriggerTests
    {
        private GetPathHttpTrigger _function;
        private Mock<ILogger<GetPathHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _requestHelper;
        private Mock<IPathService> _pathService;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<GetPathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();
            _pathService = new Mock<IPathService>();

            _function = new GetPathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object, _pathService.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestResult_When_PathIsInvalid(string path)
        {
            var result = await _function.Run(CreateHttpRequest(), path);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Produces_OkObjectResult_When_PathIsValid()
        {
            var path = "path1";
            var pathModel = new PathModel() { Path = path, TopNavigationText = "tnt1" };
            _pathService.Setup(x => x.Get(path)).ReturnsAsync(pathModel);

            var result = await _function.Run(CreateHttpRequest(), path);

            var typedActionResultResult = As<OkObjectResult>(result);
            var typedValue = typedActionResultResult.Value as PathModel;
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(pathModel.TopNavigationText, typedValue.TopNavigationText);
        }

        [Test]
        public async Task ProducesNoContentResult_When_PathDoesNotExist()
        {
            var path = "path1";
            PathModel pathModel = null;
            _pathService.Setup(x => x.Get(path)).ReturnsAsync(pathModel);

            var result = await _function.Run(CreateHttpRequest(), path);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        private HttpRequest CreateHttpRequest()
        {
            return null;
        }

        private T As<T>(IActionResult actionResult)
        {
            return (T)actionResult;
        }
    }
}
