using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Functions;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Tests.Extensions;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.Functions
{
    [TestFixture]
    public class PutPathHttpTriggerTests
    {
        private PutPathHttpTrigger _function;
        private Mock<ILogger<PutPathHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _requestHelper;
        private Mock<IPathService> _pathService;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<PutPathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();
            _pathService = new Mock<IPathService>();

            _function = new PutPathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object, _pathService.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestResult_When_PathIsInvalid(string path)
        {
            var updateToModel = new PathModel();

            var result = await _function.Run(CreateHttpRequest(updateToModel), path);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Produces_BadRequestObjectResult_When_NoPayloadDoesNotExist()
        {
            var path = "path1";
            PathModel pathModel = null;

            var result = await _function.Run(CreateHttpRequest(pathModel), path);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Produces_OKResult_When_PathIsValid()
        {
            var path = "path1";
            var savedPathModel = new PathModel();
            var updateToModel = new PathModel() { Path = "p1", Layout = Layout.SidebarRight };
            _pathService.Setup(x => x.Get(path)).ReturnsAsync(savedPathModel);

            var result = await _function.Run(CreateHttpRequest(updateToModel), path);

            Assert.IsInstanceOf<OkResult>(result);
        }

        [Test]
        public async Task Produces_UnprocessableEntityObjectResult_When_UpdateThrowsException()
        {
            var path = "path1";
            var savedPathModel = new PathModel();
            var updateToModel = new PathModel() { Path = "p1", Layout = Layout.SidebarRight };
            _pathService.Setup(x => x.Get(path)).ReturnsAsync(savedPathModel);
            _pathService.Setup(x => x.Update(It.IsAny<PathModel>())).Throws(new InvalidOperationException());

            var result = await _function.Run(CreateHttpRequest(updateToModel), path);

            Assert.IsInstanceOf<UnprocessableEntityObjectResult>(result);
        }

        private HttpRequest CreateHttpRequest(object model)
        {
            var context = new DefaultHttpContext();
            var result = new DefaultHttpRequest(context);

            if (model != null)
            {
                var json = JsonConvert.SerializeObject(model);
                result.Body = json.AsStream();
            }

            return result;
        }
    }
}
