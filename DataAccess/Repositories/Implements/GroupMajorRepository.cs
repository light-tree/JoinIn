using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class GroupMajorRepository : IGroupMajorRepository
    {
        private readonly Context _context;

        public GroupMajorRepository(Context context)
        {
            _context = context;
        }

        public GroupMajor CreateGroupMajor(Guid groudId, GroupMajorDTO groupMajorDTO)
        {
            GroupMajor groupMajor = new GroupMajor
            {
                MajorId = groupMajorDTO.MajorId,
                GroupId = groudId,
                MemberCount = groupMajorDTO.MemberCount,
                Status = groupMajorDTO.Status,
            };

            _context.GroupMajors.Add(groupMajor);
            _context.SaveChanges();
            return groupMajor;
        }

        public GroupMajor? DecreaseCurrentNeededMemberCount(GroupMajor groupMajor, int v)
        {
            groupMajor.MemberCount -= v;
            _context.GroupMajors.Update(groupMajor);
            if(_context.SaveChanges() == 1) return groupMajor;
            else throw new Exception("Decrease current needed member count fail.");
        }

        public List<GroupMajor> FindByGroupId(Guid groupId)
        {
            return _context.GroupMajors.Where(gm => gm.GroupId == groupId).ToList();
        }
    }
}
