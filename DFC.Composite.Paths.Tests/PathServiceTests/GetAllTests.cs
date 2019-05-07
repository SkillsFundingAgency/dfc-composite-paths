using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.PathServiceTests
{
    [TestFixture]
    public class GetAllTests : PathServiceTestBase
    {
        private string _collectionName = "getAll";

        private IPathService _pathService;
        private IDocumentStorage _documentStorage;
        private CosmosSettings _cosmosSettings;

        [SetUp]
        public void SetUp()
        {
            _cosmosSettings = new CosmosSettings() { };
            _cosmosSettings.Uri = CosmosEndpointUri;
            _cosmosSettings.Key = CosmosKey;
            _cosmosSettings.DatabaseName = CosmosDatabaseName;
            _cosmosSettings.CollectionName = _collectionName;

            _documentStorage = new CosmosDocumentStorage(CosmosEndpointUri, CosmosKey);
            _pathService = new PathService(_documentStorage, _cosmosSettings);
        }

        [TearDown]
        public async Task TearDown()
        {
            var paths = await _pathService.GetAll();
            foreach (var path in paths)
            {
                await _pathService.Delete(path.Path);
            }
        }

        [Test]
        public async Task Should_GetAll_WhenTheyExist()
        {
            var pathCount = 5;
            for (var i = 1; i <= pathCount; i++)
            {
                var path = $"path{i}";
                var newPath = Create(path, Layout.FullWidth);
                await _pathService.Register(newPath);
            }

            var paths = await _pathService.GetAll();
            Assert.AreEqual(pathCount, paths.Count());
        }

    }
}
