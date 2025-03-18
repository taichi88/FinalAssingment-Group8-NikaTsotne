using BankingSystem.Application.DTO.Response;
using BankingSystem.Application.Exceptions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Json;
using System.Net;

namespace BankingSystem.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger,
            IHostEnvironment env, ProblemDetailsFactory problemDetailsFactory)
        {
            _next = next;
            _logger = logger;
            _env = env;
            _problemDetailsFactory = problemDetailsFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware
                await _next(context);

                // Log successful responses (2xx status codes)
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    _logger.LogInformation(
                        "Request {Method} {Path} completed successfully with status code {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode);
                }
                // Optionally log other non-error responses (3xx status codes)
                else if (context.Response.StatusCode >= 300 && context.Response.StatusCode < 400)
                {
                    _logger.LogInformation(
                        "Request {Method} {Path} redirected with status code {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode);
                }
                // Log client errors (4xx status codes) that aren't being caught as exceptions
                else if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
                {
                    _logger.LogWarning(
                        "Request {Method} {Path} failed with client error status code {StatusCode}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // Developer-friendly logging with detailed information
                _logger.LogError(ex, "Unhandled exception occurred in {Method} {Path}: {Message}",
                    context.Request.Method, context.Request.Path, ex.Message);

                if (context.Response.HasStarted) throw;

                // Determine HTTP status code from exception type
                var statusCode = MapExceptionToStatusCode(ex);
                context.Response.StatusCode = (int)statusCode;

                // Generate response based on accept header or default to ApiResponse
                await GenerateErrorResponse(context, ex, statusCode);
            }
        }

        private async Task GenerateErrorResponse(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            // Check if the caller is specifically asking for ProblemDetails format
            bool isProblemDetailsRequested = context.Request.Headers.Accept.Any(h =>
                h.Contains("application/problem+json"));

            // If it's a development environment or ProblemDetails is requested, return detailed ProblemDetails
            if (_env.IsDevelopment() && isProblemDetailsRequested)
            {
                var problemDetails = CreateProblemDetails(context, exception, (int)statusCode);
                context.Response.ContentType = "application/problem+json";
                await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
            // Otherwise return user-friendly ApiResponse format
            else
            {
                var apiResponse = CreateApiResponse(exception, statusCode);
                context.Response.ContentType = "application/json";
                await JsonSerializer.SerializeAsync(context.Response.Body, apiResponse,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        private ProblemDetails CreateProblemDetails(HttpContext context, Exception exception, int statusCode)
        {
            return _problemDetailsFactory.CreateProblemDetails(
                context, statusCode, ReasonPhrases.GetReasonPhrase(statusCode),
                detail: exception.ToString(), // Always include full details for developers
                instance: context.Request.Path
            );
        }

        private ApiResponse CreateApiResponse(Exception exception, HttpStatusCode statusCode)
        {
            string errorMessage = GetUserFriendlyMessage(exception);

            return ApiResponse.CreateErrorResponse(statusCode, errorMessage);
        }

        private string GetUserFriendlyMessage(Exception exception)
        {
            // Return appropriate user-friendly message based on exception type
            return exception switch
            {
                ValidationException validationEx => validationEx.Message,
                NotFoundException notFoundEx => notFoundEx.Message,
                UnauthorizedException unauthorizedEx => unauthorizedEx.Message,
                _ => "An unexpected error occurred. Please try again later."
            };
        }

        private static HttpStatusCode MapExceptionToStatusCode(Exception exception)
        {
            return exception switch
            {
                NotFoundException => HttpStatusCode.NotFound,
                UnauthorizedException => HttpStatusCode.Unauthorized,
                ValidationException => HttpStatusCode.BadRequest,
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };
        }

        // Helper method for controllers to create success responses
        public static ApiResponse CreateSuccessResponse(HttpStatusCode statusCode, object? result = null)
        {
            return ApiResponse.CreateSuccessResponse(statusCode, result);
        }
    }
}