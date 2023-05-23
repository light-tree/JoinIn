using BusinessObject.Data;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class UserMajorRepository : IUserMajorRepository
    {
        Context _context;
        public UserMajorRepository(Context context)
        {
            this._context = context;
        }
        public Task<bool> assignMajorToUser(List<UserMajor> userMajors)
        {
            return null;
        }
    }
}
