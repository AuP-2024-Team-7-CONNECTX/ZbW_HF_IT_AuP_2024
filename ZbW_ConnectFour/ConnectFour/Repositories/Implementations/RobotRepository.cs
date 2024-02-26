using ConnectFour.Models;
using ConnectFour.Interfaces; // Stelle sicher, dass das Interface IGenericRepository hier definiert ist
using System.Collections.Generic;
using System.Threading.Tasks;
using ConnectFour.Repositories.Interfaces;

namespace ConnectFour.Repositories
{
    public class RobotRepository : IRobotRepository // Stelle sicher, dass IRobotRepository definiert ist
    {
        private readonly IGenericRepository _genericRepository;

        public RobotRepository(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task CreateOrUpdateAsync(Robot entity)
        {
            await _genericRepository.CreateOrUpdateAsync(entity);
        }

        public async Task<IEnumerable<Robot>> GetAllAsync()
        {
            return await _genericRepository.GetAllAsync<Robot>();
        }

        public async Task<Robot> GetByIdAsync(string id)
        {
            return await _genericRepository.GetByIdAsync<Robot>(id);
        }

        public async Task DeleteAsync<T>(string id)
        {
            await _genericRepository.DeleteAsync<Robot>(id);
        }

        
    }
}
