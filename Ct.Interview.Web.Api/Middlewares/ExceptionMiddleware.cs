using System.Text.Json;
using Ct.Domain.Exceptions;

namespace Ct.Interview.Web.Api.Middlewares
{
    internal sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionMiddleware>>();
                logger.LogError(ex, "Request Error");

                try
                {
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = null
                    };

                    var json = JsonSerializer.Serialize(new AsxExceptionDto(ex), jsonSerializerOptions);

                    context.Response.StatusCode = GetStatusCode(ex);
                    context.Response.ContentType = "application/json";
#if DEBUG
                    await context.Response.WriteAsync(json);
#else
                    await context.Response.WriteAsync(GetFirstError(e));
#endif
                    await context.Response.CompleteAsync();
                    return;
                }
                catch (Exception ex2)
                {
                    logger.LogError(ex2, "Error when serializing the exception to the response.");
                }
                throw;
            }
        }

        private static string GetFirstError(Exception ex)
        {
            if (ex is AggregateException ae)
            {
                return GetFirstError(ae.InnerExceptions[0]);
            }
            return ex.Message;
        }

        private static int GetStatusCode(Exception ex)
        {
            if (ex is RecordNotFoundException)
                return StatusCodes.Status404NotFound;

            if (ex is ServiceUnavailableException)
                return StatusCodes.Status503ServiceUnavailable;

            return StatusCodes.Status500InternalServerError;
        }
    }
}
