namespace Client.Utils.Classes;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T Value { get; private set; } = default!;
    public string ErrorMessage { get; private set; } = string.Empty;
    
    private Result() {}

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public static Result<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    
    private Result() {}

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}