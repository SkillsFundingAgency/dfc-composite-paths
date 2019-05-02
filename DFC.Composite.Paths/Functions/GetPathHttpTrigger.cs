using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using DFC.Swagger.Standard.Annotations;
using System.Net;
using System.ComponentModel.DataAnnotations;
using DFC.Composite.Paths.Models;

namespace DFC.Composite.Paths.Functions
{
    public static class GetPathHttpTrigger
    {
        [FunctionName("GetById")]
        [ProducesResponseType(typeof(Models.PathModel), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Provides details of a single path.")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "paths/{path}")] HttpRequest req,
            string path,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            if (string.IsNullOrWhiteSpace(path))
            {
                return new BadRequestResult();
            }

            await Task.CompletedTask;

            return new OkObjectResult(new PathModel());
        }
    }
}
