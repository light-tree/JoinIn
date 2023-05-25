using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccess.Repositories.Implements
{
    public class TaskRepository : ITaskRepository
    {
        private readonly Context _context;

        public TaskRepository(Context context)
        {
            _context = context;
        }

        public BusinessObject.Models.Task CreateTask(TaskDTOForCreating task, Guid createdById)
        {
            BusinessObject.Models.Task created = new BusinessObject.Models.Task();
            created.Name = task.Name;
            created.StartDateDeadline = task.StartDateDeadline;
            created.EndDateDeadline = task.EndDateDeadline;
            created.ImpotantLevel = task.ImpotantLevel;
            created.EstimatedDays = task.EstimatedDays;
            created.Description = task.Description;
            created.Status = BusinessObject.Enums.TaskStatus.NOT_STARTED_YET;
            created.CreatedById = createdById;
            created.GroupId = task.GroupId;
            created.MainTaskId = task.MainTaskId;

            _context.Tasks.Add(created);
            _context.SaveChanges();
            return created;
        }

        public CommonResponse FilterTasks(Guid userId, string name, int? pageSize, int? page)
        {
            List<TaskRecordDTO> taskDTOs = new List<TaskRecordDTO>();
            List<BusinessObject.Models.Task> tasks = _context.Tasks.Include(t => t.CreatedBy).Include(t => t.AssignedTasks).ThenInclude(a => a.AssignedFor).ThenInclude(m => m.User).
                Where(t => (t.Name.Contains(name) || string.IsNullOrEmpty(name)) && t.AssignedTasks.Any(a => a.AssignedFor.UserId == userId)).ToList();
            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();

            pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
            pagination.CurrentPage = page == null ? 1 : page.Value;
            pagination.Total = tasks.Count;

            tasks = tasks.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            foreach (BusinessObject.Models.Task task in tasks)
            {
                taskDTOs.Add(new TaskRecordDTO
                {
                    Id = task.Id,
                    Name = task.Name,
                    EndDateDeadline = task.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    ImpotantLevel = task.ImpotantLevel.ToString(),
                    EstimatedDays = task.EstimatedDays.ToString(),
                    Status = task.Status.ToString(),
                    CreatedBy = new UserDTOForTaskList
                    {
                        Id = task.CreatedBy.Id,
                        FullName = task.CreatedBy.FullName,
                        Avatar = task.CreatedBy.Avatar
                    },
                    AssignedFor = task.AssignedTasks.Select(a => new UserDTOForTaskList
                    {
                        Id = a.AssignedFor.User.Id,
                        FullName = a.AssignedFor.User.FullName,
                        Avatar = a.AssignedFor.User.Avatar,
                        Majors = _context.ApplicationMajors.Include(am => am.Application).Include(am => am.Major).
                            Where(am => am.Application.UserId == a.AssignedFor.UserId && am.Application.GroupId == a.AssignedFor.GroupId).Select(am => am.Major.Name).ToList(),
                    }).ToList(),
            });
            }

            response.Data = taskDTOs;
            response.Pagination = pagination;
            response.Message = "Filter task list success.";
            response.Status = 200;

            return response;
        }

        public BusinessObject.Models.Task FindByName(string name)
        {
            return _context.Tasks.FirstOrDefault(t => t.Name.Equals(name));
        }

        public TaskRecordDTO FindRecordById(Guid updatedTaskId)
        {
            List<TaskRecordDTO> taskDTOs = new List<TaskRecordDTO>();
            BusinessObject.Models.Task task = _context.Tasks.Include(t => t.CreatedBy).Include(t => t.AssignedTasks).
                ThenInclude(a => a.AssignedFor).ThenInclude(m => m.User).FirstOrDefault(t => t.Id == updatedTaskId);
            return new TaskRecordDTO
                {
                    Id = task.Id,
                    Name = task.Name,
                    EndDateDeadline = task.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    ImpotantLevel = task.ImpotantLevel.ToString(),
                    EstimatedDays = task.EstimatedDays.ToString(),
                    Status = task.Status.ToString(),
                    CreatedBy = new UserDTOForTaskList
                    {
                        Id = task.CreatedBy.Id,
                        FullName = task.CreatedBy.FullName,
                        Avatar = task.CreatedBy.Avatar
                    },
                    AssignedFor = task.AssignedTasks.Select(a => new UserDTOForTaskList
                    {
                        Id = a.AssignedFor.User.Id,
                        FullName = a.AssignedFor.User.FullName,
                        Avatar = a.AssignedFor.User.Avatar,
                        Majors = _context.ApplicationMajors.Include(am => am.Application).Include(am => am.Major).
                            Where(am => am.Application.UserId == a.AssignedFor.UserId && am.Application.GroupId == a.AssignedFor.GroupId).Select(am => am.Major.Name).ToList(),
                    }).ToList(),
                };
        }

        public BusinessObject.Models.Task FindById(Guid id)
        {
            return _context.Tasks.FirstOrDefault(t => t.Id == id);
        }

        public int UpdateTask(TaskDTOForUpdating task, Guid userId)
        {
            BusinessObject.Models.Task updatedTask = FindById(task.Id);
            updatedTask.Name = task.Name;
            updatedTask.StartDateDeadline = task.StartDateDeadline;
            updatedTask.EndDateDeadline = task.EndDateDeadline;
            updatedTask.ImpotantLevel = task.ImpotantLevel;
            updatedTask.EstimatedDays = task.EstimatedDays;
            updatedTask.Description = task.Description;
            updatedTask.Status = task.Status;
            if(updatedTask.Status == BusinessObject.Enums.TaskStatus.FINISHED)
            {
                updatedTask.FinishedDate = DateTime.Now;
            }
            else
            {
                updatedTask.FinishedDate = null;
            }

            _context.Tasks.Update(updatedTask);
            return _context.SaveChanges();
        }

        public List<BusinessObject.Models.Task> FindByMainTaskId(Guid taskId)
        {
            return _context.Tasks.Where(t => t.MainTaskId == taskId).ToList();
        }

        public int DeleteByTaskId(Guid taskId)
        {
            BusinessObject.Models.Task deletedTask = FindById(taskId);
            _context.Tasks.Remove(deletedTask);
            return _context.SaveChanges();
        }

        public BusinessObject.Models.Task FindByIdAndUserId(Guid id, Guid userId)
        {
            BusinessObject.Models.Task task = _context.Tasks.FirstOrDefault(t => t.Id == id);
            Guid groupIdTheTaskBelong = task.GroupId;
            if (_context.Members.FirstOrDefault(m => m.GroupId == groupIdTheTaskBelong && m.UserId == userId && m.LeftDate == null) != null)
            {
                return task;
            }
            else throw new Exception("Member does not belong to the group this task belong.");
        }

        public int UpdateTaskStatus(TaskDTOForUpdatingStatus taskDTO, Guid userId)
        {
            BusinessObject.Models.Task updatedTask = FindById(taskDTO.Id);
            updatedTask.Status = taskDTO.Status;
            if (updatedTask.Status == BusinessObject.Enums.TaskStatus.FINISHED)
            {
                updatedTask.FinishedDate = DateTime.Now;
            }
            else
            {
                updatedTask.FinishedDate = null;
            }
            _context.Tasks.Update(updatedTask);
            return _context.SaveChanges();
        }
    }
}
