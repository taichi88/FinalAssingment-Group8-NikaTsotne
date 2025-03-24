using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Net;

namespace BankingSystem.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        private readonly Serilog.ILogger _logger = Log.ForContext<ValidateModelAttribute>();

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
                _logger.Warning("Validation error: {ErrorMessage}", errorMessage);
                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}