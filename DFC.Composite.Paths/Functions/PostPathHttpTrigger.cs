using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Extensions;
using DFC.Composite.Paths.Models;
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
    public class PostPathHttpTrigger
    {
        private readonly ILogger<PostPathHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;

        public PostPathHttpTrigger(ILogger<PostPathHttpTrigger> logger, ILoggerHelper loggerHelper)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
        }

        [FunctionName("Post")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PathModel))]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Post", Description = "Creates a new resource of type 'Paths'.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paths")] HttpRequest req)
        {
            _loggerHelper.LogMethodEnter(_logger);

            IActionResult result = null;

            var body = await req.GetBodyAsync<PathModel>();
            if (body.IsValid)
            {
                result = new OkObjectResult(body);
            }
            else
            {
                result = new BadRequestObjectResult(body.ValidationResults);
            }

            _loggerHelper.LogMethodExit(_logger);

            return result;
        }
    }
}
