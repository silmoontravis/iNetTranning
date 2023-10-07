using handlang.service.Interface;
using handlang.service.Service;
using System;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace handlang.AuthFilter
{
    public class iNetAuthorize : AuthorizeAttribute
    {
        protected IAuth _authService;
        public iNetAuthorize()
        {
            _authService = new AuthService();
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext is null)
                throw new ArgumentNullException("actionContext");

            try
            {
                if (actionContext.Request.Headers.Authorization is null)
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
                if (IsTrustedSystem(actionContext))
                {
                    return;
                }
                if (!IsAuthenticated(actionContext))
                {
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
                return;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool IsTrustedSystem(HttpActionContext actionContext)
        {
            var token = actionContext.Request.Headers.Authorization.Parameter;
            var CIB_Web = ConfigurationManager.AppSettings["iNet_API_Token_In"];
            return token == CIB_Web;
        }

        private bool IsAuthenticated(HttpActionContext actionContext)
        {
            var token = actionContext.Request.Headers.Authorization.Parameter;
            //檢核JWT本身是否合法
            try
            {
                var principal = _authService.ValidateToken(token);
                actionContext.RequestContext.Principal = principal;
            }
            catch
            {
                return false;
            }

            JwtSecurityToken jwt = _authService.ReadToken(token);
            if (jwt is null)
            {
                return false;
            }
            //檢核JWT有效期限，如果還有效就直接通過
            if (jwt.ValidTo > DateTime.UtcNow)
            {
                return true;
            }

            //如果是匿名使用者，則不會產生新的Token
            if (_authService.IsAnonymous(jwt.Claims.ToList()))
            {
                return false;
            }

            var newToken = _authService.ReGenerateJwtToken(jwt);
            HttpContext.Current.Response.AddHeader("X-JWT", newToken);
            return true;
        }
    }
}