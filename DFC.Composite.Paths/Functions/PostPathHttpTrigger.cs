using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Extensions;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Functions
{
    public class PostPathHttpTrigger
    {
        private readonly ILogger<PostPathHttpTrigger> _logger;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IHttpRequestHelper _httpRequestHelper;
        private readonly IPathService _pathService;

        public PostPathHttpTrigger(
            ILogger<PostPathHttpTrigger> logger,
            ILoggerHelper loggerHelper,
            IHttpRequestHelper httpRequestHelper,
            IPathService pathService)
        {
            _logger = logger;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _pathService = pathService;
        }

        [FunctionName("Post")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(PathModel))]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Path created", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.UnprocessableEntity, Description = "Unprocessable entity", ShowSchema = false)]
        [Display(Name = "Post", Description = "Creates a new resource of type 'Paths'.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "paths")] HttpRequest req)
        {
            _loggerHelper.LogMethodEnter(_logger);

            var correlationId = _httpRequestHelper.GetOrCreateDssCorrelationId(req);

            var body = await req.GetBodyAsync<PathModel>();

            if (body == null || body.Value == null)
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.PayloadMalformed);
                return new BadRequestResult();
            }

            if (!body.IsValid)
            {
                _loggerHelper.LogInformationMessage(_logger, correlationId, Message.ValidationFailed);
                return new BadRequestObjectResult(body.ValidationResults);
            }

            if (!string.IsNullOrEmpty(body.Value.OfflineHtml))
            {
                var htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(body.Value.OfflineHtml);

                if (htmlDoc.ParseErrors.Any())
                {
                    _loggerHelper.LogInformationMessage(_logger, correlationId, $"Request value for '{nameof(body.Value.OfflineHtml)}' contains malformed HTML");
                    return new BadRequestResult();
                }
            }

            if (!string.IsNullOrEmpty(body.Value.PhaseBannerHtml))
            {
                var htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(body.Value.PhaseBannerHtml);

                if (htmlDoc.ParseErrors.Any())
                {
                    _loggerHelper.LogInformationMessage(_logger, correlationId, $"Request value for '{nameof(body.Value.PhaseBannerHtml)}' contains malformed HTML");
                    return new BadRequestResult();
                }
            }

            try
            {
                var registeredPath = await _pathService.Register(body.Value);
                _loggerHelper.LogMethodExit(_logger);
                return new CreatedResult(registeredPath.Path, registeredPath);
            }
            catch (Exception ex)
            {
                _loggerHelper.LogException(_logger, correlationId, ex);
                return new UnprocessableEntityObjectResult(ex);
            }
        }
    }
}
