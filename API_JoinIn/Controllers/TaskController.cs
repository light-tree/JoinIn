﻿using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using DataAccess.Services.Implements;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IJwtService _jwtService;

        public TaskController(ITaskService taskService, IJwtService jwtService)
        {
            _taskService = taskService;
            _jwtService = jwtService;
        }

        [HttpGet]
        public IActionResult FilterTasks(Guid? groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            Guid userId = Guid.Empty;
            var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
            if (decodedToken != null)
            {
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                    // Do something with user ID here
                }
                else throw new Exception("Internal server error");
            }
            CommonResponse response = new CommonResponse();
            try
            {
                CommonResponse commonResponse = _taskService.FilterTasks(userId, groupId, name, pageSize, page, orderBy, value);
                return new OkObjectResult(commonResponse);
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
            Guid userId = Guid.Empty;
            var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
            if (decodedToken != null)
            {
                var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (userIdClaim != null)
                {
                    userId = Guid.Parse(userIdClaim.Value);
                    // Do something with user ID here
                }
                else throw new Exception("Internal server error");
            }
            CommonResponse response = new CommonResponse();
            try
            {
                response.Status = StatusCodes.Status200OK; ;
                response.Message = "Get task's detail success.";
                response.Data = _taskService.GetDetailById(id, userId);
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
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid createdById = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            createdById = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }

                    response.Data = _taskService.CreateTask(task, createdById);
                    response.Status = StatusCodes.Status200OK; ;
                    response.Message = "Create task success.";
                    scope.Complete();
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
        [Route("team-leaders")]
        public IActionResult UpdateTaskByTeamLeaders(TaskDTOForUpdating task)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }

                    response.Data = _taskService.UpdateTask(task, userId);
                    response.Status = StatusCodes.Status200OK;
                    response.Message = "Update task success.";
                    scope.Complete();
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

        [HttpPut]
        [Route("team-member")]
        public IActionResult UpdateTaskByTeamMember(TaskDTOForUpdatingStatus task)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }

                    response.Data = _taskService.UpdateTaskStatus(task, userId);
                    response.Status = StatusCodes.Status200OK;
                    response.Message = "Update task success.";
                    scope.Complete();
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
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.Empty;
                    var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                    if (decodedToken != null)
                    {
                        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                        if (userIdClaim != null)
                        {
                            userId = Guid.Parse(userIdClaim.Value);
                            // Do something with user ID here
                        }
                        else throw new Exception("Internal server error");
                    }
                    int result = _taskService.DeleteTask(taskId, userId);
                    if (result != 0)
                    {
                        response.Status = StatusCodes.Status200OK;
                        response.Message = "Delete task success.";
                        scope.Complete();
                        return new OkObjectResult(response);
                    }
                    else throw new Exception("Delete task not success");
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
