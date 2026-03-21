using System.Net;
using System.Text.Json;
using API.Exceptions;
using API.Helpers;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _sensitiveFields;

    public ExceptionMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _sensitiveFields = new HashSet<string>(
          configuration.GetSection("ErrorLogging:SensitiveFields").Get<string[]>() ?? [],
          StringComparer.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Request.Body.Position = 0;
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

        object? parsedBody = null;
        if (!string.IsNullOrEmpty(requestBody))
        {
            try
            {
                var rawJson = JsonSerializer.Deserialize<Dictionary<string, object>>(requestBody);
                if (rawJson != null)
                {
                    foreach (var key in rawJson.Keys.ToList())
                    {
                        if (_sensitiveFields.Contains(key))
                            rawJson[key] = "***REDACTED***";
                    }
                    parsedBody = rawJson;
                }
            }
            catch
            {
                parsedBody = requestBody;
            }
        }

        var data = new
        {
            Method = context.Request.Method,
            Path = context.Request.Path.ToString(),
            QueryString = context.Request.QueryString.ToString(),
            Body = parsedBody
        };

        await LogHelper.LogErrorAsync(ex, data);
        await WriteResponseAsync(context, HttpStatusCode.InternalServerError, new
        {
            Success = false,
            Error = "An unexpected error occurred."
        });
    }
}