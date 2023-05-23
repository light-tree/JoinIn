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

        public User addUser(User user);

        public Task<bool> checkDuplicatedEmail(string email);

        public Task<bool> updateUserProfile(User user);

        public User FindAccountByEmail(string email);
    }
}
