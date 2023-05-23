using BusinessObject.Data;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class MemberRepository : IMemberRepository
    {
        private readonly Context _context;

        public MemberRepository(Context context)
        {
            _context = context;
        }

        public Member FindByUserIdAndGroupId(Guid createdById, Guid groupId)
        {
            return _context.Members.FirstOrDefault(m => m.UserId == createdById && m.GroupId == groupId && m.LeftDate == null);
        }
    }
}
