namespace ConnectFour.Repositories
{
    using ConnectFour.Models;
    using System;
    using System.Collections.Generic;
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _Users;

        public void ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void CreateOrUpdate(User entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> GetAll()
        {
            throw new NotImplementedException();
        }

        public User GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
