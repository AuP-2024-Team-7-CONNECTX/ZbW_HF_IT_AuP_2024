using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using System.Data.Entity.Core;
using static ConnectFour.Enums.Enum;

namespace ConnectFour.Repositories.Implementations
{
	public class GameRepository : IGameRepository
	{
		private readonly IGenericRepository _genericRepository;
		private readonly IMoveRepository _moveRepository;

		public GameRepository(IGenericRepository genericRepository, IMoveRepository moveRepository)
		{
			_genericRepository = genericRepository;
			_moveRepository = moveRepository;
		}

		public async Task CreateOrUpdateAsync(Game entity)
		{
			if (entity.GameMode != GameMode.PlayerVsPlayer)
			{
				if (entity.Robots.Count() == 1)
				{
					if (string.IsNullOrEmpty(entity.Robot1Id))
					{
						entity.Robot1Id = entity.Robots[0].Id;
					}
					entity.Robot2Id = "NV";
				}
			}

			if (entity.Robots.Count() == 2)
			{
				if (string.IsNullOrEmpty(entity.Robot1Id))
				{
					entity.Robot1Id = entity.Robots[0].Id;
				}
				if (string.IsNullOrEmpty(entity.Robot2Id))
				{
					entity.Robot2Id = entity.Robots[1].Id;
				}
			}

			if (entity.Users.Count() == 2)
			{
				if (string.IsNullOrEmpty(entity.User1Id))
				{
					entity.User1Id = entity.Robots[0].CurrentUserId;
				}
				if (entity.GameMode == GameMode.PlayerVsPlayer)
				{
					if (string.IsNullOrEmpty(entity.User2Id))
					{
						entity.User2Id = entity.Robots[1].CurrentUserId;
					}
				}  else
				{
					entity.User2Id = entity.Users.First(u => u.Name == "KI_Terminator@ConnectX.ch").Id;
				}
			}


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
