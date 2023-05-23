using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        public UserService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        public User AddUser(User user)
        {
            user.Password = PasswordHasher.Hash(user.Password);
            return userRepository.addUser(user);

        }
        public async Task<bool> UpdateUser(User user)
        {
            return await userRepository.updateUserProfile(user);

        }


        public async Task<bool> checkDuplicatedEmail(string email)
        {
            return await userRepository.checkDuplicatedEmail(email);
        }
    }
}
