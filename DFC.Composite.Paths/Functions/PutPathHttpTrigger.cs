using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Extensions;
using DFC.Composite.Paths.Models;
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
    public class PutPathHttpTrigger
    {
        private readonly ILogger<PutPathHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;

        public PutPathHttpTrigger(ILogger<PutPathHttpTrigger> logger, ILoggerHelper loggerHelper, IHttpRequestHelper httpRequestHelper)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
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

            IActionResult result = null;
            var correlationId = _httpRequestHelper.GetOrCreateDssCorrelationId(req);

            if (string.IsNullOrEmpty(path))
            {
                return new BadRequestResult();
            }

            var body = await req.GetBodyAsync<PathModel>();
            if (body.IsValid)
            {
                result = new OkObjectResult(body);
            }
            else
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.ValidationFailed);
                result = new BadRequestObjectResult(body.ValidationResults);
            }

            _loggerHelper.LogMethodExit(_logger);

            return result;
        }
    }
}
