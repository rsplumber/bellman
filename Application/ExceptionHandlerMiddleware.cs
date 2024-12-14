using System.Text.Json;
using Core;
using FluentValidation;

namespace Application;

public sealed class ExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            string message;
            switch (exception)
            {
                case CoreException coreException:
                    response.StatusCode = coreException.Code is >= 400 and < 500 ? coreException.Code : 400;
                    message = coreException.Message;
                    break;
                case ApplicationException applicationException:
                    response.StatusCode = 400;
                    message = applicationException.Message;

                    break;
                case ValidationException validationException:
                    response.StatusCode = 422;

                    message = string.Join(", ", validationException.Errors
                        .Select(failure => $"{failure.PropertyName}:{failure.ErrorMessage}"));
                    break;
                default:
                    response.StatusCode = 500;
                    message = "Whoops :( , something impossibly went wrong!";
                    break;
            }

            await response.WriteAsync(JsonSerializer.Serialize(new
            {
                message,
            }));
        }
    }
}