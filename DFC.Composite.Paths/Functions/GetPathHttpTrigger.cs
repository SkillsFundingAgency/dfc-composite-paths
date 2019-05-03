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
    public class GetPathHttpTrigger
    {
        private readonly ILogger<GetPathHttpTrigger> _logger;

        public GetPathHttpTrigger(ILogger<GetPathHttpTrigger> logger)
        {
            _logger = logger;
        }

        [FunctionName("GetById")]
        [ProducesResponseType(typeof(Models.PathModel), (int)HttpStatusCode.OK)]
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
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            if (string.IsNullOrWhiteSpace(path))
            {
                return new BadRequestResult();
            }

            await Task.CompletedTask;

            return new OkObjectResult(new PathModel());
        }
    }
}
