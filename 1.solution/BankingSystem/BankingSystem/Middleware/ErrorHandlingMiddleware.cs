using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text.Json;
using System.Net;

namespace BankingSystem.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IHostEnvironment _environment;
        private record ExceptionDetails(HttpStatusCode StatusCode, string Message, string ExceptionType);

        public ErrorHandlingMiddleware(
            RequestDelegate next,
            ILogger<ErrorHandlingMiddleware> logger,
            ProblemDetailsFactory problemDetailsFactory,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                ExceptionDetails exceptionDetails;
                if (_environment.IsDevelopment())
                {
                    _logger.LogError(ex, "Exception occurred: {ExceptionType} - {Message} - Stack: {StackTrace}",
                        ex.GetType().Name, ex.Message, ex.StackTrace);
                    exceptionDetails = MapException(ex);
                    _logger.LogError("{ExceptionType}: {StatusCode} - {UserFriendlyMessage}",
                        exceptionDetails.ExceptionType, (int)exceptionDetails.StatusCode, exceptionDetails.Message);
                }
                else
                {
                    exceptionDetails = MapException(ex);
                    _logger.LogError("{ExceptionType}: {StatusCode} - {UserFriendlyMessage}",
                        exceptionDetails.ExceptionType, (int)exceptionDetails.StatusCode, exceptionDetails.Message);
                }

                if (context.Response.HasStarted) throw;

                exceptionDetails = MapException(ex);
                context.Response.StatusCode = (int)exceptionDetails.StatusCode;
                await GenerateErrorResponse(context, ex, exceptionDetails);
            }
        }

        private async Task GenerateErrorResponse(HttpContext context, Exception exception, ExceptionDetails details)
        {
            var apiResponse = ApiResponse.CreateErrorResponse(details.StatusCode, details.Message);
            context.Response.ContentType = "application/json";

            await JsonSerializer.SerializeAsync(context.Response.Body, apiResponse,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        }

        private ExceptionDetails MapException(Exception exception)
        {
            return exception switch
            {
                ValidationException validationEx => new(HttpStatusCode.BadRequest, validationEx.Message, nameof(ValidationException)),
                NotFoundException notFoundEx => new(HttpStatusCode.NotFound, notFoundEx.Message, nameof(NotFoundException)),
                UnauthorizedException unauthorizedEx => new(HttpStatusCode.Unauthorized, unauthorizedEx.Message, nameof(UnauthorizedException)),
                ArgumentException argEx => new(HttpStatusCode.BadRequest, argEx.Message, nameof(ArgumentException)),
                _ => new(HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.", exception.GetType().Name)
            };
        }

        public static ApiResponse CreateSuccessResponse(HttpStatusCode statusCode, object? result = null)
        {
            return ApiResponse.CreateSuccessResponse(statusCode, result);
        }
    }
}
