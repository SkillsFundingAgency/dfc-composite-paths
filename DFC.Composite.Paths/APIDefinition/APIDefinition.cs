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
        private const string ApiTitle = "Composite Paths";
        private const string ApiDefinitionName = "API-Definition";
        private const string ApiDefinitionRoute = ApiTitle + "/" + ApiDefinitionName;
        private const string ApiDefinitionDescription = "Basic details of a National Careers Service " + ApiTitle + " Resource";
        private const string ApiVersion = "0.1.0";

        [FunctionName(ApiDefinitionName)]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiDefinitionRoute)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var swaggerDocGenerator = new SwaggerDocumentGenerator();
            var swaggerResponse = swaggerDocGenerator.GenerateSwaggerDocument(req, ApiTitle, ApiDefinitionDescription, ApiDefinitionName, ApiVersion, Assembly.GetExecutingAssembly());

            return new OkObjectResult(swaggerResponse);
        }
    }
}
