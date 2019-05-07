using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task Register(PathModel model)
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

            await _storage.Add<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, model);
        }

        public async Task Update(PathModel model)
        {
            var pathDocument = await GetPath(model.Path);

            if (pathDocument != null)
            {
                var currentDt = DateTime.Now;
                model.LastModifiedDate = currentDt;
                model.DocumentId = pathDocument.DocumentId;

                await _storage.Update<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, pathDocument.DocumentId.ToString(), model);
            }
        }

        private async Task<PathModel> GetPath(string path)
        {
            var documents = await _storage.Search<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, x => x.Path == path);
            return documents.FirstOrDefault();
        }

    }
}
