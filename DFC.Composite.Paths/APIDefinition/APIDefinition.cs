using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace DFC.Composite.Paths.APIDefinition
{
    [ExcludeFromCodeCoverage]
    public static class ApiDefinition
    {
        public const string ApiDefinitionName = "API-Definition";
        public const string ApiDefRoute = "paths/" + ApiDefinitionName;
        public const string ApiDescription = "To support the Digital First Careers Composite UI Path definitions.";

        public const string ApiVersion = "1.0.0";

        [FunctionName(ApiDefinitionName)]
        public static HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiDefRoute)]
            HttpRequest req,
            ILogger logger,
            [Inject] ISwaggerDocumentGenerator swaggerDocumentGenerator,
            [Inject] ILoggerHelper loggerHelper
        )
        {
            string ApiSuffix = Environment.GetEnvironmentVariable("ApiSuffix");
            string ApiTitle = "Composite Paths " + ApiSuffix;

            loggerHelper.LogMethodEnter(logger);

            var swagger = swaggerDocumentGenerator.GenerateSwaggerDocument(req, ApiTitle, ApiDescription, ApiDefinitionName, ApiVersion, Assembly.GetExecutingAssembly());

            loggerHelper.LogMethodExit(logger);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(swagger)
            };
        }
    }
}
