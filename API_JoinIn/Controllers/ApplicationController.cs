using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("applications")]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;
        private readonly IJwtService _jwtService;

        public ApplicationController(IApplicationService applicationService, IJwtService jwtService)
        {
            _applicationService = applicationService;
            _jwtService = jwtService;
        }

        [HttpPost]
        public IActionResult SendApplication(SentApplicationDTO sentApplicationDTO)
        {
            CommonResponse response = new CommonResponse();
            try
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
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid newApplicationId = _applicationService.CreateApplication(userId, sentApplicationDTO);
                    if (newApplicationId != Guid.Empty) scope.Complete();
                    else throw new Exception("Create application failed");
                    response.Data = newApplicationId;
                    response.Message = "Create application success";
                    response.Status = StatusCodes.Status200OK;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return Ok(response);
            }
        }

        [HttpPut]
        public IActionResult ConfirmApplication(ConfirmedApplicationDTO confirmedApplicationDTO)
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
                    Guid? applicationId = _applicationService.ConfirmApplication(userId, confirmedApplicationDTO);
                    if (applicationId != null) scope.Complete();
                    else throw new Exception("Confirm application failed");
                    response.Data = applicationId;
                    response.Message = "Confirm application success";
                    response.Status = StatusCodes.Status200OK;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Status = StatusCodes.Status500InternalServerError;
                return Ok(response);
            }
        }
    }
}
