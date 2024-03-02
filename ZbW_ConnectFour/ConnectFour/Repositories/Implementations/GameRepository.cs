using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;

namespace ConnectFour.Repositories.Implementations
{
    public class GameRepository : IGameRepository
    {
        private readonly IGenericRepository _genericRepository;

        public GameRepository(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task CreateOrUpdateAsync(Game entity)
        {
            await _genericRepository.CreateOrUpdateAsync(entity);
        }

        public async Task<IEnumerable<Game>> GetAllAsync()
        {
            return await _genericRepository.GetAllAsync<Game>();
        }

        public async Task<Game> GetByIdAsync(string id)
        {
            return await _genericRepository.GetByIdAsync<Game>(id);
        }

        public async Task DeleteAsync<T>(string id)
        {
            await _genericRepository.DeleteAsync<Game>(id);
        }
    }
}
