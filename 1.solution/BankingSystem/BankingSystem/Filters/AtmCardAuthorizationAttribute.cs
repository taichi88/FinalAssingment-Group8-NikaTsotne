// BankingSystem/Filters/AtmCardAuthorizationAttribute.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankingSystem.Filters;

public class AtmCardAuthorizationAttribute : TypeFilterAttribute
{
    public AtmCardAuthorizationAttribute() : base(typeof(AtmCardAuthorizationFilter))
    {
    }

    private class AtmCardAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var atmSessionClaim = context.HttpContext.User.FindFirst("atmSession");
            var cardNumberClaim = context.HttpContext.User.FindFirst("cardNumber");

            if (atmSessionClaim == null || cardNumberClaim == null || atmSessionClaim.Value != "true")
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}