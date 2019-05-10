using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Services
{
    public class PathService : IPathService
    {
        private readonly IDocumentStorage _storage;
        private readonly CosmosSettings _cosmosSettings;

        public PathService(IDocumentStorage storage, CosmosSettings cosmosSettings)
        {
            _storage = storage;
            _cosmosSettings = cosmosSettings;
        }

        public async Task Delete(string path)
        {
            var pathDocument = await GetPath(path);
            if (pathDocument != null)
            {
                await _storage.Delete(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, pathDocument.DocumentId.ToString());
            }
        }

        public async Task<IEnumerable<PathModel>> GetAll()
        {
            return await _storage.Search<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, null);
        }

        public async Task<PathModel> Get(string path)
        {
            return await GetPath(path);
        }

        public async Task<PathModel> Register(PathModel model)
        {
            var currentDt = DateTime.Now;

            if (model.DocumentId == Guid.Empty)
            {
                model.DocumentId = Guid.NewGuid();
            }

            if (model.DateOfRegistration == DateTime.MinValue)
            {
                model.DateOfRegistration = currentDt;
            }

            if (model.LastModifiedDate == DateTime.MinValue)
            {
                model.LastModifiedDate = currentDt;
            }

            var existingPaths = await GetAll();
            if (existingPaths.Any(x => x.Path.ToLower() == model.Path.ToLower()))
            {
                throw new InvalidOperationException($"A path with the name {model.Path} is already registered");
            }

            await _storage.Add<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, model);

            return model;
        }

        public async Task Update(PathModel updateModel)
        {
            var currentModel = await GetPath(updateModel.Path);

            if (currentModel != null)
            {
                var currentDt = DateTime.Now;

                updateModel.DateOfRegistration = currentModel.DateOfRegistration;
                updateModel.DocumentId = currentModel.DocumentId;
                updateModel.LastModifiedDate = currentDt;

                await _storage.Update<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, currentModel.DocumentId.ToString(), updateModel);
            }
        }

        private async Task<PathModel> GetPath(string path)
        {
            var documents = await _storage.Search<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, x => x.Path == path);
            return documents.FirstOrDefault();
        }
    }
}
