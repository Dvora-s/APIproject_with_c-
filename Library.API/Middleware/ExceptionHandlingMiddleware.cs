using System.Net;
using System.Text.Json;
using Library.BusinessLogic.Exceptions;

namespace Library.API.Middleware
{
    /// <summary>
    /// Middleware מרכזי לטיפול בשגיאות - תופס כל חריגה לא מטופלת, רושם אותה ל-Log,
    /// ומחזיר ללקוח תשובת JSON אחידה עם קוד הסטטוס המתאים, כדי שהמערכת לא תקרוס.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                NotFoundException => (HttpStatusCode.NotFound, exception.Message),
                BusinessRuleException => (HttpStatusCode.BadRequest, exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "אירעה שגיאה בלתי צפויה בשרת. נסה שוב מאוחר יותר.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "שגיאה לא צפויה בעת טיפול בבקשה {Path}", context.Request.Path);
            }
            else
            {
                _logger.LogWarning("שגיאה מטופלת: {Message} ({StatusCode})", message, (int)statusCode);
            }

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                status = (int)statusCode,
                error = statusCode.ToString(),
                message,
                path = context.Request.Path.Value
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>Extension נוח לרישום ה-Middleware ב-Program.cs.</summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
