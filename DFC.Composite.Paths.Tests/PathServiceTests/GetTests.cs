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
    public class GetTests
    {
        private string _cosmosEndpointUri = "https://localhost:8081";
        private string _cosmosKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        private string _databaseName = "test";
        private string _collectionName = "get";
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
        public async Task Should_GetPath_ForExistingPath()
        {
            var newPath = Create(_path, Layout.FullWidth);

            await _pathService.Register(newPath);            
            var existingPath = await _pathService.Get(newPath.Path);

            Assert.IsNotNull(existingPath);
            Assert.AreEqual(newPath.Path, existingPath.Path);
            Assert.AreEqual(newPath.Layout, existingPath.Layout);
        }

        [Test]
        public async Task Should_ReturnNull_ForNonExistingPath()
        {
            var existingPath = await _pathService.Get(Guid.NewGuid().ToString());

            Assert.IsNull(existingPath);
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
