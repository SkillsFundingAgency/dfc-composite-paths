using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;

namespace DFC.Composite.Paths.Tests.PathServiceTests
{
    public class PathServiceTestBase
    {
        protected string CosmosEndpointUri = "https://localhost:8081";
        protected string CosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        protected string CosmosDatabaseName = "test";
        protected string CosmosPartitionKey = "/path";

        protected PathModel Create(string path, Layout layout)
        {
            var pathModel = new PathModel();

            pathModel.Path = path;
            pathModel.Layout = layout;

            return pathModel;
        }
    }
}