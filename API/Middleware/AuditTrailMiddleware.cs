using System.Security.Claims;
using API.Constants;
using API.DTOs.AudiTrail;
using API.Models;
using API.Services;
using API.Extensions;
using API.Attributes;
using System.Text.Json;

namespace API.Middleware;

public class AuditTrailMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string[] _excludedPaths = ["/swagger", "/health", "/favicon.ico"];
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HashSet<string> _sensitiveFields;

    public AuditTrailMiddleware(RequestDelegate next, IConfiguration configuration, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _sensitiveFields = new HashSet<string>(
        configuration.GetSection("ErrorLogging:SensitiveFields").Get<string[]>() ?? [],
        StringComparer.OrdinalIgnoreCase);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_excludedPaths.Any(p => context.Request.Path.StartsWithSegments(p)))
        {
            await _next(context);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<AuditTrailService>();

        context.Request.EnableBuffering();
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        int? userId = null;
        try { userId = context.User?.GetUserId(); } catch { }

        object? parsedRequest = parseBody(requestBody);

        // ======== CALL CONTROLLER ========
        var originalResponseBody = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await _next(context); // controller executes here

        // ✅ read attribute AFTER _next
        var endpoint = context.GetEndpoint();
        var auditAttr = endpoint?.Metadata.GetMetadata<AuditTrailAttribute>();
        var module = auditAttr?.Module ?? "Unknown";
        var action = auditAttr?.Action ?? "Unknown";

        // ======== RESPONSE ========
        memoryStream.Position = 0;
        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
        memoryStream.Position = 0;
        await memoryStream.CopyToAsync(originalResponseBody);
        context.Response.Body = originalResponseBody;

        object? parsedResponse = parseBody(responseBody);

        // ======== REQUEST LOG ========
        await auditService.CreateAsync(new AuditTrailCreateDto
        {
            UserId = userId ?? 0,
            Module = module,
            Action = action,
            Path = context.Request.Path,
            Method = context.Request.Method,
            Data = parsedRequest != null ? JsonSerializer.Serialize(parsedRequest) : string.Empty,
            ClientIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            IsRequest = true,
            DateCreated = DateTime.UtcNow
        });

        var refId = context.Items[AuditTrailConstants.ReferenceId]?.ToString();

        // ======== RESPONSE LOG ========
        await auditService.CreateAsync(new AuditTrailCreateDto
        {
            UserId = userId ?? 0,
            Module = module,
            Action = action,
            Path = context.Request.Path,
            Method = context.Request.Method,
            Data = parsedResponse != null ? JsonSerializer.Serialize(parsedResponse) : string.Empty,
            ClientIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
            IsRequest = false,
            DateCreated = DateTime.UtcNow,
            RefId = refId?.ToString() ?? ""
        });
    }

    public object? parseBody(string requestBody)
    {
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

        return parsedBody;
    }
}

