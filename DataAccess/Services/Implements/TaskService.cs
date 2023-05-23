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
            if (_memberRepository.FindByUserIdAndGroupId(createdById, task.GroupId) == null) 
                throw new Exception("Member không thuộc group hoặc 1 trong member và group không tồn tại.");
            if (_taskRepository.FindByName(task.Name) != null) 
                throw new Exception("Tên task đã tồn tại.");
            Guid newTaskId = _taskRepository.CreateTask(task, createdById).Id;

            List<Guid> assignedForIds = new List<Guid>();
            foreach(Guid assignedFor in task.AssignedForIds){
                if (_memberRepository.FindByUserIdAndGroupId(assignedFor, task.GroupId) == null) 
                    throw new Exception("Member không thuộc group hoặc 1 trong member và group không tồn tại.");
                assignedForIds.Add(assignedFor);
            }
            _assignedTaskRepository.CreateAssignedTasks(assignedForIds, newTaskId, createdById);

            return _taskRepository.FindRecordById(newTaskId);
        }

        public CommonResponse FilterTasks(Guid userId, string name, int? pageSize, int? page)
        {
            return _taskRepository.FilterTasks(userId, name, pageSize, page);
        }

        public TaskDetailDTO GetDetailById(Guid id)
        {
            BusinessObject.Models.Task task = _taskRepository.FindById(id);
            if (task == null) throw new Exception("Task không tồn tại.");
            return new TaskDetailDTO()
            {
                Id = task.Id,
                Name = task.Name,
                StartDateDeadline = task.StartDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                EndDateDeadline = task.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                FinishedDate = task.FinishedDate == null ? "-" : task.FinishedDate.Value.ToString("MMMM d, yyyy | hh:mm tt"),
                ImpotantLevel = task.ImpotantLevel.ToString(),
                EstimatedDays = task.EstimatedDays + " ngày",
                Status = task.Status.ToString(),
                Description = task.Description
            };
        }

        public TaskRecordDTO UpdateTask(TaskDTOForUpdating taskDTO, Guid userId)
        {
            BusinessObject.Models.Task task = _taskRepository.FindById(taskDTO.Id);
            if (task == null) throw new Exception("Task không tồn tại.");
            if (_memberRepository.FindByUserIdAndGroupId(userId, task.GroupId) == null)
                throw new Exception("Member không thuộc group hoặc 1 trong member và group không tồn tại.");
            foreach (Guid assignedFor in taskDTO.AssignedForIds)
            {
                if (_memberRepository.FindByUserIdAndGroupId(assignedFor, task.GroupId) == null)
                    throw new Exception("Member không thuộc group hoặc 1 trong member và group không tồn tại.");
            }

            _taskRepository.UpdateTask(taskDTO, userId);

            List<Guid> currentAssignedForIds = _assignedTaskRepository.FindByTaskId(task.Id).Select(a => a.AssignedForId).ToList();
            List<Guid> newAssignedForIds = taskDTO.AssignedForIds;
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
                throw new Exception("Member không thuộc group hoặc 1 trong member và group không tồn tại.");

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
