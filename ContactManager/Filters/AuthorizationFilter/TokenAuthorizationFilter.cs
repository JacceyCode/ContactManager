using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.AuthorizationFilter
{
    public class TokenAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.Request.Cookies.ContainsKey("Auth-key") == false) 
            { 
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized); // 401 - Unauthorized

                return;
            }

            if (context.HttpContext.Request.Cookies["Auth-key"] != "A100")
            {
                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized); // 401 - Unauthorized
            }

        }
    }
}
