using System.Text.Json;
using Serilog;

namespace API.Helpers;

public static class LogHelper
{
    public static async Task LogErrorAsync(Exception exception, object? data = null)
    {
        var dataSection = data != null
            ? JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true })
            : "N/A";

        Log.Error(exception,
            """
        Message: {Message}
        Source: {Source}
        Type: {Type}
        Data: {Data}
        InnerException: {InnerException}

        """,
            exception.Message,
            exception.Source,
            exception.GetType().Name,
            dataSection,
            exception.InnerException?.Message ?? "N/A");

        await Task.CompletedTask;
    }
}