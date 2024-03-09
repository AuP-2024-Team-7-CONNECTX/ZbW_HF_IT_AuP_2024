using ConnectFour.Models;

namespace ConnectFour.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task CreateOrUpdateAsync(T entity);
        Task DeleteAsync<T>(string id);
    }

}
