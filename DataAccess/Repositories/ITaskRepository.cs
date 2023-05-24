using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Repositories
{
    public interface ITaskRepository
    {
        BusinessObject.Models.Task CreateTask(TaskDTOForCreating task, Guid createdById);
        CommonResponse FilterTasks(Guid userId, string name, int? pageSize, int? page);
        BusinessObject.Models.Task FindByName(string name);
        TaskRecordDTO FindRecordById(Guid newTaskId);
        BusinessObject.Models.Task FindById(Guid id);
        BusinessObject.Models.Task FindByIdAndUserId(Guid id, Guid userId);
        int UpdateTask(TaskDTOForUpdating taskDTO, Guid userId);
        List<BusinessObject.Models.Task> FindByMainTaskId(Guid taskId);
        int DeleteByTaskId(Guid taskId);
    }
}
