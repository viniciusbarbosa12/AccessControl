using System.Diagnostics;

namespace AccessControl.Middlewares
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimingMiddleware> _logger;

        public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);

            stopwatch.Stop();
            var duration = stopwatch.ElapsedMilliseconds;

            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Request-Duration"] = $"{duration}ms";
            }

            _logger.LogInformation(
                "[RequestTiming] {Method} {Path} responded in {Duration} ms",
                context.Request.Method,
                context.Request.Path,
                duration
            );
        }
    }

    public static class RequestTimingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestTimingMiddleware>();
        }
    }
}
