using API_JoinIn.Utils.Email;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.Google;
using BusinessObject.DTOs.User;
using BusinessObject.Models;
using DataAccess.Services;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Transactions;

namespace API_JoinIn.Controllers
{
    [ApiController]
    [Route("oauth2")]
    public class Oauth2Controller : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IUserService userService;
        private readonly IEmailService emailService;
        private readonly IAuthenticateService authenticateService;

        public Oauth2Controller(IConfiguration configuration, IUserService userService, IEmailService emailService, IAuthenticateService authenticateService)
        {
            _configuration = configuration;
            this.userService = userService;
            this.emailService = emailService;
            this.authenticateService = authenticateService;
        }

        [HttpGet]
        [Route("sign-in")]
        public IActionResult Index()
        {

            var clientId = _configuration["Authentication:Google:ClientId"];
            var clientSecret = _configuration["Authentication:Google:ClientSecret"];
            var authenLink = _configuration["Authentication:Google:authenLink"];
            var redirectUrl = _configuration["Authentication:Google:redirect_uri"];
            // Redirect đến Google để xác thực đăng nhập
            var redirectAuthenLink = authenLink +
                "?client_id=" + clientId +
                "&redirect_uri=" + redirectUrl +
                "&clientSecret=" + clientSecret +
                "&response_type=code" +
                "&scope=email%20profile";

            return Ok(redirectAuthenLink);
        }

        [HttpGet("call-back")]
        public async Task<IActionResult> GoogleResponse()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                string code = HttpContext.Request.Query["code"];
                var clientId = _configuration["Authentication:Google:ClientId"];
                var clientSecret = _configuration["Authentication:Google:ClientSecret"];
                string redirectUri = "https://localhost:3000/oauth2/call-back";
                string token;

                // Yêu cầu thông tin truy cập từ Google
                var client = new HttpClient();
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                });
                var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", content);
                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<GoogleTokenResponse>(tokenContent);

                // Yêu cầu thông tin người dùng từ Google
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenData.AccessToken);
                var userResponse = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                var userContent = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonSerializer.Deserialize<GoogleUserResponse>(userContent);

                // check duplicated email
                if (await userService.checkDuplicatedEmail(userData.Email)) { 

                    // nếu user đã tồn tại, tạo token
                    token =  authenticateService.authenticateByGoogleOauth2(userData.Email);
                    commonResponse.Data = $"Token: {token}";
                    commonResponse.Status = 200 ;
                    return (Ok(commonResponse));
                  }

                else
                {
                    // Sử dụng thông tin người dùng ở đây
                    UserRequestDTO userRequestDTO = new UserRequestDTO();
                    userRequestDTO.FullName = userData.Name;
                    userRequestDTO.Email = userData.Email;
                    userRequestDTO.Password = "";
                    userRequestDTO.Gender = true;
                    userRequestDTO.BirthDay = new DateTime(1975, 4, 30);
                    userRequestDTO.Phone = "";
                    userRequestDTO.Description = "";
                    userRequestDTO.Skill = "";
                    userRequestDTO.OtherContact = "";
                    userRequestDTO.Avatar = userData.Picture;
                    userRequestDTO.Theme = "";
                    userRequestDTO.Status = BusinessObject.Enums.UserStatus.UNVERIFIED;
                    userRequestDTO.MajorIdList = null;



                    var json = JsonSerializer.Serialize(userRequestDTO);

                    var inputUser = JsonSerializer.Deserialize<User>(json);

                    using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                    {

                        var user = userService.AddUser(inputUser);

                        if (user == null) throw new Exception();

                        // đẩy user lên firebase
                        var auth = FirebaseAuth.DefaultInstance;
                        var userRecord = await auth.CreateUserAsync(new UserRecordArgs
                        {
                            Email = user.Email,
                            Password = user.Password,
                            EmailVerified = false
                        });

                        //if (user.Email != null)
                        //{
                        //    // gửi email xác nhận
                        //    emailService.SendConfirmationEmail(user.Email);
                        //}
                        //else throw new Exception("Email không tồn tại");


                        //  bool isEmailVerified = userData.VerifiedEmail;
                        //string pictureUrl = userData.Picture;
                        //  string locale = userData.Locale;
                        //  string hd = userData.Hd;
                        scope.Complete(); // commit transaction

                        //tạo token
                        // khi người dùng đăng nhập bằng token vừa tạo => đẩy người dùng về trang điền thông tin
                        token = authenticateService.authenticateByGoogleOauth2(userData.Email);
                        commonResponse.Data = $"Token: {token}";
                        commonResponse.Status = 200;
                        return (Ok(commonResponse));


                    }
                }
            }
            catch (Exception e)
            {
                commonResponse.Data = "Có lỗi xảy ra";
                commonResponse.Status = 500;
                return StatusCode(StatusCodes.Status500InternalServerError, commonResponse);
            }
        }
    }
}
