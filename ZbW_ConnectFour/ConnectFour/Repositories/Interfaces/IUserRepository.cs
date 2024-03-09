using ConnectFour.Models;

namespace ConnectFour.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task ChangePasswordAsync(string userId, string oldPassword, string newPassword);
    }
}
