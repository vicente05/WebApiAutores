using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiAutores.Middleware
{

    public static class LoguearRespuestaHttpMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHttp(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaMiddleware>();
        }
    }

    public class LoguearRespuestaMiddleware
    {
        private RequestDelegate next;
        private ILogger<LoguearRespuestaMiddleware> logger;
        public LoguearRespuestaMiddleware(RequestDelegate next, ILogger<LoguearRespuestaMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var ms = new MemoryStream())
            {
                var cuerpoOriginalRepuesta = context.Response.Body;
                context.Response.Body = ms;

                await next(context);

                ms.Seek(0, SeekOrigin.Begin);
                string repuesta = new StreamReader(ms).ReadToEnd();
                ms.Seek(0, SeekOrigin.Begin);

                await ms.CopyToAsync(cuerpoOriginalRepuesta);
                context.Response.Body = cuerpoOriginalRepuesta;

                logger.LogInformation(repuesta);

            }
        }


    }
}
