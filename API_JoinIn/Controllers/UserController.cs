using BusinessObject.DTOs.User;
using BusinessObject.Models;
using DataAccess.Services;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Text.Json;
using System.Transactions;
using API_JoinIn.Utils.Email.Impl;
using DataAccess.Security;
using API_JoinIn.Utils.Email;
using Microsoft.AspNetCore.Authorization;
using BusinessObject.DTOs.Common;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        IConfiguration _configuration;
        IUserService userService;
        IMajorService majorService;
        IEmailService emailService;
      
   
        public UserController(IConfiguration configuration, IUserService userService, IMajorService majorService, IEmailService emailService)
        {
            _configuration = configuration;
            this.userService = userService;
            this.majorService = majorService;
            this.emailService = emailService;
        }


        //Register for user
        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(UserRequestDTO userRequestDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            List<UserMajor> majorsUser = new List<UserMajor>();
            try
            {

                //rollback khi có lỗi xảy ra
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // validate
                    // phone
                    //email
                    //major

                    var json = JsonSerializer.Serialize(userRequestDTO);

                    var inputUser = JsonSerializer.Deserialize<User>(json);

                    var user = userService.AddUser(inputUser);

                    // check danh sách major
                    foreach (Guid id in userRequestDTO.MajorIdList)
                    {
                        var tmp = majorService.FindMajorById(id);
                        if (tmp != null)
                        {

                            UserMajor u = new UserMajor();
                            u.MajorId = Guid.Parse(id); ;
                            u.UserId = user.Id;
                            majorsUser.Add(u);
                        }
                        else throw new Exception("major không tồn tại");
                    }
                    user.UserMajors = majorsUser;
                    bool check = await userService.UpdateUser(user);

                    if (user == null || !check) throw new Exception();

                    // đẩy account lên firebase

                    var auth = FirebaseAuth.DefaultInstance;
                    var userRecord = await auth.CreateUserAsync(new UserRecordArgs
                    {
                        Email = user.Email,
                        Password = user.Password,
                        EmailVerified = false
                    });

                    scope.Complete(); // commit transaction
                }

                commonResponse.Status = 200;
                commonResponse.Data = "Tạo thành công";

                // Quay về trang login
                return Ok(commonResponse);

            }
            catch (Exception ex)
            {
                commonResponse.Data = ex;
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

        [HttpGet("send-email-verification")]
        public async Task<IActionResult> SendEmailVerification(String email)
        {
            try
            {
                // Gửi email chứa link xác nhận đến địa chỉ email đã được chỉ định
                // (thực hiện bằng cách sử dụng thư viện gửi email của bạn)
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                //var smtpHost = configuration["SmtpConfig:Host"];
                //var smtpPort = int.Parse(configuration["SmtpConfig:Port"]);
                //var smtpUsername = configuration["SmtpConfig:Username"];
                //var smtpPassword = configuration["SmtpConfig:Password"];

                //var emailService = new EmailService(smtpHost, smtpPort, smtpUsername, smtpPassword);
                await emailService.SendConfirmationEmail(email);

                return Ok("Email verification sent successfully.");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpGet("check-email-is-vertified")]
        public async Task<bool> IsEmailVerified(string email)
        {
            try
            {
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                return user.EmailVerified;
            }
            catch (FirebaseAuthException)
            {
                // Handle exception
                return false;
            }
        }

    }
}
