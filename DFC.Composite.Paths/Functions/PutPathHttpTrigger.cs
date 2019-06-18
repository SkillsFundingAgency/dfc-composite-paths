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
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Functions
{
    public class PutPathHttpTrigger
    {
        private readonly ILogger<PutPathHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IPathService _pathService;

        public PutPathHttpTrigger(
            ILogger<PutPathHttpTrigger> logger,
            ILoggerHelper loggerHelper,
            IHttpRequestHelper httpRequestHelper,
            IPathService pathService)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _pathService = pathService;
        }

        [FunctionName("Put")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Put", Description = "Overwrites an entire record.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "paths/{path}")] HttpRequest req,
            string path)
        {
            _loggerHelper.LogMethodEnter(_logger);

            var correlationId = _httpRequestHelper.GetOrCreateDssCorrelationId(req);

            if (string.IsNullOrEmpty(path))
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.UnableToLocatePathInQueryString);
                return new BadRequestResult();
            }

            var body = await req.GetBodyAsync<PathModel>();
            if (body == null || body.Value == null)
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.PayloadMalformed);
                return new BadRequestResult();
            }

            if (!body.IsValid)
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.ValidationFailed);
                return new BadRequestObjectResult(body.ValidationResults);
            }

            _loggerHelper.LogInformationMessage(_logger, correlationId, $"Attempting to get path for {path}");
            var currentPath = await _pathService.Get(path);
            if (currentPath == null)
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.PathDoesNotExist);
                return new NoContentResult();
            }

            try
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, $"Attempting to get update path for {path}");
                await _pathService.Update(body.Value);
                _loggerHelper.LogMethodExit(_logger);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _loggerHelper.LogException(_logger, correlationId, ex);
                return new UnprocessableEntityObjectResult(ex);
            }
        }
    }
}
