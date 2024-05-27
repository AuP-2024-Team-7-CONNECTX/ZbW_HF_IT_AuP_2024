using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using System.Data.Entity.Core;

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
            if (entity.CurrentMoveId != null)
            {
                var move = await _moveRepository.GetByIdAsync(entity.CurrentMoveId);

                if (move == null)
                {
                    throw new ObjectNotFoundException($"Move mit id {entity.CurrentMoveId} konnte nicht gefunden werden");
                }
            }
            entity.Robot1Id = entity.Robots[0].Id;
			entity.Robot2Id = entity.Robots[1].Id;

			entity.User1Id = entity.Robots[0].CurrentUserId;
			entity.User2Id = entity.Robots[1].CurrentUserId;

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
