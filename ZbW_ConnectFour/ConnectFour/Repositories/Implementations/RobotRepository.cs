using ConnectFour.Models;
using ConnectFour.Repositories.Implementations;
using ConnectFour.Repositories.Interfaces;
using System.Data.Entity.Core;

namespace ConnectFour.Repositories
{
	public class RobotRepository : IRobotRepository // Stelle sicher, dass IRobotRepository definiert ist
	{
		private readonly IGenericRepository _genericRepository;
		private readonly IUserRepository _userRepository;

		private readonly ILogger<RobotRepository> _logger;

		public RobotRepository(IGenericRepository genericRepository, IUserRepository userRepository, ILogger<RobotRepository> logger)
		{
			_genericRepository = genericRepository;
			_userRepository = userRepository;
			_logger = logger;
		}

		public async Task CreateOrUpdateAsync(Robot entity)
		{
			if (entity.CurrentUserId != null)
			{
				var User = await _userRepository.GetByIdAsync(entity.CurrentUserId);

				if (User == null)
				{
					_logger.LogError("No User found with Id {0}", entity.CurrentUserId);
					throw new ObjectNotFoundException($"User mit id {entity.CurrentUserId} konnte nicht gefunden werden");
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
