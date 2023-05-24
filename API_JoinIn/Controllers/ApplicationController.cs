using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
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

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        public IActionResult SendApplication(SentApplicationDTO sentApplicationDTO)
        {
            CommonResponse response = new CommonResponse();
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    Guid userId = Guid.NewGuid();
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
                    Guid userId = Guid.NewGuid();
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
