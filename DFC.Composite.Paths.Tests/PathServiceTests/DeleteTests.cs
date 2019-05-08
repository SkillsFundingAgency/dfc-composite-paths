using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using NUnit.Framework;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.PathServiceTests
{
    [TestFixture]
    public class DeleteTests : PathServiceTestBase
    {
        private string _collectionName = "delete";
        private string _path = "path1";

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
            await _pathService.Delete(_path);
        }

        [Test]
        public async Task Should_Delete_ExistingPath()
        {
            var newPath = Create(_path, Layout.FullWidth);
            await _pathService.Register(newPath);

            await _pathService.Delete(newPath.Path);

            var existingPath = await _pathService.Get(newPath.Path);
            Assert.IsNull(existingPath);
        }
    }
}
