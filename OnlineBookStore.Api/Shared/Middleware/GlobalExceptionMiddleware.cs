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
               // var requestId = context.Response.Headers["X-Request-ID"].FirstOrDefault(); // we attempt to retrieve the request  ID from the response headers. This allows us to include the request ID in our log entry for better traceability.

                var requestId = Guid.NewGuid().ToString();
                context.Response.Headers["X-Request-ID"] = requestId; // we add the request ID to the response headers, so that the client can see the request ID associated with their request in the response.

                _logger.LogError(ex, "Unhandled exception occurred. RequestId: {RequestId}", requestId);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 
                context.Response.ContentType = "application/json";

                var response = new  // we create an anonymous object to represent the error response that we will send back to the client. This object contains a message property with a generic error message and a requestId property with the request ID for traceability.
                {
                    message = "Something went wrong.",
                   // requestId = requestId   
                };

                var json = JsonSerializer.Serialize(response); 

                await context.Response.WriteAsync(json);
            }

        }
    }
}