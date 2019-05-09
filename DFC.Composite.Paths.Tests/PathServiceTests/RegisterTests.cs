﻿using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Services;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Tests.PathServiceTests
{
    [TestFixture]
    public class RegisterTests : PathServiceTestBase
    {
        private string _collectionName = "register";
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
        public async Task Should_SaveDetails_WhenRegistering()
        {
            var newPath = Create(_path, Layout.FullWidth);

            await _pathService.Register(newPath);

            var existingPath = await _pathService.Get(newPath.Path);
            Assert.IsNotNull(existingPath);
            Assert.AreEqual(newPath.Path, existingPath.Path);
            Assert.AreEqual(newPath.Layout, existingPath.Layout);
        }

        [Test]
        public async Task Should_SetDateOfRegistration_WhenRegistering()
        {
            var newPath = Create(_path, Layout.FullWidth);

            await _pathService.Register(newPath);

            var existingPath = await _pathService.Get(newPath.Path);
            Assert.AreNotEqual(DateTime.MinValue, existingPath.DateOfRegistration);
        }

        [Test]
        public async Task Should_SetLastModifiedDate_WhenRegistering()
        {
            var newPath = Create(_path, Layout.FullWidth);

            await _pathService.Register(newPath);

            var existingPath = await _pathService.Get(newPath.Path);
            Assert.AreNotEqual(DateTime.MinValue, existingPath.LastModifiedDate);
        }

        [Test]
        public async Task ShouldNot_AllowPathToRegisterIfItsAlreadyRegistered()
        {
            var newPath = Create(_path, Layout.FullWidth);

            await _pathService.Register(newPath);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _pathService.Register(newPath));
        }
    }
}