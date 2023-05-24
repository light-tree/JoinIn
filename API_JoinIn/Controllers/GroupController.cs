using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("groups")]
    public class GroupController
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public CommonResponse CreateGroup(GroupDTOForCreating groupDTOForCreating)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid createrId = Guid.NewGuid();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid rs = _groupService.CreateGroup(createrId, groupDTOForCreating);
                    response.Data = rs;
                    response.Message = "Create group success.";
                    response.Status = StatusCodes.Status200OK;
                    scope.Complete();
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return response;
            }
        }
    }
}
