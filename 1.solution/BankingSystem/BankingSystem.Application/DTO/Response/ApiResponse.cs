using System.Net;

namespace BankingSystem.Application.DTO.Response;

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;
    public List<string> ErrorMessages { get; set; } = new();
    public object Result { get; set; }

    public static ApiResponse CreateErrorResponse(HttpStatusCode statusCode, string errorMessage)
    {
        return new ApiResponse
        {
            StatusCode = statusCode,
            IsSuccess = false,
            ErrorMessages = new List<string> { errorMessage }
        };
    }
}