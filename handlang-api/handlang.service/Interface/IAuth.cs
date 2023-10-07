using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace handlang.service.Interface
{
    public interface IAuth
    {


        #region JWT
        string GenerateJwtToken(List<Claim> claims);

        string ReGenerateJwtToken(JwtSecurityToken jwt);

        bool IsAnonymous(List<Claim> claims);

        JwtSecurityToken ReadToken(string token);

        ClaimsPrincipal ValidateToken(string token);
        #endregion
    }


    public class RegistertionDTO
    {
        public string Token { get; set; }
        public string VerificationCode { get; set; }
        public string ClientPlatform { get; set; }
        public string ClientOS { get; set; }
        public string ClientAppVersion { get; set; }
        public string DeviceToken { get; set; }
    }
}