using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IAssignedTaskRepository
    {
        List<AssignedTask> CreateAssignedTasks(List<Guid> assignedForIds, Guid newTaskId, Guid createdById);
        int DeleteByAssignedForId(Guid lostAssignedForId);
        int DeleteByTaskId(Guid taskId);
        List<AssignedTask> FindByTaskId(Guid id);
    }
}
