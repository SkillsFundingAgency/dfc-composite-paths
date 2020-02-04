using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Functions;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.UnitTests.Extensions;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.IntegrationTests.Functions
{
    [TestFixture]
    public class PostPathHttpTriggerTests
    {
        private PostPathHttpTrigger _function;
        private Mock<ILogger<PostPathHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _requestHelper;
        private Mock<IPathService> _pathService;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<PostPathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();
            _pathService = new Mock<IPathService>();

            _function = new PostPathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object, _pathService.Object);
        }

        [Test]
        public async Task Produces_BadRequestObjectResult_When_NoPayloadDoesNotExist()
        {
            PathModel pathModel = null;

            var result = await _function.Run(CreateHttpRequest(pathModel));

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestObjectResult_When_PathIsMissing(string path)
        {
            var newPathModel = new PathModel();
            newPathModel.Path = path;
            newPathModel.Layout = Layout.SidebarLeft;

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var typedResult = result as BadRequestObjectResult;
            var validationResult = typedResult.Value as List<ValidationResult>;
            Assert.Contains(string.Format(Message.FieldIsRequired, nameof(PathModel.Path)), validationResult.Select(x => x.ErrorMessage).ToList());
        }

        [TestCase("$path")]
        [TestCase("&path")]
        public async Task Produces_BadRequestObjectResult_When_PathIsInvalid(string path)
        {
            var newPathModel = new PathModel();
            newPathModel.Path = path;
            newPathModel.Layout = Layout.SidebarLeft;

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var typedResult = result as BadRequestObjectResult;
            var validationResult = typedResult.Value as List<ValidationResult>;
            Assert.Contains(Message.PathIsInvalid, validationResult.Select(x => x.ErrorMessage).ToList());
        }

        [TestCase("<div></span>")]
        [TestCase("<strong></div>")]
        public async Task Produces_BadRequestObjectResult_When_OfflineHtmlIsInvalid(string offlineHtml)
        {
            var newPathModel = new PathModel();
            newPathModel.OfflineHtml = offlineHtml;
            newPathModel.Path = "path1";
            newPathModel.Layout = Layout.SidebarLeft;

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var typedResult = result as BadRequestObjectResult;
            var validationResult = typedResult.Value as List<ValidationResult>;
            Assert.Contains(string.Format(Message.MalformedHtml, nameof(PathModel.OfflineHtml)), validationResult.Select(x => x.ErrorMessage).ToList());
        }

        [TestCase("<div></span>")]
        [TestCase("<strong></div>")]
        public async Task Produces_BadRequestObjectResult_When_PhaseBannerHtmlIsInvalid(string phaseBannerHtml)
        {
            var newPathModel = new PathModel();
            newPathModel.PhaseBannerHtml = phaseBannerHtml;
            newPathModel.Path = "path1";
            newPathModel.Layout = Layout.SidebarLeft;

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<BadRequestObjectResult>(result);

            var typedResult = result as BadRequestObjectResult;
            var validationResult = typedResult.Value as List<ValidationResult>;
            Assert.Contains(string.Format(Message.MalformedHtml, nameof(PathModel.PhaseBannerHtml)), validationResult.Select(x => x.ErrorMessage).ToList());
        }

        [Test]
        public async Task Produces_CreatedResult_When_Valid()
        {
            var newPathModel = new PathModel();
            newPathModel.Path = "p1";
            newPathModel.Layout = Layout.SidebarLeft;
            _pathService.Setup(x => x.Register(It.IsAny<PathModel>())).ReturnsAsync(newPathModel);

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<CreatedResult>(result);
        }

        [Test]
        public async Task Produces_UnprocessableEntityObjectResult_When_PathIsAlreadyRegistered()
        {
            var newPathModel = new PathModel();
            newPathModel.Path = "p1";
            newPathModel.Layout = Layout.SidebarLeft;
            _pathService.Setup(x => x.Register(It.IsAny<PathModel>())).ThrowsAsync(new InvalidOperationException());

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<UnprocessableEntityObjectResult>(result);
        }

        [TestCase(Layout.FullWidth)]
        [TestCase(Layout.SidebarLeft)]
        [TestCase(Layout.SidebarRight)]
        public async Task Produces_UnprocessableEntityObjectResult_When_RegisteringLayoutWithExternalUrl(Layout layout)
        {
            var newPathModel = new PathModel();
            newPathModel.Path = "p1";
            newPathModel.Layout = layout;
            newPathModel.ExternalURL = "http://www.google.com";
            _pathService.Setup(x => x.Register(It.IsAny<PathModel>())).ThrowsAsync(new InvalidOperationException());

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<UnprocessableEntityObjectResult>(result);
        }

        [Test]
        public async Task Produces_UnprocessableEntityObjectResult_When_RegisteringLayoutNoneWithNoneExternalUrl()
        {
            var newPathModel = new PathModel();
            newPathModel.Path = "p1";
            newPathModel.Layout = Layout.None;
            _pathService.Setup(x => x.Register(It.IsAny<PathModel>())).ThrowsAsync(new InvalidOperationException());

            var result = await _function.Run(CreateHttpRequest(newPathModel));

            Assert.IsInstanceOf<UnprocessableEntityObjectResult>(result);
        }

        private HttpRequest CreateHttpRequest(PathModel model)
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
