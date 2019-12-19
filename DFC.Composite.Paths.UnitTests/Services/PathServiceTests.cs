using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.UnitTests.Services
{
    [TestFixture]
    public class PathServiceTests : ServiceTestBase
    {
        private IPathService pathService;
        private Mock<IDocumentStorage> documentStorage;
        private const string path = "path1";

        [SetUp]
        public void SetUp()
        {
            documentStorage = new Mock<IDocumentStorage>();
            pathService = new PathService(documentStorage.Object); ;
        }

        [Test]
        public async Task CallingDelete_CallsDeleteOnDocumentStorage()
        {
            var pathModels = GetPathModels();
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            await pathService.Delete(path);

            documentStorage.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public async Task CallingGetAll_CallsSearchOnDocumentStorage()
        {
            var pathModels = GetPathModels();
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            await pathService.GetAll();

            documentStorage.Verify(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>()), Times.Once());
        }


        [Test]
        public async Task CallingGet_CallsSearchOnDocumentStorage()
        {
            var pathModels = GetPathModels();
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            await pathService.Get(path);

            documentStorage.Verify(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>()), Times.Once());
        }

        [Test]
        public async Task CallingRegister_CallsAddOnDocumentStorageForNewPath()
        {
            var newPathModel1 = new PathModel() { Path = path, Layout = Layout.SidebarLeft };
            var pathModels = new List<PathModel>();
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            await pathService.Register(newPathModel1);

            documentStorage.Verify(x => x.Add(newPathModel1), Times.Once());
        }

        [Test]
        public async Task CallingRegister_OnExistingPathFails()
        {
            var newPathModel1 = new PathModel() { Path = path, Layout = Layout.SidebarLeft };
            var pathModels = new List<PathModel>() { newPathModel1 };
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await pathService.Register(newPathModel1));
        }

        [Test]
        public async Task CallingRegister_ThrowsExceptionWhenExternalUrlIsSetAndLayoutIsNotNone()
        {
            var newPathModel1 = new PathModel()
            {
                Path = path,
                Layout = Layout.SidebarLeft,
                DocumentId = Guid.NewGuid(),
                ExternalURL = "http://www.google.com"
            };
            var pathModels = new List<PathModel>();
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await pathService.Register(newPathModel1));
        }

        [Test]
        public async Task CallingRegister_ThrowsExceptionWhenExternalUrlIsNotSetAndLayoutIsNone()
        {
            var newPathModel1 = new PathModel()
            {
                Path = path,
                Layout = Layout.None,
                DocumentId = Guid.NewGuid()
            };
            var pathModels = new List<PathModel>();
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await pathService.Register(newPathModel1));
        }

        [Test]
        public async Task CallingUpdate_CallsUpdateOnDocumentStorage()
        {
            var pathModel1 = new PathModel() { Path = path, Layout = Layout.SidebarLeft, DocumentId = Guid.NewGuid() };
            var pathModels = new List<PathModel>() { pathModel1 };
            documentStorage.Setup(x => x.Search(It.IsAny<Expression<Func<PathModel, bool>>>())).ReturnsAsync(pathModels);

            await pathService.Update(pathModel1);

            documentStorage.Verify(x => x.Update<PathModel>(pathModel1.DocumentId.ToString(), pathModel1), Times.Once());
        }


        private List<PathModel> GetPathModels()
        {
            var pathModel = new PathModel() { Path = path };
            return new List<PathModel>() { pathModel };
        }
    }
}
