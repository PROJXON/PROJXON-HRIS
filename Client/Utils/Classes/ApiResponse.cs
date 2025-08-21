using System.Collections.Generic;

namespace Client.Utils.Classes;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();

    public static ApiResponse<T> Success(T data, Dictionary<string, string> headers, int statusCode = 200) => new()
    {
        IsSuccess = true,
        Data = data,
        StatusCode = statusCode,
        Headers = headers
    };

    public static ApiResponse<T> Failure(string error, Dictionary<string, string> headers, int statusCode) => new()
    {
        IsSuccess = false,
        ErrorMessage = error,
        StatusCode = statusCode,
        Headers = headers
    };
}