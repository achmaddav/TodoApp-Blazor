using System.Net;
using System.Text.Json;
using TodoApp.Domain.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace TodoApp.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next,
            ILogger<ExceptionMiddleware> logger)
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
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message, errors) = ex switch
            {
                NotFoundException nfe =>
                    (HttpStatusCode.NotFound, nfe.Message, (List<string>?)null),

                ValidationException ve =>
                    (HttpStatusCode.BadRequest, "Validation failed.",
                     ve.Errors.Select(e => e.ErrorMessage).ToList()),

                UnauthorizedAccessException =>
                    (HttpStatusCode.Unauthorized, "Unauthorized.", null),

                InvalidOperationException ioe =>
                    (HttpStatusCode.BadRequest, ioe.Message, null),

                _ => (HttpStatusCode.InternalServerError,
                      "An unexpected error occurred.", null)
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                isSuccess = false,
                statusCode = (int)statusCode,
                message,
                errors = errors ?? new List<string> { message }
            };

            var options = new JsonSerializerOptions
            { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, options));
        }
    }
}
