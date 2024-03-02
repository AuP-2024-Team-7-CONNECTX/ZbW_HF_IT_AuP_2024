using ConnectFour.Models;
using ConnectFour.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConnectFour.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IGenericRepository _genericRepository;

        public UserRepository(IGenericRepository genericRepository)
        {
            _genericRepository = genericRepository;
        }


        public async Task ChangePasswordAsync(string userId, string oldPassword, string newPassword)
        {
            // Die Implementierung dieser Methode würde von deiner Anwendungslogik abhängen.
            // Beispiel:
            // 1. Überprüfe, ob das alte Passwort korrekt ist.
            // 2. Wenn ja, aktualisiere das Passwort mit dem neuen Passwort.
            throw new NotImplementedException();
        }

        public async Task CreateOrUpdateAsync(User entity)
        {
            await _genericRepository.CreateOrUpdateAsync(entity);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _genericRepository.GetAllAsync<User>();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _genericRepository.GetByIdAsync<User>(id);
        }

        public async Task DeleteAsync<T>(string id)
        {
            await _genericRepository.DeleteAsync<User>(id);

        }
    }
}
