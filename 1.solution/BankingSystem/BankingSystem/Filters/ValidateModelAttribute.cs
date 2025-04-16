using BankingSystem.Application.DTO.Response;
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
                var errorMessage = string.Join("; ", context.ModelState
                    .Where(m => m.Value?.Errors.Count > 0)
                    .Select(m => $"{m.Key}: {m.Value!.Errors.First().ErrorMessage}"));

                var response = ApiResponse.CreateErrorResponse(HttpStatusCode.BadRequest, errorMessage);
                _logger.Warning("Validation error: {ErrorMessage}", errorMessage);
                context.Result = new BadRequestObjectResult(response);
            }
        }
    }
}