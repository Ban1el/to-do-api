using System;

namespace API.Common;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    //Why service result over throwing an exception?
    //Only use exception when the error is not expected
    //Since validation is expected by the user, we should return a bad request instead
    public static ServiceResult<T> Ok(T data)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data
        };
    }

    public static ServiceResult<T> Ok(string message) =>
    new() { Success = true, Message = message };

    public static ServiceResult<T> Fail(string message)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Message = message
        };
    }
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }

    public static ServiceResult Ok() =>
        new() { Success = true };

    public static ServiceResult Fail(string message) =>
        new() { Success = false, Message = message };
}