using handlang.service.Domain;
using handlang.service.Interface;
using iNet.Service;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
//using System.Timers;
using System.Threading;
using iNet.Helper;

namespace handlang.service.Service
{
    public class AuthService : IAuth
    {
        public CacheEntryRemovedCallback OnCacheRemove;

        public string GenerateJwtToken(List<Claim> claims)
        {
            try
            {
                string key = IsAnonymous(claims) ? "TokenLifetime_Anonymous" : "TokenLifetime";
                if (!int.TryParse(ConfigurationManager.AppSettings[key], out int tokenLifetime))
                {
                    throw new Exception("TokenLifetime 設定錯誤！");
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Issuer = ConfigurationManager.AppSettings["ValidIssuer"],
                    Audience = ConfigurationManager.AppSettings["ValidAudience"],
                    NotBefore = DateTime.Now,
                    Expires = DateTime.Now.AddSeconds(tokenLifetime),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["IssuerSigningKey"])),
                                       SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);

                return token;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ReGenerateJwtToken(JwtSecurityToken jwt)
        {
            try
            {
                ObjectCache cache = MemoryCache.Default;
                string cacheKey = $"JWT_{jwt.Id}";
                string newToken = string.Empty;

                //30秒內重複用舊Token進行請求時，不再次產生新的Token
                if (cache[cacheKey] != null)
                {
                    newToken = (string)cache[cacheKey];
                }
                else
                {
                    newToken = GenerateJwtToken(jwt.Claims.ToList());

                    var policy = new CacheItemPolicy
                    {
                        RemovedCallback = OnCacheRemove,
                        SlidingExpiration = TimeSpan.FromSeconds(30)
                    };
                    cache.Add(cacheKey, newToken, policy);
                }

                return newToken;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsAnonymous(List<Claim> claims)
        {
            Claim claim = claims.FirstOrDefault(x => x.Type == "nameid" || x.Type == ClaimTypes.NameIdentifier);
            return Guid.Parse(claim.Value).Equals(Guid.Empty);
        }

        public JwtSecurityToken ReadToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(token);
        }


        public ClaimsPrincipal ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();

            string secretKey = ConfigurationManager.AppSettings["IssuerSigningKey"];
            byte[] secretBytes = Encoding.UTF8.GetBytes(secretKey);

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = false,
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidIssuer = ConfigurationManager.AppSettings["ValidIssuer"],
                ValidateAudience = true,
                ValidAudience = ConfigurationManager.AppSettings["ValidAudience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretBytes),
                ClockSkew = TimeSpan.Zero,
            };

            SecurityToken securityToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

            return principal;
        }

    }
}
