using System.Net;
using System.Text.Json;
using API.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await HandleNotFoundExceptionAsync(context, ex);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (UnauthorizedException ex)
        {
            await HandleUnauthorizedExceptionAsync(context, ex);
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

    private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException ex) =>
        WriteResponseAsync(context, HttpStatusCode.NotFound, new
        {
            Success = false,
            Error = ex.Message
        });

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex) =>
        WriteResponseAsync(context, HttpStatusCode.BadRequest, new
        {
            Success = false,
            Error = ex.Message,
            ex.Errors
        });

    private static Task HandleUnauthorizedExceptionAsync(HttpContext context, UnauthorizedException ex) =>
        WriteResponseAsync(context, HttpStatusCode.Unauthorized, new
        {
            Success = false,
            Error = ex.Message
        });

    private static Task HandleExceptionAsync(HttpContext context, Exception ex) =>
        WriteResponseAsync(context, HttpStatusCode.InternalServerError, new
        {
            Success = false,
            Error = "An unexpected error occurred."
        });
}