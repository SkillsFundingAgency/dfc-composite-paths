using DFC.Composite.Paths.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.Composite.Paths.Services
{
    public interface IPathService
    {
        Task Delete(string path);

        Task<IEnumerable<PathModel>> GetAll();

        Task<PathModel> Get(string path);

        Task<PathModel> Register(PathModel model);

        Task Update(PathModel model);
    }
}
