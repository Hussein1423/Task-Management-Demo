using Microsoft.Extensions.Options;
using Task_Management_API.Entities;

namespace Task_Management_API.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;


        public RequestLoggingMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine(
                $"\n Remaining tasks requests: {--_appSettings.MaxTasks}"+ 
                $"Request: {context.Request.Method} {context.Request.Path}" 

            );

            await _next(context);

            Console.WriteLine(
                $"Response: {context.Response.StatusCode}"
            );
        }
    }
}
