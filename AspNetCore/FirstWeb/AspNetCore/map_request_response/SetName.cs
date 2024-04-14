namespace FirstWeb.AspNetCore.map_request_response
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class SetName
    {
        private readonly RequestDelegate _next;

        public SetName(RequestDelegate next)
        {
            _next = next;
        }


        public async Task Invoke(HttpContext httpContext)
        {

            httpContext.Items.Add("user", "sinh");
            httpContext.Items.Add("password", "123");

            await _next(httpContext);

        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SetNameExtensions
    {
        public static IApplicationBuilder UseSetName(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SetName>();
        }
    }
}
