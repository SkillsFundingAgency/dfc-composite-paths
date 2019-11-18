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
        private readonly string _collectionName = "delete";
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
