using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace PointOfSale.Middleware
{
    public class RequestLogging
    {
        private readonly RequestDelegate _next;

        public RequestLogging(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, ILogger<RequestLogging> logger)
        {
            string preLogMessage = "Entering request";
            string postLogMessage = "Exiting request";
            if (httpContext?.Request != null)
            {
                preLogMessage = $"Entering request - Path: [{httpContext.Request.Path}], Method: [{httpContext.Request.Method}], Query String: [{httpContext.Request.QueryString}]";
                postLogMessage = $"Exiting request - Path: [{httpContext.Request.Path}], Method: [{httpContext.Request.Method}], Query String: [{httpContext.Request.QueryString}]";
            }

            logger.LogTrace(preLogMessage);
            await _next(httpContext);
            logger.LogTrace(postLogMessage);
        }
    }

    public static class RequestLoggingExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLogging>();
        }
    }
}
