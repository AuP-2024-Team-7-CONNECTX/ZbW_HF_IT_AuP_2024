using ConnectFour.Models;
using ConnectFour.Interfaces; // Stelle sicher, dass das Interface IGenericRepository hier definiert ist
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectFour.Repositories.Interfaces;

namespace ConnectFour.Repositories.Implementations
{
    public class PlayerRepository : IPlayerRepository // Stelle sicher, dass IPlayerRepository definiert ist
    {
        private readonly IGenericRepository _genericRepository;

        public PlayerRepository(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task CreateOrUpdateAsync(Player entity)
        {
            await _genericRepository.CreateOrUpdateAsync(entity);
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _genericRepository.GetAllAsync<Player>();
        }

        public async Task<Player> GetByIdAsync(string id)
        {
            return await _genericRepository.GetByIdAsync<Player>(id);
        }
              
        public async Task DeleteAsync<T>(string id)
        {
            await _genericRepository.DeleteAsync<Player>(id);

        }

    }
}