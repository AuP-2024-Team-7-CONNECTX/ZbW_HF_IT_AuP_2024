using ConnectFour.Models;

namespace ConnectFour.Repositories
{
    public interface IRepository<T>
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void CreateOrUpdate(T entity);
        //void Delete(T entity);
    }

}
