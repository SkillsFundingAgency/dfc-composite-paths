using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.IntegrationTests.PathServiceTests
{
    [TestFixture]
    public class GetAllTests : PathServiceTestBase
    {
        private readonly string _collectionName = "getAll";

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
