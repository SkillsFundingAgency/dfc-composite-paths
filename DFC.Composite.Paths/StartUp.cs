using DFC.Common.Standard.Logging;
using DFC.Composite.Paths;
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
            var cosmosSettings = new CosmosSettings()
            {
                Key = configuration["cosmosKey"],
                Uri = configuration["cosmosUrl"],
                DatabaseName = configuration["cosmosDatabase"],
                CollectionName = configuration["cosmosCollection"],
                PartitionKey = configuration["cosmosPartitionKey"]
            };
            services.AddSingleton(cosmosSettings);

            services.AddTransient<IDocumentStorage>(x => new CosmosDocumentStorage(cosmosSettings.Uri, cosmosSettings.Key, cosmosSettings.PartitionKey));

            services.AddTransient<IHttpRequestHelper, HttpRequestHelper>();
            services.AddTransient<ILoggerHelper, LoggerHelper>();
            services.AddTransient<IPathService, PathService>();
            services.AddTransient<ISwaggerDocumentGenerator, SwaggerDocumentGenerator>();
        }
    }
}
