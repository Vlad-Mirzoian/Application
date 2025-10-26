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
            var (statusCode, message) = ex switch
            {
                InvalidCredentialsException => (HttpStatusCode.Unauthorized, ex.Message),
                EmailAlreadyExistsException => (HttpStatusCode.Conflict, ex.Message),
                EventNotFoundException => (HttpStatusCode.NotFound, ex.Message),
                ForbiddenAccessException => (HttpStatusCode.Forbidden, ex.Message),
                AlreadyJoinedException => (HttpStatusCode.Conflict, ex.Message),
                EventFullException => (HttpStatusCode.Conflict, ex.Message),
                NotParticipantException => (HttpStatusCode.Forbidden, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Internal server error")
            };

            var result = JsonSerializer.Serialize(new { error = message });
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