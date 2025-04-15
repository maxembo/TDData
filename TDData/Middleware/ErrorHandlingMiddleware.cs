using System.Net.Mime;
using Newtonsoft.Json;

namespace TDData.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unhandled exception occurred while processing the request.");
            await HandleException(context, ex);
        }
    }

    public static Task HandleException(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = MediaTypeNames.Application.Json;

        var errorResponse = new
        {
            message = exception.Message,
            description = exception.StackTrace,
        };

        var result = JsonConvert.SerializeObject(errorResponse);
        return response.WriteAsync(result);
    }
}