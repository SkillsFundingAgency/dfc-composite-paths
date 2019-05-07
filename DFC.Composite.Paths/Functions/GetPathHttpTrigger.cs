using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Extensions;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Functions
{
    public class GetPathHttpTrigger
    {
        private readonly ILogger<GetPathHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IPathService _pathService;

        public GetPathHttpTrigger(
            ILogger<GetPathHttpTrigger> logger,
            ILoggerHelper loggerHelper,
            IHttpRequestHelper httpRequestHelper,
            IPathService pathService)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _pathService = pathService;
        }

        [FunctionName("GetById")]
        [ProducesResponseType(typeof(PathModel), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Provides details of a single path.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paths/{path}")] HttpRequest req,
            string path)
        {
            _loggerHelper.LogMethodEnter(_logger);

            var correlationId = _httpRequestHelper.GetOrCreateDssCorrelationId(req);

            if (string.IsNullOrWhiteSpace(path))
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.UnableToLocatePathInQueryString);
                return new BadRequestResult();
            }

            var pathModel = await _pathService.Get(path);

            _loggerHelper.LogMethodExit(_logger);

            return new OkObjectResult(pathModel);
        }
    }
}
