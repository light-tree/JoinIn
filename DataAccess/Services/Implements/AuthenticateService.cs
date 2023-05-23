using BusinessObject.DTOs.User;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUserRepository userRepository;
        private readonly IJwtService jwtService;

        public AuthenticateService(IUserRepository userRepository, IJwtService jwtService)
        {
            this.userRepository = userRepository;
            this.jwtService = jwtService;
     
        }

        public string authenticate(LoginDTO loginDTO)
        {
            User account = userRepository.FindAccountByEmail(loginDTO.UserName);
            bool checkPassword = false;
            if (account == null)
            {
                return null;
            }
            else
            {

                checkPassword = PasswordHasher.Verify(loginDTO.Password, account.Password);
                if (!checkPassword)
                {
                    return null;
                }

            }

            //if (account.Status == BussinessObject.Status.AccountStatus.INACTIVE)
            //{
            //    throw new TaskCanceledException("Tài khoản đã bị khóa");
            //}

            string role = null;
            role = account.GetType().Name.ToString();
           
            string token = jwtService.GenerateJwtToken(account, role);
            return token;
        }

        // Dành cho đăng nhập bằng google không cần mật khẩu

        public string authenticateByGoogleOauth2(string email)
        {
            User account = userRepository.FindAccountByEmail(email);
            if (account == null)
            {
                return null;
            }
            else
            {
                //if (account.Status == BussinessObject.Status.AccountStatus.INACTIVE)
                //{
                //    throw new TaskCanceledException("Tài khoản đã bị khóa");
                //}

                string role = null;
                role = account.GetType().Name.ToString();

                string token = jwtService.GenerateJwtToken(account, role);
                return token;

            }

        }
    }
}
