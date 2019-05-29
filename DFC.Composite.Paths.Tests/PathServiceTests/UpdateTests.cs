using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.PathServiceTests
{
    [TestFixture]
    public class UpdateTests : PathServiceTestBase
    {
        private string _collectionName = "update";
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
            _cosmosSettings.PartitionKey = CosmosPartitionKey;
            _cosmosSettings.DatabaseName = CosmosDatabaseName;
            _cosmosSettings.CollectionName = _collectionName;

            _documentStorage = new CosmosDocumentStorage(CosmosEndpointUri, CosmosKey, CosmosPartitionKey);
            _pathService = new PathService(_documentStorage, _cosmosSettings);
        }


        [TearDown]
        public async Task TearDown()
        {
            await _pathService.Delete(_path);
        }

        [Test]
        public async Task Should_UpdateDetails()
        {
            var newPath = Create(_path, Layout.FullWidth);

            await _pathService.Register(newPath);
            var existingPath = await _pathService.Get(newPath.Path);
            existingPath.TopNavigationText = "TopNavigationText";
            await _pathService.Update(existingPath);
            var modifiedPath = await _pathService.Get(newPath.Path);

            Assert.IsNotNull(existingPath);
            Assert.AreEqual(existingPath.Path, modifiedPath.Path);
            Assert.AreEqual(existingPath.Layout, modifiedPath.Layout);
            Assert.AreEqual(existingPath.TopNavigationText, modifiedPath.TopNavigationText);
        }

        [Test]
        public async Task Should_SetLastModifiedDate_WhenUpdating()
        {
            var newPath = Create(_path, Layout.FullWidth);

            await _pathService.Register(newPath);
            var lastModifiedOriginal = newPath.LastModifiedDate;
            var existingPath = await _pathService.Get(newPath.Path);
            existingPath.TopNavigationText = "TopNavigationText";
            await _pathService.Update(existingPath);
            var lastModified = existingPath.LastModifiedDate;
            var modifiedPath = await _pathService.Get(newPath.Path);

            Assert.AreNotEqual(DateTime.MinValue, modifiedPath.LastModifiedDate);
            Assert.Greater(lastModified, lastModifiedOriginal);
        }        
    }
}
