﻿using BusinessObject.Models;
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

        public Task<bool> CheckDuplicatedEmail(string email);

        public Task<bool> UpdateUser(User user);

        public Task<User> FindUserByGuid(Guid guid);

        public Task<User> FindUserByEmail(string email);

        public Task<User> FindUserByToken(string token);
    }
}
