using DFC.Common.Standard.Logging;
using DFC.Swagger.Standard;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DFC.Composite.Paths.APIDefinition
{
    public class APIDefinition
    {
        private const string ApiTitle = "Composite Paths";
        private const string ApiDefinitionName = "API-Definition";
        private const string ApiDefinitionRoute = ApiTitle + "/" + ApiDefinitionName;
        private const string ApiDefinitionDescription = "Basic details of a National Careers Service " + ApiTitle + " Resource";
        private const string ApiVersion = "0.1.0";

        private ISwaggerDocumentGenerator _swaggerDocumentGenerator;
        private readonly ILogger<APIDefinition> _logger;
        private ILoggerHelper _loggerHelper;

        public APIDefinition(ISwaggerDocumentGenerator swaggerDocumentGenerator, ILogger<APIDefinition> logger, ILoggerHelper loggerHelper)
        {
            _swaggerDocumentGenerator = swaggerDocumentGenerator;
            _logger = logger;
            _loggerHelper = loggerHelper;
        }

        [FunctionName(ApiDefinitionName)]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ApiDefinitionRoute)] HttpRequest req)
        {
            _loggerHelper.LogMethodEnter(_logger);

            var swaggerResponse = _swaggerDocumentGenerator.GenerateSwaggerDocument(
                req,
                ApiTitle,
                ApiDefinitionDescription,
                ApiDefinitionName,
                ApiVersion,
                Assembly.GetExecutingAssembly());

            _loggerHelper.LogMethodExit(_logger);

            return new OkObjectResult(swaggerResponse);
        }
    }
}
