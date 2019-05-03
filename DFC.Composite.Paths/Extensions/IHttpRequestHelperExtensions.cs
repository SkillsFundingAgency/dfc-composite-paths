using DFC.HTTP.Standard;
using Microsoft.AspNetCore.Http;
using System;

namespace DFC.Composite.Paths.Extensions
{
    public static class IHttpRequestHelperExtensions
    {
        public static Guid GetOrCreateDssCorrelationId(this IHttpRequestHelper httpRequestHelper, HttpRequest req)
        {
            var correlationId = httpRequestHelper.GetDssCorrelationId(req);
            var result = Guid.NewGuid();

            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                result = Guid.Parse(correlationId);
            }

            return result;
        }
    }
}
