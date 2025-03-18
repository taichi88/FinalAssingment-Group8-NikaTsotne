using BankingSystem.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BankingSystem.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
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

                throw new ValidationException(errorMessage);
            }
        }
    }
}