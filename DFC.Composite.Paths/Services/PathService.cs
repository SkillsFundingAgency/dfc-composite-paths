using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DFC.Composite.Paths.Common;
using DFC.Composite.Paths.Models;
using DFC.Composite.Paths.Storage;

namespace DFC.Composite.Paths.Services
{
    public class PathService : IPathService
    {
        private readonly IDocumentStorage _storage;

        public PathService(IDocumentStorage storage)
        {
            _storage = storage;
        }

        public async Task Delete(string path)
        {
            path = path.ToLower();
            var pathDocument = await GetPath(path);
            if (pathDocument != null)
            {
                await _storage.Delete(pathDocument.DocumentId.ToString(), path);
            }
        }

        public async Task<IEnumerable<PathModel>> GetAll()
        {
            return await _storage.Search<PathModel>(null);
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
            model.Path = model.Path.ToLower();

            await _storage.Add<PathModel>(model);

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

                await _storage.Update<PathModel>(currentModel.DocumentId.ToString(), updateModel);
            }
        }

        private async Task<PathModel> GetPath(string path)
        {
            var documents = await _storage.Search<PathModel>(x => x.Path.ToLower() == path.ToLower());
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
