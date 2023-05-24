using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IGroupService
    {
        Guid CreateGroup(Guid createrId, GroupDTOForCreating groupDTOForCreating);
    }
}
