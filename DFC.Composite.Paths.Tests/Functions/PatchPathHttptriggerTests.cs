using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Functions;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Tests.Extensions;
using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.Functions
{
    [TestFixture]
    public class PatchPathHttpTriggerTests
    {
        private PatchPathHttpTrigger _function;
        private Mock<ILogger<PatchPathHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _requestHelper;
        private Mock<IPathService> _pathService;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<PatchPathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();
            _pathService = new Mock<IPathService>();

            _function = new PatchPathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object, _pathService.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestResult_When_PathIsInvalid(string path)
        {
            var model = new PathModel() { Path = path };
            var result = await _function.Run(CreateHttpRequest(model), path);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [Test]
        public async Task Produces_OKContentResult_When_Patching()
        {
            var path = "path1";
            var oldLayout = Layout.FullWidth;
            var newLayout = Layout.SidebarLeft;
            var savedPath = new PathModel() { Path = path, Layout = oldLayout };
            var patch = new JsonPatchDocument<PathModel>();
            patch.Add(x => x.Layout, newLayout);
            _pathService.Setup(x => x.Get(path)).ReturnsAsync(savedPath);

            var result = await _function.Run(CreateHttpRequest(patch), path);

            var typedActionResultResult = As<OkObjectResult>(result);
            var typedValue = typedActionResultResult.Value as PathModel;
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(newLayout, typedValue.Layout);
        }

        private HttpRequest CreateHttpRequest(object model)
        {
            var context = new DefaultHttpContext();
            var result = new DefaultHttpRequest(context);

            var json = JsonConvert.SerializeObject(model);
            result.Body = json.AsStream();

            return result;
        }

        private T As<T>(IActionResult actionResult)
        {
            return (T)actionResult;
        }
    }
}
