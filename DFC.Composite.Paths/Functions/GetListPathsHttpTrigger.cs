using DFC.Common.Standard.Logging;
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
    public class GetListPathsHttpTrigger
    {
        private readonly ILogger<GetListPathsHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;

        public GetListPathsHttpTrigger(ILogger<GetListPathsHttpTrigger> logger, ILoggerHelper loggerHelper)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
        }

        [FunctionName("Get")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Retrieves a list off all registered applications paths.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paths")] HttpRequest req)
        {
            _loggerHelper.LogMethodEnter(_logger);

            await Task.CompletedTask;

            _loggerHelper.LogMethodExit(_logger);

            return new OkResult();
        }
    }
}
