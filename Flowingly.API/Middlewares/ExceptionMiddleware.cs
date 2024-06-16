using Flowingly.Domain.Models;
using System.Net;
using System.Text.Json;

namespace Flowingly.API.Middlewares;

/// <summary>
/// Middleware for handling exceptions and returning appropriate error responses.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger for logging exceptions.</param>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    /// Handles the exception and returns an appropriate error response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception to handle.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "An unexpected error occurred.";
        var details = exception.Message;

        switch (exception)
        {
            case UnauthorizedAccessException:
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "Unauthorized access.";
                break;
            case ArgumentNullException:
                statusCode = (int)HttpStatusCode.BadRequest;
                message = "Invalid Arguments.";
                break;
            case FormatException:
                statusCode = (int)HttpStatusCode.NotAcceptable;
                message = "Invalid XML File format.";
                break;
        }

        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Details = details
        };

        var result = JsonSerializer.Serialize(errorResponse);
        response.StatusCode = statusCode;
        return response.WriteAsync(result);
    }
}
