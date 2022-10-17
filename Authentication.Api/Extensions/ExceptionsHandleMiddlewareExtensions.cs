using Authentication.Api.Middlewares;

namespace Authentication.Api.Extensions;

public static class ExceptionsHandleMiddlewareExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandleMiddleware>();
    }
}