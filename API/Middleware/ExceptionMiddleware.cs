using System.Net;
using System.Text.Json;
using API.DTOs.ErrorLog;
using API.Extensions;
using API.Exceptions;
using API.Helpers;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Diagnostics;
using API.Errors;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _sensitiveFields;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, IConfiguration configuration, IServiceScopeFactory scopeFactory, IHostEnvironment env)
    {
        _next = next;
        _env = env;
        _scopeFactory = scopeFactory;
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

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var errorLogService = scope.ServiceProvider.GetRequiredService<ErrorLogService>();

            int? userId = null;
            try { userId = context.User?.GetUserId(); } catch { }

            await errorLogService.CreateAsync(new ErrorLogCreateDto
            {
                UserId = userId ?? 0,
                Description = JsonSerializer.Serialize(new
                {
                    ex.Message,
                    ex.Source,
                    Type = ex.GetType().Name,
                    InnerException = ex.InnerException?.Message ?? "N/A",
                    Request = data
                }),
                ClientIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "N/A",
                DateCreated = DateTime.UtcNow
            });
        }
        catch (Exception dbException)
        {
            await LogHelper.LogErrorAsync(ex, data);
            await LogHelper.LogErrorAsync(dbException);
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var reponse = _env.IsDevelopment()
              ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace)
              : new ApiException(context.Response.StatusCode, "Internal Server Error", "An error occured");

        var json = JsonSerializer.Serialize(reponse, options);

        await context.Response.WriteAsync(json);
    }
}