using Serilog.Context;
namespace OnlineBookStore.Api.Shared.Middleware
{
    public class RequestIdMiddleware
    {
        private readonly RequestDelegate _next; // this field will hold the next middleware in the pipeline that we will call after processing the current request.

        public RequestIdMiddleware(RequestDelegate next) // we added a constructor that takes a RequestDelegate as a parameter and assigns it to the _next field. This allows us to call the next middleware in the pipeline after we have processed the current request.
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context) // we added an InvokeAsync method that takes an HttpContext as a parameter. This method will be called for each incoming HTTP request and will allow us to process the request and add a request ID to the response headers.
        {
            var requestId = context.Request.Headers["X-Request-ID"].FirstOrDefault(); // we attempt to retrieve the value of the "X-Request-ID" header from the incoming HTTP request. If the header is present, we take its first value; if it's not present, requestId will be null.
            if (string.IsNullOrEmpty(requestId)) // we check if the requestId is null or an empty string. If it is, that means the "X-Request-ID" header was not provided in the request, or it was provided but has no value.
            {
                requestId = Guid.NewGuid().ToString(); // if the "X-Request-ID" header is not present or is empty, we generate a new unique identifier (GUID) and convert it to a string to use as the request ID.
            }

            context.Response.Headers["X-Request-ID"] = requestId; // we add the request ID to the response headers, so that the client can see the request ID associated with their request in the response.

            using (LogContext.PushProperty("RequestId", requestId)) // we use Serilog's LogContext to push the request ID as a property that will be included in all log entries created during the processing of this request. This allows us to easily correlate log entries with specific requests by including the request ID in our log output.
            {
                await _next(context); // we call the next middleware in the pipeline, passing along the HttpContext. This allows the request to continue through the pipeline and eventually reach the appropriate controller action to handle it.
            }

        }
    }
}
