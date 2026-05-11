using System.Net;
using System.Text.Json;

namespace OnlineBookStore.Api.Shared.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger; // this field will hold the instance of ILogger that we will use to log exceptions that occur during the processing of HTTP requests.

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) // we added an InvokeAsync method that takes an HttpContext as a parameter. This method will be called for each incoming HTTP request and will allow us to catch any unhandled exceptions that occur during the processing of the request and return a standardized error response to the client.
        {
            try
            {
                await _next(context); //
            }
            catch (Exception ex)
            {
       
                var requestId = context.Items["RequestId"]?.ToString()
                    ?? Guid.NewGuid().ToString(); // fallback if RequestIdMiddleware is missing

                _logger.LogError(ex , "Unhandled exception occurred. RequestId: {RequestId}", requestId);

                if (!context.Response.HasStarted)
                {
                    context.Response.Headers["X-Request-ID"] = requestId; // we add the request ID to the response headers, so that the client can see the request ID associated with their request in the response.
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        message = "something went wrong, contact support",
                        requestId = requestId
                    };

                    var json = JsonSerializer.Serialize(response);

                    await context.Response.WriteAsync(json);

                }
            }

        }
    }
}