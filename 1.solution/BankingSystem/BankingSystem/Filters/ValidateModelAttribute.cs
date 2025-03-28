using BankingSystem.Application.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

using System.Net;

namespace BankingSystem.Filters
{
    public class ValidateModelAttribute(ILogger<ValidateModelAttribute> logger) : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .SelectMany(e => e.Value!.Errors.Select(er =>
                        $"{e.Key}: {er.ErrorMessage}"));

                // Combine all validation errors into a single message
                var errorMessage = string.Join("; ", errors);
                var response = ApiResponse.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
                logger.LogWarning("Validation error: {ErrorMessage}", errorMessage);
                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}