using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace ContactManager.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IDiagnosticContext diagnosticContext)
        {
            _next = next; // Represents the subsequent middleware in the HTTP request pipeline
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(httpContext);

            }
            catch (Exception ex)
            {
                // You can also log the exception here using your preferred logging framework
                if (ex.InnerException != null)
                {
                    _logger.LogError("An exception occurred: {ExceptionType} {ExceptionMessage}",
                        ex.InnerException.GetType().ToString(), ex.InnerException.Message);
                }
                else
                {
                    _logger.LogError("An unhandled exception occurred: {ExceptionType} {ExceptionMessage}", ex.GetType().ToString(), ex.Message);
                }

                // Handle the exception (e.g., log it, return a custom error response, etc.)
                httpContext.Response.StatusCode = 500; // Internal Server Error
                await httpContext.Response.WriteAsync("An unexpected error occurred.");

                throw; // Re-throw the exception after handling
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
