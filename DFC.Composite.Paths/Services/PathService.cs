using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Storage;
using DFC.Composite.Paths.Storage.Cosmos;
using System.Collections.Generic;
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
            await _storage.Delete(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, path);
        }

        public async Task<IEnumerable<PathModel>> GetAll()
        {
            return await _storage.Search<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, null);
        }

        public async Task<PathModel> Get(string path)
        {
            return await _storage.Get<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, path);
        }

        public async Task Register(PathModel model)
        {
            await _storage.Add<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, model);
        }

        public async Task Update(PathModel model)
        {
            await _storage.Update<PathModel>(_cosmosSettings.DatabaseName, _cosmosSettings.CollectionName, model.Path, model);
        }

    }
}
