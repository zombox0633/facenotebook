using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using utils.apiFormatResponse;


public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred. Request: {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = GetErrorResponse(exception);
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiFormatErrorResponse((int)statusCode, message, context.Request.Path);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var result = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(result);
    }

    private (HttpStatusCode statusCode, string message) GetErrorResponse(Exception exception)
    {
        return exception switch
        {
            // Validation and Input Errors
            ArgumentException or ArgumentNullException =>
                (HttpStatusCode.BadRequest, "Invalid input provided."),

            // Conflict Errors
            InvalidOperationException when exception.Message.Contains("duplicate") ||
                exception.Message.Contains("already exists") =>
                (HttpStatusCode.Conflict, "Resource already exists."),

            // Invalid Operation
            InvalidOperationException =>
                (HttpStatusCode.BadRequest, exception.Message),

            // Database Related Errors
            DbUpdateException dbEx => HandleDbUpdateException(dbEx),

            // Authentication and Authorization
            UnauthorizedAccessException =>
                (HttpStatusCode.Unauthorized, exception.Message),

            // Access/Permission Errors
            System.Security.SecurityException =>
                (HttpStatusCode.Forbidden, "Access denied."),

            // Not Found Errors
            KeyNotFoundException =>
                (HttpStatusCode.NotFound, exception.Message),

            FileNotFoundException =>
                (HttpStatusCode.NotFound, "Requested resource not found."),

            // Timeout Errors
            TimeoutException =>
                (HttpStatusCode.RequestTimeout, "Request timeout."),

            // Default
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };
    }

    private (HttpStatusCode statusCode, string message) HandleDbUpdateException(DbUpdateException dbEx)
    {
        var innerException = dbEx.InnerException?.Message?.ToLower();

        if (innerException?.Contains("unique") == true || innerException?.Contains("duplicate") == true)
        {
            return (HttpStatusCode.Conflict, "A record with this information already exists.");
        }

        if (innerException?.Contains("foreign key") == true)
        {
            return (HttpStatusCode.BadRequest, "Referenced record does not exist.");
        }

        return (HttpStatusCode.BadRequest, "Database operation failed. Please check your data.");
    }
}
