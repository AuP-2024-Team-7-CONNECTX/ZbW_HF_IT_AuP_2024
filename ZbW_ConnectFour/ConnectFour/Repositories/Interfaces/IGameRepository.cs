using ConnectFour.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectFour.Repositories.Interfaces
{
    public interface IGameRepository
    {
        Task CreateOrUpdateAsync(Game entity);
        Task<IEnumerable<Game>> GetAllAsync();
        Task<Game> GetByIdAsync(string id);
        Task DeleteAsync<T>(string id);
    }
}
