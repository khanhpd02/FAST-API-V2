using Core.Exceptions;
using FAST_API_V2.ExceptionModel;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FAST_API_V2.Middlewares
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity!.IsAuthenticated)
            {
                throw new InvalidTokenException();
            }
            return;
        }
    }
}
