using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DFC.Composite.Paths.APIDefinition
{
    public static class APIDefinition
    {
        public const string APITitle = "Composite Paths";
        public const string APIDefinitionName = "API-Definition";
        public const string APIDefRoute = APITitle + "/" + APIDefinitionName;
        public const string APIDescription = "Basic details of a National Careers Service " + APITitle + " Resource";
        private const string ApiVersion = "1.0.0";

        [FunctionName(APIDefinitionName)]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var swaggerDocGenerator = new SwaggerDocumentGenerator();
            var swaggerResponse = swaggerDocGenerator.GenerateSwaggerDocument(req, APITitle, APIDescription, APIDefinitionName, ApiVersion, Assembly.GetExecutingAssembly());

            return new OkObjectResult(swaggerResponse);
        }
    }
}
