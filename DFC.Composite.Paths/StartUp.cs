using DFC.Common.Standard.Logging;
using DFC.Composite.Paths;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using DFC.HTTP.Standard;
using DFC.Swagger.Standard;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

[assembly: WebJobsStartup(typeof(StartUp))]
namespace DFC.Composite.Paths
{
    public class StartUp : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var config = CreateConfiguration(builder);
            RegisterServices(builder.Services, config);
        }

        private IConfiguration CreateConfiguration(IWebJobsBuilder builder)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var descriptor = builder.Services.FirstOrDefault(d => d.ServiceType == typeof(IConfiguration));
            if (descriptor?.ImplementationInstance is IConfigurationRoot configuration)
            {
                configurationBuilder.AddConfiguration(configuration);
            }

            return configurationBuilder.Build();
        }

        private void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            var cosmosConnectionString = new CosmosConnectionString(configuration[Cosmos.CosmosConnectionString]);
            var cosmosDatabase = configuration[Cosmos.CosmosDatabase];
            var cosmosCollection = configuration[Cosmos.CosmosCollection];
            var cosmosPartitionKey = configuration[Cosmos.CosmosPartitionKey];

            services.AddTransient<IDocumentStorage>(x => new CosmosDocumentStorage(cosmosConnectionString, cosmosPartitionKey));
            services.AddTransient<IHttpRequestHelper, HttpRequestHelper>();
            services.AddTransient<ILoggerHelper, LoggerHelper>();
            services.AddScoped<IPathService, PathService>(sp => new PathService(sp.GetService<IDocumentStorage>(), cosmosDatabase, cosmosCollection));
            services.AddTransient<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
        }
    }
}
