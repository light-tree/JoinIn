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

        public User addUser(User user)
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


        public async Task<bool> checkDuplicatedEmail(string email)
        {
            bool check = await _context.Users.AnyAsync(u => u.Email == email);
            return check;
        }

        public User FindAccountByEmail(string email)
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


        public async Task<bool> updateUserProfile(User user)
        {
            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }

        }
    }
}
