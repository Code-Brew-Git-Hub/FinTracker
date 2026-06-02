namespace FinTracker.Domain.Dtos.Universal;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public bool Success { get; set; } = true;
    public string? Error { get; set; }

    public static ApiResponse<T> Ok(T data) => new() { Data = data };
    public static ApiResponse<T> Fail(string error) => new() { Success = false, Error = error };
}

public static class ApiResponse
{
    public static ApiResponse<T> Ok<T>(T data) => ApiResponse<T>.Ok(data);

    public static ApiResponse<T> Fail<T>(string error) => ApiResponse<T>.Fail(error);
}