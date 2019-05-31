using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DFC.Composite.Paths.Tests.PathServiceTests
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

            CosmosConnectionString = configurationRoot[Cosmos.CosmosConnectionString];
            CosmosDatabase = configurationRoot[Cosmos.CosmosDatabaseId];
            CosmosPartitionKey = configurationRoot[Cosmos.CosmosPartitionKey];
        }

        protected PathModel Create(string path, Layout layout)
        {
            var pathModel = new PathModel();

            pathModel.Path = path;
            pathModel.Layout = layout;

            return pathModel;
        }
    }
}