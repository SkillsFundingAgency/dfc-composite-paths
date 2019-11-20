using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.IntegrationTests.PathServiceTests
{
    [TestFixture]
    public class UpdateTests : PathServiceTestBase
    {
        private readonly string _collectionName = "update";
        private readonly string _path = "path1";

        private IPathService _pathService;
        private IDocumentStorage _documentStorage;

        [SetUp]
        public void SetUp()
        {
            _documentStorage = new CosmosDocumentStorage(new CosmosConnectionString(CosmosConnectionString), CosmosPartitionKey, CosmosDatabase, _collectionName);
            _pathService = new PathService(_documentStorage);
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

        [TestCase(Layout.FullWidth)]
        [TestCase(Layout.SidebarLeft)]
        [TestCase(Layout.SidebarRight)]
        public async Task Should_AllowExternalUrlWithLayoutNoneOnly(Layout layout)
        {
            var newPath = Create(_path, layout);

            await _pathService.Register(newPath);
            var existingPath = await _pathService.Get(newPath.Path);
            existingPath.ExternalURL = "http://www.google.com";
            
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pathService.Update(existingPath));
            Assert.AreEqual(Message.ExternalUrlMustUseLayoutNone, ex.Message);
        }

        [TestCase(Layout.FullWidth)]
        [TestCase(Layout.SidebarLeft)]
        [TestCase(Layout.SidebarRight)]
        public async Task NonExternalUrls_ShouldNotUseLayoutNone(Layout layout)
        {
            var newPath = Create(_path, layout);

            await _pathService.Register(newPath);
            var existingPath = await _pathService.Get(newPath.Path);
            existingPath.Layout = Layout.None;

            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () => await _pathService.Update(existingPath));
            Assert.AreEqual(Message.NonExternalUrlMustNotUseLayoutNone, ex.Message);
        }
    }
}
