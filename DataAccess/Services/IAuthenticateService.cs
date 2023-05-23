using BusinessObject.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IAuthenticateService
    {
        public string authenticate(LoginDTO loginDTO);

        public string authenticateByGoogleOauth2(string email);
    }
}
