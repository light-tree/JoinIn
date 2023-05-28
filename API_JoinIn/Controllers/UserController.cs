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
using Newtonsoft.Json.Linq;
using BusinessObject.Enums;
using System.Data;
using Microsoft.Extensions.Hosting.Internal;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using Google.Cloud.Storage.V1;
using System.Text;
using Google.Apis.Auth.OAuth2;
using Firebase.Storage;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService userService;
        private readonly IMajorService majorService;
        private readonly IEmailService emailService;
        private readonly IJwtService jwtService;
        private readonly IUserMajorService userMajorService;
        private readonly StorageClient storageClient;


        public UserController(IConfiguration configuration, IUserService userService, IMajorService majorService, IEmailService emailService, IJwtService jwtService, IUserMajorService userMajorService, StorageClient storageClient)
        {
            _configuration = configuration;
            this.userService = userService;
            this.majorService = majorService;
            this.emailService = emailService;
            this.jwtService = jwtService;
            this.userMajorService = userMajorService;
            this.storageClient = storageClient;
        }


        //Register for user
        [HttpPost]
        [Route("/register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            CommonResponse commonResponse = new CommonResponse();

            if (await userService.CheckDuplicatedEmail(registerDTO.Email))
            {
                commonResponse.Message = "Email đã tồn tại";
                commonResponse.Status = 400;
                return BadRequest(commonResponse);
            }

            User user = new User();
            user.Email = registerDTO.Email;
            user.FullName = "";
            user.Password = registerDTO.Password;
            user.BirthDay = new DateTime(1975, 4, 30);
            user.Gender = true;
            user.Description = "";
            user.OtherContact = "";
            user.Skill = "";
            user.Avatar = "";
            user.Theme = "";
            user.Status = UserStatus.UNVERIFIED;
            user.IsAdmin = false;
            //set up verify token
            string role = "";
            if (user.IsAdmin)
            {
                role = "Admin";
            }
            else
            {
                role = "User";
            }
            var token = jwtService.GenerateJwtToken(user, role);
            user.Token = token;
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    userService.AddUser(user);

                } catch
                {
                    commonResponse.Message = "There is error";
                    commonResponse = new CommonResponse();
                    return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
                }
               
                string baseUrlVerify = _configuration["BaseVerifyLink"]; 
                var queryString = $"{token}";
                var url = $"{baseUrlVerify}/{queryString}";
                emailService.SendConfirmationEmail(user.Email, url);
                scope.Complete();
                // success
                commonResponse.Message = "Add user successfully";
                commonResponse.Status = 200;
                return Ok(commonResponse);
            }
        }

        [HttpGet("send-email-verification")]
        public async Task<IActionResult> SendEmailVerification(String email)
        {
            try
            {
                // Gửi email chứa link xác nhận đến địa chỉ email đã được chỉ định
                // (thực hiện bằng cách sử dụng thư viện gửi email của bạn)
                //var configuration = new ConfigurationBuilder()
                //    .SetBasePath(Directory.GetCurrentDirectory())
                //    .AddJsonFile("appsettings.json")
                //    .Build();
                string baseUrlVerify = _configuration["BaseVerifyLink"];
                CommonResponse commonResponse = new CommonResponse();

                User user = await userService.FindUserByEmail(email);
                if (user != null && user.Email == email)
                {
                    string role = "";
                    if (user.IsAdmin)
                        role = "Admin";
                    else
                        role = "User";
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var jwtToken = jwtService.GenerateJwtToken(user, role);
                        user.Token = jwtToken;
                        var rs = await userService.UpdateUser(user);
                        var queryString = $"{jwtToken}";
                        var url = $"{baseUrlVerify}/{queryString}";
                        await emailService.SendConfirmationEmail(user.Email, url);
                        commonResponse.Message = "Email verification were sent successfully";
                        commonResponse.Status = 200;
                        scope.Complete();
                        return Ok(commonResponse);
                       
                    }
                }

                else
                {
                    commonResponse.Message = "Email not found";
                    commonResponse.Status = 400;
                    return BadRequest(commonResponse);

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        [HttpGet("vertified-user/{token}")]
        public async Task<IActionResult> IsEmailVerified(string token )
        {
            CommonResponse commonResponse = new CommonResponse();
            User user;

           
                user = await userService.FindUserByToken(token);
            
            if(user == null)
            {
                commonResponse.Message = "Unauthorize";
                commonResponse.Status = StatusCodes.Status401Unauthorized;
                return Unauthorized(commonResponse);
            }


            if (user.Token == token)
            {
                string role = "";
                if (user.IsAdmin)
                    role = "Admin";
                else
                    role = "User";
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        // tạo jwt mới
                        var jwtToken = jwtService.GenerateJwtToken(user, role);
                        user.Token = jwtToken;

                        var rs = await userService.UpdateUser(user);
                        if (rs == null)
                        {
                            throw new Exception();
                        }

                        scope.Complete();
                        commonResponse.Message = "Successfully";
                        commonResponse.Data = jwtToken;
                        return Ok(commonResponse);

                    } catch
                    {
                        commonResponse.Message = "Internal Server Error";
                        return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);

                    }
                    scope.Complete();
                }
            }
            else {
                commonResponse.Message = "Unauthorize";
                commonResponse.Status = StatusCodes.Status401Unauthorized;
                return Unauthorized(commonResponse); 
            }
            //CommonResponse commonResponse = new CommonResponse();
            //var baseUrl = "https://localhost:3000/update-profile";

            //try
            //{
            //    User user = await userService.FindUserByEmail(email);
            //    string role;
            //    if (user != null)
            //    {

            //        if (user.IsAdmin)
            //        {
            //            role = "Admin";
            //        }
            //        else
            //        {
            //            role = "User";
            //        }
            //        var token = jwtService.GenerateJwtToken(user, role);

            //        // update token trong bảng users


            //        var queryString = $"?token={token}";
            //        var url = $"{baseUrl}{queryString}";
            //        commonResponse.Message = "Email where send";
            //        commonResponse.Status = 200;


            //        await emailService.SendConfirmationEmail(email, url);


            //        return Ok(commonResponse);

            //    }
            //    else throw new Exception();


            //}
            //catch 
            //{
            //    commonResponse.Message = "Internal server error";
            //    commonResponse.Status = 500;
            //    return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);

        //}

    }
        [Authorize]
        [HttpPost("update-profile")]
        public async Task<IActionResult> UpdateProfile(UserRequestDTO userRequestDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            List<UserMajor> majorsUser = new List<UserMajor>();
            string userId = "";
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal server error");
                }
                //rollback khi có lỗi xảy ra
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = await userService.FindUserByGuid(Guid.Parse(userId));
                    user.FullName = userRequestDTO.FullName;
                    user.BirthDay = userRequestDTO.BirthDay;
                    user.Gender = userRequestDTO.Gender;
                    user.Description = userRequestDTO.Description;
                    user.OtherContact = userRequestDTO.OtherContact;
                    user.Skill = userRequestDTO.Skill;
                    user.Avatar = userRequestDTO.Avatar;
                    user.Theme = userRequestDTO.Theme;
                    user.Status = UserStatus.ACTIVE;
                    user.IsAdmin = false;
                  

                    // check danh sách major
                    foreach (Guid id in userRequestDTO.MajorIdList)
                    {
                        var tmp = majorService.FindMajorById(id);
                        if (tmp != null)
                        {
                           await userMajorService.UpdateUserMajor(id, user.Id);
                          
                        }
                        else throw new Exception("Your major is invalid");
                    }

                    bool check = await userService.UpdateUser(user);

                    if (user == null || !check) throw new Exception();

                    // đẩy account lên firebase

                    //var auth = FirebaseAuth.DefaultInstance;
                    //var userRecord = await auth.CreateUserAsync(new UserRecordArgs
                    //{
                    //    Email = user.Email,
                    //    Password = user.Password,
                    //    EmailVerified = false
                    //});

                    scope.Complete(); // commit transaction
                }

                commonResponse.Status = 200;
                commonResponse.Message = "Update successfully";

                // Quay về trang login
                return Ok(commonResponse);

            }
            catch 
            {
                commonResponse.Data = "Internal server error";
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }

        [HttpGet("send-email-verification/forget-password")]
        public async Task<IActionResult> SendEmailRecoveryPassword(String email)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                // Gửi email chứa link xác nhận đến địa chỉ email đã được chỉ định
                // (thực hiện bằng cách sử dụng thư viện gửi email của bạn)
               
                string baseUrlVerify = _configuration["BaseVerifyLink"];
            

                User user = await userService.FindUserByEmail(email);
                if (user != null && user.Email == email)
                {
                    string role = "";
                    if (user.IsAdmin)
                        role = "Admin";
                    else
                        role = "User";
                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var jwtToken = jwtService.GenerateJwtToken(user, role);
                        user.Token = jwtToken;
                        var rs = await userService.UpdateUser(user);
                        var queryString = $"{jwtToken}";
                        var url = $"{baseUrlVerify}/{queryString}";
                        await emailService.SendRecoveryPasswordEmail(user.Email, url);
                        commonResponse.Message = "Email  were sent successfully";
                        commonResponse.Status = 200;
                        scope.Complete();
                        return Ok(commonResponse);

                    }
                }

                else
                {
                    commonResponse.Message = "Email not found";
                    commonResponse.Status = 400;
                    return BadRequest(commonResponse);

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có

                commonResponse.Data = "Internal server error";
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }


        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> UpdatePassword(String password)
        {

            CommonResponse commonResponse = new CommonResponse();
         
            string userId = "";
            try
            {
                var jwtToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var decodedToken = jwtService.DecodeJwtToken(jwtToken);
                if (decodedToken != null)
                {
                    var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "Id");
                    if (userIdClaim != null)
                    {
                        userId = userIdClaim.Value;
                        // Do something with user ID here
                    }
                    else throw new Exception("Internal server error");
                }
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    var user = await userService.FindUserByGuid(Guid.Parse(userId));
                    user.Password = PasswordHasher.Hash(password);
                    await userService.UpdateUser(user);
                    scope.Complete();
                    commonResponse.Status = 200;
                    commonResponse.Message = "Change password successfully";
                    return Ok(commonResponse);

                }
            } catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal server error";
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }


         }


        [HttpPost("upload")]
       
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");
           


            // Read the uploaded file into a memory stream
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                // Authenticate with Firebase using a service account key
               
                // Define the bucket and object names
                var bucketName = "gs://joinin-387312.appspot.com";
                var objectName = "uploads/" + file.FileName;

                // Upload the file to Firebase Storage
                storageClient.UploadObject(bucketName, objectName, null, memoryStream);
            }

            return Ok();
        }


    }

 
}
