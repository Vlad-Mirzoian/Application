using System.Net;
using System.Text.Json;

namespace EventApi.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                InvalidCredentialsException => HttpStatusCode.Unauthorized,
                EmailAlreadyExistsException => HttpStatusCode.Conflict,
                EventNotFoundException => HttpStatusCode.NotFound,
                ForbiddenAccessException => HttpStatusCode.Forbidden,
                AlreadyJoinedException => HttpStatusCode.Conflict,
                EventFullException => HttpStatusCode.Conflict,
                NotParticipantException => HttpStatusCode.Forbidden,
                _ => HttpStatusCode.BadRequest
            };

            var result = JsonSerializer.Serialize(new { error = ex.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }

    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message) : base(message) { }
    }

    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException(string message) : base(message) { }
    }
    public class EventNotFoundException : Exception
    {
        public EventNotFoundException(string message) : base(message) { }
    }

    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message) : base(message) { }
    }

    public class AlreadyJoinedException : Exception
    {
        public AlreadyJoinedException(string message) : base(message) { }
    }

    public class EventFullException : Exception
    {
        public EventFullException(string message) : base(message) { }
    }

    public class NotParticipantException : Exception
    {
        public NotParticipantException(string message) : base(message) { }
    }

}
