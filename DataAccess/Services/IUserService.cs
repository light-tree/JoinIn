using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IUserService
    {
        public User AddUser(User user);

        public Task<bool> checkDuplicatedEmail(string email);

        public Task<bool> UpdateUser(User user);
    }
}
