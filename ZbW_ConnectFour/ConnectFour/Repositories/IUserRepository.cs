using ConnectFour.Models;

namespace ConnectFour.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        void ChangePassword(string oldPassword, string newPassword);
    }
}
