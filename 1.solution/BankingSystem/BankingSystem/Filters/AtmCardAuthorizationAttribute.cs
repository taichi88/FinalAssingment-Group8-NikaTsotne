using BankingSystem.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankingSystem.Filters;

public class AtmCardAuthorizationAttribute() : TypeFilterAttribute(typeof(AtmCardAuthorizationFilter))
{
    private class AtmCardAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private const string AtmSessionClaimType = "atmSession";
        private const string CardNumberClaimType = "cardNumber";

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!IsAuthorized(context))
            {
                throw new UnauthorizedException("Bank Card is not Authorized");
            }

            await Task.CompletedTask;
        }

        private static bool IsAuthorized(AuthorizationFilterContext context)
        {
            var atmSessionClaim = context.HttpContext.User.FindFirst(AtmSessionClaimType);
            var cardNumberClaim = context.HttpContext.User.FindFirst(CardNumberClaimType);

            return atmSessionClaim != null &&
                   cardNumberClaim != null &&
                   atmSessionClaim.Value == "true";
        }
    }
}