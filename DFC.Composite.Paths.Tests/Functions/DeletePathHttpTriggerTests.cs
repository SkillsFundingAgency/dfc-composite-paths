using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Functions;
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

        [SetUp]
        public void CanDoIt()
        {
            _logger = new Mock<ILogger<DeletePathHttpTrigger>>();
            _loggerHelper = new Mock<ILoggerHelper>();
            _requestHelper = new Mock<IHttpRequestHelper>();

            _function = new DeletePathHttpTrigger(_logger.Object, _loggerHelper.Object, _requestHelper.Object);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Produces_BadRequestResult_When_PathIsInvalid(string path)
        {
            var result = await _function.Run(CreateHttpRequest(), path);

            Assert.IsInstanceOf<BadRequestResult>(result);
        }

        [TestCase("path1")]
        public async Task Produces_NoContentResult_When_PathIsValid(string path)
        {
            var result = await _function.Run(CreateHttpRequest(), path);

            Assert.IsInstanceOf<NoContentResult>(result);
        }

        private HttpRequest CreateHttpRequest()
        {
            return null;
        }
    }
}
