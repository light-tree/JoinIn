using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IGroupMajorRepository
    {
        GroupMajor CreateGroupMajor(GroupMajorDTO groupMajorDTO);
        GroupMajor? DecreaseCurrentNeededMemberCount(GroupMajor groupMajor, int v);
        List<GroupMajor> FindByGroupId(Guid groupId);
    }
}
