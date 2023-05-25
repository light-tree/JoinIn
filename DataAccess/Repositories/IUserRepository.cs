using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IUserRepository
    {

        public User AddUser(User user);

        public Task<bool> CheckDuplicatedEmail(string email);

        public Task<bool> UpdateUserProfile(User user);

        public Task<User> FindAccountByEmail(string email);

        public Task<User> FindAccountByGUID(Guid id);

        public Task<User> FindAccountByToken(string token);
    }
}
