using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CardAuthorizationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Check if the card is authorized by retrieving the session value
        var authorizedCard = context.HttpContext.Session.GetString("AuthorizedCard");

        if (string.IsNullOrEmpty(authorizedCard))
        {
            // Return Unauthorized if there is no authorized card
            context.Result = new UnauthorizedResult();
            return;
        }

        // Proceed with the action execution if the card is authorized
        await next();
    }
}