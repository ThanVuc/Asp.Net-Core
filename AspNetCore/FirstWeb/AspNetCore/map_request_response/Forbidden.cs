namespace FirstWeb.AspNetCore.map_request_response
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class Forbidden
    {
        private readonly RequestDelegate _next;

        public Forbidden(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string url = httpContext.Request.Path;
            if (url.Contains("xxx"))
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync(httpContext.Response.StatusCode.ToString());
                return;
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ForbiddenExtensions
    {
        public static IApplicationBuilder UseForbidden(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Forbidden>();
        }
    }
}
