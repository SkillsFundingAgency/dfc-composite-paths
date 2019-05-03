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
    public class PatchPathHttpTrigger
    {
        private readonly ILogger<PatchPathHttpTrigger> _logger;

        public PatchPathHttpTrigger(ILogger<PatchPathHttpTrigger> logger)
        {
            _logger = logger;
        }

        [FunctionName("Patch")]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Patch", Description = "Updates specific values without specifying the entire path record")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "paths/{path}")] HttpRequest req,
            string path)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            
            if (string.IsNullOrWhiteSpace(path))
            {
                return new BadRequestResult();
            }

            await Task.CompletedTask;

            return new NoContentResult();
        }
    }
}