using CloudSync.Exceptions.Business;
using CloudSync.Exceptions.Infrastructure;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CloudSync.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Unhandled exception");
            await HandleExceptionAsync(context, e);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception e)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = e switch
        {
            AuthenticationException ex => ex.StatusCode, 
            AuthorizationException ex => ex.StatusCode, 
            BusinessRuleException ex => ex.StatusCode, 
            DuplicateEntityException ex => ex.StatusCode, 
            EntityNotFoundException ex => ex.StatusCode, 
            ValidationException ex => ex.StatusCode, 
            ConfigurationException ex => ex.StatusCode, 
            DataAccessException ex => ex.StatusCode, 
            ExternalServiceException ex => ex.StatusCode, 
            _ => StatusCodes.Status500InternalServerError
        };

        response.StatusCode = statusCode;

        var result = JsonSerializer.Serialize(new
        {
            error = e.Message,
            status = statusCode
        });

        return response.WriteAsync(result);
    }
}