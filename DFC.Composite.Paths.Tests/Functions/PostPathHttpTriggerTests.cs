using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Functions;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Tests.Extensions;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.Functions
{
    [TestFixture]
    public class PostPathHttpTriggerTests
    {
        private PostPathHttpTrigger _function;
        private Mock<ILogger<PostPathHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _requestHelper;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<PostPathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();

            _function = new PostPathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestObjectResult_When_PathIsInvalid(string path)
        {
            var pathModel = new PathModel();
            pathModel.Path = path;
            pathModel.Layout = Layout.SidebarLeft;

            var result = await _function.Run(CreateHttpRequest(pathModel));

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [TestCase(null)]
        public async Task Produces_BadRequestObjectResult_When_LayoutIsInvalid(Layout layout)
        {
            var pathModel = new PathModel();
            pathModel.Path = "path1";
            pathModel.Layout = layout;

            var result = await _function.Run(CreateHttpRequest(pathModel));

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Produces_NoContentResult_When_Valid()
        {
            var pathModel = new PathModel();
            pathModel.Path = "p1";
            pathModel.Layout = Layout.SidebarLeft;

            var result = await _function.Run(CreateHttpRequest(pathModel));

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        private HttpRequest CreateHttpRequest(PathModel model)
        {
            var context = new DefaultHttpContext();
            var result = new DefaultHttpRequest(context);

            var json = JsonConvert.SerializeObject(model);
            result.Body = json.AsStream();

            return result;
        }


    }
}
