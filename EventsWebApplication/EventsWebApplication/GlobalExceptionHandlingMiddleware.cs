using System.ComponentModel.DataAnnotations;

using EventsWebApplication.Models;
using EventsWebApplication.Exceptions;


namespace EventsWebApplication
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex);
            }
        }

        private async Task HandleException(HttpContext httpContext, Exception ex)
        {
            if (httpContext.Response.HasStarted)
            {
                return;
            }

            var statusCode = MapStatusCode(ex);

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/json";

            var error = new ProblemDetails
            {
                Status = statusCode,
                Detail = ex.Message
            };

            await httpContext.Response.WriteAsJsonAsync(error);
        }

        private static int MapStatusCode(Exception ex)
            => ex switch
            {
                ValidationException ve => StatusCodes.Status400BadRequest,
                NotFoundException nf => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };
    }
}
