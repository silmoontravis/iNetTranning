using handlang.service.Models;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

namespace handlang.Model
{
    public class APIExceptionHandler : ExceptionHandler
    {
        public override void Handle(ExceptionHandlerContext context)
        {
            var msgs = new List<string>();
            var ex = context.Exception;
            while (ex != null)
            {
                msgs.Add(ex.Message);
                ex = ex.InnerException;
            }
            context.Result = new APIExceptionResponseActoinResult
            {
                Request = context.ExceptionContext.Request,
                Content = new APIResponse() { IsSuccess = false, Message = string.Join("，", msgs) }
            };
        }

        private class APIExceptionResponseActoinResult : IHttpActionResult
        {
            public HttpRequestMessage Request { get; set; }
            public APIResponse Content { get; set; }
            public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response.RequestMessage = Request;
                response.Content = new ObjectContent(Content.GetType(), Content, new JsonMediaTypeFormatter()
                {
                    SerializerSettings = { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                });

                return Task.FromResult(response);
            }
        }

    }
}