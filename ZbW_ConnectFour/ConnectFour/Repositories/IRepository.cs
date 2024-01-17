using ConnectFour.Models;

namespace ConnectFour.Repositories
{
    public interface IRepository<T>
    {
        public interface IRepository<T>
        {
            T GetById(int id);
            IEnumerable<T> GetAll();
            void Add(T entity);
            void Update(T entity);
            void Delete(T entity);
        }

    }
}
