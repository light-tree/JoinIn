using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Authorize]
    [Route("groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IJwtService _jwtService;

        public GroupController(IGroupService groupService, IJwtService jwtService)
        {
            _groupService = groupService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public CommonResponse CreateGroup(GroupDTOForCreating groupDTOForCreating)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                Guid createrId = Guid.Empty;
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = _jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        createrId = Guid.Parse(userIdClaim.Value);
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal server error");
                }
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
