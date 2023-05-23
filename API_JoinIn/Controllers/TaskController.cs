using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController
    {
        private readonly Context _context;
        private readonly ITaskService _taskService;

        public TaskController(Context context, ITaskService taskService)
        {
            _context = context;
            _taskService = taskService;
        }

        [HttpGet]
        [Route("initial")]
        public IActionResult Initial()
        {
            Group group = new Group();
            group.Name = "a";
            group.CreatedDate = DateTime.Now;
            group.GroupSize = 10;
            group.MemberCount = 1;
            group.SchoolName = "b";
            group.ClassName = "c";
            group.SubjectName = "d";
            group.Description = "e";
            group.Skill = "f";
            group.Status = GroupStatus.ACTIVE;
            group.CurrentMilestone = null;
            _context.Add(group);
            _context.SaveChanges();

            Milestone milestone = new Milestone();
            milestone.Name = "a";
            milestone.Description = "b";
            milestone.Order = 1;
            milestone.GroupId = _context.Groups.FirstOrDefault(g => g.Name == "a").Id;
            _context.Add(milestone);
            _context.SaveChanges();

            group.CurrentMilestoneId = _context.Milestones.FirstOrDefault(m => m.Name == "a").Id;
            _context.Update(group);
            _context.SaveChanges();

            for (int i = 1; i < 4; i++)
            {
                User user = new User();
                user.FullName = "user fullname " + i;
            }

            CommonResponse response = new CommonResponse();
            response.Status = StatusCodes.Status200OK;;
            response.Message = "Initiated";
            return new OkObjectResult(response);
        }

        [HttpGet]
        public IActionResult FilterTasks(string? name, int? pageSize, int? page)
        {
            Guid userId = Guid.NewGuid();
            CommonResponse response = new CommonResponse();
            try
            {
                return new OkObjectResult(_taskService.FilterTasks(userId, name, pageSize, page));
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetTaskDetail(Guid id)
        {
            Guid userId = Guid.NewGuid();
            CommonResponse response = new CommonResponse();
            try
            {
                response.Status = StatusCodes.Status200OK;;
                response.Message = "Lấy chi tiết task thành công.";
                response.Data = _taskService.GetDetailById(id);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        [HttpPost]
        public IActionResult CreateTask(TaskDTOForCreating task)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid createdById = Guid.NewGuid();
                TaskRecordDTO taskRecordDTO = _taskService.CreateTask(task, createdById);
                if (taskRecordDTO != null)
                {
                    response.Status = StatusCodes.Status200OK;;
                    response.Message = "Tạo task thành công.";
                    response.Data = taskRecordDTO;
                    return new OkObjectResult(response);
                }
                else
                {
                    response.Status = StatusCodes.Status500InternalServerError;;
                    response.Message = "Tạo task không thành công.";
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError;;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        [HttpPut]
        public IActionResult UpdateTask(TaskDTOForUpdating task)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid userId = Guid.NewGuid();
                TaskRecordDTO taskRecordDTO = _taskService.UpdateTask(task, userId);
                if (taskRecordDTO != null)
                {
                    response.Status = StatusCodes.Status200OK;
                    response.Message = "Cập nhật task thành công.";
                    response.Data = taskRecordDTO;
                    return new OkObjectResult(response);
                }
                else
                {
                    response.Status = StatusCodes.Status500InternalServerError; ;
                    response.Message = "Cập nhật task không thành công.";
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError; ;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }

        [HttpDelete]
        public IActionResult DeleteTask(Guid taskId)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid userId = Guid.NewGuid();
                int result = _taskService.DeleteTask(taskId, userId);
                if (result != 0)
                {
                    response.Status = StatusCodes.Status200OK;
                    response.Message = "Xóa task thành công.";
                    return new OkObjectResult(response);
                }
                else
                {
                    response.Status = StatusCodes.Status500InternalServerError; ;
                    response.Message = "Xóa task không thành công.";
                    return new OkObjectResult(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = StatusCodes.Status500InternalServerError; ;
                response.Message = ex.Message;
                return new OkObjectResult(response);
            }
        }
    }
}
