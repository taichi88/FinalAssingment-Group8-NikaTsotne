using System.Net;

namespace BankingSystem.Application.DTO.Response
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = [];
        public object? Result { get; set; }

        public static ApiResponse CreateErrorResponse(HttpStatusCode statusCode, string errorMessage) => new()
        {
            StatusCode = statusCode,
            IsSuccess = false,
            ErrorMessages = [errorMessage]
        };

        public static ApiResponse CreateSuccessResponse(HttpStatusCode statusCode, object? result = null) => new()
        {
            StatusCode = statusCode,
            IsSuccess = true,
            Result = result
        };
    }
}