using System.IO;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using Microsoft.Extensions.Configuration;

namespace DFC.Composite.Paths.IntegrationTests.PathServiceTests
{
    public class PathServiceTestBase
    {
        protected readonly string CosmosConnectionString;
        protected readonly string CosmosDatabase;
        protected readonly string CosmosPartitionKey;

        public PathServiceTestBase()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
             .AddEnvironmentVariables();

            var configurationRoot = builder.Build();

            CosmosConnectionString = configurationRoot["Values:" + Cosmos.CosmosConnectionString];
            CosmosDatabase = configurationRoot["Values:" + Cosmos.CosmosDatabaseId];
            CosmosPartitionKey = configurationRoot["Values:" + Cosmos.CosmosPartitionKey];
        }

        protected PathModel Create(string path, Layout layout)
        {
            var pathModel = new PathModel
            {
                Path = path,
                Layout = layout
            };

            return pathModel;
        }
    }
}