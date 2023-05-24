using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IMemberRepository
    {
        Member CreateMember(Guid userId, Guid groupId, MemberRole role);
        Member FindByUserIdAndGroupId(Guid createdById, Guid groupId);
        MemberRole? GetRoleInThisGroup(Guid userId, Guid groupId);
    }
}
