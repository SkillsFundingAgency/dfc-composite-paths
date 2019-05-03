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
    public class PutPathHttpTriggerTests
    {
        private PutPathHttpTrigger _function;
        private Mock<ILogger<PutPathHttpTrigger>> _logger;
        private Mock<ILoggerHelper> _loggerHelper;
        private Mock<IHttpRequestHelper> _requestHelper;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<PutPathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();

            _function = new PutPathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestResult_When_PathIsInvalid(string path)
        {
            var model = new PathModel();

            var result = await _function.Run(CreateHttpRequest(model), path);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        public async Task Produces_NoContentResult_When_PathIsValid(string path)
        {
            var model = new PathModel() { Path = "p1", Layout = Layout.SidebarRight };

            var result = await _function.Run(CreateHttpRequest(model), path);

            Assert.IsInstanceOf<NoContentResult>(result);
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
