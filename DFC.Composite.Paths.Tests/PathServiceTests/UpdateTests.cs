using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.PathServiceTests
{
    [TestFixture]
    public class UpdateTests
    {
        private string _cosmosEndpointUri = "https://localhost:8081";
        private string _cosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private string _databaseName = "test";
        private string _collectionName = "update";
        private string _path = "path1";

        private IPathService _pathService;
        private IDocumentStorage _documentStorage;
        private CosmosSettings _cosmosSettings;

        [SetUp]
        public void SetUp()
        {
            _cosmosSettings = new CosmosSettings() { };
            _cosmosSettings.Uri = _cosmosEndpointUri;
            _cosmosSettings.Key = _cosmosKey;
            _cosmosSettings.DatabaseName = _databaseName;
            _cosmosSettings.CollectionName = _collectionName;

            _documentStorage = new CosmosDocumentStorage(_cosmosEndpointUri, _cosmosKey);
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

        private PathModel Create(string path, Layout layout)
        {
            var pathModel = new PathModel();

            pathModel.Path = path;
            pathModel.Layout = layout;

            return pathModel;
        }
    }
}
