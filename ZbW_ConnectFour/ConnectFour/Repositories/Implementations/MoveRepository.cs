using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;

namespace ConnectFour.Repositories.Implementations
{
    public class MoveRepository : IMoveRepository
    {
        private readonly IGenericRepository _genericRepository;

        public MoveRepository(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task CreateOrUpdateAsync(Move entity)
        {
            await _genericRepository.CreateOrUpdateAsync(entity);
        }

        public async Task<IEnumerable<Move>> GetAllAsync()
        {
            return await _genericRepository.GetAllAsync<Move>();
        }

        public async Task<Move> GetByIdAsync(string id)
        {
            return await _genericRepository.GetByIdAsync<Move>(id);
        }

        public async Task DeleteAsync<T>(string id)
        {
            await _genericRepository.DeleteAsync<Move>(id);
        }
    }
}
