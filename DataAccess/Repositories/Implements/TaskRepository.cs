﻿using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Cache;
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

        public BusinessObject.Models.Task FindByName(string name)
        {
            return _context.Tasks.FirstOrDefault(t => t.Name.Equals(name));
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

        public TaskDetailDTO FindByIdAndUserId(Guid id, Guid userId)
        {
            BusinessObject.Models.Task task = _context.Tasks
                .Include(t => t.Group)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTasks).ThenInclude(at => at.AssignedFor).ThenInclude(af => af.User)
                .Include(t => t.Comments)
                .FirstOrDefault(t => t.Id == id);

            if (task == null) 
                throw new Exception("Task is not exist.");

            Guid groupIdTheTaskBelong = task.GroupId;
            if (_context.Members.FirstOrDefault(m => m.GroupId == groupIdTheTaskBelong && m.UserId == userId && m.LeftDate == null) == null)
                throw new Exception("Member does not belong to the group this task belong.");

            //Set all values for task detail except sub-tasks and comments
            TaskDetailDTO taskDetailDTO = new TaskDetailDTO()
            {
                Id = task.Id,
                Name = task.Name,
                StartDateDeadline = task.StartDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                EndDateDeadline = task.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                FinishedDate = task.FinishedDate == null ? "-" : task.FinishedDate.Value.ToString("MMMM d, yyyy | hh:mm tt"),
                ImpotantLevel = task.ImpotantLevel.ToString(),
                EstimatedDays = task.EstimatedDays,
                Description = task.Description,
                Status = task.Status.ToString(),
                Group = new GroupDTOForToDoTaskRecord
                {
                    Id = task.Group.Id,
                    Name = task.Group.Name,
                    Avatar = task.Group.Avatar,
                },
                CreatedBy = new UserDTOForTaskList
                {
                    Id = task.CreatedBy.Id,
                    FullName = task.CreatedBy.FullName,
                    Avatar = task.CreatedBy.Avatar,
                },
                AssignedFor = task.AssignedTasks.Select(a => new UserDTOForTaskList
                {
                    Id = a.AssignedFor.User.Id,
                    FullName = a.AssignedFor.User.FullName,
                    Avatar = a.AssignedFor.User.Avatar
                }).ToList(),
            };

            //Get sub-tasks for task detail
            List<BusinessObject.Models.Task> subTasks = _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTasks).ThenInclude(a => a.AssignedFor).ThenInclude(m => m.User)
                .Where(t => t.MainTaskId == task.Id).ToList();

            List<GroupTaskRecordDTO> subTasksDTO = new List<GroupTaskRecordDTO>();
            foreach (BusinessObject.Models.Task subTask in subTasks)
            {
                subTasksDTO.Add(new GroupTaskRecordDTO
                {
                    Id = subTask.Id,
                    Name = subTask.Name,
                    StartDateDeadline = subTask.StartDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    EndDateDeadline = subTask.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    ImpotantLevel = subTask.ImpotantLevel.ToString(),
                    EstimatedDays = subTask.EstimatedDays,
                    Status = subTask.Status.ToString(),
                    CreatedBy = new UserDTOForTaskList
                    {
                        Id = subTask.CreatedBy.Id,
                        FullName = subTask.CreatedBy.FullName,
                        Avatar = subTask.CreatedBy.Avatar
                    },
                    AssignedFor = subTask.AssignedTasks.Select(a => new UserDTOForTaskList
                    {
                        Id = a.AssignedFor.User.Id,
                        FullName = a.AssignedFor.User.FullName,
                        Avatar = a.AssignedFor.User.Avatar
                    }).ToList(),
                });
            }

            //Get comments for task detail
            List<CommentRecordDTO> comments = _context.Comments
                .Where(c => c.TaskId == task.Id).Select(c => new CommentRecordDTO
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedDate = c.CreatedDate.ToString("MMMM d, yyyy | hh:mm tt")
                }).ToList();

            //Set sub-tasks and comments for task detail
            taskDetailDTO.Comments = comments;
            taskDetailDTO.SubTasks = subTasksDTO;
            return taskDetailDTO;
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

        public CommonResponse FilterToDoTasks(Guid userId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            List<ToDoTaskRecordDTO> taskDTOs = new List<ToDoTaskRecordDTO>();
            List<BusinessObject.Models.Task> tasks = _context.Tasks
                .Include(t => t.Group)
                .Include(t => t.AssignedTasks).ThenInclude(a => a.AssignedFor).ThenInclude(m => m.User)
                .Where(t => t.MainTaskId == null && (t.Name.Contains(name) || string.IsNullOrEmpty(name)) && t.AssignedTasks.Any(a => a.AssignedFor.UserId == userId)).ToList();

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    tasks = tasks.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    tasks = tasks.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }
            else
                tasks = tasks.OrderBy(t => CalculateOrder(t.EndDateDeadline, t.EstimatedDays, t.ImpotantLevel)).ToList();

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
            pagination.CurrentPage = page == null ? 1 : page.Value;
            pagination.Total = tasks.Count;

            tasks = tasks.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            foreach (BusinessObject.Models.Task task in tasks)
            {
                taskDTOs.Add(new ToDoTaskRecordDTO
                {
                    Id = task.Id,
                    Name = task.Name,
                    StartDateDeadline = task.StartDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    EndDateDeadline = task.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    ImpotantLevel = task.ImpotantLevel.ToString(),
                    EstimatedDays = task.EstimatedDays,
                    Status = task.Status.ToString(),
                    Group = new GroupDTOForToDoTaskRecord
                    {
                        Id = task.Group.Id,
                        Name = task.Group.Name,
                        Avatar = task.Group.Avatar,
                    },
                    AssignedFor = task.AssignedTasks.Select(a => new UserDTOForTaskList
                    {
                        Id = a.AssignedFor.User.Id,
                        FullName = a.AssignedFor.User.FullName,
                        Avatar = a.AssignedFor.User.Avatar,
                    }).ToList(),
                });
            }

            response.Data = taskDTOs;
            response.Pagination = pagination;
            response.Message = "Filter task list success.";
            response.Status = 200;

            return response;
        }

        public CommonResponse FilterGroupTasks(Guid userId, Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            List<GroupTaskRecordDTO> taskDTOs = new List<GroupTaskRecordDTO>();
            List<BusinessObject.Models.Task> tasks = _context.Tasks
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedTasks).ThenInclude(a => a.AssignedFor).ThenInclude(m => m.User).
                Where(t => t.MainTaskId == null && (t.Name.Contains(name) || string.IsNullOrEmpty(name)) && t.GroupId == groupId).ToList();

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    tasks = tasks.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    tasks = tasks.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }
            else
                tasks = tasks.OrderBy(t => CalculateOrder(t.EndDateDeadline, t.EstimatedDays, t.ImpotantLevel)).ToList();

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
            pagination.CurrentPage = page == null ? 1 : page.Value;
            pagination.Total = tasks.Count;

            tasks = tasks.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            foreach (BusinessObject.Models.Task task in tasks)
            {
                taskDTOs.Add(new GroupTaskRecordDTO
                {
                    Id = task.Id,
                    Name = task.Name,
                    StartDateDeadline = task.StartDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    EndDateDeadline = task.EndDateDeadline.ToString("MMMM d, yyyy | hh:mm tt"),
                    ImpotantLevel = task.ImpotantLevel.ToString(),
                    EstimatedDays = task.EstimatedDays,
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
                        Avatar = a.AssignedFor.User.Avatar
                    }).ToList(),
                });
            }

            response.Data = taskDTOs;
            response.Pagination = pagination;
            response.Message = "Filter task list success.";
            response.Status = 200;

            return response;
        }

        static object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(obj);
        }

        public double CalculateOrder(DateTime endDateDeadline, int estimatedDays, ImportantLevel importantLevel)
        {
            double remainingDays = (endDateDeadline - DateTime.Now).TotalDays - estimatedDays;
            double importantLevelIndex = Array.IndexOf(Enum.GetValues(typeof(ImportantLevel)), importantLevel) + 1;
            double orderedResult = remainingDays <= 0 ?
                (remainingDays * importantLevelIndex) :
                (remainingDays * (1 / importantLevelIndex));
            return orderedResult;
        }
    }
}
