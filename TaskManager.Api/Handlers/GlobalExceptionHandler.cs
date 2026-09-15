using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "Unhandled exception occurred.");

        if (exception is DbUpdateException)
        {
            httpContext.Response.StatusCode =
                StatusCodes.Status409Conflict;

            httpContext.Response.ContentType =
                "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "Conflict",
                    Detail = "The operation conflicts with the current state of the data."
                },
                cancellationToken);

            return true;
        }

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        httpContext.Response.ContentType =
            "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Detail = "An unexpected error occurred."
            },
            cancellationToken);

        return true;
    }
}