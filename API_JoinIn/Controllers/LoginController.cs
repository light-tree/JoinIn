using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.User;
using DataAccess.Security;
using DataAccess.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("auth")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticateService authenticateService;
        


        public LoginController(IUserService userService, IAuthenticateService authenticateService)
        {
            this.authenticateService = authenticateService;
         

        }

        [HttpPost]
        [Route("/authenticate")]
        public IActionResult Authenticate(LoginDTO loginDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {

                var token = authenticateService.authenticate(loginDTO);
                if (token == null)
                {
                    commonResponse.Message = "Tài khoản hoặc mật khẩu không chính xác";
                    return Unauthorized(commonResponse);

                }
                else
                {
                    return Ok(token);

                }
            }
            catch (TaskCanceledException ex)
            {

                commonResponse.Message = ex.Message;
                return StatusCode(StatusCodes.Status400BadRequest, commonResponse);
            }

            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

      
    }
}
