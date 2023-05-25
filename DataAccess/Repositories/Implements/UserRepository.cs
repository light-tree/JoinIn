using BusinessObject.Data;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _context;
        public UserRepository(Context context)
        {
            this._context = context;
        }

        public User AddUser(User user)
        {
            try
            {
                //User user = new User();
                //user.Email = email;
                //user.FullName = fullName;
                //user.Password= password;
                //user.Phone = phone;
                //user.BirthDay = dob;
                //user.Gender = gender;
                //user.Description = description;
                //user.OtherContact = otherContact;
                //user.Skill= skill;
                //user.Avatar = avatarLink;
                //user.Theme= themeLink;
                //user.Status= userStatus;
                //user.IsAdmin = false;
                _context.Users.Add(user);
                _context.SaveChanges();
                return user;
            }
            catch (Exception e)
            {

                return null;
            }
        }


        public async Task<bool> CheckDuplicatedEmail(string email)
        {
            bool check = await _context.Users.AnyAsync(u => u.Email == email);
            return check;
        }

        public async Task<User> FindAccountByEmail(string email)
        {
            try
            {
                return _context.Users.FirstOrDefault(u => u.Email == email);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> FindAccountByGUID(Guid id)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<User> FindAccountByToken(string token)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(p => p.Token == token);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUserProfile(User user)
        {
            try
            {
                
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
