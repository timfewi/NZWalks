using System.Net;

namespace NZWalks.API.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next)
        {

            _logger = logger;
            _next = next;

        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex) // Catching all exceptions
            {
                var errorId = Guid.NewGuid();

                // Log this Exception
                _logger.LogError(ex, $"{errorId} : {ex.Message}");

                //Return a Custom Error Response to Client
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong. Please try again later or contact support."
                };

                await context.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
