using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using DFC.Swagger.Standard.Annotations;
using System.Net;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Extensions;

namespace DFC.Composite.Paths.Functions
{
    public static class RegisterPath
    {
        [FunctionName(nameof(RegisterPath))]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Path found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Path does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = nameof(RegisterPath), Description = "Creates a new resource of type 'Paths'.")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paths")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var body = await req.GetBodyAsync<PathModel>();
            if (body.IsValid)
            {
                return new OkObjectResult(body);
            }
            else
            {
                return new BadRequestObjectResult(body.ValidationResults);
            }
        }
    }
}
