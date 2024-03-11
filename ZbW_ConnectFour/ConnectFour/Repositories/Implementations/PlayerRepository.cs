using ConnectFour.Models;
using ConnectFour.Interfaces; // Stelle sicher, dass das Interface IGenericRepository hier definiert ist
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectFour.Repositories.Interfaces;
using Azure.Core;
using System.Data.Entity.Core;

namespace ConnectFour.Repositories.Implementations
{
    public class PlayerRepository : IPlayerRepository // Stelle sicher, dass IPlayerRepository definiert ist
    {
        private readonly IGenericRepository _genericRepository;
        private readonly IUserRepository _userRepository;

        private readonly ILogger<PlayerRepository> _logger;
        public PlayerRepository(IGenericRepository genericRepository, IUserRepository userRepository, ILogger<PlayerRepository> logger)
        {
            _genericRepository = genericRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task CreateOrUpdateAsync(Player entity)
        {
            var user = await _userRepository.GetByIdAsync(entity.UserId);

            if (user == null)
            {
                _logger.LogError("No User found with Id {0}", entity.UserId);
                throw new ObjectNotFoundException($"User mit id {entity.UserId} konnte nicht gefunden werden");

            }

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