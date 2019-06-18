using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Extensions;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Functions
{
    public class PatchPathHttpTrigger
    {
        private readonly ILogger<PatchPathHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IPathService _pathService;

        public PatchPathHttpTrigger(
            ILogger<PatchPathHttpTrigger> logger,
            ILoggerHelper loggerHelper,
            IHttpRequestHelper httpRequestHelper,
            IPathService pathService)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _pathService = pathService;
        }

        [FunctionName("Patch")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.UnprocessableEntity, Description = "Unprocessable entity", ShowSchema = false)]
        [Display(Name = "Patch", Description = "Updates specific values without specifying the entire path record")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "paths/{path}")] HttpRequest req,
            string path)
        {
            _loggerHelper.LogMethodEnter(_logger);

            var correlationId = _httpRequestHelper.GetOrCreateDssCorrelationId(req);

            if (string.IsNullOrWhiteSpace(path))
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.UnableToLocatePathInQueryString);
                return new BadRequestResult();
            }

            JsonPatchDocument<PathModel> pathPatch = null;
            try
            {
                var requestBody = await req.ReadAsStringAsync();
                pathPatch = JsonConvert.DeserializeObject<JsonPatchDocument<PathModel>>(requestBody);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogException(_logger, correlationId, ex);
                return new BadRequestResult();
            }

            _loggerHelper.LogInformationMessage(_logger, correlationId, $"Attempting to get path {path}'");
            var currentPath = await _pathService.Get(path);
            if (currentPath == null)
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.PathDoesNotExist);
                return new NoContentResult();
            }

            try
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, $"Attempting to apply patch to {path}");
                pathPatch.ApplyTo(currentPath);
                var validationResults = currentPath.Validate(new ValidationContext(currentPath));

                if (validationResults.Any())
                {
                    _loggerHelper.LogInformationMessage(_logger, correlationId, Message.ValidationFailed);
                    return new BadRequestObjectResult(validationResults);
                }
            }
            catch (Exception ex)
            {
                _loggerHelper.LogException(_logger, correlationId, ex);
                return new BadRequestResult();
            }

            try
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, $"Attempting to update path {path}");
                await _pathService.Update(currentPath);
                _loggerHelper.LogMethodExit(_logger);
                return new OkObjectResult(currentPath);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogException(_logger, correlationId, ex);
                return new UnprocessableEntityObjectResult(ex);
            }

        }
    }
}