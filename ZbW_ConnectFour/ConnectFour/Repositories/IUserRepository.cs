using ConnectFour.Models;

namespace ConnectFour.Repositories
{
    public interface IUserRepository
    {
        void ChangePassword(string oldPassword, string newPassword);
    }
}
