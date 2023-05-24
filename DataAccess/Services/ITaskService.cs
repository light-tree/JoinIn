using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Services
{
    public interface ITaskService
    {
        TaskRecordDTO CreateTask(TaskDTOForCreating task, Guid createdById);
        int DeleteTask(Guid taskId, Guid userId);
        CommonResponse FilterTasks(Guid userId, string name, int? pageSize, int? page);
        TaskDetailDTO GetDetailById(Guid id, Guid userId);
        TaskRecordDTO UpdateTask(TaskDTOForUpdating task, Guid userId);
    }
}
