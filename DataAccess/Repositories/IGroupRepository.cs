using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IGroupRepository
    {
        Group CreateGroup(GroupDTOForCreating groupDTOForCreating);
        Group FindById(Guid groupId);
        Group IncreaseCurrentMemberCount(Guid groupId, int v);
    }
}
