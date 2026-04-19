using Purchase_Orders.Application.Common.Exceptions;
using System.Net;

namespace PurchaseOrders.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
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

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = exception.Message;

            switch (exception)
            {
                case NotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;

                case DuplicateException:
                    statusCode = HttpStatusCode.Conflict;
                    break;

                case BusinessException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;

                case ValidationException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(message);
        }
    }
}
