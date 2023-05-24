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
    public class GroupRepository : IGroupRepository
    {
        private readonly Context _context;

        public GroupRepository(Context context)
        {
            _context = context;
        }

        public Group CreateGroup(GroupDTOForCreating groupDTOForCreating)
        {
            Group group = new Group
            {
                Name = groupDTOForCreating.Name,
                CreatedDate = DateTime.Now,
                GroupSize = groupDTOForCreating.GroupSize,
                MemberCount = 0,
                SchoolName = groupDTOForCreating.SchoolName,
                SubjectName = groupDTOForCreating.SubjectName,
                ClassName = groupDTOForCreating.ClassName,
                Description = groupDTOForCreating.Description,
                Skill = groupDTOForCreating.Skill,
                Avatar = groupDTOForCreating.Avatar,
                Status = BusinessObject.Enums.GroupStatus.ACTIVE
            };

            _context.Groups.Add(group);
            _context.SaveChanges();
            return group;
        }

        public Group FindById(Guid groupId)
        {
            return _context.Groups.FirstOrDefault(g => g.Id == groupId);
        }

        public Group? IncreaseCurrentMemberCount(Guid groupId, int v)
        {
            Group group = _context.Groups.FirstOrDefault(g =>g.Id == groupId);
            group.MemberCount += v;
            _context.Update(group);
            if(_context.SaveChanges() != 1) throw new Exception("Increase current member count fail.");
            return group;
        }
    }
}
