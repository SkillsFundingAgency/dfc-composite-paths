using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Services
{
    public class PathService : IPathService
    {
        private readonly IDocumentStorage _storage;
        private readonly string _database;
        private readonly string _collection;

        public PathService(IDocumentStorage storage, string database, string collection)
        {
            _storage = storage;
            _database = database;
            _collection = collection;
        }

        public async Task Delete(string path)
        {
            var pathDocument = await GetPath(path);
            if (pathDocument != null)
            {
                await _storage.Delete(_database, _collection, pathDocument.DocumentId.ToString());
            }
        }

        public async Task<IEnumerable<PathModel>> GetAll()
        {
            return await _storage.Search<PathModel>(_database, _collection, null);
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

            Validate(model);

            var existingPaths = await GetAll();
            if (existingPaths.Any(x => x.Path.ToLower() == model.Path.ToLower()))
            {
                throw new InvalidOperationException(string.Format(Message.PathAlreadyExists, model.Path));
            }

            await _storage.Add<PathModel>(_database, _collection, model);

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

                Validate(updateModel);

                await _storage.Update<PathModel>(_database, _collection, currentModel.DocumentId.ToString(), updateModel);
            }
        }

        private async Task<PathModel> GetPath(string path)
        {
            var documents = await _storage.Search<PathModel>(_database, _collection, x => x.Path == path);
            return documents.FirstOrDefault();
        }

        private void Validate(PathModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.ExternalURL) && model.Layout != Layout.None)
            {
                throw new InvalidOperationException(Message.ExternalUrlMustUseLayoutNone);
            }
            if (string.IsNullOrWhiteSpace(model.ExternalURL) && model.Layout == Layout.None)
            {
                throw new InvalidOperationException(Message.NonExternalUrlMustNotUseLayoutNone);
            }
        }
    }
}
