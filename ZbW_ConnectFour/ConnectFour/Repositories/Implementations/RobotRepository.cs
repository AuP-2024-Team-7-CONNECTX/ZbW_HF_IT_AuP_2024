using ConnectFour.Models;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using System.Data.Entity.Core;

namespace ConnectFour.Repositories
{
	public class RobotRepository : IRobotRepository // Stelle sicher, dass IRobotRepository definiert ist
	{
		private readonly IGenericRepository _genericRepository;
		private readonly IPlayerRepository _playerRepository;

		private readonly ILogger<RobotRepository> _logger;

		public RobotRepository(IGenericRepository genericRepository, IPlayerRepository playerRepository, ILogger<RobotRepository> logger)
		{
			_genericRepository = genericRepository;
			_playerRepository = playerRepository;
			_logger = logger;
		}

		public async Task CreateOrUpdateAsync(Robot entity)
		{
			if (entity.CurrentPlayerId != null)
			{
				var player = await _playerRepository.GetByIdAsync(entity.CurrentPlayerId);

				if (player == null)
				{
					_logger.LogError("No Player found with Id {0}", entity.CurrentPlayerId);
					throw new ObjectNotFoundException($"Player mit id {entity.CurrentPlayerId} konnte nicht gefunden werden");
				}
			}
			//if (_genericRepository.GetAllAsync<Robot>().Result.Where(r => r.Name == entity.Name).Count() > 0)
			//{
			//	_logger.LogError($"A robot with the name '{entity.Name}' already exists.");
			//	throw new InvalidOperationException($"A robot with the name '{entity.Name}' already exists.");

			//}

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
