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

namespace DFC.Composite.Paths.Tests.Functions
{
    [TestFixture]
    public class DeletePathHttpTriggerTests
    {
        private DeletePathHttpTrigger _function;
        private Mock<ILogger<DeletePathHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _requestHelper;
        private Mock<IPathService> _pathService;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<DeletePathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();
            _pathService = new Mock<IPathService>();

            _function = new DeletePathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object, _pathService.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestResult_When_PathIsInvalid(string path)
        {
            var result = await _function.Run(CreateHttpRequest(), path);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase("path1")]
        public async Task Produces_OKContentResult_When_PathIsValid(string path)
        {
            var pathModel = new PathModel() { Path = path };
            _pathService.Setup(x => x.Get(path)).ReturnsAsync(pathModel);

            var result = await _function.Run(CreateHttpRequest(), path);

            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task Produces_NotFoundResult_When_PathDoesNotExist()
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
    }
}
