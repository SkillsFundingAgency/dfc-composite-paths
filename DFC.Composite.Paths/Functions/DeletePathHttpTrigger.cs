using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Extensions;
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
    public class DeletePathHttpTrigger
    {
        private readonly ILogger<DeletePathHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IPathService _pathService;

        public DeletePathHttpTrigger(
            ILogger<DeletePathHttpTrigger> logger, 
            ILoggerHelper loggerHelper, 
            IHttpRequestHelper httpRequestHelper,
            IPathService pathService)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _pathService = pathService;
        }

        [FunctionName("Delete")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Delete", Description = "Deletes a single path record.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "paths/{path}")] HttpRequest req,
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
            if (pathModel == null)
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.PathNotFound);
                return new NotFoundResult();
            }

            await _pathService.Delete(path);

            _loggerHelper.LogMethodExit(_logger);

            return new NoContentResult();
        }
    }
}
