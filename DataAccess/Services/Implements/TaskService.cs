using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IAssignedTaskRepository _assignedTaskRepository;
        private readonly ICommentRepository _commentRepository;

        public TaskService(ITaskRepository taskRepository, IMemberRepository memberRepository, IAssignedTaskRepository assignedTaskRepository, ICommentRepository commentRepository)
        {
            _taskRepository = taskRepository;
            _memberRepository = memberRepository;
            _assignedTaskRepository = assignedTaskRepository;
            _commentRepository = commentRepository;
        }

        public TaskRecordDTO CreateTask(TaskDTOForCreating task, Guid createdById)
        {
            if (task.AssignedForIds != null) 
                if (task.AssignedForIds.GroupBy(x => x).Any(g => g.Count() > 1))
                    throw new Exception("Exist duplicated member Id.");
            if (_memberRepository.FindByUserIdAndGroupId(createdById, task.GroupId) == null) 
                throw new Exception("Creater not belong to group or 1 between member or group is not exist.");
            if (_taskRepository.FindByName(task.Name) != null) 
                throw new Exception("Duplicated task's name.");
            Guid newTaskId = _taskRepository.CreateTask(task, createdById).Id;

            List<Guid> assignedForIds = new List<Guid>();
            foreach(Guid assignedFor in task.AssignedForIds == null ? new List<Guid>() : task.AssignedForIds)
            {
                if (_memberRepository.FindByUserIdAndGroupId(assignedFor, task.GroupId) == null) 
                    throw new Exception("Member with Id: " + assignedFor + " not belong to group or 1 between member or group is not exist.");
                assignedForIds.Add(assignedFor);
            }
            _assignedTaskRepository.CreateAssignedTasks(assignedForIds, newTaskId, createdById);

            return _taskRepository.FindRecordById(newTaskId);
        }

        public CommonResponse FilterTasks(Guid userId, string name, int? pageSize, int? page)
        {
            return _taskRepository.FilterTasks(userId, name, pageSize, page);
        }

        public TaskDetailDTO GetDetailById(Guid id, Guid userId)
        {
            BusinessObject.Models.Task task = _taskRepository.FindByIdAndUserId(id, userId);
            if (task == null) throw new Exception("Task is not exist.");
            return new TaskDetailDTO()
            {
                Id = task.Id,
                Name = task.Name,
                StartDateDeadline = task.StartDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                EndDateDeadline = task.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                FinishedDate = task.FinishedDate == null ? "-" : task.FinishedDate.Value.ToString("MMMM d, yyyy | hh:mm tt"),
                ImpotantLevel = task.ImpotantLevel.ToString(),
                EstimatedDays = task.EstimatedDays.ToString(),
                Status = task.Status.ToString(),
                Description = task.Description
            };
        }

        public TaskRecordDTO UpdateTask(TaskDTOForUpdating taskDTO, Guid userId)
        {
            if (taskDTO.AssignedForIds != null)
                if (taskDTO.AssignedForIds.GroupBy(x => x).Any(g => g.Count() > 1))
                    throw new Exception("Exist duplicated member Id.");
            BusinessObject.Models.Task task = _taskRepository.FindById(taskDTO.Id);
            if (task == null) throw new Exception("Task is not exist.");
            if (_memberRepository.FindByUserIdAndGroupId(userId, task.GroupId) == null)
                throw new Exception("Updater not belong to group or 1 between member or group is not exist.");

            foreach (Guid assignedFor in taskDTO.AssignedForIds == null ? new List<Guid>() : taskDTO.AssignedForIds)
            {
                if (_memberRepository.FindByUserIdAndGroupId(assignedFor, task.GroupId) == null)
                    throw new Exception("Member with Id: " + assignedFor + " not belong to group or 1 between member or group is not exist.");
            }

            _taskRepository.UpdateTask(taskDTO, userId);

            List<Guid> currentAssignedForIds = _assignedTaskRepository.FindByTaskId(task.Id).Select(a => a.AssignedForId).ToList();
            List<Guid> newAssignedForIds = taskDTO.AssignedForIds == null ? new List<Guid>() : taskDTO.AssignedForIds;
            List<Guid> lostAssignedForIds = currentAssignedForIds.Except(newAssignedForIds).ToList();
            foreach(Guid id in lostAssignedForIds)
            {
                _assignedTaskRepository.DeleteByAssignedForId(id);
            }
            List<Guid> addedAssignedForIds = newAssignedForIds.Except(currentAssignedForIds).ToList();
            _assignedTaskRepository.CreateAssignedTasks(addedAssignedForIds, task.Id, userId);

            return _taskRepository.FindRecordById(task.Id);
        }

        public int DeleteTask(Guid taskId, Guid userId)
        {
            BusinessObject.Models.Task deletedTask = _taskRepository.FindById(taskId);
            if (deletedTask == null) throw new Exception("Task không tồn tại.");
            if (_memberRepository.FindByUserIdAndGroupId(userId, deletedTask.GroupId) == null)
                throw new Exception("Deleter not belong to group or 1 between member or group is not exist.");

            List<BusinessObject.Models.Task> deletedSubTasks = _taskRepository.FindByMainTaskId(taskId);
            if(deletedSubTasks.Count != 0)
            {
                foreach (BusinessObject.Models.Task task in deletedSubTasks)
                {
                    DeleteTask(task.Id, userId);
                }
            }

            _commentRepository.DeleteByTaskId(taskId);
            _assignedTaskRepository.DeleteByTaskId(taskId);
            return _taskRepository.DeleteByTaskId(taskId);
        }
    }
}
