

using CarScrapper.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.ComponentModel.DataAnnotations;

namespace CarScrapper.Utils
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class ApiProtectedAttribute: ActionFilterAttribute
    {
        public static string ApiKey { private get; set; }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer", "").Trim();
            if (!token.Equals(ApiKey))
                context.Result = new UnauthorizedResult();
        }
    }
}
