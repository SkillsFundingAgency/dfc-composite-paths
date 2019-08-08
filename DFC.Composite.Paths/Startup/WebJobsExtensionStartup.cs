using DFC.Common.Standard.Logging;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using DFC.Functions.DI.Standard;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

[assembly: WebJobsStartup(typeof(DFC.Composite.Paths.Startup.WebJobsExtensionStartup), "Web Jobs Extension Startup")]

namespace DFC.Composite.Paths.Startup
{
    public class WebJobsExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var cosmosConnectionString = new CosmosConnectionString(Environment.GetEnvironmentVariable(Cosmos.CosmosConnectionString));
            var cosmosDatabaseId = Environment.GetEnvironmentVariable(Cosmos.CosmosDatabaseId);
            var cosmosCollectionId = Environment.GetEnvironmentVariable(Cosmos.CosmosCollectionId);
            var cosmosPartitionKey = Environment.GetEnvironmentVariable(Cosmos.CosmosPartitionKey);

            builder.AddDependencyInjection();

            builder.Services.AddSingleton<IDocumentStorage>(x => new CosmosDocumentStorage(cosmosConnectionString, cosmosPartitionKey, cosmosDatabaseId, cosmosCollectionId));
            builder.Services.AddTransient<IHttpRequestHelper, HttpRequestHelper>();
            builder.Services.AddTransient<ILoggerHelper, LoggerHelper>();
            builder.Services.AddScoped<IPathService, PathService>(sp => new PathService(sp.GetService<IDocumentStorage>()));
            builder.Services.AddTransient<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
        }
    }
}
