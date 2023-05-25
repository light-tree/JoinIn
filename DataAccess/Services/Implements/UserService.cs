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
            return userRepository.AddUser(user);

        }
        public async Task<bool> UpdateUser(User user)
        {
            return await userRepository.UpdateUserProfile(user);

        }


        public async Task<bool> CheckDuplicatedEmail(string email)
        {
            return await userRepository.CheckDuplicatedEmail(email);
        }

        public async Task<User> FindUserByGuid(Guid id)
        {
            return await userRepository.FindAccountByGUID(id);
        }

        public async Task<User> FindUserByEmail(string email)
        {
            return await userRepository.FindAccountByEmail(email);
        }

        public async Task<User> FindUserByToken(string token)
        {
           return await userRepository.FindAccountByToken(token);
        }
    }
}
