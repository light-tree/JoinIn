using BusinessObject.Data;
using BusinessObject.Enums;
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

        public Member CreateMember(Guid userId, Guid groupId, MemberRole role)
        {
            Member member = new Member
            {
                UserId = userId,
                GroupId = groupId,
                JoinedDate = DateTime.Now,
                Role = role
            };

            _context.Members.Add(member);
            _context.SaveChanges();
            return member;
        }

        public Member FindByUserIdAndGroupId(Guid createdById, Guid groupId)
        {
            return _context.Members.FirstOrDefault(m => m.UserId == createdById && m.GroupId == groupId && m.LeftDate == null);
        }

        public MemberRole? GetRoleInThisGroup(Guid userId, Guid groupId)
        {
            Member member = _context.Members.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId);
            return member != null ? member.Role : null ;
        }
    }
}
